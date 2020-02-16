using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3Channel
{
    public interface IChannelFactory
    {
        IChannel Create(string channelName);

        IChannel Create(string exchangeName, string name);

        IChannel Create(string channelName, string exchangeName, string name);

        Dictionary<string, IChannelDefinition> ChannelDefinitions { get; set; }
    }
}
