
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Moq;
using log4net;
using K3Channel;

namespace K3ChannelTest
{
    [TestClass]
    public class MemoryGWTest
    {
        private int msgCount = 0;

        private void OnMessageEvent(object sender, IMessage myMessage)
        {
            msgCount++;
        }

        [TestMethod]
        public void MemorySingleMessageTest()
        {
            var log = Mock.Of<ILog>();
            msgCount = 0;
            IChannel ch = K3Channel.MemoryChannelFactory.Instance().GetChannel(MQRoutingKeyPrefix.INFO, log);
            ch.OnMessage += new OnMessageEvent(OnMessageEvent);

            IMessage msg = new Message();

            msg.Label = "bloop";
            msg.Data = "bloopper";
            msg.TargetSubID = MQRoutingKeyPrefix.INFO;
            msg.TargetID = MQExchanges.DEFAULT;

            ch.Begin();

            ch.Send(msg);

            System.Threading.Thread.Sleep(500);
            Assert.AreEqual<int>(1, msgCount);

            ch.Send(msg);
            ch.Send(msg);

            Thread.Sleep(500);

            Assert.IsTrue(msgCount == 3);
        }
    }
}