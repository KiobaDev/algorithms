namespace Ryanair;

public static class Algorithms
{
    private const double Epsilon = 1e-9;
    
    public static double? FindKthBiggestDivisibleBy12And16(double?[] numbers, int k)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        if (k <= 0)
        {
            throw new ArgumentException("k must be positive", nameof(k));
        }

        const double divisor = 48.0;

        var minHeap = new PriorityQueue<double, double>();

        foreach (var num in numbers)
        {
            if (!num.HasValue)
                continue;

            var value = num.Value;
                
            var remainder = Math.Abs(value % divisor);

            if (remainder > divisor / 2)
                remainder = divisor - remainder;

            if (remainder >= Epsilon)
            {
                continue;
            }

            if (minHeap.Count < k)
            {
                minHeap.Enqueue(value, value);
            }
            else
            {
                if (value > minHeap.Peek())
                {
                    minHeap.Dequeue();
                    minHeap.Enqueue(value, value);
                }
            }
        }

        if (minHeap.Count < k)
        {
            return null;
        }

        return minHeap.Peek();
    }
}