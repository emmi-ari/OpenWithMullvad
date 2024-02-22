using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.NetworkInformation;
using System;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
namespace CeresDSP
{
    internal class Program
    {
        private static Task Main(string[] args) => new Program().MainAsync(args);

        private async Task MainAsync(string[] args)
        {
            #region Get up to date list of relays
            Tuple<string, string> update = await ExecuteCommand("mullvad relay update");
            if (update.Item2 == null || update.Item1.Trim() != "Updating relay list in the background...") return; // return if relay list can't be updated
            #endregion

            #region Parse list of relays
            Tuple<string, string> list = await ExecuteCommand("mullvad relay list");
            List<string> relayNames = [];
            if (list.Item2 == string.Empty && !string.IsNullOrEmpty(list.Item1))
            {
                Regex relayNamePattern = new(@"\t{2}([a-z]{2}-[a-z]{3}-[a-z]{2,4}-\d{3})", RegexOptions.Multiline);
                MatchCollection relayNameMatches = relayNamePattern.Matches(list.Item1);
                relayNames.AddRange(relayNameMatches.Select(relayNameMatch => relayNameMatch.Groups[1]?.Value));
            }
            #endregion

            #region Connection status/connection establishment
            Tuple<string, string> status = await ExecuteCommand("mullvad status");
            Tuple<string, string> connect = new(null, null);
            if (status.Item1.Trim() == "Disconnected")
            {
                await ConnectToMullvadRelay(status, connect, relayNames);
            }

            string connectedRelay = Regex.Match(status.Item1.Trim(), @"([a-z]{2}-[a-z]{3}-[a-z]{2,4}-\d{3})").Value;
            bool relayValida = relayNames.Where(relay => relay == connectedRelay).Count() == 1;
            if (relayValida) StartTargetApplication(args);
            else return; // return if connected relay cannot be validated
            #endregion
        }

        private void StartTargetApplication(string[] args)
        {
            throw new NotImplementedException();
        }

        static async Task<bool> ConnectToMullvadRelay(Tuple<string, string> status, Tuple<string, string> connect, List<string> relayNames)
        {
            connect = await ExecuteCommand("mullvad connect");
            await Console.Out.WriteLineAsync("Not connected to a Mullvad VPN relay. Connecting...");
            Thread.Sleep(2000);
            status = await ExecuteCommand("mullvad status");
            if (status.Item1.Trim() != "Disconnected")
            {
                string relayName = Regex.Match(status.Item1.Trim(), @"([a-z]{2}-[a-z]{3}-[a-z]{2,4}-\d{3})").Value;
                bool relayValidated = relayNames.Where(relay => relay == relayName).Count() == 1;
                if (relayValidated) return true;
            }
            else await Console.Out.WriteLineAsync("Connection to Mullvad VPN failed");
            return false;
        }

        static async Task<Tuple<string, string>> ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = "powershell.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"-Command {command}",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            Process process = new()
            {
                StartInfo = processStartInfo
            };

            process.Start();
            string stdo = null;
            string stderr = null;
            while (!process.StandardOutput.EndOfStream)
            {
                stdo = await process.StandardOutput.ReadToEndAsync();
                stderr = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();
            }

            Tuple<string, string> outputs = new(stdo, stderr);
            return outputs;
        }
    }
}
#pragma warning restore SYSLIB1045