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
using System.Windows.Media;
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

        // RecaptchaV2 / RecaptchaV3 / HCaptcha
        public string SiteKey { get; set; } = "";
        public string Url { get; set; } = "";
        public bool Invisible { get; set; } = false;

        // RecaptchaV3
        public string Action { get; set; } = "";
        public string MinScore { get; set; } = "0.3";

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
            #endregion
        }

        private void serviceCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceType = (CaptchaServiceType)((ComboBox)e.OriginalSource).SelectedIndex;

            switch (ServiceType)
            {
                case CaptchaServiceType.TwoCaptcha:
                    authTabControl.SelectedIndex = 0;
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
                { CaptchaType.HCaptcha,     2 },
                { CaptchaType.ReCaptchaV3,  3 }
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
                MessageBox.Show($"Oopsie! An exception of type {ex.GetType()} has been thrown!\r\nHere's the message: {ex.Message}");
            }
        }

        private async void solveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var solution = await Solve(GetService(ServiceType), CaptchaType);
                MessageBox.Show($"The captcha was solved successfully! Response: {solution.Response}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Oopsie! An exception of type {ex.GetType()} has been thrown!\r\nHere's the message: {ex.Message}");
            }
        }

        private CaptchaService GetService(CaptchaServiceType service)
        {
            switch (service)
            {
                case CaptchaServiceType.TwoCaptcha:
                    return new TwoCaptchaService(ApiKey);
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
                    return await service.SolveRecaptchaV2Async(SiteKey, Url, Invisible);

                case CaptchaType.ReCaptchaV3:
                    return await service.SolveRecaptchaV3Async(SiteKey, Url, Action, float.Parse(MinScore, CultureInfo.InvariantCulture));

                case CaptchaType.HCaptcha:
                    return await service.SolveHCaptchaAsync(SiteKey, Url);
            }

            throw new NotSupportedException($"Captcha type {captchaType} is not supported by the tester yet!");
        }

        private async void downloadImageButton_Click(object sender, RoutedEventArgs e)
        {
            CaptchaImage = await new HttpClient().DownloadBitmapAsync(captchaImageUrlTextbox.Text);
            captchaImage.Source = BitmapToImageSource(CaptchaImage);
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
        #endregion
    }
}
