using Core;
using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace TagFinder
{
    public partial class MainWindow : Window
    {
        private UrlResult[] urlResults;
        private CancellationTokenSource tokenSource;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ChoiceUrlsFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file(*.txt) | *.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileText = File.ReadAllText(openFileDialog.FileName);
                if (fileText.Length == 0)
                {
                    MessageBox.Show("Выбранный файл пустой");
                    return;
                }

                FileParser fileParser = new();
                string[] urls = fileParser.Parse(fileText);
                  

                pathTextBlock.Text = openFileDialog.FileName;
                maxResultTextBlock.Text = "Здесь будет максимальный результат";

                urlResults = urls.Select(x => new UrlResult(x, 0)).ToArray();
                dataGrid.ItemsSource = urlResults;
            }
        }

        private async void StartClick(object sender, RoutedEventArgs e)
        {
            tokenSource = new();

            TagParser parser = new();

            ConnectToParser(parser);
            await parser.ParseAsync(urlResults, tokenSource.Token);
            DisonnectFromParser(parser);
        }


        private void ShowResults()
        {
            dataGrid.ItemsSource = urlResults.OrderByDescending(x => x.Count);

            var max = urlResults.MaxBy(x => x.Count);
            maxResultTextBlock.Text = $"{max.Url}:{max.Count}";
        }

        private void CancelOperationClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }

        private void ConnectToParser(TagParser parser)
        {
            parser.OnStarted += CloseCurtains;
            parser.OnProgressUpdated += UpdateProgress;
            parser.OnFinished += OpenCurtains;
            parser.OnFinished += ShowResults;
        }

        private void DisonnectFromParser(TagParser parser)
        {
            try
            {
                parser.OnStarted -= CloseCurtains;
                parser.OnProgressUpdated -= UpdateProgress;
                parser.OnFinished -= OpenCurtains;
                parser.OnFinished -= ShowResults;
            }
            catch { }
        }

        private void CloseCurtains()
        {
            curtains.Visibility = System.Windows.Visibility.Visible;
        }

        private void OpenCurtains()
        {
            curtains.Visibility = System.Windows.Visibility.Hidden;
        }

        private void UpdateProgress(float progress)
        {
            progressTextBlock.Text = $"{Math.Round(progress * 100)}%";
        }

    }
}
