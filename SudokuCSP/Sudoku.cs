using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuCSP
{
    /// <summary>
    /// Console class used to solve a provided sudoku.
    /// </summary>
    public class Sudoku
    {
        /// <summary>
        /// Set the sudoku size.
        /// </summary>
        private int m_iSudokuSize = 9;
        /// <summary>
        /// Store the sudoku array.
        /// </summary>
        private Cell[,] m_aiSudokuGrid;
        /// <summary>
        /// The remaining unknown values.
        /// </summary>
        private int m_iRemainingZeros;

        /// <summary>
        /// Get the sudoku size.
        /// </summary>
        public int SudokuSize
        {
            get
            {
                return m_iSudokuSize;
            }
        }

        /// <summary>
        /// Get the sudoku grid.
        /// </summary>
        public Cell[,] SudokuGrid
        {
            get
            {
                return m_aiSudokuGrid;
            }
        }

        /// <summary>
        /// Get / set the number of missing numbers.
        /// </summary>
        public int RemainingZeros
        {
            get
            {
                return m_iRemainingZeros;
            }
            set
            {
                m_iRemainingZeros = value;
            }
        }

        /// <summary>
        /// Create a new SudokuSolver.
        /// </summary>
        private Sudoku()
        {
            m_aiSudokuGrid = new Cell[m_iSudokuSize, m_iSudokuSize];

            for (int i = 0; i < m_iSudokuSize; i++)
            {
                Parallel.For(0, m_iSudokuSize, j =>
                {
                    m_aiSudokuGrid[i, j] = new Cell();
                });
            }
        }

        /// <summary>
        /// Read the CSV file provided.
        /// </summary>
        private void ReadCSV(string p_sPath)
        {
            try
            {
                // Read the CSV file and store it as a Sudoku Grid.
                using (StreamReader srReader = new StreamReader(File.OpenRead(p_sPath)))
                {
                    int iIndexRead = 0;

                    while (!(srReader.EndOfStream))
                    {
                        string sLine = srReader.ReadLine();
                        string[] asValues = sLine.Split(',');

                        Parallel.For(0, m_iSudokuSize, i =>
                        {
                            m_aiSudokuGrid[iIndexRead, i].CellValue = Convert.ToInt32(asValues[i]);
                            if (Convert.ToInt32(asValues[i]) == 0)
                            {
                                m_iRemainingZeros++;
                            }
                        });

                        iIndexRead++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The CSV file could not be read:");
                Console.WriteLine(e.Message + "\n");
            }
        }

        /// <summary>
        /// Print the stored sudoku grid in the console.
        /// </summary>
        private void PrintSudokuGrid()
        {
            if (m_aiSudokuGrid != null)
            {
                Console.Write("Grille de Sudoku stockée :\n");
                for (int i = 0; i < m_iSudokuSize; i++)
                {
                    if (i % Math.Sqrt(m_iSudokuSize) == 0)
                    {
                        Console.Write(" +-------+-------+-------+\n");
                    }
                    for (int j = 0; j < m_iSudokuSize; j++)
                    {
                        if (j % Math.Sqrt(m_iSudokuSize) == 0)
                        {
                            Console.Write(" |");
                        }
                        if (m_aiSudokuGrid[i, j].CellValue == 0)
                        {
                            Console.Write(" .");
                        }
                        else
                        {
                            Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                        }
                    }
                    Console.Write(" |\n");
                }
                Console.Write(" +-------+-------+-------+\n");
            }
        }

        /// <summary>
        /// Return the possible values for a given cell as a list.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <returns></returns>
        private List<int> ComputePossibleValues(int p_iRow, int p_iColumn)
        {
            List<int> liConstraintValues = new List<int>();
            for (int i = 0; i < m_aiSudokuGrid[p_iRow, p_iColumn].Peers.Count; i++)
            {
                liConstraintValues.Add(m_aiSudokuGrid[p_iRow, p_iColumn].Peers[i].CellValue);
            }

            List<int> liPossibleValues = new List<int>();
            //for (int i = 0; i < m_iSudokuSize; i++)
            Parallel.For(0, m_iSudokuSize, i =>
            {
                if (!(liConstraintValues.Contains(i + 1)))
                {
                    liPossibleValues.Add(i + 1);
                }
            });

            return liPossibleValues;
        }

        /// <summary>
        /// Return the row and column peers for a given cell.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <returns> The row and column peers as a list. </returns>
        private List<Cell> ComputeRowAndColumnPeers(int p_iRow, int p_iColumn)
        {
            List<Cell> lcRowPeers = new List<Cell>();
            List<Cell> lcColumnPeers = new List<Cell>();

            for (int i = 0; i < m_iSudokuSize; i++)
            {
                if (i != p_iColumn)
                {
                    lcRowPeers.Add(m_aiSudokuGrid[p_iRow, i]);
                }
                if (i != p_iRow)
                {
                    lcColumnPeers.Add(m_aiSudokuGrid[i, p_iColumn]);
                }
            }

            lcRowPeers.AddRange(lcColumnPeers);
            return lcRowPeers;
        }

        /// <summary>
        /// Return the region peers for a given cell.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <returns> The region peers as a list. </returns>
        private List<Cell> ComputeRegionPeers(int p_iRow, int p_iColumn)
        {
            List<Cell> lcResult = new List<Cell>();
            int iRowPosition = p_iRow % 3;
            int iColumnPosition = p_iColumn % 3;

            switch (iRowPosition)
            {
                case 0:

                    switch (iColumnPosition)
                    {
                        case 0:
                            List<Cell> lcPeers00 = new List<Cell> { m_aiSudokuGrid[p_iRow + 1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn + 2],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn + 2] };
                            lcResult.AddRange(lcPeers00);
                            break;
                        case 1:
                            List<Cell> lcPeers01 = new List<Cell> { m_aiSudokuGrid[p_iRow + 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn + 1] };
                            lcResult.AddRange(lcPeers01);
                            break;
                        case 2:
                            List<Cell> lcPeers02 = new List<Cell> { m_aiSudokuGrid[p_iRow + 1, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow + 2, p_iColumn - 1] };
                            lcResult.AddRange(lcPeers02);
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    switch (iColumnPosition)
                    {
                        case 0:
                            List<Cell> lcPeers10 = new List<Cell> { m_aiSudokuGrid[p_iRow -1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn + 2],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn + 2] };
                            lcResult.AddRange(lcPeers10);
                            break;
                        case 1:
                            List<Cell> lcPeers11 = new List<Cell> { m_aiSudokuGrid[p_iRow - 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn + 1] };
                            lcResult.AddRange(lcPeers11);
                            break;
                        case 2:
                            List<Cell> lcPeers12 = new List<Cell> { m_aiSudokuGrid[p_iRow - 1, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow + 1, p_iColumn - 1] };
                            lcResult.AddRange(lcPeers12);
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    switch (iColumnPosition)
                    {
                        case 0:
                            List<Cell> lcPeers20 = new List<Cell> { m_aiSudokuGrid[p_iRow - 2, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow - 2, p_iColumn + 2],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn + 2] };
                            lcResult.AddRange(lcPeers20);
                            break;
                        case 1:
                            List<Cell> lcPeers21 = new List<Cell> { m_aiSudokuGrid[p_iRow - 2, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow - 2, p_iColumn + 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn + 1] };
                            lcResult.AddRange(lcPeers21);
                            break;
                        case 2:
                            List<Cell> lcPeers22 = new List<Cell> { m_aiSudokuGrid[p_iRow - 2, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow - 2, p_iColumn - 1],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn - 2],
                                m_aiSudokuGrid[p_iRow - 1, p_iColumn - 1] };
                            lcResult.AddRange(lcPeers22);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return lcResult;
        }

        /// <summary>
        /// Initialise the sudoku by finding the peers and possible values for each cell.
        /// </summary>
        private void InitSudoku()
        {
            for (int i = 0; i < m_iSudokuSize; i++)
            {
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    List<Cell> lcPeers = new List<Cell>();
                    lcPeers.AddRange(ComputeRowAndColumnPeers(i, j));
                    lcPeers.AddRange(ComputeRegionPeers(i, j));
                    m_aiSudokuGrid[i, j].Peers.AddRange(lcPeers);

                    if (m_aiSudokuGrid[i, j].CellValue == 0)
                    {
                        List<int> lcPossibleValues = new List<int>();
                        lcPossibleValues.AddRange(ComputePossibleValues(i, j));
                        m_aiSudokuGrid[i, j].PossibleValues.AddRange(lcPossibleValues);
                    }
                }
            }
        }

        /// <summary>
        /// Remove a value from the peers of a given cell.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <param name="p_iValue"> The value to be removed from the peers' possible values. </param>
        public void RemoveValueFromPeers(int p_iRow, int p_iColumn, int p_iValue)
        {
            for (int i = 0; i < m_aiSudokuGrid[p_iRow, p_iColumn].Peers.Count; i++)
            {
                if (m_aiSudokuGrid[p_iRow, p_iColumn].Peers[i].PossibleValues.Contains(p_iValue))
                {
                    m_aiSudokuGrid[p_iRow, p_iColumn].Peers[i].PossibleValues.Remove(p_iValue);
                }
            }
        }

        /// <summary>
        /// Run the main program.
        /// </summary>
        /// <param name="args">  </param>
        public static void Main(string[] args)
        {
            Sudoku mpSudokuSolver = new Sudoku();
            mpSudokuSolver.ReadCSV(@"SudokuGrid\sudoku0.csv");
            mpSudokuSolver.PrintSudokuGrid();
            Console.Write("Start solving ? Press enter...\n");
            Console.ReadKey(true);

            mpSudokuSolver.InitSudoku();
            Solver.FillWithObviousValues(mpSudokuSolver);

            Console.Write("\n");
            mpSudokuSolver.PrintSudokuGrid();
            Console.ReadKey(true);

            int a = 0;
            a++;
        }

    }
}
