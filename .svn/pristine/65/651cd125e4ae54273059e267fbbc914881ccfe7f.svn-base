using System;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// <summary>
    /// Las 1.4 header with wave information and large number of points support.
    /// N.B. Converting members between arrays to property breaks the sequential layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 375)]
    public struct TsLasHeader14 : TiLasHeader
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
        public UInt32 LegNumberOfPointRecords { get; set; }
        public UInt32 LegNumberofPointsByReturn1 { get; set; }
        public UInt32 LegNumberofPointsByReturn2 { get; set; }
        public UInt32 LegNumberofPointsByReturn3 { get; set; }
        public UInt32 LegNumberofPointsByReturn4 { get; set; }
        public UInt32 LegNumberofPointsByReturn5 { get; set; }
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

        // Additions from Las 1.2 version.
        public UInt64 StartOfWaveFormData { get; set; }

        // Additions from Las 1.3 version.
        public UInt64 StartOfFirstExtendedVLR { get; set; }
        public UInt32 NumberOfExtendedVLR { get; set; }
        public UInt64 NumberOfPointRecords { get; set; }
        public UInt64 NumberofPointsByReturn1 { get; set; }
        public UInt64 NumberofPointsByReturn2 { get; set; }
        public UInt64 NumberofPointsByReturn3 { get; set; }
        public UInt64 NumberofPointsByReturn4 { get; set; }
        public UInt64 NumberofPointsByReturn5 { get; set; }
        public UInt64 NumberofPointsByReturn6 { get; set; }
        public UInt64 NumberofPointsByReturn7 { get; set; }
        public UInt64 NumberofPointsByReturn8 { get; set; }
        public UInt64 NumberofPointsByReturn9 { get; set; }
        public UInt64 NumberofPointsByReturn10 { get; set; }
        public UInt64 NumberofPointsByReturn11 { get; set; }
        public UInt64 NumberofPointsByReturn12 { get; set; }
        public UInt64 NumberofPointsByReturn13 { get; set; }
        public UInt64 NumberofPointsByReturn14 { get; set; }
        public UInt64 NumberofPointsByReturn15 { get; set; }

        public UInt16 GetPointSize()
        {
            switch (PointDataFormatID)
            {
                case 6:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint6));

                case 7:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint7));

                case 8:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint8));

                case 9:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint9));

                case 10:
                    return (UInt16)Marshal.SizeOf(typeof(TsLasPoint10));

                default:
                    throw new ArgumentException("Invalid point format for this LAS file version");
            }
        }

        public Int64 GetNumberOfPoints()
        {
            return (Int64)NumberOfPointRecords;
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
