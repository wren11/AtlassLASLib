using System;
using System.Collections.Generic;
using Atlass.LAS.Lib.Support.Types.Interfaces;

namespace Atlass.LAS.Lib.Support.Types
{
    public class TcTorObject
    {
        /// <summary>
        /// Information as a tol object.
        /// </summary>
        public TcTolObject Info { get; set; }

        /// <summary>
        /// List of blocks containing points.
        /// </summary>
        public List<TcTorBlock32> Blocks { get; set; }

        /// <summary>
        /// Maximum number of rows allowed in a block.
        /// </summary>
        public Int32 MaxRowsPerBlock { get; private set; }

        /// <summary>
        /// Number of blocks available.
        /// </summary>
        public Int32 TotalBlocks { get { return Blocks != null ? Blocks.Count : 0; } }

        public TcTorObject(TcTolObject prmInfo)
        {
            Info = prmInfo;
            Blocks = new List<TcTorBlock32>();

            MaxRowsPerBlock = 1;
        }
    }
}
