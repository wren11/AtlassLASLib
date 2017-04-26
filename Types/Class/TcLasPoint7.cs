using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.4 PDRF7
    /// </summary>
    public class TcLasPoint7 : TcLasPoint6
    {
        [DisplayName("Red")]
        public UInt16 Red { get; protected set; }

        [DisplayName("Green")]
        public UInt16 Green { get; protected set; }

        [DisplayName("Blue")]
        public UInt16 Blue { get; protected set; }

        public TcLasPoint7() { }

        public TcLasPoint7(TsLasPoint7 prmPoint, TsLasHeader14 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.BitMask, prmPoint.Classification, prmPoint.ScanAngleRank,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.GPSTime,
            prmPoint.Red, prmPoint.Green, prmPoint.Blue)
        {
        }

        public TcLasPoint7(TsLasHeader14 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, UInt16 prmBitMask, Byte prmClassification,
            UInt16 prmScanAngleRank, Byte prmUserData, UInt16 prmPointSourceID, Double prmGPSTime,
            UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue)
            : base(prmHeader, prmX, prmY, prmZ,
            prmIntensity, prmBitMask, prmClassification, prmScanAngleRank,
            prmUserData, prmPointSourceID, prmGPSTime)
        {
            Red = prmRed;
            Green = prmGreen;
            Blue = prmBlue;
        }
    }
}
