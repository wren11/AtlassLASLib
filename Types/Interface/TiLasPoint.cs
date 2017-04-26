using System;

namespace Atlass.LAS.Lib.Types.Interface
{
    public interface TiLasPoint
    {
        Int32 X { get; set; }
        Int32 Y { get; set; }
        Int32 Z { get; set; }
        UInt16 Intensity { get; set; }
        Byte Classification { get; set; }
        Byte ReturnNumber();
        Byte NumberOfReturns();
    }
}
