using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenTNF.Library
{
    public static class Extensions
    {
        private static readonly uint[] Lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        public static string ToHex(this byte[] value)
        {
            var lookup32 = Lookup32;
            var result = new char[value.Length * 2];

            for (int i = 0; i < value.Length; i++)
            {
                var val = lookup32[value[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }

            return FormatHex(new string(result));
        }

        private static string FormatHex(string value)
        {
            var res = Regex.Replace(value, ".{4}", "$0 ");
            return Regex.Replace(res, "(.{39})(.)", "$0" + Environment.NewLine);
        }

        public static bool? ToBoolean(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            return Convert.ToBoolean(value);
        }

        public static DateTime? ToDateTime(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            if (value is DateTime)
            {
                return (DateTime)value;
            }

            if (value is string || value is int)
            {
                DateTime date;

                if (DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date;
                }

                if (DateTime.TryParse(value.ToString(), out date))
                {
                    return date;
                }
            }

            return null;
        }

        public static double? ToDouble(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            if (value is decimal)
            {
                return Convert.ToDouble(value);
            }

            return (double)value;
        }

        public static double? ToDouble(this IDataRecord reader, int index)
        {
            if (reader[index] is DBNull)
            {
                return null;
            }

            return (double)reader.GetDecimal(index);
        }

        public static Int16? ToInt16(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            return Convert.ToInt16(value);
        }

        public static int? ToInt32(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            return Convert.ToInt32(value);
        }

        public static Int64? ToInt64(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            return Convert.ToInt64(value);
        }

        public static int ToInt(this object value)
        {
            return Convert.ToInt32(value);
        }

        public static string FromDbString(this object value)
        {
            if (value is DBNull)
            {
                return null;
            }

            return value.ToString();
        }

        public static object ReadIfExists(this IDataRecord dataRecord, string name)
        {
            if (dataRecord.HasName(name))
            {
                return dataRecord[name];
            }
            return DBNull.Value;
        }

        public static bool HasName(this IDataRecord dataRecord, string name)
        {
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                if (string.Equals(dataRecord.GetName(i), name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
