using CaptchaSharp.Services.More;
using System.Threading.Tasks;
using Xunit;

namespace CaptchaSharp.Tests
{
    public class TrueCaptchaFixture : ServiceFixture
    {
        public TrueCaptchaFixture()
        {
            Service = new TrueCaptchaService(
                Config.Credentials.TrueCaptchaUsername,
                Config.Credentials.TrueCaptchaApiKey);
        }
    }

    public class TrueCaptchaServiceTests : ServiceTests, IClassFixture<TrueCaptchaFixture>
    {
        public TrueCaptchaServiceTests(TrueCaptchaFixture fixture) : base(fixture) { }

        [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
        [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    }
}