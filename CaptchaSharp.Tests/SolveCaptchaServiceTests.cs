using System.Reflection;
using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class SolveCaptchaFixture : ServiceFixture
{
    public SolveCaptchaFixture()
    {
        Service = new SolveCaptchaService(
            Config.Credentials.SolveCaptchaApiKey);
        
        Service.GetType().GetProperty("AffiliateId",
                BindingFlags.NonPublic | BindingFlags.Instance)?
            .SetValue(Service, "123");
    }
}

public class SolveCaptchaServiceTests(SolveCaptchaFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<SolveCaptchaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_WithProxy_ValidSolution() => RecaptchaV2InvisibleTest_WithProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV2EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_WithProxy_ValidSolution() => RecaptchaV2EnterpriseTest_WithProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
    [Fact] public Task SolveFuncaptchaAsync_WithProxy_ValidSolution() => FunCaptchaTest_WithProxy();
    [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
    [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
    [Fact] public Task SolveGeeTestAsync_NoProxy_ValidSolution() => GeeTestTest_NoProxy();
    [Fact] public Task SolveGeeTestAsync_WithProxy_ValidSolution() => GeeTestTest_WithProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_NoProxy_ValidSolution() => CloudflareTurnstileTest_NoProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_WithProxy_ValidSolution() => CloudflareTurnstileTest_WithProxy();
}
