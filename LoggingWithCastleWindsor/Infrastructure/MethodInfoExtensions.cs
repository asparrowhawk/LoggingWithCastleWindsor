using System;
using System.Linq;
using System.Reflection;

namespace LoggingWithCastleWindsor.Infrastructure
{
    internal static class MethodInfoExtensions
    {
        public static string NameAndArguments(this MethodInfo method, object[] argumentValues)
        {
            if (method == null || method.DeclaringType == null)
            {
                return "";
            }

            var parameterTypesAndValues = method.GetParameters()
                                                .Zip(argumentValues, Tuple.Create);

            var argumentsAsString = string.Join(
                ", ", parameterTypesAndValues.Select(ArgumentAsString)
                );

            var declaringTypeAndName = method.DeclaringTypeAndName();

            return string.Format("{0}({1})", declaringTypeAndName, argumentsAsString);
        }

        public static string DeclaringTypeAndName(this MethodInfo method)
        {
            if (method == null || method.DeclaringType == null)
            {
                return "";
            }

            return string.Format("{0}.{1}", method.DeclaringType.Name, method.Name);
        }

        static string ArgumentAsString(Tuple<ParameterInfo, object> value)
        {
            return string.Format(new TypeAndPropertiesFormatter(), "{0:TP}", value.Item2);
        }
    }
}
