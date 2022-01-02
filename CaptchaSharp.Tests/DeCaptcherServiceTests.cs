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
        [Fact] public Task SolveRecaptchaV3Async_NoProxy_NotSupported() => ShouldNotBeSupported(RecaptchaV3Test_NoProxy);
        [Fact] public Task SolveRecaptchaV3Async_WithProxy_NotSupported() => ShouldNotBeSupported(RecaptchaV3Test_WithProxy);
        [Fact] public Task SolveFuncaptchaAsync_NoProxy_NotSupported() => ShouldNotBeSupported(FunCaptchaTest_NoProxy);
        [Fact] public Task SolveFuncaptchaAsync_WithProxy_NotSupported() => ShouldNotBeSupported(FunCaptchaTest_WithProxy);
        [Fact] public Task SolveHCaptchaAsync_NoProxy_NotSupported() => ShouldNotBeSupported(HCaptchaTest_NoProxy);
        [Fact] public Task SolveHCaptchaAsync_WithProxy_NotSupported() => ShouldNotBeSupported(HCaptchaTest_WithProxy);
        [Fact] public Task SolveKeyCaptchaAsync_NoProxy_NotSupported() => ShouldNotBeSupported(KeyCaptchaTest_NoProxy);
        [Fact] public Task SolveKeyCaptchaAsync_WithProxy_NotSupported() => ShouldNotBeSupported(KeyCaptchaTest_WithProxy);
        [Fact] public Task SolveGeeTestAsync_NoProxy_NotSupported() => ShouldNotBeSupported(GeeTestTest_NoProxy);
        [Fact] public Task SolveGeeTestAsync_WithProxy_NotSupported() => ShouldNotBeSupported(GeeTestTest_WithProxy);
        [Fact] public Task SolveCapyAsync_NoProxy_NotSupported() => ShouldNotBeSupported(CapyTest_NoProxy);
        [Fact] public Task SolveCapyAsync_WithProxy_NotSupported() => ShouldNotBeSupported(CapyTest_WithProxy);
    }
}