using RedLockNet;

namespace DistributedLockRedis_Demo;

public class PersonProcessor
{
    private readonly IDistributedLockFactory _redlockFactory;
    private readonly IReadOnlyCollection<Person> _people;
    private const int UnlockAfterSeconds = 30;
    private const int ProcessTimeInSeconds = 3;
    
    public PersonProcessor(
        IDistributedLockFactory redlockFactory,
        PersonFakeRepository personRepository)
    {
        _redlockFactory = redlockFactory;
        _people = personRepository.GetPeople();
    }

    public async Task ProcessPerson(string client)
    {
        var random = new Random();
        var person = _people.ElementAt(random.Next(0, _people.Count));

        await using var redLock = await _redlockFactory.CreateLockAsync(
            person.GetKey(),
            TimeSpan.FromSeconds(UnlockAfterSeconds));

        if (redLock.IsAcquired)
        {
            Console.WriteLine($"[({client}) - Lock Acquired :{DateTime.UtcNow:hh:mm:ss}] Picked up {person.Name}.");
            await Task.Delay(TimeSpan.FromSeconds(ProcessTimeInSeconds));
        }
        else
        {
            Console.WriteLine($"[({client}) - Lock Failed   :{DateTime.UtcNow:hh:mm:ss}] Failed to pick up {person.Name}");
        }
    }
}
