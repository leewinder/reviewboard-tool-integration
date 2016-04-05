using System;
using System.ComponentModel;
using System.Reflection;

namespace RB_Tools.Shared.Extensions
{
    // Enum extension methods
    public static class EnumExtensions
    {
        //
        // Returns the description of the enum if it's attached
        //
        public static string GetDescription(this Enum value)
        {
            // Get the type and name of this enum
            Type enumType = value.GetType();
            string enumName = Enum.GetName(enumType, value);

            // If we have a name, we can try and get the description
            if (enumName != null)
            {
                // Pull out the field
                FieldInfo field = enumType.GetField(enumName);
                if (field != null)
                {
                    // Get the description attribute if we can get it
                    DescriptionAttribute descAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (descAttribute != null)
                        return descAttribute.Description;
                }

                // Nothing, just return the name of the enum
                return enumName;
            }

            // Can't find anything
            return string.Empty;
        }
    }
}
