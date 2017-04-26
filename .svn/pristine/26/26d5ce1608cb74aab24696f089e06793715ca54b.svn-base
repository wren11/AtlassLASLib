using System;
using System.IO;
using Atlass.LAS.Lib.Support.Types;

namespace Atlass.LAS.Lib.Support.IO
{
    public class TcTorWriter : IDisposable
    {
        private TcTolObject m_TolObject;
        
        public TcTorWriter(TcTolObject prmTolObject)
        {
            m_TolObject = prmTolObject;
        }

        public TcTorWriter(String prmTorFile)
        {
            using (TcTorReader reader = new TcTorReader(prmTorFile))
            {
                m_TolObject = reader.Info;
            }
        }
        //------------------------------------------------------------------

        public void WriteTol(String prmOutputFile)
        {
            prmOutputFile = String.Format(@"{0}\{1}.tol", Path.GetDirectoryName(prmOutputFile), Path.GetFileNameWithoutExtension(prmOutputFile));
            String data = String.Empty;
            data += "IMAGENAME   :  " + String.Format(@"{0}\{1}", Path.GetDirectoryName(m_TolObject.TorFile), Path.GetFileNameWithoutExtension(m_TolObject.TorFile)) + (char)10;
            data += "INFO        :  IMAGEFORMAT 1 : Trimble Rasterformat BIL" + (char)10;
            data += "DATE        :  " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + (char)10;
            data += "LINES       :  " + m_TolObject.Rows + (char)10;
            data += "COLUMNS     :  " + m_TolObject.Columns + (char)10;
            data += "BANDS       :  1" + (char)10;
            data += "BAND-NUMBERS:  1" + (char)10;
            data += "BIT/PIX /SW :  32  1" + (char)10;
            data += "UNIT        :  METRIC" + (char)10;
            data += "------------------------------" + (char)10;
            data += "HOR_SPC E/N :  " + m_TolObject.ScalingX.ToString("f4") + "   " + m_TolObject.ScalingY.ToString("f4") + (char)10;
            data += "VER_SPC Z   :  1.00000" + (char)10;
            data += "HEIGHT_OFF  :  .00000" + (char)10;
            data += "------------------------------ " + (char)10;
            data += "GEOREF      :  GDA94 / GRS 80 / (UNIV.) TR. MERCATOR" + (char)10;
            data += "UP_LEFT_E   :  " + m_TolObject.UpperLeftEast.ToString("f4") + (char)10;
            data += "UP_LEFT_N   :  " + m_TolObject.UpperLeftNorth.ToString("f4") + (char)10;
            data += "LO_RIGHT_E  :  " + m_TolObject.LowerRightEast.ToString("f4") + (char)10;
            data += "LO_RIGHT_N  :  " + m_TolObject.LowerRightNorth.ToString("f4") + (char)10;
            data += "MIN_Z/G     :  " + m_TolObject.MinHeight.ToString("f4") + (char)10;
            data += "MAX_Z/G     :  " + m_TolObject.MaxHeight.ToString("f4") + (char)10;
            data += "MODEL       :  " + m_TolObject.Model + (char)10;
            data += "ELLIPSOID   :  32" + (char)10;
            data += "PROJECTION  :  1" + (char)10;
            data += "FALSE_E     :  500000.00000" + (char)10;
            data += "FALSE_N     :  10000000.00000" + (char)10;
            data += "ORI_LON /P1 :  153.00000    .00000" + (char)10;
            data += "ORI_LAT /P2 :  .00000    .00000" + (char)10;
            data += "SC_FAC      :  .9996000" + (char)10;
            data += "------------------------------" + (char)10;
            data += "BEGIN-HISTORY" + (char)10;
            if (m_TolObject.Adjusted)
            {
                data += "ADJUSTED    :  " + (m_TolObject.Adjusted ? "1" : "0") + (char)10;
            }
            data += " *******************" + (char)10;
            data += "END-HISTORY" + (char)10;

            using (StreamWriter writer = new StreamWriter(prmOutputFile))
            {
                try
                {
                    writer.Write(data);
                }
                finally
                {
                    writer.Close();
                }
            }
        }
        //------------------------------------------------------------------

        public void WriteTol()
        {
            WriteTol(m_TolObject.TolFile);
        }
        //------------------------------------------------------------------

        protected void Write(BinaryWriter wrt, Single[,] prmPoints)
        {
            Int32 rows = prmPoints.GetLength(0);
            Int32 columns = prmPoints.GetLength(1);

            Int32 length = rows * columns;
            Byte[] byteStream;
            
            for (int i = 0; i < rows; i++)
            {
                byteStream = new Byte[columns * sizeof(Single)];
                for (int j = 0; j < columns; j++)
                {
                    Array.Copy(BitConverter.GetBytes(prmPoints[i, j]), 0, byteStream, j * sizeof(Single), sizeof(Single));
                }
                wrt.Write(byteStream);
            }
        }
        //------------------------------------------------------------------

        protected void Write(BinaryWriter prmWriter, TcTorBlock32 prmBlock)
        {
            Write(prmWriter, prmBlock.Points);
        }
        //------------------------------------------------------------------

        public void Write(TcTorObject prmTor, String prmOutputFile)
        {
            m_TolObject = prmTor.Info;
            using (BinaryWriter writer = new BinaryWriter(new FileStream(prmOutputFile, FileMode.Create)))
            {
                WriteTol(prmOutputFile);
                foreach (TcTorBlock32 block in prmTor.Blocks)
                {
                    Write(writer, block);
                }
            }
        }
        //------------------------------------------------------------------

        public void Write(TcTorObject prmTor)
        {
            Write(prmTor, prmTor.Info.TorFile);
        }
        //------------------------------------------------------------------

//         static public void Write(TcTorObject prmTor)
//         {
//             Write(prmTor.File, prmTor.Info, prmTor.Points);
//         }
        //------------------------------------------------------------------


        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------