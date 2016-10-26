using System;
using System.Diagnostics;

namespace SudokuCSP
{
    /// <summary>
    /// Run the console interface to interact with the user.
    /// </summary>
    public class MainProgram
    {
        /// <summary>
        /// Run the main program.
        /// </summary>
        /// <param name="args">  </param>
        public static void Main(string[] args)
        {
            while (true)
            {
                // Create a new Sudoku to store the imported Sudoku.
                Sudoku sSudoku = new Sudoku();
                // Create a new sudiku to store the answer.
                Sudoku sSudokuSolved = new Sudoku();
                // Create a StopWatch to calculate how long it took to solve the Sudoku.
                Stopwatch swStopWatch = new Stopwatch();

                // Get the grid name from the console and 
                Console.Write("Please enter the name of the Sudoku to solve (without the CSV extension) and press enter.\n");
                string sSudokuName = Console.ReadLine();
                sSudoku.ReadCSV(@"SudokuGrid\" + sSudokuName + ".csv");
                // For quick tests purposes.
                //sSudoku.ReadCSV(@"SudokuGrid\SudokuHardest.csv");

                // Display the starting Sudoku on the console.
                Console.Write("\nStarting Sudoku grid :\n");
                sSudoku.PrintSudokuGrid();
                Console.Write("Start solving ? Press enter...\n");
                Console.ReadKey(true);

                // Start the StopWatch.
                swStopWatch.Start();
                // Solve the Sudoku using BacktrackingSearch and Forward Checking constraint propagation.
                sSudokuSolved = Solver.SolveSudoku(sSudoku);

                // Stop the StopWatch when the solution has been found.
                swStopWatch.Stop();
                // Display the solved Sudoku.

                // handle the case the Sudoku isn't having any answer.
                if (sSudokuSolved == null)
                {
                    Console.Write("\nImpossible to solve this Sudoku, please try with another one.");
                    // Display the time elapsed to solve the Sudoku.
                    long lTimeElapsed = swStopWatch.ElapsedMilliseconds;
                    Console.WriteLine("\nTime to find error : " + lTimeElapsed + " milliseconds.");
                    // Display the number of backtrack required to solve the Sudoku.
                    Console.WriteLine("Number of backtracks : " + Solver.BacktrackNumber.ToString() + ".");
                    Solver.BacktrackNumber = 0;
                    Console.WriteLine("\nPress enter to solve another Sudoku or close the console to exit.\n");
                    Console.ReadKey(true);
                }
                else
                {
                    Console.Write("\nSolved Sudoku grid :\n");
                    sSudokuSolved.PrintSudokuGrid();
                    // Display the time elapsed to solve the Sudoku.
                    long lTimeElapsed = swStopWatch.ElapsedMilliseconds;
                    Console.WriteLine("\nTime to solve Sudoku : " + lTimeElapsed + " milliseconds.");
                    // Display the number of backtrack required to solve the Sudoku.
                    Console.WriteLine("\nNumber of backtracks : " + Solver.BacktrackNumber.ToString() + ".");
                    Solver.BacktrackNumber = 0;
                    Console.WriteLine("\nPress enter to solve another Sudoku or close the console to exit.\n");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
