using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class SolvechaFixture : ServiceFixture
{
    public SolvechaFixture()
    {
        Service = new SolvechaService(Config.Credentials.SolvechaApiKey);
    }
}

public class SolvechaServiceTests(SolvechaFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<SolvechaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
}
