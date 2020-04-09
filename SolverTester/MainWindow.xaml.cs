using CaptchaSharp;
using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using CaptchaSharp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SolverTester
{
    public partial class MainWindow : Window
    {
        public CaptchaServiceType ServiceType { get; set; } = CaptchaServiceType.TwoCaptcha;
        public CaptchaType CaptchaType { get; set; } = CaptchaType.ReCaptchaV2;

        // Authentication
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // TextCaptcha / ImageCaptcha
        public string Text { get; set; }
        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;
        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;

        // RecaptchaV2 / RecaptchaV3 / HCaptcha
        public string SiteKey { get; set; }
        public string Url { get; set; }
        public bool Invisible { get; set; }

        // RecaptchaV3
        public string Action { get; set; }
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
                textLanguageGroupCombobox.Items.Add(g);

            textLanguageGroupCombobox.SelectedIndex = (int)CaptchaLanguageGroup;

            foreach (var l in Enum.GetNames(typeof(CaptchaLanguage)))
                textLanguageCombobox.Items.Add(l);

            textLanguageCombobox.SelectedIndex = (int)CaptchaLanguage;
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
                { CaptchaType.ReCaptchaV2,  1 },
                { CaptchaType.HCaptcha,     1 },
                { CaptchaType.ReCaptchaV3,  2 }
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
                        LanguageGroup = CaptchaLanguageGroup,
                        Language = CaptchaLanguage
                    };
                    return await service.SolveTextCaptchaAsync(Text, textOptions);

                case CaptchaType.ReCaptchaV2:
                    return await service.SolveRecaptchaV2Async(SiteKey, Url, Invisible);

                case CaptchaType.ReCaptchaV3:
                    return await service.SolveRecaptchaV3Async(SiteKey, Url, Action, float.Parse(MinScore, CultureInfo.InvariantCulture));

                case CaptchaType.HCaptcha:
                    return await service.SolveHCaptchaAsync(SiteKey, Url);
            }

            throw new NotSupportedException($"Captcha type {captchaType} is not supported by the tester yet!");
        }

        #region Combo Boxes SelectionChanged events
        private void textLanguageGroupCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaptchaLanguageGroup = (CaptchaLanguageGroup)((ComboBox)e.OriginalSource).SelectedIndex;
        }

        private void textLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaptchaLanguage = (CaptchaLanguage)((ComboBox)e.OriginalSource).SelectedIndex;
        }
        #endregion
    }
}
