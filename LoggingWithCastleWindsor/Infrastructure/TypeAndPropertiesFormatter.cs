using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace LoggingWithCastleWindsor.Infrastructure
{
    public class TypeAndPropertiesFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly string _equals;

        public TypeAndPropertiesFormatter(bool compact = false)
        {
            _equals = compact ? "=" : " = ";
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!Equals(formatProvider))
            {
                return TryHandleOtherFormats(format, arg);
            }

            if (string.IsNullOrEmpty(format))
            {
                return TryHandleOtherFormats(null, arg);
            }

            if (arg == null)
            {
                return format.ToUpper() == "T" ? "Object" : TryHandleOtherFormats(null, null);
            }

            var type = arg.GetType();
            if (format.ToUpper() == "TP")
            {
                return string.Format(formatProvider, "{0:T}: {0:P}", arg);
            }
            if (format.ToUpper() == "T")
            {
                return GetTypeName(type);
            }
            if (format.ToUpper() != "P")
            {
                return TryHandleOtherFormats(format, arg);
            }

            var collection = arg as ICollection;
            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || collection != null
                || type == typeof(DateTime) || type == typeof(TimeSpan)
                )
            {
                return collection != null
                    ? string.Format(formatProvider, "{0}", string.Format(CollectionType(type), collection.Count))
                    : TryHandleOtherFormats(null, arg);
            }

            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var namesAndValues = propertyInfos
                .OrderBy(propertyInfo => propertyInfo.Name)
                .Where(propertyInfo => propertyInfo.GetIndexParameters().Length == 0) // Exclude indexer properties
                .Where(propertyInfo => propertyInfo.PropertyType != type) // Stop recursion i.e. classes with properties of the same type e.g. Composite Pattern or DateTime.Date
                .Select(propertyInfo => string.Format(formatProvider, "{0}{1}'{2:P}'", propertyInfo.Name, _equals, propertyInfo.GetValue(arg, null)))
                .ToArray();

            return namesAndValues.Length > 0
                ? string.Join(", ", namesAndValues)
                : TryHandleOtherFormats(null, arg);
        }

        private static string CollectionType(Type type)
        {
            return type.IsArray
                ? "[{0}]"
                : GetGenericTypes(type) + "[{0}]";
        }

        private static string GetTypeName(Type type)
        {
            var typeName = type.Name;
            if (type.IsGenericType)
            {
                return string.Format("{0}<{1}>", typeName.Remove(typeName.IndexOf('`')), GetGenericTypes(type));
            }
            return typeName;
        }

        private static string GetGenericTypes(Type type)
        {
            var argTypes = type.GetGenericArguments()
                .Select(GetTypeName)
                .ToArray();
            return string.Join(", ", argTypes);
        }

        private static string TryHandleOtherFormats(string format, object arg)
        {
            try
            {
                return HandleOtherFormats(format, arg);
            }
            catch (FormatException e)
            {
                throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
            }
        }

        private static string HandleOtherFormats(string format, object arg)
        {
            var formattable = arg as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
            return (arg != null) ? arg.ToString() : "null";
        }
    }
}
