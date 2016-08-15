using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
        }

        protected override void OnStop()
        {
        }
    }
}
