using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlass.LAS.Lib.Types.DataType
{
    [Serializable]
    public class TcPolygon : TcRectangle
    {
        public List<TcPoint> Points { get; private set; }

        public TcPolygon()
            : this(new List<TcPoint>())
        {
        }
        //-----------------------------------------------------------------------------

        public TcPolygon(IEnumerable<TcPoint> prmPoints)
            : this(prmPoints, 0)
        {
        }
        //-----------------------------------------------------------------------------
        
        public TcPolygon(IEnumerable<TcPoint> prmPoints, Double prmBuffer)
            : base()
        {
            Points = new List<TcPoint>();

            if (prmPoints.Count() > 0)
            {
                foreach (TcPoint point in prmPoints)
                {
                    Points.Add(new TcPoint(point));
                }

                UpperLeftX = prmPoints.Min(iter => iter.X) - prmBuffer;
                UpperLeftY = prmPoints.Max(iter => iter.Y) + prmBuffer;
                LowerRightX = prmPoints.Max(iter => iter.X) + prmBuffer;
                LowerRightY = prmPoints.Min(iter => iter.Y) - prmBuffer;
            }
        }
        //-----------------------------------------------------------------------------

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            TcPolygon rhs = obj as TcPolygon;
            return base.Equals(rhs) && Points.Count == rhs.Points.Count;
        }
        //-----------------------------------------------------------------------------

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + base.GetHashCode();
                hash = hash * 23 + Points.GetHashCode();
                return hash;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------