using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class DeathByCaptchaFixture : ServiceFixture
{
    public DeathByCaptchaFixture()
    {
        Service = new DeathByCaptchaService(
            Config.Credentials.DeathByCaptchaUsername,
            Config.Credentials.DeathByCaptchaPassword);
    }
}

public class DeathByCaptchaServiceTests(DeathByCaptchaFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<DeathByCaptchaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
    [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
    [Fact] public Task SolveFuncaptchaAsync_WithProxy_ValidSolution() => FunCaptchaTest_WithProxy();
}
