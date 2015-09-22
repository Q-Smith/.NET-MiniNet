using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Utils
{
    public static class Ensure
    {
        public static void NotNull<T>(T argument, string argumentName) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void NotNullOrEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(argument, argumentName);
        }

        public static void GreaterThan(int target, int actual, string argumentName)
        {
            if (target >= actual)
                throw new ArgumentException(string.Format("{0} actual value: {1} is less than or equal to target value: {2}", argumentName, actual, target));
        }

        public static void GreaterThan(long target, long actual, string argumentName)
        {
            if (target >= actual)
                throw new ArgumentException(string.Format("{0} actual value: {1} is less than or equal to target value: {2}", argumentName, actual, target));
        }

        public static void Equal(int expected, int actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} expected value: {1}, actual value: {2}", argumentName, expected, actual));
        }

        public static void Equal(long expected, long actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} expected value: {1}, actual value: {2}", argumentName, expected, actual));
        }

        public static void Equal(bool expected, bool actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} expected value: {1}, actual value: {2}", argumentName, expected, actual));
        }
    }
}
