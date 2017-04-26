using System;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    public class TcTileGrid : TcRectangle
    {
        public Int32 Row { get; set; }
        public Int32 Col { get; set; }

        #region Constructors / Destructor

        public TcTileGrid(Double prmUpperLeftX, Double prmUpperLeftY, Double prmLowerRightX, Double prmLowerRightY)
            : base(prmUpperLeftX, prmUpperLeftY, prmLowerRightX, prmLowerRightY)
        {
        }

        public TcTileGrid(TcPoint prmUpperLeft, TcPoint prmLowerRight)
            : base(prmUpperLeft, prmLowerRight)
        {
        }

        public TcTileGrid(TcTileGrid prmTileGrid)
            : base(prmTileGrid)
        {
            Row = prmTileGrid.Row;
            Col = prmTileGrid.Col;
        }

        #endregion Constructors / Destructor

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

            TcTileGrid rhs = obj as TcTileGrid;
            return base.Equals(obj) && Row == rhs.Row && Col == rhs.Col;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = base.GetHashCode();
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Col.GetHashCode();
                return hash;
            }
        }
    }
}
