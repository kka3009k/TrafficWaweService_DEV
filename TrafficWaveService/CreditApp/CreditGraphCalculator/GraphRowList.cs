
using System.Collections;
using System.Collections.Generic;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator
{
    /// <summary>
    /// Список для хранения списка графика
    /// </summary>
    public class GraphRowList : IList<GraphRow>
    {
        private IList<GraphRow> rowList = new List<GraphRow>();

        public GraphRow this[int index]
        {
            get
            {
                return rowList[index];
            }

            set
            {
                rowList[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return rowList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return rowList.IsReadOnly;
            }
        }
        //private GraphRow _lastRow;
        //private GraphRow lastRow {
        //    get { return _lastRow; }
        //    set {
        //        if(_lastRow != null)
        //        {
        //            _lastRow.IsLast = false;
        //        }
        //        _lastRow = value;
        //        if (_lastRow != null)
        //            _lastRow.IsLast = true;
        //    }
        //}

        ///// <summary>
        ///// Последняя строка в графике
        ///// </summary>
        //public GraphRow LastRow
        //{
        //    get
        //    {
        //        return lastRow;
        //    }
        //}

        public void Add(GraphRow item)
        {
            if (rowList.Count > 0 && item != null)
            {
                item.UpperRow = rowList[rowList.Count - 1];
            }
            rowList.Add(item);
        }

        public void Clear()
        {
            rowList.Clear();
        }

        public bool Contains(GraphRow item)
        {
            return rowList.Contains(item);
        }

        public void CopyTo(GraphRow[] array, int arrayIndex)
        {
            rowList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GraphRow> GetEnumerator()
        {
            return rowList.GetEnumerator();
        }

        public int IndexOf(GraphRow item)
        {
            return rowList.IndexOf(item);
        }

        public void Insert(int index, GraphRow item)
        {
            if (item != null)
            {
                GraphRow row = rowList[index];
                if (row != null)
                    row.UpperRow = item;
            }
            rowList.Insert(index, item);
        }

        public bool Remove(GraphRow item)
        {
            if (rowList.Contains(item))
            {
                int index = rowList.IndexOf(item);
                if (index > 0)
                {
                    GraphRow upperRow = rowList[index - 1];
                    if (upperRow != null)
                        upperRow.BottomRow = rowList.Count > index + 1 ? rowList[index + 1] : null;
                }
            }
            return rowList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (rowList.Count > index + 1)
                rowList.Remove(rowList[index]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rowList.GetEnumerator();
        }

        public void AddRange(IEnumerable<GraphRow> range)
        {
            foreach (GraphRow r in range)
            {
                rowList.Add(r);
            }
        }
    }
}