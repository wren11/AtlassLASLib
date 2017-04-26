using System;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Operations.Types.Class
{
    public class TcLasTilingTaskInfo : TiLasTaskInfo
    {
        public Int32 Id { get; set; }
        public String LasFile { get; set; }
        public TeTaskType Type { get; set; }
        public TeTaskStatus Status { get; set; }
        public Int32 Progress { get; set; }
        public String Error { get; set; }
        public TiLasHeader LasHeader { get; set; }
    }
}
