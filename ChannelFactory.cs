using K3Channel;
using log4net;
using System.Collections.Generic;

namespace K3Channel
{
    public class ChannelFactory : IChannelFactory
    {
 
        private ILog log;
        private Dictionary<string, IChannelDefinition> channelDefinitions;

        public Dictionary<string, IChannelDefinition> ChannelDefinitions { get => channelDefinitions; set => channelDefinitions = value; }

        public ChannelFactory(List<IChannelDefinition> settings, ILog log)
        {
            this.log = log;

            ChannelDefinitions = new Dictionary<string, IChannelDefinition>();
            foreach (var def in settings)
            {
                if (!ChannelDefinitions.ContainsKey(def.ChannelName))
                {
                    ChannelDefinitions.Add(def.ChannelName, def);
                }
                else
                {
                    log.Error("duplicate channel definition");
                }
            }
        }

        public IChannel Create(string exchangeName, string name)
        {
            string channelName = "DEFAULT";
            return Create(channelName, exchangeName, name);
        }

        public IChannel Create(string channelName, string exchangeName, string name)
        {
            if (ChannelDefinitions.ContainsKey(channelName))
            {
                var modelDef = ChannelDefinitions[channelName];
                IChannelDefinition overridenDef = new ChannelDefinition(channelName, modelDef.HostName, modelDef.Port, exchangeName, name, modelDef.TypeName);
                var channel = ConstructChannel(overridenDef);
                return channel;
            }
            else
            {
                return null;
            }
        }

        public IChannel Create(string channelName)
        {
            if (ChannelDefinitions.ContainsKey(channelName))
            {
                return ConstructChannel(ChannelDefinitions[channelName]);
            }
            else
            {
                return null;
            }
        }

        

        public IChannel ConstructChannel(IChannelDefinition def)
        {
            IChannel newChannel = null;
            switch (def.TypeName)
            {
                case "K3Channel.RMQChannel":
                    var rmqChannel = new RMQChannel(log);
                    rmqChannel.Host = def.HostName;
                    rmqChannel.Name = def.Name;
                    rmqChannel.Port = def.Port;
                    newChannel = rmqChannel;
                    break;

                case "K3Channel.MSBChannel":
                    var msbChannel = new MSBChannel(log);
                    msbChannel.Host = def.HostName;
                    msbChannel.Name = def.Name;
                    newChannel = msbChannel;
                    break;

                case "K3Channel.MSTopicChannel":
                    var msTopicChannel = new MSTopicChannel(log);
                    msTopicChannel.Host = def.HostName;
                    msTopicChannel.Name = def.Name;
                    newChannel = msTopicChannel;
                    break;

                default:
                    newChannel = MemoryChannelFactory.Instance().GetChannel(def.Name, log);
                    break;
            }

            return newChannel;
        }
    }
}