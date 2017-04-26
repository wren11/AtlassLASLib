using System;
using System.Collections.Generic;
using System.Linq;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Utilities
{
    public class TcMathUtil
    {
        private const Single c_TorNullHeight = -100000;

        /// <summary>
        /// Returns row and column for a scaled las/tor file with given upper-left point
        /// </summary>
        /// <param name="prmEast">X of the given point</param>
        /// <param name="prmNorth">Y of the given point</param>
        /// <param name="prmUpperLeftEast">X of the start point of the area (upper left)</param>
        /// <param name="prmUpperLeftNorth">Y of the start point of the area (upper left)</param>
        /// <param name="prmScalingX">X scaling factor</param>
        /// <param name="prmScalingY">Y scaling factor</param>
        /// <returns>[row, col] of the point in the area</returns>
        static public Int32[] GetRowCol(Double prmEast, Double prmNorth, Double prmUpperLeftEast, Double prmUpperLeftNorth, Double prmScalingX, Double prmScalingY)
        {
            Double east = prmEast % prmScalingX == 0 ? prmEast : prmEast - (prmEast % prmScalingX);
            Double north = prmNorth % prmScalingY == 0 ? prmNorth : prmNorth + (prmScalingY - (prmNorth % prmScalingY));
            return new Int32[2] { Convert.ToInt32((prmUpperLeftNorth - north) / prmScalingY), Convert.ToInt32((east - prmUpperLeftEast) / prmScalingX) };
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Returns row and column for a scaled tor file with given tol object information
        /// </summary>
        /// <param name="prmEast">X of the given point</param>
        /// <param name="prmNorth">Y of the given point</param>
        /// <param name="prmInfo">Tol object information</param>
        /// <returns>[row, col] of the point in the area</returns>
//         static public Int32[] GetRowCol(Double prmEast, Double prmNorth, TcTolObject prmInfo)
//         {
//             return GetRowCol(prmEast, prmNorth, prmInfo.UpperLeftEast, prmInfo.UpperLeftNorth, prmInfo.ScalingX, prmInfo.ScalingY);
//         }
        //-----------------------------------------------------------------------------

        static public Int32[] MinMax(Int32[,] prmData)
        {
            Int32 min = Int32.MaxValue;
            Int32 max = Int32.MinValue;
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] < min)
                        min = prmData[i, j];
                    if (prmData[i, j] > max)
                        max = prmData[i, j];
                }
            return new Int32[2] { min, max };
        }
        //-----------------------------------------------------------------------------

        static public Single[] MinMax(Single[,] prmData)
        {
            Single min = Single.MaxValue;
            Single max = Single.MinValue;
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] < min)
                        min = prmData[i, j];
                    if (prmData[i, j] > max)
                        max = prmData[i, j];
                }
            return new Single[2] { min, max };
        }
        //-----------------------------------------------------------------------------

        static public Double[] MinMax(Double[,] prmData)
        {
            Double min = Double.MaxValue;
            Double max = Double.MinValue;
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] < min)
                        min = prmData[i, j];
                    if (prmData[i, j] > max)
                        max = prmData[i, j];
                }
            return new Double[2] { min, max };
        }
        //-----------------------------------------------------------------------------

        static public Single[] TorMinMax(Single[,] prmData)
        {
            Single min = Single.MaxValue;
            Single max = Single.MinValue;
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] != c_TorNullHeight)
                    {
                        if (prmData[i, j] < min)
                            min = prmData[i, j];
                        if (prmData[i, j] > max)
                            max = prmData[i, j];
                    }
                }
            return new Single[2] { min, max };
        }
        //-----------------------------------------------------------------------------

        static public Double Average(Int32[,] prmData)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += prmData[i, j];
            return sum / rows * cols;
        }
        //-----------------------------------------------------------------------------

        static public Double Average(Single[,] prmData)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += prmData[i, j];
            return sum / rows * cols;
        }
        //-----------------------------------------------------------------------------

        static public Double Average(Double[,] prmData)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += prmData[i, j];
            return sum / rows * cols;
        }
        //-----------------------------------------------------------------------------

        static public Single Average(IList<Single> prmData)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmData.Count; i++)
                sum += prmData[i];
            return (Single)(sum / prmData.Count);
        }
        //-----------------------------------------------------------------------------

        static public Double Average(IList<Double> prmData)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmData.Count; i++)
                sum += prmData[i];
            return sum / prmData.Count;
        }
        //-----------------------------------------------------------------------------

        static public Double TorAverage(Single[,] prmData)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double count = 0.0;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] != c_TorNullHeight)
                    {
                        sum += prmData[i, j];
                        count++;
                    }
                }
            return count > 0 ? sum / count : c_TorNullHeight;
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Int32[,] prmData)
        {
            Double avg = Average(prmData);

            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - avg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Single[,] prmData)
        {
            Double avg = Average(prmData);

            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - avg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Double[,] prmData)
        {
            Double avg = Average(prmData);

            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - avg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Int32[,] prmData, Double prmAvg)
        {
            Double avg = Average(prmData);

            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - prmAvg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));

        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Single[,] prmData, Double prmAvg)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - prmAvg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));

        }
        //-----------------------------------------------------------------------------

        static public Double StDev(Double[,] prmData, Double prmAvg)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    sum += Math.Pow(prmData[i, j] - prmAvg, 2);
            return Math.Sqrt(sum / (rows * cols - 1));

        }
        //-----------------------------------------------------------------------------

        static public Double TorStDev(Single[,] prmData)
        {
            Double avg = TorAverage(prmData);

            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            Double count = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] != c_TorNullHeight)
                    {
                        sum += Math.Pow(prmData[i, j] - avg, 2);
                        count++;
                    }
                }
            return count > 1 ? Math.Sqrt(sum / (count - 1)) : c_TorNullHeight;
        }
        //-----------------------------------------------------------------------------

        static public Double TorStDev(Single[,] prmData, Double prmAvg)
        {
            Int32 rows = prmData.GetLength(0);
            Int32 cols = prmData.GetLength(1);

            int i, j;
            Double sum = 0.0;
            Double count = 0.0;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                {
                    if (prmData[i, j] != c_TorNullHeight)
                    {
                        sum += Math.Pow(prmData[i, j] - prmAvg, 2);
                        count++;
                    }
                }
            return count > 1 ? Math.Sqrt(sum / (count - 1)) : c_TorNullHeight;
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(IEnumerable<Double> prmHeights, Double prmAvg)
        {
            return Math.Sqrt(prmHeights.Sum(iter => Math.Pow(iter - prmAvg, 2)) / (prmHeights.Count() - 1));
        }
        //-----------------------------------------------------------------------------

        static public Single StDev(IEnumerable<Single> prmHeights, Single prmAvg)
        {
            return (Single)Math.Sqrt(prmHeights.Sum(iter => Math.Pow(iter - prmAvg, 2)) / (prmHeights.Count() - 1));
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(IEnumerable<Double> prmHeights)
        {
            return StDev(prmHeights, prmHeights.Average());
        }
        //-----------------------------------------------------------------------------

        static public Single StDev(IEnumerable<Single> prmHeights)
        {
            return StDev(prmHeights, prmHeights.Average());
        }
        //-----------------------------------------------------------------------------

        // List
        static public Double StDev(IList<Double> prmHeights, Double prmAvg)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += Math.Pow(prmHeights[i] - prmAvg, 2);
            return Math.Sqrt(sum / (prmHeights.Count - 1));
        }
        //-----------------------------------------------------------------------------

        static public Single StDev(IList<Single> prmHeights, Single prmAvg)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += Math.Pow(prmHeights[i] - prmAvg, 2);
            return (Single)Math.Sqrt(sum / (prmHeights.Count - 1));
        }
        //-----------------------------------------------------------------------------

        static public Double StDev(IList<Double> prmHeights)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += prmHeights[i];
            Double avg = sum / prmHeights.Count;

            sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += Math.Pow(prmHeights[i] - avg, 2);
            return Math.Sqrt(sum / (prmHeights.Count - 1));
        }
        //-----------------------------------------------------------------------------

        static public Single StDev(IList<Single> prmHeights)
        {
            Double sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += prmHeights[i];
            Single avg = (Single)(sum / prmHeights.Count);

            sum = 0.0;
            for (int i = 0; i < prmHeights.Count; i++)
                sum += Math.Pow(prmHeights[i] - avg, 2);
            return (Single)Math.Sqrt(sum / (prmHeights.Count - 1));
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Calculates the distance from a point to a line.
        /// Source: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        /// </summary>
        /// <param name="ptX">X of the point</param>
        /// <param name="ptY">Y of the point</param>
        /// <param name="ln1X">X of the line segmet point 1</param>
        /// <param name="ln1Y">Y of the line segmet point 1</param>
        /// <param name="ln2X">X of the line segmet point 2</param>
        /// <param name="ln2Y">Y of the line segmet point 2</param>
        /// <returns>Distance to a line from a point</returns>
        static public Double DistToLine(Double ptX, Double ptY, Double ln1X, Double ln1Y, Double ln2X, Double ln2Y)
        {
            // Calculates the distance in square between two points
            Func<Double, Double, Double, Double, Double> GetDistance = delegate(Double pt1X, Double pt1Y, Double pt2X, Double pt2Y)
            {
                return Math.Pow(pt1X - pt2X, 2) + Math.Pow(pt1Y - pt2Y, 2);
            };

            Double lineLength = GetDistance(ln1X, ln1Y, ln2X, ln2Y);

            if (lineLength == 0)
            {
                return Math.Sqrt(GetDistance(ptX, ptY, ln1X, ln1Y));
            }

            Double projection = ((ptX - ln1X) * (ln2X - ln1X) + (ptY - ln1Y) * (ln2Y - ln1Y)) / lineLength;

            // When the point is apart from line segment point 1
            if (projection < 0)
            {
                return Math.Sqrt(GetDistance(ptX, ptY, ln1X, ln1Y));
            }

            // When the point is apart from line segment point 2
            if (projection > 1)
            {
                return Math.Sqrt(GetDistance(ptX, ptY, ln2X, ln2Y));
            }

            return Math.Sqrt(GetDistance(ptX, ptY, ln1X + projection * (ln2X - ln1X), ln1Y + projection * (ln2Y - ln1Y)));
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Calculates the angle between two points on a 2D plane.
        /// </summary>
        /// <param name="pt1X">X of point 1</param>
        /// <param name="pt1Y">Y of point 1</param>
        /// <param name="pt2X">X of point 2</param>
        /// <param name="pt2Y">Y of point 2</param>
        /// <returns>Angle between two points</returns>
        static public Double PointAngle(Double pt1X, Double pt1Y, Double pt2X, Double pt2Y)
        {
            return Math.Atan2(pt2Y - pt1Y, pt2X - pt1X) * 180 / Math.PI;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Calculates the angle between two points on a 2D plane.
        /// </summary>
        /// <param name="pt1">Point 1</param>
        /// <param name="pt2">Point 2</param>
        /// <returns>Angle between two points</returns>
        static public Double PointAngle(TcPoint pt1, TcPoint pt2)
        {
            return PointAngle(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Check whether two lines intersect or not.
        /// </summary>
        /// <param name="l1p1">TcPoint 1 of Line 1</param>
        /// <param name="l1p2">TcPoint 2 of Line 1</param>
        /// <param name="l2p1">TcPoint 1 of Line 2</param>
        /// <param name="l2p2">TcPoint 2 of Line 2</param>
        /// <returns>True if intersects and False otherwise</returns>
        static private Boolean LineIntersectsLine(TcPoint l1p1, TcPoint l1p2, TcPoint l2p1, TcPoint l2p2)
        {
            Double q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            Double d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            Double r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            Double s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Check whether a line intersect a rectangle or not.
        /// </summary>
        /// <param name="p1">TcPoint 1 of Line 1</param>
        /// <param name="p2">TcPoint 2 of Line 1</param>
        /// <param name="r">Rectangle</param>
        /// <returns>True if intersects and False otherwise</returns>
        static public Boolean LineIntersectRectangle(TcPoint p1, TcPoint p2, TcRectangle r)
        {
            return LineIntersectsLine(p1, p2, new TcPoint(r.UpperLeftX, r.LowerRightY), new TcPoint(r.LowerRightX, r.LowerRightY)) ||
                   LineIntersectsLine(p1, p2, new TcPoint(r.LowerRightX, r.LowerRightY), new TcPoint(r.LowerRightX, r.UpperLeftY)) ||
                   LineIntersectsLine(p1, p2, new TcPoint(r.LowerRightX, r.UpperLeftY), new TcPoint(r.UpperLeftX, r.UpperLeftY)) ||
                   LineIntersectsLine(p1, p2, new TcPoint(r.UpperLeftX, r.UpperLeftY), new TcPoint(r.UpperLeftX, r.LowerRightY)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Compare whether two rectangles are equal or not.
        /// </summary>
        /// <param name="prmRect1">Rectangle 1</param>
        /// <param name="prmRect2">Rectangle 2</param>
        /// <returns>True if equal, and False otherwise</returns>
        static public Boolean RectangleEquals(TcRectangle prmRect1, TcRectangle prmRect2)
        {
            return prmRect1.UpperLeftX == prmRect2.UpperLeftX && prmRect1.UpperLeftY == prmRect2.UpperLeftY
                && prmRect1.LowerRightX == prmRect2.LowerRightX && prmRect1.LowerRightY == prmRect2.LowerRightY;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------