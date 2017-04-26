using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atlass.LAS.Lib.Support.Types;
using Atlass.LAS.Lib.Operations.Types;
using Atlass.LAS.Lib.Support.Types.Interfaces;
using Atlass.LAS.Lib.Global;
using System.Runtime.InteropServices;

namespace Atlass.LAS.Lib.Support.IO
{
    public class TcTorReader : IDisposable
    {
        private String m_TorFile;
        private BinaryReader m_Reader;
        public TcTolObject Info { get; private set; }
        
        public TcTorReader(String prmTorFile)
        {
            m_TorFile = prmTorFile;
            Info = ReadTol(prmTorFile);
            m_Reader = new BinaryReader(new FileStream(prmTorFile, FileMode.Open));
        }
        //------------------------------------------------------------------

        static public TcTolObject ReadTol(String prmTorFile)
        {
            if (!prmTorFile.EndsWith(".tor", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new FileNotFoundException(String.Format("Invalid TOR file : {0}", prmTorFile));
            }

            if (!File.Exists(prmTorFile))
            {
                throw new FileNotFoundException(String.Format("Cannot find the TOL file : {0}", prmTorFile));
            }

            TcTolObject returnObject = new TcTolObject(prmTorFile);
            String[] lines = File.ReadAllLines(returnObject.TolFile);
            String[] splittedItems;

            foreach (String line in lines)
            {
                IEnumerable<String> keys = line.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(iter => iter.Trim());
                if (keys.Count() < 1)
                {
                    continue;
                }

                TeTolTags info = TcEnums.GetEnumFromDescription<TeTolTags>(keys.First());
                switch (TcEnums.GetEnumFromDescription<TeTolTags>(keys.First()))
                {
                    case TeTolTags.ImageName:
                        returnObject.ImageName = keys.ElementAt(1);
                        break;

                    case TeTolTags.DateTime:
                        returnObject.DateTime = keys.ElementAt(1);
                        break;

                    case TeTolTags.Rows:
                        returnObject.Rows = Int32.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.Columns:
                        returnObject.Columns = Int32.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.BitSize:
                        splittedItems = keys.ElementAt(1).Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (splittedItems.Length >= 1)
                        {
                            returnObject.BitSize = Int32.Parse(splittedItems[0]);
                        }
                        break;

                    case TeTolTags.Scaling:
                        splittedItems = keys.ElementAt(1).Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (splittedItems.Length >= 2)
                        {
                            returnObject.ScalingX = Double.Parse(splittedItems[0]);
                            returnObject.ScalingY = Double.Parse(splittedItems[1]);
                        }
                        break;

                    case TeTolTags.UpperLeftEast:
                        returnObject.UpperLeftEast = Double.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.UpperLeftNorth:
                        returnObject.UpperLeftNorth = Double.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.LowerRightEast:
                        returnObject.LowerRightEast = Double.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.LowerRightNorth:
                        returnObject.LowerRightNorth = Double.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.MinHeight:
                        returnObject.MinHeight = Single.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.MaxHeight:
                        returnObject.MaxHeight = Single.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.Model:
                        returnObject.Model = Int32.Parse(keys.ElementAt(1));
                        break;

                    case TeTolTags.Adjusted:
                        returnObject.Adjusted = Int32.Parse(keys.ElementAt(1)) == 1;
                        break;

                    case TeTolTags.Unknown:
                        break;
                }
            }
            
            return returnObject;
        }
        //------------------------------------------------------------------

        private List<TcTorBlock32> GetTorBlocks(TcTolObject prmTol)
        {
            Int32 pointSize = sizeof(Single);
            Int32 rowsInBlock = (Int32)Math.Floor(TcConstants.MaxBytesToLoadInTorBlock * 1.0 / (prmTol.Columns * pointSize));
            Int64 noOfPointsAvailable = (Int64)((m_Reader.BaseStream.Length - m_Reader.BaseStream.Position) / pointSize);

            // Create a las point array of required size.
            List<TcTorBlock32> returnBlocks = new List<TcTorBlock32>();

            // Memory stream.
            Byte[] readBuffer;

            Int32 index = 0;
            Int32 rowsProcessed = 0;

            while (rowsProcessed < prmTol.Rows)
            {
                Int32 rowsToProcess = Math.Min(rowsInBlock, prmTol.Rows - rowsProcessed);
                Int32 bytesToRead = pointSize * rowsToProcess * prmTol.Columns;

                // Reads the points in a byte array.
                readBuffer = m_Reader.ReadBytes(bytesToRead);

                TcTorBlock32 block = new TcTorBlock32(index++, rowsToProcess, prmTol.Columns) { Points = new Single[rowsToProcess, prmTol.Columns] };

                // Set a handle to the las point array.
                GCHandle pinnedHandle = GCHandle.Alloc(block.Points, GCHandleType.Pinned);

                // Copy the stream to the structures.
                Marshal.Copy(readBuffer, 0, pinnedHandle.AddrOfPinnedObject(), bytesToRead);

                returnBlocks.Add(block);
                rowsProcessed += rowsToProcess;
            }

            return returnBlocks;
        }
        //------------------------------------------------------------------

        public TcTorObject GetTor()
        {
            TcTorObject returnObject = new TcTorObject(Info);
            returnObject.Blocks.AddRange(GetTorBlocks(Info));
            return returnObject;
        }
        //------------------------------------------------------------------

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Reader != null)
            {
                m_Reader.Close();
                m_Reader.Dispose();
                m_Reader = null;
            }
        }

        #endregion
    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------
