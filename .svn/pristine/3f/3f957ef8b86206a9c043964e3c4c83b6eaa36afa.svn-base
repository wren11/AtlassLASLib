using System;
using System.Xml.Serialization;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    public class TcTileInfo
    {
        [XmlAttribute("Size")]
        public Int32 TileSize { get; set; }

        [XmlAttribute("Row")]
        public Int32 Row { get; set; }

        [XmlAttribute("Col")]
        public Int32 Col { get; set; }

        [XmlAttribute("Count")]
        public Int32 TileCount { get; set; }

        public TcTileInfo()
            : this(0, 0, 0)
        {
        }
        //-----------------------------------------------------------------------------

        public TcTileInfo(Int32 prmTileSize, Int32 prmRow, Int32 prmCol)
        {
            TileSize = prmTileSize;
            Row = prmRow;
            Col = prmCol;
            TileCount = prmRow * prmCol;
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

            TcTileInfo rhs = obj as TcTileInfo;
            return TileSize == rhs.TileSize && Row == rhs.Row && Col == rhs.Col && TileCount == rhs.TileCount;
        }
        //-----------------------------------------------------------------------------

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + TileSize.GetHashCode();
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Col.GetHashCode();
                hash = hash * 23 + TileCount.GetHashCode();
                return hash;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-------------------------------------------------------------------
