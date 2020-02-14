using K3Channel;
using System.Collections.Generic;

namespace K3Channel
{
    public class ChannelDefinition : K3Channel.IChannelDefinition
    {
        public string ChannelName { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public string ExchangeName { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public ChannelDefinition(string channelName, string hostName, int port, string exchangeName, string name, string typeName)
        {
            ChannelName = channelName;
            HostName = hostName;
            Port = port;
            ExchangeName = exchangeName;
            Name = name;
            TypeName = typeName;
        }
    }

    
}