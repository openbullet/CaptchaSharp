using CaptchaSharp.Models;
using Newtonsoft.Json;
using System.IO;

namespace CaptchaSharp.Tests;

public class ConfigFixture
{
    private const string _credentialsFile = "config.json";
    public Config Config { get; set; }

    public ConfigFixture()
    {
        if (File.Exists(_credentialsFile))
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_credentialsFile))!;
        }
        else
        {
            // Write a blank structure if it doesn't exist so that we don't have to manually create it from scratch
            Config = new Config();
            File.WriteAllText(_credentialsFile, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}

public class Config
{
    public Credentials Credentials { get; set; } = new();
    public SessionParams SessionParams { get; set; } = new();
}

public class Credentials
{
    public string TwoCaptchaApiKey { get; set; } = string.Empty;
    public string AntiCaptchaApiKey { get; set; } = string.Empty;
    public string CustomTwoCaptchaApiKey { get; set; } = string.Empty;
    public string CustomTwoCaptchaHost { get; set; } = string.Empty;
    public int CustomTwoCaptchaPort { get; set; } = 80;
    public bool CustomTwoCaptchaOverrideHostHeader { get; set; } = true;
    public string DeathByCaptchaUsername { get; set; } = string.Empty;
    public string DeathByCaptchaPassword { get; set; } = string.Empty;
    public string CaptchaCoderApiKey { get; set; } = string.Empty;
    public string HumanCoderApiKey { get; set; } = string.Empty;
    public string ImageTyperzApiKey { get; set; } = string.Empty;
    public string CapMonsterHost { get; set; } = string.Empty;
    public int CapMonsterPort { get; set; } = 80;
    public string AzCaptchaApiKey { get; set; } = string.Empty;
    public string CaptchasIoApiKey { get; set; } = string.Empty;
    public string RuCaptchaApiKey { get; set; } = string.Empty;
    public string TrueCaptchaApiKey { get; set; } = string.Empty;
    public string TrueCaptchaUsername { get; set; } = string.Empty;
    public string NineKWApiKey { get; set; } = string.Empty;
    public string CapSolverApiKey { get; set; } = string.Empty;
    public string CapMonsterCloudApiKey { get; set; } = string.Empty;
    public string MetaBypassTechClientId { get; set; } = string.Empty;
    public string MetaBypassTechClientSecret { get; set; } = string.Empty;
    public string MetaBypassTechUsername { get; set; } = string.Empty;
    public string MetaBypassTechPassword { get; set; } = string.Empty;
    public string NextCaptchaApiKey { get; set; } = string.Empty;
    public string NoCaptchaAiApiKey { get; set; } = string.Empty;
    public string NopechaApiKey { get; set; } = string.Empty;
    public string BestCaptchaSolverApiKey { get; set; } = string.Empty;
    public string CaptchaAiApiKey { get; set; } = string.Empty;
    public string EzCaptchaApiKey { get; set; } = string.Empty;
    public string SolveCaptchaApiKey { get; set; } = string.Empty;
    public string EndCaptchaUsername { get; set; } = string.Empty;
    public string EndCaptchaPassword { get; set; } = string.Empty;
    public string CapGuruApiKey { get; set; } = string.Empty;
    public string AycdApiKey { get; set; } = string.Empty;
}
