using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMDR_Service
{
    class SMDR_Server
    {
        private int port;
        private IPAddress ip;
        private string logDirectory;
        private TcpListener tcpListener;
        private Thread listenThread;
        private string eventSource;
        private bool logRaw;

        public SMDR_Server(IPAddress ip, int port, string logDirectory)
        {
            this.port = port;
            this.ip = ip;
            this.logDirectory = logDirectory;
            this.eventSource = "SMDR Logging";
            try
            {
                if (!EventLog.Exists(this.eventSource))
                    EventLog.CreateEventSource(this.eventSource, "Application");
            }
            catch (Exception ex)
            {
            }
            if (this.ip.Equals(IPAddress.Parse("0.0.0.0")))
            {
                this.tcpListener = new TcpListener(IPAddress.Any, this.port);
            }
            else
            {
                this.tcpListener = new TcpListener(this.ip, this.port);
            }
            this.listenThread = new Thread(new ThreadStart(this.ListenForClients));
            this.listenThread.Start();
            this.logRaw = true;
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();
            while (true)
                new Thread(new ParameterizedThreadStart(this.HandleClientComm)).Start((object)this.tcpListener.AcceptTcpClient());
        }

        private void HandleClientComm(object client)
        {
            using (TcpClient tcpClient = (TcpClient)client)
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    byte[] numArray = new byte[4096];
                    while (true)
                    {
                        string @string;
                        do
                        {
                            int count;
                            try
                            {
                                count = stream.Read(numArray, 0, 4096);
                            }
                            catch
                            {
                                goto label_14;
                            }
                            if (count != 0)
                            {
                                @string = new ASCIIEncoding().GetString(numArray, 0, count);
                                if (this.logRaw)
                                    this.OutputSMDR(@string, true);
                            }
                            else
                                goto label_14;
                        }
                        while (!this.isSMDRRecord(@string));
                        this.OutputSMDR(this.createBasicSMDRRecordFromString(@string).ToString(), false);
                    }
                }
                label_14:
                tcpClient.Close();
            }
        }

        private bool isSMDRRecord(string _in)
        {
            return _in.Split(',').Length == 30;
        }

        private SMDRRecord createBasicSMDRRecordFromString(string _in)
        {
            string[] strArray = _in.Split(',');
            foreach (string str in strArray)
                ;
            SMDRRecord smdrRecord;
            smdrRecord.CallStart = DateTime.Parse(strArray[0]);
            smdrRecord.ConnectedTime = TimeSpan.Parse(strArray[1]);
            smdrRecord.RingTime = TimeSpan.FromSeconds(Convert.ToDouble(strArray[2]));
            smdrRecord.Caller = string.IsNullOrEmpty(strArray[3]) ? "Unknown" : strArray[3];
            smdrRecord.CallDirection = !(strArray[4] == "I") ? "Outbound" : "Inbound";
            smdrRecord.DialedNumber = string.IsNullOrEmpty(strArray[5]) ? "Unknown" : strArray[5];
            return smdrRecord;
        }

        private void OutputSMDR(string _record, bool isRaw)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + "Reports\\" + DateTime.Now.ToString("yyyy.MM") + ".csv";
            string path = this.logDirectory + DateTime.Now.ToString("yyyy.MM") + ".csv";
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path, true))
                {
                    if (streamWriter.BaseStream.Length == 0L)
                    {
                        if (isRaw)
                            streamWriter.WriteLine("Call Start,Connected Time,Ring Time,Caller,Direction,Called Number,Dialled Number,Account,Is Internal,Call ID,Continuation,Party1Device,Party1Name,Party2Device,Party2Name,Hold Time,Park Time,Auth Valid,Auth Code,User Charged,Call Charge,Currency,Account at Last User Change,Call Units,Units at Last User Change,Cost per Unit,Mark Up,External Targeting Cause,External Targeter Id,External Targeted Number");
                        else
                            streamWriter.WriteLine("Call Start,Connected Time,Ring Time,Caller,Call Direction,Dialed Number");
                    }
                    streamWriter.WriteLine(_record.TrimEnd('\r', '\n'));
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
