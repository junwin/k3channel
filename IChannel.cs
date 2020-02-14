using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3Channel
{
    public delegate void OnStatusMessageEvent(object sender, ChannelState prevStatus, ChannelState newStatus, IMessage myMessage);

    public delegate void OnMessageEvent(object sender, IMessage myMessage);

    public enum ChannelState { open, opening, closed, closing, error, none }

    /// <summary>
    /// Type of queue on Rabbit MQ or other queue
    /// </summary>
    public struct MQType
    {
        public const string SUBSCRIBE = "subscribe";
        public const string UPDATE = "update";
        public const string TRADESIGNAL = "TradeSignal";
        public const string PRICES = "Prices";
        public const string TRADE = "Trade";
        public const string DEPTHSLICE = "DepthSlice";
        public const string TSBAR = "BarData";
        public const string ORDER = "Order";
        public const string EXECREPORT = "Order";
        public const string ACCOUNT = "Account";
        public const string PRODUCT = "Product";
        public const string DRIVERMESSAGE = "DriverMessage";
        public const string CONNECTIONSTATUS = "ConnectionStatusUpdate";
        public const string CONTROLLERLISTENER = "ControllerListener";
    }

    public struct MQExchanges
    {
        public const string DEFAULT = "KTDefault";
        public const string CHARTINFO = "ChartInfo";
        public const string TRADESIGNAL = "TradeSignal";
        public const string PRICES = "PRICES";
        public const string ORDERROUTING = "OrderRouting";
        public const string ACCOUNT = "Account";
        public const string PRODUCT = "Product";
    }

    public struct MQRoutingKeyPrefix
    {
        public const string TRADESIGNAL = "TR.";
        public const string PRICES = "PX.";
        public const string TRADE = "TRD.";
        public const string DEPTHSLICE = "DS.";
        public const string TSBAR = "TS.";
        public const string ORDER = "OR.";
        public const string EXECREPORT = "ER.";
        public const string ACCOUNT = "ACCT.";
        public const string PRODUCT = "PRD.";
        public const string INFO = "INFO.";
        public const string CONTROLLER = "CTRLLR.";
    }


    public interface IChannel
    {
        /// <summary>
        /// The name of the queue or topic
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Server Address - depends on channel type but usually an
        /// ip address or azure endpoint
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Port if required
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Open access to broker
        /// </summary>
        void Open();

        /// <summary>
        /// Close access to broker
        /// </summary>
        void Close();

        /// <summary>
        /// Send/publish a message
        /// </summary>
        /// <param name="mess"></param>
        void Send(IMessage mess);

        /// <summary>
        /// Event called when a message arrives
        /// </summary>
        OnMessageEvent OnMessage { get; set; }

        /// <summary>
        /// Event when status of the connection changes
        /// </summary>
        OnStatusMessageEvent OnStatusMessage { get; set; }

        /// <summary>
        /// Begin receiving on the channel - point to point
        /// </summary>
        void Begin();

        /// <summary>
        /// Begin/subscribe to some topic,
        /// </summary>
        /// <param name="topic">topic name to subscribe</param>
        /// <param name="routingKey">Routing key to select messages</param>
        void Begin(string routingKey);

        /// <summary>
        /// Return the state  of the channel
        /// </summary>
        ChannelState State { get; set; }
    }

}
