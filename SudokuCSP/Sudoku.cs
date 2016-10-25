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
        /// Store the resolution state of the sudoku.
        /// </summary>
        private bool m_bSolved = false;
        /// <summary>
        /// Store the starting values coordinates.
        /// </summary>
        private List<Coordinate> m_lcStartingValuesCoordinate = new List<Coordinate>();

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
        /// Get / set the resolution state.
        /// </summary>
        public bool Solved
        {
            get
            {
                return m_bSolved;
            }
            set
            {
                m_bSolved = value;
            }
        }

        /// <summary>
        /// Get / set the coordinates of the starting values.
        /// </summary>
        public List<Coordinate> StartingValuesCoordinate
        {
            get
            {
                return m_lcStartingValuesCoordinate;
            }
            set
            {
                m_lcStartingValuesCoordinate = value;
            }
        }

        /// <summary>
        /// Create a new SudokuSolver.
        /// </summary>
        public Sudoku()
        {
            m_aiSudokuGrid = new Cell[m_iSudokuSize, m_iSudokuSize];

            for (int i = 0; i < m_iSudokuSize; i++)
            {
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    m_aiSudokuGrid[i, j] = new Cell();
                }
            }
        }

        /// <summary>
        /// Read the CSV file provided.
        /// </summary>
        public void ReadCSV(string p_sPath)
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
                            if (Convert.ToInt32(asValues[i]) != 0)
                            {
                                m_aiSudokuGrid[iIndexRead, i].Assigned = true;
                                m_lcStartingValuesCoordinate.Add(new Coordinate(iIndexRead, i));
                            }
                        });

                        iIndexRead++;
                    }
                }
                InitSudoku();
            }
            catch (Exception e)
            {
                Console.WriteLine("The CSV file could not be read:");
                Console.WriteLine(e.Message + "\n");

                Console.Write("Please enter the name of the sudoku to solve (without the extension) and press enter.\n");
                string sSudokuName = Console.ReadLine();
                ReadCSV(@"SudokuGrid\" + sSudokuName + ".csv");
            }
        }

        /// <summary>
        /// Print the stored sudoku grid in the console.
        /// </summary>
        public void PrintSudokuGrid()
        {
            if (m_aiSudokuGrid != null)
            {
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
                            // If the sudoku isn't solved, diplay starting value in red.
                            if (!(m_bSolved))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                                Console.ResetColor();
                            }
                            if (m_bSolved)
                            {
                                // Flag used to detect a starting value.
                                bool bFlagStartingValue = false;

                                //for (int k = 0; k < m_lcStartingValuesCoordinate.Count; k++)
                                Parallel.For(0, m_lcStartingValuesCoordinate.Count, k =>
                                {
                                    // Test if the value to display is one of the starting value -> display in red.
                                    if (m_lcStartingValuesCoordinate[k].Row == i && m_lcStartingValuesCoordinate[k].Column == j)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                                        Console.ResetColor();
                                        bFlagStartingValue = true;
                                    }
                                });

                                // If the value to display isn't a starting value, display it in green.
                                if (!(bFlagStartingValue))
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                                    Console.ResetColor();
                                }
                            }
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
                liConstraintValues.Add(m_aiSudokuGrid[m_aiSudokuGrid[p_iRow, p_iColumn].Peers[i].Row, m_aiSudokuGrid[p_iRow, p_iColumn].Peers[i].Column].CellValue);
            }

            List<int> liPossibleValues = new List<int>();

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
        /// Return the row and column peers' coordinates for a given cell.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <returns> The row and column peers' coordinates as a list. </returns>
        private List<Coordinate> ComputeRowAndColumnPeers(int p_iRow, int p_iColumn)
        {
            List<Coordinate> lcRowPeers = new List<Coordinate>();
            List<Coordinate> lcColumnPeers = new List<Coordinate>();

            Parallel.For(0, m_iSudokuSize, i =>
            {
                if (i != p_iColumn)
                {
                    lcRowPeers.Add(new Coordinate(p_iRow, i));
                }
                if (i != p_iRow)
                {
                    lcColumnPeers.Add(new Coordinate(i, p_iColumn));
                }
            });

            lcRowPeers.AddRange(lcColumnPeers);
            return lcRowPeers;
        }

        /// <summary>
        /// Return the region peers' coordinates for a given cell.
        /// </summary>
        /// <param name="p_iRow"> The row of the given cell. </param>
        /// <param name="p_iColumn"> The column of the given cell. </param>
        /// <returns> The region peers' coordinates as a list. </returns>
        private List<Coordinate> ComputeRegionPeers(int p_iRow, int p_iColumn)
        {
            List<Coordinate> lcResult = new List<Coordinate>();
            int iRowPosition = p_iRow % (int)Math.Sqrt(m_iSudokuSize);
            int iColumnPosition = p_iColumn % (int)Math.Sqrt(m_iSudokuSize);

            switch (iRowPosition)
            {
                case 0:
                    switch (iColumnPosition)
                    {
                        case 0:
                            List<Coordinate> lcPeers00 = new List<Coordinate> { new Coordinate(p_iRow + 1, p_iColumn + 1),
                                new Coordinate(p_iRow + 1, p_iColumn + 2),
                                new Coordinate(p_iRow + 2, p_iColumn + 1),
                                new Coordinate(p_iRow + 2, p_iColumn + 2) };
                            lcResult.AddRange(lcPeers00);
                            break;
                        case 1:
                            List<Coordinate> lcPeers01 = new List<Coordinate> { new Coordinate(p_iRow + 1, p_iColumn - 1),
                                new Coordinate(p_iRow + 1, p_iColumn + 1),
                                new Coordinate(p_iRow + 2, p_iColumn - 1),
                                new Coordinate(p_iRow + 2, p_iColumn + 1) };
                            lcResult.AddRange(lcPeers01);
                            break;
                        case 2:
                            List<Coordinate> lcPeers02 = new List<Coordinate> { new Coordinate(p_iRow + 1, p_iColumn - 2),
                                new Coordinate(p_iRow + 1, p_iColumn - 1),
                                new Coordinate(p_iRow + 2, p_iColumn - 2),
                                new Coordinate(p_iRow + 2, p_iColumn - 1) };
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
                            List<Coordinate> lcPeers10 = new List<Coordinate> { new Coordinate(p_iRow -1, p_iColumn + 1),
                                new Coordinate(p_iRow - 1, p_iColumn + 2),
                                new Coordinate(p_iRow + 1, p_iColumn + 1),
                                new Coordinate(p_iRow + 1, p_iColumn + 2) };
                            lcResult.AddRange(lcPeers10);
                            break;
                        case 1:
                            List<Coordinate> lcPeers11 = new List<Coordinate> { new Coordinate(p_iRow - 1, p_iColumn - 1),
                                new Coordinate(p_iRow - 1, p_iColumn + 1),
                                new Coordinate(p_iRow + 1, p_iColumn - 1),
                                new Coordinate(p_iRow + 1, p_iColumn + 1) };
                            lcResult.AddRange(lcPeers11);
                            break;
                        case 2:
                            List<Coordinate> lcPeers12 = new List<Coordinate> { new Coordinate(p_iRow - 1, p_iColumn - 2),
                                new Coordinate(p_iRow - 1, p_iColumn - 1),
                                new Coordinate(p_iRow + 1, p_iColumn - 2),
                                new Coordinate(p_iRow + 1, p_iColumn - 1) };
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
                            List<Coordinate> lcPeers20 = new List<Coordinate> { new Coordinate(p_iRow - 2, p_iColumn + 1),
                                new Coordinate(p_iRow - 2, p_iColumn + 2),
                                new Coordinate(p_iRow - 1, p_iColumn + 1),
                                new Coordinate(p_iRow - 1, p_iColumn + 2) };
                            lcResult.AddRange(lcPeers20);
                            break;
                        case 1:
                            List<Coordinate> lcPeers21 = new List<Coordinate> { new Coordinate(p_iRow - 2, p_iColumn - 1),
                                new Coordinate(p_iRow - 2, p_iColumn + 1),
                                new Coordinate(p_iRow - 1, p_iColumn - 1),
                                new Coordinate(p_iRow - 1, p_iColumn + 1) };
                            lcResult.AddRange(lcPeers21);
                            break;
                        case 2:
                            List<Coordinate> lcPeers22 = new List<Coordinate> { new Coordinate(p_iRow - 2, p_iColumn - 2),
                                new Coordinate(p_iRow - 2, p_iColumn - 1),
                                new Coordinate(p_iRow - 1, p_iColumn - 2),
                                new Coordinate(p_iRow - 1, p_iColumn - 1) };
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
        /// Initialise the sudoku by finding the peers' coordinates and possible values for each cell.
        /// </summary>
        private void InitSudoku()
        {
            for (int i = 0; i < m_iSudokuSize; i++)
            {
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    List<Coordinate> lcPeers = new List<Coordinate>();
                    lcPeers.AddRange(ComputeRowAndColumnPeers(i, j));
                    lcPeers.AddRange(ComputeRegionPeers(i, j));
                    m_aiSudokuGrid[i, j].Peers.AddRange(lcPeers);

                    if (!(m_aiSudokuGrid[i, j].Assigned))
                    {
                        List<int> liPossibleValues = new List<int>();
                        liPossibleValues.AddRange(ComputePossibleValues(i, j));
                        m_aiSudokuGrid[i, j].PossibleValues.AddRange(liPossibleValues);
                    }
                }
            }
        }

    }
}
