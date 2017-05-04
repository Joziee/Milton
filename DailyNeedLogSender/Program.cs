using Milton.Services.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Mvc;
using System.Net.Mail;
using System.IO;

namespace DailyNeedLogSender
{
    class Program
    {
        #region Fields

        protected IReportService _reportService;

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

            builder.RegisterType<ReportService>().As<IReportService>();

            var container = builder.Build();

            #endregion

            #region Services

            _reportService = container.Resolve<IReportService>();

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
            var botswanaReport = _reportService.GenerateLogReport(1,true);
            var swazilandReport = _reportService.GenerateLogReport(2, true);

            //send botswana
            SendReport(1, botswanaReport);
            SendReport(2, swazilandReport);
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
                message.Subject = "Concordia Lodge Patients Authorizations Request - " + DateTime.Now.ToString("dd MMMM yyyy hh:mm");



                message.Body = "Good Day\nPlease find attached the outstanding Authorizations for " + part + " patients that Concordia Lodge still require.  Please could you send them as soon as possible through to Sbu at sbuconcordia@gmail.com.\nYour assistance in this regard will be greatly appreciated.";
                message.Attachments.Add(new Attachment(ms, "Concordia Lodge Patient Authorizations and Referrals.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                message.From = new MailAddress("system@concordiamilton.co.za", "Concordia Lodge");

                message.To.Add(new MailAddress(recipient));
                message.CC.Add(new MailAddress("bookings@concordialodge.co.za"));
                message.CC.Add(new MailAddress("sbuconcordia@gmail.com"));
                message.CC.Add(new MailAddress("uwan@cloudmeg.co.za"));

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
