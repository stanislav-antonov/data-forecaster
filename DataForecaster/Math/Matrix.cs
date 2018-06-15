using System;
using System.Runtime.InteropServices;

namespace DataForecaster
{
    public class Matrix<T> where T : IComparable<T>
    {
        // m - rows total
        // n - columns total
        // i - row index
        // j - column index

        private readonly T[,] _matrix;

        public int RowsNumber => _matrix.GetLength(0);
        public int ColsNumber => _matrix.GetLength(1);

        public Matrix(int m, int n)
        {
            _matrix = new T[m, n];
        }

        public Matrix(T[,] matrix)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Primitive type is expected");

            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            _matrix = matrix;
        }

        public T this[int i, int j]
        {
            get { return _matrix[i, j]; }
            set { _matrix[i, j] = value; }
        }

        public T[] GetColumn(int j)
        {
            int rowsNumber = RowsNumber;
            var result = new T[rowsNumber];

            for (int i = 0; i < rowsNumber; i++)
                result[i] = _matrix[i, j];

            return result;
        }

        public Vector<T> GetColumnVector(int j)
        {
            return new Vector<T>(GetColumn(j));
        }

        public T[] GetRow(int i)
        {
            int colsNumber = ColsNumber;
            var result = new T[colsNumber];
            int size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(_matrix, i * colsNumber * size, result, 0, colsNumber * size);

            return result;
        }

        public Vector<T> GetRowVector(int i)
        {
            return new Vector<T>(GetRow(i));
        }

        public void SetColumn(Vector<T> column, int j)
        {
            for (int i = 0; i < RowsNumber; i++)
            {
                _matrix[i, j] = column[i];
            }
        }

        public void SetRow(Vector<T> row, int i)
        {
            for (int j = 0; j < ColsNumber; j++)
            {
                _matrix[i, j] = row[j];
            }
        }
    }
}
