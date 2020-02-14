using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using K3Channel;
using Moq;
using log4net;
using K3Channel;

namespace K3ChannelTest
{
    [TestClass]
    public class MSBChannelTest
    {
        int msgCount = 0;
         private ILog log;
        private IChannelDefinitionSettings cdSettings;

        public MSBChannelTest()
        {
            log = Mock.Of<ILog>();
            cdSettings = new ChannelDefinitionSettings();
            
            TestScafold.SetChannelDefinitionDefaults(cdSettings);
        }


        private void OnMessageEvent(object sender, IMessage myMessage)
        {
            msgCount++;
        }

        [TestMethod]
        public void MSBBasicChannelTests()
        {
            var log =  Mock.Of<ILog>();
            K3Channel.ChannelFactory cf = new K3Channel.ChannelFactory(cdSettings, log);


            IChannel ch = cf.Create("MSB");
            ch.OnMessage += new OnMessageEvent(OnMessageEvent);
            ch.Open();

            IMessage msg = new Message();

            msg.Label = "bloop";
            msg.Data = "bloopper";
            msg.TargetSubID = MQRoutingKeyPrefix.INFO;

            ch.Send(msg);

            ch.Begin();

            ch.Send(msg);
            System.Threading.Thread.Sleep(5000);

        }
    }
}
