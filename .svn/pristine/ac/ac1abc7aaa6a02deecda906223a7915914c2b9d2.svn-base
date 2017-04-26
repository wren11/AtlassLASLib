using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Utilities
{
    public class TcFileUtils
    {
        /// <summary>
        /// Function to return points from a polygon file with empty lines as null.
        /// </summary>
        /// <param name="prmFile">Polygon file</param>
        /// <param name="prmSeparator">Separator</param>
        /// <param name="prmIsXYInverted">True if the sequence is X, Y and False otherwise</param>
        /// <returns>List of points</returns>
        static private List<TcPoint> GetPointsFromPolygonFileWithNull(String prmFile, Char prmSeparator, Boolean prmIsXYInverted = false)
        {
            String[] lines = File.ReadAllLines(prmFile);

            Int32 orderOfX = prmIsXYInverted ? 1 : 0;
            Int32 orderOfY = prmIsXYInverted ? 0 : 1;
            Double data;

            List<TcPoint> points = new List<TcPoint>();
            String[] values;
            for (int i = 0; i < lines.Length; i++)
            {
                values = lines[i].Split(String.Format("{0}\t", prmSeparator).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                // Check whether sufficient data is available.
                if (values.Length > 1)
                {
                    points.Add(
                            new TcPoint()
                            {
                                X = Double.TryParse(values[orderOfX], out data) ? data : 0,
                                Y = Double.TryParse(values[orderOfY], out data) ? data : 0,
                                Z = values.Length > 2 && Double.TryParse(values[2], out data) ? data : 0
                            }
                        );
                }
                else
                {
                    points.Add(null);
                }
            }
            return points;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to return points from a polygon file.
        /// </summary>
        /// <param name="prmFile">Polygon file</param>
        /// <param name="prmSeparator">Separator</param>
        /// <param name="prmIsXYInverted">True if the sequence is X, Y and False otherwise</param>
        /// <returns>List of points</returns>
        static public IEnumerable<TcPoint> GetPoints(String prmFile, Char prmSeparator, Boolean prmIsXYInverted = false)
        {
            IEnumerable<TcPoint> allPoints = GetPointsFromPolygonFileWithNull(prmFile, prmSeparator, prmIsXYInverted);
            return allPoints.Count() > 0 ? allPoints.Where(iter => iter != null) : new List<TcPoint>();
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Get a list of points from the polygon files
        /// </summary>
        /// <param name="prmFile">Input file</param>
        /// <param name="prmIsInverted">Is X and Y are inverted</param>
        /// <returns>Returns the list of points</returns>
        static public IEnumerable<TcPoint> GetPoints(String prmFile, Boolean prmIsInverted = false)
        {
            if (prmFile.EndsWith(".coo"))
            {
                return GetPoints(prmFile, ' ', prmIsInverted);
            }

            if (prmFile.EndsWith(".asc"))
            {
                return GetPoints(prmFile, ',', prmIsInverted);
            }

            throw new InvalidDataException("Polygon file format not supported");
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to return polygons from a polygon file with empty lines as null.
        /// </summary>
        /// <param name="prmFile">Polygon file</param>
        /// <param name="prmSeparator">Separator</param>
        /// <param name="prmIsXYInverted">True if the sequence is X, Y and False otherwise</param>
        /// <returns>List of polygons</returns>
        static private List<TcPolygon> GetPolygons(String prmFile, Char prmSeparator, Double prmBuffer, Boolean prmIsXYInverted)
        {
            IEnumerable<TcPoint> allPoints = GetPointsFromPolygonFileWithNull(prmFile, prmSeparator, prmIsXYInverted);

            List<TcPolygon> polygons = new List<TcPolygon>();
            List<TcPoint> points = new List<TcPoint>();
            foreach (TcPoint point in allPoints)
            {
                if (point == null && points.Count > 0)
                {
                    TcPolygon poly = new TcPolygon(points);
                    poly.AddOffset(prmBuffer);
                    polygons.Add(poly);
                    points.Clear();
                }
                else if (point != null)
                {
                    points.Add(point);
                }
            }

            if (points.Count > 0)
            {
                polygons.Add(new TcPolygon(points));
            }

            return polygons;
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Get a list of polygons from the polygon files
        /// </summary>
        /// <param name="prmFile">Input file</param>
        /// <param name="prmXYInverted">Is X and Y are inverted</param>
        /// <returns>Returns the list of polygons</returns>
        static public IEnumerable<TcPolygon> GetPolygons(String prmFile, Double prmOffset, Boolean prmXYInverted)
        {
            if (prmFile.EndsWith(".coo"))
            {
                return GetPolygons(prmFile, ' ', prmOffset, prmXYInverted);
            }

            if (prmFile.EndsWith(".asc"))
            {
                return GetPolygons(prmFile, ',', prmOffset, prmXYInverted);
            }

            throw new InvalidDataException("Polygon file format not supported");
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Get a list of polygons from the polygon files
        /// </summary>
        /// <param name="prmFile">Input file</param>
        /// <param name="prmBuffer">The buffer around the polygon</param>
        /// <returns>Returns the list of polygons</returns>
        static public IEnumerable<TcPolygon> GetPolygons(String prmFile, Double prmBuffer)
        {
            return GetPolygons(prmFile, prmBuffer, false);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Get a list of polygons from the polygon files
        /// </summary>
        /// <param name="prmFile">Input file</param>
        /// <param name="prmXYInverted">Is X and Y are inverted</param>
        /// <returns>Returns the list of polygons</returns>
        static public IEnumerable<TcPolygon> GetPolygons(String prmFile, Boolean prmXYInverted)
        {
            return GetPolygons(prmFile, 0, prmXYInverted);
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Get a list of polygons from the polygon files
        /// </summary>
        /// <param name="prmFile">Input file</param>
        /// <returns>Returns the list of polygons</returns>
        static public IEnumerable<TcPolygon> GetPolygons(String prmFile)
        {
            return GetPolygons(prmFile, 0, false);
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
