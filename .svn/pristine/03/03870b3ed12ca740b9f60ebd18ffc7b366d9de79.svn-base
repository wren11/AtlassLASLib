using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.2 PDRF2
    /// </summary>
    public class TcLasPoint2 : TcLasPoint0
    {
        [DisplayName("Red")]
        public UInt16 Red { get; set; }

        [DisplayName("Green")]
        public UInt16 Green { get; set; }

        [DisplayName("Blue")]
        public UInt16 Blue { get; set; }

        public TcLasPoint2() { }

        protected TcLasPoint2(TsLasHeader12 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank,
            UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue)
            : base(prmHeader, prmX, prmY, prmZ, prmIntensity, prmClassification,
            prmUserData, prmPointSourceID, prmBitMask, prmScanAngleRank)
        {
            Red = prmRed;
            Green = prmGreen;
            Blue = prmBlue;
        }

        public TcLasPoint2(TsLasPoint2 prmPoint, TsLasHeader12 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z, prmPoint.Intensity, prmPoint.Classification,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank,
            prmPoint.Red, prmPoint.Green, prmPoint.Blue)
        {
        }
    }
}
