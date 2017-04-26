using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.3 PDRF5
    /// </summary>
    public class TcLasPoint5 : TcLasPoint4
    {
        [DisplayName("Red")]
        public UInt16 Red { get; protected set; }

        [DisplayName("Green")]
        public UInt16 Green { get; protected set; }

        [DisplayName("Blue")]
        public UInt16 Blue { get; protected set; }

        public TcLasPoint5() { }

        public TcLasPoint5(TsLasPoint5 prmPoint, TsLasHeader13 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.Classification, prmPoint.UserData,
            prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank, prmPoint.GPSTime,
            prmPoint.WPDI, prmPoint.WFOffset, prmPoint.WFPacketSize, prmPoint.WFReturnLocation,
            prmPoint.WFXt, prmPoint.WFYt, prmPoint.WFZt, prmPoint.Red, prmPoint.Green, prmPoint.Blue)
        {
        }

        protected TcLasPoint5(TsLasHeader13 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank, Double prmGPSTime,
            Byte prmWPDI, UInt64 prmWFOffset, UInt32 prmWFPacketSize, Single prmWFReturnLocation,
            Single prmWFXt, Single prmWFYt, Single prmWFZt, UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue)
            : base(prmHeader, prmX, prmY, prmZ, prmIntensity, prmClassification,
            prmUserData, prmPointSourceID, prmBitMask, prmScanAngleRank, prmGPSTime,
            prmWPDI, prmWFOffset, prmWFPacketSize, prmWFReturnLocation, prmWFXt, prmWFYt, prmWFZt)
        {
            Red = prmRed;
            Green = prmGreen;
            Blue = prmBlue;
        }
    }
}
