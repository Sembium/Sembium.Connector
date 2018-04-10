using Autofac;
using Sembium.Connector.Data;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using Sembium.Connector.Oracle;
using Sembium.Connector.Services;

namespace Sembium.Connector
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<OracleSqlDataConnection>().As<ISqlDataConnection>();
            builder.RegisterType<DataConnection>().As<IDataConnection>().InstancePerLifetimeScope();
            builder.RegisterType<Authorization>().As<IAuthorization>().InstancePerLifetimeScope();
            builder.RegisterType<IpAddressUtils>().As<IIpAddressUtils>();

            builder.RegisterType<ProductsService>().As<IProductsService>();
            builder.RegisterType<EmployeesService>().As<IEmployeesService>();
            builder.RegisterType<ProductionService>().As<IProductionService>();
        }
    }
}
