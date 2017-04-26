using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Atlass.LAS.Lib.Types.DataType;

namespace Atlass.LAS.Lib.Operations.Types
{
    [DefaultValue(TeTaskStatus.Unknown)]
    public enum TeTaskStatus
    {
        [Description("Unknown Status")]
        Unknown = -1,

        [Description("Waiting")]
        Waiting = 0,

        [Description("Processing")]
        Processing = 1,

        [Description("Finished")]
        Finished = 2,

        [Description("Error")]
        Error = 3
    }
    //------------------------------------------------------------------

    [DefaultValue(TeTaskType.Unknown)]
    public enum TeTaskType
    {
        [Description("Unknown Task")]
        Unknown = -1,

        [Description("Export LAS as Tiles")]
        LasTileExport = 0,

        [Description("Index the LAS Files")]
        LasIndex = 1,

        [Description("Combine LAS Files to One")]
        LasCombine = 2
    }
    //------------------------------------------------------------------

    [DefaultValue(TeTilingMethod.Unknown)]
    public enum TeTilingMethod
    {
        [Description("Unknown Method")]
        Unknown = -1,

        [Description("One Row at Once")]
        OneRowAtOnce = 0,

        [Description("Five Tiles at Once")]
        FiveTiles = 5,

        [Description("Ten Tiles at Once")]
        TenTiles = 10,

        [Description("Twenty Tiles at Once")]
        TwentyTiles = 20
    }
    //------------------------------------------------------------------

    [DefaultValue(TeGriddingType.Unknown)]
    public enum TeGriddingType
    {
        [Description("Unknown Type")]
        [ShortName("unk")]
        Unknown = -1,

        [Description("1 Meter Grid with First Echo")]
        [ShortName("m1fe")]
        M1FirstEcho = 0,

        [Description("1 Meter Grid with Last Echo")]
        [ShortName("m1le")]
        M1LastEcho = 1,

        [Description("2 Meters Grid for Levelling")]
        [ShortName("lvl")]
        Levelling = 2,

        [Description("5 Meters Grid for Display")]
        [ShortName("dsp")]
        Display = 5
    }
    //------------------------------------------------------------------

    [DefaultValue(TeTolTags.Unknown)]
    public enum TeTolTags
    {
        [Description("Unknown")]
        Unknown,

        [Description("IMAGENAME")]
        ImageName,

        [Description("DATETIME")]
        DateTime,

        [Description("LINES")]
        Rows,

        [Description("COLUMNS")]
        Columns,

        [Description("BIT/PIX /SW")]
        BitSize,

        [Description("HOR_SPC E/N")]
        Scaling,

        [Description("UP_LEFT_E")]
        UpperLeftEast,

        [Description("UP_LEFT_N")]
        UpperLeftNorth,

        [Description("LO_RIGHT_E")]
        LowerRightEast,

        [Description("LO_RIGHT_N")]
        LowerRightNorth,

        [Description("MIN_Z/G")]
        MinHeight,

        [Description("MAX_Z/G")]
        MaxHeight,

        [Description("MODEL")]
        Model,

        [Description("AJUSTED")]
        Adjusted
    }
    //------------------------------------------------------------------

    public enum TeClsFileType
    {
        [Description("Classification file (*.acl)")]
        [ShortName("acl")]
        ACL,

        [Description("Zipped class file (*.zcl)")]
        [ShortName("zcl")]
        ZCL
    }
    //------------------------------------------------------------------

    public enum TeLasSortType
    {
        [Description("Sort by GPS Time")]
        [ShortName("_gps")]
        GpsTime,

        [Description("Sort by X")]
        [ShortName("_x")]
        X,

        [Description("Sort by Y")]
        [ShortName("_y")]
        Y,

        [Description("Sort by Z")]
        [ShortName("_z")]
        Z,

        [Description("Sort by Classification")]
        [ShortName("_cls")]
        Classification,
    }
    //------------------------------------------------------------------

    public class TcEnums
    {
        static public String Description(Enum prmValue)
        {
            FieldInfo info = prmValue.GetType().GetField(prmValue.ToString());
            DescriptionAttribute[] attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes.Count() > 0 ? attributes[0].Description : prmValue.ToString();
        }
        //------------------------------------------------------------------

        static public List<String> Descriptions(Type prmType)
        {
            return Enum.GetValues(prmType).Cast<Enum>().Select(iter => Description(iter)).ToList();
        }
        //------------------------------------------------------------------

        static public T GetEnumFromDescription<T>(String description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields.SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                            .Description == description).SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
        //------------------------------------------------------------------

        static public String ShortName(Enum prmValue)
        {
            FieldInfo info = prmValue.GetType().GetField(prmValue.ToString());
            ShortName[] attributes = info.GetCustomAttributes(typeof(ShortName), false) as ShortName[];
            return attributes.Count() > 0 ? attributes[0].Value : prmValue.ToString();
        }
        //------------------------------------------------------------------

        static public List<String> ShortNames(Type prmType)
        {
            return Enum.GetValues(prmType).Cast<Enum>().Select(iter => ShortName(iter)).ToList();
        }
        //------------------------------------------------------------------

        static public T GetEnumFromShortName<T>(String description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields.SelectMany(f => f.GetCustomAttributes(
                                typeof(ShortName), false), (
                                    f, a) => new { Field = f, Att = a })
                              .Where(a => ((ShortName)a.Att).Value == description)
                              .SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
        //------------------------------------------------------------------

        public static T GetDefaultValue<T>() where T : struct
        {
            Type t = typeof(T);
            DefaultValueAttribute[] attributes = (DefaultValueAttribute[])t.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            if (attributes != null &&
                attributes.Length > 0)
            {
                return (T)attributes[0].Value;
            }
            else
            {
                return (T)Enum.GetValues(typeof(T)).GetValue(0);
            }
        }
        //------------------------------------------------------------------

        static public T GetEnumFromInt<T>(Int32 value)
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        //------------------------------------------------------------------

    }
    //------------------------------------------------------------------

}
//------------------------------------------------------------------