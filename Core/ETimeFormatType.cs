using System;

namespace SS.Gather.Core
{
    public enum ETimeFormatType
    {
        ShortTime,					//8:09
        LongTime					//8:09:24
    }

    public class ETimeFormatTypeUtils
    {
        public static string GetValue(ETimeFormatType type)
        {
            if (type == ETimeFormatType.ShortTime)
            {
                return "ShortTime";
            }
            if (type == ETimeFormatType.LongTime)
            {
                return "LongTime";
            }
            throw new Exception();
        }

        public static string GetText(ETimeFormatType type)
        {
            if (type == ETimeFormatType.ShortTime)
            {
                return "8:09";
            }
            if (type == ETimeFormatType.LongTime)
            {
                return "8:09:24";
            }
            throw new Exception();
        }

        public static ETimeFormatType GetEnumType(string typeStr)
        {
            var retVal = ETimeFormatType.ShortTime;

            if (Equals(ETimeFormatType.ShortTime, typeStr))
            {
                retVal = ETimeFormatType.ShortTime;
            }
            else if (Equals(ETimeFormatType.LongTime, typeStr))
            {
                retVal = ETimeFormatType.LongTime;
            }

            return retVal;
        }

        public static bool Equals(ETimeFormatType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ETimeFormatType type)
        {
            return Equals(type, typeStr);
        }
    }
}
