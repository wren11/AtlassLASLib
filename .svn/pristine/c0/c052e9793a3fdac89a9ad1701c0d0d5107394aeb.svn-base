using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.4 PDRF8
    /// </summary>
    public class TcLasPoint8 : TcLasPoint7
    {
        [DisplayName("Near Infrared")]
        public UInt16 NIR { get; protected set; }

        public TcLasPoint8() { }

        public TcLasPoint8(TsLasPoint8 prmPoint, TsLasHeader14 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z,
            prmPoint.Intensity, prmPoint.BitMask, prmPoint.Classification, prmPoint.ScanAngleRank,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.GPSTime,
            prmPoint.Red, prmPoint.Green, prmPoint.Blue, prmPoint.NIR)
        {
        }

        public TcLasPoint8(TsLasHeader14 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, UInt16 prmBitMask, Byte prmClassification,
            UInt16 prmScanAngleRank, Byte prmUserData, UInt16 prmPointSourceID, Double prmGPSTime,
            UInt16 prmRed, UInt16 prmGreen, UInt16 prmBlue, UInt16 prmNIR)
            : base(prmHeader, prmX, prmY, prmZ,
            prmIntensity, prmBitMask, prmClassification, prmScanAngleRank,
            prmUserData, prmPointSourceID, prmGPSTime, prmRed, prmGreen, prmBlue)
        {
            NIR = prmNIR;
        }
    }
}
