namespace DistributedLockRedis_Demo;

public record Person(int Id, string Name, string Work)
{
    public string GetKey() => Id.ToString();
};