using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.4 PDRF10
    /// </summary>
    public class TcLasPoint10 : TcLasPoint7
    {
        [DisplayName("WPDI")]
        public Byte WPDI { get; protected set; }

        [DisplayName("WF Offset")]
        public UInt64 WFOffset { get; protected set; }

        [DisplayName("WF Size")]
        public UInt32 WFPacketSize { get; protected set; }

        [DisplayName("Return WF Location")]
        public Single WFReturnLocation { get; protected set; }

        [DisplayName("X(t)")]
        public Single WFXt { get; protected set; }

        [DisplayName("Y(t)")]
        public Single WFYt { get; protected set; }

        [DisplayName("Z(t)")]
        public Single WFZt { get; protected set; }

        public TcLasPoint10(TsLasPoint10 prmPoint, TsLasHeader14 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.BitMask, prmPoint.Classification, prmPoint.ScanAngleRank,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.GPSTime,
            prmPoint.Red, prmPoint.Green, prmPoint.Blue,
            prmPoint.WPDI, prmPoint.WFOffset, prmPoint.WFPacketSize, prmPoint.WFReturnLocation,
            prmPoint.WFXt, prmPoint.WFYt, prmPoint.WFZt)
        {
        }

        public TcLasPoint10(TsLasHeader14 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, UInt16 prmBitMask, Byte prmClassification,
            UInt16 prmScanAngleRank, Byte prmUserData, UInt16 prmPointSourceID, Double prmGPSTime,
            UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue,
            Byte prmWPDI, UInt64 prmWFOffset, UInt32 prmWFPacketSize, Single prmWFReturnLocation,
            Single prmWFXt, Single prmWFYt, Single prmWFZt)
            : base(prmHeader, prmX, prmY, prmZ,
            prmIntensity, prmBitMask, prmClassification, prmScanAngleRank,
            prmUserData, prmPointSourceID, prmGPSTime,
            prmRed, prmGreen, prmBlue)
        {
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
