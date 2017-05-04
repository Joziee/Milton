using Milton.Services.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.Net.Mail;
using System.IO;
using Milton.Services.Business;
using Milton.Database.Models.Business;

namespace DailyReportSender
{
    class Program
    {
        #region Fields

        protected IReportService _reportService;
        protected IDailyReportLogService _dailyReportLogService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Program()
        {
            #region Configure Autofac

            var builder = new Autofac.ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterGeneric(typeof(Milton.Database.DataRepository<>)).As(typeof(Milton.Database.IDataRepository<>));

            builder.Register<Milton.Database.IDataContext>(c => new Milton.Database.DatabaseContext(new Milton.Database.DefaultDatabaseInitializer())).InstancePerLifetimeScope();

            builder.RegisterType<Milton.Services.Cache.MemoryCacheManager>().As<Milton.Services.Cache.ICacheManager>().InstancePerLifetimeScope();

            builder.RegisterType<ReportService>().As<IReportService>();
            builder.RegisterType<DailyReportLogService>().As<IDailyReportLogService>();

            var container = builder.Build();

            #endregion

            #region Services

            _reportService = container.Resolve<IReportService>();
            _dailyReportLogService = container.Resolve<IDailyReportLogService>();

            #endregion

        }

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //start the reporter
            try
            {
                Program prog = new Program();
                prog.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong! : " + e.Message);
            }

        }

        /// <summary>
        /// Main program
        /// </summary>
        public void Run()
        {
            //generate report
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var botswanaReport = _reportService.GenerateDailyPatients(today, 1, true);
            var swazilandReport = _reportService.GenerateDailyPatients(today, 2, true);

            //send botswana
            SendReport(1, botswanaReport);
            SendReport(2, swazilandReport);

            //File.WriteAllBytes(Environment.CurrentDirectory + "\\tmpbots.xlsx", botswanaReport);
            //File.WriteAllBytes(Environment.CurrentDirectory + "\\tmpswazi.xlsx", swazilandReport);

            //save into log
            DailyReportLog log = new DailyReportLog()
            {
                CreatedOnUtc = DateTime.Now,
                LastSent = DateTime.Now
            };

            _dailyReportLogService.Insert(log);
        }

        private void SendReport(int regionId, byte[] data)
        {
            string part = "Botswana";
            if (regionId == 2) part = "Swaziland";

            string recipient = "casemanagement@healthshare.co.za";
            if (regionId == 2) recipient = "casemanagement.swazi@healthshare.co.za";

            //Send the email
            MailMessage message = null;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("username", "password");
            client.Host = "host";

            MemoryStream ms = new MemoryStream(data);

            try
            {
                message = new MailMessage();
                message.Subject = "Concordia Lodge Patient Report - " + DateTime.Now.ToString("dd MMMM yyyy hh:mm");



                message.Body = "Good Day\nPlease find attached the patient report for " + part + " for today.  Accommodation is on the first sheet and transport is on the second sheet.\nShould you have any technical queries, please feel free to contact Uwan Pretorius at uwan@cloudmeg.co.za.\nShould you have any disputes, please contact Sbu at sbuconcordia@gmail.com.";
                message.Attachments.Add(new Attachment(ms, "Concordia Lodge Patient Report.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                message.From = new MailAddress("system@concordiamilton.co.za", "Concordia Lodge");

                message.To.Add(new MailAddress(recipient));
                message.CC.Add(new MailAddress("bookings@concordialodge.co.za"));
                message.CC.Add(new MailAddress("sbuconcordia@gmail.com"));
                message.CC.Add(new MailAddress("uwan@cloudmeg.co.za"));
                message.CC.Add(new MailAddress("marcharlj@gmail.com"));

                client.Send(message);
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (message != null)
                    message.Dispose();

                ms.Dispose();
            }
        }
    }
}