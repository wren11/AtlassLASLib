using System;
using System.IO;

namespace Atlass.LAS.Lib.Support.Types
{
    public class TcTolObject
    {
        /// <summary>
        /// Name of the Tor.
        /// </summary>
        public String ImageName { get; set; }

        /// <summary>
        /// Date and Time when created.
        /// </summary>
        public String DateTime { get; set; }

        /// <summary>
        /// Number of rows (Y direction).
        /// </summary>
        public Int32 Rows { get; set; }

        /// <summary>
        /// Number of columns (X direction).
        /// </summary>
        public Int32 Columns { get; set; }

        /// <summary>
        /// Bit depth of the file (either 8 or 32).
        /// </summary>
        public Int32 BitSize { get; set; }

        /// <summary>
        /// Scaling in Y direction.
        /// </summary>
        public Double ScalingY { get; set; }

        /// <summary>
        /// Scaling in X direction.
        /// </summary>
        public Double ScalingX { get; set; }

        /// <summary>
        /// X (East) of the upper-left corner.
        /// </summary>
        public Double UpperLeftEast { get; set; }

        /// <summary>
        /// Y (North) of the upper-left corner (upside down).
        /// </summary>
        public Double UpperLeftNorth { get; set; }

        /// <summary>
        /// X (East) of the lower-right corner.
        /// </summary>
        public Double LowerRightEast { get; set; }

        /// <summary>
        /// Y (North) of the lower-right corner (upside down).
        /// </summary>
        public Double LowerRightNorth { get; set; }

        /// <summary>
        /// Minimum height (Z).
        /// </summary>
        public Single MinHeight { get; set; }

        /// <summary>
        /// Maximum height (Z).
        /// </summary>
        public Single MaxHeight { get; set; }

        /// <summary>
        /// Model of the device used.
        /// </summary>
        public Int32 Model { get; set; }

        /// <summary>
        /// Flag to mention whether the file is adjusted or not.
        /// </summary>
        public Boolean Adjusted { get; set; }
        
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Extra : Full path of the tor file.
        /// </summary>
        private String m_TorFile;
        public String TorFile
        { 
            get { return m_TorFile; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    TolFile = String.Format(@"{0}\{1}.tol", Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value));
                    m_TorFile = value;
                }
            }
        }

        /// <summary>
        /// Extra : Full path of the tol file.
        /// </summary>
        public String TolFile { get; private set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public TcTolObject(String prmTorFile)
        {
            TorFile = prmTorFile;
            ImageName = String.Empty;
            DateTime = String.Empty;
            Rows = 0;
            Columns = 0;
            ScalingX = Double.MinValue;
            ScalingY = Double.MinValue;
            UpperLeftEast = Double.MinValue;
            UpperLeftNorth = Double.MinValue;
            LowerRightEast = Double.MinValue;
            LowerRightNorth = Double.MinValue;
            MinHeight = Single.MaxValue;
            MaxHeight = Single.MinValue;
            Model = Int32.MinValue;
            Adjusted = false;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="prmRight">Object to be copied from</param>
        public TcTolObject(TcTolObject prmRight)
        {
            TorFile = prmRight.TorFile;
            TolFile = prmRight.TolFile;
            ImageName = prmRight.ImageName;
            DateTime = prmRight.DateTime;
            Rows = prmRight.Rows;
            Columns = prmRight.Columns;
            ScalingX = prmRight.ScalingX;
            ScalingY = prmRight.ScalingY;
            UpperLeftEast = prmRight.UpperLeftEast;
            UpperLeftNorth = prmRight.UpperLeftNorth;
            LowerRightEast = prmRight.LowerRightEast;
            LowerRightNorth = prmRight.LowerRightNorth;
            MinHeight = prmRight.MinHeight;
            MaxHeight = prmRight.MaxHeight;
            Model = prmRight.Model;
            Adjusted = prmRight.Adjusted;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------