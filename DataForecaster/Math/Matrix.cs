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
        public void Inverse()
        {
            int h = 0, k = 0;
            int m = RowsNumber;
            int n = ColsNumber;

            while (h <= m && k <= n)
            {
                int iMax = ArgMax(h, m, k);
                if (Convert.ToDouble(_matrix[iMax, k]) == 0)
                {
                    k++;
                }
                else
                {
                    SwapRows(h, iMax);

                    foreach (var i in Enumerable.Range(h + 1, m))
                    {
                        double f = Convert.ToDouble(_matrix[i, k]) / Convert.ToDouble(_matrix[h, k]);
                        _matrix[i, k] = default(T);

                        foreach (var j in Enumerable.Range(k + 1, n))
                        {
                            _matrix[i, j] = Convert.ToDouble(_matrix[i, j]) - Convert.ToDouble(_matrix[h, j]) * f;
                        }
                    }

                    h++;
                    k++;
                }
            }
        }

        private int ArgMax(int iFrom, int iTo, int j)
        {
            var iMax = iFrom;
            var max = Math.Abs(Convert.ToDouble(_matrix[iMax, j]));
            foreach (var i in Enumerable.Range(iFrom + 1, iTo))
            {
                var candidate = Math.Abs(Convert.ToDouble(_matrix[i, j]));
                if (candidate > max)
                {
                    max = candidate;
                    iMax = i;
                }
            }

            return iMax;
        }

        /*
            // h := 1 /* Initialization of the pivot row */
            // k := 1 /* Initialization of the pivot column */
            // while  h ≤ m and k ≤ n
            /* Find the k-th pivot: */
            // i_max := argmax(i = h...m, abs(A[i, k]))
            // if A[i_max, k] = 0
            /* No pivot in this column, pass to next column */
            // k := k+1
            // else
            // swap rows(h, i_max)
            /* Do for all rows below pivot: */
            // for i = h + 1 ... m:
            // f := A[i, k] / A[h, k]
            /* Fill with zeros the lower part of pivot column: */
            // A[i, k]  := 0
            /* Do for all remaining elements in current row: */
            // for j = k + 1 ... n:
            // A[i, j] := A[i, j] - A[h, j] * f
            /* Increase pivot row and column */
            // h := h+1 
            // k := k+1
         
         
    }
}
