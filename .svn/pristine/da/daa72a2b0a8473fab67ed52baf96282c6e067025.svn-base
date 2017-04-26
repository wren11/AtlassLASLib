using System;
using System.Collections.Generic;
using System.Linq;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Types.DataType;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Operations.Gridding.Types;
using Atlass.LAS.Lib.Types.Class;

namespace Atlass.LAS.Lib.Operations.Tiling.IO
{
    public class TcTileLasReader : TcLasReader
    {
        protected TcTileBlockInfoCollection m_TileInfo;
        
        public TcTileLasReader(String prmInput, TcTileBlockInfoCollection prmTileInfo)
            : base(prmInput)
        {
            m_TileInfo = prmTileInfo;
        }
        //-----------------------------------------------------------------------------

        public TcTileLasReader(TcIndexedLasInfo prmInfo)
            : this(prmInfo.LasFile, prmInfo.TileInfoCollection)
        {
        }
        //-----------------------------------------------------------------------------

        public T[] GetPointsByTileBlock<T>(TcTileBlockInfo block) where T : TiLasPoint
        {
            SeekToPoint(block.StartPoint);
            return ReadPoints<T>(block.NoOfPoints);
        }
        //-----------------------------------------------------------------------------

        public T[] GetPointsByTileBlocks<T>(IEnumerable<TcTileBlockInfo> blocks) where T : TiLasPoint
        {
            Int32 numberOfPoints = blocks.Aggregate(0, (result, iter) => result += iter.NoOfPoints);
            T[] points = new T[numberOfPoints];
            Int32 arrayStart = 0;

            foreach (TcTileBlockInfo info in blocks)
            {
                Array.Copy(GetPointsByTileBlock<T>(info), 0, points, arrayStart, info.NoOfPoints);
                arrayStart += info.NoOfPoints;
            }
            return points;
        }
        //-----------------------------------------------------------------------------

        public T[] GetPointsByTile<T>(Int32 prmRow, Int32 prmColumn) where T : TiLasPoint
        {
            return GetPointsByTileBlocks<T>(m_TileInfo.TileBlocks.Where(iter => iter.Row == prmRow && iter.Col == prmColumn));
        }
        //-----------------------------------------------------------------------------

        public T[] GetPointsByTile<T>(Int32 prmIndex) where T : TiLasPoint
        {
            Int32 col = prmIndex % m_TileInfo.TileInfo.Col;
            Int32 row = (prmIndex - col) / m_TileInfo.TileInfo.Col;
            return GetPointsByTile<T>(row, col);
        }
        //-----------------------------------------------------------------------------

        public T[] GetPointsByArea<T>(TcRectangle prmArea) where T : TiLasPoint
        {
            return GetPointsByTileBlocks<T>(m_TileInfo.GetTileBlocks(prmArea));
        }
        //-----------------------------------------------------------------------------

        public T[] FilterPointsByArea<T>(T[] prmPoints, TcRectangle prmArea) where T : TiLasPoint
        {
            List<T> filteredPoints = new List<T>();
            for (int i = 0; i < prmPoints.Length; i++)
                if ((prmPoints[i].X >= prmArea.UpperLeftX && prmPoints[i].X <= prmArea.LowerRightX)
                 && (prmPoints[i].Y <= prmArea.UpperLeftY && prmPoints[i].Y >= prmArea.LowerRightY))
                {
                    filteredPoints.Add(prmPoints[i]);
                }
            return filteredPoints.ToArray();
        }
        //-----------------------------------------------------------------------------

        public TcLasPointBase[] GetPointObjectsByTileBlock<T>(TcTileBlockInfo block, TiLasHeader prmHeader) where T : TiLasPoint
        {
            SeekToPoint(block.StartPoint);
            return ReadPointsAsObject<T>(block.NoOfPoints, prmHeader);
        }
        //-----------------------------------------------------------------------------

        public TcLasPointBase[] GetPointObjectsByTileBlocks<T>(IEnumerable<TcTileBlockInfo> blocks, TiLasHeader prmHeader) where T : TiLasPoint
        {
            Int32 numberOfPoints = blocks.Aggregate(0, (result, iter) => result += iter.NoOfPoints);
            TcLasPointBase[] points = new TcLasPointBase[numberOfPoints];
            Int32 arrayStart = 0;

            foreach (TcTileBlockInfo info in blocks)
            {
                Array.Copy(GetPointObjectsByTileBlock<T>(info, prmHeader), 0, points, arrayStart, info.NoOfPoints);
                arrayStart += info.NoOfPoints;
            }
            return points;
        }
        //-----------------------------------------------------------------------------

        public TcLasPointBase[] GetPointObjectsByTile<T>(Int32 prmRow, Int32 prmColumn, TiLasHeader prmHeader) where T : TiLasPoint
        {
            return GetPointObjectsByTileBlocks<T>(m_TileInfo.TileBlocks.Where(iter => iter.Row == prmRow && iter.Col == prmColumn), prmHeader);
        }
        //-----------------------------------------------------------------------------

        public TcLasPointBase[] GetPointObjectsByTile<T>(Int32 prmIndex, TiLasHeader prmHeader) where T : TiLasPoint
        {
            Int32 col = prmIndex % m_TileInfo.TileInfo.Col;
            Int32 row = (prmIndex - col) / m_TileInfo.TileInfo.Col;
            return GetPointObjectsByTile<T>(row, col, prmHeader);
        }
        //-----------------------------------------------------------------------------

        public TcLasPointBase[] GetPointObjectsByArea<T>(TcRectangle prmArea, TiLasHeader prmHeader) where T : TiLasPoint
        {
            return GetPointObjectsByTileBlocks<T>(m_TileInfo.GetTileBlocks(prmArea), prmHeader);
        }
        //-----------------------------------------------------------------------------

        public T[] FilterPointObjectsByArea<T>(T[] prmPoints, TcRectangle prmArea) where T : TcLasPointBase
        {
            List<T> filteredPoints = new List<T>();
            for (int i = 0; i < prmPoints.Length; i++)
                if ((prmPoints[i].X >= prmArea.UpperLeftX && prmPoints[i].X <= prmArea.LowerRightX)
                 && (prmPoints[i].Y <= prmArea.UpperLeftY && prmPoints[i].Y >= prmArea.LowerRightY))
                {
                    filteredPoints.Add(prmPoints[i]);
                }
            return filteredPoints.ToArray();
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------