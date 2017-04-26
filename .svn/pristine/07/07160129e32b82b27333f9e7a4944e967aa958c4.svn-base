using System;

namespace Atlass.LAS.Lib.Types.Interface
{
    public interface TiLasHeader : ICloneable
    {
        UInt16 DoY { get; set; }
        UInt16 Year { get; set; }
        UInt16 HeaderSize { get; set; }
        UInt32 PointOffset { get; set; }
        Byte PointDataFormatID { get; set; }
        UInt16 PointDataRecordLength { get; set; }
        Double XScaleFactor { get; set; }
        Double YScaleFactor { get; set; }
        Double ZScaleFactor { get; set; }
        Double XOffset { get; set; }
        Double YOffset { get; set; }
        Double ZOffset { get; set; }
        Double MaxX { get; set; }
        Double MinX { get; set; }
        Double MaxY { get; set; }
        Double MinY { get; set; }
        Double MaxZ { get; set; }
        Double MinZ { get; set; }
        UInt16 GetPointSize();
        Int64 GetNumberOfPoints();
        TiLasHeader Duplicate();
    }
}
