using CaptchaSharp.Models;
using Newtonsoft.Json;
using System.IO;

namespace CaptchaSharp.Tests
{
    public class ConfigFixture
    {
        private readonly string credentialsFile = "config.json";
        public Config Config { get; set; }

        public ConfigFixture()
        {
            if (File.Exists(credentialsFile))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(credentialsFile));
            }
            else
            {
                // Write a blank structure if it doesn't exist so that we don't have to manually create it from scratch
                Config = new Config();
                File.WriteAllText(credentialsFile, JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
        }
    }

    public class Config
    {
        public Credentials Credentials { get; set; } = new();
        public Proxy Proxy { get; set; } = new();
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
        public string DeCaptcherUsername { get; set; } = string.Empty;
        public string DeCaptcherPassword { get; set; } = string.Empty;
        public string ImageTyperzApiKey { get; set; } = string.Empty;
        public string CapMonsterHost { get; set; } = string.Empty;
        public int CapMonsterPort { get; set; } = 80;
        public string AZCaptchaApiKey { get; set; } = string.Empty;
        public string CaptchasIOApiKey { get; set; } = string.Empty;
        public string RuCaptchaApiKey { get; set; } = string.Empty;
        public string SolveCaptchaApiKey { get; set; } = string.Empty;
        public string SolveRecaptchaApiKey { get; set; } = string.Empty;
        public string TrueCaptchaApiKey { get; set; } = string.Empty;
        public string TrueCaptchaUsername { get; set; } = string.Empty;
        public string NineKWApiKey { get; set; } = string.Empty;
        public string AnyCaptchaApiKey { get; set; } = string.Empty;
        public string CapSolverApiKey { get; set; } = string.Empty;
    }
}
