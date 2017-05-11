using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO.Ports;
using System.IO;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using MoreLinq;
using System.Windows.Threading;
using System.Runtime;

namespace CANBUSSniffer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public SerialPort serialPort { get; set; }

        public bool IsOpen { get { return serialPort.IsOpen; } }

        string _comName = "COM6";
        public string ComName
        {
            get => _comName;
            set
            {
                _comName = value;
                NotifyPropertyChanged();
            }
        }

        int _comSpeed = 115200;
        public int ComSpeed
        {
            get => _comSpeed;
            set
            {
                _comSpeed = value;
                NotifyPropertyChanged();
            }
        }

        private Queue<CanMessage> _dataBuffer = new Queue<CanMessage>();
        public Queue<CanMessage> DataBuffer
        {
            get => _dataBuffer;
            set
            {
                _dataBuffer = value;
                NotifyPropertyChanged();


            }
        }


        private StringBuilder _log = new StringBuilder();
        public StringBuilder Log
        {
            get => _log;
            set
            {
                _log = value;
                NotifyPropertyChanged();
            }
        }

        private readonly object _lockObject = new object();

        public int _bufferSize = 512;
        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                _bufferSize = value;
                NotifyPropertyChanged();
            }
        }

        public CanMessages DataList { get; set; } = new CanMessages();
        //List<CanMessage> DataList { get; set; } = new List<CanMessage>();
        ListCollectionView collection;
        List<TextBox> filterBoxes;
        public MainWindow()
        {
            InitializeComponent();
            serialPort = new SerialPort(ComName, ComSpeed);
            this.DataContext = this;
            StartMenuBtn.IsEnabled = true;
            serialPort.ReadBufferSize = 512;
            serialPort.DataReceived += SerialPort_DataReceived;
            //var tbWriter = new TextBoxConsoleWriter(ConsoleLogTb, this.Dispatcher);

            MessagesDataGrid.GenerateMessagesColumns();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += UppdateUi;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimer.Start();

            collection = new ListCollectionView(DataList);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("Id"));
            MessagesDataGrid.ItemsSource = collection;
            filterBoxes = FilterSP.Children.OfType<TextBox>().Select(a => {
                a.PreviewTextInput += ComSpeedTextBox_PreviewTextInput;
                a.KeyUp += A_KeyUp;
                return a;
            }).ToList();
        }

        private void A_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void UppdateUi(object sender, EventArgs e)
        {
            var oldMode = GCSettings.LatencyMode;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                if (serialPort.IsOpen)
                {
                    while (DataBuffer.Count > 0)
                    {
                        var item = DataBuffer.Dequeue();
                        if (DataList.Count > _bufferSize) DataList.RemoveAt(0);
                        if(!DataList.Contains(item))
                        DataList.Add(item);
                        //collection.AddNewItem(item);
                    }
                }
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }

        }

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var oldMode = GCSettings.LatencyMode;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                var serialDataStringResult = serialPort.ReadLine();
                if (DataParser.VerifyData(serialDataStringResult))
                {
                    var message = DataParser.ParseToCanMessage(serialDataStringResult);
                    DataBuffer.Enqueue(message);
                }
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
        }

        void ComSpeedTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumberAllowed(e.Text);
        }

        static bool IsNumberAllowed(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }

        void Start_Menu_Click(object sender, RoutedEventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.BaudRate = ComSpeed;
                    serialPort.PortName = ComName;
                    serialPort.Open();
                    StartMenuBtn.Header = "Stop";

                }
                catch (IOException ie)
                {
                    MessageBox.Show("Port NOT available!");
                }
            }
            else
            {
                serialPort.Close();
                StartMenuBtn.Header = "Start";
            }
            NotifyPropertyChanged("IsOpen");

        }

        void ComSettingsTextBoxes_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ComSpeed = int.Parse(ComSpeedTextBox.Text.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid Speed {ex}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Clear_Menu_Click(object sender, RoutedEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(() => Messages.Clear()));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            serialPort.Close();
            serialPort.Dispose();
        }

        private void MessagesDataGrid_CleanUpVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs e)
        {

        }
    }
}
