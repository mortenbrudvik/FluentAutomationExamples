using System;
using System.Diagnostics;
using System.Linq;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Application = FlaUI.Core.Application;

namespace AutomationConsole
{
    public class Program
    {
        private static void Main()
        {
            static void MeasureTime(Action searchMethod)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                searchMethod();

                stopwatch.Stop();
                Console.Out.WriteLine($"Time: {stopwatch.ElapsedMilliseconds/ 1000f} seconds");
            }

            var process = CreateProcess("chrome.exe", "--new-window www.vg.no");

            process.Start();
            var app = Application.Attach(process);
            
            using var automation = new UIA3Automation();
            
            var window =  app.GetMainWindow(automation);
            var conditionFactory = new ConditionFactory(new UIA3PropertyLibrary());

            MeasureTime(() =>
            {
                var url = window.FindAllDescendants(conditionFactory.ByControlType(ControlType.Edit))
                    .Select(x => (string)x?.Patterns.Value.Pattern.Value)
                    .Where(x => !string.IsNullOrEmpty(x)).ToList()
                    .FirstOrDefault();
                
                Console.Out.WriteLine("FindAllDescendants Result: " + url);
            });

            MeasureTime(() =>
            {
                var editElement = window.FindFirstByXPath("//Edit");
                var url = editElement?.Patterns.Value.Pattern.Value;
                Console.Out.WriteLine("FindFirstByXPath Result: " + url);
            });

            window.Close();
        }
        
        public static Process CreateProcess(string filePath, string arguments = "") =>
            new Process { StartInfo = new ProcessStartInfo(filePath, arguments)
                { UseShellExecute = true, WorkingDirectory = "", WindowStyle = ProcessWindowStyle.Minimized}};
    }
}
