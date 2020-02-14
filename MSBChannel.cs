using K3Channel;
using log4net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace K3Channel
{
    public class MSBChannel : Channel
    {
        private static QueueClient queueClient;

        //private static string QueueName = "SampleQueue";
        private static string QueueName = "ProcessingQueue";

        private const Int16 maxTrials = 4;

        public void Configure()
        {
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);
        }

        public MSBChannel(ILog log)
        {
            //connectionString = @"Endpoint=sb://k3data.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=RDoromKnrvwCdcW2XjSkUGZbNT6QMCLC7wXnyAuOv7o=";
            //Name = "procq1";
            State = ChannelState.none;
            this.log = log;
        }

        public override void Begin()
        {
            if (listenTask == null)
            {
                listenTask = Task.Run(() => { ReceiveMessages(); });
            }
        }

        private void ReceiveMessages()
        {
            BrokeredMessage message = null;
            queueClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            while (true)
            {
                try
                {
                    //receive messages from Queue
                    message = queueClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        if (onMessage != null)
                        {
                            IMessage msg = new Message();
                            msg.Label = message.Label;
                            msg.Data = message.GetBody<string>();
                            msg.Identity = message.MessageId;
                            msg.CorrelationID = message.CorrelationId;
                            var props = message.Properties;
                            if (props.ContainsKey("Tag"))
                            {
                                msg.Tag = props["Tag"].ToString();
                            }
                            if (props.ContainsKey("TargetID"))
                            {
                                msg.TargetID = props["TargetID"].ToString();
                            }
                            if (props.ContainsKey("TargetSubID"))
                            {
                                msg.TargetSubID = props["TargetSubID"].ToString();
                            }
                            if (props.ContainsKey("ClientID"))
                            {
                                msg.ClientID = props["ClientID"].ToString();
                            }
                            if (props.ContainsKey("ClientSubID"))
                            {
                                msg.ClientSubID = props["ClientSubID"].ToString();
                            }

                            onMessage(this, msg);
                        }
                        message.Complete();
                    }
                    else
                    {
                        //no more messages in the queue
                        break;
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
            }
            queueClient.Close();
        }

        public override void Close()
        {
            State = ChannelState.closed;
        }

        public override void Open()
        {
            CreateQueue();
            State = ChannelState.open;
        }

        public override int Port
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void Send(IMessage msg)
        {
            queueClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);

            BrokeredMessage message = new BrokeredMessage(msg.Data);
            message.MessageId = msg.Identity;
            message.Label = msg.Label;
            message.CorrelationId = msg.CorrelationID;
            message.Properties["Tag"] = msg.Tag;
            message.Properties["TargetSubID"] = msg.TargetSubID;
            message.Properties["TargetID"] = msg.TargetID;
            message.Properties["ClientID"] = msg.ClientID;
            message.Properties["ClientSubID"] = msg.ClientSubID;

            while (true)
            {
                try
                {
                    queueClient.Send(message);
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
                Console.WriteLine(string.Format("Message sent: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                break;
            }
        }

        private void CreateQueue()
        {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            Console.WriteLine("\nCreating Queue '{0}'...", QueueName);

            // Delete if exists
            if (namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.DeleteQueue(QueueName);
            }

            namespaceManager.CreateQueue(QueueName);
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry
            Console.WriteLine(e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}