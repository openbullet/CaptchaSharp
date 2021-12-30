using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
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

    public class TwoCaptchaServiceTests : IClassFixture<TwoCaptchaFixture>
    {
        private readonly TwoCaptchaFixture fixture;

        public TwoCaptchaServiceTests(TwoCaptchaFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetBalanceAsync_ValidKey_GetsBalance()
        {
            var balance = await fixture.Service.GetBalanceAsync();
            Assert.True(balance > 0);
        }

        [Fact]
        public async Task GetBalanceAsync_InvalidKey_Throws()
        {
            var service = new TwoCaptchaService("invalid_key");
            await Assert.ThrowsAsync<BadAuthenticationException>(async () =>
                await service.GetBalanceAsync());
        }

        [Fact]
        public async Task SolveTextCaptchaAsync_ValidCaptcha_ValidSolution()
        {
            var solution = await fixture.Service.SolveTextCaptchaAsync(
                fixture.Config.Captchas.TextCaptcha.Question,
                fixture.Config.Captchas.TextCaptcha.Options);

            Assert.Equal(fixture.Config.Captchas.TextCaptcha.Solution, solution.Response);
        }
    }
}