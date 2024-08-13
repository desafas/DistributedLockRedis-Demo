using Microsoft.Extensions.Hosting;

namespace DistributedLockRedis_Demo;

public class Client1 : IHostedService, IDisposable
{
    private readonly PersonProcessor _personProcessor;

    public Client1(PersonProcessor personProcessor)
    {
        _personProcessor = personProcessor;
    }

    private Timer? _timer = null;
    private const int PersonPickupDelay = 5;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ProcessPerson, null, TimeSpan.Zero, TimeSpan.FromSeconds(PersonPickupDelay));

        return Task.CompletedTask;
    }

    private async void ProcessPerson(object? state)
    {
        await _personProcessor.ProcessPerson(nameof(Client1));
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
