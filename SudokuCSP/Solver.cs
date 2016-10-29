using System.Collections.Generic;
using System.Linq;

namespace SudokuCSP
{
    /// <summary>
    /// Static class containing the method to solve the Sudoku.
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Variable used to keep track of the number of backtrack performed.
        /// </summary>
        private static int m_iBacktrackNumber = 0;
        /// <summary>
        /// Index used to compute the step number.
        /// </summary>
        //private static int m_iStepIndex = 0;

        /// <summary>
        /// Get the number of backtrack performed.
        /// </summary>
        public static int BacktrackNumber
        {
            get
            {
                return m_iBacktrackNumber;
            }
            set
            {
                m_iBacktrackNumber = value;
            }
        }

        /// <summary>
        /// Solve a given Sudoku using the Backtracking Search.
        /// </summary>
        /// <param name="p_sSudokuToSolve"> The Sudoku to solve. </param>
        /// <returns> The Sudoku solved, null otherwise. </returns>
        public static Sudoku SolveSudoku(Sudoku p_sSudokuToSolve)
        {
            return BacktrackingSearch(p_sSudokuToSolve);
        }

        /// <summary>
        /// Check if the Sudoku is solved, i.e. if every cell has been assigned.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <returns> True if the Sudoku is solved, false otherwise. </returns>
        private static bool IsSolved(Sudoku p_sSudoku)
        {
            for (int i = 0; i < p_sSudoku.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudoku.SudokuSize; j++)
                {
                    if (!(p_sSudoku.SudokuGrid[i, j].Assigned))
                    {
                        return false;
                    }
                }
            }
            p_sSudoku.Solved = true;
            return true;
        }

        /// <summary>
        /// Return a copy of a given Sudoku (used for backtrack Search).
        /// </summary>
        /// <param name="p_sSudokuToCopy"> The Sudoku to copy. </param>
        /// <returns> A copy of the given Sudoku. </returns>
        private static Sudoku Clone(Sudoku p_sSudokuToCopy)
        {
            Sudoku sSudokuCopied = new Sudoku(p_sSudokuToCopy.SudokuSize);

            for (int i = 0; i < p_sSudokuToCopy.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudokuToCopy.SudokuSize; j++)
                {
                    sSudokuCopied.SudokuGrid[i, j] = new Cell(p_sSudokuToCopy.SudokuGrid[i, j]);
                }
            }
            sSudokuCopied.StartingValuesCoordinate = p_sSudokuToCopy.StartingValuesCoordinate;
            sSudokuCopied.NotAtStart = p_sSudokuToCopy.NotAtStart;

            return sSudokuCopied;
        }

        /// <summary>
        /// Optimize the algorithm by cleaning the list of possible values for each cell with the already assigned cells at start.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The coordinates of the given cell. </param>
        /// <returns> The list of possible values without the peers cells values. </returns>
        public static List<int> CleanPossibleValuesAtStart(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
        {
            // Find the constraints imposed by the peers.
            List<int> liConstraintValues = new List<int>();

            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers.Count; i++)
            {
                liConstraintValues.Add(p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[i].Row,
                    p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[i].Column].CellValue);
            }

            // Compare the list of possible values with the list of constraints imposed by the peers.
            List<int> liPossibleValues = new List<int>();
            for (int i = 1; i < p_sSudoku.SudokuSize + 1; i++)
            {
                if (!(liConstraintValues.Contains(i)))
                {
                    liPossibleValues.Add(i);
                }
            }
            return liPossibleValues;
        }

        /// <summary>
        /// Return the list of possible values for a given cell.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The coordinates of the given cell. </param>
        /// <returns> The list of possible values for a given cell. </returns>
        private static List<int> ComputePossibleValues(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
        {
            // Find the constraints imposed by the peers.
            List<int> liConstraintValues = new List<int>();

            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers.Count; i++)
            {
                liConstraintValues.Add(p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[i].Row,
                    p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[i].Column].CellValue);
            }

            // Compare the list of possible values with the constraints imposed by the peers.
            List<int> liPossibleValues = new List<int>();

            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues.Count; i++)
            {
                if (!(liConstraintValues.Contains(p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[i])))
                {
                    liPossibleValues.Add(p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[i]);
                }
            }

            return liPossibleValues;
        }

        /// <summary>
        /// Look for the next unassigned cell.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <returns> The coordinates of the first unassigned cell. </returns>
        private static Coordinate FirstUnassignedCell(Sudoku p_sSudoku)
        {
            for (int i = 0; i < p_sSudoku.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudoku.SudokuSize; j++)
                {
                    if (!(p_sSudoku.SudokuGrid[i, j].Assigned))
                    {
                        return new Coordinate(i, j);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Look for the cell with a minimum number of possible values and return the first one found.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <returns> The first cell with a minimum number of possible values. </returns>
        private static Coordinate CellWithMinimumRemainingValue(Sudoku p_sSudoku)
        {
            int iCurrentMinRemainingValue = p_sSudoku.SudokuSize + 1;
            Coordinate cResult = new Coordinate();

            for (int i = 0; i < p_sSudoku.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudoku.SudokuSize; j++)
                {
                    if ((p_sSudoku.SudokuGrid[i, j].PossibleValues.Count < iCurrentMinRemainingValue) && (!(p_sSudoku.SudokuGrid[i, j].Assigned)))
                    {
                        iCurrentMinRemainingValue = p_sSudoku.SudokuGrid[i, j].PossibleValues.Count;
                        cResult.Row = i;
                        cResult.Column = j;
                    }
                }
            }
            return cResult;
        }

        /// <summary>
        /// Return the first item of the list of possible value for a given cell.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The cell coordinates.  </param>
        /// <returns> The first item of the list of possible value for a given cell. </returns>
        private static int FirstPossibleValue(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
        {
            return p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[0];
        }

        /// <summary>
        /// Return the item in the list of possible value which appear the most in the peers' possible values.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The cell coordinates. </param>
        /// <returns> The item in the list of possible value which appear the most in the peers' possible values. </returns>
        private static int ValueWithMaxConstraints(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
        {
            int[] aiConstraintCountForValues = new int[p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues.Count];

            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues.Count; i++)
            {
                for (int j = 0; j < p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers.Count; j++)
                {
                    if (p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[j].Row,
                        p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].Peers[j].Column].PossibleValues.Contains(
                        p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[i]))
                    {
                        aiConstraintCountForValues[i]++;
                    }
                }
            }
            return p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[aiConstraintCountForValues.ToList().IndexOf(
                aiConstraintCountForValues.ToList().Min())];
        }

        /// <summary>
        /// Remove value p_iValue from the list of possible values of a given cell.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider. </param>
        /// <param name="p_cCoordinate"> The coordinates of the given cell. </param>
        /// <param name="p_iValue"> The value to remove from the list of possible values of the cell. </param>
        /// <returns> The Sudoku where the list of possible values of the cell has been updated. </returns>
        private static Sudoku RemoveValueFromPossibleValues(Sudoku p_sSudoku, Coordinate p_cCoordinate, int p_iValue)
        {
            if (!(p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Assigned))
            {
                p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].PossibleValues.Remove(p_iValue);
            }
            return p_sSudoku;
        }

        /// <summary>
        /// Assign p_iValue to the cell with the coordinates p_cCoordinate and remove the value 
        /// with the ForwardChecking method from the peers' list of possible values.
        /// </summary>
        /// <param name="p_sSudoku"> The Sudoku to consider </param>
        /// <param name="p_cCoordinate"> The coordinates of the cell considered. </param>
        /// <param name="p_iValue"> The value to give to the cell and to remove from the peers' list of possible values </param>
        /// <returns> The Sudoku with the cell value changed and the peers' list of possible values updated. </returns>
        private static Sudoku AssignValueWithForwardChecking(Sudoku p_sSudoku, Coordinate p_cCoordinate, int p_iValue)
        {
            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers.Count; i++)
            {
                // We remove the given value from the peers' list of possible values.
                p_sSudoku = RemoveValueFromPossibleValues(p_sSudoku, p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i], p_iValue);
                // Check if a peer has its list of possible value empty and has no value assigned, i.e. need to backtrack.
                if ((p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Row,
                        p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Column].PossibleValues.Count == 0) &&
                        (p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Row,
                        p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Column].Assigned == false))
                {
                    return null;
                }
            }
            // If the value doesn't leave a peer without possible value, assign this value to the given cell.
            p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].CellValue = p_iValue;
            p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Assigned = true;

            /* Code used to display the steps in the console.
            Console.SetCursorPosition(0, 0);
            Console.Write("Step number " + index + " :\n");
            p_sSudoku.PrintSudokuGrid();
            Console.Write("\n");
            index++;
            */
            return p_sSudoku;
        }

        /// <summary>
        /// Run a backtracking search on a given Sudoku.
        /// </summary>
        /// <param name="p_sSudokuToSolve"> The Sudoku to solve. </param>
        /// <returns> The resulting Sudoku if solved, null otherwise. </returns>
        private static Sudoku BacktrackingSearch(Sudoku p_sSudokuToSolve)
        {
            // Handles the case of backtracking.
            if (p_sSudokuToSolve == null)
            {
                return null;
            }
            // Variable to store the backtracking result.
            Sudoku sBacktrackResultingSudoku = null;
            // If the Sudoku is solved, return the result.
            if (IsSolved(p_sSudokuToSolve))
            {
                return p_sSudokuToSolve;
            }
            // Find the cell with the smallest list of possible values.
            Coordinate cUnassignedVariableCoord = CellWithMinimumRemainingValue(p_sSudokuToSolve);
            // As long as we have some values in the list of possible values, we keep going deeper in the tree.
            while (p_sSudokuToSolve.SudokuGrid[cUnassignedVariableCoord.Row, cUnassignedVariableCoord.Column].PossibleValues.Count > 0)
            {
                // In the list of possible values of the chosen cell, get the one with the most constraint from peers. 
                int iValue = ValueWithMaxConstraints(p_sSudokuToSolve, cUnassignedVariableCoord);

                // If the value with most constraints is consistent with the assignement, i.e. if not present in a peer's cell value.
                if (ComputePossibleValues(p_sSudokuToSolve, cUnassignedVariableCoord).Contains(iValue))
                {
                    // Run a backtracking search with this new cell value and with the peers' value updated.
                    sBacktrackResultingSudoku = BacktrackingSearch(AssignValueWithForwardChecking(Clone(p_sSudokuToSolve), cUnassignedVariableCoord, iValue));
                    // Used to backtrack once the solution is found.
                    if (sBacktrackResultingSudoku != null)
                    {
                        return sBacktrackResultingSudoku;
                    }
                }

                // if the peers update failed, i.e. if one peer is not assigned and has no possible values remaining.
                // m_iBacktrackNumber is used to keep track of the count of backtracks.
                m_iBacktrackNumber++;
                // Remove the most constrained value from the list of possible values. 
                p_sSudokuToSolve = RemoveValueFromPossibleValues(p_sSudokuToSolve, cUnassignedVariableCoord, iValue);
            }
            return null;
        }

    }
}
