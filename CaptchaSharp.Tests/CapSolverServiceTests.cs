using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class CapSolverFixture : ServiceFixture
{
    public CapSolverFixture()
    {
        Service = new CapSolverService(Config.Credentials.CapSolverApiKey);
    }
}

public class CapSolverServiceTests(CapSolverFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<CapSolverFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task ReportSolution_NoException() => ReportImageSolutionTest();
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
    [Fact] public Task SolveGeeTestAsync_NoProxy_ValidSolution() => GeeTestTest_NoProxy();
    [Fact] public Task SolveDataDomeTestAsync_WithProxy_ValidSolution() => DataDomeTest_WithProxy();
    [Fact] public Task SolveCloudflareTurnstileAsync_NoProxy_ValidSolution() => CloudflareTurnstileTest_NoProxy();
    [Fact] public Task SolveAmazonWafAsync_NoProxy_ValidSolution() => AmazonWafTest_NoProxy();
    [Fact] public Task SolveAmazonWafAsync_WithProxy_ValidSolution() => AmazonWafTest_WithProxy();
    [Fact] public Task SolveMtCaptchaAsync_NoProxy_ValidSolution() => MtCaptchaTest_NoProxy();
    [Fact] public Task SolveMtCaptchaAsync_WithProxy_ValidSolution() => MtCaptchaTest_WithProxy();
}
