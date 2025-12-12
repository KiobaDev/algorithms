using Ryanair;

DemonstrateKthBiggestDivisible();

Console.WriteLine();

await DemonstrateSharedMemoryContainerAsync();

return;

static void DemonstrateKthBiggestDivisible()
{
    Console.WriteLine("=== Algorithms: Find K-th Biggest Divisible by 12 and 16 ===\n");
    
    var numbers = new double?[] { 48, 96, 144, 12, 16, 24, 192, 240, 288, 336, null, 0, -48 };
    const int k = 3;
    
    Console.WriteLine($"Input numbers: {string.Join(", ", numbers.Where(n => n.HasValue).Select(n => n!.Value))}");
    Console.WriteLine($"Finding {k}-th biggest number divisible by 12 and 16...\n");
    
    var result = Algorithms.FindKthBiggestDivisibleBy12And16(numbers, k);

    Console.WriteLine
    (
        result.HasValue
            ? $"✓ The {k}-th biggest number divisible by 12 and 16 is: {result.Value}"
            : $"✗ No {k}-th biggest number found (not enough divisible numbers)"
    );

}

static async Task DemonstrateSharedMemoryContainerAsync()
{
    Console.WriteLine("=== SharedMemoryContainer: Thread-Safe FIFO Queue ===\n");
    
    Console.WriteLine("Thread-Safety Demonstration");
    Console.WriteLine("────────────────────────────────────\n");
    
    const int itemsPerThread = 100;
    const int threadCount = 5;
    
    var container = new SharedMemoryContainer();
    var allAddedItems = new HashSet<string>();
    var retrievedItems = new System.Collections.Concurrent.ConcurrentBag<string>();
    var lockObject = new Lock();
    
    Console.WriteLine($"Running {threadCount} producer tasks and {threadCount} consumer tasks concurrently...");
    Console.WriteLine($"Each producer adds {itemsPerThread} items.\n");
    
    var producerTasks = new List<Task>();
    
    for (var i = 0; i < threadCount; i++)
    {
        var producerId = i;
        
        var task = Task.Run(() =>
        {
            for (var j = 0; j < itemsPerThread; j++)
            {
                var item = $"Producer{producerId}_Item{j}";
                
                lock (lockObject)
                {
                    allAddedItems.Add(item);
                }
                
                container.Add([item]);
            }
        });
        
        producerTasks.Add(task);
    }
    
    var consumerTasks = new List<Task>();
    var addCompleted = new TaskCompletionSource();
    
    for (var i = 0; i < threadCount; i++)
    {
        var task = Task.Run(async () =>
        {
            while (!addCompleted.Task.IsCompleted || container.Count > 0)
            {
                var item = container.Get();
                
                if (item != null)
                {
                    retrievedItems.Add(item);
                }
                else
                {
                    await Task.Delay(1);
                }
            }
        });
        
        consumerTasks.Add(task);
    }
    
    await Task.WhenAll(producerTasks);
    
    addCompleted.SetResult();
    
    await Task.WhenAll(consumerTasks);
    
    var totalAdded = allAddedItems.Count;
    var totalRetrieved = retrievedItems.Count;
    var uniqueRetrieved = retrievedItems.Distinct().Count();
    var noDuplicates = uniqueRetrieved == totalRetrieved;
    var noDataLoss = totalRetrieved == totalAdded;
    var allItemsMatch = retrievedItems.ToHashSet().SetEquals(allAddedItems);
    
    Console.WriteLine("Results:");
    Console.WriteLine($"  Items added:     { totalAdded }");
    Console.WriteLine($"  Items retrieved: { totalRetrieved }");
    Console.WriteLine($"  Unique retrieved: { uniqueRetrieved }");
    Console.WriteLine($"  Container empty: { (container.Count == 0 ? "✓ YES" : "✗ NO") }");
    Console.WriteLine($"  No data loss:    { (noDataLoss ? "✓ YES" : "✗ NO") }");
    Console.WriteLine($"  No duplicates:   { (noDuplicates ? "✓ YES" : "✗ NO") }");
    Console.WriteLine($"  All items match: { (allItemsMatch ? "✓ YES" : "✗ NO") }");
    
    var threadSafe = noDataLoss && noDuplicates && allItemsMatch && container.Count == 0;
    Console.WriteLine($"\n✓ Thread-safety: { (threadSafe ? "VERIFIED" : "FAILED") }");
}
