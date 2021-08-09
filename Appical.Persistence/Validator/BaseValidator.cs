using System;
using System.Collections.Generic;

namespace Appical.Persistence.Validator
{
    public static class BaseValidator
    {
        public static List<string> ValidateGuid(Guid guid)
        {
            if (guid == null || guid == new Guid()) return new List<string> { "Guid is empty or default value" };
            return new List<string>();
        }

        public static List<string> ValidateStringLength(string text, int maxLength, string propName)
        {
            if (text.Length >= maxLength) return new List<string> { $"{propName} is too long" };
            return new List<string>();
        }

        public static List<string> ValidateDecimalNotNegative(decimal value, string propName)
        {
            if (value < 0) return new List<string> { $"{propName} cannot be negative" };
            return new List<string>();
        }

        public static List<string> ValidateDateNotFuture(DateTime value, string propName)
        {
            if (value > DateTime.Now) return new List<string> { $"{propName} cannot be in the future" };
            return new List<string>();
        }
    }
}
