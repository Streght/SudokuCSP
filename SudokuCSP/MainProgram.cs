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
                Console.Write("Please enter the size of the Sudoku (9 for 9x9 and 16 for 16x16), the Sudoku must have a square shape.\n");
            RestartForWrongSize:;
                string sConsoleString = Console.ReadLine();
                // For quick tests purposes.
                //string sConsoleString = Convert.ToString(9);

                // Create a new Sudoku to store the imported Sudoku.
                Sudoku sSudoku = null;
                // Create a new sudiku to store the answer.
                Sudoku sSudokuSolved = null;

                if (((Math.Sqrt(Convert.ToInt32(sConsoleString)) - Math.Truncate(Math.Sqrt(Convert.ToInt32(sConsoleString)))) != 0) || 
                    ((Convert.ToInt32(sConsoleString) > 16) && (Math.Sqrt(Convert.ToInt32(sConsoleString)) < 9)))
                {
                    Console.Write("Wrong size of Sudoku, please enter the size of the Sudoku (9 for 9x9 and 16 for 16x16).\n");
                    goto RestartForWrongSize;
                }
                else
                {
                    sSudoku = new Sudoku(Convert.ToInt32(sConsoleString));
                    sSudokuSolved = new Sudoku(Convert.ToInt32(sConsoleString));
                }

                // Create a StopWatch to calculate how long it took to solve the Sudoku.
                Stopwatch swStopWatch = new Stopwatch();

                // Get the grid name from the console and 
                Console.Write("Please enter the name of the Sudoku to solve (without the CSV extension) and press enter.\n");
                sConsoleString = Console.ReadLine();
                sSudoku.ReadCSV(@"SudokuGrid\" + sConsoleString + ".csv");
                // For quick tests purposes.
                //sSudoku.ReadCSV(@"SudokuGrid\Sudoku8Hardest.csv");

                // Display the starting Sudoku on the console.
                Console.Write("\nStarting Sudoku grid :\n");
                sSudoku.PrintSudokuGrid();
                Console.Write("Start solving ? Press enter...\n");
                Console.ReadKey(true);
                sSudoku.NotAtStart = true;

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
