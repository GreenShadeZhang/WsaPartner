using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WsaPartner.APKViewer
{
    public static class ProcessExecuter
    {
        static List<int> indexes = new List<int>();
        public static async Task<string> ExecuteProcess(ProcessStartInfo startInfo, bool useBothErrorAndNormal = false, bool useUTF8 = true)
        {
            // string result = string.Empty;

            using (Process process = new Process())
            {
                //CultureInfo.CurrentCulture.TextInfo.OEMCodePage
                if (useUTF8)
                {
                    startInfo.StandardOutputEncoding = Encoding.UTF8;
                    startInfo.StandardErrorEncoding = Encoding.UTF8;
                }

                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.EnableRaisingEvents = true;

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() setup: \r\n" + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

                StringBuilder output = new StringBuilder();

                TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                process.OutputDataReceived +=
                    (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            //Console.WriteLine("ProcessExecuter.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
                            output.AppendLine(e.Data);
                        }
                    };

                if (useBothErrorAndNormal)
                {
                    process.ErrorDataReceived +=
                        (sender, e) =>
                        {
                            if (e.Data != null)
                            {
                                //Console.WriteLine("ProcessExecuter.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
                                output.AppendLine(e.Data);
                            }
                        };
                }
                process.Exited += (sender, e) => { tcs.SetResult(output.ToString()); };

                process.Start();

                process.BeginOutputReadLine();

                process.BeginErrorReadLine();


                Debug.WriteLine("ProcessExecuter.ExecuteProcess() process started.");

                await tcs.Task;

                await Task.Delay(50);

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() Final result=\r\n" + output.ToString());

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() finish.");

                return output.ToString();
            }
        }

        public static Task<Dictionary<string, string>> GetIconNameProcess(ProcessStartInfo startInfo, bool useBothErrorAndNormal = false, bool useUTF8 = true, string iconKey = "")
        {
            // string result = string.Empty;

            using (Process process = new Process())
            {
                //CultureInfo.CurrentCulture.TextInfo.OEMCodePage
                if (useUTF8)
                {
                    startInfo.StandardOutputEncoding = Encoding.UTF8;
                    startInfo.StandardErrorEncoding = Encoding.UTF8;
                }

                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.EnableRaisingEvents = true;

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() setup: \r\n" + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

                process.Start();

                //process.BeginOutputReadLine();

                //process.BeginErrorReadLine();

                var outPutList = new Dictionary<string, string>();

                var strList = new List<string>();

                TaskCompletionSource<Dictionary<string, string>> tcs = new TaskCompletionSource<Dictionary<string, string>>();

                var configNames = Enum.GetNames(typeof(Configs)).Reverse();

                var terminated = false;

                int index = 0;

                while (!process.StandardOutput.EndOfStream && !terminated)
                {
                    var msg = process.StandardOutput.ReadLine();

                    if (CallbackMethod(msg, index, iconKey))
                    {
                        terminated = true;

                        try
                        {
                            process.Kill();
                        }
                        catch { }
                    }
                    if (!terminated)
                        index++;
                    strList.Add(msg);
                }

                outPutList = SetIconList(indexes, strList);

                //if (useBothErrorAndNormal)
                //{
                //    process.ErrorDataReceived +=
                //        (sender, e) =>
                //        {
                //            if (e.Data != null)
                //            {
                //                //Console.WriteLine("ProcessExecuter.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
                //                outPutList.Add(e.Data);
                //            }
                //        };
                //}

                //process.Exited += (sender, e) => { tcs.SetResult(outPutList); };

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() process started.");

                //await tcs.Task;

                //await Task.Delay(50);

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() Final result=\r\n" + outPutList);

                Debug.WriteLine("ProcessExecuter.ExecuteProcess() finish.");

                return Task.FromResult(outPutList);
            }
        }

        public static bool CallbackMethod(string msg, int index, string iconKey)
        {
            var matchedEntry = false;            

            if (Regex.IsMatch(msg, $"^\\s*resource\\s{iconKey}"))

                indexes.Add(index);

            if (!matchedEntry)
            {
                if (msg.Contains("mipmap/"))
                    matchedEntry = true;    // Begin mipmap entry
            }
            else
            {
                if (Regex.IsMatch(msg, $"^\\s*type\\s\\d*\\sconfigCount=\\d*\\sentryCount=\\d*$"))
                {  // Next entry, terminate
                    matchedEntry = false;
                    return true;
                }
            }
            return false;
        }

        private static Dictionary<string, string> SetIconList(List<int> positions, List<string> messages)
        {
            string configEnum =
            string.Join("|", System.Enum.GetNames(typeof(Configs)));

            if (positions.Count == 0 || messages.Count <= 2)
                return new Dictionary<string, string>();

            const char seperator = '\"';
            // Prevent duplicate key when add to Dictionary,
            // because comparison statement with 'hdpi' in config's values,
            // reverse list and get first elem with LINQ
            var configNames = Enum.GetNames(typeof(Configs)).Reverse();

            var iconList = new Dictionary<string, string>();

            Action<string, string> addIcon2Table = (cfg, iconName) => 
                {
                if (!iconList.ContainsKey(cfg))
                {
                    iconList.Add(cfg,iconName);
                }
            };
            string msg, resValue, config;

            foreach (int index in positions)
            {
                for (int i = index; ; i--)
                {
                    // Go prev to find config
                    msg = messages[i];

                    if (Regex.IsMatch(msg, $"^\\s*type\\s\\d*\\sconfigCount=\\d*\\sentryCount=\\d*$"))  // Out of entry and not found
                        break;
                    if (Regex.IsMatch(msg, $"^\\s*config\\s\\(?({configEnum})(-v\\d*)?\\)?:"))
                    {
                        // Match with predefined configs,
                        // go next to get icon name
                        resValue = messages[index + 1];

                        config = configNames.FirstOrDefault(c => msg.Contains(c));

                        if (Regex.IsMatch(resValue, @"^\s*\((string\d*)\)*"))
                        {
                            // Resource value is icon url
                            var iconName = resValue.Split(seperator)
                                .FirstOrDefault(n => n.Contains("/"));
                            addIcon2Table(config, iconName);
                            break;
                        }
                        if (Regex.IsMatch(resValue, @"^\s*\((reference)\)*"))
                        {
                            var iconID = resValue.Trim().Split(' ')[1];
                            addIcon2Table(config, iconID);
                            break;
                        }

                        break;
                    }
                }
            }
            return iconList;
        }
    }
}