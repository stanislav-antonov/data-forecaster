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

        private T[,] _matrix;

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

        // https://en.wikipedia.org/wiki/Transpose
        public void Transpose()
        {
            int m = RowsNumber;
            int n = ColsNumber;
            
            T[,] transposed = new T[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    transposed[i, j] = _matrix[j, i];
                }
            }

            _matrix = transposed;
        }

        // https://mathinsight.org/matrix_vector_multiplication
        public static Vector<double> operator *(Matrix<T> matrix, Vector<T> vector)
        {
            int m = matrix.RowsNumber;
            int n = matrix.ColsNumber;
            var result = new Vector<double>(m);

            for (int i = 0; i < m; i++)
            {
                double s = 0.0D;
                for (int j = 0; j < n; j++)
                {
                    s += Convert.ToDouble(matrix[i, j]) * Convert.ToDouble(vector[i]);
                }

                result[i] = s;
            }

            return result;
        } 
    }
}
