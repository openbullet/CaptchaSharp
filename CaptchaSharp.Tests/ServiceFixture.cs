namespace CaptchaSharp.Tests;

public abstract class ServiceFixture
{
    private readonly ConfigFixture _configFixture;
    public Config Config => _configFixture.Config;
    public CaptchaService Service { get; init; }

    protected ServiceFixture()
    {
        _configFixture = new ConfigFixture();
    }
}
