///<summary> TcLasTileMaker
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class exports the indexed LAS files into tiles with a given tile size and
/// offset around it. The naming convention for the tile files is:
/// T[$INDEX]_E{$EAST}_N{$NORTH}.las

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
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Operations.Gridding.Types;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Operations.Tiling.IO;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Operations.Types;
using Atlass.LAS.Lib.Types;
using Atlass.LAS.Lib.Types.DataType;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;
using Atlass.LAS.Lib.Utilities;

namespace Atlass.LAS.Lib.Operations.Tiling
{
    public class TcLasTileMaker : TiOperation, IDisposable
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

        public TeTilingMethod TilingMethod { private get; set; }
        public Int32 TileSize { private get; set; }
        public Int32 Buffer { private get; set; }
        public Int32 ScalingFactor { private get; set; }
        public Boolean OverwriteExistingTiles { private get; set; }
        public Boolean ApplyFiltering { private get; set; }
        public String SoftwareName { private get; set; }

        // Constructor parameters.
        private String m_OutputDirectory;
        private Int32 m_LasPointFormat;

        // Internal private data.
        private Int32 m_TileIndex;
        private Int32 m_Rows;
        private Int32 m_Columns;
        private Int32 m_TilesToProcessAtOnce;
        private TcRectangle m_FullArea;
        private TiLasHeader m_AreaHeader;
        private List<TcTileGrid> m_TilePolygons;
        private Dictionary<String, TcIndexedLasInfo> m_TileInfos;

