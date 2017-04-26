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
using System.Linq;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Types.Class;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Operations.IO
{
    public class TcLasReader : IDisposable
    {
        protected String m_Input;
        protected BinaryReader m_Reader;

        protected TiLasHeader m_Header;
        public TiLasHeader Header { get { return m_Header; } }

        protected Int64 m_TotalPoints;
        public Int64 TotalPoints { get { return m_TotalPoints; } }

        protected Byte[] m_OffsetBytes;
        public Byte[] OffsetBytes { get { return m_OffsetBytes; } }

        public TcLasReader(String prmInput)
        {
            if (!File.Exists(prmInput))
            {
                throw new FileNotFoundException(String.Format("Las file not found : {0}", prmInput));
            }

            if (!prmInput.EndsWith(".las", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new FileNotFoundException(String.Format("Invalid LAS file : {0}", prmInput));
            }

            m_Input = prmInput;
            m_Reader = new BinaryReader(new FileStream(m_Input, FileMode.Open));
            m_Header = GetHeader();
            m_OffsetBytes = GetOffsetBytes();
            m_TotalPoints = (Int64)((m_Reader.BaseStream.Length - m_Header.PointOffset) / m_Header.PointDataRecordLength);
        }
        //-----------------------------------------------------------------------------

        protected T ReadHeader<T>() where T : TiLasHeader
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

        protected TiLasHeader GetHeader()
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

        protected Byte[] GetOffsetBytes()
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
        public T[] ReadPoints<T>(Int64 prmNoOfPoints) where T : TiLasPoint
        {
            // Size of the point.
            Int32 pointSize = Marshal.SizeOf(typeof(T));

            // If the required point size mismatched against what is in the file.
            if (pointSize != m_Header.PointDataRecordLength)
            {
                throw new InvalidDataException("Cannot read LAS point. Point size mismatched.");
            }

            // Points loaded from file.
            Int64 noOfPointsLoaded = 0;

            // Points need to be loaded.
            Int64 noOfPointsToRead = 0;

            // No of bytes to read from the file.
            Int32 bytesToRead = 0;

            // Memory stream.
            Byte[] readBuffer;

            Int64 noOfPointsAvailable = (Int64)((m_Reader.BaseStream.Length - m_Reader.BaseStream.Position) / m_Header.PointDataRecordLength);
            prmNoOfPoints = Math.Min(prmNoOfPoints, noOfPointsAvailable);

            // Create a las point array of required size.
            T[] returnBlocks = new T[prmNoOfPoints];

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
                    bytesToRead = pointSize * (Int32)noOfPointsToRead;
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

        public T ReadNthPoint<T>(Int64 prmPointNumber) where T : TiLasPoint
        {
            // Size of the point.
            Int32 pointSize = Marshal.SizeOf(typeof(T));

            // If the required point size mismatched against what is in the file.
            if (pointSize != m_Header.PointDataRecordLength)
            {
                throw new InvalidDataException("Cannot read LAS point. Point size mismatched.");
            }
            
            //Set the position to the end of header.
            m_Reader.BaseStream.Position = m_Header.PointOffset;

            GCHandle handle = default(GCHandle);

            try
            {
                m_Reader.BaseStream.Seek(((prmPointNumber - 1) * pointSize), SeekOrigin.Begin);
                Byte[] readBuffer = m_Reader.ReadBytes(pointSize);
                
                // Set a pointer to the allocated memory.
                handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                
                // Convert the buffer from the pointer to a managed object.
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        public TcLasPointBase[] ReadPoints(Int64 prmNoOfPoints, TiLasHeader prmHeader)
        {
            Int64 pointCounter = 0;
            switch (prmHeader.PointDataFormatID)
            {
                case 0:
                    TsLasHeader12 header0 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint0>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint0(iter, header0);
                        return res;
                    });

                case 1:
                    TsLasHeader12 header1 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint1>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint1(iter, header1);
                        return res;
                    });

                case 2:
                    TsLasHeader12 header2 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint2>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint2(iter, header2);
                        return res;
                    });

                case 3:
                    TsLasHeader12 header3 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint3>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint3(iter, header3);
                        return res;
                    });

                case 4:
                    TsLasHeader13 header4 = (TsLasHeader13)prmHeader;
                    return ReadPoints<TsLasPoint4>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint4(iter, header4);
                        return res;
                    });

                case 5:
                    TsLasHeader13 header5 = (TsLasHeader13)prmHeader;
                    return ReadPoints<TsLasPoint5>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint5(iter, header5);
                        return res;
                    });

                case 6:
                    TsLasHeader14 header6 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint6>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint6(iter, header6);
                        return res;
                    });

                case 7:
                    TsLasHeader14 header7 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint7>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint7(iter, header7);
                        return res;
                    });

                case 8:
                    TsLasHeader14 header8 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint8>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint8(iter, header8);
                        return res;
                    });

                case 9:
                    TsLasHeader14 header9 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint9>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint9(iter, header9);
                        return res;
                    });

                case 10:
                    TsLasHeader14 header10 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint10>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint10(iter, header10);
                        return res;
                    });
            }
            return null;
        }

        //------------------------------------------------------------------
        /// <summary>
        /// This function reads a given number of points from the current location of the reader.
        /// If enough points not found in the file, it returns the available points.
        /// </summary>
        /// <typeparam name="T">Type of the las point</typeparam>
        /// <param name="prmNoOfPoints">Number of points to read</param>
        /// <param name="prmHeader">LAS header interface</param>
        /// <returns>An array of LAS points</returns>
        public TcLasPointBase[] ReadPointsAsObject<T>(Int64 prmNoOfPoints, TiLasHeader prmHeader) where T : TiLasPoint
        {
            Int64 pointCounter = 0;
            switch (prmHeader.PointDataFormatID)
            {
                case 0:
                    TsLasHeader12 header0 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint0>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                        {
                            res[pointCounter++] = new TcLasPoint0(iter, header0);
                            return res;
                        });

                case 1:
                    TsLasHeader12 header1 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint1>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint1(iter, header1);
                        return res;
                    });

                case 2:
                    TsLasHeader12 header2 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint2>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint2(iter, header2);
                        return res;
                    });

                case 3:
                    TsLasHeader12 header3 = (TsLasHeader12)prmHeader;
                    return ReadPoints<TsLasPoint3>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint3(iter, header3);
                        return res;
                    });

                case 4:
                    TsLasHeader13 header4 = (TsLasHeader13)prmHeader;
                    return ReadPoints<TsLasPoint4>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint4(iter, header4);
                        return res;
                    });

                case 5:
                    TsLasHeader13 header5 = (TsLasHeader13)prmHeader;
                    return ReadPoints<TsLasPoint5>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint5(iter, header5);
                        return res;
                    });

                case 6:
                    TsLasHeader14 header6 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint6>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint6(iter, header6);
                        return res;
                    });

                case 7:
                    TsLasHeader14 header7 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint7>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint7(iter, header7);
                        return res;
                    });

                case 8:
                    TsLasHeader14 header8 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint8>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint8(iter, header8);
                        return res;
                    });

                case 9:
                    TsLasHeader14 header9 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint9>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint9(iter, header9);
                        return res;
                    });

                case 10:
                    TsLasHeader14 header10 = (TsLasHeader14)prmHeader;
                    return ReadPoints<TsLasPoint10>(prmNoOfPoints).Aggregate(new TcLasPointBase[prmNoOfPoints], (res, iter) =>
                    {
                        res[pointCounter++] = new TcLasPoint10(iter, header10);
                        return res;
                    });
            }
            return null;
        }
        //------------------------------------------------------------------

        public void SeekToPoint(Int64 prmPoint)
        {
            m_Reader.BaseStream.Seek(m_Header.PointOffset + prmPoint * m_Header.PointDataRecordLength, SeekOrigin.Begin);
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