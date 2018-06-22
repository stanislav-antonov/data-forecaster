using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SwapRows(int i1, int i2)
        {
            for (var j = 0; j < ColsNumber; ++j)
            {
                T temp = _matrix[i2, j];
                _matrix[i2, j] = _matrix[i1, j];
                _matrix[i1, j] = temp;
            }
        }

        // Use Gaussian elimination approach
        // https://en.wikipedia.org/wiki/Gaussian_elimination
        public Matrix<double> Inverse()
        {
            int h = 0, k = 0;
            int m = RowsNumber;
            int n = ColsNumber;

            var inversed = (double[,])Convert.ChangeType(_matrix.Clone(), typeof(double[,]));

            while (h < m && k < n)
            {
                int iMax = ArgMax(h, m - 1, k, inversed);
                if (inversed[iMax, k] == 0)
                {
                    k++;
                }
                else
                {
                    SwapRows(h, iMax);

                    for (var i = h + 1; i < m; i++)
                    {
                        double f = inversed[i, k] / inversed[h, k];
                        inversed[i, k] = 0;

                        for (var j = k + 1; j < n; j++)
                        {
                            inversed[i, j] = inversed[i, j] - inversed[h, j] * f;
                        }
                    }

                    h++; k++;
                }
            }

            return new Matrix<double>(inversed);
        }

        private int ArgMax(int iFrom, int iTo, int j, double[,] matrix)
        {
            var iMax = iFrom;
            var max = Math.Abs(matrix[iMax, j]);

            for (var i = iFrom + 1; i <= iTo; i++)
            {
                var candidate = Math.Abs(matrix[i, j]);
                if (candidate > max)
                {
                    max = candidate;
                    iMax = i;
                }
            }

            return iMax;
        }
    }
}
