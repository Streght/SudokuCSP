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
        private int m_iSudokuSize = -1;
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
        /// The list of coordinate stored by region.
        /// </summary>
        private List<Coordinate>[,] m_lcRegionPeers = null;
        /// <summary>
        /// Used to do the correspondance between number and letter.
        /// </summary>
        private char[] m_acValueToDisplay = null;
        /// <summary>
        /// Used to indicate if the Sudoku is in the start state or not.
        /// </summary>
        private bool m_bNotAtStart = false;

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
        /// Get / set if the Sudoku is in the start state or not.
        /// </summary>
        public bool NotAtStart
        {
            get
            {
                return m_bNotAtStart;
            }
            set
            {
                m_bNotAtStart = true;
            }
        }

        /// <summary>
        /// Create a new SudokuSolver.
        /// </summary>
        public Sudoku(int p_iSudokuSize)
        {
            m_iSudokuSize = p_iSudokuSize;
            m_aiSudokuGrid = new Cell[m_iSudokuSize, m_iSudokuSize];
            m_lcRegionPeers = new List<Coordinate>[(int)Math.Sqrt(m_iSudokuSize), (int)Math.Sqrt(m_iSudokuSize)];
            switch (m_iSudokuSize)
            {
                case 9:
                    m_acValueToDisplay = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    break;
                case 16:
                    m_acValueToDisplay = new char[16] { '1', '2', '3', '4', '5', '6', '7', '8', '9',
                        'A', 'B', 'C', 'D', 'E', 'F', 'G' };
                    break;
                default:
                    break;
            }

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
            string sLineSeparator = "+";
            for(int i = 0; i < Math.Sqrt(m_iSudokuSize); i++)
            {
                sLineSeparator = sLineSeparator + "--";
            }
            sLineSeparator = sLineSeparator + "-";

            if (m_aiSudokuGrid != null)
            {
                for (int i = 0; i < m_iSudokuSize; i++)
                {
                    if (i % Math.Sqrt(m_iSudokuSize) == 0)
                    {
                        Console.Write(" " + sLineSeparator);
                        for (int k = 1; k < Math.Sqrt(m_iSudokuSize); k++)
                        {
                            Console.Write(sLineSeparator);
                        }
                        Console.Write("+\n");
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
                            if (!(m_bSolved) && !(m_bNotAtStart))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" " + m_acValueToDisplay[m_aiSudokuGrid[i, j].CellValue - 1].ToString());
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
                                        Console.Write(" " + m_acValueToDisplay[m_aiSudokuGrid[i, j].CellValue - 1].ToString());
                                        Console.ResetColor();
                                        bFlagStartingValue = true;
                                        break;
                                    }
                                }

                                // If the value to display isn't a starting value, display it in green.
                                if (!(bFlagStartingValue))
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(" " + m_acValueToDisplay[m_aiSudokuGrid[i, j].CellValue - 1].ToString());
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                    Console.Write(" |\n");
                }
                Console.Write(" " + sLineSeparator);
                for (int k = 1; k < Math.Sqrt(m_iSudokuSize); k++)
                {
                    Console.Write(sLineSeparator);
                }
                Console.Write("+\n");
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
        /// Create a matrix of list of coordinates grouped by region.
        /// </summary>
        /// <returns> A matrix of list of coordinates grouped by region. </returns>
        private List<Coordinate>[,] FillInPeersByRegion()
        {
            List<Coordinate>[,] lcPeersByRegion = new List<Coordinate>[(int)Math.Sqrt(m_iSudokuSize), (int)Math.Sqrt(m_iSudokuSize)];
            for (int i = 0; i < (int)Math.Sqrt(m_iSudokuSize); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(m_iSudokuSize); j++)
                {
                    lcPeersByRegion[i, j] = new List<Coordinate>();
                }
            }

            int iRegionRowIndex = 0;
            int iRowOffset = -1;

            for (int i = 0; i < m_iSudokuSize; i++)
            {
                if (iRegionRowIndex % (int)Math.Sqrt(m_iSudokuSize) == 0)
                {
                    iRowOffset++;
                }
                int iRegionColumnIndex = 0;
                int iColumnOffset = -1;
                for (int j = 0; j < m_iSudokuSize; j++)
                {
                    if (iRegionColumnIndex % (int)Math.Sqrt(m_iSudokuSize) == 0)
                    {
                        iColumnOffset++;
                    }
                    int RowRegionIndex = iRowOffset;
                    int ColumnRegionIndex = iColumnOffset;
                    lcPeersByRegion[RowRegionIndex, ColumnRegionIndex].Add(new Coordinate(i, j));
                    iRegionColumnIndex++;
                }
                iRegionRowIndex++;
            }
            return lcPeersByRegion;
        }

        /// <summary>
        /// Return the region peers' coordinates for a given cell.
        /// </summary>
        /// <param name="p_cCellCoordinate"> The coordinates of the given cell. </param>
        /// <returns> The region peers' coordinates as a list. </returns>
        private List<Coordinate> ComputeRegionPeers(Coordinate p_cCellCoordinate)
        {
            List<Coordinate> result = null;

            for (int i = 0; i < (int)Math.Sqrt(m_iSudokuSize); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(m_iSudokuSize); j++)
                {
                    for (int k = 0; k < m_lcRegionPeers[i, j].Count; k++)
                    {
                        if ((m_lcRegionPeers[i, j][k].Row == p_cCellCoordinate.Row) &&
                            (m_lcRegionPeers[i, j][k].Column == p_cCellCoordinate.Column))
                        {
                            result = m_lcRegionPeers[i, j];
                        }
                    }
                }
            }

            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (result[i].Row == p_cCellCoordinate.Row || result[i].Column == p_cCellCoordinate.Column)
                {
                    result.Remove(result[i]);
                }
            }
            return result;
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

                    m_lcRegionPeers = FillInPeersByRegion();

                    lcPeers.AddRange(ComputeRowAndColumnPeers(cCellCoordinate));
                    lcPeers.AddRange(ComputeRegionPeers(cCellCoordinate));
                    m_aiSudokuGrid[i, j].Peers.AddRange(lcPeers);

                    if (!(m_aiSudokuGrid[i, j].Assigned))
                    {
                        m_aiSudokuGrid[i, j].PossibleValues.AddRange(Solver.CleanPossibleValuesAtStart(this, cCellCoordinate));
                    }
                }
            }
        }

    }
}
