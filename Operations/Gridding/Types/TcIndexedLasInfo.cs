using System;
using System.IO;
using Atlass.LAS.Lib.Operations.Tiling;
using Atlass.LAS.Lib.Operations.Tiling.Types;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Operations.Gridding.Types
{
    public class TcIndexedLasInfo
    {
        public String LasFile { get; private set; }
        public String IndexFile { get; private set; }
        public TcTileBlockInfoCollection TileInfoCollection { get; private set; }
        public TiLasHeader Header { get; set; }

        public TcIndexedLasInfo(String prmFile)
        {
            LasFile = prmFile;
            IndexFile = String.Format(@"{0}\{1}.xml", Path.GetDirectoryName(LasFile), Path.GetFileNameWithoutExtension(LasFile));

            if (File.Exists(IndexFile))
            {
                TileInfoCollection = TcTileUtils.GetTileBlocks(IndexFile);
            }
        }
    }
}
