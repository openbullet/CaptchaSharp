using System;
using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class CustomTwoCaptchaFixture : ServiceFixture
{
    public CustomTwoCaptchaFixture()
    {
        Service = new CustomTwoCaptchaService(
            Config.Credentials.CustomTwoCaptchaApiKey,
            GetUri(Config.Credentials.CustomTwoCaptchaHost, Config.Credentials.CustomTwoCaptchaPort),
            httpClient: null,
            overrideHostHeader: Config.Credentials.CustomTwoCaptchaOverrideHostHeader
            );
    }
    
    private static Uri GetUri(string host, int port)
    {
        // If there is no http(s) then add http by default
        if (!host.StartsWith("http"))
        {
            host = $"http://{host}";
        }

        return new Uri($"{host}:{port}");
    }
}

public class CustomTwoCaptchaServiceTests(CustomTwoCaptchaFixture fixture, ITestOutputHelper output)
    : ServiceTests(fixture, output), IClassFixture<CustomTwoCaptchaFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_ValidCaptcha_ValidSolution() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV2EnterpriseTest_NoProxy();
    [Fact] public Task SolveFuncaptchaAsync_NoProxy_ValidSolution() => FunCaptchaTest_NoProxy();
    [Fact] public Task SolveHCaptchaAsync_NoProxy_ValidSolution() => HCaptchaTest_NoProxy();
}
