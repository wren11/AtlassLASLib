using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Atlass.LAS.Lib.Types.Class;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    [XmlRoot("TileBlockCollection", Namespace = "http://www.atlass.com.au", IsNullable = false)]
    public class TcTileBlockInfoCollection : IDisposable
    {
        [XmlElement("TileInfo")]
        public TcTileInfo TileInfo { get; set; }

        [XmlArray("TileBlocks")]
        [XmlArrayItem("Block", typeof(TcTileBlockInfo))]
        public List<TcTileBlockInfo> TileBlocks { get; set; }

        [XmlIgnore]
        private Int32 m_Row;

        [XmlIgnore]
        private Int32 m_Col;

        [XmlIgnore]
        private Boolean[,] m_Indices;

        public TcTileBlockInfoCollection()
            : this(0, 0, 0)
        {
        }
        //-----------------------------------------------------------------------------

        public TcTileBlockInfoCollection(Int32 prmTileSize, Int32 prmRow, Int32 prmCol)
        {
            m_Row = prmRow;
            m_Col = prmCol;
            TileBlocks = new List<TcTileBlockInfo>();
            TileInfo = new TcTileInfo(prmTileSize, prmRow, prmCol);
            m_Indices = new Boolean[prmCol, prmRow];
        }
        //-----------------------------------------------------------------------------

        public Boolean DoesBlockExist(Int32 prmRow, Int32 prmCol)
        {
            return prmRow < m_Row && prmCol < m_Col && m_Indices[prmRow, prmCol];
        }
        //-----------------------------------------------------------------------------

        public Boolean HasOverlap(Double prmUpperLeftEast, Double prmUpperLeftNorth, Double prmLowerRightEast, Double prmLowerRightNorth)
        {
            Int32 upperLeftEast = TileBlocks.Min(iter => iter.East);
            Int32 upperLeftNorth = TileBlocks.Max(iter => iter.North);
            Int32 lowerRightEast = TileBlocks.Max(iter => iter.East);
            Int32 lowerRightNorth = TileBlocks.Min(iter => iter.North);

            return TcRectangle.HasOverlap(upperLeftEast, upperLeftNorth, lowerRightEast, lowerRightNorth, prmUpperLeftEast, prmUpperLeftNorth, prmLowerRightEast, prmLowerRightNorth);
        }
        //-----------------------------------------------------------------------------

        public Boolean HasOverlap(TcRectangle prmRect)
        {
            return HasOverlap(prmRect.UpperLeftX, prmRect.UpperLeftY, prmRect.LowerRightX, prmRect.LowerRightY);
        }
        //-----------------------------------------------------------------------------

        public TcRectangle GetOverlappedArea(Double prmUpperLeftEast, Double prmUpperLeftNorth, Double prmLowerRightEast, Double prmLowerRightNorth)
        {
            Int32 upperLeftEast = TileBlocks.Min(iter => iter.East);
            Int32 upperLeftNorth = TileBlocks.Max(iter => iter.North);
            Int32 lowerRightEast = TileBlocks.Max(iter => iter.East);
            Int32 lowerRightNorth = TileBlocks.Min(iter => iter.North);

            return TcRectangle.OverlapArea(upperLeftEast, upperLeftNorth, lowerRightEast, lowerRightNorth, prmUpperLeftEast, prmUpperLeftNorth, prmLowerRightEast, prmLowerRightNorth);
        }
        //-----------------------------------------------------------------------------

        public TcRectangle GetOverlappedArea(TcRectangle prmArea)
        {
            return GetOverlappedArea(prmArea.UpperLeftX, prmArea.UpperLeftY, prmArea.LowerRightX, prmArea.LowerRightY);
        }
        //-----------------------------------------------------------------------------

        public List<TcTileBlockInfo> GetTileBlocks(TcRectangle prmRect)
        {
            List<TcTileBlockInfo> tileBlocks = new List<TcTileBlockInfo>();
            foreach (TcTileBlockInfo info in TileBlocks)
            {
                if (prmRect.HasOverlap(info.East, info.North, info.East + TileInfo.TileSize, info.North - TileInfo.TileSize))
                {
                    tileBlocks.Add(info);
                }
            }
            return tileBlocks;
        }
        //-----------------------------------------------------------------------------

        public List<TcTileBlockInfo> GetTileBlocks(Double prmUpperLeftEast, Double prmUpperLeftNorth, Double prmLowerRightEast, Double prmLowerRightNorth)
        {
            return GetTileBlocks(new TcRectangle(prmUpperLeftEast, prmUpperLeftNorth, prmLowerRightEast, prmLowerRightNorth));
        }
        //-----------------------------------------------------------------------------

        public List<Int32> GetTileIndices(TcRectangle prmArea)
        {
            HashSet<Int32> indices = new HashSet<Int32>();
            foreach (TcTileBlockInfo info in TileBlocks)
            {
                if (prmArea.HasOverlap(info.East, info.North, info.East + TileInfo.TileSize, info.North - TileInfo.TileSize))
                {
                    indices.Add(info.Row * TileInfo.Col + info.Col);
                }
            }
            return indices.ToList();
        }
        //-----------------------------------------------------------------------------

        public Int32 GetNumberOfPointsByTile(Int32 prmRow, Int32 prmCol)
        {
            IEnumerable<TcTileBlockInfo> blocks = TileBlocks.Where(iter => iter.Row == prmRow && iter.Col == prmCol);
            return blocks.Count() > 0 ? blocks.Sum(iter => iter.NoOfPoints) : 0;
        }
        //-----------------------------------------------------------------------------

        public Int32 GetNumberOfPointsByTile(Int32 prmIndex)
        {
            Int32 col = prmIndex % TileInfo.Col;
            Int32 row = (prmIndex - col) / TileInfo.Row;
            return GetNumberOfPointsByTile(row, col);
        }
        //-----------------------------------------------------------------------------

        public void Dispose()
        {
            TileBlocks.Clear();
            m_Indices = null;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
