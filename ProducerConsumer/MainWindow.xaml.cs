using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProducerConsumerDemo
{
    public partial class MainWindow : Window
    {
        List<int> queue = new List<int>();
        object locker = new object();
        bool running = true;
        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            LogText.Text = "";
            running = true;
            Task.Run(Producer);
            Task.Run(Consumer);
        }

        void Producer()
        {
            while (running)
            {
                lock (locker)
                {
                    queue.Add(++counter);
                    Log($"Added item {counter}");
                    Monitor.Pulse(locker);
                }
                Thread.Sleep(500);
            }
        }

        void Consumer()
        {
            while (running)
            {
                int item = 0;
                lock (locker)
                {
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(locker);
                    }
                    item = queue[0];
                    queue.RemoveAt(0);
                }
                Log($"Processed item {item}");
                Thread.Sleep(1000);
            }
        }

        void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogText.Text += message + "\n";
            });
        }
    }
}