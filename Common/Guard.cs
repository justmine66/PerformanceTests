using System;

namespace PerformanceTests.Common
{
    public class Guard
    {
        public static void NotNull(dynamic argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName + " should not be null.");
        }

        public static void NotNullOrEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(argument, argumentName + " should not be null or empty.");
        }

        public static void Positive(int number, string argumentName)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be positive.");
        }
    }
}
