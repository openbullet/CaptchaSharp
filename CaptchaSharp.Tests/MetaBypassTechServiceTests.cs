using System;
using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class MetaBypassTechFixture : ServiceFixture
{
    public MetaBypassTechFixture()
    {
        Service = new MetaBypassTechService(
            Config.Credentials.MetaBypassTechClientId,
            Config.Credentials.MetaBypassTechClientSecret,
            Config.Credentials.MetaBypassTechUsername,
            Config.Credentials.MetaBypassTechPassword)
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
    }
}

public class MetaBypassTechServiceTests(MetaBypassTechFixture fixture, ITestOutputHelper output) 
    : ServiceTests(fixture, output), IClassFixture<MetaBypassTechFixture>
{
    [Fact] public Task GetBalanceAsync_ValidKey_GetsBalance() => BalanceTest();
    [Fact] public Task SolveImageCaptchaAsync_InvalidImage_ThrowsException() => ImageCaptchaTest();
    [Fact] public Task SolveRecaptchaV2Async_NoProxy_ValidSolution() => RecaptchaV2Test_NoProxy();
    [Fact] public Task SolveRecaptchaV2InvisibleAsync_NoProxy_ValidSolution() => RecaptchaV2InvisibleTest_NoProxy();
    [Fact] public Task SolveRecaptchaV2EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV2EnterpriseTest_NoProxy();
    [Fact] public Task SolveRecaptchaV3Async_NoProxy_ValidSolution() => RecaptchaV3Test_NoProxy();
    [Fact] public Task SolveRecaptchaV3EnterpriseAsync_NoProxy_ValidSolution() => RecaptchaV3EnterpriseTest_NoProxy();
}
