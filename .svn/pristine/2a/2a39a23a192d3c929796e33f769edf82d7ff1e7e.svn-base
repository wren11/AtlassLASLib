using System;

namespace Atlass.LAS.Lib.Types.DataType
{
    [Serializable]
    public class TcPoint
    {
        private Double m_X;
        public Double X { get { return m_X; } set { m_X = value; } }

        private Double m_Y;
        public Double Y { get { return m_Y; } set { m_Y = value; } }

        private Double m_Z;
        public Double Z { get { return m_Z; } set { m_Z = value; } }

        public TcPoint(Double prmX, Double prmY, Double prmZ = 0)
        {
            m_X = prmX;
            m_Y = prmY;
            m_Z = prmZ;
        }

        public TcPoint()
            : this(0, 0, 0)
        {
        }

        public TcPoint(TcPoint prmPoint)
        {
            m_X = prmPoint.m_X;
            m_Y = prmPoint.m_Y;
            m_Z = prmPoint.m_Z;
        }

        public Double Distance(Double prmX, Double prmY, Double prmZ = 0)
        {
            return Math.Sqrt(Math.Pow(m_X - prmX, 2) + Math.Pow(m_Y - prmY, 2) + Math.Pow(m_Z - prmZ, 2));
        }

        public Double Distance(TcPoint prmPoint)
        {
            return Distance(prmPoint.X, prmPoint.Y, prmPoint.Z);
        }
    }
}
