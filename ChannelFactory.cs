using K3Channel;
using log4net;
using System.Collections.Generic;

namespace K3Channel
{
    public class ChannelFactory : IChannelFactory
    {
        private IChannelDefinitionSettings settings;
        private ILog log;
        private Dictionary<string, IChannelDefinition> channelDefinitions;

        public ChannelFactory(IChannelDefinitionSettings settings, ILog log)
        {
            this.log = log;
            this.settings = settings;
            channelDefinitions = new Dictionary<string, IChannelDefinition>();
            foreach (var def in settings.ChannelDefinitions)
            {
                if (!channelDefinitions.ContainsKey(def.ChannelName))
                {
                    channelDefinitions.Add(def.ChannelName, def);
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
            if (channelDefinitions.ContainsKey(channelName))
            {
                var modelDef = channelDefinitions[channelName];
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
            if (channelDefinitions.ContainsKey(channelName))
            {
                return ConstructChannel(channelDefinitions[channelName]);
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