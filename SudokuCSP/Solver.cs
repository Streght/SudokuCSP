using System.Linq;

namespace SudokuCSP
{
    /// <summary>
    /// Static class containing the method to solve the sudoku.
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Variable used to keep track of the number of backtrack performed.
        /// </summary>
        private static int m_iBacktrackNumber = 0;

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
        /// Check if the sudoku is solved, i.e. if every cell has been assigned.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to consider. </param>
        /// <returns> True if the sudoku is solved, false otherwise. </returns>
        private static bool IsFinished(Sudoku p_sSudoku)
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
        /// Return a copy of a given sudoku (used for backtrackSearch).
        /// </summary>
        /// <param name="p_sSudokuToCopy"> The sudoku to copy. </param>
        /// <returns> A copy of the given sudoku. </returns>
        private static Sudoku Clone(Sudoku p_sSudokuToCopy)
        {
            Sudoku sSudokuCopy = new Sudoku();

            for (int i = 0; i < p_sSudokuToCopy.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudokuToCopy.SudokuSize; j++)
                {
                    sSudokuCopy.SudokuGrid[i, j] = new Cell(p_sSudokuToCopy.SudokuGrid[i, j]);
                }
            }
            sSudokuCopy.StartingValuesCoordinate = p_sSudokuToCopy.StartingValuesCoordinate;

            return sSudokuCopy;
        }

        /// <summary>
        /// Look for the cell with a minimum number of possible values and return the first one found.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to consider. </param>
        /// <returns> The first cell with a minimum number of possible values. </returns>
        private static Coordinate MinimumRemainingValue(Sudoku p_sSudoku)
        {
            int iCurrentMinPossibleValue = p_sSudoku.SudokuSize + 1;
            Coordinate cResult = new Coordinate();

            for (int i = 0; i < p_sSudoku.SudokuSize; i++)
            {
                for (int j = 0; j < p_sSudoku.SudokuSize; j++)
                {
                    if ((p_sSudoku.SudokuGrid[i, j].PossibleValues.Count < iCurrentMinPossibleValue) && (!(p_sSudoku.SudokuGrid[i, j].Assigned)))
                    {
                        iCurrentMinPossibleValue = p_sSudoku.SudokuGrid[i, j].PossibleValues.Count;
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
        /// <param name="p_sSudoku"> The sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The cell coordinates.  </param>
        /// <returns> The first item of the list of possible value for a given cell. </returns>
        private static int FirstPossibleValue(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
        {
            return p_sSudoku.SudokuGrid[p_cCellCoordinate.Row, p_cCellCoordinate.Column].PossibleValues[0];
        }

        /// <summary>
        /// Return the item in the list of possible value which appear the least in the peers' possible values.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to consider. </param>
        /// <param name="p_cCellCoordinate"> The cell coordinates. </param>
        /// <returns> The item in the list of possible value which appear the least in the peers' possible values. </returns>
        private static int ValueWithLeastConstraint(Sudoku p_sSudoku, Coordinate p_cCellCoordinate)
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
        /// Remove value with the ForwardChecking method for a given cell, i.e. remove the value from the list of possible values of the cell.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to consider. </param>
        /// <param name="p_cCoordinate"> The coordinates of the cell. </param>
        /// <param name="p_iValue"> The value to remove from the list of possible values if the cell. </param>
        /// <returns> The sudoku where the list of possible values of the cell has been updated. </returns>
        private static Sudoku RemoveValueFC(Sudoku p_sSudoku, Coordinate p_cCoordinate, int p_iValue)
        {
            p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].PossibleValues.Remove(p_iValue);
            return p_sSudoku;
        }

        /// <summary>
        /// Assign p_iValue to the cell with the coordinates p_cCoordinate and remove the value with the ForwardChecking method from the peers' list of possible values.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to consider </param>
        /// <param name="p_cCoordinate"> The coordinates of the cell considered. </param>
        /// <param name="p_iValue"> The value to give to the cell and to remove from the peers' list of possible values </param>
        /// <returns> The sudoku with the cell value changed and the peers' list of possible values updated. </returns>
        private static Sudoku AssignValueFC(Sudoku p_sSudoku, Coordinate p_cCoordinate, int p_iValue)
        {
            for (int i = 0; i < p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers.Count; i++)
            {
                p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Row,
                        p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Column].PossibleValues.Remove(p_iValue);
                // Check if a peers has its list of possible value empty and has no value assigned, i.e. need to backtrack.
                if (p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Row,
                        p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Column].PossibleValues.Count == 0 &&
                        p_sSudoku.SudokuGrid[p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Row,
                        p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Peers[i].Column].Assigned == false)
                {
                    return null;
                }
            }
            p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].CellValue = p_iValue;
            p_sSudoku.SudokuGrid[p_cCoordinate.Row, p_cCoordinate.Column].Assigned = true;
            return p_sSudoku;
        }

        /// <summary>
        /// Run a backtracking search on a given sudoku.
        /// </summary>
        /// <param name="p_sSudoku"> The sudoku to solve. </param>
        /// <returns> The resulting sudoku, solved or not. </returns>
        public static Sudoku BacktrackingSearch(Sudoku p_sSudoku)
        {
            Sudoku sSudoku;

            if (p_sSudoku == null)
            {
                return null;
            }
            if (IsFinished(p_sSudoku))
            {
                return p_sSudoku;
            }

            Coordinate cUnassignedVariableCoord = MinimumRemainingValue(p_sSudoku);

            while (p_sSudoku.SudokuGrid[cUnassignedVariableCoord.Row, cUnassignedVariableCoord.Column].PossibleValues.Count > 0)
            {
                int iValue = ValueWithLeastConstraint(p_sSudoku, cUnassignedVariableCoord);
                sSudoku = BacktrackingSearch(AssignValueFC(Clone(p_sSudoku), cUnassignedVariableCoord, iValue));
                if (sSudoku != null)
                {
                    return sSudoku;
                }

                p_sSudoku = RemoveValueFC(p_sSudoku, cUnassignedVariableCoord, iValue);
                if (p_sSudoku == null)
                {
                    return null;
                }
                m_iBacktrackNumber++;
            }
            return null;

        }

    }
}
