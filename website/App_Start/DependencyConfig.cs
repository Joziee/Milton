using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Milton.Database.Infrastructure;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Milton.Website
{
    public class DependencyConfig : IDependencyRegistrar
    {
        public void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacWebTypesModule>();

            //Change controller action parameter injection by changing web.config.
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>().InstancePerLifetimeScope();

            //Register the data context
            builder.Register<Milton.Database.IDataContext>(c => new Milton.Database.DatabaseContext(new Milton.Database.DefaultDatabaseInitializer())).InstancePerLifetimeScope();

            //Register generics
            builder.RegisterGeneric(typeof(Milton.Database.DataRepository<>)).As(typeof(Milton.Database.IDataRepository<>)).InstancePerLifetimeScope();

            //Register the Cache
            builder.RegisterType<Milton.Services.Cache.MemoryCacheManager>().As<Milton.Services.Cache.ICacheManager>().InstancePerLifetimeScope();

            //Register services
            builder.RegisterType<Milton.Services.Business.PersonService>().As<Milton.Services.Business.IPersonService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Configuration.SettingsService>().As<Milton.Services.Configuration.ISettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Security.SecurityService>().As<Milton.Services.Security.ISecurityService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.PatientService>().As<Milton.Services.Medical.IPatientService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.RegionService>().As<Milton.Services.Business.IRegionService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.HospitalService>().As<Milton.Services.Medical.IHospitalService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.HealthShareReconService>().As<Milton.Services.Medical.IHealthShareReconService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Finance.PaymentService>().As<Milton.Services.Finance.IPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.ReconService>().As<Milton.Services.Medical.IReconService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.AccountService>().As<Milton.Services.Business.IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.BorderTripService>().As<Milton.Services.Business.IBorderTripService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.HospitalTripService>().As<Milton.Services.Business.IHospitalTripService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.OtherExpenseService>().As<Milton.Services.Business.IOtherExpenseService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.AdmitDischargeService>().As<Milton.Services.Medical.IAdmitDischargeService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.BatchService>().As<Milton.Services.Business.IBatchService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.AdmitService>().As<Milton.Services.Medical.IAdmitService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Medical.DischargeService>().As<Milton.Services.Medical.IDischargeService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Reporting.ReportService>().As<Milton.Services.Reporting.IReportService>().InstancePerLifetimeScope();
            builder.RegisterType<Milton.Services.Business.DailyReportLogService>().As<Milton.Services.Business.IDailyReportLogService>().InstancePerLifetimeScope();

            /************************* Asp.Net MVC ****************************/
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();

            builder.RegisterControllers(assemblies);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();
        }
    }
}
