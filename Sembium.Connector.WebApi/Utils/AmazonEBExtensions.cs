using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.WebApi.Utils
{
    public static class AmazonEBExtensions
    {
        public static IConfigurationBuilder AddAmazonElasticBeanstalk(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new AmazonEBConfigurationSource());
            return configurationBuilder;
        }
    }

    public class AmazonEBConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AmazonEBConfigurationProvider();
        }
    }

    public class AmazonEBConfigurationProvider : ConfigurationProvider
    {
        private const string ConfigurationFilename = @"C:\Program Files\Amazon\ElasticBeanstalk\config\containerconfiguration";

        public override void Load()
        {
            foreach (var e in GetAmazonEBEnvironmentVariables())
            {
                Data[e.Key] = e.Value;
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetAmazonEBEnvironmentVariables()
        {
            if (File.Exists(ConfigurationFilename))
            {
                string configJson;
                try
                {
                    configJson = File.ReadAllText(ConfigurationFilename);
                }
                catch
                {
                    configJson = null;
                }

                if (!string.IsNullOrEmpty(configJson))
                {
                    var config = JObject.Parse(configJson);
                    var env = (JArray)config["iis"]["env"];

                    if (env.Count > 0)
                    {
                        foreach (var item in env.Select(i => (string)i))
                        {
                            var eqIndex = item.IndexOf('=');
                            var key = item.Substring(0, eqIndex);
                            var value = item.Substring(eqIndex + 1);

                            yield return new KeyValuePair<string, string>(key, value);
                        }
                    }
                }
            }
        }
    }
}
