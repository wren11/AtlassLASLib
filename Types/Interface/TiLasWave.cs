using System;

namespace Atlass.LAS.Lib.Types.Interface
{
    public interface TiLasWave : TiLasPoint
    {
        Byte WPDI { get; set; }
        UInt64 WFOffset { get; set; }
        UInt32 WFPacketSize { get; set; }
        Single WFReturnLocation { get; set; }
        Single WFXt { get; set; }
        Single WFYt { get; set; }
        Single WFZt { get; set; }
    }
}
