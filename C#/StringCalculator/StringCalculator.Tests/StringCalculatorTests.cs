using System;
using System.Net;
using System.Security.Cryptography;
using NUnit.Framework;
using NSubstitute;
using StringCalculator;
using StringCalculator.Interfaces;

namespace StringCalculator.Tests
{
    [TestFixture]
    public class StringCalculatorTests
    {
        #region Helper Methods

        private static StringCalculator MakeStringCalculator()
        {
            return new StringCalculator(MakeFakeLogger(), MakeFakeWebService());
        }

        private static ILogger MakeFakeLogger()
        {
            return Substitute.For<ILogger>();
        }

        private static IWebService MakeFakeWebService()
        {
            return Substitute.For<IWebService>();
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
        [TestCase("0, 1, 2, #")]
        [TestCase("1, 3, f")]
        [TestCase("4, !")]
        public void Add_ThrowsArgurmentException_WhenInputContainsNonIntegerValues(string input)
        {
            var calc = MakeStringCalculator();

            var ex = Assert.Throws<ArgumentException>(() => calc.Add(input));
            var expected = "are not valid whole numbers";

            StringAssert.Contains(expected, ex.Message);
        }
        
        [TestCase("0, 1, 2, #", "#")]
        [TestCase("1, 3, f", "f")]
        [TestCase("4, !", "!")]
        public void Add_ArgurmentExceptionMessage_ContainInvalidValuesFromInputString(string input, string expected)
        {
            var calc = MakeStringCalculator();

            var ex = Assert.Throws<ArgumentException>(() => calc.Add(input));

            StringAssert.Contains(expected, ex.Message);
        }

        [TestCase("//,\n1,2,3", 6)]
        [TestCase("//;\n1;2;3", 6)]
        [TestCase("//+\n1+2+3", 6)]
        public void Add_ReturnsCorrectSum_WhenSingleCustomDelimiterIsPassedIn(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("1, -1")]
        [TestCase("0, -1, 2, -2")]
        public void Add_ThrowsArgurmentOutOfRangeException_WhenInputContainsNegativeIntegerValues(string input)
        {
            var calc = MakeStringCalculator();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => calc.Add(input));
            var expected = "Negatives are not allowed.";

            StringAssert.Contains(expected, ex.Message);
        }

        [TestCase("1, -1", "-1")]
        [TestCase("0, -1, 2, -2", "-1, -2")]
        public void Add_ArgurmentOutOfRangeExceptionMessage_ContainsNegativeIntegerValuesFromInputString(string input, string expected)
        {
            var calc = MakeStringCalculator();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => calc.Add(input));

            StringAssert.Contains(expected, ex.Message);
        }

        [TestCase("0, 1000", 0)]
        [TestCase("2, 1, 1001", 3)]
        [TestCase("2000, 1, 1000, 5", 6)]
        [TestCase("1, 2, 3, 4000, 5000", 6)]
        public void Add_Excludes_NumbersOverOneThousandFromSum(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("//[,][***]\n1***2,3", 6)]
        [TestCase("//[;][!]\n1!2;3", 6)]
        [TestCase("//[+][*.]\n1+2*.3", 6)]
        public void Add_ReturnsCorrectSum_WhenCustomMultipleDelimitersArePassedIn(string input, int expected)
        {
            var calc = MakeStringCalculator();

            var actual = calc.Add(input);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("0, 1, 2, 3", "6")]
        [TestCase("1, 4, 5", "10")]
        [TestCase("//[**]\n2**3**4", "9")]
        public void Add_CallsLoggerWrite_WithCorrectSumValue(string input, string expected)
        {
            var mockLogger = MakeFakeLogger();
            var stubWebService = MakeFakeWebService();
            var calc = new StringCalculator(mockLogger, stubWebService);

            calc.Add(input);

            mockLogger.Received().Write(expected);
        }

        [Test]
        public void Add_CallsWebServiceWrite_WhenLoggerThrowsException()
        {
            string input = "1";
            var mockWebService = MakeFakeWebService();
            var stubLogger = MakeFakeLogger();
            var calc = new StringCalculator(stubLogger, mockWebService);

            stubLogger.When(l => l.Write(input)).Throw(new Exception("Fake Exception"));
            calc.Add(input);
            
            mockWebService.Received().Write("Fake Exception");
        }
    }
}
