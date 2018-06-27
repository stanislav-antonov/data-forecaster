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
            {
                result[i] = _matrix[i, j];
            }

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
            T[,] result = new T[n, m];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[j, i] = _matrix[i, j];
                }
            }

            return new Matrix<T>(result);
        }

        public static Matrix<double> operator -(Matrix<T> matrixA, Matrix<T> matrixB)
        {
            int m = matrixA.RowsNumber;
            int n = matrixB.ColsNumber;
            var result = new Matrix<double>(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = Convert.ToDouble(matrixA[i, j]) - Convert.ToDouble(matrixB[i, j]);
                }
            }

            return result;
        }

        public static Matrix<double> operator /(Matrix<T> matrix, double value)
        {
            int m = matrix.RowsNumber;
            int n = matrix.ColsNumber;
            var result = new Matrix<double>(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = Convert.ToDouble(matrix[i, j]) / value;
                }
            }

            return result;
        }

        public static Matrix<double> operator *(Matrix<T> matrix, double value)
        {
            int m = matrix.RowsNumber;
            int n = matrix.ColsNumber;
            var result = new Matrix<double>(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = Convert.ToDouble(matrix[i, j]) * value;
                }
            }

            return result;
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

        // https://mathinsight.org/matrix_vector_multiplication
        public static Matrix<double> operator *(Matrix<T> matrixA, Matrix<T> matrixB)
        {
            int aRowsNumber = matrixA.RowsNumber;
            int aColsNumber = matrixA.ColsNumber;
            int bRowsNumber = matrixB.RowsNumber;
            int bColsNumber = matrixB.ColsNumber;

            if (aColsNumber != bRowsNumber)
            {
                throw new Exception("Non-conformable matrices");
            }

            var result = new Matrix<double>(aRowsNumber, bColsNumber);

            for (int i = 0; i < aRowsNumber; ++i) // each row of A
            {
                for (int j = 0; j < bColsNumber; ++j) // each col of B
                {
                    for (int k = 0; k < aColsNumber; ++k)
                    {
                        result[i, j] += Convert.ToDouble(matrixA[i, k]) * Convert.ToDouble(matrixB[k, j]);
                    }
                }
            }

            return result;
        }

        public Matrix<double> Inverse()
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = ColsNumber;
            var result = Clone() as Matrix<double>;
            int toggle = result.CroutProcess(out Matrix<double> lum, out int[] perm);
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

        public double Determinant()
        {
            int toggle = (this as Matrix<double>).CroutProcess(out Matrix<double> lum, out int[] perm);
            double result = toggle;

            for (int i = 0; i < lum.RowsNumber; ++i)
            {
                result *= lum[i, i];
            }

            return result;
        }

        public void Fill(T value)
        {
            int m = RowsNumber;
            int n = ColsNumber;

            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    this[i, j] = value;
                }
            }
        }

        public void FillAsIdentity()
        {
            int n = ColsNumber;

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    (this as Matrix<double>)[i, j] = (i == j) ? 1 : 0;
                }
            }
        }

        public override string ToString()
        {
            int m = RowsNumber;
            int n = ColsNumber;
            string result = "";

            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    result += _matrix[i, j].ToString().PadLeft(8) + " ";
                }
                    
                result += Environment.NewLine;
            }

            return result;
        }

        public object Clone()
        {
            return new Matrix<T>(_matrix);
        }
    }
}
