using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CaptchaSharp.Services;
using Xunit;
using Xunit.Abstractions;

namespace CaptchaSharp.Tests;

public class ServiceTests
{
    private readonly ServiceFixture _fixture;
    private readonly ITestOutputHelper _output;

    private const string _captchaImageBase64 = "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";

    private CaptchaService Service => _fixture.Service;

    protected ServiceTests(ServiceFixture fixture, ITestOutputHelper output)
    {
        this._fixture = fixture;
        _output = output;
    }

    protected static Task ShouldNotBeSupported(Func<Task> method) => Assert.ThrowsAsync<NotSupportedException>(method);

    protected async Task BalanceTest()
    {
        var balance = await Service.GetBalanceAsync();
        Assert.True(balance > 0);
        
        _output.WriteLine($"Balance: {balance}");
    }

    protected async Task ReportImageSolutionTest(bool correct = true)
    {
        var options = new ImageCaptchaOptions
        {
            IsPhrase = true,
            CaseSensitive = true,
            CharacterSet = CharacterSet.NotSpecified,
            RequiresCalculation = false,
            MinLength = 0,
            MaxLength = 0,
            CaptchaLanguageGroup = CaptchaLanguageGroup.NotSpecified,
            CaptchaLanguage = CaptchaLanguage.NotSpecified,
            TextInstructions = ""
        };
            
        var solution = await Service.SolveImageCaptchaAsync(
            base64: _captchaImageBase64,
            options);

        await Service.ReportSolution(
            solution.Id, CaptchaType.ImageCaptcha, correct);
            
        Assert.True(true);
    }
    
    protected async Task ReportRecaptchaSolutionTest(bool correct = true)
    {
        var solution = await Service.SolveRecaptchaV2Async(
            siteKey: "6LfD3PIbAAAAAJs_eEHvoOl75_83eXSqpPSRFJ_u",
            siteUrl: "https://2captcha.com/demo/recaptcha-v2",
            dataS: "",
            enterprise: false,
            invisible: false);

        await Service.ReportSolution(
            solution.Id, CaptchaType.ReCaptchaV2, correct);
            
        Assert.True(true);
    }

    protected async Task TextCaptchaTest()
    {
        var options = new TextCaptchaOptions
        {
            CaptchaLanguageGroup = CaptchaLanguageGroup.NotSpecified,
            CaptchaLanguage = CaptchaLanguage.NotSpecified
        };

        var solution = await Service.SolveTextCaptchaAsync(
            text: "What is 2+2?",
            options);

        Assert.Equal("4", solution.Response);
    }

    protected async Task ImageCaptchaTest()
    {
        var options = new ImageCaptchaOptions
        {
            IsPhrase = true,
            CaseSensitive = true,
            CharacterSet = CharacterSet.NotSpecified,
            RequiresCalculation = false,
            MinLength = 0,
            MaxLength = 0,
            CaptchaLanguageGroup = CaptchaLanguageGroup.NotSpecified,
            CaptchaLanguage = CaptchaLanguage.NotSpecified,
            TextInstructions = ""
        };

        var solution = await Service.SolveImageCaptchaAsync(
            base64: _captchaImageBase64,
            options);

        Assert.Equal("w68hp", solution.Response.Replace(" ", "").ToLower());
    }

