using CaptchaSharp;
using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using CaptchaSharp.Services;
using CaptchaSharp.Services.More;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SolverTester
{
    public partial class MainWindow : Window
    {
        public string ServiceTypeName { get; set; } = "TwoCaptchaService";
        public CaptchaType CaptchaType { get; set; } = CaptchaType.ReCaptchaV2;
        public Bitmap CaptchaImage { get; set; } = null;

        // Authentication
        public string ApiKey { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        // Configuration
        public string CustomTwoCaptchaBaseUri { get; set; } = "http://2captcha.com/";
        public string Timeout { get; set; } = "180";
        public string Proxy { get; set; } = "";
        public ProxyType ProxyType { get; set; } = ProxyType.HTTPS;
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36";

        // TextCaptcha / ImageCaptcha
        public string Text { get; set; }
        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;
        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;

        // ImageCaptcha
        public bool IsPhrase { get; set; } = false;
        public bool CaseSensitive { get; set; } = true;
        public CharacterSet CharacterSet { get; set; } = CharacterSet.NotSpecified;
        public bool RequiresCalculation { get; set; } = false;
        public string MinLength { get; set; } = "0";
        public string MaxLength { get; set; } = "0";
        public string TextInstructions { get; set; } = "";

        // RecaptchaV2 / RecaptchaV3 / HCaptcha / FunCaptcha / KeyCaptcha / GeeTest
        public string SiteKey { get; set; } = "";
        public bool Enterprise { get; set; } = false;

        // RecaptchaV2 / RecaptchaV3 / HCaptcha
        public string SiteUrl { get; set; } = "";

        // RecaptchaV2
        public bool Invisible { get; set; } = false;
        public string SData { get; set; } = "";

        // RecaptchaV3
        public string Action { get; set; } = "";
        public string MinScore { get; set; } = "0.3";

        // FunCaptcha
        public string PublicKey { get; set; } = "";
        public string ServiceUrl { get; set; } = "";
        public bool NoJS { get; set; } = false;

        // KeyCaptcha
        public string UserId { get; set; } = "";
        public string SessionId { get; set; } = "";
        public string WebServerSign1 { get; set; } = "";
        public string WebServerSign2 { get; set; } = "";

        // GeeTest
        public string GT { get; set; } = "";
        public string Challenge { get; set; } = "";
        public string ApiServer { get; set; } = "";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Dummy line so that the JIT compiler loads the assembly containing the extra solvers
            new RuCaptchaService("", null);

            #region Setup Combo Boxes
            foreach (var s in GetServiceTypes().Select(t => t.Name))
                serviceCombobox.Items.Add(s);

            serviceCombobox.SelectedValue = ServiceTypeName;

            foreach (var c in Enum.GetNames(typeof(CaptchaType)))
                captchaTypeCombobox.Items.Add(c);

            captchaTypeCombobox.SelectedIndex = (int)CaptchaType;

            foreach (var g in Enum.GetNames(typeof(CaptchaLanguageGroup)))
            {
                textLanguageGroupCombobox.Items.Add(g);
                imageLanguageGroupCombobox.Items.Add(g);
            }

            textLanguageGroupCombobox.SelectedIndex = (int)CaptchaLanguageGroup;
            imageLanguageGroupCombobox.SelectedIndex = (int)CaptchaLanguageGroup;

            foreach (var l in Enum.GetNames(typeof(CaptchaLanguage)))
            {
                textLanguageCombobox.Items.Add(l);
                imageLanguageCombobox.Items.Add(l);
            }
                
            textLanguageCombobox.SelectedIndex = (int)CaptchaLanguage;
            imageLanguageCombobox.SelectedIndex = (int)CaptchaLanguage;

            foreach (var s in Enum.GetNames(typeof(CharacterSet)))
                imageCharacterSetCombobox.Items.Add(s);

            imageCharacterSetCombobox.SelectedIndex = (int)CharacterSet;

            foreach (var p in Enum.GetNames(typeof(ProxyType)))
                proxyTypeCombobox.Items.Add(p);

            proxyTypeCombobox.SelectedIndex = (int)ProxyType;
            #endregion
        }

        private void serviceCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceTypeName = ((ComboBox)e.OriginalSource).SelectedValue as string;
            configTabControl.SelectedIndex = 0;
            var serviceType = GetServiceType(ServiceTypeName);

            if (serviceType == typeof(CustomTwoCaptchaService) || serviceType == typeof(CapMonsterService))
                configTabControl.SelectedIndex = 1;
            else
                configTabControl.SelectedIndex = 0;

            if (IsApiKeyService(serviceType))
                authTabControl.SelectedIndex = 0;
            else
                authTabControl.SelectedIndex = 1;
        }

        private void captchaTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaptchaType = (CaptchaType)((ComboBox)e.OriginalSource).SelectedIndex;

            var dict = new Dictionary<CaptchaType, int>
            {
                { CaptchaType.TextCaptcha,  0 },
                { CaptchaType.ImageCaptcha, 1 },
                { CaptchaType.ReCaptchaV2,  2 },
                { CaptchaType.ReCaptchaV3,  3 },
                { CaptchaType.FunCaptcha,   4 },
                { CaptchaType.HCaptcha,     5 },
                { CaptchaType.KeyCaptcha,   6 },
                { CaptchaType.GeeTest,      7 },
                { CaptchaType.Capy,         8 }
            };

            paramsTabControl.SelectedIndex = dict[CaptchaType];
        }

        private async void checkBalanceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show($"The balance is {await GetService(ServiceTypeName).GetBalanceAsync()}");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private async void solveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await Solve(GetService(ServiceTypeName), CaptchaType);

                switch (response)
                {
                    case StringResponse r:
                        MessageBox.Show($"The captcha was solved successfully!\r\nResponse: {r.Response}");
                        break;

                    case GeeTestResponse r:
                        MessageBox.Show($"The captcha was solved successfully!\r\nChallenge: {r.Challenge}\r\nValidate: {r.Validate}\r\nSecCode: {r.SecCode}");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private IEnumerable<Type> GetServiceTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(CaptchaService)));
        }

        private Type GetServiceType(string typeName)
        {
            return GetServiceTypes().First(t => t.Name == typeName);
        }

        private bool IsApiKeyService(Type type)
        {
            try { return type.GetConstructors().Any(c => c.GetParameters().First().Name == "apiKey"); }
            catch { return false; }
        }

        private bool IsUserPassService(Type type)
        {
            try { return type.GetConstructors().Any(c => c.GetParameters().First().Name == "username"); }
            catch { return false; }
        }

        private CaptchaService GetService(string serviceTypeName)
        {
            return GetService(GetServiceType(serviceTypeName));
        }

        private CaptchaService GetService(Type serviceType)
        {
            var timeout = TimeSpan.FromSeconds(double.Parse(Timeout));
            CaptchaService service;

            if (serviceType == typeof(CustomTwoCaptchaService) || serviceType == typeof(CapMonsterService))
            {
                service = Activator.CreateInstance(serviceType, new object[] { ApiKey, new Uri(CustomTwoCaptchaBaseUri), null }) as CaptchaService;
            }
            else
            {
                if (IsApiKeyService(serviceType))
                    service = Activator.CreateInstance(serviceType, new object[] { ApiKey, null }) as CaptchaService;
                else if (IsUserPassService(serviceType))
                    service = Activator.CreateInstance(serviceType, new object[] { Username, Password, null }) as CaptchaService;
                else
                    throw new NotSupportedException($"{serviceType} is not supported by the tester yet!");
            }

            service.Timeout = timeout;
            return service;
        }

        private async Task<CaptchaResponse> Solve(CaptchaService service, CaptchaType captchaType)
        {
            var proxy = MakeProxy(Proxy, ProxyType);

            switch (captchaType)
            {
                case CaptchaType.TextCaptcha:
                    var textOptions = new TextCaptchaOptions()
                    {
                        CaptchaLanguageGroup = CaptchaLanguageGroup,
                        CaptchaLanguage = CaptchaLanguage
                    };
                    return await service.SolveTextCaptchaAsync(Text, textOptions);

                case CaptchaType.ImageCaptcha:
                    if (CaptchaImage == null)
                        throw new ArgumentNullException("You must download an image first!");

                    var imageOptions = new ImageCaptchaOptions()
                    {
                        IsPhrase = IsPhrase,
                        CaseSensitive = CaseSensitive,
                        CharacterSet = CharacterSet,
                        RequiresCalculation = RequiresCalculation,
                        MinLength = int.Parse(MinLength),
                        MaxLength = int.Parse(MaxLength),
                        CaptchaLanguageGroup = CaptchaLanguageGroup,
                        CaptchaLanguage = CaptchaLanguage,
                        TextInstructions = TextInstructions
                    };
                    return await service.SolveImageCaptchaAsync(CaptchaImage.ToBase64(ImageFormat.Jpeg), imageOptions);

                case CaptchaType.ReCaptchaV2:
                    return await service.SolveRecaptchaV2Async(SiteKey, SiteUrl, SData, Enterprise, Invisible, proxy);

                case CaptchaType.ReCaptchaV3:
                    return await service.SolveRecaptchaV3Async(SiteKey, SiteUrl, Action,
                        float.Parse(MinScore, CultureInfo.InvariantCulture), Enterprise, proxy);

                case CaptchaType.FunCaptcha:
                    return await service.SolveFuncaptchaAsync(PublicKey, ServiceUrl, SiteUrl, NoJS, proxy);

                case CaptchaType.HCaptcha:
                    return await service.SolveHCaptchaAsync(SiteKey, SiteUrl, proxy);

                case CaptchaType.KeyCaptcha:
                    return await service.SolveKeyCaptchaAsync(UserId, SessionId, WebServerSign1, WebServerSign2,
                        SiteUrl, proxy);

                case CaptchaType.GeeTest:
                    return await service.SolveGeeTestAsync(GT, Challenge, ApiServer, SiteUrl, proxy);

                case CaptchaType.Capy:
                    return await service.SolveCapyAsync(SiteKey, SiteUrl, proxy);
            }

            throw new NotSupportedException($"Captcha type {captchaType} is not supported by the tester yet!");
        }

        private async void downloadImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CaptchaImage = await new HttpClient().DownloadBitmapAsync(captchaImageUrlTextbox.Text);
                captchaImage.Source = BitmapToImageSource(CaptchaImage);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            MessageBox.Show($"Oopsie! An exception of type {ex.GetType()} has been thrown!\r\nHere's the message: {ex}");
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private Proxy MakeProxy(string proxy, ProxyType type)
        {
            if (proxy == string.Empty)
                return null;

            Proxy p = new Proxy() { Type = type, UserAgent = UserAgent };

            if (proxy.Contains("@"))
            {
                var groups = Regex.Match(proxy, "^([^:]*):([^@]*)@([^:]*):(.*)$").Groups;
                
                p.Username = groups[1].Value;
                p.Password = groups[2].Value;
                p.Host = groups[3].Value;
                p.Port = int.Parse(groups[4].Value);
            }
            else
            {
                var split = proxy.Split(':');
                p.Host = split[0];
                p.Port = int.Parse(split[1]);
            }

            return p;
        }

        #region Combo Boxes SelectionChanged events
        private void languageGroupCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaptchaLanguageGroup = (CaptchaLanguageGroup)((ComboBox)e.OriginalSource).SelectedIndex;
        }

        private void languageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaptchaLanguage = (CaptchaLanguage)((ComboBox)e.OriginalSource).SelectedIndex;
        }

        private void imageCharacterSetCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CharacterSet = (CharacterSet)((ComboBox)e.OriginalSource).SelectedIndex;
        }

        private void proxyTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProxyType = (ProxyType)((ComboBox)e.OriginalSource).SelectedIndex;
        }
        #endregion
    }
}
