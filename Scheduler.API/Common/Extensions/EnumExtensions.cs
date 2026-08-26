using System.ComponentModel;

namespace Scheduler.API.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string ToIntegerValueString(this Enum enumeration)
        {
            var field = enumeration.GetType().GetField(enumeration.ToString());
            return ((int)field.GetValue(enumeration)).ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

    }
}
