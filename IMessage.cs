using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3Channel
{
    public struct MessageType
    {
        public const string NEWORDERSINGLE = "D";
        public const string EXECUTION = "8";
        public const string REQUESTBARDATA = "BDREQ";
        public const string REQUESTPRICES = "PXREQ";
        public const string OPENPRICES = "PXREQ";
        public const string REQUESTINSTRUMENT = "INSTRREQ";
        public const string REJECT = "REJECT";
        public const string CANCELORDER = "F";
        public const string REJECTCANCEL = "9";
        public const string MODIFYORDER = "G";
        public const string REJECTMODIFYORDER = "RMO";
        public const string NOP = "NOP";
    }


    public interface IMessageHeader
    {
        string ClientID { get; set; }
        string ClientSubID { get; set; }
        string CorrelationID { get; set; }
    }
    /// <summary>
    /// Defines an internal message used
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// unique ID for this message
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Message data JSON, FIX, FIXML
        /// </summary>
        string Data
        {
            get;
            set;
        }

        /// <summary>
        /// CorrelationID used for tiing messages together - not that same as the Trade/Algo CorrelationID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        /// <summary>
        /// Message label - NewOrder,...
        /// </summary>
        string Label
        {
            get;
            set;
        }

        /// <summary>
        /// get/set TargetID (destination) for the message  
        /// </summary>
        string TargetID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set Client SubID
        /// </summary>
        string ClientSubID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set target subID
        /// </summary>
        string TargetSubID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set format - JSON, FIX, FIXML other
        /// </summary>
        string Format
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set AppSpecific  
        /// </summary>
        long AppSpecific
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set AppState  
        /// </summary>

        int AppState
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set App type  
        /// </summary>
        string AppType
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set Tag - user tag
        /// </summary>
        string Tag
        {
            get;
            set;
        }
        /// <summary>
        /// get/Set ClientID
        /// </summary>
        string ClientID
        {
            get;
            set;
        }
        /// <summary>
        /// get/Set user ID  
        /// </summary>
        string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// get set the Venue Code that the message is intended for
        /// </summary>
        string VenueCode
        {
            get;
            set;
        }

        /// <summary>
        /// get set creation time  
        /// </summary>
        string CreationTime
        {
            get;
            set;
        }
    }
}
