using System;

namespace Atlass.LAS.Lib.Types.DataType
{
    [Serializable]
    public class TcRectangle
    {
        public Double UpperLeftX { get; protected set;  }
        public Double UpperLeftY { get; protected set; }
        public Double LowerRightX { get; protected set; }
        public Double LowerRightY { get; protected set; }
        public Boolean IsEmpty { get { return LowerRightX - UpperLeftX == 0 && LowerRightY - UpperLeftY == 0; } }

        #region Constructors / Destructor

        public TcRectangle(Double prmUpperLeftX, Double prmUpperLeftY, Double prmLowerRightX, Double prmLowerRightY)
        {
            UpperLeftX = prmUpperLeftX;
            UpperLeftY = prmUpperLeftY;
            LowerRightX = prmLowerRightX;
            LowerRightY = prmLowerRightY;
        }

        protected TcRectangle()
            : this(0, 0, 0, 0)
        {
        }

        public TcRectangle(TcPoint prmUpperLeft, TcPoint prmLowerRight)
            : this(prmUpperLeft.X, prmUpperLeft.Y, prmLowerRight.X, prmLowerRight.Y)
        {
        }

        public TcRectangle(TcRectangle prmRectangle)
            : this(prmRectangle.UpperLeftX, prmRectangle.UpperLeftY, prmRectangle.LowerRightX, prmRectangle.LowerRightY)
        {
        }

        #endregion Constructors / Destructor

        public void AddOffset(Double prmOffset)
        {
            UpperLeftX -= prmOffset;
            UpperLeftY += prmOffset;
            LowerRightX += prmOffset;
            LowerRightY -= prmOffset;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            TcRectangle rhs = obj as TcRectangle;
            return UpperLeftX == rhs.UpperLeftX && UpperLeftY == rhs.UpperLeftY && LowerRightX == rhs.LowerRightX && LowerRightY == rhs.LowerRightY;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + UpperLeftX.GetHashCode();
                hash = hash * 23 + UpperLeftY.GetHashCode();
                hash = hash * 23 + LowerRightX.GetHashCode();
                hash = hash * 23 + LowerRightY.GetHashCode();
                return hash;
            }
        }

        #region 'HasOverlap' functions to determine whether there is an overlap between two rectangles.

