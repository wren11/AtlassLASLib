﻿///<summary> TcLasGridder
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class grids the indexed LAS files and export them into TOR files with
/// TOL as header. There are some predefined griding types which is commonly used
/// for LiDAR processing, but the class can handle any grid size. It takes only
/// a single indexed LAS file into account. For covering an entire area, LAS files
/// need to be merged into one and re-indexed. An object of this class takes the
/// LAS file and a list of gridding type as parameters.
/// 
/// @Pros:
///  - Passing multiple gridding types at once is faster than processing them
///    separately. It's because the class will read the LAS file only once.
///  - Can process any LAS format.
/// 
/// @Cons:
///  - Processing multiple gridding at once may have slight memory impact.
///  - LAS files always need to be indexed using TcLasIndexMaker.
///  
/// @Pre-defined Grid Types:
///  - Leveling = 2m gridded with ground points only
///  - M1FirstEcho = 1m gridded with points from first-echo
///  - M1LastEcho = 1m gridded with points from last-echo
///  - Display = 5m gridded with average points
///  
/// <author>
/// Name: S M Kamrul Hasan
/// Date: 19-AUG-2014
/// </author>
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
///</summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Operations.Gridding.Types;
using Atlass.LAS.Lib.Operations.Tiling;
using Atlass.LAS.Lib.Operations.Tiling.IO;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Operations.Types;
using Atlass.LAS.Lib.Support.IO;
using Atlass.LAS.Lib.Support.Types;
using Atlass.LAS.Lib.Types;
using Atlass.LAS.Lib.Types.Class;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;
using Atlass.LAS.Lib.Utilities;

namespace Atlass.LAS.Lib.Operations.Gridding
{
    public class TcLasGridder : TiOperation, IDisposable
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

        private TiLasHeader m_LasHeader;
        private TcTileBlockInfoCollection m_Info;
        private Int32 m_CoordinateSystem;
        private String m_OutputDirectory;

