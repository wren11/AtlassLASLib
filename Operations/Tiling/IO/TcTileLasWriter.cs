using System;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Operations.Tiling.IO
{
    public class TcTileLasWriter : TcLasWriter
    {
        public TcTileLasWriter(String prmOutput)
            : base(prmOutput)
        {
        }

        /// <summary>
        /// Write the LAS points with adjusted X, Y and Z based on a common header.
        /// </summary>
        /// <typeparam name="T">Type of the LAS ponts</typeparam>
        /// <param name="prmPoints">LAS points</param>
        /// <param name="prmHeader">Actual header for these points</param>
        /// <param name="prmCommonHeader">Common header for a whole area</param>
        /// <param name="prmNoOfPoints">Total number of points to write from the array</param>
        public void WriteModifiedPoints<T>(T[] prmPoints, TiLasHeader prmHeader, TiLasHeader prmCommonHeader, Int64 prmNoOfPoints) where T : TiLasPoint
        {
            for (int i = 0; i < prmNoOfPoints; i++)
            {
                prmPoints[i].X = (Int32)(((prmHeader.XOffset + prmPoints[i].X * prmHeader.XScaleFactor) - prmCommonHeader.XOffset) / prmCommonHeader.XScaleFactor);
                prmPoints[i].Y = (Int32)(((prmHeader.YOffset + prmPoints[i].Y * prmHeader.YScaleFactor) - prmCommonHeader.YOffset) / prmCommonHeader.YScaleFactor);
                prmPoints[i].Z = (Int32)(((prmHeader.ZOffset + prmPoints[i].Z * prmHeader.ZScaleFactor) - prmCommonHeader.ZOffset) / prmCommonHeader.ZScaleFactor);
            }

            base.WritePoints<T>(prmPoints, prmNoOfPoints);
        }
    }
}
