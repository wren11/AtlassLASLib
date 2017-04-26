using System;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// <summary>
    /// Las 1.4 PDRF 7
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 36)]
    public struct TsLasPoint7 : TiLasPoint, TiLasGPS, TiLasRGB
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 Z { get; set; }
        public UInt16 Intensity { get; set; }
        public Byte ReturnMask { get; set; }
        public Byte BitMask { get; set; }
        public Byte Classification { get; set; }
        public UInt16 ScanAngleRank { get; set; }
        public Byte UserData { get; set; }
        public UInt16 PointSourceID { get; set; }
        public Double GPSTime { get; set; }
        public UInt16 Red { get; set; }
        public UInt16 Green { get; set; }
        public UInt16 Blue { get; set; }

        public Byte ReturnNumber() { return (Byte)(ReturnMask & 15); }
        public Byte NumberOfReturns() { return (Byte)((ReturnMask & 240) >> 4); }
    }
}
