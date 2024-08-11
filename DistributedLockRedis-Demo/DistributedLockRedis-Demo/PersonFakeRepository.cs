namespace DistributedLockRedis_Demo;

public class PersonFakeRepository
{
    public IReadOnlyCollection<Person> GetPeople() =>
        new List<Person>()
        {
            new Person(1, "William", string.Empty),
            new Person(2, "Olivia", string.Empty),
            new Person(3, "James", string.Empty),
        };
}

