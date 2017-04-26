///<summary> TcLasReader
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class reads different las file formats and return them as a collection
/// of las points.

/// <author>
/// Name: S M Kamrul Hasan
/// Date: 16-JUL-2014
/// </author>
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
///</summary>

using System;
using System.IO;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Operations.IO
{
    public class TcClsReader : IDisposable
    {
        private readonly Int32 c_PointSize;

        private String m_Input;
        private BinaryReader m_Reader;

        private TiLasHeader m_Header;
        public TiLasHeader Header { get { return m_Header; } }

        private Int64 m_TotalPoints;
        public Int64 TotalPoints { get { return m_TotalPoints; } }

        private Byte[] m_OffsetBytes;
        public Byte[] OffsetBytes { get { return m_OffsetBytes; } }

        public TcClsReader(String prmInput)
        {
            if (!File.Exists(prmInput))
            {
                throw new FileNotFoundException(String.Format("Cls file not found : {0}", prmInput));
            }

            m_Input = prmInput;
            c_PointSize = Marshal.SizeOf(typeof(TsClsLasPoint));
            m_Reader = new BinaryReader(new FileStream(m_Input, FileMode.Open));
            m_Header = GetHeader();
            m_OffsetBytes = GetOffsetBytes();
            m_TotalPoints = (Int64)((m_Reader.BaseStream.Length - m_Header.PointOffset) / c_PointSize);
        }
        //-----------------------------------------------------------------------------

        private T ReadHeader<T>() where T : TiLasHeader
        {
            Int32 size = Marshal.SizeOf(typeof(T));
            GCHandle handle = default(GCHandle);
            T retHeader = default(T);

            try
            {
                // Read the required bytes from the file.
                m_Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Byte[] readBuffer = m_Reader.ReadBytes(size);

                // Set a pointer to the allocated memory.
                handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);

                // Convert the buffer from the pointer to a managed object.
                retHeader = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

                return retHeader;
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        //-----------------------------------------------------------------------------

        private TiLasHeader GetHeader()
        {
            // Read the required bytes from the file.
            m_Reader.BaseStream.Seek(25, SeekOrigin.Begin);
            Byte versionMinor = m_Reader.ReadByte();

            switch (versionMinor)
            {
                case 1:
                case 2:
                    return ReadHeader<TsLasHeader12>();

                case 3:
                    return ReadHeader<TsLasHeader13>();

                case 4:
                    return ReadHeader<TsLasHeader14>();

                default:
                    throw new InvalidDataException(String.Format("Could not read LAS header. Version 1.{0} not supported.", versionMinor));
            }
        }
        //-----------------------------------------------------------------------------

        private Byte[] GetOffsetBytes()
        {
            Int32 pointSize = m_Header.HeaderSize;
            m_Reader.BaseStream.Seek(pointSize, SeekOrigin.Begin);
            return m_Reader.ReadBytes((int)m_Header.PointOffset - pointSize);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// This function reads a given number of points from the current location of the reader.
        /// If enough points not found in the file, it returns the available points.
        /// </summary>
        /// <typeparam name="T">Type of the las point</typeparam>
        /// <param name="prmNoOfPoints">Number of points to read</param>
        /// <returns>An array of LAS points</returns>
        public TsClsLasPoint[] ReadPoints(Int64 prmNoOfPoints)
        {
            // Points loaded from file.
            Int64 noOfPointsLoaded = 0;

            // Points need to be loaded.
            Int64 noOfPointsToRead = 0;

            // No of bytes to read from the file.
            Int32 bytesToRead = 0;

            // Memory stream.
            Byte[] readBuffer;

            Int64 noOfPointsAvailable = (Int64)((m_Reader.BaseStream.Length - m_Reader.BaseStream.Position) / c_PointSize);
            prmNoOfPoints = Math.Min(prmNoOfPoints, noOfPointsAvailable);

            // Create a las point array of required size.
            TsClsLasPoint[] returnBlocks = new TsClsLasPoint[prmNoOfPoints];

            // Set a handle to the las point array.
            GCHandle pinnedHandle = GCHandle.Alloc(returnBlocks, GCHandleType.Pinned);

            // Pointer to the current pointed object.
            IntPtr blockPtr = pinnedHandle.AddrOfPinnedObject();

            // Value of the pointer.
            Int64 ptrLoc = blockPtr.ToInt64();

            try
            {
                while (noOfPointsLoaded < prmNoOfPoints)
                {
                    noOfPointsToRead = Math.Min(TcConstants.MaxLasPointsToProcessAtOnce, prmNoOfPoints - noOfPointsLoaded);

                    // Read required bytes from the file.
                    bytesToRead = c_PointSize * (Int32)noOfPointsToRead;
                    readBuffer = m_Reader.ReadBytes(bytesToRead);

                    // Copy the stream to the structures.
                    Marshal.Copy(readBuffer, 0, blockPtr, bytesToRead);

                    // Update the processed item counter.
                    noOfPointsLoaded += noOfPointsToRead;

                    // Update the pointer value.
                    ptrLoc += bytesToRead;

                    // Move the pointer.
                    blockPtr = new IntPtr(ptrLoc);
                }

                return returnBlocks;
            }
            finally
            {
                if (pinnedHandle.IsAllocated)
                {
                    pinnedHandle.Free();
                }

                readBuffer = new Byte[0];
            }
        }

        //------------------------------------------------------------------

        public void SeekToPoint(Int64 prmPoint)
        {
            m_Reader.BaseStream.Seek(m_Header.PointOffset + prmPoint * c_PointSize, SeekOrigin.Begin);
        }
        //------------------------------------------------------------------

        public void Dispose()
        {
            if (m_Reader != null)
            {
                m_Reader.Close();
                m_Reader.Dispose();
                m_Reader = null;
            }
        }
        //------------------------------------------------------------------

    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------