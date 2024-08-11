using Microsoft.Extensions.Hosting;
using RedLockNet;

namespace DistributedLockRedis_Demo;

public class PersonProcessor : IHostedService, IDisposable
{
    private readonly IDistributedLockFactory _redlockFactory;
    private readonly IReadOnlyCollection<Person> _people;
    private Timer? _timer = null;
    private const int UnlockAfterSeconds = 30;
    private const int ProcessTimeInSeconds = 3;
    private const int PersonPickupDelay = 5;

    public PersonProcessor(
        IDistributedLockFactory redlockFactory,
        PersonFakeRepository personRepository)
    {
        _redlockFactory = redlockFactory;
        _people = personRepository.GetPeople();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ProcessPerson, null, TimeSpan.Zero, TimeSpan.FromSeconds(PersonPickupDelay));

        return Task.CompletedTask;
    }

    private async void ProcessPerson(object? state)
    {
        var random = new Random();
        var person = _people.ElementAt(random.Next(0, _people.Count));

        await using var redLock = await _redlockFactory.CreateLockAsync(
            person.GetKey(),
            TimeSpan.FromSeconds(UnlockAfterSeconds));

        if (redLock.IsAcquired)
        {
            Console.WriteLine($"[Lock Acquired:{DateTime.UtcNow:hh:mm:ss}] Picked up {person.Name}.");
            await Task.Delay(TimeSpan.FromSeconds(ProcessTimeInSeconds));
        }
        else
        {
            Console.WriteLine($"[Lock Failed:{DateTime.UtcNow:hh:mm:ss}] Failed to pick up {person.Name}");
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
