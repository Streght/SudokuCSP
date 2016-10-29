namespace SudokuCSP
{
    /// <summary>
    /// Public class coordinate used to store the row and column of the peers of a given cell.
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Row coordinate.
        /// </summary>
        private int m_iRow;
        /// <summary>
        /// Column coordinate.
        /// </summary>
        private int m_iColumn;

        /// <summary>
        /// Get / set the row coordinate.
        /// </summary>
        public int Row
        {
            get
            {
                return m_iRow;
            }
            set
            {
                m_iRow = value;
            }
        }

        /// <summary>
        /// Get / set the column coordinate.
        /// </summary>
        public int Column
        {
            get
            {
                return m_iColumn;
            }
            set
            {
                m_iColumn = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Coordinate() { }

        /// <summary>
        /// Constructor used to create a coordinate from a given row and column.
        /// </summary>
        /// <param name="p_iRow"> The given row. </param>
        /// <param name="p_iColumn"> The given column. </param>
        public Coordinate(int p_iRow, int p_iColumn)
        {
            m_iRow = p_iRow;
            m_iColumn = p_iColumn;
        }

    }
}
