using System;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// <summary>
    /// Las 1.2 header with no wave information.
    /// N.B. Converting members between arrays to property breaks the sequential layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 227)]
    public struct TsLasHeader12 : TiLasHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Char[] FileSignature;
        public UInt16 SourceID;
        public UInt16 GlobalEncoding;
        public UInt32 GUID1;
        public UInt16 GUID2;
        public UInt16 GUID3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Char[] GUID4;
        public Byte VersionMajor;
        public Byte VersionMinor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Char[] SystemID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Char[] GeneratingSoftware;
        public UInt16 DoY { get; set; }
        public UInt16 Year { get; set; }
        public UInt16 HeaderSize { get; set; }
        public UInt32 PointOffset { get; set; }
        public UInt32 NumberOfVariableLengthRecords { get; set; }
        public Byte PointDataFormatID { get; set; }
        public UInt16 PointDataRecordLength { get; set; }
        public UInt32 NumberOfPointRecords { get; set; }
        public UInt32 NumberofPointsByReturn1 { get; set; }
        public UInt32 NumberofPointsByReturn2 { get; set; }
        public UInt32 NumberofPointsByReturn3 { get; set; }
        public UInt32 NumberofPointsByReturn4 { get; set; }
        public UInt32 NumberofPointsByReturn5 { get; set; }
        public Double XScaleFactor { get; set; }
        public Double YScaleFactor { get; set; }
        public Double ZScaleFactor { get; set; }
        public Double XOffset { get; set; }
        public Double YOffset { get; set; }
        public Double ZOffset { get; set; }
        public Double MaxX { get; set; }
        public Double MinX { get; set; }
        public Double MaxY { get; set; }
        public Double MinY { get; set; }
        public Double MaxZ { get; set; }
        public Double MinZ { get; set; }

        public UInt16 GetPointSize()
        {
            switch (PointDataFormatID)
            {
                case 0:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint0));

                case 1:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint1));

                case 2:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint2));

                case 3:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint3));

                default:
                    throw new ArgumentException("Invalid point format for this LAS file version");
            }
        }

        public Int64 GetNumberOfPoints()
        {
            return NumberOfPointRecords;
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        public TiLasHeader Duplicate()
        {
            return (TiLasHeader)Clone();
        }

        #endregion ICloneable Members
    }
}
