using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class DeCaptcherFixture : ServiceFixture
    {
        public DeCaptcherFixture()
        {
            Service = new DeCaptcherService(Config.Credentials.DeCaptcherApiKey);
        }
    }

    public class DeCaptcherServiceTests(DeCaptcherFixture fixture)
        : ServiceTests(fixture), IClassFixture<DeCaptcherFixture>
    {
        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
    }
}
