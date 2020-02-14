using K3Channel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using Moq;
using K3Channel;

namespace K3ChannelTest
{
    [TestClass]
    public class RMQChannelTest
    {
        private int msgCount = 0;
        private int statusCount = 0;
        private ILog log;
        private IChannelDefinitionSettings cdSettings;
        private ChannelState channelState = ChannelState.none;

        public RMQChannelTest()
        {
            log = Mock.Of<ILog>();
            cdSettings = new ChannelDefinitionSettings();
            
            TestScafold.SetChannelDefinitionDefaults(cdSettings);
        }

        private void OnMessageEvent(object sender, IMessage myMessage)
        {
            msgCount++;
        }

        private void OnStatusMessageEvent(object sender, ChannelState prevStatus, ChannelState newStatus, IMessage myMessage)
        {
            statusCount++;
            channelState = newStatus;
        }

        /*
        [TestMethod]
        public void BasicChannelTests()
        {
            msgCount = 0;
            
            IChannel ch = new K3Channel.RMQChannel(log);
            
            ch.OnMessage += new OnMessageEvent(OnMessageEvent);

            IMessage msg = new Message();

            msg.Label = "bloop";
            msg.Data = "bloopper";
            msg.TargetSubID = MQRoutingKeyPrefix.INFO;

            ch.Begin(MQRoutingKeyPrefix.INFO);

            ch.Send(msg);

            System.Threading.Thread.Sleep(500);
            Assert.AreEqual<int>(1, msgCount);
        }
        */

        [TestMethod]
        [ExpectedException(typeof(RabbitMQ.Client.Exceptions.BrokerUnreachableException))]
        public void BadHostRMQTests()
        {
            msgCount = 0;

            IChannel ch = new K3Channel.RMQChannel(log);
            ch.Host = "192.1.99.99";

            ch.OnMessage += new OnMessageEvent(OnMessageEvent);

            IMessage msg = new Message();

            msg.Label = "bloop";
            msg.Data = "bloopper";
            msg.TargetSubID = MQRoutingKeyPrefix.INFO;

            ch.Begin(MQRoutingKeyPrefix.INFO);

            ch.Send(msg);

            System.Threading.Thread.Sleep(500);
            Assert.AreEqual<int>(1, msgCount);
        }

        [TestMethod]
        public void BasicRMQStatusOpenChangeTest()
        {
            statusCount = 0;
            var channelFactory = new ChannelFactory(cdSettings, log);
            var channel = channelFactory.Create(MQType.CONTROLLERLISTENER);
            channel.OnStatusMessage += new K3Channel.OnStatusMessageEvent(OnStatusMessageEvent);
            channel.Open();
            //IMessage msg = new Message();
            //msg.Label = MessageType.NOP;
            //msg.Data = MessageType.NOP;
            //msg.ClientID = "XYZ";
            //msg.TargetID = "DA1";
            //msg.TargetSubID = MQRoutingKeyPrefix.CONTROLLER;
            //channel.Send(msg);
            //System.Threading.Thread.Sleep(500);
            Assert.AreEqual(1, statusCount);
            Assert.AreEqual(ChannelState.open, channelState);
        }
        [TestMethod]
        public void BasicRMQStatusCloseChangeTest()
        {
            statusCount = 0;
            var channelFactory = new ChannelFactory(cdSettings, log);
            var channel = channelFactory.Create(MQType.CONTROLLERLISTENER);
            channel.OnStatusMessage += new K3Channel.OnStatusMessageEvent(OnStatusMessageEvent);
            channel.Open();
            channel.Close();
            //IMessage msg = new Message();
            //msg.Label = MessageType.NOP;
            //msg.Data = MessageType.NOP;
            //msg.ClientID = "XYZ";
            //msg.TargetID = "DA1";
            //msg.TargetSubID = MQRoutingKeyPrefix.CONTROLLER;
            //channel.Send(msg);
            //System.Threading.Thread.Sleep(500);
            Assert.AreEqual(2, statusCount);
            Assert.AreEqual(ChannelState.closed, channelState);
        }



        [TestMethod]
        public void BasicSendRMQTests()
        {

            var channelFactory = new ChannelFactory(cdSettings, log);
            var channel = channelFactory.Create(MQType.CONTROLLERLISTENER);
            channel.Open();
            IMessage msg = new Message();
            msg.Label = MessageType.NOP;
            msg.Data = MessageType.NOP;
            msg.ClientID = "XYZ";
            msg.TargetID = "DA1";
            msg.TargetSubID = MQRoutingKeyPrefix.CONTROLLER;
            channel.Send(msg);
            System.Threading.Thread.Sleep(500);
        }


        [TestMethod]
        public void SendTestRMQ1()
        {
            var channelFactory = new ChannelFactory(cdSettings, log);
            var channel = channelFactory.Create(MQType.CONTROLLERLISTENER);

            channel.OnMessage += new OnMessageEvent(OnMessageEvent);

            channel.Open();
            channel.Begin(MQRoutingKeyPrefix.CONTROLLER);


            IMessage msg = new Message();
            msg.Label = MessageType.NOP;
            msg.Data = MessageType.NOP;
            msg.ClientID = "JUB";
            msg.ClientSubID = "TEST";
            msg.TargetID = "sim";
            msg.TargetSubID = MQRoutingKeyPrefix.CONTROLLER;

            msgCount = 0;
            channel.Send(msg);

            System.Threading.Thread.Sleep(500);

            Assert.AreEqual(1, msgCount);
        }


        
    }
}