using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Utilities
{
    public class Logger : ObservableObject
    {
        public string String => stringBuilder.ToString();

        public void AppendLine(string str)
        {
            stringBuilder.AppendLine(str);
            OnPropertyChanged(nameof(String));
        }

        public void Clear()
        {
            stringBuilder.Clear();
            OnPropertyChanged(nameof(String));
        }

        private readonly StringBuilder stringBuilder = new();
    }
}
