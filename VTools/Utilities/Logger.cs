using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Utilities
{
    public class Logger : ObservableObject
    {
        public override string ToString()
        {
            OnPropertyChanged(nameof(ToString));
            return stringBuilder.ToString();
        }

        public void AppendLine(string line) => stringBuilder.AppendLine(line);

        public void Clear() => stringBuilder.Clear();

        private readonly StringBuilder stringBuilder = new();
    }
}
