using CaptchaSharp.Services;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class DeathByCaptchaFixture : ServiceFixture
    {
        public DeathByCaptchaFixture()
        {
            Service = new DeathByCaptchaService(
                Config.Credentials.DeathByCaptchaUsername,
                Config.Credentials.DeathByCaptchaPassword);
        }
    }

    public class DeathByCaptchaServiceTests : ServiceTests, IClassFixture<DeathByCaptchaFixture>
    {
        public DeathByCaptchaServiceTests(DeathByCaptchaFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveTextCaptchaAsync_ValidCaptcha_NotSupported() => ShouldNotBeSupported(TextCaptchaTest);
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV2Async_WithProxy_ValidSolution() => RecaptchaV2Test_WithProxy();
        [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
        [Fact] public Task SolveRecaptchaV3Async_WithProxy_ValidSolution() => RecaptchaV3Test_WithProxy();
        [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
        [Fact] public Task SolveFuncaptchaAsync_WithProxy_ValidSolution() => FunCaptchaTest_WithProxy();
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