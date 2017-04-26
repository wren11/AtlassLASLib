using System;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// <summary>
    /// Las 1.2 PDRF 0
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
    public struct TsLasPoint0 : TiLasPoint
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 Z { get; set; }
        public UInt16 Intensity { get; set; }
        public Byte BitMask { get; set; }
        public Byte Classification { get; set; }
        public SByte ScanAngleRank { get; set; }
        public Byte UserData { get; set; }
        public UInt16 PointSourceID { get; set; }

        public Byte ReturnNumber() { return (Byte)(BitMask & 7); }
        public Byte NumberOfReturns() { return (Byte)((BitMask & 56) >> 3); }
    }
}
