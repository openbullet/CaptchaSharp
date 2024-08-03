namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class FuncaptchaTaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public string? FuncaptchaApiJSSubdomain { get; set; }
    
    public FuncaptchaTaskProxyless()
    {
        Type = "FuncaptchaTaskProxyless";
    }
}
