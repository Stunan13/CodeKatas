using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            int sum = 0;
            var delimiters = GetDelimiters(ref input);

            if (input != string.Empty)
            {
                string[] numbers = input.Replace(" ", "")
                                        .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (ContainsNegativeNumbers(numbers))
                {
                    ThrowNumbersLessThanZeroException(numbers);
                }

                sum = SumNumbers(numbers);
            }

            return sum;
        }

        private int SumNumbers(string[] numbers)
        {
            int sum = 0;
            foreach (var number in numbers)
            {
                int numToAdd;
                if (int.TryParse(number, out numToAdd))
                {
                    sum += numToAdd;
                }
                else
                {
                    ThrowNumberInvalidException(number);
                }
            }
            return sum;
        }

        private bool ContainsNegativeNumbers(string[] numbers)
        {
            return numbers.Any(n => n.Contains("-"));
        }

        private void ThrowNumberInvalidException(string number)
        {
            throw new ArgumentException(string.Format("Argument: {0} is not a valid whole number", number));
        }

        private void ThrowNumbersLessThanZeroException(string[] numbers)
        {
            string[] numbersLessThanZero = numbers.Where(n => n.Contains("-")).ToArray();
            
            throw new ArgumentOutOfRangeException(string.Format("Numbers must be greater than zero. Numbers: {0} are less than zero.", String.Join(", ", numbersLessThanZero)));
        }

        private char[] GetDelimiters(ref string input)
        {
            char[] delimiters = new char[] { ',', '\n' };

            var regex = new Regex(@"^(\/\/).+(?:\n)");
            
            Match m = regex.Match(input);
            if (m.Success)
            {
                delimiters = m.Value.Remove(m.Value.Length - 1, 1)
                                    .Remove(0, 2)
                                    .ToCharArray();

                input = input.Remove(0, m.Length);
            }

            return delimiters;
        }
    }
}
