using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SP6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a number for factorial calculation: ");
            int number = int.Parse(Console.ReadLine());

            long factorial = CalculateFactorial(number);
            Console.WriteLine($"Factorial of {number} is {factorial}");

            int digitCount = 0;
            int digitSum = 0;

            Parallel.Invoke(
                () => { digitCount = CountDigits(number); },
                () => { digitSum = SumDigits(number); }
            );

            Console.WriteLine($"Number of digits: {digitCount}");
            Console.WriteLine($"Sum of digits: {digitSum}");

            Console.Write("Enter the lower bound for multiplication table: ");
            int lowerBound = int.Parse(Console.ReadLine());
            Console.Write("Enter the upper bound for multiplication table: ");
            int upperBound = int.Parse(Console.ReadLine());

            GenerateMultiplicationTable(lowerBound, upperBound);

            List<int> numbersFromFile = new List<int>();
            try
            {
                numbersFromFile = new List<int>(File.ReadAllLines("numbers.txt").Select(int.Parse));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found. Please ensure 'numbers.txt' exists.");
                return; 
            }
            catch (FormatException)
            {
                Console.WriteLine("File contains invalid numbers. Please ensure the file has valid integers.");
                return; 
            }

            if (numbersFromFile.Count == 0)
            {
                Console.WriteLine("No numbers found in the file. Please add some numbers.");
                return; 
            }

            List<long> factorials = new List<long>(new long[numbersFromFile.Count]);

            Parallel.ForEach(numbersFromFile, (num, state, index) =>
            {
                factorials[(int)index] = CalculateFactorial(num);
            });

            Console.WriteLine("\nFactorials from file:");
            for (int i = 0; i < factorials.Count; i++)
            {
                Console.WriteLine($"Factorial of {numbersFromFile[i]} is {factorials[i]}");
            }

            var sum = numbersFromFile.AsParallel().Sum();
            var max = numbersFromFile.AsParallel().Max();
            var min = numbersFromFile.AsParallel().Min();

            Console.WriteLine($"\nSum: {sum}, Max: {max}, Min: {min}");
        }

        static long CalculateFactorial(int n)
        {
            if (n < 0) throw new ArgumentException("Factorial is not defined for negative numbers.");
            if (n == 0) return 1;

            long[] results = new long[n + 1];
            results[0] = 1; 

            Parallel.For(1, n + 1, i =>
            {
                results[i] = results[i - 1] * i; 
            });

            return results[n]; 
        }

        static int CountDigits(int n)
        {
            return n.ToString().Length;
        }

        static int SumDigits(int n)
        {
            int sum = 0;
            while (n > 0)
            {
                sum += n % 10;
                n /= 10;
            }
            return sum;
        }

        static void GenerateMultiplicationTable(int lower, int upper)
        {
            using (StreamWriter writer = new StreamWriter("MultiplicationTable.txt"))
            {
                Parallel.For(lower, upper + 1, i =>
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        writer.WriteLine($"{i} * {j} = {i * j}");
                    }
                });
            }

            Console.WriteLine($"Multiplication table saved to MultiplicationTable.txt");
        }
    }
}
