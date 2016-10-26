using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuCSP
{
    /// <summary>
    /// Console class used to solve a provided Sudoku.
    /// </summary>
    public class Sudoku
    {
        /// <summary>
        /// Set the Sudoku size.
        /// </summary>
        private int m_iSudokuSize = 9;
        /// <summary>
        /// Store the Sudoku array.
        /// </summary>
        private Cell[,] m_aiSudokuGrid;
        /// <summary>
        /// Store the resolution state of the Sudoku.
        /// </summary>
        private bool m_bSolved = false;
        /// <summary>
        /// Store the starting values coordinates.
        /// </summary>
        private List<Coordinate> m_lcStartingValuesCoordinate = new List<Coordinate>();
        /// <summary>
        /// The list of the possible values for an unassigned cell at start.
        /// </summary>
        private List<int> m_liPossibleValueAtStart = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// Get the Sudoku size.
        /// </summary>
        public int SudokuSize
        {
            get
            {
                return m_iSudokuSize;
            }
        }

        /// <summary>
        /// Get the Sudoku grid.
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

                        for (int i = 0; i < m_iSudokuSize; i++)
                        {
                            m_aiSudokuGrid[iIndexRead, i].CellValue = Convert.ToInt32(asValues[i]);
                            if (Convert.ToInt32(asValues[i]) != 0)
                            {
                                m_aiSudokuGrid[iIndexRead, i].Assigned = true;
                                m_lcStartingValuesCoordinate.Add(new Coordinate(iIndexRead, i));
                            }
                        }

                        iIndexRead++;
                    }
                }
                InitSudoku();
            }
            catch (Exception e)
            {
                Console.WriteLine("The CSV file could not be read:");
                Console.WriteLine(e.Message + "\n");

                Console.Write("Please enter the name of the Sudoku to solve (without the extension) and press enter.\n");
                string sSudokuName = Console.ReadLine();
                ReadCSV(@"SudokuGrid\" + sSudokuName + ".csv");
            }
        }

        /// <summary>
        /// Print the stored Sudoku grid in the console.
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
                            // If the Sudoku isn't solved, diplay starting value in red.
                            if (!(m_bSolved))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                                Console.ResetColor();
                            }
                            else
                            {
                                // Flag used to detect a starting value.
                                bool bFlagStartingValue = false;

                                for (int k = 0; k < m_lcStartingValuesCoordinate.Count; k++)
                                {
                                    // Test if the value to display is one of the starting value -> display in red.
                                    if (m_lcStartingValuesCoordinate[k].Row == i && m_lcStartingValuesCoordinate[k].Column == j)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write(" " + m_aiSudokuGrid[i, j].CellValue.ToString());
                                        Console.ResetColor();
                                        bFlagStartingValue = true;
                                        break;
                                    }
                                }

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
        /// Return the row and column peers' coordinates for a given cell.
        /// </summary>
        /// <param name="p_cCellCoordinate"> The coordinates of the given cell. </param>
        /// <returns> The row and column peers' coordinates as a list. </returns>
        private List<Coordinate> ComputeRowAndColumnPeers(Coordinate p_cCellCoordinate)
        {
            List<Coordinate> lcRowPeers = new List<Coordinate>();
            List<Coordinate> lcColumnPeers = new List<Coordinate>();

            for (int i = 0; i < m_iSudokuSize; i++)
            {
                if (i != p_cCellCoordinate.Column)
                {
                    lcRowPeers.Add(new Coordinate(p_cCellCoordinate.Row, i));
                }
                if (i != p_cCellCoordinate.Row)
                {
                    lcColumnPeers.Add(new Coordinate(i, p_cCellCoordinate.Column));
                }
            }

            lcRowPeers.AddRange(lcColumnPeers);
            return lcRowPeers;
        }

        /// <summary>
        /// Return the region peers' coordinates for a given cell.
        /// </summary>
        /// <param name="p_cCellCoordinate"> The coordinates of the given cell. </param>
        /// <returns> The region peers' coordinates as a list. </returns>
        private List<Coordinate> ComputeRegionPeers(Coordinate p_cCellCoordinate)
        {
            List<Coordinate> lcResult = new List<Coordinate>();
            int iRowPosition = p_cCellCoordinate.Row % (int)Math.Sqrt(m_iSudokuSize);
            int iColumnPosition = p_cCellCoordinate.Column % (int)Math.Sqrt(m_iSudokuSize);

            switch (iRowPosition)
            {
                case 0:
                    switch (iColumnPosition)
                    {
                        case 0:
                            List<Coordinate> lcPeers00 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 2),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column + 2) };
                            lcResult.AddRange(lcPeers00);
                            break;
                        case 1:
                            List<Coordinate> lcPeers01 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column + 1) };
                            lcResult.AddRange(lcPeers01);
                            break;
                        case 2:
                            List<Coordinate> lcPeers02 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row + 2, p_cCellCoordinate.Column - 1) };
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
                            List<Coordinate> lcPeers10 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row -1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column + 2),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 2) };
                            lcResult.AddRange(lcPeers10);
                            break;
                        case 1:
                            List<Coordinate> lcPeers11 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column + 1) };
                            lcResult.AddRange(lcPeers11);
                            break;
                        case 2:
                            List<Coordinate> lcPeers12 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row + 1, p_cCellCoordinate.Column - 1) };
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
                            List<Coordinate> lcPeers20 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column + 2),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column + 2) };
                            lcResult.AddRange(lcPeers20);
                            break;
                        case 1:
                            List<Coordinate> lcPeers21 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column + 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column + 1) };
                            lcResult.AddRange(lcPeers21);
                            break;
                        case 2:
                            List<Coordinate> lcPeers22 = new List<Coordinate> { new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row - 2, p_cCellCoordinate.Column - 1),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 2),
                                new Coordinate(p_cCellCoordinate.Row - 1, p_cCellCoordinate.Column - 1) };
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
        /// Initialise the Sudoku by finding the peers' coordinates and filling the list of possible values for the unassigned cells.
        /// </summary>
        private void InitSudoku()
        {
            for (int i = 0; i < m_iSudokuSize; i++)
            {
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    Coordinate cCellCoordinate = new Coordinate(i, j);
                    List<Coordinate> lcPeers = new List<Coordinate>();

                    lcPeers.AddRange(ComputeRowAndColumnPeers(cCellCoordinate));
                    lcPeers.AddRange(ComputeRegionPeers(cCellCoordinate));
                    m_aiSudokuGrid[i, j].Peers.AddRange(lcPeers);

                    if (!(m_aiSudokuGrid[i, j].Assigned))
                    {
                        m_aiSudokuGrid[i, j].PossibleValues.AddRange(m_liPossibleValueAtStart);
                    }
                }
            }
        }

    }
}