        public TcLasGridder(String prmOutputDirectory, Int32 prmCoordinateSystem)
        {
            m_OutputDirectory = prmOutputDirectory;
            m_CoordinateSystem = prmCoordinateSystem;
            ProgressFrequency = 5;
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

        public Double GetLevellingGridHeight(List<TcLasPointBase> prmPoints)
        {
            // When there is no point.
            if (prmPoints.Count == 0)
            {
                return TcConstants.TorNullValue32Bit;
            }

            // When there is less than 2 points with last echo.
            List<Double> heights = new List<Double>();
            Double maxHeight = Double.MinValue;
            Double minHeight = Double.MaxValue;

            for (int i = 0; i < prmPoints.Count; i++)
            {
                if (prmPoints[i].ReturnNumber == prmPoints[i].NumberOfReturns)
                {
                    heights.Add(prmPoints[i].Z);
                    maxHeight = Math.Max(maxHeight, prmPoints[i].Z);
                    minHeight = Math.Min(minHeight, prmPoints[i].Z);
                }
            }

            if (heights.Count <= 2 || (maxHeight - minHeight > TcConstants.MaxToleranceForFlatPoints))
            {
                return TcConstants.TorNullValue32Bit;
            }

            // Sort the heights.
            //TcSort.Quicksort(heights, 0, heights.Count - 1);

            Double avg = TcMathUtil.Average(heights);
            Double stDev = TcMathUtil.StDev(heights, avg);
            Int32 count = 0;
            Double sum = 0.0;

            // Get points inside 2 sigma boundary.
            for (int i = 0; i < heights.Count; i++)
            {
                if (Math.Abs(heights[i] - avg) < 2 * stDev)
                {
                    sum += heights[i];
                    count++;
                }
            }

            return count > 0 ? sum / count : TcConstants.TorNullValue32Bit;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function that update the grid with given points from a LAS tile block.
        /// </summary>
        /// <param name="prmObj">Gridding data object</param>
        /// <param name="prmInfo">Information of the LAS block</param>
        /// <param name="prmPoints">Collection of LAS points</param>
        private void UpdateGrid(TcGridObject prmObj, TcTileBlockInfo prmInfo, TcLasPointBase[] prmPoints)
        {
            Dictionary<Int32, List<TcLasPointBase>> gridPointCollection = new Dictionary<Int32, List<TcLasPointBase>>(prmObj.GridCount * prmObj.GridCount);
            Int32 row, col, index;
            Int32[] rowCol = new Int32[2];
            //Boolean[] availableIndices = new Boolean[(Int32)Math.Pow((Int32)(prmObj.Info.TileInfo.Row * prmObj.Info.TileInfo.Col * (prmObj.TileSize * 1.0 / prmObj.GridSize)), 2)];
            Boolean[] availableIndices = new Boolean[prmObj.Info.TileInfo.Row * prmObj.GridCount * prmObj.Info.TileInfo.Col * prmObj.GridCount];

            for (int i = 0; i < prmPoints.Length; i++)
            {
                rowCol = TcMathUtil.GetRowCol(prmPoints[i].X, prmPoints[i].Y, prmInfo.East, prmInfo.North, prmObj.GridSize, prmObj.GridSize);
                index = rowCol[1] * prmObj.GridCount + rowCol[0];

                if (index < 0)
                    index = 0;

                // Add that into the intermediate tile block.
                if (!availableIndices[index])
                {
                    gridPointCollection[index] = new List<TcLasPointBase>();
                    availableIndices[index] = true;
                }

                gridPointCollection[index].Add(prmPoints[i]);
            }

            Single height;
            Int32 subGridIndex;
            Int32 rowIndex;
            foreach (Int32 key in gridPointCollection.Keys)
            {
                row = key % prmObj.GridCount;
                col = (key - row) / prmObj.GridCount;
                height = (Single)GetGridHeight(gridPointCollection[key], prmObj.Type);
                
                // Save the point into the specific TOR block.
                rowIndex = prmInfo.Row * prmObj.GridCount + row;
                subGridIndex = (Int32)Math.Floor(rowIndex * 1.0 / prmObj.MaxRowsInGridBlock);
                prmObj.TorBlocks[subGridIndex].Points[rowIndex % prmObj.MaxRowsInGridBlock, prmInfo.Col * prmObj.GridCount + col] = height;
                
                // Update the min and max.
                if (height != TcConstants.TorNullValue32Bit)
                {
                    prmObj.MinZ = Math.Min(prmObj.MinZ, height);
                    prmObj.MaxZ = Math.Max(prmObj.MaxZ, height);
                }
            }
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to return the grid height based on gridding type.
        /// </summary>
        /// <param name="prmPoints">LAS points</param>
        /// <param name="prmType">Gridding type</param>
        /// <returns>The height for that specific grid</returns>
        private Double GetGridHeight(List<TcLasPointBase> prmPoints, TeGriddingType prmType)
        {
            switch (prmType)
            {
                case TeGriddingType.Levelling:
                    return GetLevellingGridHeight(prmPoints);

                case TeGriddingType.M1FirstEcho:
                    return GetM1FirstEchoGridHeight(prmPoints);

                case TeGriddingType.M1LastEcho:
                    return GetM1LastEchoGridHeight(prmPoints);

                case TeGriddingType.Display:
                    return prmPoints.Max(iter => iter.Z);

                default:
                    return prmPoints.Max(iter => iter.Z);
            }
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to return the grid height for the 1m first echo.
        /// </summary>
        /// <param name="prmPoints">List of las points</param>
        /// <returns>The height for that specific grid</returns>
        private Double GetM1FirstEchoGridHeight(List<TcLasPointBase> prmPoints)
        {
            // When there is no point.
            if (prmPoints.Count == 0)
            {
                return TcConstants.TorNullValue32Bit;
            }

            Double maxHeight = TcConstants.TorNullValue32Bit;
            for (int i = 0; i < prmPoints.Count; i++)
            {
                if (prmPoints[i].ReturnNumber == 1)
                {
                    maxHeight = Math.Max(maxHeight, prmPoints[i].Z);
                }
            }

            return maxHeight;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to return the grid height for the 1m last echo.
        /// </summary>
        /// <param name="prmPoints">List of las points</param>
        /// <returns>The height for that specific grid</returns>
        private Double GetM1LastEchoGridHeight(List<TcLasPointBase> prmPoints)
        {
            // When there is no point.
            if (prmPoints.Count == 0)
            {
                return TcConstants.TorNullValue32Bit;
            }

            Double maxHeight = TcConstants.TorNullValue32Bit;
            for (int i = 0; i < prmPoints.Count; i++)
            {
                if (prmPoints[i].ReturnNumber == prmPoints[i].NumberOfReturns)
                {
                    maxHeight = Math.Max(maxHeight, prmPoints[i].Z);
                }
            }

            return maxHeight;
        }
        //-----------------------------------------------------------------------------
        
        /// <summary>
        /// This function filters the low dense points.
        /// </summary>
        /// <param name="prmPoints">List of points inside a tile.</param>
        /// <returns>Min and Max after filtering.</returns>
        private Double[] FilterOutOfInterestPoints(TcLasPointBase[] prmPoints)
        {
            const Int32 minHeightRange = -1000;
            const Int32 maxHeightRange = 4000;
            const Int32 heightBlockSize = 10;

            Int32 noOfBlocks = (maxHeightRange - minHeightRange) / heightBlockSize;
            
            Double min = Double.MaxValue;
            Double max = Double.MinValue;

            Int32[] range = Enumerable.Repeat(0, noOfBlocks).ToArray();

            for (int i = 0; i < prmPoints.Length; i++)
            {
                if (prmPoints[i].Z > minHeightRange && prmPoints[i].Z < maxHeightRange)
                {
                    int index = (int)Math.Floor(prmPoints[i].Z / heightBlockSize);
                    range[index - (minHeightRange / heightBlockSize)]++;
                }
            }

            // Get max number of points
            Int32 maxIndex = -1;
            Int32 rangeMax = range.Max();
            for (int i = 0; i < range.Length; i++)
            {
                if (range[i] == rangeMax)
                {
                    maxIndex = i;
                    break;
                }
            }

            for (int i = maxIndex + 1; i < noOfBlocks; i++)
            {
                if (range[i] == 0)
                {
                    max = i;
                    break;
                }
            }

            for (int i = maxIndex - 1; i > 0; i--)
            {
                if (range[i] == 0)
                {
                    min = i - 1;
                    break;
                }
            }

            int cnt = 0;
            min = heightBlockSize * min + minHeightRange;
            max = heightBlockSize * max + minHeightRange;
            
            Double minZ = Double.MaxValue;
            Double maxZ = Double.MinValue;
            for (int i = 0; i < prmPoints.Length; i++)
            {
                if (prmPoints[i].Z < min || prmPoints[i].Z > max)
                {
                    prmPoints[i].Z = TcConstants.TorNullValue32Bit;
                    cnt++;
                }
                else
                {
                    minZ = Math.Min(prmPoints[i].Z, minZ);
                    maxZ = Math.Max(prmPoints[i].Z, maxZ);
                }
            }
            return new Double[] { minZ, maxZ };
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// This function reads the LAS file and arrange points into the specific grid.
        /// It also applies filtering for some given pre-defined gridding types.
        /// </summary>
        /// <typeparam name="T">Type of the LAS points</typeparam>
        /// <param name="prmReader">Tile LAS reader which reads LAS points by tile</param>
        /// <param name="prmObjects">A collection of grid data objects (1 per gridding type)</param>
        private void Grid<T>(TcTileLasReader prmReader, IEnumerable<TcGridObject> prmObjects) where T : TiLasPoint
        {
            TcLasPointBase[] points;
            TcTileBlockInfo info;

            for (int row = 0; row < m_Info.TileInfo.Row; row++)
            {
                for (int col = 0; col < m_Info.TileInfo.Col; col++)
                {
                    if ((info = m_Info.TileBlocks.FirstOrDefault(iter => iter.Row == row && iter.Col == col)) != null)
                    {
                        points = prmReader.GetPointObjectsByTile<T>(row, col, m_LasHeader);

                        foreach (TcGridObject gridObject in prmObjects)
                        {
                            // Report the message.
                            ReportMessage(String.Format("Processing Tile({0},{1}) of {2} ({3}m)", row, col, TcEnums.ShortName(gridObject.Type), gridObject.GridSize));

                            // Filter points that are not required by 1M grid.
                            if (gridObject.Type == TeGriddingType.M1FirstEcho || gridObject.Type == TeGriddingType.M1LastEcho)
                            {
                                FilterOutOfInterestPoints(points);
                            }

                            UpdateGrid(gridObject, info, points);
                        }
                    }
                }
            }

            foreach (TcGridObject gridObject in prmObjects)
            {
                // Write the tor file.
                TcTolObject objDiff = new TcTolObject(gridObject.OutputFile)
                {
                    Rows = m_Info.TileInfo.Row * gridObject.GridCount,
                    Columns = m_Info.TileInfo.Col * gridObject.GridCount,
                    UpperLeftEast = m_LasHeader.MinX,
                    UpperLeftNorth = m_LasHeader.MaxY,
                    LowerRightEast = m_LasHeader.MaxX,
                    LowerRightNorth = m_LasHeader.MinY,
                    MinHeight = (Single)gridObject.MinZ,
                    MaxHeight = (Single)gridObject.MaxZ,
                    ScalingX = gridObject.GridSize,
                    ScalingY = gridObject.GridSize,
                    Model = m_CoordinateSystem,
                    BitSize = 32
                };

                // Report the message.
                ReportMessage(String.Format("Writing {0}", Path.GetFileName(gridObject.OutputFile)));

                TcTorObject objTor = new TcTorObject(objDiff) { Blocks = gridObject.TorBlocks };
                using (TcTorWriter writer = new TcTorWriter(objDiff))
                {
                    writer.WriteTol();
                    writer.Write(objTor);
                }
            }

            ReportFinished();
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Internal function to produce a single grid file.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmObjects">Grid objects to keep the information together</param>
        private void Grid(String prmInput, IEnumerable<TcGridObject> prmObjects)
        {
            using (TcTileLasReader reader = new TcTileLasReader(prmInput, m_Info))
            {
                m_LasHeader = reader.Header;
                switch (reader.Header.PointDataFormatID)
                {
                    case 0:
                        Grid<TsLasPoint0>(reader, prmObjects);
                        break;

                    case 1:
                        Grid<TsLasPoint1>(reader, prmObjects);
                        break;

                    case 2:
                        Grid<TsLasPoint2>(reader, prmObjects);
                        break;

                    case 3:
                        Grid<TsLasPoint3>(reader, prmObjects);
                        break;

                    case 4:
                        Grid<TsLasPoint4>(reader, prmObjects);
                        break;

                    case 5:
                        Grid<TsLasPoint5>(reader, prmObjects);
                        break;

                    case 6:
                        Grid<TsLasPoint6>(reader, prmObjects);
                        break;

                    case 7:
                        Grid<TsLasPoint7>(reader, prmObjects);
                        break;

                    case 8:
                        Grid<TsLasPoint8>(reader, prmObjects);
                        break;

                    case 9:
                        Grid<TsLasPoint9>(reader, prmObjects);
                        break;

                    case 10:
                        Grid<TsLasPoint10>(reader, prmObjects);
                        break;

                    default:
                        throw new FormatException("Couldn't process the tile. LAS format not supported");
                }
            }
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a list of grid files.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmGridTypes">List of grid types</param>
        public void Grid(String prmInput, IEnumerable<TeGriddingType> prmGridTypes)
        {
            List<TcGridObject> gridObjects = new List<TcGridObject>(prmGridTypes.Count());
            String xmlFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            m_Info = TcTileUtils.GetTileBlocks(xmlFile);

            // Type defined las tiling.
            foreach (TeGriddingType type in prmGridTypes)
            {
                String outputFile = String.Format(@"{0}\{1}_{2}.tor"
                                        , m_OutputDirectory
                                        , Path.GetFileNameWithoutExtension(prmInput)
                                        , TcEnums.ShortName(type));
                gridObjects.Add(new TcGridObject(outputFile, type, m_Info));
            }

            Grid(prmInput, gridObjects);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a list of grid files.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmGridSizes">List of grid sizes</param>
        public void Grid(String prmInput, IEnumerable<Int32> prmGridSizes)
        {
            List<TcGridObject> gridObjects = new List<TcGridObject>(prmGridSizes.Count());
            String xmlFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            m_Info = TcTileUtils.GetTileBlocks(xmlFile);

            // Type defined las tiling.
            foreach (Int32 size in prmGridSizes.Distinct())
            {
                String outputFile = String.Format(@"{0}\{1}_m{2}.tor"
                                        , m_OutputDirectory
                                        , Path.GetFileNameWithoutExtension(prmInput)
                                        , size);

                gridObjects.Add(new TcGridObject(outputFile, size, m_Info));
            }

            Grid(prmInput, gridObjects);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a list of grid files.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmGridTypes">List of grid types</param>
        /// <param name="prmGridSizes">List of grid sizes</param>
        public void Grid(String prmInput, IEnumerable<TeGriddingType> prmGridTypes, IEnumerable<Int32> prmGridSizes)
        {
            List<TcGridObject> gridObjects = new List<TcGridObject>(prmGridTypes.Count() + prmGridSizes.Count());
            String xmlFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            m_Info = TcTileUtils.GetTileBlocks(xmlFile);

            // Type defined las tiling.
            foreach (TeGriddingType type in prmGridTypes)
            {
                String outputFile = String.Format(@"{0}\{1}_{2}.tor"
                                        , m_OutputDirectory
                                        , Path.GetFileNameWithoutExtension(prmInput)
                                        , TcEnums.ShortName(type));
                gridObjects.Add(new TcGridObject(outputFile, type, m_Info));
            }

            // Type defined las tiling.
            foreach (Int32 size in prmGridSizes.Distinct())
            {
                String outputFile = String.Format(@"{0}\{1}_m{2}.tor"
                                        , m_OutputDirectory
                                        , Path.GetFileNameWithoutExtension(prmInput)
                                        , size);

                gridObjects.Add(new TcGridObject(outputFile, size, m_Info));
            }

            Grid(prmInput, gridObjects);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a single grid file with a given type.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmOutputFileName">Output tor file name</param>
        /// <param name="prmGridType">Grid type</param>
        public void Grid(String prmInput, String prmOutputFileName, TeGriddingType prmGridType)
        {
            // Start of the actual function.
            String blockFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            m_Info = TcTileUtils.GetTileBlocks(blockFile);
            
            String outputFile = String.Format(@"{0}\{1}", m_OutputDirectory, prmOutputFileName);
            Grid(prmInput, new List<TcGridObject>(1) { new TcGridObject(outputFile, prmGridType, m_Info) });
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a single grid file with a given type.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmOutputFileName">Output tor file name</param>
        /// <param name="prmGridSize">Size of the grid</param>
        public void Grid(String prmInput, String prmOutputFileName, Int32 prmGridSize)
        {
            // Start of the actual function.
            String blockFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            m_Info = TcTileUtils.GetTileBlocks(blockFile);
            
            String outputFile = String.Format(@"{0}\{1}", m_OutputDirectory, prmOutputFileName);
            Grid(prmInput, new List<TcGridObject>(1) { new TcGridObject(outputFile, prmGridSize, m_Info) });
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a list of grid files with pre-defined types.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        public void GridAllPredefine(String prmInput)
        {
            Grid(prmInput, Enum.GetValues(typeof(TeGriddingType)).Cast<TeGriddingType>().Where(iter => iter != TeGriddingType.Unknown));
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Open function to produce a list of grid files with pre-defined types.
        /// </summary>
        /// <param name="prmInput">Input las file</param>
        /// <param name="prmGridSize">Size of the grid</param>
        public void GridAllPredefine(String prmInput, List<Int32> prmGridSizes)
        {
            Grid(prmInput, Enum.GetValues(typeof(TeGriddingType)).Cast<TeGriddingType>().Where(iter => iter != TeGriddingType.Unknown), prmGridSizes);
        }
        //-----------------------------------------------------------------------------

        public void Dispose()
        {
            OnMessage = null;
            OnError = null;
            OnFinish = null;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------