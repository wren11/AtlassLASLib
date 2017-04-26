using System;
using System.Collections.Generic;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Operations.Types;
using Atlass.LAS.Lib.Support.Types;

namespace Atlass.LAS.Lib.Operations.Gridding.Types
{
    public class TcGridObject
    {
        /// <summary>
        /// File to export this grid object into.
        /// </summary>
        public String OutputFile { get; set; }

        /// <summary>
        /// Type of the gridding (i.e. 1m First/Last Echo or 2m Flat etc.)
        /// </summary>
        public TeGriddingType Type { get; set; }

        /// <summary>
        /// Size of the each small block where a single point resides.
        /// </summary>
        public Int32 GridSize { get; set; }
        
        /// <summary>
        /// Number of grids in each tile.
        /// </summary>
        public Int32 GridCount { get; set; }

        /// <summary>
        /// Minimum height for the gridded object in meters.
        /// </summary>
        public Double MinZ { get; set; }

        /// <summary>
        /// Maximum height for the gridded object in meters.
        /// </summary>
        public Double MaxZ { get; set; }
        
        /// <summary>
        /// Maximum number of rows to be allowed in one block.
        /// </summary>
        public Int32 MaxRowsInGridBlock { get; private set; }

        /// <summary>
        /// Total number of grid blocks to be included in the TOR.
        /// </summary>
        public Int32 NumberOfGridBlocks { get; private set; }
        
        /// <summary>
        /// Collection of heights gridded according to the GridSubSize.
        /// </summary>
        public List<TcTorBlock32> TorBlocks;

        /// <summary>
        /// Collection of tile blocks from the indexed LAS.
        /// </summary>
        public TcTileBlockInfoCollection Info { get; set; }

        /// <summary>
        /// Height and width of the grid in meters.
        /// </summary>
        public Int32 TileSize { get { return Info != null ? Info.TileInfo.TileSize : 0; } }

        public TcGridObject(String prmFile, TeGriddingType prmType, TcTileBlockInfoCollection prmInfo)
        {
            Type = prmType;
            GridSize = GetGridSize(prmType);
            Initialize(prmFile, prmInfo);
        }
        //-----------------------------------------------------------------------------

        public TcGridObject(String prmFile, Int32 prmGridSize, TcTileBlockInfoCollection prmInfo)
        {
            Type = TeGriddingType.Unknown;
            GridSize = prmGridSize;
            Initialize(prmFile, prmInfo);
        }
        //-----------------------------------------------------------------------------

        private void Initialize(String prmFile, TcTileBlockInfoCollection prmInfo)
        {
            OutputFile = prmFile;
            Info = prmInfo;
            GridCount = (Int32)Math.Ceiling(TileSize * 1.0 / GridSize);
            MinZ = Double.MaxValue;
            MaxZ = Double.MinValue;
            MaxRowsInGridBlock = (Int32)Math.Floor(TcConstants.MaxBytesToLoadInTorBlock * 1.0 / (Info.TileInfo.Col * GridCount * sizeof(Single)));
            NumberOfGridBlocks = (Int32)Math.Ceiling(Info.TileInfo.Row * GridCount * 1.0 / MaxRowsInGridBlock);
            TorBlocks = new List<TcTorBlock32>();

            Int32 rowsProcessed = 0;
            for (int gc = 0; gc < NumberOfGridBlocks; gc++)
            {
                Int32 rowsInGrid = Math.Min(MaxRowsInGridBlock, Info.TileInfo.Row * GridCount - rowsProcessed);
                TcTorBlock32 block = new TcTorBlock32(gc, rowsInGrid, prmInfo.TileInfo.Col * GridCount);
                for (int r = 0; r < rowsInGrid; r++)
                {
                    for (int c = 0; c < prmInfo.TileInfo.Col * GridCount; c++)
                    {
                        block.Points[r, c] = TcConstants.TorNullValue32Bit;
                    }
                }
                TorBlocks.Add(block);
            }
        }
        //-----------------------------------------------------------------------------

        private Int32 GetGridSize(TeGriddingType prmType)
        {
            switch (prmType)
            {
                case TeGriddingType.M1FirstEcho:
                    return 1;

                case TeGriddingType.M1LastEcho:
                    return 1;

                case TeGriddingType.Levelling:
                    return 2;

                case TeGriddingType.Display:
                    return 5;

                default:
                    return 20;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
