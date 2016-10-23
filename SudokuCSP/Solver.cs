using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCSP
{
    /// <summary>
    /// 
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_sSudoku"></param>
        public static void FillWithObviousValues(Sudoku p_sSudoku)
        {
            int iStartRemainingValues = int.MaxValue;
            int iEndRemainingValues = int.MinValue;

            while (iEndRemainingValues < iStartRemainingValues)
            {
                iStartRemainingValues = p_sSudoku.RemainingZeros;
                for (int i = 0; i < p_sSudoku.SudokuSize; i++)
                {
                    for (int j = 0; j < p_sSudoku.SudokuSize; j++)
                    {
                        if (p_sSudoku.SudokuGrid[i, j].PossibleValues.Count == 1)
                        {
                            p_sSudoku.SudokuGrid[i, j].CellValue = p_sSudoku.SudokuGrid[i, j].PossibleValues[0];
                            for (int k = 0; k < p_sSudoku.SudokuGrid[i, j].Peers.Count; k++)
                            {
                                if (p_sSudoku.SudokuGrid[i, j].Peers[k].PossibleValues.Contains(p_sSudoku.SudokuGrid[i, j].CellValue))
                                {
                                    p_sSudoku.SudokuGrid[i, j].Peers[k].PossibleValues.Remove(p_sSudoku.SudokuGrid[i, j].CellValue);
                                }
                            }
                            p_sSudoku.SudokuGrid[i, j].PossibleValues.Clear();
                            p_sSudoku.RemainingZeros--;
                        }
                    }
                }
                iEndRemainingValues = p_sSudoku.RemainingZeros;
            }
        }

    }
}
