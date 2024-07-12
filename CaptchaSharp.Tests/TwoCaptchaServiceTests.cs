using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests;

public class TwoCaptchaFixture : ServiceFixture
{
    public TwoCaptchaFixture()
    {
        Service = new TwoCaptchaService(Config.Credentials.TwoCaptchaApiKey);
    }
}

public class TwoCaptchaServiceTests(TwoCaptchaFixture fixture) : ServiceTests(fixture), IClassFixture<TwoCaptchaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task ReportSolution_NoException() => ReportSolutionTest();
    [Fact] public Task SolveTextCaptchaAsync_ValidCaptcha_ValidSolution() => TextCaptchaTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_WithProxy_ValidSolution() => RecaptchaV2InvisibleTest_WithProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV2EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_WithProxy_ValidSolution() => RecaptchaV2EnterpriseTest_WithProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV3EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_WithProxy_ValidSolution() => RecaptchaV3EnterpriseTest_WithProxy();
    [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
    [Fact] public Task SolveFuncaptchaAsync_WithProxy_ValidSolution() => FunCaptchaTest_WithProxy();
    [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
    [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
    [Fact] public Task SolveKeyCaptchaAsync_NoProxy_ValidSolution() => KeyCaptchaTest_NoProxy();
    [Fact] public Task SolveKeyCaptchaAsync_WithProxy_ValidSolution() => KeyCaptchaTest_WithProxy();
    [Fact] public Task SolveGeeTestAsync_NoProxy_ValidSolution() => GeeTestTest_NoProxy();
    [Fact] public Task SolveGeeTestAsync_WithProxy_ValidSolution() => GeeTestTest_WithProxy();
    [Fact] public Task SolveCapyAsync_NoProxy_ValidSolution() => CapyTest_NoProxy();
    [Fact] public Task SolveCapyAsync_WithProxy_ValidSolution() => CapyTest_WithProxy();
    [Fact] public Task SolveDataDomeAsync_WithProxy_ValidSolution() => DataDomeTest_WithProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_NoProxy_ValidSolution() => CloudflareTurnstileTest_NoProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_WithProxy_ValidSolution() => CloudflareTurnstileTest_WithProxy();
}
