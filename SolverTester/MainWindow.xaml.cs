using CaptchaSharp;
using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using CaptchaSharp.Services;
using System;
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

        // RecaptchaV2
        public string SiteKey { get; set; }
        public string Url { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            foreach (var s in Enum.GetNames(typeof(CaptchaServiceType)))
                serviceCombobox.Items.Add(s);

            serviceCombobox.SelectedIndex = (int)ServiceType;

            foreach (var c in Enum.GetNames(typeof(CaptchaType)))
                captchaTypeCombobox.Items.Add(c);

            captchaTypeCombobox.SelectedIndex = (int)CaptchaType;
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

            switch (CaptchaType)
            {
                case CaptchaType.ReCaptchaV2:
                    authTabControl.SelectedIndex = 0;
                    break;
            }
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
                MessageBox.Show($"The captcha was solved successfully! Response: {await Solve(GetService(ServiceType), CaptchaType)}");
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
                case CaptchaType.ReCaptchaV2:
                    return await service.SolveRecaptchaV2Async(SiteKey, Url);
            }

            throw new NotSupportedException($"Captcha type {captchaType} is not supported by the tester yet!");
        }
    }
}
