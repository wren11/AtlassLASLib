using System;

namespace Atlass.LAS.Lib.Types.Interface
{
    public interface TiLasGPS : TiLasPoint
    {
        Double GPSTime { get; set; }
    }
}
