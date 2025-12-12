using Ryanair;

var numbers = new double?[] { 48, 96, 144, 12, 16, 24, 192, 240, 288, 336, null, 0, -48 };

const int k = 2;

var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

Console.WriteLine(result.HasValue
    ? $"The {k}-th biggest number divisible by 12 and 16 is: {result.Value}"
    : $"No {k}-th biggest number found (not enough divisible numbers)");
