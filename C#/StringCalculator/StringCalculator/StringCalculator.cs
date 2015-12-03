using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace StringCalculator
{
    public class StringCalculator
    {
        /// <summary>
        /// Adds together numbers delimited by commas e.g. "0,1,2,3"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object Add(string input)
        {
            var delimiters = GetDelimiters(ref input);

            if (input != string.Empty)
            {
                string[] numbers = input.Replace(" ", "")
                                        .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);


                ValidateInput(numbers);
                return SumNumbers(numbers);
            }

            return 0;
        }

        #region Private Members

        private static int SumNumbers(string[] numbers)
        {
            return numbers.Select(number => Convert.ToInt32(number)).Where(numToAdd => numToAdd < 1000).Sum();
        }

        private void ValidateInput(string[] numbers)
        {
            var invalidNumbers = GetInvalidNumbers(numbers);
            if (invalidNumbers.Length > 0)
            {
                ThrowNumberInvalidException(invalidNumbers);
            }

            var negativeNumbers = GetNegativeNumbers(numbers);
            if (negativeNumbers.Length > 0)
            {
                ThrowNumbersLessThanZeroException(negativeNumbers);
            }
        }

        private string[] GetNegativeNumbers(string[] numbers)
        {
            return numbers.Where(n => n.Contains("-")).ToArray();
        }

        private string[] GetInvalidNumbers(string[] numbers)
        {
            int i;
            return numbers.Where(n => !int.TryParse(n, out i)).ToArray();
        }

        private void ThrowNumberInvalidException(string[] numbers)
        {
            throw new ArgumentException(string.Format("Arguments: {0} are not valid whole numbers", string.Join(", ", numbers)));
        }

        private void ThrowNumbersLessThanZeroException(string[] numbers)
        {
            throw new ArgumentOutOfRangeException(string.Format("Negatives are not allowed. Numbers: {0} are negative.", String.Join(", ", numbers)));
        }

        private string[] GetDelimiters(ref string input)
        {
            var delimiters = new List<string> ();
            string customDelimPattern =  @"^(\/\/).+(?:\n)";
            string multiDelimPattern = @"(\[.+?\])";

            Match m = Regex.Match(input, customDelimPattern);
            if (m.Success)
            {
                if (Regex.IsMatch(input, multiDelimPattern))
                {
                    foreach (Match match in Regex.Matches(input, multiDelimPattern))
                    {
                        delimiters.Add(match.Value.Remove(match.Length - 1, 1).Remove(0, 1));        
                    }
                }
                else
                {
                    delimiters = new List<string> { m.Value.Remove(m.Value.Length - 1, 1).Remove(0, 2) };
                }

                input = input.Remove(0, m.Length);
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
