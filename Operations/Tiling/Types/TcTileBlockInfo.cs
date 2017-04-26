using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Atlass.LAS.Lib.Operations.Tiling.Types
{
    public class TcTileBlockInfo
    {
        [XmlAttribute("Row")]
        public Int32 Row { get; set; }
        
        [XmlAttribute("Col")]
        public Int32 Col { get; set; }

        [XmlAttribute("North")]
        public Int32 North { get; set; }

        [XmlAttribute("East")]
        public Int32 East { get; set; }
        
        [XmlAttribute("Start")]
        public Int32 StartPoint { get; set; }
        
        [XmlAttribute("Count")]
        public Int32 NoOfPoints { get; set; }

        [XmlAttribute("Deleted")]
        public Boolean IsDeleted { get; set; }

        public TcTileBlockInfo()
            : this(0, 0, 0, 0, 0, 0)
        {
        }
        //-----------------------------------------------------------------------------

        public TcTileBlockInfo(Int32 prmRow, Int32 prmCol, Int32 prmEast, Int32 prmNorth, Int32 prmStartPoint, Int32 prmNoOfPoints)
        {
            Row = prmRow;
            Col = prmCol;
            East = prmEast;
            North = prmNorth;
            StartPoint = prmStartPoint;
            NoOfPoints = prmNoOfPoints;
            IsDeleted = false;
        }
        //-----------------------------------------------------------------------------

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

            TcTileBlockInfo rhs = obj as TcTileBlockInfo;
            return Row == rhs.Row && Col == rhs.Col && North == rhs.North && East == rhs.East
                && StartPoint == rhs.StartPoint && NoOfPoints == rhs.NoOfPoints;
        }
        //-----------------------------------------------------------------------------

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Col.GetHashCode();
                hash = hash * 23 + East.GetHashCode();
                hash = hash * 23 + North.GetHashCode();
                hash = hash * 23 + StartPoint.GetHashCode();
                hash = hash * 23 + NoOfPoints.GetHashCode();
                return hash;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
