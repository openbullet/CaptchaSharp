using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class TwoCaptchaFixture : ServiceFixture
    {
        public TwoCaptchaFixture()
        {
            Service = new TwoCaptchaService(Config.Credentials.TwoCaptchaApiKey);
        }
    }

    public class TwoCaptchaServiceTests : ServiceTests, IClassFixture<TwoCaptchaFixture>
    {
        public TwoCaptchaServiceTests(TwoCaptchaFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveTextCaptchaAsync_ValidCaptcha_ValidSolution() => TextCaptchaTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
        [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
        [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
        [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
        [Fact] public Task SolveFuncaptchaAsync_WithProxy_ValidSolution() => FunCaptchaTest_WithProxy();
        [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
        [Fact] public Task SolveHCaptchaAsync_WithProxy_ValidSolution() => HCaptchaTest_WithProxy();
        [Fact] public Task SolveCapyAsync_NoProxy_ValidSolution() => ShouldNotBeSupported(CapyTest_NoProxy);
        [Fact] public Task SolveCapyAsync_WithProxy_ValidSolution() => ShouldNotBeSupported(CapyTest_WithProxy);
    }
}