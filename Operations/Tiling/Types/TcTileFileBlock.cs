using System;
using System.Collections.Generic;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    public class TcTileFileBlock
    {
        public Int32 Index { get; set; }
        public String File { get; private set; }
        public HashSet<TcTileBlockInfo> TileBlocks { get; private set; }

        public TcTileFileBlock(String prmFile)
        {
            File = prmFile;
            TileBlocks = new HashSet<TcTileBlockInfo>();

            Index = -1;
        }
    }
}
