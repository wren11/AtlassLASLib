using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Atlass.LAS.Lib.Operations.Tiling.Types;

namespace Atlass.LAS.Lib.Operations.Tiling
{
    public class TcTileUtils
    {
        static public TcTileBlockInfoCollection GetTileBlocks(String prmFile)
        {
            using (StreamReader xmlReader = new StreamReader(prmFile))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TcTileBlockInfoCollection));
                    return serializer.Deserialize(xmlReader) as TcTileBlockInfoCollection;
                }
                finally
                {
                    xmlReader.Close();
                }
            }
        }
        //-----------------------------------------------------------------------------

        static public void SaveTileBlocks(TcTileBlockInfoCollection prmInfo, String prmFile)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(prmFile))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TcTileBlockInfoCollection));
                    serializer.Serialize(xmlWriter, prmInfo);
                }
                finally
                {
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------
