using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using log4net;

namespace K3Channel
{
    public class RMQListner
    {
        /// <summary>
        /// Used to get price updates from the driver
        /// </summary>
        /// <param name="pxUpdate"></param>
        public delegate void RMQMessage(IBasicProperties props, string messageData);

        private RMQMessage onRMQMessage = null;

        private QueueingBasicConsumer m_Consumer;
        private ulong m_InCount = 0;
        private ulong m_OutCount = 0;

        private Thread m_SubscribeThread;
        private bool m_Subscribe = false;

        // Create a logger for use in this class
        private log4net.ILog m_Log;

        private string UID = "";

        private string localRMQInfoQueueName = "";

        public RMQListner(ILog log)
        {
            m_Log = log;
            UID = Environment.MachineName;
            localRMQInfoQueueName = "I" + UID + "_" + DateTime.Now.Ticks.ToString();
        }

        public string RMQInfoQueueName
        { get { return localRMQInfoQueueName; } }

        public void Close()
        {
            RMQFactory.Instance().Close();
        }

        public RMQMessage OnRMQMessage
        {
            get { return onRMQMessage; }
            set { onRMQMessage = value; }
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="exchange">things are published to an exchange
        /// with a routing key</param>
        /// <param name="routingKey"></param>
        public void SubscribeInfo(string hostName, string exchange, string routingKey)
        {
            try
            {
                RMQFactory.Instance().GetRMQChannel(exchange,hostName).ExchangeDeclare(exchange, ExchangeType.Topic);

                //toolStripStatusLabel1.Text="Creating queue...";
                RMQFactory.Instance().GetRMQChannel(exchange, hostName).QueueDeclare(RMQInfoQueueName, false, false, true, null);
                RMQFactory.Instance().GetRMQChannel(exchange, hostName).QueueBind(RMQInfoQueueName, exchange, routingKey, null);

                m_Consumer = new QueueingBasicConsumer(RMQFactory.Instance().GetRMQChannel(exchange));
                RMQFactory.Instance().GetRMQChannel(exchange, hostName).BasicConsume(RMQInfoQueueName, false, m_Consumer);

                m_Subscribe = true;
                m_SubscribeThread = new Thread(RMQListen);
                m_SubscribeThread.Start();
            }
            catch (Exception myE)
            {
                m_Log.Error("SubscribeInfo", myE);
                throw myE;
            }
        }

        //public string RMQInfoQueueName
        //{ get { return "I" + UID; } }

        /// <summary>
        /// Request a subscription to tradesignals - trade signals can simply be conditions
        /// </summary>
        /// <param name="key">AMQP routing key</param>
        public void SubscribeTradeSignals(string key)
        {
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, key, null);
        }

        public void SubscribePricesRMQ(string mnemonic)
        {
            string key = MQRoutingKeyPrefix.PRICES + mnemonic;
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, key, null);
        }

        public void SubscribeTSBarsRMQ(string mnemonic)
        {
            string key = MQRoutingKeyPrefix.TSBAR + mnemonic;
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, key, null);
        }

        public void SubscribeAccountsRMQ(string routingKay)
        {
            string key = MQRoutingKeyPrefix.ACCOUNT + routingKay;
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, key, null);
        }

        public void SubscribeProductsRMQ(string routingKay)
        {
            string key = MQRoutingKeyPrefix.PRODUCT + routingKay;
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, key, null);
        }

        private void RMQListen()
        {
            while (m_Subscribe)
            {
                try
                {
                    BasicDeliverEventArgs inE = (BasicDeliverEventArgs)m_Consumer.Queue.Dequeue();
                    IBasicProperties props = inE.BasicProperties;
                    byte[] body = inE.Body;

                    if (onRMQMessage != null)
                    {
                        onRMQMessage(props, Encoding.UTF8.GetString(body));
                    }

                    RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).BasicAck(inE.DeliveryTag, false);
                }
                catch
                {
                }
            }
            RMQFactory.Instance().CloseRMQChannel(MQExchanges.DEFAULT);
        }

        /*
         * if (props.Type == K2ServiceSupportIF.MQDataType.PRICE)
                    {
                        K2DataObjects.PXUpdateBase px = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.PXUpdateBase>(Encoding.UTF8.GetString(body));
                        if (m_PxUpdate != null)
                        {
                            m_PxUpdate("", px);
                        }
                    }
                    else if (props.Type == K2ServiceSupportIF.MQDataType.TSITEM)
                    {
                        K2DataObjects.TSItem[] tsDataItems = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.TSItem[]>(Encoding.UTF8.GetString(body));
                        if (tsDataItems.Length > 0)
                        {
                            if (m_TsdUpdate != null)
                            {
                                m_TsdUpdate("", "", tsDataItems[0].Mnemonic, tsDataItems);
                            }
                        }
                    }
                    else if (props.Type == K2ServiceSupportIF.MQDataType.SIGNAL)
                    {
                        K2DataObjects.TradeSignal[] signals = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.TradeSignal[]>(Encoding.UTF8.GetString(body));
                        if (signals.Length > 0)
                        {
                            if (m_TradeSignalUpdate != null)
                            {
                                foreach (K2DataObjects.TradeSignal s in signals)
                                {
                                    m_TradeSignalUpdate(s);
                                }
                            }
                        }
                    }
                    else if (props.Type == K2ServiceSupportIF.MQDataType.ORDER)
                    {
                        K2DataObjects.Order order = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.Order>(Encoding.UTF8.GetString(body));

                        if (m_OrderUpdate != null)
                        {
                            m_OrderUpdate("*", order);
                        }
                    }
                    else if (props.Type == K2ServiceSupportIF.MQDataType.FILL)
                    {
                        K2DataObjects.Fill fill = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.Fill>(Encoding.UTF8.GetString(body));

                        if (m_FillUpdate != null)
                        {
                            m_FillUpdate("*", fill);
                        }
                    }
                    else
                    {
                        if (m_DataObjectUpdate != null)
                        {
                            m_DataObjectUpdate("*", props.Type, Encoding.UTF8.GetString(body));
                        }
                    }
         */
    }
}