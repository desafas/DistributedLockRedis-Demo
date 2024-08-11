using Microsoft.Extensions.Hosting;
using RedLockNet;
using System.Diagnostics;

namespace DistributedLockRedis_Demo;

public class PersonProcessor : IHostedService, IDisposable
{
    private readonly IDistributedLockFactory _redlockFactory;
    private readonly IReadOnlyCollection<Person> _people;
    private Timer? _timer = null;
    private const int UnlockAfterSeconds = 30;
    private const int TryToAquireForSeconds = 30;
    private const int RetryDelay = 2;
    private const int ProcessTimeInSeconds = 3;

    public PersonProcessor(
        IDistributedLockFactory redlockFactory,
        PersonFakeRepository personRepository)
    {
        _redlockFactory = redlockFactory;
        _people = personRepository.GetPeople();
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ProcessPerson, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private async void ProcessPerson(object? state)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var random = new Random();
        var person = _people.ElementAt(random.Next(0, _people.Count));

        await using var redLock = await _redlockFactory.CreateLockAsync(
            person.GetKey(),
            TimeSpan.FromSeconds(UnlockAfterSeconds),
            TimeSpan.FromSeconds(TryToAquireForSeconds),
            TimeSpan.FromSeconds(RetryDelay));

        if (redLock.IsAcquired)
        {
            stopwatch.Stop();
            Console.WriteLine($"Picked up {person.Name} after {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
            await Task.Delay(TimeSpan.FromSeconds(ProcessTimeInSeconds));
        }
        else
        {
            // Exceeded retry attempts.
            Console.WriteLine($"Failed to up {person.Name}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop Async");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
