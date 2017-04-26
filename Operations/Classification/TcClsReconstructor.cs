///<summary> TcClsReconstructor
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class extracts the classification bytes from the LAS files after sorting
/// the LAS points according to the GPS time. It exports the points with zero GPS
/// time as a collection of new LAS points in a format [X Y Z Classification]. At
/// the end, it creates a .acb (Atlass Classification Bytes) and .acl (Atlass
/// Classification) for each LAS tile.
/// 
/// @Functions:
///  - LasToCls - Extracts [X Y Z Classification] (13 bytes) from any LAS file
///    and produce .acl file. The file header is of the same size of the LAS
///    header which would be 227/235/325. The structure of the file would be
///    +------------------+------+------+------+~ ~ ~ ~ ~ ~ ~+------+
///    | LAS Header (227) | XYZC | XYZC | XYZC |             | XYZC |
///    +------------------+------+------+------+~ ~ ~ ~ ~ ~ ~+------+
///                    0      1      2                     N
///    The function also produced a binary stream of classification bytes, 1 byte
///    per LAS point (doesn't count added/removed points). If there is a removed
///    point, it fills that with 255 (dummy classification). The file header is of
///    8 bytes which contains the number of records in the file.
///    
///    +------------+---+---+---+~ ~ ~ ~ ~ ~ ~+---+
///    | Header (8) | C | C | C |             | C |
///    +------------+---+---+---+~ ~ ~ ~ ~ ~ ~+---+
///                   0   1   2                 N
///    
///  - ClsToLas - Creates a dummy LAS file from the intermediate .acl file with
///    real [X Y Z Classification] in the LAS 1.2 PDRF1 format. This function
///    updates the GPS time with a sequential number started from 1, which
///    represents the sort key of the points.

