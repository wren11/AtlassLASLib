using System;
using System.Runtime.InteropServices;

namespace Atlass.LAS.Lib.Types.Struct
{
    /// <summary>
    /// Point structure with XYZ and Classification
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 13)]
    public struct TsClsLasPoint
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 Z { get; set; }
        public Byte Classification { get; set; }
    }
}
