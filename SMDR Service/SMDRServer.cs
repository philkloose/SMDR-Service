using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;

namespace SMDR_Service
{
    public partial class SMDRServer : ServiceBase
    {
        public SMDRServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["TCPPort"], out port))
            { throw new InvalidOperationException("Invalid TCPPort in app.config"); }

            IPAddress ip;
            if(!IPAddress.TryParse(ConfigurationManager.AppSettings["IPAddress"], out ip))
            { throw new InvalidOperationException("Invalid IP Address specified in app.config"); }

            string logDirectory;
            if (Directory.Exists(ConfigurationManager.AppSettings["LogDirectory"]))
            {
                logDirectory = ConfigurationManager.AppSettings["LogDirectory"];
            }
            else
            { throw new DirectoryNotFoundException("Specified directory does not exist."); }

            this.EventLog.WriteEntry("SMDR Service starting on " + ip + ":" + port);
            SMDR_Server smdrServer = new SMDR_Server(ip, port, logDirectory);
            this.EventLog.WriteEntry("SMDR Service started on " + ip + ":" + port);
        }

        protected override void OnStop()
        {
        }
    }
}
