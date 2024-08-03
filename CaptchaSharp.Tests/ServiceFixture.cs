using CaptchaSharp.Services;

namespace CaptchaSharp.Tests;

public abstract class ServiceFixture
{
    private readonly ConfigFixture _configFixture = new();
    public Config Config => _configFixture.Config;
    public required CaptchaService Service { get; init; }
}
