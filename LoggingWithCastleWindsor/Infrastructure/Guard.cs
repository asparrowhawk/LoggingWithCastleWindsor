using System;
using System.Linq;
using JetBrains.Annotations;

namespace LoggingWithCastleWindsor.Infrastructure
{
    /// <summary>
    /// Utility class to support procedure/function argument validation.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks the <c>condition</c> and throws a <see cref="System.ArgumentException"/> if it is <c>false</c>
        /// </summary>
        /// <param name="condition">The condition to throw on.</param>
        /// <param name="argumentName">The argument value to check.</param>
        /// <param name="message">The reason for the exception.</param>
        public static void ArgumentConditionIsTrue(
            bool condition, [InvokerParameterName] string argumentName, [NotNull] string message
            )
        {
            if (!condition)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Checks the <c>condition</c> and throws a <see cref="System.ArgumentException"/> if it is <c>true</c>
        /// </summary>
        /// <param name="condition">The condition to throw on.</param>
        /// <param name="argumentName">The argument value to check.</param>
        /// <param name="message">The reason for the exception.</param>
        public static void ArgumentConditionIsFalse(
            bool condition, [InvokerParameterName] string argumentName, [NotNull] string message
            )
        {
            if (condition)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyString(
            [CanBeNull] string argumentValue, [InvokerParameterName] string argumentName
            )
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(string.Format("The provided String argument {0} must not be empty.", argumentName));
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null, empty or white space
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyStringOrWhiteSpace(
            [CanBeNull] string argumentValue, [InvokerParameterName] string argumentName
            )
        {
            ArgumentNotNullOrEmptyString(argumentValue, argumentName);

            if (String.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException(
                    string.Format("The provided String argument {0} must not consist only of white space.", argumentName)
                    );
            }
        }

        /// <summary>
        /// Checks a reference argument type to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull<ArgumentType>(
            [CanBeNull, NoEnumeration] ArgumentType argumentValue,
            [InvokerParameterName] string argumentName
            )
            where ArgumentType : class
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Checks an nullable argument type to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull<ArgumentType>(
            [CanBeNull] ArgumentType? argumentValue,
            [InvokerParameterName] string argumentName
            )
            where ArgumentType : struct
        {
            if (!argumentValue.HasValue)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Checks one or more arguments to ensure they are not null
        /// </summary>
        /// <param name="arg">The first argument value and associated name to check.</param>
        /// <param name="args">Subsequent argument values and names.</param>
        public static void ArgumentNotNull(
            [NotNull] Tuple<object, string> arg, params Tuple<object, string>[] args
            )
        {
            var guard = new[]
                        {
                            arg
                        }
                        .Concat(args)
                        .FirstOrDefault(value => value.Item1 == null);
            if (guard != null)
            {
                throw new ArgumentNullException(guard.Item2);
            }
        }

        /// <summary>
        /// Checks an Enum argument to ensure that its value is defined by the specified Enum type.
        /// </summary>
        /// <param name="enumType">The Enum type the value should correspond to.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        public static void EnumValueIsDefined(
            [NotNull] Type enumType, [NotNull] object value, [InvokerParameterName] string argumentName)
        {
            if (Enum.IsDefined(enumType, value) == false)
            {
                throw new ArgumentException(
                    string.Format("The value of the argument {0} provided for the enumeration {1} is invalid.", argumentName, enumType)
                    );
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignee">The argument type.</param>
        /// <param name="providedType">The type it must be assignable from.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void TypeIsAssignableFromType(
            [NotNull] Type assignee, [NotNull] Type providedType, [InvokerParameterName] string argumentName
            )
        {
            if (!providedType.IsAssignableFrom(assignee))
            {
                throw new ArgumentException(
                    string.Format("The provided type {0} is not compatible with {1}.", assignee, providedType),
                    argumentName
                    );
            }
        }
    }
}
