///<summary> TcClsConstructor
///+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// This class thins the LAS files and prepare .acl file for classification. It
/// only uses the [X Y Z Classification] from the original LAS file to create the
/// intermediate .acl file. The file contains the actual LAS header at the start
/// of the file. The class has function to reproduce a dummy LAS file of the same
/// format with real [X Y Z Classification].
/// 
/// @Functions:
///  - LasToCls - Extracts [X Y Z Classification] (13 bytes) from any LAS file
///    and produce .acl file. The structure of the file would be
///    +------------+------+------+------+~ ~ ~ ~ ~ ~ ~+------+
///    | LAS Header | XYZC | XYZC | XYZC |             | XYZC |
///    +------------+------+------+------+~ ~ ~ ~ ~ ~ ~+------+
///                    0      1      2                     N
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
using Atlass.LAS.Lib.Global;
using Atlass.LAS.Lib.Operations.Classification.IO;
using Atlass.LAS.Lib.Operations.IO;
using Atlass.LAS.Lib.Operations.Types;
using Atlass.LAS.Lib.Types;
using Atlass.LAS.Lib.Types.Interface;
using Atlass.LAS.Lib.Types.Struct;

namespace Atlass.LAS.Lib.Operations.Classification
{
    public class TcClsConstructor : TiOperation, IDisposable
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
        private TeClsFileType m_Type;

        public TcClsConstructor(TeClsFileType prmType)
        {
            m_Type = prmType;
            m_Progress = 0;
            ProgressFrequency = 5;
        }
        //-----------------------------------------------------------------------------

