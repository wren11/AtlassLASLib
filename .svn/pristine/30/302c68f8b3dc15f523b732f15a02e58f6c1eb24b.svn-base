using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AtlassLib
{
    public class TcLasTiler : IDisposable
    {
        public event EventHandler<TcTileProgressEventArgs> OnTileProgress;
        
        private Int32 m_TileBlockSize;
        private Int32 m_Factor;

        // Key: Index of the Tile, Value: Array of 1000 points.
        private Dictionary<Int32, LasPoint2[]> m_TileBlocks;

        // Key: Index of the Tile, Value: Number of points found;
        private Dictionary<Int32, Int32> m_BlockPointCount;
        private TcTileBlockInfoCollection m_TileBlockInfoCollection;

        private Int32 m_TileSize;
        private Int32 m_TileRows;
        private Int32 m_TileColumns;
        private Int32 m_RevisedNorth;
        private Int32 m_RevisedEast;
        
        public TcLasTiler(Int32 prmTileSize = 1000, Int32 prmTileBlockSize = 1000, Int32 prmFactor = 1)
        {
            m_TileSize = prmTileSize;
            m_TileBlockSize = prmTileBlockSize;
            m_Factor = prmFactor;
            m_TileBlocks = new Dictionary<Int32, LasPoint2[]>();
            m_BlockPointCount = new Dictionary<Int32, Int32>();
            m_TileBlockInfoCollection = null;
        }
        //-----------------------------------------------------------------------------

        private void UpdateTileCounts(LasHeader prmHeader, Int32 prmSize)
        {
            prmHeader.MinX = prmHeader.MinX - (prmHeader.MinX % (prmSize * m_Factor));
            prmHeader.MinY = prmHeader.MinY - (prmHeader.MinY % (prmSize * m_Factor));
            prmHeader.MaxX = prmHeader.MaxX - (prmHeader.MaxX % (prmSize * m_Factor)) + (prmSize * m_Factor);
            prmHeader.MaxY = prmHeader.MaxY - (prmHeader.MaxY % (prmSize * m_Factor)) + (prmSize * m_Factor);

            m_RevisedEast = (Int32)prmHeader.MinX;
            m_RevisedNorth = (Int32)prmHeader.MaxY;
            m_TileColumns = (Int32)(prmHeader.MaxX - prmHeader.MinX) / (prmSize * m_Factor);
            m_TileRows = (Int32)(prmHeader.MaxY - prmHeader.MinY) / (prmSize * m_Factor);

            m_TileBlockInfoCollection = new TcTileBlockInfoCollection(prmSize, m_TileRows, m_TileColumns);
        }
        //-----------------------------------------------------------------------------

        private void ProcessTileBlock(Int32 prmRow, Int32 prmCol, Int32 prmTileIndex, Int32 prmProcessed)
        {
            // Geographic north and east.
            Int32 east = m_RevisedEast + prmCol * m_TileSize;
            Int32 north = m_RevisedNorth - prmRow * m_TileSize;

            m_TileBlockInfoCollection.TileBlocks.Add(new TcTileBlockInfo(prmRow, prmCol, east, north, prmProcessed, m_BlockPointCount[prmTileIndex]));
            m_TileBlocks.Remove(prmTileIndex);
        }
        //-----------------------------------------------------------------------------

        public LasPoint2[] GetPointsByTile(String prmInput, Int32 prmRow, Int32 prmCol)
        {
            String blockFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            TcTileBlockInfoCollection tileInfoCollection = TcTileUtils.GetTileBlocks(blockFile);

            IEnumerable<TcTileBlockInfo> blocks = tileInfoCollection.TileBlocks.Where(iter => iter.Row == prmRow && iter.Col == prmCol);
            Int32 numberOfPoints = blocks.Aggregate(0, (result, iter) => result += iter.NoOfPoints);

            using (TcLasReader reader = new TcLasReader(prmInput))
            {
                LasHeader header = reader.GetHeaderOld();
                LasPoint2[] points = new LasPoint2[numberOfPoints];
                Int32 arrayStart = 0;
                foreach (TcTileBlockInfo info in blocks)
                {
                    if (reader.GetLasBlock(ref points, arrayStart, info.StartPoint, info.NoOfPoints, header))
                        arrayStart += info.NoOfPoints;
                    else
                        throw new Exception(String.Format("Couldn't read the las tile ({0},{1})", prmRow, prmCol));
                }
                return points;
            }
        }
        //-----------------------------------------------------------------------------

        public void Tile(String prmInput, String prmOutput)
        {
            // Variables to be used inside the big loop.
            LasPoint2 point;
            // Int32 east, north, col, row;
            Int32 tileOffset = 0;
            Int32 index = -1;
            Int32 pointsProcessed = 0;

            using (TcLasReader reader = new TcLasReader(prmInput))
            {
                LasHeader header = reader.GetHeaderOld();
                UpdateTileCounts(header, m_TileSize);

                String blockFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmOutput), Path.GetFileNameWithoutExtension(prmOutput));
                if (File.Exists(blockFile))
                    File.Delete(blockFile);

                using (TcLasWriter writer = new TcLasWriter(prmOutput))
                {
                    header.MinX = m_RevisedEast;
                    header.MaxY = m_RevisedNorth;

                    writer.WriteHeader(header);
                    Int32 onePercent = header.NumberOfPointRecords / 100;
                    
                    for (int i = 0; i < header.NumberOfPointRecords; i++)
                    {
                        point = reader.ReadPoint(header);
                        if (point.X <= 0 || point.Y <= 0)
                            continue;

                        // Calculate the tile index for the point. 
                        tileOffset = m_TileSize * m_Factor;
/*
                        east = Convert.ToInt32(point.X - (point.X % tileOffset));
                        north = Convert.ToInt32(point.Y - (point.Y % tileOffset)) + tileOffset;
                        col = (east - m_RevisedEast) / tileOffset;
                        row = (m_RevisedNorth - north) / tileOffset;
*/
                        Int32[] rowCol = TcMathUtil.GetRowCol(point.X, point.Y, m_RevisedEast, m_RevisedNorth, tileOffset, tileOffset);
                        index = rowCol[1] * m_TileRows + rowCol[0];

                        // Add that into the intermediate tile block.
                        if (!m_TileBlocks.ContainsKey(index))
                        {
                            m_TileBlocks[index] = new LasPoint2[m_TileBlockSize];
                            m_BlockPointCount[index] = 0;
                        }

                        m_TileBlocks[index][m_BlockPointCount[index]] = point;
                        m_BlockPointCount[index]++;

                        // When a tile block is full, write into file and remove the points.
                        if (m_BlockPointCount[index] == m_TileBlockSize)
                        {
                            writer.WritePoints(m_TileBlocks[index], header, m_TileBlockSize);
                            ProcessTileBlock(rowCol[0], rowCol[1], index, pointsProcessed);
                            pointsProcessed += m_TileBlockSize;
                        }

                        if (i % onePercent == 0)
                            NotifyTileProgress(prmInput, prmOutput, rowCol[0], rowCol[1], pointsProcessed, i / onePercent);
                    }

                    // Process the remaining blocks with incomplete size.
                    int row, col;
                    foreach (Int32 idx in m_TileBlocks.Keys.ToList())
                    {
                        row = idx % m_TileRows;
                        col = (idx - row) / m_TileRows;

                        writer.WritePoints(m_TileBlocks[idx], header, m_BlockPointCount[idx]);
                        ProcessTileBlock(row, col, idx, pointsProcessed);
                        pointsProcessed += m_BlockPointCount[idx];
                    }

                    TcTileUtils.SaveTileBlocks(m_TileBlockInfoCollection, blockFile);
                }
            }
        }
        //-----------------------------------------------------------------------------

        private void NotifyTileProgress(String prmInput, String prmOutput, Int32 prmRow, Int32 prmCol, Int32 prmPointsProcessed, Int32 prmProgress)
        {
            if (OnTileProgress != null)
                OnTileProgress(this, new TcTileProgressEventArgs(prmInput, prmOutput, prmRow, prmCol, prmPointsProcessed, prmProgress));
        }
        //-----------------------------------------------------------------------------
        
        public void Dispose()
        {
            m_TileBlocks.Clear();
            m_BlockPointCount.Clear();

            if (m_TileBlockInfoCollection != null)
                m_TileBlockInfoCollection.Dispose();
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
