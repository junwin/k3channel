using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3Channel;
using System.Collections.Generic;

namespace K3Channel
{
    public class ChannelDefinitionSettings : K3Channel.IChannelDefinitionSettings
    {
        private List<IChannelDefinition> channelDefinitions;

        public ChannelDefinitionSettings()
        {
            channelDefinitions = new List<IChannelDefinition>();
            
        }

        public List<IChannelDefinition> ChannelDefinitions
        {
            get { return channelDefinitions; }
            set { channelDefinitions = value; }
        }

        
    }
}
