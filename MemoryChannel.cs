using K3Channel;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace K3Channel
{
    public class MemoryChannel : Channel
    {
        private BlockingCollection<IMessage> messages = new BlockingCollection<IMessage>();

        private IChannel channel = null;

        public MemoryChannel(ILog log)
        {
            this.log = log;
        }

        public IChannel Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public override void Open()
        {
            State = ChannelState.open;
        }

        public override void Close()
        {
            if (listenTask != null)
            {
                listenTask.Dispose();
                listenTask = null;
            }
            State = ChannelState.closed;
        }

        public override void Begin()
        {
            if (listenTask == null)
            {
                listenTask = Task.Run(() =>
                {
                    IMessage msg = null;
                    while (!messages.IsCompleted)
                    {
                        try
                        {
                            msg = messages.Take();
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine("Adding was compeleted!");
                            break;
                        }
                        if (onMessage != null)
                        {
                            onMessage(this, msg);
                        }
                    }

                    Console.WriteLine("\r\nNo more items to take. Press the Enter key to exit.");
                });
            }
        }

        public override void Send(IMessage msg)
        {
            try
            {
                messages.Add(msg);
                //messages.CompleteAdding();
            }
            catch
            {
            }
        }
    }
}