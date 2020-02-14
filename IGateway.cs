using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3Channel
{

    public struct GatewayStatus
    {
        public const string CLOSED = "CLOSED";
        public const string CLOSING = "CLOSING";
        public const string OPEN = "OPEN";
        public const string OPENING = "OPENING";
        public const string NONE = "NOTKNOWN";
    }

    public interface IGateway
    {
        IChannel Channel { get; set; }

        void Send(IMessage message);

        void Accept(IMessage message);

        void Reject(IMessage message);

        event OnMessageEvent OnMessage;

        event OnStatusMessageEvent OnStatusMessage;

        void Begin();

        void Subscribe(string routingKey);

        void Open();

        void Close();
    }
}
