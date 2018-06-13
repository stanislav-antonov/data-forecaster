using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace DataForecaster
{
    public class Vector<T> : IEnumerable<T>, ICloneable where T : IComparable<T>
    {
        private readonly T[] _vector;

        public int Length => _vector.Length;

        public Vector(int size)
        {
            _vector = new T[size];
        }

        public Vector(T[] vector)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Primitive type is expected");

            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            _vector = vector;
        }

        public T this[int index]
        {
            get { return _vector[index]; }
            set { _vector[index] = value; }
        }   

        public double Norm()
        {
            double result = 0.0D;
            foreach (T e in _vector) result += Math.Pow(Convert.ToDouble(e), 2);
            
            return Math.Sqrt(result);
        }

        public static Vector<double> operator /(Vector<T> v1, double scalar)
        {
            var result = new Vector<double>(v1.Length);
            for (int i = 0; i < v1.Length; i++)
            {
                result[i] = Convert.ToDouble(v1[i]) * (1 / scalar); 
            }
            
            return result;
        }

        public static Vector<double> operator *(Vector<T> v1, double scalar)
        {
            var result = new Vector<double>(v1.Length);
            for (int i = 0; i < v1.Length; i++)
            {
                result[i] = Convert.ToDouble(v1[i]) * scalar;
            }

            return result;
        }

        // http://mathworld.wolfram.com/DotProduct.html
        public static double operator *(Vector<T> v1, Vector<T> v2)
        {
            double result = 0D;
            for (int i = 0; i < v1.Length; i++)
                result += Convert.ToDouble(v1[i]) * Convert.ToDouble(v2[i]);
            
            return result;
        }

        public static Vector<double> operator -(Vector<T> v1, Vector<T> v2)
        {
            var result = new Vector<double>(v1.Length);
            for (int i = 0; i < v1.Length; i++)
            {
                result[i] = Convert.ToDouble(v1[i]) - Convert.ToDouble(v2[i]);
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _vector.Cast<T>().GetEnumerator();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
