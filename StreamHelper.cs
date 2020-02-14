using K3Channel;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace K3Channel
{
    public class StreamHelper
    {
        static private readonly string strTok = "<XX$$";

        static private readonly int TokLen = 5;
        static private readonly string strEndTok = "$$XX>";
        static private readonly int EndTokLen = 5;
        static private readonly string strDelim = "/";

        public StreamHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static public long GetMessage(ref string myData, out string myMsg, out string myLabel, out string myCLientID)
        {
            myMsg = "";
            myLabel = "";
            myCLientID = "";
            try
            {
                // find the begining of a valid message by searching for the
                // start token
                int myPos = myData.IndexOf(strTok);
                if (myPos == -1)
                {
                    // there is no begin token, hence no usuable message
                    Exception myE = new Exception("GetMessage:no begin token, hence no usuable message");
                    throw myE;
                }
                if (myPos != 0)
                {
                    // potential issue since we should always start on a
                    // message boundary
                    //Exception myE = new Exception("GetMessage:begin token not first character, hence potential  message problems");
                    //throw myE;
                    return -1;
                }

                int myEndPos = myData.IndexOf(strEndTok, myPos);
                if (myEndPos == -1)
                {
                    // there is no begin token, hence no usuable message
                    Exception myE = new Exception("GetMessage:no end token, hence no usuable message");
                    throw myE;
                }
                // Now get the length and lable
                int myStart = myPos + TokLen;            // start of label skip token
                string parmData = myData.Substring(myStart, myEndPos - myStart);
                string[] parms = parmData.Split(strDelim[0]);
                if (parms.Length < 3)
                {
                    // there is no begin token, hence no usuable message
                    Exception myE = new Exception("GetMessage:no params, hence no usuable message");
                    throw myE;
                }
                myLabel = parms[0];

                myCLientID = parms[1];

                long myLen = long.Parse(parms[2]);

                // get the message data
                myStart = myEndPos + EndTokLen;

                if (myStart + myLen > myData.Length)
                {
                    // we dont have the whole message
                    return -1;
                }
                myMsg = myData.Substring(myStart, (int)myLen);

                //Return the number of bytes of content read i.e. *not* the message len
                return myStart + myLen;
            }
            catch
            {
            }
            return -1;
        }

        static public void WriteMsg(out string myData, ref string myMsg, string myLabel, string myClientID)
        {
            myData = strTok;
            myData += myLabel;
            myData += strDelim;
            myData += myClientID;
            myData += strDelim;
            myData += myMsg.Length.ToString();
            myData += strEndTok;
            myData += myMsg;
        }

        static public void WriteMsg(out string myData, IMessage myKaiMsg)
        {
            myData = strTok;
            myData += myKaiMsg.Label;
            myData += strDelim;
            myData += myKaiMsg.ClientID;
            myData += strDelim;
            myData += myKaiMsg.Data.Length.ToString();
            myData += strEndTok;
            myData += myKaiMsg.Data;
        }

        static public string XGetAsString(string[] myStringArray)
        {
            Stream memStream = new MemoryStream();
            BinaryFormatter myBinaryFormatter = new BinaryFormatter();
            myBinaryFormatter.Serialize(memStream, myStringArray);

            byte[] byteArray = new byte[memStream.Length];

            memStream.Position = 0;
            memStream.Read(byteArray, 0, byteArray.Length);
            memStream.Flush();
            /*
            string myTemp = System.Text.ASCIIEncoding.ASCII.GetString(byteArray);

            byte[] byteArray2 = new byte[myTemp.Length];
            byteArray2 = System.Text.ASCIIEncoding.ASCII.GetBytes(myTemp.ToString());
            for (int myIndex = 0; myIndex < myTemp.Length; myIndex++)
            {
                if (byteArray[myIndex] != byteArray2[myIndex])
                {
                    // bang
                }
            }

            string[] myTempAr;
            GetStringArray(out myTempAr, myTemp);
             */

            return System.Text.ASCIIEncoding.ASCII.GetString(byteArray);
        }

        static public string XXGetAsString(string[] myStringArray)
        {
            Stream memStream = new MemoryStream();
            BinaryFormatter myBinaryFormatter = new BinaryFormatter();
            myBinaryFormatter.Serialize(memStream, myStringArray);

            byte[] byteArray = new byte[memStream.Length];

            memStream.Position = 0;
            memStream.Read(byteArray, 0, byteArray.Length);
            memStream.Flush();
            System.Text.Encoding en1252 = System.Text.Encoding.GetEncoding(1252);
            /*
            string myTemp = System.Text.ASCIIEncoding.ASCII.GetString(byteArray);

            byte[] byteArray2 = new byte[myTemp.Length];
            byteArray2 = System.Text.ASCIIEncoding.ASCII.GetBytes(myTemp.ToString());
            for (int myIndex = 0; myIndex < myTemp.Length; myIndex++)
            {
                if (byteArray[myIndex] != byteArray2[myIndex])
                {
                    // bang
                }
            }

            string[] myTempAr;
            GetStringArray(out myTempAr, myTemp);
             */

            return en1252.GetString(byteArray);
        }

        static public string GetAsString(string[] myStringArray)
        {
            string myString = "";
            bool bFirst = true;
            foreach (string myVal in myStringArray)
            {
                if (bFirst)
                {
                    bFirst = false;
                }
                else
                {
                    myString += "\t";
                }
                myString += myVal;
            }

            return myString;
        }

        static public void XGetStringArray(out string[] myStringArray, string myData)
        {
            BinaryFormatter myBinaryFormatter = new BinaryFormatter();
            byte[] byteArray = new byte[myData.Length];
            byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(myData.ToString());

            Stream memStream = new MemoryStream(byteArray);
            memStream.Flush();
            memStream.Position = 0;

            myStringArray = new string[] { };
            myStringArray = (string[])myBinaryFormatter.Deserialize(memStream);
            memStream.Close();
        }

        static public void XXGetStringArray(out string[] myStringArray, string myData)
        {
            BinaryFormatter myBinaryFormatter = new BinaryFormatter();
            byte[] byteArray = new byte[myData.Length];
            System.Text.Encoding en1252 = System.Text.Encoding.GetEncoding(1252);

            byteArray = en1252.GetBytes(myData.ToString());

            Stream memStream = new MemoryStream(byteArray);
            memStream.Flush();
            memStream.Position = 0;

            myStringArray = new string[] { };
            myStringArray = (string[])myBinaryFormatter.Deserialize(memStream);
            memStream.Close();
        }

        static public void GetStringArray(out string[] myStringArray, string myData)
        {
            myStringArray = myData.Split('\t');
        }
    }
}