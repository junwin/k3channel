using K3Channel;
using System.Collections.Generic;

namespace K3ChannelTest
{
    public class TestScafold
    {
        public static void SetChannelDefinitionDefaults(List<IChannelDefinition> cds)
        {
            //channelDefinitions.Add(new ChannelDefinition("ControllerListener", "127.0.0.1", 0, MQExchanges.DEFAULT, "ControllerListener", "mem"));
            cds.Add(new ChannelDefinition("DriverSender", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.Add(new ChannelDefinition("MEM", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.Add(new ChannelDefinition("RMQ", "10.1.11.19", 5672, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition("DEFAULT", "127.0.0.1", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "mem"));
            cds.Add(new ChannelDefinition("MSB", @"Endpoint=sb://k3data.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YdCmLwMmMhgkY3HIiZ5AoCwZQOyOBGA+LyDvgrE5sN4=", 0, MQExchanges.DEFAULT, "textxyz", "K3Channel.MSBChannel"));
            cds.Add(new ChannelDefinition("MSTOPIC", @"Endpoint=sb://k3data.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YdCmLwMmMhgkY3HIiZ5AoCwZQOyOBGA+LyDvgrE5sN4=", 0, MQExchanges.DEFAULT, "textzzz", "K3Channel.MSTopicChannel"));

            cds.Add(new ChannelDefinition(MQType.CONTROLLERLISTENER, "10.1.11.19", 5672, MQExchanges.DEFAULT, MQRoutingKeyPrefix.CONTROLLER, "K3Channel.RMQChannel"));

            // Used in the DA Publisher Gateway
            cds.Add(new ChannelDefinition(MQType.DRIVERMESSAGE, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.PRICES, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.PRICES, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.PRODUCT, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.PRODUCT, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.TSBAR, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.TSBAR, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.TRADESIGNAL, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.TRADESIGNAL, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.CONNECTIONSTATUS, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.INFO, "K3Channel.RMQChannel"));
            cds.Add(new ChannelDefinition(MQType.ACCOUNT, "10.1.11.19", 0, MQExchanges.DEFAULT, MQRoutingKeyPrefix.ACCOUNT, "K3Channel.RMQChannel"));
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cds);
        }
    }
}