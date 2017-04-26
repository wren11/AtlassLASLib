using System;
using System.Collections.Generic;
using Atlass.LAS.Lib.Types.Class;

namespace Atlass.LAS.Lib.Global
{
    static public class TcConstants
    {
        public const Single TorNullValue32Bit = -100000.0f;
        public const Byte TorNullValue8Bit = 255;
        public const Single MaxToleranceForFlatPoints = 0.2f;
        public const Int64 MaxBytesToLoadInTorBlock = 50000000;
        public const Int32 MaxLasPointsToProcessAtOnce = 1000;
        
        // LAS format specification.
        public const Int32 DefaultLasSpecIndex = 1;
        public static readonly List<TcLasFormatSpec> LasSpecs = new List<TcLasFormatSpec>(10)
        {
            new TcLasFormatSpec("LAS Spec 1.2", "1.2", 0, "Basic Point Data Record Format (PDRF 0)", false),
            new TcLasFormatSpec("", "1.2", 1, "PDRF 0 + GPS Time", false),
            new TcLasFormatSpec("", "1.2", 2, "PDRF 0 + RGB", false),
            new TcLasFormatSpec("", "1.2", 3, "PDRF 1 + RGB", false),
            new TcLasFormatSpec("LAS Spec 1.3", "1.3", 4, "Format 1 + Waveform in .wdp", true),
            new TcLasFormatSpec("", "1.3", 5, "Format 3 + Waveform in .wdp", true),
            new TcLasFormatSpec("LAS Spec 1.4", "1.4", 6, "New Basic PDRF in Spec 1.4 + GPS Time", false),
            new TcLasFormatSpec("", "1.4", 7, "PDRF 6 + RGB", false),
            new TcLasFormatSpec("", "1.4", 8, "PDRF 7 + NIR", false),
            new TcLasFormatSpec("", "1.4", 9, "PDRF 6 + Waveform", true),
            new TcLasFormatSpec("", "1.4", 10, "PDRF 7 + Waveform", true)
        };
    }
}
