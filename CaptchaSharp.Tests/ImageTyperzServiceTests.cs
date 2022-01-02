using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class ImageTyperzFixture : ServiceFixture
    {
        public ImageTyperzFixture()
        {
            Service = new ImageTyperzService(Config.Credentials.ImageTyperzApiKey);
        }
    }

    public class ImageTyperzServiceTests : ServiceTests, IClassFixture<ImageTyperzFixture>
    {
        public ImageTyperzServiceTests(ImageTyperzFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
        [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
        [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
        [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
        [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
        [Fact] public Task SolveGeeTestAsync_NoProxy_ValidSolution() => GeeTestTest_NoProxy();
        [Fact] public Task SolveGeeTestAsync_WithProxy_ValidSolution() => GeeTestTest_WithProxy();
        [Fact] public Task SolveCapyAsync_NoProxy_ValidSolution() => CapyTest_NoProxy();
        [Fact] public Task SolveCapyAsync_WithProxy_ValidSolution() => CapyTest_WithProxy();
    }
}