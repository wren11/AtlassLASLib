using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.4 PDRF6
    /// </summary>
    public class TcLasPoint6 : TcLasPointBase
    {
        [DisplayName("Intensity")]
        public UInt16 Intensity { get; protected set; }

        [DisplayName("Classification Flags")]
        public Byte ClassificationFlags { get; protected set; }

        [DisplayName("Scan Direction")]
        public Byte ScanDirection { get; protected set; }

        [DisplayName("Scanner Channel")]
        public Byte ScannerChannel { get; protected set; }

        [DisplayName("Classification")]
        public Byte Classification { get; protected set; }

        [DisplayName("Scan Angle Rank")]
        public UInt16 ScanAngleRank { get; protected set; }

        [DisplayName("User Data")]
        public Byte UserData { get; protected set; }

        [DisplayName("Point Source Id")]
        public UInt16 PointSourceID { get; protected set; }

        [DisplayName("GPS Time")]
        public Double GPSTime { get; protected set; }

        public TcLasPoint6() { }

        public TcLasPoint6(TsLasPoint6 prmPoint, TsLasHeader14 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.BitMask, prmPoint.Classification, prmPoint.ScanAngleRank,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.GPSTime)
        {
        }

        public TcLasPoint6(TsLasHeader14 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, UInt16 prmBitMask, Byte prmClassification,
            UInt16 prmScanAngleRank, Byte prmUserData, UInt16 prmPointSourceID, Double prmGPSTime)
        {
            X = prmHeader.XOffset + prmX * prmHeader.XScaleFactor;
            Y = prmHeader.YOffset + prmY * prmHeader.YScaleFactor;
            Z = prmHeader.ZOffset + prmZ * prmHeader.ZScaleFactor;
            Intensity = prmIntensity;
            Classification = prmClassification;
            ScanAngleRank = prmScanAngleRank;
            UserData = prmUserData;
            PointSourceID = prmPointSourceID;
            GPSTime = prmGPSTime;
            ReturnNumber = (Byte)(prmBitMask & 15);
            NumberOfReturns = (Byte)((prmBitMask & 240) >> 4);
            ClassificationFlags = (Byte)(prmBitMask & 3840 >> 8);
            ScannerChannel = (Byte)(prmBitMask & 12288 >> 12);
            ScanDirection = (Byte)(prmBitMask & 16384 >> 14);
            EdgeOfFlightLine = (Byte)(prmBitMask & 32768 >> 15);
        }
    }
}
