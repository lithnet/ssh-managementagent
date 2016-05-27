// -----------------------------------------------------------------------
// <copyright file="ComparisonEngine.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;

    /// <summary>
    /// Provides methods for comparing values
    /// </summary>
    public class ComparisonEngine
    {
        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a string value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static ValueOperator[] GetAllowedStringValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.None,
                ValueOperator.Equals,
                ValueOperator.NotEquals,
                ValueOperator.IsPresent,
                ValueOperator.NotPresent,
                ValueOperator.Contains,
                ValueOperator.NotContains,
                ValueOperator.StartsWith,
                ValueOperator.EndsWith
            };
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a version value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static ValueOperator[] GetAllowedVersionValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.None,
                ValueOperator.Equals,
                ValueOperator.NotEquals,
                ValueOperator.GreaterThan,
                ValueOperator.LessThan,
                ValueOperator.GreaterThanOrEq,
                ValueOperator.LessThanOrEq,
                ValueOperator.IsPresent,
                ValueOperator.NotPresent
            };
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a numeric value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static ValueOperator[] GetAllowedNumericValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.None,
                ValueOperator.Equals,
                ValueOperator.NotEquals,
                ValueOperator.GreaterThan,
                ValueOperator.LessThan,
                ValueOperator.GreaterThanOrEq,
                ValueOperator.LessThanOrEq
            };
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with a binary value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static ValueOperator[] GetAllowedBinaryValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.None,
                ValueOperator.Equals,
                ValueOperator.NotEquals,
                ValueOperator.IsPresent,
                ValueOperator.NotPresent
            };
        }

        /// <summary>
        /// Gets the ValueOperators that are allowed to be used with an object value
        /// </summary>
        /// <returns>An array of ValueOperator values</returns>
        public static ValueOperator[] GetAllowedObjectValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.None,
                ValueOperator.IsPresent,
                ValueOperator.NotPresent
            };
        }

        /// <summary>
        /// Gets the ValueOperator that are allowed to be used with a state value
        /// </summary>
        /// <returns>An array of allowed ValueOperator values</returns>
        public static ValueOperator[] GetAllowedStateValueOperators()
        {
            return new ValueOperator[]
            {
                ValueOperator.Equals,
                ValueOperator.NotEquals
            };
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a state value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedStateValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedStateValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a string value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedStringValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedStringValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a string value
        /// </summary>
        /// <param name="valueOperator">The string representation of the ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedStringValueOperator(string valueOperator)
        {
            return IsAllowedStringValueOperator((ValueOperator)Enum.Parse(typeof(ValueOperator), valueOperator, true));
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a version value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedVersionValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedVersionValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a version value
        /// </summary>
        /// <param name="valueOperator">The string representation of the ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedVersionValueOperator(string valueOperator)
        {
            return IsAllowedVersionValueOperator((ValueOperator)Enum.Parse(typeof(ValueOperator), valueOperator, true));
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a numeric value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedNumericValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedNumericValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a numeric value
        /// </summary>
        /// <param name="valueOperator">The string representation of the ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedNumericValueOperator(string valueOperator)
        {
            return IsAllowedNumericValueOperator((ValueOperator)Enum.Parse(typeof(ValueOperator), valueOperator, true));
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a binary value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedBinaryValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedBinaryValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare a binary value
        /// </summary>
        /// <param name="valueOperator">The string representation of the ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedBinaryValueOperator(string valueOperator)
        {
            return IsAllowedBinaryValueOperator((ValueOperator)Enum.Parse(typeof(ValueOperator), valueOperator, true));
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare an object value
        /// </summary>
        /// <param name="valueOperator">The ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedObjectValueOperator(ValueOperator valueOperator)
        {
            foreach (ValueOperator allowedOperator in GetAllowedObjectValueOperators())
            {
                if (allowedOperator == valueOperator)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified ValueOperator is allowed to be used to compare an object value
        /// </summary>
        /// <param name="valueOperator">The string representation of the ValueOperator to check</param>
        /// <returns>A boolean value indicating whether the specified value operator is allowed</returns>
        public static bool IsAllowedObjectValueOperator(string valueOperator)
        {
            return IsAllowedObjectValueOperator((ValueOperator)Enum.Parse(typeof(ValueOperator), valueOperator, true));
        }

        /// <summary>
        /// Compares a string using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareString(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (string.Equals(actualValue, expectedValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (!string.Equals(actualValue, expectedValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    throw new NotSupportedException();

                case ValueOperator.LessThan:
                    throw new NotSupportedException();

                case ValueOperator.GreaterThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.LessThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.IsPresent:
                    return !string.IsNullOrWhiteSpace(actualValue);

                case ValueOperator.NotPresent:
                    return string.IsNullOrWhiteSpace(actualValue);

                case ValueOperator.Contains:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.ContainsIgnoreCase(expectedValue))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotContains:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (!actualValue.ContainsIgnoreCase(expectedValue))
                    {
                        return true;
                    }

                    break;
                case ValueOperator.StartsWith:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.StartsWith(expectedValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.EndsWith:
                    if (actualValue == null)
                    {
                        break;
                    }

                    if (actualValue.EndsWith(expectedValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }

            return false;
        }

        /// <summary>
        /// Compares a binary value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="base64ExpectedValue">The expected value specified in the query logic, in base64 format</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(byte[] actualValue, string base64ExpectedValue, ValueOperator valueOperator)
        {
            byte[] expectedValue = Convert.FromBase64String(base64ExpectedValue);
            return CompareBinary(actualValue, expectedValue, valueOperator);
        }

        /// <summary>
        /// Compares a binary value using the specified ValueOperator
        /// </summary>
        /// <param name="base64ActualValue">The value obtained from the query</param>
        /// <param name="base64ExpectedValue">The expected value specified in the query logic, in base64 format</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(string base64ActualValue, string base64ExpectedValue, ValueOperator valueOperator)
        {
            byte[] expectedValue = Convert.FromBase64String(base64ExpectedValue);
            byte[] actualValue = Convert.FromBase64String(base64ActualValue);

            return CompareBinary(actualValue, expectedValue, valueOperator);
        }
        
        /// <summary>
        /// Compares a binary using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBinary(byte[] actualValue, byte[] expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    throw new NotSupportedException();

                case ValueOperator.LessThan:
                    throw new NotSupportedException();

                case ValueOperator.GreaterThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.LessThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.IsPresent:
                    if ((actualValue != null) && (actualValue.Length > 0))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotPresent:
                    if ((actualValue == null) || (actualValue.Length == 0))
                    {
                        return true;
                    }

                    break;

                case ValueOperator.Contains:
                    throw new NotSupportedException();

                case ValueOperator.StartsWith:
                    throw new NotSupportedException();

                case ValueOperator.EndsWith:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }

            return false;
        }

        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            long expectedValueConverted = Convert.ToInt64(expectedValue);
            long actualValueConverted = Convert.ToInt64(actualValue);
            return CompareLong(actualValueConverted, expectedValueConverted, valueOperator);
        }
        
        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(long actualValue, string expectedValue, ValueOperator valueOperator)
        {
            long expectedValueConverted = Convert.ToInt64(expectedValue);
            return CompareLong(actualValue, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a numeric value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareLong(long actualValue, long expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    if (actualValue > expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThan:
                    if (actualValue < expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThanOrEq:
                    if (actualValue >= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.LessThanOrEq:
                    if (actualValue <= expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.IsPresent:
                    throw new NotSupportedException();

                case ValueOperator.NotPresent:
                    throw new NotSupportedException();

                case ValueOperator.Contains:
                    throw new NotSupportedException();

                case ValueOperator.StartsWith:
                    throw new NotSupportedException();

                case ValueOperator.EndsWith:
                    throw new NotSupportedException();

                case ValueOperator.Or:
                    if ((actualValue | expectedValue) == actualValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.And:
                    if ((actualValue & expectedValue) == expectedValue)
                    {
                        return true;
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }

            return false;
        }

        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(string actualValue, string expectedValue, ValueOperator valueOperator)
        {
            bool expectedValueConverted = Convert.ToBoolean(expectedValue);
            bool actualValueConverted = Convert.ToBoolean(actualValue);
            return CompareBoolean(actualValueConverted, expectedValueConverted, valueOperator);
        }

        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(bool actualValue, string expectedValue, ValueOperator valueOperator)
        {
            bool expectedValueConverted = Convert.ToBoolean(expectedValue);
            return CompareBoolean(actualValue, expectedValueConverted, valueOperator);
        }
      
        /// <summary>
        /// Compares a boolean value using the specified ValueOperator
        /// </summary>
        /// <param name="actualValue">The value obtained from the query</param>
        /// <param name="expectedValue">The expected value specified in the query logic</param>
        /// <param name="valueOperator">The value operator to use in the comparison</param>
        /// <returns>A boolean value indicating if the actual and expected value matched when using the specified ValueOperator</returns>
        public static bool CompareBoolean(bool actualValue, bool expectedValue, ValueOperator valueOperator)
        {
            switch (valueOperator)
            {
                case ValueOperator.None:
                    return true;

                case ValueOperator.Equals:
                    if (actualValue == expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.NotEquals:
                    if (actualValue != expectedValue)
                    {
                        return true;
                    }

                    break;

                case ValueOperator.GreaterThan:
                    throw new NotSupportedException();

                case ValueOperator.LessThan:
                    throw new NotSupportedException();

                case ValueOperator.GreaterThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.LessThanOrEq:
                    throw new NotSupportedException();

                case ValueOperator.IsPresent:
                    throw new NotSupportedException();

                case ValueOperator.NotPresent:
                    throw new NotSupportedException();

                case ValueOperator.Contains:
                    throw new NotSupportedException();

                case ValueOperator.StartsWith:
                    throw new NotSupportedException();

                case ValueOperator.EndsWith:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }

            return false;
        }

        /*
         
        private static bool CompareValues(MASchemaAttribute reference, object value1, object value2)
        {
            if (reference.IsMultiValued)
            {
                return CompareMultiValuedObject(reference, value1, value2);
            }
            else
            {
                return CompareSingleValuedObject(reference, value1, value2);
            }
        }

        private static bool CompareSingleValuedObject(MASchemaAttribute reference, object value1, object value2)
        {
            switch (reference.Type)
            {
                case AttributeType.Binary:
                    return ((byte[])value1) == ((byte[])value2);

                case AttributeType.Boolean:
                    return ((bool)value1) == ((bool)value2);

                case AttributeType.Integer:
                    return ((long)value1) == ((long)value2);

                case AttributeType.String:
                case AttributeType.Reference:
                    return ((string)value1).Equals((string)value2);

                default:
                    throw new ArgumentException("The reference type is unknown");
            }
        }

        private static bool CompareMultiValuedObject(MASchemaAttribute reference, object value1, object value2)
        {
            switch (reference.Type)
            {
                case AttributeType.Binary:
                    return CompareMultiValuedAttributeBinary((byte[][])value1, (byte[][])value2);

                case AttributeType.Integer:
                    return CompareMultiValuedAttributeInt((long[])value1, (long[])value2);

                case AttributeType.Reference:
                case AttributeType.String:
                    return CompareMultiValuedAttributeString((string[])value1, (string[])value2);

                case AttributeType.Boolean:
                case AttributeType.Undefined:
                default:
                    throw new ArgumentException("The specified reference type cannot support multi-valued objects");
            }
        }

        private static bool CompareMultiValuedAttributeBinary(byte[][] value1, byte[][] value2)
        {
            if (value1.Length != value2.Length)
            {
                return false;
            }

            for (int x = 0; x < value1.Length; x++)

                if (value1[x] != value2[x])
                {
                    return false;
                }

            return true;
        }

        private static bool CompareMultiValuedAttributeString(string[] value1, string[] value2)
        {
            if (value1.Length != value2.Length)
            {
                return false;
            }

            for (int x = 0; x < value1.Length; x++)

                if (value1[x] != value2[x])
                {
                    return false;
                }

            return true;
        }

        private static bool CompareMultiValuedAttributeInt(long[] value1, long[] value2)
        {
            if (value1.Length != value2.Length)
            {
                return false;
            }

            for (int x = 0; x < value1.Length; x++)

                if (value1[x] != value2[x])
                {
                    return false;
                }

            return true;
        }

         */
    }
}
