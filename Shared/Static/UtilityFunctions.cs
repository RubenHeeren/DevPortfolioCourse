using System;
using System.Reflection;

namespace Shared.Static
{
    public static class UtilityFunctions
    {
        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyObjectProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();

            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }

                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);

                if (targetProperty == null)
                {
                    continue;
                }

                if (!targetProperty.CanWrite)
                {
                    continue;
                }

                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }

                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }

                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }

                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }

        public static string ConvertURLToTitle(this string str)
        {
            string stringWithHypensReplacedBySpaces = str.Replace('-', ' ');

            if (ContainsSpaceThreeTimesInARow(stringWithHypensReplacedBySpaces))
            {
                return stringWithHypensReplacedBySpaces.Replace("   ", " - ");
            }
            else
            {
                return stringWithHypensReplacedBySpaces;
            }                      
        }

        public static string ConvertTitleToURL(this string str) => str.Replace(' ', '-');

        private static bool ContainsSpaceThreeTimesInARow(string str)
        {           
            for (int i = 2; i < str.Length; ++i)
            {
                if (str[i] == ' ' && str[i] == str[i - 1] && str[i] == str[i - 2])
                {
                    return true;
                }                    
            }

            return false;
        }
    }
}