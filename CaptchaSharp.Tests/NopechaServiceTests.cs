using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class NopechaFixture : ServiceFixture
{
    public NopechaFixture()
    {
        Service = new NopechaService(Config.Credentials.NopechaApiKey);
    }
}

public class NopechaServiceTests(NopechaFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<NopechaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV2EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV3EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_WithProxy_ValidSolution() => RecaptchaV3EnterpriseTest_WithProxy();
    [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
    [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_NoProxy_ValidSolution() => CloudflareTurnstileTest_NoProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_WithProxy_ValidSolution() => CloudflareTurnstileTest_WithProxy();
}
