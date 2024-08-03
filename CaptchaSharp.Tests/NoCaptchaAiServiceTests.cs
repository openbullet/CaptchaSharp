using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class NoCaptchaAiFixture : ServiceFixture
{
    public NoCaptchaAiFixture()
    {
        Service = new NoCaptchaAiService(
            Config.Credentials.NoCaptchaAiApiKey);
    }
}

public class NoCaptchaAiServiceTests(NoCaptchaAiFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<NoCaptchaAiFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
    [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
}
