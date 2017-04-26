using System;
using System.ComponentModel;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Types.Class
{
    /// <summary>
    /// Las 1.2 PDRF1
    /// </summary>
    public class TcLasPoint1 : TcLasPoint0
    {
        [DisplayName("GPS Time")]
        public Double GPSTime { get; protected set; }

        public TcLasPoint1() { }

        protected TcLasPoint1(TsLasHeader12 prmHeader, Double prmX, Double prmY, Double prmZ,
            UInt16 prmIntensity, Byte prmClassification, Byte prmUserData,
            UInt16 prmPointSourceID, Byte prmBitMask, SByte prmScanAngleRank, Double prmGPSTime)
            : base(prmHeader, prmX, prmY, prmZ, prmIntensity, prmClassification,
            prmUserData, prmPointSourceID, prmBitMask, prmScanAngleRank)
        {
            GPSTime = prmGPSTime;
        }

        public TcLasPoint1(TsLasPoint1 prmPoint, TsLasHeader12 prmHeader)
            : this(prmHeader, prmPoint.X, prmPoint.Y, prmPoint.Z, prmPoint.Intensity, prmPoint.Classification,
            prmPoint.UserData, prmPoint.PointSourceID, prmPoint.BitMask, prmPoint.ScanAngleRank, prmPoint.GPSTime)
        {
        }
    }
}