        /// <summary>
        /// Function to determine whether there is an overlap between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// http://stackoverflow.com/questions/13390333/two-rectangles-intersection
        /// </summary>
        /// <param name="prmULX1">Upper left X (East) of the first rectangle</param>
        /// <param name="prmULY1">Upper left Y (North) of the first rectangle</param>
        /// <param name="prmLRX1">Lower right X (East) of the first rectangle</param>
        /// <param name="prmLRY1">Lower right Y (North) of the first rectangle</param>
        /// <param name="prmULX2">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY2">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX2">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY2">Lower right Y (North) of the second rectangle</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public static Boolean HasOverlap(Double prmULX1, Double prmULY1, Double prmLRX1, Double prmLRY1,
                                         Double prmULX2, Double prmULY2, Double prmLRX2, Double prmLRY2)
        {
            return (prmULY1 > prmLRY1 || prmULY2 > prmLRY2)
                ? !(prmLRX1 <= prmULX2 || prmLRX2 <= prmULX1 || prmULY1 <= prmLRY2 || prmULY2 <= prmLRY1)
                : !(prmLRX1 <= prmULX2 || prmLRX2 <= prmULX1 || prmLRY1 <= prmULY2 || prmLRY2 <= prmULY1);
        }

        /// <summary>
        /// Function to determine whether there is an overlap between two rectangles.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect1">Rectangle 1</param>
        /// <param name="prmRect2">Rectangle 2</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public static Boolean HasOverlap(TcRectangle prmRect1, TcRectangle prmRect2)
        {
            return TcRectangle.HasOverlap(prmRect1.UpperLeftX, prmRect1.UpperLeftY, prmRect1.LowerRightX, prmRect1.LowerRightY,
                prmRect2.UpperLeftX, prmRect2.UpperLeftY, prmRect2.LowerRightX, prmRect2.LowerRightY);
        }

        /// <summary>
        /// Function to determine whether there is an overlap between two rectangles.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect1">Rectangle 1</param>
        /// <param name="prmULX">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY">Lower right Y (North) of the second rectangle</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public static Boolean HasOverlap(TcRectangle prmRect, Double prmULX, Double prmULY, Double prmLRX, Double prmLRY)
        {
            return TcRectangle.HasOverlap(prmRect.UpperLeftX, prmRect.UpperLeftY, prmRect.LowerRightX, prmRect.LowerRightY, prmULX, prmULY, prmLRX, prmLRY);
        }

        /// <summary>
        /// Function to determine whether there is an overlap between this and another rectangles.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect">Rectangle 2</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public Boolean HasOverlap(TcRectangle prmRect)
        {
            return TcRectangle.HasOverlap(this, prmRect);
        }

        /// <summary>
        /// Function to determine whether there is an overlap between this and another rectangles.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmULX">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY">Lower right Y (North) of the second rectangle</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public Boolean HasOverlap(Double prmULX, Double prmULY, Double prmLRX, Double prmLRY)
        {
            return TcRectangle.HasOverlap(this, prmULX, prmULY, prmLRX, prmLRY);
        }

        #endregion

        #region 'OverlapArea' functions to calculate the overlapping area between two rectangles.

        /// <summary>
        /// Function to calculate the overlapping area between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmULX1">Upper left X (East) of the first rectangle</param>
        /// <param name="prmULY1">Upper left Y (North) of the first rectangle</param>
        /// <param name="prmLRX1">Lower right X (East) of the first rectangle</param>
        /// <param name="prmLRY1">Lower right Y (North) of the first rectangle</param>
        /// <param name="prmULX2">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY2">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX2">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY2">Lower right Y (North) of the second rectangle</param>
        /// <returns>The area of overlap. Empty if there is no overlap</returns>
        public static TcRectangle OverlapArea(Double prmULX1, Double prmULY1, Double prmLRX1, Double prmLRY1,
            Double prmULX2, Double prmULY2, Double prmLRX2, Double prmLRY2)
        {
            if (!HasOverlap(prmULX1, prmULY1, prmLRX1, prmLRY1, prmULX2, prmULY2, prmLRX2, prmLRY2))
            {
                return new TcRectangle(new TcRectangle(0, 0, 0, 0));
            }

            return (prmULY1 > prmLRY1 || prmULY2 > prmLRY2)
                ? new TcRectangle(Math.Max(prmULX1, prmULX2), Math.Min(prmULY1, prmULY2), Math.Min(prmLRX1, prmLRX2), Math.Max(prmLRY1, prmLRY2))
                : new TcRectangle(Math.Max(prmULX1, prmULX2), Math.Max(prmULY1, prmULY2), Math.Min(prmLRX1, prmLRX2), Math.Min(prmLRY1, prmLRY2));
        }

        /// <summary>
        /// Function to calculate the overlapping area between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect1">Rectangle 1</param>
        /// <param name="prmRect2">Rectangle 2</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public static TcRectangle OverlapArea(TcRectangle prmRect1, TcRectangle prmRect2)
        {
            return TcRectangle.OverlapArea(prmRect1.UpperLeftX, prmRect1.UpperLeftY, prmRect1.LowerRightX, prmRect1.LowerRightY,
                prmRect2.UpperLeftX, prmRect2.UpperLeftY, prmRect2.LowerRightX, prmRect2.LowerRightY);
        }

        /// <summary>
        /// Function to calculate the overlapping area between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect1">Rectangle 1</param>
        /// <param name="prmULX">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY">Lower right Y (North) of the second rectangle</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public static TcRectangle OverlapArea(TcRectangle prmRect, Double prmULX, Double prmULY, Double prmLRX, Double prmLRY)
        {
            return TcRectangle.OverlapArea(prmRect.UpperLeftX, prmRect.UpperLeftY, prmRect.LowerRightX, prmRect.LowerRightY, prmULX, prmULY, prmLRX, prmLRY);
        }

        /// <summary>
        /// Function to calculate the overlapping area between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmRect">Rectangle 2</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public TcRectangle OverlapArea(TcRectangle prmRect)
        {
            return TcRectangle.OverlapArea(this, prmRect);
        }

        /// <summary>
        /// Function to calculate the overlapping area between two rectangles represented by points.
        /// It takes of the graphical rectangle where the Y (North) coordinates are inverted.
        /// </summary>
        /// <param name="prmULX">Upper left X (East) of the second rectangle</param>
        /// <param name="prmULY">Upper left Y (North) of the second rectangle</param>
        /// <param name="prmLRX">Lower right X (East) of the second rectangle</param>
        /// <param name="prmLRY">Lower right Y (North) of the second rectangle</param>
        /// <returns>True if the rectangles intersect, False otherwise</returns>
        public TcRectangle OverlapArea(Double prmULX, Double prmULY, Double prmLRX, Double prmLRY)
        {
            return TcRectangle.OverlapArea(this, prmULX, prmULY, prmLRX, prmLRY);
        }

        #endregion

        public Boolean Contains(Double prmX, Double prmY)
        {
            return prmX >= UpperLeftX && prmX <= LowerRightX && prmY <= UpperLeftY && prmY >= LowerRightY;
        }

        public Boolean Contains(TcPoint prmPoint)
        {
            return Contains(prmPoint.X, prmPoint.Y);
        }
    }
}
