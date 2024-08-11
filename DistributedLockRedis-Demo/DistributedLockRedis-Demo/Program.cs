using DistributedLockRedis_Demo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

InitializeRedlockFactory();

builder.Services.AddSingleton<PersonFakeRepository>();

builder.Services.AddHostedService<PersonProcessor>();

using IHost host = builder.Build();

await host.RunAsync();

void InitializeRedlockFactory()
{
    var redisMultiplexer = ConnectionMultiplexer.Connect("localhost");

    var redLockMultiplexer = new List<RedLockMultiplexer> { redisMultiplexer };

    var redlockFactory = RedLockFactory.Create(redLockMultiplexer);

    builder.Services.AddSingleton<IDistributedLockFactory>(redlockFactory);
}