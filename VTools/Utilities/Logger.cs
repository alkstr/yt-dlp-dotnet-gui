using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTools.Utilities
{
    public class Logger : ObservableObject
    {
        public ObservableCollection<string> Lines => logs;

        public void AppendLine(string line)
        {
            line = line.Trim(' ', '\n', '\r');
            if (!string.IsNullOrWhiteSpace(line)) { logs.Add(line); }
        }

        public void Clear() => logs.Clear();

        private readonly ObservableCollection<string> logs = [];
    }
}
