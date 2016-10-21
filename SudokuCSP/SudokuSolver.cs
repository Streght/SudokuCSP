using System;
using System.IO;
using System.Threading.Tasks;

namespace SudokuCSP
{
    /// <summary>
    /// Console class used to solve a provided sudoku.
    /// </summary>
    public class SudokuSolver
    {
        /// <summary>
        /// Set the sudoku size.
        /// </summary>
        private int m_iSudokuSize = 9;
        /// <summary>
        /// Store the sudoku array.
        /// </summary>
        private int[,] m_aiSudokuGrid;

        /// <summary>
        /// Create a new SudokuSolver.
        /// </summary>
        private SudokuSolver()
        {
            m_aiSudokuGrid = new int[m_iSudokuSize, m_iSudokuSize];
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadCSV(string p_sPath)
        {
            StreamReader srReader = new StreamReader(File.OpenRead(p_sPath));
            int iIndexRead = 0;

            while (!(srReader.EndOfStream))
            {
                string sLine = srReader.ReadLine();
                string[] asValues = sLine.Split(',');

                Parallel.For(0, m_iSudokuSize, i =>
                {
                    m_aiSudokuGrid[iIndexRead, i] = Convert.ToInt32(asValues[i]);
                });

                iIndexRead++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintCSV()
        {
            for (int i = 0; i < m_iSudokuSize; i++)
            {
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    Console.Write(m_aiSudokuGrid[i, j].ToString() + " ");
                }
                Console.Write("\n");
            }
            //Console.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">  </param>
        public static void Main(string[] args)
        {
            SudokuSolver mpSudokuSolver = new SudokuSolver();
            mpSudokuSolver.ReadCSV(@"SudokuGrid\sudoku0.csv");
            mpSudokuSolver.PrintCSV();
        }

    }
}
