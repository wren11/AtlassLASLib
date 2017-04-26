using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.3 PDRF4
    /// </summary>
    public class TcLasPoint4 : TcLasPointBase
    {
        [DisplayName("Intensity")]
        public UInt16 Intensity { get; protected set; }

        [DisplayName("Classification")]
        public Byte Classification { get; protected set; }

        [DisplayName("User Data")]
        public Byte UserData { get; protected set; }

        [DisplayName("Scan Direction")]
        public Byte ScanDirection { get; protected set; }

        [DisplayName("Scan Angle Rank")]
        public SByte ScanAngleRank { get; protected set; }

        [DisplayName("Point Source Id")]
        public UInt16 PointSourceID { get; protected set; }

        [DisplayName("GPS Time")]
        public Double GPSTime { get; protected set; }

        [DisplayName("WPDI")]
        public Byte WPDI { get; set; }

        [DisplayName("WF Offset")]
        public UInt64 WFOffset { get; set; }

        [DisplayName("WF Size")]
        public UInt32 WFPacketSize { get; set; }

        [DisplayName("Return WF Location")]
        public Single WFReturnLocation { get; set; }

        [DisplayName("X(t)")]
        public Single WFXt { get; set; }

        [DisplayName("Y(t)")]
        public Single WFYt { get; set; }

        [DisplayName("Z(t)")]
        public Single WFZt { get; set; }

        public TcLasPoint4() { }

        public TcLasPoint4(TsLasPoint4 prmPoint, TsLasHeader13 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.Classification, prmPoint.UserData,
            prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank, prmPoint.GPSTime,
            prmPoint.WPDI, prmPoint.WFOffset, prmPoint.WFPacketSize, prmPoint.WFReturnLocation,
            prmPoint.WFXt, prmPoint.WFYt, prmPoint.WFZt)
        {
        }

        protected TcLasPoint4(TsLasHeader13 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank, Double prmGPSTime,
            Byte prmWPDI, UInt64 prmWFOffset, UInt32 prmWFPacketSize, Single prmWFReturnLocation,
            Single prmWFXt, Single prmWFYt, Single prmWFZt)
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
            GPSTime = prmGPSTime;
            WPDI = prmWPDI;
            WFOffset = prmWFOffset;
            WFPacketSize = prmWFPacketSize;
            WFReturnLocation = prmWFReturnLocation;
            WFXt = prmWFXt;
            WFYt = prmWFYt;
            WFZt = prmWFZt;
        }
    }
}
