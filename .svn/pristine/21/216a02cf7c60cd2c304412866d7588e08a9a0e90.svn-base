using System;
using Atlass.LAS.Lib.Support.Types.Interfaces;

namespace Atlass.LAS.Lib.Support.Types
{
    public class TcTorBlock32 : TiTorBlock
    {
        /// <summary>
        /// Index of this block in the tor file.
        /// </summary>
        public Int32 Index { get; set; }

        /// <summary>
        /// Number of rows int the block.
        /// </summary>
        public Int32 Rows { get; set; }

        /// <summary>
        /// Number of columns in the block.
        /// </summary>
        public Int32 Columns { get; set; }

        /// <summary>
        /// East (X) the upper left corner.
        /// </summary>
        public Double East { get; set; }

        /// <summary>
        /// North (Y) of the upper left corner.
        /// </summary>
        public Double North { get; set; }

        /// <summary>
        /// Information as a tol object.
        /// </summary>
        public TcTolObject Info { get; set; }

        /// <summary>
        /// An array of heights represented. 
        /// </summary>
        public Single[,] Points { get; set; }

        public TcTorBlock32(Int32 prmIndex, Int32 prmRows, Int32 prmColumns)
        {
            Index = prmIndex;
            Rows = prmRows;
            Columns = prmColumns;
            East = 0;
            North = 0;
            Points = new Single[prmRows, prmColumns];
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
