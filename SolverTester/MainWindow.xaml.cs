using CaptchaSharp;
using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using CaptchaSharp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SolverTester
{
    public partial class MainWindow : Window
    {
        public CaptchaServiceType ServiceType { get; set; } = CaptchaServiceType.TwoCaptcha;
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

        // RecaptchaV2 / RecaptchaV3 / HCaptcha
        public string SiteUrl { get; set; } = "";

        // RecaptchaV2
        public bool Invisible { get; set; } = false;

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

            #region Setup Combo Boxes
            foreach (var s in Enum.GetNames(typeof(CaptchaServiceType)))
                serviceCombobox.Items.Add(s);

            serviceCombobox.SelectedIndex = (int)ServiceType;

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
            ServiceType = (CaptchaServiceType)((ComboBox)e.OriginalSource).SelectedIndex;
            configTabControl.SelectedIndex = 0;

            switch (ServiceType)
            {
                case CaptchaServiceType.TwoCaptcha:
                case CaptchaServiceType.AntiCaptcha:
                    authTabControl.SelectedIndex = 0;
                    break;

                case CaptchaServiceType.CustomTwoCaptcha:
                    authTabControl.SelectedIndex = 0;
                    configTabControl.SelectedIndex = 1;
                    break;
            }
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
                { CaptchaType.GeeTest,      7 }
            };

            paramsTabControl.SelectedIndex = dict[CaptchaType];
        }

        private async void checkBalanceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show($"The balance is {await GetService(ServiceType).GetBalanceAsync()}");
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
                var response = await Solve(GetService(ServiceType), CaptchaType);

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

        private CaptchaService GetService(CaptchaServiceType service)
        {
            var timeout = TimeSpan.FromSeconds(double.Parse(Timeout));

            switch (service)
            {
                case CaptchaServiceType.TwoCaptcha:
                    return new TwoCaptchaService(ApiKey) { Timeout = timeout };

                case CaptchaServiceType.CustomTwoCaptcha:
                    return new CustomTwoCaptchaService(ApiKey, new Uri(CustomTwoCaptchaBaseUri)) { Timeout = timeout };

                case CaptchaServiceType.AntiCaptcha:
                    return new AntiCaptchaService(ApiKey) { Timeout = timeout };
            }

            throw new NotSupportedException($"Service {service} is not supported by the tester yet!");
        }

        private async Task<CaptchaResponse> Solve(CaptchaService service, CaptchaType captchaType)
        {
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
                    return await service.SolveImageCaptchaAsync(CaptchaImage, null, imageOptions);

                case CaptchaType.ReCaptchaV2:
                    return await service.SolveRecaptchaV2Async(SiteKey, SiteUrl, Invisible, MakeProxy(Proxy, ProxyType));

                case CaptchaType.ReCaptchaV3:
                    return await service.SolveRecaptchaV3Async(SiteKey, SiteUrl, Action,
                        float.Parse(MinScore, CultureInfo.InvariantCulture), MakeProxy(Proxy, ProxyType));

                case CaptchaType.FunCaptcha:
                    return await service.SolveFuncaptchaAsync(PublicKey, ServiceUrl, SiteUrl, NoJS, MakeProxy(Proxy, ProxyType));

                case CaptchaType.HCaptcha:
                    return await service.SolveHCaptchaAsync(SiteKey, SiteUrl, MakeProxy(Proxy, ProxyType));

                case CaptchaType.KeyCaptcha:
                    return await service.SolveKeyCaptchaAsync(UserId, SessionId, WebServerSign1, WebServerSign2,
                        SiteUrl, MakeProxy(Proxy, ProxyType));

                case CaptchaType.GeeTest:
                    return await service.SolveGeeTestAsync(GT, Challenge, ApiServer, SiteUrl, MakeProxy(Proxy, ProxyType));
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
            MessageBox.Show($"Oopsie! An exception of type {ex.GetType()} has been thrown!\r\nHere's the message: {ex.Message}");
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

            Proxy p = new Proxy() { Type = type };

            if (proxy.Contains("@"))
            {
                var split = proxy.Split('@');
                var creds = split[0];
                var split2 = creds.Split(':');
                p.Username = split2[0];
                p.Password = split2[1];
                proxy = split[1];
            }

            var split3 = proxy.Split(':');
            p.Host = split3[0];
            p.Port = int.Parse(split3[1]);

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
