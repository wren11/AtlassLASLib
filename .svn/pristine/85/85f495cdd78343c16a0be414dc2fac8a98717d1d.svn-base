using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.2 PDRF 0
    /// </summary>
    public class TcLasPoint0 : TcLasPointBase
    {
        [DisplayName("Intensity")]
        public UInt16 Intensity { get; protected set; }

        [DisplayName("Classification")]
        public Byte Classification { get; protected set; }

        [DisplayName("User Data")]
        public Byte UserData { get; protected set; }

        [DisplayName("Point Source Id")]
        public UInt16 PointSourceID { get; protected set; }

        [DisplayName("Scan Direction")]
        public Byte ScanDirection { get; protected set; }

        [DisplayName("Scan Angle Rank")]
        public SByte ScanAngleRank { get; protected set; }

        public TcLasPoint0() { }

        public TcLasPoint0(TsLasPoint0 prmPoint, TsLasHeader12 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z, prmPoint.Intensity, prmPoint.Classification,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank)
        {
        }

        protected TcLasPoint0(TsLasHeader12 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank)
        {
            X = prmHeader.XOffset + prmX * prmHeader.XScaleFactor;
            Y = prmHeader.YOffset + prmY * prmHeader.YScaleFactor;
            Z = prmHeader.ZOffset + prmZ * prmHeader.ZScaleFactor;
            Intensity = prmIntensity;
            Classification = prmClassification;
            UserData = prmUserData;
            PointSourceID = prmPointSourceID;
            ReturnNumber = (byte)(prmBitMask & 7);
            NumberOfReturns = (byte)((prmBitMask & 56) >> 3);
            ScanDirection = (byte)((prmBitMask & 64) >> 6);
            EdgeOfFlightLine = (byte)((prmBitMask & 128) >> 7);
            ScanAngleRank = prmScanAngleRank;
        }
    }
}
