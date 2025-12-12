namespace Ryanair.Tests;

[TestFixture]
internal sealed class AlgorithmsTests
{
    [TestCase(new double[] { 48, 96, 144, 192, 240 }, 1, 240)]
    [TestCase(new double[] { 48, 96, 144, 192, 240, 288, 336 }, 3, 240)]
    [TestCase(new double[] { 48, 96, 144, 192, 240, 288, 336, 384, 432, 480 }, 10, 48)]
    public void FindKthBiggestDivisibleBy12And16_ValidInput_ReturnsCorrectValue
    (
        double[] numbersArray, 
        int k, 
        double expected
    )
    {
        var numbers = CastToNullableDoubleArray(numbersArray);
        
        var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value, Is.EqualTo(expected));
        });
    }

    [TestCase(new double[] { 0, 48, 96, 144 }, 4, 0)]
    [TestCase(new double[] { -48, -96, 48, 96, 144 }, 3, 48)]
    [TestCase(new double[] { 48, 48, 96, 96, 144, 144 }, 3, 96)]
    [TestCase(new double[] { -144, -96, -48, 0, 48, 96, 144 }, 4, 0)]
    public void FindKthBiggestDivisibleBy12And16_SpecialCases_ReturnsCorrectValue
    (
        double[] numbersArray, 
        int k, 
        double expected
    )
    {
        var numbers = CastToNullableDoubleArray(numbersArray);
        
        var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value, Is.EqualTo(expected));
        });
    }

    [TestCase(new double[] { 48, 96 }, 3)]
    [TestCase(new double[] { 12, 16, 24, 30, 50 }, 1)]
    [TestCase(new double[0], 1)]
    public void FindKthBiggestDivisibleBy12And16_NotEnoughNumbers_ReturnsNull
    (
        double[] numbersArray, 
        int k
    )
    {
        var numbers = CastToNullableDoubleArray(numbersArray);
        
        var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

        Assert.That(result.HasValue, Is.False);
    }

    [TestCaseSource(nameof(NullValuesTestCases))]
    public void FindKthBiggestDivisibleBy12And16_WithNullValues_IgnoresNulls
    (
        double?[] numbers, 
        int k, 
        double expected
    )
    {
        var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value, Is.EqualTo(expected));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-10)]
    public void FindKthBiggestDivisibleBy12And16_InvalidK_ThrowsArgumentException(int k)
    {
        var numbers = new double?[] { 48, 96, 144 };

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(ex!.ParamName, Is.EqualTo("k"));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        });
    }

    [Test]
    public void FindKthBiggestDivisibleBy12And16_NullSequence_ThrowsArgumentNullException()
    {
        double?[]? numbers = null;
        const int k = 1;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Algorithms.FindKthBiggestDivisibleBy12And16(numbers!, k);
        });
    }
    
    private static IEnumerable<TestCaseData> NullValuesTestCases()
    {
        yield return new TestCaseData
        (
            new double?[] { null, 48, null, 96, 144, null, 192 }, 
            2, 
            144.0
        ).SetName("WithNullValues_MultipleNulls_ReturnsCorrectValue");
        
        yield return new TestCaseData
        (
            new double?[] { null, null, 48, 96 }, 
            1, 
            96.0
        ).SetName("WithNullValues_LeadingNulls_ReturnsCorrectValue");
        
        yield return new TestCaseData
        (
            new double?[] { 48, 96, null, 144 }, 
            1, 
            144.0
        ).SetName("WithNullValues_TrailingNull_ReturnsCorrectValue");
        
        yield return new TestCaseData
        (
            new double?[] { null, 0, null, 48, null, 96 }, 
            3, 
            0.0
        ).SetName("WithNullValues_IncludesZero_ReturnsCorrectValue");
    }

    private static double?[] CastToNullableDoubleArray(double[] numbers)
    {
        return numbers.Cast<double?>().ToArray();
    }
}