/// 
/// @Pros:
///  - Highest possible thinned classification information to be transmitted over
///    the Internet with minimum time.
///  - Can export from any LAS format.
/// 
/// @Cons:
///  - Slow processing because of the restriction of sorting the points.
///  - Block reading is not possible as the sorting needs to be done on all points.
///  
/// <author>
/// Name: S M Kamrul Hasan
/// Date: 31-AUG-2014
/// </author>
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
///</summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Operations.Classification.IO;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Types;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Operations.Classification
{
    public class TcClsReconstructor : TiOperation, IDisposable
    {
        /// <summary>
        /// Callback for any message to be passed to the parent thread.
        /// </summary>
        public override event EventHandler<TcMessageEventArgs> OnMessage;

        /// <summary>
        /// Callback for any error happened in the processing.
        /// </summary>
        public override event EventHandler<TcErrorEventArgs> OnError;

        /// <summary>
        /// Callback to notify the parent thread about process finish.
        /// </summary>
        public override event EventHandler<EventArgs> OnFinish;

        /// <summary>
        /// The percentage of progress to be used for notifying the caller.
        /// </summary>
        public override Double ProgressFrequency { protected get; set; }

        private Double m_Progress;
        
        public TcClsReconstructor()
        {
            m_Progress = 0;
            ProgressFrequency = 5;
        }
        //-----------------------------------------------------------------------------

        protected override void ReportMessage(String prmMessage)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new TcMessageEventArgs(prmMessage));
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportFinished()
        {
            if (OnFinish != null)
            {
                OnFinish(this, new EventArgs());
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportError(String prmError, Exception prmEx)
        {
            if (OnError != null)
            {
                OnError(this, new TcErrorEventArgs(prmError, prmEx));
            }
        }
        //-----------------------------------------------------------------------------

        private void ReportProgress(Int64 prmPointsProcessed, Int64 prmTotalPoints)
        {
            Double progress = (prmPointsProcessed * 1.0 / prmTotalPoints) * 100;
            if (progress - m_Progress > ProgressFrequency)
            {
                ReportMessage(String.Format("{0:0.00}% points processed", progress));
                m_Progress = progress;
            }
        }
        //-----------------------------------------------------------------------------

        private TiLasHeader GetHeaderForCls(TiLasHeader prmHeader, Int64 prmNewNumberOfPoints)
        {
            if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader12)))
            {
                TsLasHeader12 header = (TsLasHeader12)prmHeader;
                header.NumberOfPointRecords = (UInt32)prmNewNumberOfPoints;
                header.NumberofPointsByReturn1 = header.NumberOfPointRecords;
                header.NumberofPointsByReturn2 = 0;
                header.NumberofPointsByReturn3 = 0;
                header.NumberofPointsByReturn4 = 0;
                header.NumberofPointsByReturn5 = 0;
                return header;
            }
            else if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader13)))
            {
                TsLasHeader13 header = (TsLasHeader13)prmHeader;
                header.NumberOfPointRecords = (UInt32)prmNewNumberOfPoints;
                header.NumberofPointsByReturn1 = header.NumberOfPointRecords;
                header.NumberofPointsByReturn2 = 0;
                header.NumberofPointsByReturn3 = 0;
                header.NumberofPointsByReturn4 = 0;
                header.NumberofPointsByReturn5 = 0;
                return header;
            }
            else if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader14)))
            {
                TsLasHeader14 header = (TsLasHeader14)prmHeader;
                header.LegNumberOfPointRecords = (UInt32)prmNewNumberOfPoints;
                header.LegNumberofPointsByReturn1 = header.LegNumberOfPointRecords;
                header.LegNumberofPointsByReturn2 = 0;
                header.LegNumberofPointsByReturn3 = 0;
                header.LegNumberofPointsByReturn4 = 0;
                header.LegNumberofPointsByReturn5 = 0;
                
                header.NumberOfPointRecords = (UInt64)prmNewNumberOfPoints;
                header.NumberofPointsByReturn1 = header.NumberOfPointRecords;
                header.NumberofPointsByReturn2 = 0;
                header.NumberofPointsByReturn3 = 0;
                header.NumberofPointsByReturn4 = 0;
                header.NumberofPointsByReturn5 = 0;
                header.NumberofPointsByReturn6 = 0;
                header.NumberofPointsByReturn7 = 0;
                header.NumberofPointsByReturn8 = 0;
                header.NumberofPointsByReturn9 = 0;
                header.NumberofPointsByReturn10 = 0;
                header.NumberofPointsByReturn11 = 0;
                header.NumberofPointsByReturn12 = 0;
                header.NumberofPointsByReturn13 = 0;
                header.NumberofPointsByReturn14 = 0;
                header.NumberofPointsByReturn15 = 0;
                return header;
            }

            throw new InvalidDataException("Header size is wrong");
        }
        //-----------------------------------------------------------------------------

        private TiLasHeader GetHeaderForMergedLas(TiLasHeader prmHeader, Int64 prmAddedPoints)
        {
            if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader12)))
            {
                TsLasHeader12 header = (TsLasHeader12)prmHeader;
                header.NumberOfPointRecords += (UInt32)prmAddedPoints;
                header.NumberofPointsByReturn1 += (UInt32)prmAddedPoints;
                return header;
            }
            else if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader13)))
            {
                TsLasHeader13 header = (TsLasHeader13)prmHeader;
                header.NumberOfPointRecords += (UInt32)prmAddedPoints;
                header.NumberofPointsByReturn1 += (UInt32)prmAddedPoints;
                return header;
            }
            else if (prmHeader.HeaderSize == Marshal.SizeOf(typeof(TsLasHeader14)))
            {
                TsLasHeader14 header = (TsLasHeader14)prmHeader;
                header.LegNumberOfPointRecords += (UInt32)prmAddedPoints;
                header.LegNumberofPointsByReturn1 += (UInt32)prmAddedPoints;
                header.NumberOfPointRecords += (UInt64)prmAddedPoints;
                header.NumberofPointsByReturn1 += (UInt64)prmAddedPoints;
                return header;
            }

            throw new InvalidDataException("Header size is wrong");
        }
        //-----------------------------------------------------------------------------

        private TiLasGPS ClsToLasPoint(TsClsLasPoint prmPoint, Byte prmPDRF)
        {
            switch (prmPDRF)
            {
                case 1:
                    return new TsLasPoint1()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0
                    };

                case 3:
                    return new TsLasPoint3()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        Red = 0,
                        Green = 0,
                        Blue = 0
                    };

                case 4:
                    return new TsLasPoint4()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        WPDI = 0,
                        WFOffset = 0,
                        WFPacketSize = 0,
                        WFReturnLocation = 0,
                        WFXt = 0,
                        WFYt = 0,
                        WFZt = 0
                    };

                case 5:
                    return new TsLasPoint5()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        Red = 0,
                        Green = 0,
                        Blue = 0,
                        WPDI = 0,
                        WFOffset = 0,
                        WFPacketSize = 0,
                        WFReturnLocation = 0,
                        WFXt = 0,
                        WFYt = 0,
                        WFZt = 0
                    };

                case 6:
                    return new TsLasPoint6()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        ReturnMask = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0
                    };

                case 7:
                    return new TsLasPoint7()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        ReturnMask = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        Red = 0,
                        Green = 0,
                        Blue = 0
                    };

                case 8:
                    return new TsLasPoint8()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        ReturnMask = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        Red = 0,
                        Green = 0,
                        Blue = 0,
                        NIR = 0
                    };

                case 9:
                    return new TsLasPoint9()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        ReturnMask = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        WPDI = 0,
                        WFOffset = 0,
                        WFPacketSize = 0,
                        WFReturnLocation = 0,
                        WFXt = 0,
                        WFYt = 0,
                        WFZt = 0
                    };

                case 10:
                    return new TsLasPoint10()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        ReturnMask = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0,
                        GPSTime = 0,
                        NIR = 0,
                        WPDI = 0,
                        WFOffset = 0,
                        WFPacketSize = 0,
                        WFReturnLocation = 0,
                        WFXt = 0,
                        WFYt = 0,
                        WFZt = 0
                    };

                default:
                    throw new FormatException("Invalid LAS file format");
            }
        }
        //-----------------------------------------------------------------------------

        private void LasToCls<T>(TcLasReader prmReader, Int64 prmOriginalNumberOfPoints, String prmOutputAcl, String prmOutputAcb) where T : TiLasGPS
        {
            ReportMessage(String.Format("Processing {0}", Path.GetFileName(prmOutputAcb)));
            ReportMessage("Reading all LAS points");

            // Read all the points.
            T[] clsPoints = prmReader.ReadPoints<T>(prmReader.TotalPoints);

            ReportMessage("Sorting LAS points according to the GPS time");

            // Sort the points according to the GPS time.
            new TcLasSort<T>().SortByGpsTime(clsPoints, 0, prmReader.TotalPoints - 1);

            Int64 pointsProcessed = 0;
            Byte[] clsData = new Byte[prmOriginalNumberOfPoints];

            ReportMessage("Extracting the new points");

            // Count the number of points with GPSTime ~ 0 (new point)
            Int32 exptCnt = 0;
            while (clsPoints[exptCnt].GPSTime < 1) { exptCnt++; }

            // Write the new points in .acl file.
            if (exptCnt > 0)
            {
                using (TcClsWriter clsWriter = new TcClsWriter(GetHeaderForCls(prmReader.Header, exptCnt), prmOutputAcl))
                {
                    clsWriter.WriteHeader(prmReader.OffsetBytes);
                    clsWriter.WritePoints<T>(clsPoints.Take(exptCnt).ToArray());
                }
            }
            pointsProcessed += exptCnt;

            // Counter for the original points.
            Int64 origPtCnt = 0;

            // Current expected GPS time to map the deleted points.
            Double expecteGPSTime = 1.0;

            ReportMessage("Processing original points");

            while (pointsProcessed < clsPoints.Length && origPtCnt < prmOriginalNumberOfPoints)
            {
                // If deleted points have been detected.
                if (clsPoints[pointsProcessed].GPSTime != expecteGPSTime)
                {
                    clsData[origPtCnt++] = 255;
                    expecteGPSTime++;
                    continue;
                }

                clsData[origPtCnt++] = clsPoints[pointsProcessed++].Classification;
                expecteGPSTime++;
            }

            ReportMessage("Writing the output classification files");

            // Write into the stream.
            if (origPtCnt > 0)
            {
                using (BinaryWriter acbWriter = new BinaryWriter(new FileStream(prmOutputAcb, FileMode.Create)))
                {
                    acbWriter.Write(origPtCnt);
                    acbWriter.Write(clsData);
                }
            }
            ReportMessage(String.Format("Finished {0}", Path.GetFileName(prmOutputAcb)));
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(String prmInput, Int64 prmOriginalNumberOfPoints, String prmOutputAcl, String prmOutputAcb)
        {
            try
            {
                // Process the LAS file.
                using (TcLasReader reader = new TcLasReader(prmInput))
                {
                    switch (reader.Header.PointDataFormatID)
                    {
                        case 1:
                            LasToCls<TsLasPoint1>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 3:
                            LasToCls<TsLasPoint3>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 4:
                            LasToCls<TsLasPoint4>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 5:
                            LasToCls<TsLasPoint5>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 6:
                            LasToCls<TsLasPoint6>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 7:
                            LasToCls<TsLasPoint7>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 8:
                            LasToCls<TsLasPoint8>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 9:
                            LasToCls<TsLasPoint9>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        case 10:
                            LasToCls<TsLasPoint10>(reader, prmOriginalNumberOfPoints, prmOutputAcl, prmOutputAcb);
                            break;

                        default:
                            throw new FormatException("Couldn't produce the file. LAS format not supported");
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError("Could not construct Acl and Acb files from LAS", ex);
            }
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(String prmInput, Int64 prmOriginalNumberOfPoints)
        {
            String fileBase = String.Format(@"{0}\{1}", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput));
            LasToCls(prmInput, prmOriginalNumberOfPoints, String.Format("{0}.acl", fileBase), String.Format("{0}.acb", fileBase));
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(String prmInput, Int64 prmOriginalNumberOfPoints, String prmOutputDirectory)
        {
            String fileBase = String.Format(@"{0}\{1}", prmOutputDirectory, Path.GetFileNameWithoutExtension(prmInput));
            LasToCls(prmInput, prmOriginalNumberOfPoints, String.Format("{0}.acl", fileBase), String.Format("{0}.acb", fileBase));
        }
        //-----------------------------------------------------------------------------

        private void ClsToLas<T>(TcLasReader prmReader, String prmAclFile, String prmAcbFile, String prmOutputLas) where T : TiLasGPS
        {
            using (BinaryReader acbReader = new BinaryReader(new FileStream(prmAcbFile, FileMode.Open)))
            {
                Int64 totalPoints = acbReader.ReadInt64();

                // When the binary file doesn't have enough points.
                if (totalPoints != (acbReader.BaseStream.Length - acbReader.BaseStream.Position))
                {
                    throw new InvalidDataException(String.Format("Length of data mismatched in {0} file", Path.GetFileName(prmAcbFile)));
                }

                // Check against the number of points in the original LAS files.
                if (totalPoints != prmReader.TotalPoints)
                {
                    throw new InvalidDataException(String.Format("{0} file doesn't have save number of points as LAS", Path.GetFileName(prmAcbFile)));
                }

                using (TcLasWriter lasWriter = new TcLasWriter(prmOutputLas))
                {
                    Int64 noOfPointsToProcess = 0;
                    Int64 noOfPointsProcessed = 0;
                    T[] points = new T[TcConstants.MaxLasPointsToProcessAtOnce];
                    Byte[] clsData = acbReader.ReadBytes((Int32)totalPoints);

                    // Write the header.
                    lasWriter.WriteHeader(prmReader.Header, prmReader.OffsetBytes);

                    while (noOfPointsProcessed < prmReader.TotalPoints)
                    {
                        // Calculate how many points to read.
                        noOfPointsToProcess = Math.Min(TcConstants.MaxLasPointsToProcessAtOnce, prmReader.TotalPoints - noOfPointsProcessed);

                        // Read a block of points from the LAS file.
                        points = prmReader.ReadPoints<T>(noOfPointsToProcess);

                        // Update the classification flag.
                        for (int i = 0; i < noOfPointsToProcess; i++)
                        {
                            points[i].Classification = clsData[noOfPointsProcessed + i];
                        }

                        lasWriter.WritePoints<T>(points);

                        // Notify the progress to the caller thread.
                        ReportProgress(noOfPointsProcessed, totalPoints);

                        noOfPointsProcessed += noOfPointsToProcess;
                    }

                    // Process the extra points if there is any.
                    if (!String.IsNullOrWhiteSpace(prmAclFile) && File.Exists(prmAclFile))
                    {
                        using (TcClsReader clsReader = new TcClsReader(prmAclFile))
                        {
                            TsClsLasPoint[] extraPoints = clsReader.ReadPoints(clsReader.TotalPoints);
                            TiLasGPS[] extraLasPoints = new TiLasGPS[clsReader.TotalPoints];

                            for (int i = 0; i < clsReader.TotalPoints; i++)
                            {
                                extraLasPoints[i] = ClsToLasPoint(extraPoints[i], clsReader.Header.PointDataFormatID);

                                // Point transformation as the offset and/or scaling factor might be changed in different software.
                                extraLasPoints[i].X = (Int32)(((clsReader.Header.XOffset + extraLasPoints[i].X * clsReader.Header.XScaleFactor) - prmReader.Header.XOffset) / prmReader.Header.XScaleFactor);
                                extraLasPoints[i].Y = (Int32)(((clsReader.Header.YOffset + extraLasPoints[i].Y * clsReader.Header.YScaleFactor) - prmReader.Header.YOffset) / prmReader.Header.YScaleFactor);
                                extraLasPoints[i].Z = (Int32)(((clsReader.Header.ZOffset + extraLasPoints[i].Z * clsReader.Header.ZScaleFactor) - prmReader.Header.ZOffset) / prmReader.Header.ZScaleFactor);
                            }
                            lasWriter.WritePoints(extraLasPoints);

                            // Write the updated header.
                            lasWriter.WriteHeader(GetHeaderForMergedLas(prmReader.Header, clsReader.TotalPoints));
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------------

        public void ClsToLas(String prmOriginalLasFile, String prmAclFile, String prmAcbFile, String prmOutputLas)
        {
            try
            {
                using (TcLasReader lasReader = new TcLasReader(prmOriginalLasFile))
                {
                    switch (lasReader.Header.PointDataFormatID)
                    {
                        case 1:
                            ClsToLas<TsLasPoint1>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 3:
                            ClsToLas<TsLasPoint3>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 4:
                            ClsToLas<TsLasPoint4>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 5:
                            ClsToLas<TsLasPoint5>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 6:
                            ClsToLas<TsLasPoint6>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 7:
                            ClsToLas<TsLasPoint7>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 8:
                            ClsToLas<TsLasPoint8>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 9:
                            ClsToLas<TsLasPoint9>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        case 10:
                            ClsToLas<TsLasPoint10>(lasReader, prmAclFile, prmAcbFile, prmOutputLas);
                            break;

                        default:
                            throw new FormatException("Couldn't produce the file. LAS format not supported");
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError("Could not reconstruct LAS from Acb and Acl files", ex);
            }
        }
        //-----------------------------------------------------------------------------

        public void Dispose()
        {
            OnMessage = null;
            OnFinish = null;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------