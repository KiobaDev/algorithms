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
    public void AddAndGet_ConcurrentOperations_ThreadSafe()
    {
        const int itemsToAdd = 5000;
        
        var container = new SharedMemoryContainer();
        var allAddedItems = new HashSet<string>();
        var retrievedItems = new ConcurrentBag<string>();
        var exceptions = new ConcurrentBag<Exception>();
        var lockObject = new Lock();
        var addCompleted = new ManualResetEventSlim(false);
        var allThreads = new List<Thread>();

        for (var i = 0; i < 5; i++)
        {
            var threadId = i;
            
            var thread = new Thread(() =>
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
            
            allThreads.Add(thread);
            thread.Start();
        }

        for (var i = 0; i < 5; i++)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    while (!addCompleted.IsSet || container.Count > 0)
                    {
                        var item = container.Get();
                        
                        if (item != null)
                        {
                            retrievedItems.Add(item);
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            
            allThreads.Add(thread);
            thread.Start();
        }

        for (var i = 0; i < 5; i++)
        {
            allThreads[i].Join();
        }
        
        addCompleted.Set();
        
        for (var i = 5; i < 10; i++)
        {
            allThreads[i].Join();
        }
        
        addCompleted.Dispose();

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

