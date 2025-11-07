using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ThreadSyncDemo
{
    public partial class MainWindow : Window
    {
        List<int> numbers = new List<int>();
        object locker = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartRace_Click(object sender, RoutedEventArgs e)
        {
            numbers.Clear();
            Task t1 = Task.Run(AddItems);
            Task t2 = Task.Run(AddItems);
            Task.WhenAll(t1, t2).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => ResultText.Text = $"Total elements: {numbers.Count}");
            });
        }

        private void StartSafe_Click(object sender, RoutedEventArgs e)
        {
            numbers.Clear();
            Task t1 = Task.Run(() => AddItemsSafe());
            Task t2 = Task.Run(() => AddItemsSafe());
            Task.WhenAll(t1, t2).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => ResultText.Text = $"Total elements: {numbers.Count}");
            });
        }

        private void CheckMonitor_Click(object sender, RoutedEventArgs e)
        {
            bool entered = false;
            try
            {
                entered = Monitor.TryEnter(locker, 1000);
                if (entered)
                {
                    ResultText.Text = "Monitor.TryEnter: Access granted";
                }
                else
                {
                    ResultText.Text = "Monitor.TryEnter: Timeout";
                }
            }
            finally
            {
                if (entered) Monitor.Exit(locker);
            }
        }

        void AddItems()
        {
            for (int i = 0; i < 10000; i++)
                numbers.Add(i);
        }

        void AddItemsSafe()
        {
            for (int i = 0; i < 10000; i++)
            {
                lock (locker)
                {
                    numbers.Add(i);
                }
            }
        }
    }
}