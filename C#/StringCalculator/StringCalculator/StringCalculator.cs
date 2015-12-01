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
            var delimiters = GetDelimiters(input);

            if (input != string.Empty)
            {
                string[] numbers = input.Replace(" ", "")
                                        .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (var number in numbers)
                {
                    int numToAdd = 0;
                    if (int.TryParse(number, out numToAdd))
                    {
                        sum += int.Parse(number);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Argument: {0} is not a valid whole number", number));
                    }
                }
            }

            return sum;
        }

        private char[] GetDelimiters(string input)
        {
            char[] delimiters = new char[] { ',', '\n' };
            var regex = new Regex(@"/^(\/\/).+(?:\\n)/g");

            if (regex.IsMatch(input))
            {
                Match m = regex.Match(input);
                //delimiters = new char[] { m.Value.Remove(0, 2).Replace("\n", "").ToCharArray() };
            }

            return delimiters;
        }
    }
}
