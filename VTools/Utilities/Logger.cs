using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Utilities
{
    public class Logger : ObservableObject
    {
        public ObservableCollection<string> Lines => logs;

        public void AppendLine(string line) => logs.Add(line.Trim(' ', '\n', '\r'));

        public void Clear() => logs.Clear();

        private readonly ObservableCollection<string> logs = [];
    }
}
