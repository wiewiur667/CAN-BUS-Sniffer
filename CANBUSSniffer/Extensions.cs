
using System.Windows.Controls;
using System.Windows.Data;

namespace CANBUSSniffer
{
    public static class Extensions
    {
        public static void GenerateMessagesColumns(this MessagesDataGrid grid)
        {
            grid.Columns.Clear();
            var idBinding = new Binding("Id")
            {
                FallbackValue = "00",
                StringFormat = "{0:X2}"
            };
            var idColumn = new DataGridTextColumn
            {
                Header = "Id",

                Binding = idBinding
            };

            //grid.Columns.Add(idColumn);
            for (int i = 0; i < 8; i++)
            {
                var dataBinding = new Binding($"Data[{i}]")
                {
                    TargetNullValue = "",
                    NotifyOnValidationError= false,
                    FallbackValue = "00",
                    StringFormat="{0:X2}",

                };
                var tempColumn = new DataGridTextColumn
                {
                    Header = $"Data {i:X2}",
                    Binding = dataBinding,
                };

                grid.Columns.Add(tempColumn);
            }
        }
    }
}
