using System.Numerics;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Core vector mathematics
    ///     - Arithmetic operations
    ///     - Statistica nalysis
    ///     - Geometric transformation
    /// Key Practices:
    ///     - Generic numeric support
    ///     - Dimension validation
    ///     - Tolerance-based floating-point
    ///     - Functional programming pattern
    /// </summary>
    public class Vectors
    {
        public struct Vector<T>(params T[] components) where T : INumber<T>
        {
            private readonly T[] _components = components?.ToArray() ?? throw new ArgumentNullException(nameof(components));

            // public Vector(params T[] components)
            // {
            //     _components = components?.ToArray() ?? throw new ArgumentNullException(nameof(components));
            // }

            public readonly int Dimension => _components.Length;
            public readonly T this[int index] => _components[index];

            public readonly T Magnitude()
            {
                T sum = T.Zero;

                foreach (var c in _components)
                {
                    sum += c * c;
                }

                return T.CreateTruncating(Math.Sqrt(Convert.ToDouble(sum)));
            }

            public readonly Vector<T> Normalize()
            {
                T mag = Magnitude();

                if (mag == T.Zero)
                {
                    throw new InvalidOperationException("normalize a zero vector? :/");
                }

                return Scale(T.One / mag);
            }

            public readonly T DotProduct(Vector<T> other)
            {
                var self = this;

                return ValidateDimensions(other, () => 
                        {
                        T result = T.Zero;

                        for (int i = 0; i < self.Dimension; i++)
                        {
                        result += self._components[i] * other[i];
                        }

                        return result;
                        });
            }

            public readonly Vector<T> CrossProduct(Vector<T> other)
            {
                ValidateDimensions(3);
                other.ValidateDimensions(3);

                return new Vector<T>(
                        _components[1] * other[2] - _components[2] * other[1],
                        _components[2] * other[0] - _components[0] * other[2],
                        _components[0] * other[1] - _components[1] * other[0]
                        );
            }

            public readonly T DistanceTo(Vector<T> other) => (this - other).Magnitude();

            public readonly double AngleBetween(Vector<T> other)
            {
                T magProduct = Magnitude() * other.Magnitude();

                if (magProduct == T.Zero)
                {
                    throw new InvalidOperationException("Cannot compute angle with zero vector");
                }

                double cosTheta = Convert.ToDouble(DotProduct(other)) / Convert.ToDouble(magProduct);

                return Math.Acos(Math.Clamp(cosTheta, -1.0, 1.0));
            }

            public readonly bool IsOrthogonalTo(Vector<T> other, T tolerance)
            {
                return T.Abs(DotProduct(other)) <= tolerance;
            }

            public readonly bool IsParallelTo(Vector<T> other, T tolerance)
            {
                T magProduct = Magnitude() * other.Magnitude();

                if (magProduct == T.Zero) 
                {
                    return true;
                }

                return T.Abs(T.Abs(DotProduct(other)) - magProduct) <= tolerance;
            }

            public readonly Vector<T> ProjectOnto(Vector<T> basis)
            {
                T basisMagSq = basis.DotProduct(basis);

                if (basisMagSq == T.Zero)
                {
                    throw new ArgumentException("Cannot project onto zero vector");
                }

                return basis.Scale(DotProduct(basis) / basisMagSq);
            }

            public static Vector<T> operator +(Vector<T> a, Vector<T> b) => a.Add(b);
            public static Vector<T> operator -(Vector<T> a, Vector<T> b) => a.Subtract(b);

            public readonly Vector<T> Add(Vector<T> other) => BinaryOp(other, (a, b) => a + b);
            public readonly Vector<T> Subtract(Vector<T> other) => BinaryOp(other, (a, b) => a - b);
            public readonly Vector<T> Scale(T scalar) => Map(x => x * scalar);
            public readonly Vector<T> HadamardProduct(Vector<T> other) => BinaryOp(other, (a, b) => a * b);
            public readonly Vector<T> Negate() => Scale(-T.One);

            public readonly Vector<T> Clamp(T min, T max)
            {
                return Map(x => x < min ? min : x > max ? max : x);
            }

            public readonly Vector<T> Abs()
            {
                return Map(x => T.Abs(x));
            }

            public readonly T Mean()
            {
                if (_components.Length == 0) return T.Zero;

                T sum = T.Zero;

                foreach (var item in _components)
                {
                    sum += item;
                }

                return sum / T.CreateChecked(_components.Length);
            }

            public readonly T Median()
            {
                if (_components.Length == 0) return T.Zero;

                var sorted = _components.Order().ToArray();
                int mid = sorted.Length / 2;

                return sorted.Length % 2 != 0 ? sorted[mid] : 
                    (sorted[mid - 1] + sorted[mid]) / T.CreateChecked(2);
            }

            public readonly T Mode() => _components
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .First().Key;

            public readonly double Variance()
            {
                double mean = Convert.ToDouble(Mean());
                return _components.Average(x => Math.Pow(Convert.ToDouble(x) - mean, 2));
            }

            public readonly double StandardDeviation() => Math.Sqrt(Variance());

            public readonly Vector<double> ZScoreNormalize()
            {
                double mean = Convert.ToDouble(Mean());
                double std = StandardDeviation();

                return new Vector<double>(_components
                        .Select(x => std != 0 ? (Convert.ToDouble(x) - mean) / std : 0)
                        .ToArray());
            }

            public readonly T? Min()
            {
                if (_components.Length == 0)
                    return default;

                return _components.Min();
            }

            public readonly T? Max()
            {
                if (_components.Length == 0)
                    return default;

                return _components.Max();
            }

            public readonly T? Range()
            {
                var min = Min();
                var max = Max();

                if (min is null || max is null)
                    return default;

                return (min, max) switch
                {
                    (T minVal, T maxVal) => maxVal - minVal,
                    _ => default
                };
            }

            public readonly Vector<T> Rotate2D(double angleRadians)
            {
                ValidateDimensions(2);
                double cos = Math.Cos(angleRadians);
                double sin = Math.Sin(angleRadians);
                double x = Convert.ToDouble(_components[0]);
                double y = Convert.ToDouble(_components[1]);
                return new Vector<T>(
                        T.CreateTruncating(cos * x - sin * y),
                        T.CreateTruncating(sin * x + cos * y)
                        );
            }

            public readonly Vector<T> Rotate3D(Vector<T> axis, double angleRadians)
            {
                ValidateDimensions(3);
                axis = axis.Normalize();
                double cos = Math.Cos(angleRadians);
                double sin = Math.Sin(angleRadians);
                double oneMinusCos = 1 - cos;

                double ux = Convert.ToDouble(axis[0]);
                double uy = Convert.ToDouble(axis[1]);
                double uz = Convert.ToDouble(axis[2]);

                double x = Convert.ToDouble(_components[0]);
                double y = Convert.ToDouble(_components[1]);
                double z = Convert.ToDouble(_components[2]);

                double dot = ux*x + uy*y + uz*z;

                return new Vector<T>(
                        T.CreateTruncating(x*cos + (uy*z - uz*y)*sin + ux*dot*oneMinusCos),
                        T.CreateTruncating(y*cos + (uz*x - ux*z)*sin + uy*dot*oneMinusCos),
                        T.CreateTruncating(z*cos + (ux*y - uy*x)*sin + uz*dot*oneMinusCos)
                        );
            }

            public readonly Vector<T> ReflectAcross(Vector<T> normal)
            {
                return this - normal.Scale(T.CreateChecked(2) * DotProduct(normal) / normal.DotProduct(normal));
            }

            public readonly Vector<T> Translate(Vector<T> translation)
            {
                return this + translation;
            }

            public readonly Vector<T> Shear(double xShear, double yShear)
            {
                ValidateDimensions(2);

                return new Vector<T>(
                        _components[0] + T.CreateTruncating(xShear * Convert.ToDouble(_components[1])),
                        _components[1] + T.CreateTruncating(yShear * Convert.ToDouble(_components[0]))
                        );
            }

            public readonly Vector<T> ScaleNonUniform(T x, T y, T z)
            {
                ValidateDimensions(3);

                return new Vector<T>(
                        _components[0] * x,
                        _components[1] * y,
                        _components[2] * z
                        );
            }

            public readonly bool Equals(Vector<T> other, T tolerance)
            {
                if (Dimension != other.Dimension) return false;

                for (int i = 0; i < Dimension; i++)
                {
                    if (T.Abs(_components[i] - other[i]) > tolerance) 
                        return false;
                }

                return true;
            }

            public readonly bool IsZeroVector(T tolerance)
            {
                return _components.All(x => T.Abs(x) <= tolerance);
            }

            public readonly bool IsUnitVector(T tolerance)
            {
                return T.Abs(T.One - Magnitude()) <= tolerance;
            }

            public readonly bool HasNaNs()
            {
                return _components.Any(x =>
                        (x is double d && double.IsNaN(d)) ||
                        (x is float f && float.IsNaN(f)));
            }

            public readonly bool HasInfinities()
            {
                return _components.Any(x =>
                        (x is double d && double.IsInfinity(d)) ||
                        (x is float f && float.IsInfinity(f)));
            }

            public readonly Vector<T> Map(Func<T, T> func)
            {
                return new(_components.Select(func).ToArray());
            }

            public readonly Vector<T> Filter(Func<T, bool> predicate)
            {
                return new(_components.Where(predicate).ToArray());
            }

            public readonly T Reduce(Func<T, T, T> reducer)
            {
                return _components.Aggregate(reducer);
            }

            public readonly void ForEach(Action<T> action)
            {
                foreach (var item in _components) action(item);
            }

            public readonly TAccumulate Aggregate<TAccumulate>(
                    TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
            {
                return _components.Aggregate(seed, func);
            }

            public readonly Span<T> AsSpan() => new([.. _components]);
            public readonly T[] ToArray() => [.. _components];
            public readonly List<T> ToList() => [.. _components];
            public readonly void CopyTo(Span<T> destination)
            {
                _components.AsSpan().CopyTo(destination);
            }

            public void Swap(ref Vector<T> other)
            {
                (this, other) = (other, this);
            }

            public readonly void BenchmarkOperations(int iterations = 1_000_000)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                for (int i = 0; i < iterations; i++)
                {
                    _ = Magnitude();
                }
                sw.Stop();

                Console.WriteLine($"Magnitude x {iterations}: {sw.ElapsedMilliseconds}ms");
            }

            public readonly T[,] OuterProduct(Vector<T> other)
            {
                var result = new T[Dimension, other.Dimension];

                for (int i = 0; i < Dimension; i++)
                    for (int j = 0; j < other.Dimension; j++)
                        result[i, j] = _components[i] * other[j];

                return result;
            }

            public readonly Vector<T> KroneckerProduct(Vector<T> other)
            {
                var result = new T[Dimension * other.Dimension];

                for (int i = 0; i < Dimension; i++)
                {
                    for (int j = 0; j < other.Dimension; j++)
                    {
                        result[i * other.Dimension + j] = _components[i] * other[j];
                    }
                }

                return new Vector<T>(result);
            }

            public double Covariance(Vector<T> other)
            {
                var self = this;

                return ValidateDimensions(other, ComputeCovariance);

                double ComputeCovariance()
                {
                    double meanX = Convert.ToDouble(self.Mean());
                    double meanY = Convert.ToDouble(other.Mean());
                    double sum = 0;

                    for (int i = 0; i < self.Dimension; i++)
                    {
                        double diffX = Convert.ToDouble(self[i]) - meanX;
                        double diffY = Convert.ToDouble(other[i]) - meanY;
                        sum += diffX * diffY;
                    }

                    return sum / self.Dimension;
                }
            }

            public double Correlation(Vector<T> other)
            {
                return Covariance(other) / (StandardDeviation() * other.StandardDeviation());
            }

            public readonly T[,] ToColumnMatrix()
            {
                var matrix = new T[Dimension, 1];

                for (int i = 0; i < Dimension; i++)
                    matrix[i, 0] = _components[i];

                return matrix;
            }

            public readonly T[,] ToRowMatrix()
            {
                var matrix = new T[1, Dimension];

                for (int i = 0; i < Dimension; i++)
                    matrix[0, i] = _components[i];

                return matrix;
            }

            public readonly T[,] TransposeAsMatrix()
            {
                return ToRowMatrix();
            }

            public readonly string ToDebugString(int precision = 4)
            {
                return string.Join(", ",
                        _components.Select(x =>
                            Math.Round(Convert.ToDouble(x), precision).ToString($"F{precision}")));
            }

            public readonly void DumpToConsole()
            {
                Console.WriteLine(ToString());
            }

            public override readonly string ToString()
            {
                return $"Vector<{typeof(T).Name}>[{string.Join(", ", _components)}]";
            }

            public readonly void VisualizeAsBarChart(int maxWidth = 50)
            {
                if (_components.Length == 0) return;

                double max = Convert.ToDouble(Max());
                double min = Convert.ToDouble(Min());
                double range = max - min;

                if (range == 0) range = 1;

                foreach (var value in _components)
                {
                    double val = Convert.ToDouble(value);
                    double normalized = (val - min) / range;
                    int width = (int)(normalized * maxWidth);

                    Console.WriteLine(new string('■', width));
                }
            }

            public readonly int Checksum()
            {
                var hash = new HashCode();

                foreach (var item in _components)
                {
                    hash.Add(item);
                }

                return hash.ToHashCode();
            }


            private readonly Vector<T> BinaryOp(Vector<T> other, Func<T, T, T> op)
            {
                ValidateDimensions(other);
                var result = new T[Dimension];

                for (int i = 0; i < Dimension; i++)
                {
                    result[i] = op(_components[i], other[i]);
                }

                return new Vector<T>(result);
            }

            private readonly void ValidateDimensions(int expected)
            {
                if (Dimension != expected)
                {
                    throw new InvalidOperationException($"Operation requires vector dimension {expected}");
                }
            }

            private readonly void ValidateDimensions(Vector<T> other)
            {
                if (Dimension != other.Dimension)
                {
                    throw new ArgumentException("Vectors must have same dimensions");
                }
            }

            private readonly TResult ValidateDimensions<TResult>(Vector<T> other, Func<TResult> operation)
            {
                ValidateDimensions(other);

                return operation();
            }
        }
    }
}
