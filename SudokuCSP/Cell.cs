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
    public class Cell
    {
        /// <summary>
        /// 
        /// </summary>
        private int m_iCellValue;
        /// <summary>
        /// 
        /// </summary>
        private List<int> m_liPossibleValues = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        private List<Cell> m_lcPeers = new List<Cell>();

        /// <summary>
        /// 
        /// </summary>
        public int CellValue
        {
            get
            {
                return m_iCellValue;
            }
            set
            {
                m_iCellValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<int> PossibleValues
        {
            get
            {
                return m_liPossibleValues;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Cell> Peers
        {
            get
            {
                return m_lcPeers;
            }
        }
    }
}
