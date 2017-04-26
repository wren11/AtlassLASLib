using System;

namespace Atlass.LAS.Lib.Types.Class
{
    public class TcLasFormatSpec
    {
        public String Specification { get; private set; }
        public String Version { get; private set; }
        public Int32 Value { get; private set; }
        public String Description { get; private set; }
        public Boolean FullWave { get; private set; }

        public TcLasFormatSpec(String prmSpec, String prmVersion, Int32 prmValue, String prmDesc, Boolean prmHasWF)
        {
            Specification = prmSpec;
            Version = prmVersion;
            Value = prmValue;
            Description = prmDesc;
            FullWave = prmHasWF;
        }
        //------------------------------------------------------------------

    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------
