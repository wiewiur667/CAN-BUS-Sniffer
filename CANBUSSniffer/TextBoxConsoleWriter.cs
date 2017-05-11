using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CANBUSSniffer
{
    public class TextBoxConsoleWriter : TextWriter
    {
        private TextBox Control;
        private Dispatcher Dispatcher;
        private Queue<string> buffer = new Queue<string>();
        public TextBoxConsoleWriter(TextBox control, Dispatcher dispatcher)
        {
            Control = control;
            Dispatcher = dispatcher;
            Thread writer = new Thread(new ThreadStart(WriteToTextBox));
            writer.Start();
        }

        void WriteToTextBox()
        {
            while (true)
            {
                var builder = new StringBuilder();
                while (buffer.Count > 0)
                {
                    builder.AppendLine(buffer.Dequeue());
                }
                this.Dispatcher.BeginInvoke(new Action(() => {
                    var tbText = new StringBuilder();
                    tbText.Append(Control.Text);

                    Control.Text = tbText.ToString() + builder;
                    Control.ScrollToEnd();
                }));
                Thread.Sleep(100);

            }

        }

        public override void WriteLine(string value)
        {
            buffer.Enqueue(value);
        }

        public override Encoding Encoding => Encoding.ASCII;
    }
}
