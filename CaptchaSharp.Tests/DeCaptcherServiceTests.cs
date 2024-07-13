using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class DeCaptcherFixture : ServiceFixture
{
    public DeCaptcherFixture()
    {
        Service = new DeCaptcherService(Config.Credentials.DeCaptcherApiKey);
    }
}

public class DeCaptcherServiceTests(DeCaptcherFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<DeCaptcherFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        
    // Do not overly use this test, or you will get banned.
    [Fact] public Task ReportSolution_NoException() => ReportSolutionTest();
        
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV3EnterpriseTest_NoProxy();
}
