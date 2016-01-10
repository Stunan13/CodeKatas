using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StringCalculator.Interfaces;

namespace StringCalculator
{
    public class StringCalculator
    {

        private static ILogger _logger;
        private static IWebService _service;

        public StringCalculator()
        {
            
        }

        public StringCalculator(ILogger logger, IWebService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Adds together numbers delimited by commas/newlines/custom delimiters e.g. "0,1,2,3"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object Add(string input)
        {
            int sum = 0;
            var delimiters = GetDelimiters(ref input);

            if (input != string.Empty)
            {
                sum = GetNumbersFromInput(input, delimiters).Sum();
            }

            LogMessage(sum.ToString());

            return sum;
        }

        #region Private Members

        private static void LogMessage(string message)
        {
            try
            {
                _logger.Write(message);
            }
            catch (Exception ex)
            {
                _service.Write(ex.Message);
            }
        }

        private IEnumerable<int> GetNumbersFromInput(string input, string[] delimiters)
        {
            var numbers = new List<int>();
            var negativeNumbers = new List<int>();
            var invalidEntries = new List<string>();
            int valueToInt;

            foreach (string value in input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!int.TryParse(value, out valueToInt))
                {
                    invalidEntries.Add(value);
                }
                else if (valueToInt < 0)
                {
                    negativeNumbers.Add(valueToInt);
                }
                else if (valueToInt < 1000)
                {
                    numbers.Add(valueToInt);
                }
            }

            if (invalidEntries.Count > 0)
            {
                ThrowNumberInvalidException(invalidEntries.ToArray());
            }
            else if (negativeNumbers.Count > 0)
            {
                ThrowNumbersLessThanZeroException(negativeNumbers.ToArray());
            }

            return numbers.AsEnumerable();
        }
        
        private void ThrowNumberInvalidException(string[] numbers)
        {
            throw new ArgumentException(string.Format("Arguments: {0} are not valid whole numbers", String.Join(", ", numbers)));
        }

        private void ThrowNumbersLessThanZeroException(int[] numbers)
        {
            throw new ArgumentOutOfRangeException(string.Format("Negatives are not allowed. Numbers: {0} are negative.", String.Join(", ", numbers)));
        }

        /// <summary>
        /// Returns the delimiter to use for splitting the input string, this can be default { , and \n } or custom (single) //delimiter\n : (Multiple) //[delim1][delim2]\n
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string[] GetDelimiters(ref string input)
        {
            var delimiters = new List<string> ();
            string multipleCustomeDelimiterPattern = @"(\[.+?\])";
            
            bool hasCustomDelimiter = input.StartsWith("//");
            if (hasCustomDelimiter)
            {
                input = input.Remove(0, 2);

                bool hasMultipleCustomDelimiters = Regex.IsMatch(input, multipleCustomeDelimiterPattern);
                if (hasMultipleCustomDelimiters)
                {
                    foreach (Match match in Regex.Matches(input, multipleCustomeDelimiterPattern))
                    {
                        delimiters.Add(match.Value.Remove(match.Length - 1, 1).Remove(0, 1));
                    }
                }
                else
                {
                    delimiters.Add(input.Substring(0, input.IndexOf('\n')));
                }

                input = input.Remove(0, input.IndexOf('\n'));
            }
            else
            {
                delimiters = new List<string> { ",", "\n" };
            }

            return delimiters.ToArray();
        }

        #endregion
    }
}
