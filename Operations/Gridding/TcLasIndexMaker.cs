﻿///<summary> TcLasIndexMaker
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class duplicates any LAS file and reorder the points to arrange them per
/// block of small tiles. It also creates and xml index file which contains the
/// information about the location, number of points and position of that tile
/// block inside the indexed LAS file. These indexed LAS files are very useful to
/// extracts polygons or crop some data out of it.
/// 
/// The tiles are stored in a block of N number of points determined by the user.
/// A tile can have multiple blocks of N points saved into different part of the
/// LAS file. Software reads the index file to determine which point blocks to
/// read in order to produce a tile.
/// 
/// Format of the index file:
/// @ TileInfo
///  - Size = Height / Width of each tile block.
///  - Row = Number of tiles in Y direction.
///  - Col = Number of tiles in X direction.
///  - Count = Total number of tiles (Row x Col).
/// 
/// @ TileBlock
///  - Row = Index of a tile in Y direction.
///  - Col = Index of a tile in X direction.
///  - North = Geographic north of a tile.
///  - East = Geographic east of a tile.
///  - Start = The LAS point index where this block starts.

/// <author>
/// Name: S M Kamrul Hasan
/// Date: 08-AUG-2014
/// </author>
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
///</summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Operations.Tiling;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;
using Atlass.LAS.Lib.Utilities;
using Atlass.LAS.Lib.Types;

namespace Atlass.LAS.Lib.Operations.Gridding
{
    public class TcLasIndexMaker : TiOperation, IDisposable
    {
        /// <summary>
        /// Callback for any message to be passed to the parent thread.
        /// </summary>
        public override event EventHandler<TcMessageEventArgs> OnMessage;

        /// <summary>
        /// Callback for any error happened in the processing.
        /// </summary>
        public override event EventHandler<TcErrorEventArgs> OnError;

        /// <summary>
        /// Callback to notify the parent thread about process finish.
        /// </summary>
        public override event EventHandler<EventArgs> OnFinish;

        /// <summary>
        /// The percentage of progress to be used for notifying the caller.
        /// </summary>
        public override Double ProgressFrequency { protected get; set; }

        public Int32 TileSize { private get; set; }
        public Int32 PointBlockSize { private get; set; }
        public Int32 Factor { private get; set; }
        public Double XAdjustment { private get; set; }
        public Double YAdjustment { private get; set; }
        public Double ZAdjustment { private get; set; }
        public Double MinClampZ { private get; set; }
        public Double MaxClampZ { private get; set; }
        
        // Key: Index of the Tile, Value: Number of points found;
        private Dictionary<Int32, Int32> m_BlockPointCount;
        private TcTileBlockInfoCollection m_TileBlockInfoCollection;

        private Int32 m_TileRows;
        private Int32 m_TileColumns;
        private Int32 m_RevisedNorth;
        private Int32 m_RevisedEast;

        public TcLasIndexMaker()
        {
            m_BlockPointCount = new Dictionary<Int32, Int32>();
            m_TileBlockInfoCollection = null;

            ProgressFrequency = 5;
            TileSize = 200;
            PointBlockSize = 20000;
            Factor = 1;
            XAdjustment = 0.0;
            YAdjustment = 0.0;
            ZAdjustment = 0.0;
            MinClampZ = Double.MinValue;
            MaxClampZ = Double.MaxValue;
        }
        //-----------------------------------------------------------------------------

