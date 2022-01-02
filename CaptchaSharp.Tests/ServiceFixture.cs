namespace CaptchaSharp.Tests
{
    public abstract class ServiceFixture
    {
        private readonly ConfigFixture configFixture;
        public Config Config => configFixture.Config;
        public CaptchaService Service { get; set; }

        public ServiceFixture()
        {
            configFixture = new ConfigFixture();
        }
    }
}
