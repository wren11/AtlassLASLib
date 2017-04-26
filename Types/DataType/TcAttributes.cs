using System;

namespace Atlass.LAS.Lib.Types.DataType
{
    public class ShortName : System.Attribute
    {
        private String m_ShortName;
        public ShortName(String prmValue)
        {
            m_ShortName = prmValue;
        }
        public String Value { get { return m_ShortName; } }
    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------
