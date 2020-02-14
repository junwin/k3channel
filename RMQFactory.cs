using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;

namespace K3Channel
{
    public class RMQFactory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile RMQFactory s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        /// <summary>
        /// Rabbit connection Factory
        /// </summary>
        private ConnectionFactory m_RMQConnectionFactory = null;

        private IConnection m_Conn = null;

        /// <summary>
        /// RMQ channel used to publish general info - initially used to support charts
        /// </summary>
        //private IModel m_InfoChannel = null;

        private Dictionary<string, IModel> infoChannels = null;

        private string hostName = "127.0.0.1";

        // Create a logger for use in this class
        private log4net.ILog m_Log;

        public static RMQFactory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new RMQFactory();
                    }
                }
            }
            return s_instance;
        }

        protected RMQFactory()
        {
            m_Log = log4net.LogManager.GetLogger("KTARemote");
            infoChannels = new Dictionary<string, IModel>();
        }

        public void Close()
        {
            if (m_Conn != null)
            {
                try
                {
                    foreach (var infoChannel in infoChannels.Values)
                    {
                        infoChannel.Close();
                    }
                    infoChannels.Clear();
                }
                catch
                {
                }
                m_Conn.Close();
                m_Conn = null;
                m_RMQConnectionFactory = null;
            }
        }

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        public bool CanConnect()
        {
            bool canConnect = false;
            try
            {
                canConnect = true;
            }
            catch
            {
            }
            return canConnect;
        }

        public ConnectionFactory GetConnectionFactory(string hostName)
        {
            try
            {
                if (m_RMQConnectionFactory == null)
                {
                    m_RMQConnectionFactory = new ConnectionFactory();

                    m_RMQConnectionFactory.Protocol = Protocols.DefaultProtocol;

                    m_RMQConnectionFactory.HostName = hostName;
                    m_RMQConnectionFactory.UserName = "k3user";
                    m_RMQConnectionFactory.Password = "changeit";


                    m_Conn = m_RMQConnectionFactory.CreateConnection();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetConnectionFactory", myE);
                throw myE;
            }
            return m_RMQConnectionFactory;
        }

        public IModel GetRMQChannel(string exchangeName, string hostName = "127.0.0.1")
        {
            try
            {
                if (!infoChannels.ContainsKey(exchangeName))
                {
                    GetConnectionFactory(hostName);
                    var infoChannel = m_Conn.CreateModel();
                    infoChannel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
                    infoChannels.Add(exchangeName, infoChannel);
                }
                return infoChannels[exchangeName];
            }
            catch (Exception myE)
            {
                m_Log.Error("GetRMQInfoChannel", myE);
                throw myE;
            }
            return null;
        }

        public IModel CloseRMQChannel(string exchangeName)
        {
            try
            {
                if (infoChannels.ContainsKey(exchangeName))
                {
                    //infoChannels[exchangeName].Close()
                    infoChannels[exchangeName].Close(Constants.ReplySuccess, "Closing the channel");
                    //m_Conn.Close(Constants.ReplySuccess, "Closing the connection");
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetRMQInfoChannel", myE);
            }
            return null;
        }
    }
}