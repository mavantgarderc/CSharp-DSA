using static Modules.DataStructures.Vectors;

namespace Tests.DataStructureTests
{
    public class VectorTests
    {
        private const double Tolerance = 1e-6;

        [Theory]
        [InlineData(new double[] { 3, 4 }, 5)]
        [InlineData(new double[] { 1, 1, 1, 1 }, 2)]
        [InlineData(new double[] { 0, 0, 0 }, 0)]
        [InlineData(new double[] { -3, -4 }, 5)]
        public void Magnitude_CalculatesCorrectly(double[] components, double expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.Magnitude();

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 3, 4 }, new double[] { 0.6, 0.8 })]
        [InlineData(new double[] { 0, 5 }, new double[] { 0, 1 })]
        [InlineData(new double[] { -3, -4 }, new double[] { -0.6, -0.8 })]
        public void Normalize_ReturnsUnitVector(double[] components, double[] expected)
        {
            var vector = new Vector<double>(components);
            var expectedVector = new Vector<double>(expected);

            var result = vector.Normalize();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
            Assert.Equal(1.0, result.Magnitude(), Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 2 }, new double[] { 3, 4 }, 11)]
        [InlineData(new double[] { 0, 0 }, new double[] { 5, 6 }, 0)]
        [InlineData(new double[] { -1, -2 }, new double[] { 3, 4 }, -11)]
        [InlineData(new double[] { 1.5, 2.5 }, new double[] { 3.5, 4.5 }, 16.5)]
        public void DotProduct_CalculatesCorrectly(double[] v1, double[] v2, double expected)
        {
            var vector1 = new Vector<double>(v1);
            var vector2 = new Vector<double>(v2);

            var result = vector1.DotProduct(vector2);

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, new double[] { 0, 0, 1 })]
        [InlineData(new double[] { 2, 3, 4 }, new double[] { 5, 6, 7 }, new double[] { -3, 6, -3 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 1, 2, 3 }, new double[] { 0, 0, 0 })]
        [InlineData(new double[] { -1, -2, -3 }, new double[] { 4, 5, 6 }, new double[] { 3, -6, 3 })]
        public void CrossProduct_CalculatesCorrectly(double[] v1, double[] v2, double[] expected)
        {
            var vector1 = new Vector<double>(v1);
            var vector2 = new Vector<double>(v2);
            var expectedVector = new Vector<double>(expected);

            var result = vector1.CrossProduct(vector2);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2 }, new double[] { 3, 4 }, new double[] { 4, 6 })]
        [InlineData(new double[] { 0, 0 }, new double[] { 5, 6 }, new double[] { 5, 6 })]
        [InlineData(new double[] { -1, -2 }, new double[] { 3, 4 }, new double[] { 2, 2 })]
        [InlineData(new double[] { 1.5, 2.5 }, new double[] { 3.5, 4.5 }, new double[] { 5, 7 })]
        public void Add_ReturnsSum(double[] v1, double[] v2, double[] expected)
        {
            var vector1 = new Vector<double>(v1);
            var vector2 = new Vector<double>(v2);
            var expectedVector = new Vector<double>(expected);

            var result = vector1 + vector2;

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2 }, 2, new double[] { 2, 4 })]
        [InlineData(new double[] { 3, 4 }, 0, new double[] { 0, 0 })]
        [InlineData(new double[] { -1, -2 }, 3, new double[] { -3, -6 })]
        [InlineData(new double[] { 1.5, 2.5 }, 2, new double[] { 3, 5 })]
        public void Scale_ReturnsScaledVector(double[] components, double scalar, double[] expected)
        {
            var vector = new Vector<double>(components);
            var expectedVector = new Vector<double>(expected);

            var result = vector.Scale(scalar);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3, 4 }, 2.5)]
        [InlineData(new double[] { 5, 5, 5, 5 }, 5)]
        [InlineData(new double[] { -1, -2, -3 }, -2)]
        [InlineData(new double[] { 1.5, 2.5, 3.5 }, 2.5)]
        public void Mean_CalculatesAverage(double[] components, double expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.Mean();

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3, 4, 5 }, 3)]
        [InlineData(new double[] { 1, 2, 3, 4 }, 2.5)]
        [InlineData(new double[] { 5, 1, 3 }, 3)]
        [InlineData(new double[] { -5, -1, 3 }, -1)]
        public void Median_CalculatesMiddleValue(double[] components, double expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.Median();

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 2, 3 }, 2)]
        [InlineData(new double[] { 1, 1, 2, 2 }, 1)]
        [InlineData(new double[] { 5 }, 5)]
        [InlineData(new double[] { 1.1, 1.1, 2.2 }, 1.1)]
        public void Mode_ReturnsMostFrequent(double[] components, double expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.Mode();

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 0 }, Math.PI / 2, new double[] { 0, 1 })]
        [InlineData(new double[] { 0, 1 }, Math.PI, new double[] { 0, -1 })]
        [InlineData(new double[] { 1, 0 }, 0, new double[] { 1, 0 })]
        public void Rotate2D_RotatesVector(double[] components, double angle, double[] expected)
        {
            var vector = new Vector<double>(components);
            var expectedVector = new Vector<double>(expected);

            var result = vector.Rotate2D(angle);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, Math.PI / 2, new double[] { 0, 0, -1 })]
        [InlineData(new double[] { 0, 1, 0 }, new double[] { 1, 0, 0 }, Math.PI, new double[] { 0, -1, 0 })]
        [InlineData(new double[] { 0, 0, 1 }, new double[] { 0, 1, 0 }, Math.PI, new double[] { 0, 0, -1 })]
        [InlineData(new double[] { 1, 0, 0 }, new double[] { 1, 1, 1 }, 2 * Math.PI / 3, new double[] { 0, 1, 0 })]
        public void Rotate3D_RotatesVector(double[] components, double[] axis, double angle, double[] expected)
        {
            var vector = new Vector<double>(components);
            var axisVector = new Vector<double>(axis).Normalize();
            var expectedVector = new Vector<double>(expected);

            var result = vector.Rotate3D(axisVector, angle);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2 }, new double[] { 1, 2 }, 0.01, true)]
        [InlineData(new double[] { 1, 2 }, new double[] { 1.01, 2.01 }, 0.1, true)]
        [InlineData(new double[] { 1, 2 }, new double[] { 3, 4 }, 0.01, false)]
        [InlineData(new double[] { 1, 2 }, new double[] { 1, 2, 3 }, 0.01, false)]
        public void Equals_ComparesVectors(double[] v1, double[] v2, double tolerance, bool expected)
        {
            var vector1 = new Vector<double>(v1);
            var vector2 = new Vector<double>(v2);

            var result = vector1.Equals(vector2, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, 0.01, true)]
        [InlineData(new double[] { 0.001, -0.001 }, 0.01, true)]
        [InlineData(new double[] { 1, 2 }, 0.01, false)]
        [InlineData(new double[] { 0.1, 0 }, 0.01, false)]
        public void IsZeroVector_DetectsZero(double[] components, double tolerance, bool expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.IsZeroVector(tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3 }, new double[] { 2, 4, 6 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 })]
        [InlineData(new double[] { -1, -2 }, new double[] { -2, -4 })]
        [InlineData(new double[] { 1.5, 2.5 }, new double[] { 3, 5 })]
        public void Map_TransformsElements(double[] components, double[] expected)
        {
            var vector = new Vector<double>(components);
            var expectedVector = new Vector<double>(expected);

            var result = vector.Map(x => x * 2);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3, 4 }, 10)]
        [InlineData(new double[] { 5 }, 5)]
        [InlineData(new double[] { -1, 1 }, 0)]
        [InlineData(new double[] { 1.5, 2.5 }, 4)]
        public void Reduce_AggregatesValues(double[] components, double expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.Reduce((a, b) => a + b);

            Assert.Equal(expected, result, Tolerance);
        }

        [Theory]
        [InlineData(new double[] { 1, 2 })]
        [InlineData(new double[] { 3 })]
        [InlineData(new double[] { 4, 5, 6 })]
        [InlineData(new double[] { 0, 0, 0 })]
        public void ToColumnMatrix_CreatesMatrix(double[] components)
        {
            var vector = new Vector<double>(components);

            var matrix = vector.ToColumnMatrix();

            Assert.Equal(components.Length, matrix.GetLength(0));
            Assert.Equal(1, matrix.GetLength(1));

            for (int i = 0; i < components.Length; i++)
            {
                Assert.Equal(components[i], matrix[i, 0], Tolerance);
            }
        }

        [Fact]
        public void AsSpan_ReturnsCorrectView()
        {
            var components = new double[] { 1, 2, 3 };
            var vector = new Vector<double>(components);

            var span = vector.AsSpan();

            Assert.Equal(components.Length, span.Length);
            for (int i = 0; i < components.Length; i++)
            {
                Assert.Equal(components[i], span[i], Tolerance);
            }
        }

        [Fact]
        public void Swap_ExchangesVectors()
        {
            var v1 = new Vector<double>(1, 2);
            var v2 = new Vector<double>(3, 4);
            var originalV1 = v1;
            var originalV2 = v2;

            v1.Swap(ref v2);

            Assert.Equal(originalV1, v2);
            Assert.Equal(originalV2, v1);
        }

        [Theory]
        [InlineData(new double[] { 1, 2 }, new double[] { 3, 4 }, new double[] { 3, 4, 6, 8 })]
        [InlineData(new double[] { 1 }, new double[] { 2 }, new double[] { 2 })]
        [InlineData(new double[] { 0, 0 }, new double[] { 1, 2 }, new double[] { 0, 0, 0, 0 })]
        [InlineData(new double[] { -1, -2 }, new double[] { 3, 4 }, new double[] { -3, -4, -6, -8 })]
        public void KroneckerProduct_CalculatesCorrectly(double[] v1, double[] v2, double[] expected)
        {
            var vector1 = new Vector<double>(v1);
            var vector2 = new Vector<double>(v2);
            var expectedVector = new Vector<double>(expected);

            var result = vector1.KroneckerProduct(vector2);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expectedVector[i], result[i], Tolerance);
            }
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3 }, "1.0000, 2.0000, 3.0000")]
        [InlineData(new double[] { 0.123456, 0.654321 }, "0.1235, 0.6543")]
        [InlineData(new double[] { 1 }, "1.0000")]
        [InlineData(new double[] { -1.5, 2.5 }, "-1.5000, 2.5000")]
        public void ToDebugString_FormatsCorrectly(double[] components, string expected)
        {
            var vector = new Vector<double>(components);

            var result = vector.ToDebugString();

            Assert.Equal(expected, result);
        }
    }
}
