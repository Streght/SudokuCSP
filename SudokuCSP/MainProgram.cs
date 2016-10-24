using System;
using System.Diagnostics;

namespace SudokuCSP
{
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
                // Create a new sudoku to store the imported sudoku.
                Sudoku sSudoku = new Sudoku();
                // Create a new sudiku to store the answer.
                Sudoku sSudokuSolution = new Sudoku();
                // Create a StopWatch to calculate how long it took to solve the sudoku.
                Stopwatch swStopWatch = new Stopwatch();

                // Get the grid name from the console and 
                Console.Write("Please enter the name of the sudoku to solve (without the extension) and press enter.\n");
                //string sSudokuName = Console.ReadLine();
                //sSudoku.ReadCSV(@"SudokuGrid\" + sSudokuName + ".csv");
                // For quick tests purposes.
                sSudoku.ReadCSV(@"SudokuGrid\sudokuHardest.csv");

                // Display the starting sudoku on the console.
                Console.Write("\nStarting sudoku grid :\n");
                sSudoku.PrintSudokuGrid();
                sSudoku.InitSudoku();
                Console.Write("Start solving ? Press enter...\n");
                Console.ReadKey(true);

                // Start the StopWatch.
                swStopWatch.Start();
                // Solve the sudoku using BacktrackingSearch and Forward Checking constraint propagation.
                sSudokuSolution = Solver.BacktrackingSearch(sSudoku);

                // Stop the StopWatch when the solution has been found.
                swStopWatch.Stop();
                // Display the solved sudoku.

                // handle the case the sudoku isn't having any answer.
                if (sSudokuSolution == null)
                {
                    Console.Write("\nImpossible to solve this sudoku, please try with another one.");
                    // Display the time elapsed to solve the sudoku.
                    long lTimeElapsed = swStopWatch.ElapsedMilliseconds;
                    Console.WriteLine("\nTime to find error : " + lTimeElapsed + " milliseconds.");
                    // Display the number of backtrack required to solve the sudoku.
                    Console.WriteLine("Number of backtrack : " + Solver.BacktrackNumber.ToString() + ".");
                    Solver.BacktrackNumber = 0;
                    Console.WriteLine("\nPress enter to solve another sudoku.\n");
                    Console.ReadKey(true);
                }
                else
                {
                    Console.Write("\nSolved sudoku grid :\n");
                    sSudokuSolution.PrintSudokuGrid();
                    // Display the time elapsed to solve the sudoku.
                    long lTimeElapsed = swStopWatch.ElapsedMilliseconds;
                    Console.WriteLine("\nTime to solve Sudoku : " + lTimeElapsed + " milliseconds.");
                    // Display the number of backtrack required to solve the sudoku.
                    Console.WriteLine("\nNumber of backtrack : " + Solver.BacktrackNumber.ToString() + ".");
                    Solver.BacktrackNumber = 0;
                    Console.WriteLine("\nPress enter to solve another sudoku.\n");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
