using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SmartTcpComService
{
    public class CreateClient
    {
        private readonly string serverIP;
        private readonly int serverPort;
        private readonly TcpClient tcpClient;
        public NetworkStream stream;
        public bool IsConnected => tcpClient.Connected;
        public CreateClient(string ip, int port)
        {
            serverIP = ip;
            serverPort = port;
            tcpClient = new TcpClient();
        }

        public bool connect()
        {
            try
            {
                tcpClient.Connect(IPAddress.Parse(serverIP), serverPort);
                stream = tcpClient.GetStream();
                Logger.WriteExtraLog("Connected to server.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                return false;
            }
        }

        public void disconnect()
        {
            try
            {
                if (IsConnected)
                {
                    tcpClient.Close();
                    stream.Close();
                    Logger.WriteExtraLog("Disconnected from server.");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        public bool sendMessage(string str)
        {

            try
            {
                if (!IsConnected)
                {
                    Logger.WriteExtraLog("Not connected to server.");
                    return false;
                }

                byte[] data = Encoding.ASCII.GetBytes(str);
                stream.Write(data, 0, data.Length);
                //print in hexa 
                foreach (byte b in data)
                {
                    Logger.WriteExtraLog("Sent: " + b.ToString("X2"));
                }
                Logger.WriteExtraLog("Sent: " + str);
                return true;

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                return false;
            }
        }

        public string receiveMessage()
        {
            try
            {

                if (!IsConnected)
                {
                    Logger.WriteExtraLog("Not connected to server.");
                    return null;
                }
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }

            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                return null;
            }
        }

        public bool sendByte(byte[] bytes)
        {
            try
            {
                if (!IsConnected)
                {
                    Logger.WriteExtraLog("Not connected to server.");
                    return false;
                }
                stream.Write(bytes, 0, bytes.Length);
                Logger.WriteExtraLog("Sent: " + bytes.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                return false;
            }
        }

    }
}