        public TcClsConstructor()
            : this(TeClsFileType.ACL)
        {
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

        #region LasToCls - Converter function from LAS to Classification format

        private void LasToCls<T>(TcLasReader prmReader, String prmOutput) where T : TiLasPoint
        {
            m_Progress = 0;

            ReportMessage(String.Format("Processing {0}", Path.GetFileName(prmOutput)));
            using (TcClsWriter writer = new TcClsWriter(prmReader.Header, prmOutput))
            {
                writer.WriteHeader(prmReader.OffsetBytes);
                
                Int64 numberOfPointRecords = prmReader.Header.GetNumberOfPoints();
                Int64 noOfPointsLoaded = 0;
                Int64 noOfPointsToRead = 0;
                
                while (noOfPointsLoaded < numberOfPointRecords)
                {
                    noOfPointsToRead = Math.Min(TcConstants.MaxLasPointsToProcessAtOnce, numberOfPointRecords - noOfPointsLoaded);
                    writer.WritePoints<T>(prmReader.ReadPoints<T>(noOfPointsToRead));
                    noOfPointsLoaded += noOfPointsToRead;
                    ReportProgress(noOfPointsLoaded, numberOfPointRecords);
                }
            }
            ReportMessage(String.Format("Finished {0}", Path.GetFileName(prmOutput)));
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(String prmInput, String prmOutput)
        {
            try
            {
                if (!File.Exists(prmInput))
                {
                    throw new FileNotFoundException("LAS file not found");
                }

                if (!Path.GetExtension(prmInput).Contains("las"))
                {
                    throw new InvalidDataException("Invalid input file format");
                }

                if (!Path.GetExtension(prmOutput).Contains(TcEnums.ShortName(m_Type)))
                {
                    throw new InvalidDataException("Invalid output file format");
                }

                using (TcLasReader reader = new TcLasReader(prmInput))
                {
                    switch (reader.Header.PointDataFormatID)
                    {
                        case 0:
                            LasToCls<TsLasPoint0>(reader, prmOutput);
                            break;

                        case 1:
                            LasToCls<TsLasPoint1>(reader, prmOutput);
                            break;

                        case 2:
                            LasToCls<TsLasPoint2>(reader, prmOutput);
                            break;

                        case 3:
                            LasToCls<TsLasPoint3>(reader, prmOutput);
                            break;

                        case 4:
                            LasToCls<TsLasPoint4>(reader, prmOutput);
                            break;

                        case 5:
                            LasToCls<TsLasPoint5>(reader, prmOutput);
                            break;

                        case 6:
                            LasToCls<TsLasPoint6>(reader, prmOutput);
                            break;

                        case 7:
                            LasToCls<TsLasPoint7>(reader, prmOutput);
                            break;

                        case 8:
                            LasToCls<TsLasPoint8>(reader, prmOutput);
                            break;

                        case 9:
                            LasToCls<TsLasPoint9>(reader, prmOutput);
                            break;

                        case 10:
                            LasToCls<TsLasPoint10>(reader, prmOutput);
                            break;

                        default:
                            throw new FormatException("Couldn't produce the file. LAS format not supported");
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError("Failed to convert LAS file into Acl", ex);
            }
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(String prmInput)
        {
            LasToCls(prmInput, String.Format(@"{0}\{1}.{2}", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput), TcEnums.ShortName(m_Type)));
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(IEnumerable<String> prmInputFiles, String prmOutputDirectory)
        {
            foreach (String file in prmInputFiles)
            {
                LasToCls(file, String.Format(@"{0}\{1}.{2}", prmOutputDirectory, Path.GetFileNameWithoutExtension(file), TcEnums.ShortName(m_Type)));
            }
        }
        //-----------------------------------------------------------------------------

        public void LasToCls(IEnumerable<String> prmInputFiles)
        {
            LasToCls(prmInputFiles, Path.GetDirectoryName(prmInputFiles.First()));
        }
        //-----------------------------------------------------------------------------

        #endregion LasToCls - Converter function from LAS to Classification format

        #region ClsToLas - Converter function from Classification format to LAS

        private TiLasPoint ClsToLasByPDRF(TsClsLasPoint prmPoint, Byte prmPDRF, Double prmGpsTime)
        {
            switch (prmPDRF)
            {
                case 0:
                    return new TsLasPoint0()
                    {
                        X = prmPoint.X,
                        Y = prmPoint.Y,
                        Z = prmPoint.Z,
                        Intensity = 0,
                        BitMask = 0,
                        Classification = prmPoint.Classification,
                        ScanAngleRank = 0,
                        UserData = 0,
                        PointSourceID = 0
                    };

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
                        GPSTime = prmGpsTime
                    };

                case 2:
                    return new TsLasPoint2()
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
                        Red = 0,
                        Green = 0,
                        Blue = 0
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime,
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
                        GPSTime = prmGpsTime,
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

        private void ClsToLas<T>(TcClsReader prmReader, String prmOutput) where T : TiLasPoint
        {
            m_Progress = 0;

            ReportMessage(String.Format("Processing {0}", Path.GetFileName(prmOutput)));
            using (TcLasWriter writer = new TcLasWriter(prmOutput))
            {
                writer.WriteHeader(prmReader.Header, prmReader.OffsetBytes);

                Int64 numberOfPointRecords = prmReader.Header.GetNumberOfPoints();
                Int64 noOfPointsLoaded = 0;
                Int64 noOfPointsToRead = 0;
                TiLasPoint[] lasPoints = new TiLasPoint[TcConstants.MaxLasPointsToProcessAtOnce];
                TsClsLasPoint[] clsPoints;
                Double pointIndex = 1;

                while (noOfPointsLoaded < numberOfPointRecords)
                {
                    noOfPointsToRead = Math.Min(TcConstants.MaxLasPointsToProcessAtOnce, numberOfPointRecords - noOfPointsLoaded);
                    clsPoints = prmReader.ReadPoints(noOfPointsToRead);
                    for (int i = 0; i < noOfPointsToRead; i++)
                    {
                        lasPoints[i] = ClsToLasByPDRF(clsPoints[i], prmReader.Header.PointDataFormatID, pointIndex++);
                    }

                    writer.WritePoints(lasPoints, noOfPointsToRead);
                    noOfPointsLoaded += noOfPointsToRead;
                    ReportProgress(noOfPointsLoaded, numberOfPointRecords);
                }
            }
            ReportMessage(String.Format("Finished {0}", Path.GetFileName(prmOutput)));
        }
        //-----------------------------------------------------------------------------

        public void ClsToLas(String prmInput, String prmOutput)
        {
            try
            {
                if (!File.Exists(prmInput))
                {
                    throw new FileNotFoundException("CLS file not found");
                }

                if (!Path.GetExtension(prmInput).Contains(TcEnums.ShortName(m_Type)))
                {
                    throw new InvalidDataException("Invalid input file format");
                }

                if (!Path.GetExtension(prmOutput).Contains("las"))
                {
                    throw new InvalidDataException("Invalid output file format");
                }

                using (TcClsReader reader = new TcClsReader(prmInput))
                {
                    switch (reader.Header.PointDataFormatID)
                    {
                        case 0:
                            ClsToLas<TsLasPoint0>(reader, prmOutput);
                            break;

                        case 1:
                            ClsToLas<TsLasPoint1>(reader, prmOutput);
                            break;

                        case 2:
                            ClsToLas<TsLasPoint2>(reader, prmOutput);
                            break;

                        case 3:
                            ClsToLas<TsLasPoint3>(reader, prmOutput);
                            break;

                        case 4:
                            ClsToLas<TsLasPoint4>(reader, prmOutput);
                            break;

                        case 5:
                            ClsToLas<TsLasPoint5>(reader, prmOutput);
                            break;

                        case 6:
                            ClsToLas<TsLasPoint6>(reader, prmOutput);
                            break;

                        case 7:
                            ClsToLas<TsLasPoint7>(reader, prmOutput);
                            break;

                        case 8:
                            ClsToLas<TsLasPoint8>(reader, prmOutput);
                            break;

                        case 9:
                            ClsToLas<TsLasPoint9>(reader, prmOutput);
                            break;

                        case 10:
                            ClsToLas<TsLasPoint10>(reader, prmOutput);
                            break;

                        default:
                            throw new FormatException("Couldn't produce the file. LAS format not supported");
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError("Failed to convert Acl file to LAS", ex);
            }
        }
        //-----------------------------------------------------------------------------

        public void ClsToLas(String prmInput)
        {
            ClsToLas(prmInput, String.Format(@"{0}\{1}_fake.las", Path.GetDirectoryName(prmInput), Path.GetFileNameWithoutExtension(prmInput)));
        }
        //-----------------------------------------------------------------------------

        public void ClsToLas(IEnumerable<String> prmInputFiles, String prmOutputDirectory)
        {
            foreach (String file in prmInputFiles)
            {
                ClsToLas(file, String.Format(@"{0}\{1}_fake.las", prmOutputDirectory, Path.GetFileNameWithoutExtension(file)));
            }
        }
        //-----------------------------------------------------------------------------

        public void ClsToLas(IEnumerable<String> prmInputFiles)
        {
            ClsToLas(prmInputFiles, Path.GetDirectoryName(prmInputFiles.First()));
        }
        //-----------------------------------------------------------------------------

        #endregion ClsToLas - Converter function from Classification format to LAS

        public void Dispose()
        {
            OnMessage = null;
            OnError = null;
            OnFinish = null;
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------