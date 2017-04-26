using System;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// Las 1.4 PDRF 6
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 30)]
    public struct TsLasPoint6 : TiLasPoint, TiLasGPS
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 Z { get; set; }
        public UInt16 Intensity { get; set; }
        public Byte ReturnMask { get; set; }
        public Byte BitMask { get; set; }
        public Byte Classification { get; set; }
        public Byte UserData { get; set; }
        public UInt16 ScanAngleRank { get; set; }
        public UInt16 PointSourceID { get; set; }
        public Double GPSTime { get; set; }

        public Byte ReturnNumber() { return (Byte)(ReturnMask & 15); }
        public Byte NumberOfReturns() { return (Byte)((ReturnMask & 240) >> 4); }
    }
}
