using System.Collections.Generic;

namespace SudokuCSP
{
    /// <summary>
    /// Class used to represent a sudoku cell. It is composed of the following :
    /// - m_iCellValue which is the current value of the cell. It equals 0 if the cell doesn't have any value.
    /// - m_liPossibleValues which is the list of possible value for the current cell. It is empty if the cell has a m_iCellValue different from zero.
    /// - m_lcPeers which is the list of the peers' coordinates for the current cell, i.e. the coordinate of the cells in the same row / column / region.
    /// - m_bAssigned which indicates if the cell has an assigned value.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The current value of the cell.
        /// </summary>
        private int m_iCellValue;
        /// <summary>
        /// The list of possible value for the current cell.
        /// </summary>
        private List<int> m_liPossibleValues = new List<int>();
        /// <summary>
        /// The list of the peers' coordinates for the current cell.
        /// </summary>
        private List<Coordinate> m_lcPeers = new List<Coordinate>();
        /// <summary>
        /// The current assignement state for the cell.
        /// </summary>
        private bool m_bAssigned = false;

        /// <summary>
        /// Get / set the value of the cell.
        /// </summary>
        public int CellValue
        {
            get
            {
                return m_iCellValue;
            }
            set
            {
                // If we set the m_iCellValue to something else than 0, we empty the list m_liPossibleValues.
                if (value != 0)
                {
                    m_liPossibleValues.Clear();
                }
                m_iCellValue = value;
            }
        }

        /// <summary>
        /// Get the list of possible values.
        /// </summary>
        public List<int> PossibleValues
        {
            get
            {
                return m_liPossibleValues;
            }
        }

        /// <summary>
        /// Get the list of peers.
        /// </summary>
        public List<Coordinate> Peers
        {
            get
            {
                return m_lcPeers;
            }
        }

        /// <summary>
        /// Get / set the assignement state of the cell.
        /// </summary>
        public bool Assigned
        {
            get
            {
                return m_bAssigned;
            }
            set
            {
                m_bAssigned = value;
            }
        }

        /// <summary>
        /// Default cell constructor.
        /// </summary>
        public Cell() { }

        /// <summary>
        /// Cell constructor used to clone a cell.
        /// </summary>
        /// <param name="p_cCellToCopy"></param>
        public Cell(Cell p_cCellToCopy)
        {
            m_iCellValue = p_cCellToCopy.m_iCellValue;
            m_bAssigned = p_cCellToCopy.m_bAssigned;

            // COpy the cell's peers.
            for (int i = 0; i < p_cCellToCopy.m_lcPeers.Count; i++)
            {
                m_lcPeers.Add(p_cCellToCopy.m_lcPeers[i]);
            }

            // Copy the cell's list of possible value.
            for (int i = 0; i < p_cCellToCopy.m_liPossibleValues.Count; i++)
            {
                m_liPossibleValues.Add(p_cCellToCopy.m_liPossibleValues[i]);
            }
        }

    }
}
