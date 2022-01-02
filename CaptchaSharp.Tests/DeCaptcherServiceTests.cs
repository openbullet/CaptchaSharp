using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class DeCaptcherFixture : ServiceFixture
    {
        public DeCaptcherFixture()
        {
            Service = new DeCaptcherService(
                Config.Credentials.DeCaptcherUsername,
                Config.Credentials.DeCaptcherPassword);
        }
    }

    public class DeCaptcherServiceTests : ServiceTests, IClassFixture<DeCaptcherFixture>
    {
        public DeCaptcherServiceTests(DeCaptcherFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveTextCaptchaAsync_ValidCaptcha_ValidSolution() => TextCaptchaTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
    }
}