using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls; // Добавлено для TabControl
using NetworkAnalyzer.Models;
using NetworkAnalyzer.Services;

namespace NetworkAnalyzer
{
    public partial class MainWindow : Window
    {
        private readonly NetworkService _networkService;
        private readonly UrlAnalysisService _urlAnalysisService;
        private readonly List<UrlAnalysisResult> _history;

        public MainWindow()
        {
            InitializeComponent();
            _networkService = new NetworkService();
            _urlAnalysisService = new UrlAnalysisService();
            _history = new List<UrlAnalysisResult>();

            Loaded += MainWindow_Loaded; // Используем событие Loaded вместо прямого вызова
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNetworkInterfaces();
            UpdateStatus("Приложение запущено");
        }

        private void LoadNetworkInterfaces()
        {
            try
            {
                var interfaces = _networkService.GetNetworkInterfaces();
                InterfacesListBox.ItemsSource = interfaces;
                UpdateStatus($"Загружено {interfaces.Count} сетевых интерфейсов");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки интерфейсов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InterfacesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InterfacesListBox.SelectedItem is NetworkInterfaceInfo selectedInterface)
            {
                DisplayInterfaceInfo(selectedInterface);
            }
        }

        private void DisplayInterfaceInfo(NetworkInterfaceInfo info)
        {
            InterfaceNameText.Text = info.Name;
            InterfaceDescText.Text = info.Description;
            InterfaceIpText.Text = info.IpAddress;
            InterfaceMaskText.Text = info.SubnetMask;
            InterfaceMacText.Text = info.MacAddress;
            InterfaceStatusText.Text = info.Status.ToString();
            StatusIndicator.Fill = new SolidColorBrush(
                info.Status == System.Net.NetworkInformation.OperationalStatus.Up ?
                Colors.Green : Colors.Red);
            InterfaceSpeedText.Text = info.SpeedFormatted;
            InterfaceTypeText.Text = info.InterfaceType.ToString();
        }

        private void AnalyzeUrlButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UrlInputTextBox.Text))
            {
                MessageBox.Show("Введите URL для анализа", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UpdateStatus("Выполняется анализ URL...");

                var result = _urlAnalysisService.AnalyzeUrl(UrlInputTextBox.Text);
                DisplayUrlAnalysis(result);

                _history.Add(result);
                UpdateHistoryList();

                UpdateStatus("Анализ URL завершен успешно");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Ошибка анализа URL");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Ошибка анализа URL");
            }
        }

        private void DisplayUrlAnalysis(UrlAnalysisResult result)
        {
            UrlSchemeText.Text = result.Scheme;
            UrlHostText.Text = result.Host;
            UrlPortText.Text = result.Port.ToString();
            UrlPathText.Text = result.Path;
            UrlQueryText.Text = string.IsNullOrEmpty(result.Query) ? "Нет" : result.Query;
            UrlFragmentText.Text = string.IsNullOrEmpty(result.Fragment) ? "Нет" : result.Fragment;

            AddressTypeText.Text = result.AddressType;

            DnsAddressesList.ItemsSource = result.DnsAddresses;

            PingIndicator.Fill = new SolidColorBrush(result.IsPingSuccessful ? Colors.Green : Colors.Red);
            PingResultText.Text = result.IsPingSuccessful ?
                $"Успешно ({result.PingTime} мс)" : "Недоступен";
        }

        private void UpdateHistoryList()
        {
            HistoryListBox.ItemsSource = null;
            HistoryListBox.ItemsSource = _history;
        }

        private void HistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is UrlAnalysisResult selectedResult)
            {
                DisplayUrlAnalysis(selectedResult);
                UrlInputTextBox.Text = selectedResult.OriginalUrl;

                // Находим TabControl и переключаемся на вкладку анализа
                var tabControl = FindName("MainTabControl") as TabControl;
                if (tabControl != null)
                {
                    tabControl.SelectedIndex = 1;
                }
            }
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _history.Clear();
            UpdateHistoryList();
            UpdateStatus("История очищена");
        }

        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }
    }
}