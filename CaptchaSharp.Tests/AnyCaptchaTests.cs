using CaptchaSharp.Services.More;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class AnyCaptchaFixture : ServiceFixture
    {
        public AnyCaptchaFixture()
        {
            Service = new AnyCaptchaService(Config.Credentials.AnyCaptchaApiKey);
        }
    }

    public class AnyCaptchaServiceTests : ServiceTests, IClassFixture<AnyCaptchaFixture>
    {
        public AnyCaptchaServiceTests(AnyCaptchaFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
        [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
        [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
        [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
        [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
    }
}