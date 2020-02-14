using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3Channel;
using System.Collections.Generic;

namespace K3ChannelTest
{
    public class TestScafold
    {
        public static void  SetChannelDefinitionDefaults(IChannelDefinitionSettings cds)
        {
            //channelDefinitions.Add(new ChannelDefinition("ControllerListener", "127.0.0.1", 0, MQExchanges.DEFAULT, "ControllerListener", "mem"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("DriverSender", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("MEM", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("RMQ", "10.1.11.19", 5672, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("DEFAULT", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("MSB", @"Endpoint=sb://k3data.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YdCmLwMmMhgkY3HIiZ5AoCwZQOyOBGA+LyDvgrE5sN4=", 0, MQExchanges.DEFAULT, "textxyz", "K3Channel.MSBChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition("MSTOPIC", @"Endpoint=sb://k3data.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YdCmLwMmMhgkY3HIiZ5AoCwZQOyOBGA+LyDvgrE5sN4=", 0, MQExchanges.DEFAULT, "textzzz", "K3Channel.MSTopicChannel"));

           

            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.CONTROLLERLISTENER, "10.1.11.19", 5672, MQExchanges.DEFAULT, MQRoutingKeyPrefix.CONTROLLER, "K3Channel.RMQChannel"));

            // Used in the DA Publisher Gateway
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.DRIVERMESSAGE, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.PRICES, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.PRICES, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.PRODUCT, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.PRODUCT, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.TSBAR, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.TSBAR, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.TRADESIGNAL, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.TRADESIGNAL, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.CONNECTIONSTATUS, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.ChannelDefinitions.Add(new ChannelDefinition(MQType.ACCOUNT, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.ACCOUNT, "K3Channel.RMQChannel"));
        }
    }
}
