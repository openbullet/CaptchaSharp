using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests;

public class TrueCaptchaFixture : ServiceFixture
{
    public TrueCaptchaFixture()
    {
        Service = new TrueCaptchaService(   
            Config.Credentials.TrueCaptchaUsername,
            Config.Credentials.TrueCaptchaApiKey);
    }
}

public class TrueCaptchaServiceTests(TrueCaptchaFixture fixture)
    : ServiceTests(fixture), IClassFixture<TrueCaptchaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    
    // Do not overly use this test, or you will get banned.
    [Fact] public Task ReportSolution_NoException() => ReportSolutionTest();
    
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
}