        protected override void ReportMessage(String prmMessage)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new TcMessageEventArgs(prmMessage));
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportFinished()
        {
            if (OnFinish != null)
            {
                OnFinish(this, new EventArgs());
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportError(String prmError, Exception prmEx)
        {
            if (OnError != null)
            {
                OnError(this, new TcErrorEventArgs(prmError, prmEx));
            }
        }
        //-----------------------------------------------------------------------------

#warning Vicky: It can be removed if it didn't affect other projects. 
        private void UpdateTileCounts(ref TiLasHeader prmHeader, Int32 prmSize)
        {
            prmHeader.MinX = (prmHeader.MinX + XAdjustment) - ((prmHeader.MinX + XAdjustment) % (prmSize * Factor));
            prmHeader.MinY = (prmHeader.MinY + YAdjustment) - ((prmHeader.MinY + YAdjustment) % (prmSize * Factor));
            prmHeader.MaxX = (prmHeader.MaxX + XAdjustment) - ((prmHeader.MaxX + XAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
            prmHeader.MaxY = (prmHeader.MaxY + YAdjustment) - ((prmHeader.MaxY + YAdjustment) % (prmSize * Factor)) + (prmSize * Factor);

            m_RevisedEast = (Int32)prmHeader.MinX;
            m_RevisedNorth = (Int32)prmHeader.MaxY;
            m_TileColumns = (Int32)(prmHeader.MaxX - prmHeader.MinX) / (prmSize * Factor);
            m_TileRows = (Int32)(prmHeader.MaxY - prmHeader.MinY) / (prmSize * Factor);
            m_TileBlockInfoCollection = new TcTileBlockInfoCollection(prmSize, m_TileRows, m_TileColumns);
        }

        private TiLasHeader UpdateTileCounts<T>(TiLasHeader prmTileHeader, Int32 prmSize) where T: TiLasPoint
        {
            if (prmTileHeader is TsLasHeader12)
            {
                TsLasHeader12 tileHeader = (TsLasHeader12)prmTileHeader;
                tileHeader.MinX = (tileHeader.MinX + XAdjustment) - ((tileHeader.MinX + XAdjustment) % (prmSize * Factor));
                tileHeader.MinY = (tileHeader.MinY + YAdjustment) - ((tileHeader.MinY + YAdjustment) % (prmSize * Factor));
                tileHeader.MaxX = (tileHeader.MaxX + XAdjustment) - ((tileHeader.MaxX + XAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.MaxY = (tileHeader.MaxY + YAdjustment) - ((tileHeader.MaxY + YAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.NumberOfPointRecords = 0;
                tileHeader.NumberofPointsByReturn1 = 0;
                tileHeader.NumberofPointsByReturn2 = 0;
                tileHeader.NumberofPointsByReturn3 = 0;
                tileHeader.NumberofPointsByReturn4 = 0;
                tileHeader.NumberofPointsByReturn5 = 0;

                m_RevisedEast = (Int32)tileHeader.MinX;
                m_RevisedNorth = (Int32)tileHeader.MaxY;
                m_TileColumns = (Int32)(tileHeader.MaxX - tileHeader.MinX) / (prmSize * Factor);
                m_TileRows = (Int32)(tileHeader.MaxY - tileHeader.MinY) / (prmSize * Factor);
                m_TileBlockInfoCollection = new TcTileBlockInfoCollection(prmSize, m_TileRows, m_TileColumns);

                return tileHeader;
            }
            else if (prmTileHeader is TsLasHeader13)
            {
                TsLasHeader13 tileHeader = (TsLasHeader13)prmTileHeader;
                tileHeader.MinX = (tileHeader.MinX + XAdjustment) - ((tileHeader.MinX + XAdjustment) % (prmSize * Factor));
                tileHeader.MinY = (tileHeader.MinY + YAdjustment) - ((tileHeader.MinY + YAdjustment) % (prmSize * Factor));
                tileHeader.MaxX = (tileHeader.MaxX + XAdjustment) - ((tileHeader.MaxX + XAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.MaxY = (tileHeader.MaxY + YAdjustment) - ((tileHeader.MaxY + YAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.NumberOfPointRecords = 0;
                tileHeader.NumberofPointsByReturn1 = 0;
                tileHeader.NumberofPointsByReturn2 = 0;
                tileHeader.NumberofPointsByReturn3 = 0;
                tileHeader.NumberofPointsByReturn4 = 0;
                tileHeader.NumberofPointsByReturn5 = 0;

                m_RevisedEast = (Int32)tileHeader.MinX;
                m_RevisedNorth = (Int32)tileHeader.MaxY;
                m_TileColumns = (Int32)(tileHeader.MaxX - tileHeader.MinX) / (prmSize * Factor);
                m_TileRows = (Int32)(tileHeader.MaxY - tileHeader.MinY) / (prmSize * Factor);
                m_TileBlockInfoCollection = new TcTileBlockInfoCollection(prmSize, m_TileRows, m_TileColumns);

                return tileHeader;
            }
            else if (prmTileHeader is TsLasHeader14)
            {
                TsLasHeader14 tileHeader = (TsLasHeader14)prmTileHeader;
                tileHeader.MinX = (tileHeader.MinX + XAdjustment) - ((tileHeader.MinX + XAdjustment) % (prmSize * Factor));
                tileHeader.MinY = (tileHeader.MinY + YAdjustment) - ((tileHeader.MinY + YAdjustment) % (prmSize * Factor));
                tileHeader.MaxX = (tileHeader.MaxX + XAdjustment) - ((tileHeader.MaxX + XAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.MaxY = (tileHeader.MaxY + YAdjustment) - ((tileHeader.MaxY + YAdjustment) % (prmSize * Factor)) + (prmSize * Factor);
                tileHeader.NumberOfPointRecords = 0;
                tileHeader.NumberofPointsByReturn1 = 0;
                tileHeader.NumberofPointsByReturn2 = 0;
                tileHeader.NumberofPointsByReturn3 = 0;
                tileHeader.NumberofPointsByReturn4 = 0;
                tileHeader.NumberofPointsByReturn5 = 0;
                tileHeader.NumberofPointsByReturn6 = 0;
                tileHeader.NumberofPointsByReturn7 = 0;
                tileHeader.NumberofPointsByReturn8 = 0;
                tileHeader.NumberofPointsByReturn9 = 0;
                tileHeader.NumberofPointsByReturn10 = 0;
                tileHeader.NumberofPointsByReturn11 = 0;
                tileHeader.NumberofPointsByReturn12 = 0;
                tileHeader.NumberofPointsByReturn13 = 0;
                tileHeader.NumberofPointsByReturn14 = 0;
                tileHeader.NumberofPointsByReturn15 = 0;
                tileHeader.LegNumberofPointsByReturn1=0;
                tileHeader.LegNumberofPointsByReturn2=0;
                tileHeader.LegNumberofPointsByReturn3=0;
                tileHeader.LegNumberofPointsByReturn4=0;
                tileHeader.LegNumberofPointsByReturn5=0;

                m_RevisedEast = (Int32)tileHeader.MinX;
                m_RevisedNorth = (Int32)tileHeader.MaxY;
                m_TileColumns = (Int32)(tileHeader.MaxX - tileHeader.MinX) / (prmSize * Factor);
                m_TileRows = (Int32)(tileHeader.MaxY - tileHeader.MinY) / (prmSize * Factor);
                m_TileBlockInfoCollection = new TcTileBlockInfoCollection(prmSize, m_TileRows, m_TileColumns);
                return tileHeader;
            }

            throw new InvalidDataException("Couldn't update the Tile Header. Invalid data format.");
         }
        //-----------------------------------------------------------------------------

        private void ProcessTileBlock<T>(Dictionary<Int32, T[]> prmTileBlocks, Int32 prmRow, Int32 prmCol, Int32 prmTileIndex, Int32 prmProcessed) where T : TiLasPoint
        {
            // Geographic north and east.
            Int32 east = m_RevisedEast + prmCol * TileSize;
            Int32 north = m_RevisedNorth - prmRow * TileSize;

            m_TileBlockInfoCollection.TileBlocks.Add(new TcTileBlockInfo(prmRow, prmCol, east, north, prmProcessed, m_BlockPointCount[prmTileIndex]));
            prmTileBlocks.Remove(prmTileIndex);
        }
        //-----------------------------------------------------------------------------

        protected Int64 GetNumberOfPoints(TiLasHeader prmHeader)
        {
            if (prmHeader is TsLasHeader12)
            {
                return ((TsLasHeader12)prmHeader).NumberOfPointRecords;
            }
            if (prmHeader is TsLasHeader13)
            {
                return ((TsLasHeader13)prmHeader).NumberOfPointRecords;
            }
            if (prmHeader is TsLasHeader14)
            {
                return (Int64)((TsLasHeader14)prmHeader).NumberOfPointRecords;
            }

            throw new FormatException("Couldn't process the tile. LAS format not supported");
        }
        //-----------------------------------------------------------------------------

        private void ProcessTiles<T>(TcLasReader prmReader, String prmOutput) where T : TiLasPoint
        {
            // Variables to be used inside the big loop.
            Int32 tileOffset = 0;
            Int32 index = -1;
            Int32 pointsProcessed = 0;
            
            // Key: Index of the Tile, Value: Array of 1000 points.
            Dictionary<Int32, T[]> m_TileBlocks = new Dictionary<Int32, T[]>();

            TiLasHeader newHeader = UpdateTileCounts<T>(prmReader.Header, TileSize);
            Byte[] offsetBytes = prmReader.OffsetBytes;

            String blockFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmOutput), Path.GetFileNameWithoutExtension(prmOutput));
            if (File.Exists(blockFile))
                File.Delete(blockFile);

            using (TcLasWriter writer = new TcLasWriter(prmOutput) { MinClampZ = MinClampZ, MaxClampZ = MaxClampZ, 
                /* Added By Dean */ ZAdjustment = this.ZAdjustment, XAdjustment = this.XAdjustment, YAdjustment = this.YAdjustment })
            {
                writer.WriteHeader(newHeader, offsetBytes);
                Int64 numberOfPointRecords = GetNumberOfPoints(prmReader.Header);
                Int32 onePercent = (Int32)numberOfPointRecords / 100;
                Int32[] rowCol = new Int32[2];
                DateTime now = DateTime.Now;
                Boolean[] availIndices = new Boolean[m_TileBlockInfoCollection.TileInfo.Row * m_TileBlockInfoCollection.TileInfo.Col];
                Double x, y;
                Double z = 0; // Added Z Property  - Dean


                Int64 noOfPointsLoaded = 0;
                Int64 noOfPointsToRead = 0;
                while (noOfPointsLoaded < numberOfPointRecords)
                {
                    noOfPointsToRead = Math.Min(TcConstants.MaxLasPointsToProcessAtOnce, numberOfPointRecords - noOfPointsLoaded);
                    T[] loadedPoints = prmReader.ReadPoints<T>(noOfPointsToRead);
                    noOfPointsLoaded += noOfPointsToRead;

                    for (int i = 0; i < noOfPointsToRead; i++)
                    {
                        x = loadedPoints[i].X * newHeader.XScaleFactor + newHeader.XOffset + XAdjustment;
                        y = loadedPoints[i].Y * newHeader.YScaleFactor + newHeader.YOffset + YAdjustment;

                        //added by dean
                        z = loadedPoints[i].Z * newHeader.ZScaleFactor + newHeader.ZOffset + ZAdjustment;

                        if (x <= 0 || y <= 0)
                            continue;

                        // Calculate the tile index for the point. 
                        tileOffset = TileSize * Factor;
                        rowCol = TcMathUtil.GetRowCol(x, y, m_RevisedEast, m_RevisedNorth, tileOffset, tileOffset);
                        index = rowCol[1] * m_TileRows + rowCol[0];
                            
                        if (!availIndices[index])
                        {
                            m_TileBlocks[index] = new T[PointBlockSize];
                            m_BlockPointCount[index] = 0;
                            availIndices[index] = true;
                        }

                        // Convert the int XY back with adjusted values.
                        loadedPoints[i].X = (Int32)((x - newHeader.XOffset) / newHeader.XScaleFactor);
                        loadedPoints[i].Y = (Int32)((y - newHeader.YOffset) / newHeader.YScaleFactor);
                        loadedPoints[i].Z = (Int32)((z - newHeader.ZOffset) / newHeader.ZScaleFactor); 

                        m_TileBlocks[index][m_BlockPointCount[index]] = loadedPoints[i];
                        m_BlockPointCount[index]++;

                        // When a tile block is full, write into file and remove the points.
                        if (m_BlockPointCount[index] == PointBlockSize)
                        {
                            // writer.WritePoints<T>(m_TileBlocks[index], PointBlockSize);
                            writer.WritePointsWithOptions<T>(m_TileBlocks[index], ref newHeader, PointBlockSize);
                            ProcessTileBlock<T>(m_TileBlocks, rowCol[0], rowCol[1], index, pointsProcessed);
                            pointsProcessed += PointBlockSize;
                            availIndices[index] = false;
                        }
                    }
                }

                // Process the remaining blocks with incomplete size.
                int row, col;
                foreach (Int32 idx in m_TileBlocks.Keys.ToList())
                {
                    row = idx % m_TileRows;
                    col = (idx - row) / m_TileRows;

                    writer.WritePointsWithOptions<T>(m_TileBlocks[idx], ref newHeader, m_BlockPointCount[idx]);
                    ProcessTileBlock<T>(m_TileBlocks, row, col, idx, pointsProcessed);
                    pointsProcessed += m_BlockPointCount[idx];
                }

                // Write the updated header.
                writer.WriteHeader(newHeader);

                TcTileUtils.SaveTileBlocks(m_TileBlockInfoCollection, blockFile);
            }
        }
        //-----------------------------------------------------------------------------

        public void Index(String prmInput, String prmOutput)
        {
            if (!File.Exists(prmInput))
            {
                throw new FileNotFoundException("LAS file not found");
            }

            using (TcLasReader reader = new TcLasReader(prmInput))
            {
                switch (reader.Header.PointDataFormatID)
                {
                    case 0:
                        ProcessTiles<TsLasPoint0>(reader, prmOutput);
                        break;

                    case 1:
                        ProcessTiles<TsLasPoint1>(reader, prmOutput);
                        break;

                    case 2:
                        ProcessTiles<TsLasPoint2>(reader, prmOutput);
                        break;

                    case 3:
                        ProcessTiles<TsLasPoint3>(reader, prmOutput);
                        break;

                    case 4:
                        ProcessTiles<TsLasPoint4>(reader, prmOutput);
                        break;

                    case 5:
                        ProcessTiles<TsLasPoint5>(reader, prmOutput);
                        break;

                    case 6:
                        ProcessTiles<TsLasPoint6>(reader, prmOutput);
                        break;

                    case 7:
                        ProcessTiles<TsLasPoint7>(reader, prmOutput);
                        break;

                    case 8:
                        ProcessTiles<TsLasPoint8>(reader, prmOutput);
                        break;

                    case 9:
                        ProcessTiles<TsLasPoint9>(reader, prmOutput);
                        break;

                    case 10:
                        ProcessTiles<TsLasPoint10>(reader, prmOutput);
                        break;

                    default:
                        throw new FormatException("Couldn't process the tile. LAS format not supported");
                }
            }
        }
        //-----------------------------------------------------------------------------
        
        public void Dispose()
        {
            m_BlockPointCount.Clear();
            if (m_TileBlockInfoCollection != null)
            {
                m_TileBlockInfoCollection.Dispose();
            }

            OnMessage = null;
            OnError = null;
            OnFinish = null;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
