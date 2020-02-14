
using log4net;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using K3Channel;

namespace K3Channel
{
    public class RMQChannel : Channel
    {
        private RMQListner listner = null;
        private string Id;
        private ILog log;

        public RMQChannel(ILog log)
        {
            //listner = new RMQListner();
            string Id = "DARC_" + name;
            name = MQRoutingKeyPrefix.INFO;
            //Host = Environment.MachineName;
            Host = "localhost";
            this.log = log;
        }

        public override void Begin()
        {
            if (listner == null)
            {
                RMQFactory.Instance().HostName = Host;
                listner = new RMQListner(log);
                listner.SubscribeInfo(Host, name, "");
                listner.OnRMQMessage += OnRMQMessage;
                /*
                listner.SubscribeProductsRMQ("");
                listner.SubscribeAccountsRMQ("");
                listner.SubscribeProductsRMQ("");
                listner.SubscribeTSBarsRMQ("*");
                 */
            }
        }

        public override void Begin(string routingKey)
        {
            if (listner == null)
            {
                RMQFactory.Instance().HostName = Host;
                listner = new RMQListner(log);
                listner.SubscribeInfo(Host, name, routingKey);
                listner.OnRMQMessage += OnRMQMessage;
                /*
                listner.SubscribeProductsRMQ("");
                listner.SubscribeAccountsRMQ("");
                listner.SubscribeProductsRMQ("");
                listner.SubscribeTSBarsRMQ("*");
                 */
            }
        }

        public IChannel Channel
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RMQInfoQueueName
        { get { return "I" + Id; } }

        public void Subscribe(string routingKey)
        {
            RMQFactory.Instance().GetRMQChannel(MQExchanges.DEFAULT).QueueBind(RMQInfoQueueName, MQExchanges.DEFAULT, routingKey, null);
        }

        public override void Close()
        {
            if (listner != null)
            {
                listner.Close();
                listner = null;
            }
            State = ChannelState.closed;
        }

        public override void Open()
        {
            State = ChannelState.open;
        }

       
        private string GetHeaderAsString(object o)
        {
            var byteArray = o as byte[];
            string ret = null;
            if(byteArray != null)
                ret = System.Text.Encoding.Default.GetString(byteArray);
            return ret;
        }

        public void OnRMQMessage(IBasicProperties props, string messageData)
        {
            IMessage message = new Message();
            message.Label = props.Type;
            message.Data = messageData;
            message.CorrelationID = props.CorrelationId;
            if (props.Headers.ContainsKey("Tag"))
            {
                message.Tag = GetHeaderAsString(props.Headers["Tag"]);
            }
            if (props.Headers.ContainsKey("TargetID"))
            {
                GetHeaderAsString(props.Headers["TargetID"]);
                message.TargetID = GetHeaderAsString(props.Headers["TargetID"]);
            }
            if (props.Headers.ContainsKey("TargetSubID"))
            {
                message.TargetSubID = GetHeaderAsString(props.Headers["TargetSubID"]);
            }
            if (props.Headers.ContainsKey("ClientID"))
            {
                message.ClientID = GetHeaderAsString(props.Headers["ClientID"]);
            }
            if (props.Headers.ContainsKey("ClientSubID"))
            {
                message.ClientSubID = GetHeaderAsString(props.Headers["ClientSubID"]);
            }

            if (onMessage != null)
            {
                onMessage(this, message);
            }
        }

        public void Publish(IMessage msg)
        {
            try
            {
                byte[] messageBody = Encoding.UTF8.GetBytes(msg.Data);
                IBasicProperties props = RMQFactory.Instance().GetRMQChannel(name, Host).CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;
                props.Type = msg.Label;
                props.ReplyTo = msg.ClientID;
                props.CorrelationId = msg.CorrelationID;
                props.Headers = new Dictionary<string, object>();

                props.Headers.Add("Tag", msg.Tag);
                props.Headers.Add("TargetSubID", msg.TargetSubID);
                props.Headers.Add("TargetID", msg.TargetID);
                props.Headers.Add("ClientID", msg.ClientID);
                props.Headers.Add("ClientSubID", msg.ClientSubID);
                //props.AppId = msg.AppType;

                RMQFactory.Instance().GetRMQChannel(name).BasicPublish(name, msg.TargetSubID, props, messageBody);
            }
            catch (Exception ex)
            {
                log.Error("Publish:", ex);
                throw ex;
            }
        }

        public override void Send(IMessage msg)
        {
            try
            {
                if (msg.TargetSubID.Length == 0)
                {
                    msg.TargetSubID = MQRoutingKeyPrefix.INFO;
                }
                if (msg.Data == null)
                {
                    msg.Data = "";
                }
                Publish(msg);
            }
            catch (Exception ex)
            {
                log.Error("Send:", ex);
                throw ex;
            }
        }
    }
}