        public TcLasTileMaker(Int32 prmLasPointFormat, String prmOutputDirectory)
        {
            // Local initialization.
            m_TileIndex = 0;
            m_LasPointFormat = prmLasPointFormat;
            m_OutputDirectory = prmOutputDirectory;

            TilingMethod = TeTilingMethod.OneRowAtOnce;
            TileSize = 1000;
            Buffer = 0;
            ScalingFactor = 1;
            OverwriteExistingTiles = false;
            ApplyFiltering = true;
            SoftwareName = "Atlass LAS";

            m_TilePolygons = new List<TcTileGrid>();
            m_TileInfos = new Dictionary<String, TcIndexedLasInfo>();
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

        private void ReportProgress(Int32 prmTileIndex, Double prmProgress)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new TcMessageEventArgs(String.Format("Tile {0} {1:0.0}% completed", prmTileIndex, prmProgress)));
            }
        }
        //-----------------------------------------------------------------------------

        private void InitializeGrid(Double prmULE, Double prmULN, Double prmLRE, Double prmLRN)
        {
            m_FullArea = new TcRectangle
                (
                    prmULE - (prmULE % (TileSize * ScalingFactor)),
                    prmULN + (TileSize * ScalingFactor - (prmULN % (TileSize * ScalingFactor))),
                    prmLRE + (TileSize * ScalingFactor - (prmLRE % (TileSize * ScalingFactor))),
                    prmLRN - (prmLRN % (TileSize * ScalingFactor))
                );

            m_Rows = (Int32)((m_FullArea.UpperLeftY - m_FullArea.LowerRightY) / TileSize);
            m_Columns = (Int32)((m_FullArea.LowerRightX - m_FullArea.UpperLeftX) / TileSize);
            m_TilesToProcessAtOnce = TilingMethod == TeTilingMethod.OneRowAtOnce ? m_Columns : (Int32)TilingMethod;

            // Create the polygons to be loaded.
            for (int i = 0; i < m_Rows; i++)
            {
                for (int j = 0; j < m_Columns; j++)
                {
                    m_TilePolygons.Add
                        (
                            new TcTileGrid
                                (
                                    m_FullArea.UpperLeftX + j * TileSize - Buffer,
                                    m_FullArea.UpperLeftY - i * TileSize + Buffer,
                                    m_FullArea.UpperLeftX + (j + 1) * TileSize + Buffer,
                                    m_FullArea.UpperLeftY - (i + 1) * TileSize - Buffer
                                ) { Row = i, Col = j }
                        );
                }
            }

            // Write the tiles into a coo file.
            using (StreamWriter write = new StreamWriter(new FileStream(String.Format(@"{0}\atlass_tiling_grid.coo", m_OutputDirectory), FileMode.Create)))
            {
                foreach (TcRectangle tile in m_TilePolygons)
                {
                    write.WriteLine(String.Format("{0:0.0000} {1:0.0000} 0.0", tile.UpperLeftX, tile.UpperLeftY));
                    write.WriteLine(String.Format("{0:0.0000} {1:0.0000} 0.0", tile.LowerRightX, tile.UpperLeftY));
                    write.WriteLine(String.Format("{0:0.0000} {1:0.0000} 0.0", tile.LowerRightX, tile.LowerRightY));
                    write.WriteLine(String.Format("{0:0.0000} {1:0.0000} 0.0", tile.UpperLeftX, tile.LowerRightY));
                    write.WriteLine(String.Format("{0:0.0000} {1:0.0000} 0.0", tile.UpperLeftX, tile.UpperLeftY));
                    write.WriteLine(String.Empty);
                }
            }
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Calculates the size of the canvas and determine the number of required tiles.
        /// It then creates polygons (rectangle) for each area considering the given offset.
        /// </summary>
        /// <param name="prmFiles">List of LAS files to be processed</param>
        private void UpdateCommonHeader(IEnumerable<String> prmFiles)
        {
            m_AreaHeader = GetDefaultLasHeader();

            foreach (String lasFile in prmFiles)
            {
                using (TcLasReader lasReader = new TcLasReader(lasFile))
                {
                    m_AreaHeader.MinX = Math.Min(m_AreaHeader.MinX, lasReader.Header.MinX);
                    m_AreaHeader.MinY = Math.Min(m_AreaHeader.MinY, lasReader.Header.MinY);
                    m_AreaHeader.MinZ = Math.Min(m_AreaHeader.MinZ, lasReader.Header.MinZ);
                    m_AreaHeader.MaxX = Math.Max(m_AreaHeader.MaxX, lasReader.Header.MaxX);
                    m_AreaHeader.MaxY = Math.Max(m_AreaHeader.MaxY, lasReader.Header.MaxY);
                    m_AreaHeader.MaxZ = Math.Max(m_AreaHeader.MaxZ, lasReader.Header.MaxZ);

                    m_AreaHeader.XOffset = Math.Min(m_AreaHeader.XOffset, lasReader.Header.XOffset);
                    m_AreaHeader.YOffset = Math.Min(m_AreaHeader.YOffset, lasReader.Header.YOffset);
                    m_AreaHeader.ZOffset = Math.Min(m_AreaHeader.ZOffset, lasReader.Header.ZOffset);

                    m_AreaHeader.XScaleFactor = Math.Min(m_AreaHeader.XScaleFactor, lasReader.Header.XScaleFactor);
                    m_AreaHeader.YScaleFactor = Math.Min(m_AreaHeader.YScaleFactor, lasReader.Header.YScaleFactor);
                    m_AreaHeader.ZScaleFactor = Math.Min(m_AreaHeader.ZScaleFactor, lasReader.Header.ZScaleFactor);

                    m_TileInfos[lasFile] = new TcIndexedLasInfo(lasFile) { Header = lasReader.Header };
                }
            }
        }
        //-----------------------------------------------------------------------------

        private TiLasHeader GetDefaultLasHeader()
        {
            TiLasHeader header;
            String SystemID = "Atlass HCC";
            if (m_LasPointFormat >= 0 && m_LasPointFormat <= 3)
            {
                header = new TsLasHeader12()
                {
                    FileSignature = "LASF".ToCharArray(),
                    SourceID = 0,
                    GlobalEncoding = 0,
                    GUID1 = 0,
                    GUID2 = 0,
                    GUID3 = 0,
                    GUID4 = String.Empty.PadRight(8).ToCharArray(),
                    VersionMajor = 1,
                    VersionMinor = 2,
                    SystemID = SystemID.PadRight(32).ToCharArray(),
                    GeneratingSoftware = SoftwareName.PadRight(32).ToCharArray(),
                    NumberOfVariableLengthRecords = 1,
                    NumberOfPointRecords = 0,
                    NumberofPointsByReturn1 = 0,
                    NumberofPointsByReturn2 = 0,
                    NumberofPointsByReturn3 = 0,
                    NumberofPointsByReturn4 = 0,
                    NumberofPointsByReturn5 = 0
                };
            }
            else if (m_LasPointFormat >= 4 && m_LasPointFormat <= 5)
            {
                header = new TsLasHeader13()
                {
                    FileSignature = "LASF".ToCharArray(),
                    SourceID = 0,
                    GlobalEncoding = 0,
                    GUID1 = 0,
                    GUID2 = 0,
                    GUID3 = 0,
                    GUID4 = String.Empty.PadRight(8).ToCharArray(),
                    VersionMajor = 1,
                    VersionMinor = 3,
                    SystemID = SystemID.PadRight(32).ToCharArray(),
                    GeneratingSoftware = SoftwareName.PadRight(32).ToCharArray(),
                    NumberOfVariableLengthRecords = 1,
                    NumberOfPointRecords = 0,
                    NumberofPointsByReturn1 = 0,
                    NumberofPointsByReturn2 = 0,
                    NumberofPointsByReturn3 = 0,
                    NumberofPointsByReturn4 = 0,
                    NumberofPointsByReturn5 = 0,
                    StartOfWaveFormData = 0
                };
            }
            else
            {
                header = new TsLasHeader14()
                {
                    FileSignature = "LASF".ToCharArray(),
                    SourceID = 0,
                    GlobalEncoding = 0,
                    GUID1 = 0,
                    GUID2 = 0,
                    GUID3 = 0,
                    GUID4 = String.Empty.PadRight(8).ToCharArray(),
                    VersionMajor = 1,
                    VersionMinor = 4,
                    SystemID = SystemID.PadRight(32).ToCharArray(),
                    GeneratingSoftware = SoftwareName.PadRight(32).ToCharArray(),
                    NumberOfVariableLengthRecords = 1,
                    LegNumberOfPointRecords = 0,
                    LegNumberofPointsByReturn1 = 0,
                    LegNumberofPointsByReturn2 = 0,
                    LegNumberofPointsByReturn3 = 0,
                    LegNumberofPointsByReturn4 = 0,
                    LegNumberofPointsByReturn5 = 0,
                    NumberOfPointRecords = 0,
                    NumberofPointsByReturn1 = 0,
                    NumberofPointsByReturn2 = 0,
                    NumberofPointsByReturn3 = 0,
                    NumberofPointsByReturn4 = 0,
                    NumberofPointsByReturn5 = 0,
                    NumberofPointsByReturn6 = 0,
                    NumberofPointsByReturn7 = 0,
                    NumberofPointsByReturn8 = 0,
                    NumberofPointsByReturn9 = 0,
                    NumberofPointsByReturn10 = 0,
                    NumberofPointsByReturn11 = 0,
                    NumberofPointsByReturn12 = 0,
                    NumberofPointsByReturn13 = 0,
                    NumberofPointsByReturn14 = 0,
                    NumberofPointsByReturn15 = 0,
                    StartOfWaveFormData = 0
                };
            }

            header.HeaderSize = (UInt16)Marshal.SizeOf(header);
            header.PointOffset = (UInt32)header.HeaderSize;
            header.PointDataFormatID = (Byte)m_LasPointFormat;
            header.PointDataRecordLength = header.GetPointSize();
            header.DoY = (UInt16)DateTime.Today.DayOfYear;
            header.Year = (UInt16)DateTime.Now.Year;
            header.MinX = Double.MaxValue;
            header.MinY = Double.MaxValue;
            header.MinZ = Double.MaxValue;
            header.MaxX = Double.MinValue;
            header.MaxY = Double.MinValue;
            header.MaxZ = Double.MinValue;
            header.XOffset = Double.MaxValue;
            header.YOffset = Double.MaxValue;
            header.ZOffset = Double.MaxValue;
            header.XScaleFactor = Double.MaxValue;
            header.YScaleFactor = Double.MaxValue;
            header.ZScaleFactor = Double.MaxValue;

            return header;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Adjusts the LAS points for the tile and update the tile header. It takes the las file header for the block
        /// of points and the modified header for this specific tile. It then update the points by taking the common LAS
        /// header for the full area as reference. It also updates the tile header from the computed values of the points.
        /// </summary>
        /// <typeparam name="T">Type of the las point</typeparam>
        /// <param name="prmTileHeader">LAS header for this specific tile</param>
        /// <param name="prmLasHeader">LAS header for the las file where this points belong to</param>
        /// <param name="prmLasPoints">An array of las points to be updated</param>
        /// <returns>The header as an interface to converted to the specific header structure.</returns>
        private TiLasHeader UpdateLasInformation<T>(TiLasHeader prmTileHeader, TiLasHeader prmLasHeader, T[] prmLasPoints, Boolean prmModifyPoints) where T : TiLasPoint
        {
            Double X, Y, Z;
            if (prmTileHeader is TsLasHeader12)
            {
                TsLasHeader12 tileHeader = (TsLasHeader12)prmTileHeader;
                for (int i = 0; i < prmLasPoints.Length; i++)
                {
                    X = prmLasHeader.XOffset + prmLasPoints[i].X * prmLasHeader.XScaleFactor;
                    Y = prmLasHeader.YOffset + prmLasPoints[i].Y * prmLasHeader.YScaleFactor;
                    Z = prmLasHeader.ZOffset + prmLasPoints[i].Z * prmLasHeader.ZScaleFactor;

                    if (prmModifyPoints)
                    {
                        prmLasPoints[i].X = (Int32)((X - m_AreaHeader.XOffset) / m_AreaHeader.XScaleFactor);
                        prmLasPoints[i].Y = (Int32)((Y - m_AreaHeader.YOffset) / m_AreaHeader.YScaleFactor);
                        prmLasPoints[i].Z = (Int32)((Z - m_AreaHeader.ZOffset) / m_AreaHeader.ZScaleFactor);
                    }

                    tileHeader.MinX = Math.Min(tileHeader.MinX, X);
                    tileHeader.MinY = Math.Min(tileHeader.MinY, Y);
                    tileHeader.MinZ = Math.Min(tileHeader.MinZ, Z);
                    tileHeader.MaxX = Math.Max(tileHeader.MaxX, X);
                    tileHeader.MaxY = Math.Max(tileHeader.MaxY, Y);
                    tileHeader.MaxZ = Math.Max(tileHeader.MaxZ, Z);
                    tileHeader.NumberOfPointRecords++;

                    switch (prmLasPoints[i].ReturnNumber())
                    {
                        case 1:
                            tileHeader.NumberofPointsByReturn1++;
                            break;
                        case 2:
                            tileHeader.NumberofPointsByReturn2++;
                            break;
                        case 3:
                            tileHeader.NumberofPointsByReturn3++;
                            break;
                        case 4:
                            tileHeader.NumberofPointsByReturn4++;
                            break;
                        case 5:
                            tileHeader.NumberofPointsByReturn5++;
                            break;
                    }
                }

                return tileHeader;
            }
            else if (prmTileHeader is TsLasHeader13)
            {
                TsLasHeader13 tileHeader = (TsLasHeader13)prmTileHeader;
                for (int i = 0; i < prmLasPoints.Length; i++)
                {
                    X = prmLasHeader.XOffset + prmLasPoints[i].X * prmLasHeader.XScaleFactor;
                    Y = prmLasHeader.YOffset + prmLasPoints[i].Y * prmLasHeader.YScaleFactor;
                    Z = prmLasHeader.ZOffset + prmLasPoints[i].Z * prmLasHeader.ZScaleFactor;
                    prmLasPoints[i].X = (Int32)((X - m_AreaHeader.XOffset) / m_AreaHeader.XScaleFactor);
                    prmLasPoints[i].Y = (Int32)((Y - m_AreaHeader.YOffset) / m_AreaHeader.YScaleFactor);
                    prmLasPoints[i].Z = (Int32)((Z - m_AreaHeader.ZOffset) / m_AreaHeader.ZScaleFactor);

                    tileHeader.MinX = Math.Min(tileHeader.MinX, X);
                    tileHeader.MinY = Math.Min(tileHeader.MinY, Y);
                    tileHeader.MinZ = Math.Min(tileHeader.MinZ, Z);
                    tileHeader.MaxX = Math.Max(tileHeader.MaxX, X);
                    tileHeader.MaxY = Math.Max(tileHeader.MaxY, Y);
                    tileHeader.MaxZ = Math.Max(tileHeader.MaxZ, Z);
                    tileHeader.NumberOfPointRecords++;

                    switch (prmLasPoints[i].ReturnNumber())
                    {
                        case 1:
                            tileHeader.NumberofPointsByReturn1++;
                            break;
                        case 2:
                            tileHeader.NumberofPointsByReturn2++;
                            break;
                        case 3:
                            tileHeader.NumberofPointsByReturn3++;
                            break;
                        case 4:
                            tileHeader.NumberofPointsByReturn4++;
                            break;
                        case 5:
                            tileHeader.NumberofPointsByReturn5++;
                            break;
                    }
                }

                return tileHeader;
            }
            else if (prmTileHeader is TsLasHeader14)
            {
                TsLasHeader14 tileHeader = (TsLasHeader14)prmTileHeader;
                for (int i = 0; i < prmLasPoints.Length; i++)
                {
                    X = prmLasHeader.XOffset + prmLasPoints[i].X * prmLasHeader.XScaleFactor;
                    Y = prmLasHeader.YOffset + prmLasPoints[i].Y * prmLasHeader.YScaleFactor;
                    Z = prmLasHeader.ZOffset + prmLasPoints[i].Z * prmLasHeader.ZScaleFactor;
                    prmLasPoints[i].X = (Int32)((X - m_AreaHeader.XOffset) / m_AreaHeader.XScaleFactor);
                    prmLasPoints[i].Y = (Int32)((Y - m_AreaHeader.YOffset) / m_AreaHeader.YScaleFactor);
                    prmLasPoints[i].Z = (Int32)((Z - m_AreaHeader.ZOffset) / m_AreaHeader.ZScaleFactor);

                    tileHeader.MinX = Math.Min(tileHeader.MinX, X);
                    tileHeader.MinY = Math.Min(tileHeader.MinY, Y);
                    tileHeader.MinZ = Math.Min(tileHeader.MinZ, Z);
                    tileHeader.MaxX = Math.Max(tileHeader.MaxX, X);
                    tileHeader.MaxY = Math.Max(tileHeader.MaxY, Y);
                    tileHeader.MaxZ = Math.Max(tileHeader.MaxZ, Z);
                    tileHeader.NumberOfPointRecords++;
                    tileHeader.LegNumberOfPointRecords++;

                    switch (prmLasPoints[i].ReturnNumber())
                    {
                        case 1:
                            tileHeader.NumberofPointsByReturn1++;
                            tileHeader.LegNumberofPointsByReturn1++;
                            break;
                        case 2:
                            tileHeader.NumberofPointsByReturn2++;
                            tileHeader.LegNumberofPointsByReturn2++;
                            break;
                        case 3:
                            tileHeader.NumberofPointsByReturn3++;
                            tileHeader.LegNumberofPointsByReturn3++;
                            break;
                        case 4:
                            tileHeader.NumberofPointsByReturn4++;
                            tileHeader.LegNumberofPointsByReturn4++;
                            break;
                        case 5:
                            tileHeader.NumberofPointsByReturn5++;
                            tileHeader.LegNumberofPointsByReturn5++;
                            break;
                        case 6:
                            tileHeader.NumberofPointsByReturn6++;
                            break;
                        case 7:
                            tileHeader.NumberofPointsByReturn7++;
                            break;
                        case 8:
                            tileHeader.NumberofPointsByReturn8++;
                            break;
                        case 9:
                            tileHeader.NumberofPointsByReturn9++;
                            break;
                        case 10:
                            tileHeader.NumberofPointsByReturn10++;
                            break;
                        case 11:
                            tileHeader.NumberofPointsByReturn11++;
                            break;
                        case 12:
                            tileHeader.NumberofPointsByReturn12++;
                            break;
                        case 13:
                            tileHeader.NumberofPointsByReturn13++;
                            break;
                        case 14:
                            tileHeader.NumberofPointsByReturn14++;
                            break;
                        case 15:
                            tileHeader.NumberofPointsByReturn15++;
                            break;
                    }
                }

                return tileHeader;
            }

            throw new InvalidDataException("Couldn't update the header. Invalid data format.");
        }
        //-----------------------------------------------------------------------------

        private void ProcessTiles(IEnumerable<TcTileAreaBlock> prmTileAreaBlocks, IEnumerable<TcTileFileBlock> prmTileFileBlocks)
        {
            switch (m_LasPointFormat)
            {
                case 0:
                    ProcessTiles<TsLasPoint0>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 1:
                    ProcessTiles<TsLasPoint1>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 2:
                    ProcessTiles<TsLasPoint2>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 3:
                    ProcessTiles<TsLasPoint3>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 4:
                    ProcessTiles<TsLasPoint4>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 5:
                    ProcessTiles<TsLasPoint5>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 6:
                    ProcessTiles<TsLasPoint6>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 7:
                    ProcessTiles<TsLasPoint7>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 8:
                    ProcessTiles<TsLasPoint8>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 9:
                    ProcessTiles<TsLasPoint9>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                case 10:
                    ProcessTiles<TsLasPoint10>(prmTileAreaBlocks, prmTileFileBlocks);
                    break;

                default:
                    throw new FormatException("Couldn't process the tile. LAS format not supported");
            }
        }
        //-----------------------------------------------------------------------------

        private void ProcessTiles<T>(IEnumerable<TcTileAreaBlock> prmTileAreaBlocks
                                   , IEnumerable<TcTileFileBlock> prmTileFileBlocks) where T : TiLasPoint
        {
            // Load the LAS points into the memory.
            Dictionary<TcTileBlockInfo, T[]> loadedLasPoints = new Dictionary<TcTileBlockInfo, T[]>();

            try
            {
                foreach (TcTileFileBlock fileBlock in prmTileFileBlocks)
                {
                    if (fileBlock.TileBlocks.Count > 0)
                    {
                        // Callback for the message.
                        ReportMessage(String.Format("Loading blocks from file {0}", Path.GetFileNameWithoutExtension(fileBlock.File)));

                        using (TcTileLasReader lasReader = new TcTileLasReader(m_TileInfos[fileBlock.File]))
                        {
                            foreach (TcTileBlockInfo blockInfo in fileBlock.TileBlocks)
                            {
                                loadedLasPoints[blockInfo] = lasReader.GetPointsByTileBlock<T>(blockInfo);
                            }
                        }
                    }
                }

                // Point blocks that has been transformed.
                HashSet<TcTileBlockInfo> tranformedBlocks = new HashSet<TcTileBlockInfo>();

                // The points to be saved after filtering.
                List<T> filteredLasPoints = new List<T>();

                // Reference of the points to be written in file.
                T[] writablePoints;

                // LAS writer.
                TcTileLasWriter lasWriter = null;

                // Total number of points to be processed.
                Int64 glbTotalPointsToProcess = prmTileAreaBlocks.SelectMany(iter => iter.FileBlocks).SelectMany(iter => iter.TileBlocks).Sum(iter => iter.NoOfPoints);

                // Process each tile and write into the LAS file.
                foreach (TcTileAreaBlock areaBlock in prmTileAreaBlocks)
                {
                    // Points to be processed for this tile.
                    Int64 tilePointsToProcess = areaBlock.FileBlocks.SelectMany(iter => iter.TileBlocks).Sum(iter => iter.NoOfPoints);
                    
                    try
                    {
                        // Header for this tile.
                        TiLasHeader tileHeader = null;

                        // Progress to report to the upper layer.
                        Double progress = 0.0;

                        // Write the las points.
                        foreach (TcTileFileBlock fileBlock in areaBlock.FileBlocks)
                        {
                            TiLasHeader header = m_TileInfos[fileBlock.File].Header;

                            // Load the blocks of points from the specific LAS file.
                            foreach (TcTileBlockInfo blockInfo in fileBlock.TileBlocks)
                            {
                                // When the user wants to filter the points from the tile.
                                if (ApplyFiltering)
                                {
                                    filteredLasPoints.Clear();
                                    writablePoints = loadedLasPoints[blockInfo];

                                    for (int i = 0; i < writablePoints.Length; i++)
                                    {
                                        if (areaBlock.Area.Contains(header.XOffset + header.XScaleFactor * writablePoints[i].X, header.YOffset + header.YScaleFactor * writablePoints[i].Y))
                                        {
                                            filteredLasPoints.Add(writablePoints[i]);
                                        }
                                    }
                                    writablePoints = filteredLasPoints.ToArray();
                                }
                                else
                                {
                                    writablePoints = loadedLasPoints[blockInfo];
                                }

                                if (writablePoints.Length > 0)
                                {
                                    if (lasWriter == null)
                                    {
                                        ReportMessage(String.Format("{1}------------ Started Tile {0} ------------{1}", areaBlock.Index, Environment.NewLine));

                                        lasWriter = new TcTileLasWriter(areaBlock.OutputFile);

                                        // Get the default header based on version.
                                        tileHeader = GetDefaultLasHeader();

                                        // Write the default header based on version.
                                        lasWriter.WriteHeader(tileHeader);
                                    }

                                    // Get the updated point counts and header with Min/Max. Doesn't update if already updated once.
                                    tileHeader = UpdateLasInformation<T>(tileHeader, header, writablePoints, tranformedBlocks.Add(blockInfo) || ApplyFiltering);

                                    // Write the points after modifying the X Y Z based on a common offset.
                                    lasWriter.WritePoints<T>(writablePoints);
                                }

                                // Report the progress.
                                progress += (loadedLasPoints[blockInfo].Length) * 100.0 / tilePointsToProcess;
                            }

                            // Report the progress.
                            ReportProgress(areaBlock.Index, progress);
                        }

                        // Write the updated header if required.
                        if (tileHeader != null && lasWriter != null)
                        {
                            // Update the min-max offsets for the whole are.
                            tileHeader.XOffset = m_AreaHeader.XOffset;
                            tileHeader.YOffset = m_AreaHeader.YOffset;
                            tileHeader.ZOffset = m_AreaHeader.ZOffset;
                            tileHeader.XScaleFactor = m_AreaHeader.XScaleFactor;
                            tileHeader.YScaleFactor = m_AreaHeader.YScaleFactor;
                            tileHeader.ZScaleFactor = m_AreaHeader.ZScaleFactor;

                            lasWriter.WriteHeader(tileHeader);

                            ReportMessage(String.Format("{1}------------ Finished Tile#{0} ------------{1}", areaBlock.Index, Environment.NewLine));
                        }
                    }
                    finally
                    {
                        if (lasWriter != null)
                        {
                            lasWriter.Dispose();
                            lasWriter = null;
                        }
                    }
                }
            }
            finally
            {
                // Clean data.
                foreach (TcTileBlockInfo key in loadedLasPoints.Keys.ToList())
                {
                    loadedLasPoints[key] = new T[0];
                }
                loadedLasPoints.Clear();
            }
        }
        //-----------------------------------------------------------------------------

        private void ProcessBlockSets(IEnumerable<String> prmFiles, IEnumerable<TcTileAreaBlock> prmTileAreas)
        {
            Int32 fileIndex = 0;

            // Collection of LAS blocks to load per file.
            List<TcTileFileBlock> tileFileBlocks = prmFiles.Aggregate(new List<TcTileFileBlock>(), (res, iter) =>
                {
                    res.Add(new TcTileFileBlock(iter) { Index = fileIndex++ });
                    return res;
                });

            // The loop the create the collection of LAS blocks to load from each file and association to each tile area.
            foreach (TcTileFileBlock currentFileBlock in tileFileBlocks)
            {
                foreach (TcTileAreaBlock currentAreaBlock in prmTileAreas)
                {
                    // LAS blocks which intersects the given tile.
                    List<TcTileBlockInfo> blocks = m_TileInfos[currentFileBlock.File].TileInfoCollection.GetTileBlocks(currentAreaBlock.Area);
                    if (blocks.Count > 0)
                    {
                        // Include the blocks to the collection of loadable blocks for this tile.
                        currentFileBlock.TileBlocks.UnionWith(blocks);

                        // Create the group of point blocks to be added to this specific tile.
                        TcTileFileBlock newBlock = new TcTileFileBlock(currentFileBlock.File);
                        newBlock.TileBlocks.UnionWith(blocks);
                        currentAreaBlock.FileBlocks.Add(newBlock);
                    }
                }
            }

            // Process the current tiles and write them into file.
            ProcessTiles(prmTileAreas, tileFileBlocks);
        }
        //-----------------------------------------------------------------------------

        private void ProcessBlockSets(IEnumerable<String> prmFiles, IEnumerable<TcRectangle> prmRectangles)
        {
            // Collection of file blocks to use per tile.
            List<TcTileAreaBlock> tileAreaBlocks = prmRectangles.Aggregate(new List<TcTileAreaBlock>(), (res, iter) =>
            {
                TcTileAreaBlock areaBlock = new TcTileAreaBlock(iter) { Index = m_TileIndex++ };
                areaBlock.OutputFile = String.Format(@"{0}\T{1}_E{2:0}_N{3:0}.las", m_OutputDirectory, areaBlock.Index, areaBlock.Area.UpperLeftX, areaBlock.Area.UpperLeftY);
                res.Add(areaBlock);
                return res;
            });

            ProcessBlockSets(prmFiles, tileAreaBlocks);
        }
        //-----------------------------------------------------------------------------

        private void ProcessBlockSets(IEnumerable<String> prmFiles, TcRectangle prmTileArea, String prmOutputFileName)
        {
            TcTileAreaBlock areaBlock = new TcTileAreaBlock(prmTileArea) { Index = 0 };
            areaBlock.OutputFile = String.Format(@"{0}\{1}", m_OutputDirectory, prmOutputFileName);

            ProcessBlockSets(prmFiles, new List<TcTileAreaBlock>(1) { areaBlock });
        }
        //-----------------------------------------------------------------------------

        #region Public Function and Interface Members

        /// <summary>
        /// Processes a list of LAS files and convert them into a collection of tiles.
        /// It expects the las files to be presorted and comes with .xml indices with tile
        /// information. All this files are meant to be part of an area to get this function
        /// working correctly.
        /// </summary>
        /// <param name="prmFiles">List of LAS files to be processed</param>
        public void ExportAll(IEnumerable<String> prmFiles)
        {
            m_TileIndex = 0;

            ReportMessage("Creating the common header");

            // Function to update the list of tiles to be processed.
            UpdateCommonHeader(prmFiles);

            ReportMessage("Creating the grid");

            // Initialize the grid polygons.
            InitializeGrid(m_AreaHeader.MinX, m_AreaHeader.MaxY, m_AreaHeader.MaxX, m_AreaHeader.MinY);

            // Total number of tiles processed.
            Int32 tilesProcessed = 0;

            while (tilesProcessed < m_Rows * m_Columns)
            {
                // How many tiles need to process in one go.
                Int32 tilesToProcess = Math.Min(m_Rows * m_Columns - tilesProcessed, m_TilesToProcessAtOnce);

                ReportMessage(String.Format("Processing {0}/{1} tiles", tilesToProcess, m_Rows * m_Columns));

                // Process a set of blocks to be processed for the given tiles.
                ProcessBlockSets(prmFiles, m_TilePolygons.Skip(tilesProcessed).Take(tilesToProcess));

                ReportMessage("Cleaning the memory");

                // Clean memory.
                GC.Collect();

                // Update the process counter.
                tilesProcessed += tilesToProcess;
            }

            ReportFinished();
        }
        //-----------------------------------------------------------------------------

        public void ExportPolygon(IEnumerable<String> prmFiles, TcPolygon prmPolygon, String prmOutputFile)
        {
            // Function to update the list of tiles to be processed.
            UpdateCommonHeader(prmFiles);

            // Process a set of blocks to be processed for the given tiles.
            ProcessBlockSets(prmFiles, new TcPolygon(prmPolygon.Points, Buffer), prmOutputFile);

            ReportFinished();
        }
        //-----------------------------------------------------------------------------

        public void ExportPolygons(IEnumerable<String> prmFiles, String prmPolygonFile)
        {
            // Function to update the list of tiles to be processed.
            UpdateCommonHeader(prmFiles);

            // Not using the collection function to avoid multiple transformation of the overlapping points.
            foreach (TcRectangle rect in TcFileUtils.GetPolygons(prmPolygonFile, Buffer))
            {
                // Process a set of blocks to be processed for the given tiles.
                ProcessBlockSets(prmFiles, rect, String.Format("{0}.las", Path.GetFileNameWithoutExtension(prmPolygonFile)));
            }

            ReportFinished();
        }
        //-----------------------------------------------------------------------------

        public void TrimTile(String prmInputFile, Double prmMetersToTrim, String prmOutputFileName)
        {
            // Function to update the list of tiles to be processed.
            UpdateCommonHeader(new List<String>(1) { prmInputFile });

            TcRectangle rectangle = new TcRectangle
                (
                    m_AreaHeader.MinX + prmMetersToTrim,
                    m_AreaHeader.MaxY - prmMetersToTrim,
                    m_AreaHeader.MaxX - prmMetersToTrim,
                    m_AreaHeader.MinY + prmMetersToTrim
                );

            // Process a set of blocks to be processed for the given tiles.
            ProcessBlockSets(new List<String>(1) { prmInputFile }, rectangle, prmOutputFileName);
        }
        //-----------------------------------------------------------------------------

        public void Dispose()
        {
            m_TileInfos.Clear();
            m_TilePolygons.Clear();
        }
        //-----------------------------------------------------------------------------

        #endregion Public Function and Interface Members

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------