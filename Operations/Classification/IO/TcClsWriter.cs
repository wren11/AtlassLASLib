using System;
using System.IO;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Operations.Classification.IO
{
    public class TcClsWriter : IDisposable
    {
        private TiLasHeader m_LasHeader;
        private BinaryWriter m_ClsWriter;

        public TcClsWriter(TiLasHeader prmLasHeader, String prmOutputClsFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(prmOutputClsFile)))
            {
                throw new FileNotFoundException("Output directory not found");
            }

            m_LasHeader = prmLasHeader;
            m_ClsWriter = new BinaryWriter(new FileStream(prmOutputClsFile, FileMode.Create));
        }
        //-----------------------------------------------------------------------------

        public void WriteHeader(Byte[] prmOffsetBytes)
        {
            if (!m_ClsWriter.BaseStream.CanWrite)
            {
                throw new IOException("Cannot write the header into the LAS file");
            }

            // Allocate global memory of a size of header + extra = offset.
            IntPtr ptr = Marshal.AllocHGlobal((Int32)m_LasHeader.HeaderSize);

            try
            {
                m_ClsWriter.BaseStream.Position = 0;

                Byte[] arr = new Byte[m_LasHeader.HeaderSize];

                // Convert the header to the pointer.
                Marshal.StructureToPtr(m_LasHeader, ptr, true);

                // Copy the data.
                Marshal.Copy(ptr, arr, 0, (Int32)m_LasHeader.HeaderSize);

                m_ClsWriter.Write(arr);
                m_ClsWriter.Write(prmOffsetBytes);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        //-----------------------------------------------------------------------------

        public void WritePoints<T>(T[] prmPoints, Int64 prmNoOfPoints = -1) where T : TiLasPoint
        {
            Int32 count = prmNoOfPoints < 0 ? prmPoints.Length : (Int32)prmNoOfPoints;
            Int32 pointSize = Marshal.SizeOf(typeof(TsClsLasPoint));
            Int32 totalSize = pointSize * count;

            // Allocates memory and totalPtr points to the 1st location.
            IntPtr totalPtr = Marshal.AllocHGlobal(totalSize);
            IntPtr structPtr;

            // Classification point.
            TsClsLasPoint clsPoint;

            // This is used to increment the location of the pointer later. It cant be done with IntPtr.
            // ptrLoc points to the 1st location of totalPtr.
            Int64 ptrLoc = totalPtr.ToInt64();

            for (int i = 0; i < count; i++)
            {
                // Creating the classification point from the las point.
                clsPoint = new TsClsLasPoint() { X = prmPoints[i].X, Y = prmPoints[i].Y, Z = prmPoints[i].Z, Classification = prmPoints[i].Classification };

                // structPtr points to ptrLoc.
                structPtr = new IntPtr(ptrLoc);

                // Converts the structure into memory pointed by structPtr
                Marshal.StructureToPtr(clsPoint, structPtr, false);

                // Move the pointer.
                ptrLoc += pointSize;
            }

            byte[] buffer = new byte[totalSize];

            // Copies the data from totalPtr memory to arr array.
            Marshal.Copy(totalPtr, buffer, 0, totalSize);

            // Free the memory from the heap.
            Marshal.FreeHGlobal(totalPtr);

            // Write the array.
            m_ClsWriter.Write(buffer);
        }
        //------------------------------------------------------------------

        public void Dispose()
        {
            if (m_ClsWriter != null)
            {
                m_ClsWriter.Close();
                m_ClsWriter.Dispose();
                m_ClsWriter = null;
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
