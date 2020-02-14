using K3Channel;
using log4net;
using System;
using System.Threading.Tasks;

namespace K3Channel
{
    public class Channel : IChannel
    {
        protected Task listenTask = null;
        protected string connectionString;

        protected OnMessageEvent onMessage = null;
        protected OnStatusMessageEvent onStatusMessage = null;
        protected string name;

        protected string host;
        protected int port;

        protected ChannelState state = ChannelState.none;
        protected ILog log;

        public virtual void Begin(string routingKey)
        {
            throw new NotImplementedException();
        }

        public virtual void Begin()
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual string Host
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public virtual OnMessageEvent OnMessage
        {
            get
            {
                return onMessage;
            }
            set
            {
                onMessage = value;
            }
        }

        public virtual OnStatusMessageEvent OnStatusMessage
        {
            get
            {
                return onStatusMessage;
            }
            set
            {
                onStatusMessage = value;
            }
        }

        public virtual void Open()
        {
            throw new NotImplementedException();
        }

        public virtual int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        public virtual void Send(IMessage mess)
        {
            throw new NotImplementedException();
        }

        public virtual ChannelState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    if (onStatusMessage != null)
                    {
                        onStatusMessage(this, state, value, null);
                    }
                    state = value;
                }
            }
        }
    }
}