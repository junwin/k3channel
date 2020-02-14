using K3Channel;
using log4net;
using System.Collections.Generic;

namespace K3Channel
{
    public class MemoryChannelFactory
    {
        private Dictionary<string, IChannel> channels = null;
        private static MemoryChannelFactory s_instance = null;
        private static object s_token = new object();

        public static MemoryChannelFactory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new MemoryChannelFactory();
                    }
                }
            }
            return s_instance;
        }

        protected MemoryChannelFactory()
        {
            channels = new Dictionary<string, IChannel>();
        }

        public IChannel GetChannel(string name, ILog log)
        {
            IChannel retChannel = null;
            if (channels.ContainsKey(getKey("Default", name)))
            {
                retChannel = channels[getKey("Default", name)];
            }
            else
            {
                retChannel = new MemoryChannel(log);
                retChannel.Name = name;
                channels.Add(getKey("Default", name), retChannel);
            }
            return retChannel;
        }

        private string getKey(string exchange, string name)
        {
            return string.Format("{0}:{1}", exchange, name);
        }
    }
}