using System;
using System.Runtime.InteropServices;

namespace DataForecaster
{
    public class Matrix<T> : ICloneable where T : IComparable<T>
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
            _matrix = matrix.Clone() as T[,] ?? throw new ArgumentNullException(nameof(matrix));
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
        public Matrix<T> Transpose()
        {
            int m = RowsNumber;
            int n = ColsNumber;
            
            T[,] transposed = new T[n, m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    transposed[j, i] = _matrix[i, j];
                }
            }

            return new Matrix<T>(transposed);
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
                    s += Convert.ToDouble(matrix[i, j]) * Convert.ToDouble(vector[j]);
                }

                result[i] = s;
            }

            return result;
        }

        public Matrix<double> Inverse()
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            var n = ColsNumber;
            var result = Clone() as Matrix<double>;
            var toggle = result.CroutProcess(out Matrix<double> lum, out int[] perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    b[j] = (i == perm[j]) ? 1.0 : 0.0;
                }
                    
                double[] x = lum.CourtProcessHelper(b);
                for (int j = 0; j < n; ++j)
                {
                    result[j, i] = x[j];
                }
            }

            return result;
        }

        public object Clone()
        {
            return new Matrix<T>(_matrix);
        }
    }
}
