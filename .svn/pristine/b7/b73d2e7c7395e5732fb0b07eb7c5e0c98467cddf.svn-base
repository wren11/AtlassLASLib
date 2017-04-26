using System;

namespace Atlass.LAS.Lib.Types
{
    public class TcMessageEventArgs : EventArgs
    {
        public String Message { get; private set; }

        public TcMessageEventArgs(String prmMessage)
        {
            Message = prmMessage;
        }
    }
    //-----------------------------------------------------------------------------

    public class TcErrorEventArgs : EventArgs
    {
        public String Error { get; private set; }
        public Exception Ex { get; private set; }

        public TcErrorEventArgs(String prmError, Exception prmEx)
        {
            Error = prmError;
            Ex = prmEx;
        }
    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
