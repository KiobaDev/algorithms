namespace Ryanair;

public sealed class SharedMemoryContainer
{
    private readonly Queue<string> _queue = new();
    private readonly Lock _lock = new();

    public void Add(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        lock (_lock)
        {
            foreach (var item in items)
            {
                _queue.Enqueue(item);
            }
        }
    }

    public string? Get()
    {
        lock (_lock)
        {
            return _queue.TryDequeue(out var item) ? item : null;
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _queue.Count;
            }
        }
    }
}

