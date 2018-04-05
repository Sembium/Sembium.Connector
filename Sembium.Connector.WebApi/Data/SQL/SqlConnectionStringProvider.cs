using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace Sembium.Connector.Data.Sql
{
    public class SqlConnectionStringProvider : ISqlConnectionStringProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _amazonS3;

        public SqlConnectionStringProvider(IConfiguration configuration, IAmazonS3 amazonS3)
        {
            _configuration = configuration;
            _amazonS3 = amazonS3;
        }

        private string GetAppSetting(string settingName)
        {
            return _configuration.GetSection("AppSettings").GetValue<string>(settingName);
        }

        private string GetConfigFileText(string fileName)
        {
            using (var response = _amazonS3.GetObject(GetAppSetting("S3ConfigBucketName"), fileName))
            {
                using (var reader = new StreamReader(response.ResponseStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void CheckIfDatabaseAllowed(string dbName)
        {
            var json = GetConfigFileText(GetAppSetting("ConnectorSettingsFileName"));
            var jsonObject = JObject.Parse(json);

            if (!jsonObject["allowedDatabases"].Any(x => string.Equals((string)x, dbName, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new AuthorizeException("Connector access to database not allowed");
            }
        }

        public string GetConnectionString(string dbName)
        {
            CheckIfDatabaseAllowed(dbName);

            var json = GetConfigFileText(GetAppSetting("DatabaseSettingsFileName"));
            var jsonObject = JObject.Parse(json);
            return (string)jsonObject["databaseSettings"].First(x => string.Equals((string)x["dbName"], dbName, StringComparison.InvariantCultureIgnoreCase))["connectionString"];
        }
    }
}
