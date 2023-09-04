using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;

namespace SmartTcpComService
{
    public partial class Service : ServiceBase
    {
        private static readonly string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string ip = ConfigurationManager.AppSettings["IP"].ToString();
        private readonly int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
        private Thread thread = null;
        private bool running = false;
        private CreateClient client;
        private string response;
        private readonly List<string> ResponseList = new List<string>();
        public Service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            running = true;
            Thread.CurrentThread.Name = "MainThread";

            if (!Directory.Exists(appPath + "\\Logs\\"))
            {
                _ = Directory.CreateDirectory(appPath + "\\Logs\\");
            }
            ThreadStart job = new ThreadStart(starter);
            thread = new Thread(job)
            {
                CurrentCulture = new System.Globalization.CultureInfo("en-US"),
                Name = "SmartTcpComService"
            };
            thread.Start();
            Logger.WriteDebugLog("Service has started.");
        }

        public void starter()
        {
            while (running)
            {
                try
                {

                    client = new CreateClient(ip, port);
                    _ = client.connect();

                    //byte array  3c 30 30 30 3e 42 59 4D 0D 00
                    byte[] sendbyte = { 0x3c, 0x30, 0x30, 0x30, 0x3e, 0x42, 0x59, 0x4D, 0x0D, 0x00 };
                    if (client.IsConnected)
                    {
                        response = string.Empty;
                        string RequestTime = DateTime.Now.ToString();
                        //_ = client.sendMessage(str);
                        _ = client.sendByte(sendbyte);
                        Thread.Sleep(1000);
                        response = client.receiveMessage();
                        Logger.WriteExtraLog(response);

                        File.AppendAllText(appPath + "\\Logs\\" + "Response.txt", response + "\n");

                        string pattern = @".*\b\d{12}\b.*";
                        Regex regex = new Regex(pattern);

                        MatchCollection matches = regex.Matches(response);
                        foreach (Match match in matches)
                        {
                            ResponseList.Add(match.Value);
                        }

                        foreach (string resp in ResponseList)
                        {
                            string[] ActualVals = resp.Split(';');
                            if (ActualVals == null && ActualVals.Length < 37)
                            {
                                continue;
                            }
                            //230602162557 - Date , 2
                            string date = ActualVals[2];
                            //127 - Mixing Ratio,3
                            string mixingRatio = ActualVals[3];
                            //813 - Tempearture , 6
                            string temperature = ActualVals[6];
                            //580 - Relative Humidity ,11
                            string relativeHumidity = ActualVals[11];
                            //651 - Dew point , 16
                            string dewPoint = ActualVals[16];
                            //2117 - vapourpressure , 21
                            string vapourPressure = ActualVals[21];
                            //133 - Absolute Humidity 1 ,26 or 153 - Absolute Humidity 2 ,31
                            string absoluteHumidity = ActualVals[26];
                            //702 - wet bulb Tempearture, 36
                            string wetBulbTemperature = ActualVals[36];
                            DataBaseAcess.InsertIntoDB(date, mixingRatio, temperature, relativeHumidity, dewPoint, vapourPressure, absoluteHumidity, wetBulbTemperature, RequestTime);

                        }
                    }

                }
                catch { }
                finally
                {
                    client.disconnect();
                    Thread.Sleep(1000);
                }
            }
        }
        protected override void OnStop()
        {
            running = false;
            thread.Abort();
            Thread.CurrentThread.Name = "MainThread";
            Logger.WriteDebugLog("Service has stopped.");
        }
        internal void OnDebug()
        {
            OnStart(null);
        }

    }

}







