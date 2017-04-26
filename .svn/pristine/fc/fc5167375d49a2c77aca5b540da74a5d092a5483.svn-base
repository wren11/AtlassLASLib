using System;
using System.Collections.Generic;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    public class TcTileAreaBlock
    {
        public Int32 Index { get; set; }
        public String OutputFile { get; set; }
        public TcTileGrid Area { get; private set; }
        public HashSet<TcTileFileBlock> FileBlocks { get; private set; }

        public TcTileAreaBlock(TcRectangle prmArea)
        {
            Area = new TcTileGrid(prmArea.UpperLeftX, prmArea.UpperLeftY, prmArea.LowerRightX, prmArea.LowerRightY);
            FileBlocks = new HashSet<TcTileFileBlock>();

            Index = -1;
            OutputFile = String.Empty;
        }
        //-----------------------------------------------------------------------------

        public TcTileAreaBlock(TcTileGrid prmArea)
            : this(prmArea as TcRectangle)
        {
            Area.Row = prmArea.Row;
            Area.Col = prmArea.Col;
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

            TcTileAreaBlock rhs = obj as TcTileAreaBlock;
            return Index == rhs.Index && Area == rhs.Area;
        }
        //-----------------------------------------------------------------------------

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Index.GetHashCode();
                hash = hash * 23 + Area.GetHashCode();
                return hash;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------