    private async Task RecaptchaV2Test(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaV2Async(
            siteKey: "6LfD3PIbAAAAAJs_eEHvoOl75_83eXSqpPSRFJ_u",
            siteUrl: "https://2captcha.com/demo/recaptcha-v2",
            dataS: "",
            enterprise: false,
            invisible: false,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task RecaptchaV2Test_NoProxy() => RecaptchaV2Test(null);

    protected Task RecaptchaV2Test_WithProxy() => RecaptchaV2Test(_fixture.Config.Proxy);

    private async Task RecaptchaV2InvisibleTest(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaV2Async(
            siteKey: "6LdO5_IbAAAAAAeVBL9TClS19NUTt5wswEb3Q7C5",
            siteUrl: "https://2captcha.com/demo/recaptcha-v2-invisible",
            dataS: "",
            enterprise: false,
            invisible: true,
            proxy);
            
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
        
    protected Task RecaptchaV2InvisibleTest_NoProxy() => RecaptchaV2InvisibleTest(null);
        
    protected Task RecaptchaV2InvisibleTest_WithProxy() => RecaptchaV2InvisibleTest(_fixture.Config.Proxy);
        
    private async Task RecaptchaV2EnterpriseTest(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaV2Async(
            siteKey: "6Lf26sUnAAAAAIKLuWNYgRsFUfmI-3Lex3xT5N-s",
            siteUrl: "https://2captcha.com/demo/recaptcha-v2-enterprise",
            dataS: "",
            enterprise: true,
            invisible: true,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
        
    protected Task RecaptchaV2EnterpriseTest_NoProxy() => RecaptchaV2EnterpriseTest(null);
        
    protected Task RecaptchaV2EnterpriseTest_WithProxy() => RecaptchaV2EnterpriseTest(_fixture.Config.Proxy);
        
    private async Task RecaptchaV3Test(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaV3Async(
            siteKey: "6LfB5_IbAAAAAMCtsjEHEHKqcB9iQocwwxTiihJu",
            siteUrl: "https://2captcha.com/demo/recaptcha-v3",
            action: "demo_action",
            minScore: 0.9f,
            enterprise: false,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task RecaptchaV3Test_NoProxy() => RecaptchaV3Test(null);
    protected Task RecaptchaV3Test_WithProxy() => RecaptchaV3Test(_fixture.Config.Proxy);

    private async Task RecaptchaV3EnterpriseTest(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaV3Async(
            siteKey: "6Lel38UnAAAAAMRwKj9qLH2Ws4Tf2uTDQCyfgR6b",
            siteUrl: "https://2captcha.com/demo/recaptcha-v3-enterprise",
            action: "demo_action",
            minScore: 0.9f,
            enterprise: true,
            proxy);
            
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
        
    protected Task RecaptchaV3EnterpriseTest_NoProxy() => RecaptchaV3EnterpriseTest(null);
        
    protected Task RecaptchaV3EnterpriseTest_WithProxy() => RecaptchaV3EnterpriseTest(_fixture.Config.Proxy);
        
    private async Task FunCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveFuncaptchaAsync(
            publicKey: "3EE79F8D-13A6-474B-9278-448EA19F79B3",
            serviceUrl: "https://client-api.arkoselabs.com",
            siteUrl: "https://www.arkoselabs.com/arkose-matchkey/",
            noJs: false,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task FunCaptchaTest_NoProxy() => FunCaptchaTest(null);
    protected Task FunCaptchaTest_WithProxy() => FunCaptchaTest(_fixture.Config.Proxy);

    private async Task HCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveHCaptchaAsync(
            siteKey: "f7de0da3-3303-44e8-ab48-fa32ff8ccc7b",
            siteUrl: "https://2captcha.com/demo/hcaptcha",
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task HCaptchaTest_NoProxy() => HCaptchaTest(null);
    protected Task HCaptchaTest_WithProxy() => HCaptchaTest(_fixture.Config.Proxy);

    private async Task KeyCaptchaTest(Proxy? proxy)
    {
        // Get the required parameters from the page since they are not static
        const string siteUrl = "https://www.keycaptcha.com/contact-us/";
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(siteUrl);
        var pageSource = await response.Content.ReadAsStringAsync();

        var userId = Regex.Match(pageSource, "var s_s_c_user_id = '([^']*)'").Groups[1].Value;
        var sessionId = Regex.Match(pageSource, "var s_s_c_session_id = '([^']*)'").Groups[1].Value;
        var webServerSign1 = Regex.Match(pageSource, "var s_s_c_web_server_sign = '([^']*)'").Groups[1].Value;
        var webServerSign2 = Regex.Match(pageSource, "var s_s_c_web_server_sign2 = '([^']*)'").Groups[1].Value;

        var solution = await Service.SolveKeyCaptchaAsync(
            userId,
            sessionId,
            webServerSign1,
            webServerSign2,
            siteUrl,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task KeyCaptchaTest_NoProxy() => KeyCaptchaTest(null);
    protected Task KeyCaptchaTest_WithProxy() => KeyCaptchaTest(_fixture.Config.Proxy);

    private async Task GeeTestTest(Proxy? proxy)
    {
        // Get the required parameters from the page since they are not static
        var unixTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var siteUrl = $"https://www.geetest.com/demo/gt/register-enFullpage-official?t={unixTime}";
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(siteUrl);
        var pageSource = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(pageSource);

        var gt = obj.Value<string>("gt")!;
        var challenge = obj.Value<string>("challenge")!;

        var solution = await Service.SolveGeeTestAsync(
            gt,
            challenge,
            siteUrl,
            apiServer: "api.geetest.com",
            proxy);

        Assert.NotEqual("", solution.Challenge);
        Assert.NotEqual("", solution.SecCode);
        Assert.NotEqual("", solution.Validate);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Challenge: {solution.Challenge}");
        _output.WriteLine($"SecCode: {solution.SecCode}");
        _output.WriteLine($"Validate: {solution.Validate}");
    }

    protected Task GeeTestTest_NoProxy() => GeeTestTest(null);
    protected Task GeeTestTest_WithProxy() => GeeTestTest(_fixture.Config.Proxy);

    private async Task CapyTest(Proxy? proxy)
    {
        var solution = await Service.SolveCapyAsync(
            siteKey: "PUZZLE_Cme4hZLjuZRMYC3uh14C52D3uNms5w",
            siteUrl: $"{"https"}://www.capy.me/account/signin",
            proxy);

        Assert.NotEqual(string.Empty, solution.ChallengeKey);
        Assert.NotEqual(string.Empty, solution.CaptchaKey);
        Assert.NotEqual(string.Empty, solution.Answer);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"ChallengeKey: {solution.ChallengeKey}");
        _output.WriteLine($"CaptchaKey: {solution.CaptchaKey}");
        _output.WriteLine($"Answer: {solution.Answer}");
    }

    protected Task CapyTest_NoProxy() => CapyTest(null);
    protected Task CapyTest_WithProxy() => CapyTest(_fixture.Config.Proxy);

    // Proxy and User-Agent required
    private async Task DataDomeTest(Proxy proxy)
    {
        const string site = "https://antoinevastel.com/bots/datadome";
        
        // If it doesn't work, try a few times until it triggers
        // the captcha
        var cookieContainer = new CookieContainer();
        using var httpClientHandler = new HttpClientHandler();
        httpClientHandler.UseCookies = true;
        httpClientHandler.CookieContainer = cookieContainer;
        using var httpClient = new HttpClient(httpClientHandler);

        if (string.IsNullOrEmpty(proxy.UserAgent))
        {
            throw new ArgumentException("User-Agent is required");
        }
        
        // The User-Agent must be the same as the one used to get the page
        httpClient.DefaultRequestHeaders.Add("User-Agent", proxy.UserAgent);
        
        using var response = await httpClient.GetAsync(site);
        var pageSource = await response.Content.ReadAsStringAsync();
        
        var host = Regex.Match(pageSource, "'host':'([^']*)'").Groups[1].Value;
        var initialCid = Regex.Match(pageSource, "'cid':'([^']*)'").Groups[1].Value;
        var t = Regex.Match(pageSource, "'t':'([^']*)'").Groups[1].Value;
        var s = Regex.Match(pageSource, @"'s':(\d+)").Groups[1].Value;
        var e = Regex.Match(pageSource, "'e':'([^']*)'").Groups[1].Value;
        var hsh = Regex.Match(pageSource, "'hsh':'([^']*)'").Groups[1].Value;
        
        // Get cid from "datadome" cookie
        var cid = cookieContainer.GetCookies(new Uri(site))["datadome"]?.Value;
        proxy.Cookies = [("datadome", cid!)];

        var captchaUrl =
            $"https://{host}/captcha/?initialCid={WebUtility.UrlEncode(initialCid)}&hash={hsh}&cid={cid}&t={t}&referer={WebUtility.UrlEncode(site)}&s={s}&e={e}&dm=cd";
        
        var solution = await Service.SolveDataDomeAsync(
            siteUrl: site,
            captchaUrl: captchaUrl,
            proxy);

        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    protected Task DataDomeTest_WithProxy() => DataDomeTest(_fixture.Config.Proxy);

    private async Task CloudflareTurnstileTest(Proxy? proxy)
    {
        var solution = await Service.SolveCloudflareTurnstileAsync(
            siteKey: "0x4AAAAAAAVrOwQWPlm3Bnr5",
            siteUrl: "https://2captcha.com/demo/cloudflare-turnstile",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
        _output.WriteLine($"User-Agent: {solution.UserAgent}");
    }
    
    protected Task CloudflareTurnstileTest_NoProxy() => CloudflareTurnstileTest(new Proxy
    {
        // User-Agent required
        UserAgent = _fixture.Config.Proxy.UserAgent
    });
    
    protected Task CloudflareTurnstileTest_WithProxy() => CloudflareTurnstileTest(_fixture.Config.Proxy);

    private async Task LeminCroppedTest(Proxy? proxy)
    {
        var solution = await Service.SolveLeminCroppedAsync(
            captchaId: "CROPPED_3dfdd5c_d1872b526b794d83ba3b365eb15a200b",
            siteUrl: "https://2captcha.com/demo/lemin",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Answer);
        Assert.NotEqual(string.Empty, solution.ChallengeId);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Answer: {solution.Answer}");
        _output.WriteLine($"Challenge ID: {solution.ChallengeId}");
    }
    
    protected Task LeminCroppedTest_NoProxy() => LeminCroppedTest(null);
    
    protected Task LeminCroppedTest_WithProxy() => LeminCroppedTest(_fixture.Config.Proxy);
    
    private async Task AmazonWafTest(Proxy? proxy)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync("https://nopecha.com/captcha/awscaptcha");
        var pageSource = await response.Content.ReadAsStringAsync();
        
        var captchaPage = Regex.Match(pageSource, "<iframe src=\"([^\"]+)").Groups[1].Value;
        using var captchaResponse = await httpClient.GetAsync(captchaPage);
        var captchaSource = await captchaResponse.Content.ReadAsStringAsync();
        var siteKey = Regex.Match(captchaSource, "\"key\":\"([^\"]+)").Groups[1].Value;
        var iv = Regex.Match(captchaSource, "\"iv\":\"([^\"]+)").Groups[1].Value;
        var context = Regex.Match(captchaSource, "\"context\":\"([^\"]+)").Groups[1].Value;
        var challengeScript = Regex.Match(captchaSource, "src=\"([^\"]+challenge\\.js)").Groups[1].Value;
        var captchaScript = Regex.Match(captchaSource, "src=\"([^\"]+captcha\\.js)").Groups[1].Value;
        
        var solution = await Service.SolveAmazonWafAsync(
            siteKey: siteKey,
            siteUrl: "https://nopecha.com/captcha/awscaptcha",
            iv: iv,
            context: context,
            challengeScript: challengeScript,
            captchaScript: captchaScript,
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task AmazonWafTest_NoProxy() => AmazonWafTest(null);
    
    protected Task AmazonWafTest_WithProxy() => AmazonWafTest(_fixture.Config.Proxy);

    private async Task CyberSiAraTest(Proxy? proxy)
    {
        var solution = await Service.SolveCyberSiAraAsync(
            masterUrlId: "ABEBCAFBAAEDADFCBCCBEBAFCDDBBFEF",
            siteUrl: "https://mycybersiara.com/login",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task CyberSiAraTest_NoProxy() => CyberSiAraTest(new Proxy
    {
        // User-Agent required
        UserAgent = _fixture.Config.Proxy.UserAgent
    });
    
    protected Task CyberSiAraTest_WithProxy() => CyberSiAraTest(_fixture.Config.Proxy);
    
    private async Task MtCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveMtCaptchaAsync(
            siteKey: "MTPublic-KzqLY1cKH",
            siteUrl: "https://2captcha.com/demo/mtcaptcha",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task MtCaptchaTest_NoProxy() => MtCaptchaTest(null);
    
    protected Task MtCaptchaTest_WithProxy() => MtCaptchaTest(_fixture.Config.Proxy);

    private async Task CutCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveCutCaptchaAsync(
            siteUrl: "https://filecrypt.co/Container/BB072CC14C.html",
            miseryKey: "a46cd428f9cd7b965f6dcb84741e733769725550",
            apiKey: "SAs61IAI",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task CutCaptchaTest_NoProxy() => CutCaptchaTest(null);
    
    protected Task CutCaptchaTest_WithProxy() => CutCaptchaTest(_fixture.Config.Proxy);
    
    private async Task FriendlyCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveFriendlyCaptchaAsync(
            siteKey: "FCMGEMUD2K3JJ36P",
            siteUrl: "https://friendlycaptcha.com/signup/account/starter/",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task FriendlyCaptchaTest_NoProxy() => FriendlyCaptchaTest(null);
    
    protected Task FriendlyCaptchaTest_WithProxy() => FriendlyCaptchaTest(_fixture.Config.Proxy);

    private async Task AtbCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveAtbCaptchaAsync(
            appId: "af25e409b33d722a95e56a230ff8771c",
            apiServer: "https://cap.aisecurius.com",
            siteUrl: "https://renslider.com",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task AtbCaptchaTest_NoProxy() => AtbCaptchaTest(null);
    
    protected Task AtbCaptchaTest_WithProxy() => AtbCaptchaTest(_fixture.Config.Proxy);

    private async Task TencentCaptchaTest(Proxy? proxy)
    {
        var solution = await Service.SolveTencentCaptchaAsync(
            appId: "2009899766",
            siteUrl: "https://www.tencentcloud.com/account/register",
            proxy: proxy);

        Assert.NotEqual(string.Empty, solution.Ticket);
        Assert.NotEqual(string.Empty, solution.RandomString);

        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"AppId: {solution.AppId}");
        _output.WriteLine($"Ticket: {solution.Ticket}");
        _output.WriteLine($"ReturnCode: {solution.ReturnCode}");
        _output.WriteLine($"RandomString: {solution.RandomString}");
    }
    
    protected Task TencentCaptchaTest_NoProxy() => TencentCaptchaTest(null);
    
    protected Task TencentCaptchaTest_WithProxy() => TencentCaptchaTest(_fixture.Config.Proxy);

    protected async Task AudioCaptchaTest()
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(
            "https://audio-samples.github.io/samples/mp3/ted_speakers/BillGates/sample-1.mp3");
        var audioBytes = await response.Content.ReadAsByteArrayAsync();

        var solution = await Service.SolveAudioCaptchaAsync(
            base64: Convert.ToBase64String(audioBytes),
            options: new AudioCaptchaOptions
            {
                CaptchaLanguage = CaptchaLanguage.English,
            });
        
        Assert.Contains("phrase", solution.Response.ToLower());
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }

    private async Task RecaptchaMobileTest(Proxy? proxy)
    {
        var solution = await Service.SolveRecaptchaMobileAsync(
            appPackageName: "",
            appKey: "",
            appAction: "login",
            proxy: proxy);
        
        Assert.NotEqual(string.Empty, solution.Response);
        
        _output.WriteLine($"Captcha ID: {solution.Id}");
        _output.WriteLine($"Response: {solution.Response}");
    }
    
    protected Task RecaptchaMobileTest_NoProxy() => RecaptchaMobileTest(null);
    
    protected Task RecaptchaMobileTest_WithProxy() => RecaptchaMobileTest(_fixture.Config.Proxy);
}
