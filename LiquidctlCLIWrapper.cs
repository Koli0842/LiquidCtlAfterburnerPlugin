using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace LiquidCtlAfterburnerPlugin
{
    internal static class LiquidctlCLIWrapper
    {
        public static string liquidctlexe = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\liquidctl.exe";
        internal static void Initialize()
        {
            LiquidctlCall($"--json initialize all");
        }
        internal static List<LiquidctlStatusJSON> ReadStatus()
        {
            Process process = LiquidctlCall($"--json status");
            return JsonConvert.DeserializeObject<List<LiquidctlStatusJSON>>(process.StandardOutput.ReadToEnd());
        }
        internal static List<LiquidctlStatusJSON> ReadStatus(string address)
        {
            Process process = LiquidctlCall($"--json --address {address} status");
            return JsonConvert.DeserializeObject<List<LiquidctlStatusJSON>>(process.StandardOutput.ReadToEnd());
        }

        private static Process LiquidctlCall(string arguments)
        {
            //File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: Invoking liquidctl\r\n");
            Process process = new Process();

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.FileName = liquidctlexe;
            process.StartInfo.Arguments = arguments;

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"liquidctl returned non-zero exit code {process.ExitCode}. Last stderr output:\n{process.StandardError.ReadToEnd()}");
            }

            return process;
        }
    }
}
