using K3Channel;
using log4net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace K3Channel
{
    public class MSTopicChannel : Channel, IDisposable
    {
        private TopicClient topicClient;

        private const Int16 maxTrials = 4;
        private string subscriptionName = "";
        private NamespaceManager namespaceManager = null;
        private TopicDescription topicDescription = null;

        public MSTopicChannel(ILog log)
        {
            State = ChannelState.none;
            this.log = log;
            SubscriptionName = string.Format("SN{0}", System.Guid.NewGuid().ToString());
        }

        public string SubscriptionName
        {
            get { return subscriptionName; }
            set { subscriptionName = value; }
        }

        public override void Begin()
        {
            Begin("");
        }

        public override void Begin(string routingKey)
        {
            CreateTopic(routingKey);
            if (listenTask == null)
            {
                listenTask = Task.Run(() => { ReceiveMessages(); });
            }
            State = ChannelState.open;
        }

        private void ReceiveMessages()
        {
            // For PeekLock mode (default) where applications require "at least once" delivery of messages
            SubscriptionClient agentSubscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, Name, SubscriptionName);
            BrokeredMessage message = null;
            while (true)
            {
                try
                {
                    //receive messages from Agent Subscription
                    message = agentSubscriptionClient.Receive();
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
                        //no more messages in the subscription
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
            agentSubscriptionClient.Close();
            /*
            // For ReceiveAndDelete mode, where applications require "best effort" delivery of messages
            SubscriptionClient auditSubscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, Name, "AuditSubscription", ReceiveMode.ReceiveAndDelete);
            while (true)
            {
                try
                {
                    message = auditSubscriptionClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Console.WriteLine("\nReceiving message from AuditSubscription...");
                        Console.WriteLine(string.Format("Message received: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                        // Further custom message processing could go here...
                    }
                    else
                    {
                        //no more messages in the subscription
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
                }
            }
            auditSubscriptionClient.Close();
             */
        }

        public override void Send(IMessage msg)
        {
            List<BrokeredMessage> messageList = new List<BrokeredMessage>();
            BrokeredMessage message = new BrokeredMessage(msg.Data);
            message.MessageId = msg.Identity;
            message.Label = msg.Label;
            message.CorrelationId = msg.CorrelationID;
            message.Properties["Tag"] = msg.Tag;
            message.Properties["TargetSubID"] = msg.TargetSubID;
            message.Properties["TargetID"] = msg.TargetID;
            message.Properties["ClientID"] = msg.ClientID;
            message.Properties["ClientSubID"] = msg.ClientSubID;

            messageList.Add(message);

            topicClient = TopicClient.CreateFromConnectionString(connectionString, Name);

            foreach (BrokeredMessage bm in messageList)
            {
                while (true)
                {
                    try
                    {
                        topicClient.Send(bm);
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
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(string.Format("Message sent: Id = {0}, Body = {1}", bm.MessageId, bm.GetBody<string>()));
                    }
                    break;
                }
            }

            topicClient.Close();
        }

        private void CreateTopic(string routingKey)
        {
            namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            try
            {
                if (namespaceManager.TopicExists(Name))
                {
                    topicDescription = namespaceManager.GetTopic(Name);
                    var subs = namespaceManager.GetSubscriptions(topicDescription.Path);
                    foreach (var dong in subs)
                    {
                        Console.WriteLine(dong.Name);
                    }

                    if (!namespaceManager.SubscriptionExists(topicDescription.Path, SubscriptionName))
                    {
                        if (routingKey == string.Empty)
                        {
                            SubscriptionDescription myAuditSubscription = namespaceManager.CreateSubscription(topicDescription.Path, SubscriptionName);
                        }
                        else
                        {
                            string filterExpresssion = string.Format("TargetSubID = '{0}'", routingKey);
                            SqlFilter filter = new SqlFilter(filterExpresssion);
                            SubscriptionDescription myAuditSubscription = namespaceManager.CreateSubscription(topicDescription.Path, SubscriptionName, filter);
                        }
                    }
                }
                else
                {
                    //namespaceManager.DeleteTopic(Name);
                    topicDescription = namespaceManager.CreateTopic(Name);
                }
            }
            catch (MessagingException e)
            {
                log.Error("createTopic:", e);
                throw;
            }
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry
            Console.WriteLine(e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }

        public override void Close()
        {
            onMessage = null;
            if (namespaceManager != null && topicDescription != null)
            {
                if (!namespaceManager.SubscriptionExists(topicDescription.Path, SubscriptionName))
                {
                    namespaceManager.DeleteSubscription(topicDescription.Path, SubscriptionName);
                }
            }

            State = ChannelState.closed;
            onStatusMessage = null;
        }

        public override void Open()
        {
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

        public void Dispose()
        {
            this.Close();
        }
    }
}