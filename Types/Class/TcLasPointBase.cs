using System;

namespace Atlass.LAS.Lib.Types.Class
{
    public class TcLasPointBase
    {
        public Int64 Index { get; set; }
        
        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Z { get; set; }
        
        public Byte ReturnNumber { get; set; }
        public Byte NumberOfReturns { get; set; }
        public Byte EdgeOfFlightLine { get; set; }

        public TcLasPointBase() { }

        public TcLasPointBase(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public TcLasPointBase(double x, double y) : this(x, y, 0)
        {
        }



        public double this[int dimension]
        {
            get
            {
                if (dimension == 0)
                    return X;
                if (dimension == 1)
                    return Y;
                if (dimension == 2)
                    return Z;

                return -1;
            }
        }
    }
}
