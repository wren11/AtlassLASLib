using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlass.LAS.Lib.Types
{
    public class ArraySize
    {
        public Int32 Rows { get; private set; }
        public Int32 Cols { get; private set; }

        public ArraySize()
            : this(0, 0)
        {
        }

        public ArraySize(Int32 prmRows, Int32 prmCols)
        {
            Rows = prmRows;
            Cols = prmCols;
        }
    }

    public class ArrayElement
    {
        public Int32 Row { get; set; }
        public Int32 Col { get; set; }

        public ArrayElement()
            : this(0, 0)
        {
        }

        public ArrayElement(Int32 prmRow, Int32 prmCol)
        {
            Row = prmRow;
            Col = prmCol;
        }
    }

    [Serializable]
    public class ArrayMap<T> : IEnumerable<T>
    {
        [NonSerialized]
        private ArraySize m_Size;
        private T[] m_Items;

        public ArrayMap()
        { }

        public ArrayMap(int prmRows, int prmCols)
            : this(new ArraySize(prmRows, prmCols))
        { }

        public ArrayMap(ArraySize size)
            : this()
        {
            this.ArraySize = size;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in m_Items)
                yield return item;
        }

        public int GetItemIndex(int row, int col)
        {
            if (row < 0 || row >= this.ArraySize.Rows)
                throw new IndexOutOfRangeException("Row is out of range");
            else if (col < 0 || col >= this.ArraySize.Cols)
                throw new IndexOutOfRangeException("Col is out of range");

            return row * this.ArraySize.Cols + col;
        }

        public int GetItemIndex(ArrayElement point)
        {
            return this.GetItemIndex(point.Row, point.Col);
        }

        public ArrayElement GetItemLocation(int index)
        {
            ArrayElement point;

            if (index < 0 || index >= m_Items.Length)
                throw new IndexOutOfRangeException("Index is out of range");

            point = new ArrayElement();
            point.Row = index / this.ArraySize.Cols;
            point.Col = index - (point.Row * this.ArraySize.Rows);

            return point;
        }

        public int Count { get { return m_Items.Length; } }

        public ArraySize ArraySize
        {
            get { return m_Size; }
            set
            {
                m_Size = value;
                m_Items = new T[m_Size.Cols * m_Size.Rows];
            }
        }

        public T this[ArrayElement location]
        {
            get { return this[location.Row, location.Col]; }
            set { this[location.Row, location.Col] = value; }
        }

        public T this[int row, int col]
        {
            get { return this[this.GetItemIndex(row, col)]; }
            set { this[this.GetItemIndex(row, col)] = value; }
        }

        public T this[int index]
        {
            get { return m_Items[index]; }
            set { m_Items[index] = value; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public ArrayMap<T> GetItems(Int32 prmStartRow, Int32 prmStartCol, Int32 prmEndRow, Int32 prmEndCol)
        {
            if (prmStartRow < 0 || prmStartCol < 0 || prmStartRow >= prmEndRow || prmStartCol >= prmEndCol || prmEndRow >= m_Size.Rows || prmEndCol >= m_Size.Cols)
                throw new ArgumentOutOfRangeException();

            ArrayMap<T> points = new ArrayMap<T>(prmEndRow - prmStartRow + 1, prmEndCol - prmStartCol + 1);
            for (int i = prmStartRow; i <= prmEndRow; i++)
                for (int j = prmStartCol; j <= prmEndCol; j++)
                    points[i - prmStartRow, j - prmStartCol] = this[i, j];
            return points;
        }
    }
}
