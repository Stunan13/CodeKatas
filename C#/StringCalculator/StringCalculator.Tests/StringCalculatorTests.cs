using System;
using System.Security.Cryptography;
using NUnit.Framework;
using StringCalculator;

namespace StringCalculator.Tests
{
    [TestFixture]
    public class StringCalculatorTests
    {
        #region Helper Methods
        private static StringCalculator MakeStringCalculator()
        {
            return new StringCalculator();
        }
        #endregion

        [Test]
        public void Add_ReturnsZero_WithEmptyStringInput()
        {
            var calc = MakeStringCalculator();

            var expected = 0;
            var actual = calc.Add("");

            Assert.AreEqual(expected, actual);
        }

        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("2", 2)]
        public void Add_ReturnsParameterAsNumber_WithOneNumberInput(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("0, 0", 0)]
        [TestCase("2, 1, 1", 4)]
        [TestCase("2, 1, 1, 5", 9)]
        [TestCase("1, 2, 3, 4, 5", 15)]
        public void Add_ReturnsCorrectSum_OfAnyNumbersDelimitedByCommas(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }
        
        [TestCase("2\n 1", 3)]
        [TestCase("2\n 2\n 3", 7)]
        [TestCase("2\n 1\n 1\n 5", 9)]
        [TestCase("1\n 2\n 3\n 4\n 5", 15)]
        public void Add_ReturnsCorrectSum_OfAnyNumbersDelimitedByNewLine(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("0\n 0", 0)]
        [TestCase("0, 1", 1)]
        [TestCase("2\n 1, 1", 4)]
        [TestCase("2, 2\n 3", 7)]
        [TestCase("2\n 1, 1\n 5", 9)]
        [TestCase("1, 2\n 3, 4\n 5", 15)]
        public void Add_ReturnsCorrectSum_OfAnyNumbersDelimitedByNewLineOrCommas(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase(",.")]
        [TestCase("0, 1, 2, -")]
        [TestCase("1, 3, f")]
        [TestCase("4, !")]
        public void Add_ThrowsArgurmentException_WhenInputContainsNonIntegerValues(string input)
        {
            var calc = MakeStringCalculator();

            var ex = Assert.Throws<ArgumentException>(() => calc.Add(input));
            var expected = "is not a valid whole number";

            StringAssert.Contains(expected, ex.Message);
        }

        [TestCase(",\n2, 3", 5)]
        [TestCase("1,,, 2,", 3)]
        [TestCase("1, 3, \n\n", 4)]
        [TestCase("4, \n,,\n", 4)]
        public void Add_SkipsEmptyParams_WhenInputContainsThem(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
