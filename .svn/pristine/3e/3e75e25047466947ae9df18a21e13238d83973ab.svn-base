using System;
using Atlass.LAS.Lib.Types;

namespace Atlass.LAS.Lib.Operations
{
    public abstract class TiOperation
    {
        public abstract event EventHandler<TcMessageEventArgs> OnMessage;
        public abstract event EventHandler<TcErrorEventArgs> OnError;
        public abstract event EventHandler<EventArgs> OnFinish;
        public abstract Double ProgressFrequency { protected get; set; }
        protected abstract void ReportMessage(String prmMessage);
        protected abstract void ReportError(String prmError, Exception ex);
        protected abstract void ReportFinished();
    }
}
