using System.Collections.Concurrent;

namespace Ryanair.Tests;

[TestFixture]
internal sealed class SharedMemoryContainerTests
{
    [Test]
    public void Get_EmptyContainer_ReturnsNull()
    {
        var container = new SharedMemoryContainer();

        var result = container.Get();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Get_MultipleItems_ReturnsInFifoOrder()
    {
        var container = new SharedMemoryContainer();
        
        container.Add(["ProcessOrder", "SendNotification", "UpdateInventory"]);

        Assert.Multiple(() =>
        {
            Assert.That(container.Get(), Is.EqualTo("ProcessOrder"));
            Assert.That(container.Get(), Is.EqualTo("SendNotification"));
            Assert.That(container.Get(), Is.EqualTo("UpdateInventory"));
            Assert.That(container.Get(), Is.Null);
        });
    }

    [Test]
    public void Add_NullCollection_ThrowsArgumentNullException()
    {
        var container = new SharedMemoryContainer();
        
        IEnumerable<string>? items = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            container.Add(items!);
        });
    }

    [Test]
    public async Task AddAndGet_ConcurrentOperations_ThreadSafe()
    {
        const int itemsToAdd = 5000;
        
        var container = new SharedMemoryContainer();
        var allAddedItems = new HashSet<string>();
        var retrievedItems = new ConcurrentBag<string>();
        var exceptions = new ConcurrentBag<Exception>();
        var lockObject = new Lock();
        var addCompleted = new TaskCompletionSource();
        var producerTasks = new List<Task>();
        
        for (var i = 0; i < 5; i++)
        {
            var threadId = i;
            
            var task = Task.Run(() =>
            {
                try
                {
                    for (var j = 0; j < itemsToAdd / 5; j++)
                    {
                        var item = $"Task_{threadId}_{j}";
                        
                        lock (lockObject)
                        {
                            allAddedItems.Add(item);
                        }
                        
                        container.Add([item]);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            
            producerTasks.Add(task);
        }

        var consumerTasks = new List<Task>();
        
        for (var i = 0; i < 5; i++)
        {
            var task = Task.Run(async () =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            
            consumerTasks.Add(task);
        }

        await Task.WhenAll(producerTasks);
        
        addCompleted.SetResult();
        
        await Task.WhenAll(consumerTasks);

        Assert.Multiple(() =>
        {
            Assert.That(exceptions, Is.Empty, "No exceptions should occur in thread-safe operations");
            
            Assert.That
            (
                retrievedItems, 
                Has.Count.EqualTo(allAddedItems.Count), 
                $"All {allAddedItems.Count} added items should be retrieved. Got {retrievedItems.Count}."
            );
            
            var uniqueRetrieved = retrievedItems.Distinct().Count();
            
            Assert.That
            (
                uniqueRetrieved, 
                Is.EqualTo(allAddedItems.Count), 
                $"No duplicates allowed. Expected {allAddedItems.Count} unique items, got {uniqueRetrieved}."
            );
            
            Assert.That
            (
                container.Count, 
                Is.EqualTo(0), 
                "Container should be empty after all items are retrieved"
            );
            
            var retrievedSet = retrievedItems.ToHashSet();
            
            Assert.That
            (
                retrievedSet.SetEquals(allAddedItems), 
                Is.True, 
                "All retrieved items should exactly match all added items"
            );
        });
    }
}

