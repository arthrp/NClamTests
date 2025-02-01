using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using nClam;
using NUnit.Framework;

namespace NClamTests;

[TestFixture]
public class NClamLatestTests
{
    private IContainer _container;
    private const int Port = 3310;

    [OneTimeSetUp]
    public async Task SetupOnce()
    {
        _container = new ContainerBuilder()
            .WithImage("clamav/clamav:1.4")
            .WithPortBinding(Port, false)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(Port))
            .Build();

        await _container.StartAsync();
    }
    
    [Test]
    public async Task Ping_Works()
    {
        var clam = new ClamClient("localhost", Port);
        var result = await clam.TryPingAsync();
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanScanFile()
    {
        var clam = new ClamClient("localhost", Port);
        var exists = File.Exists("testfile.txt");

        var scanResult = await clam.SendAndScanFileAsync("testfile.txt");
        Assert.That(scanResult.Result, Is.EqualTo(ClamScanResults.Clean));
    }
}