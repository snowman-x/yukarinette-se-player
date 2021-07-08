using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using IrrKlang;
using Yukarinette;
using YukarinetteSePlayer.Data;
using YukarinetteSePlayer.Properties;
using YukarinetteSePlayer.ViewModel;

namespace YukarinetteSePlayer
{
    public class SePlayer : IYukarinetteFilterInterface
    {
        public override string Name => "ゆかりねっとSE再生";

        public override UserControl GetSettingUserControl()
        {
            return SettingPanel.Instance;
        }

        private DateTime CacheDate { get; set; }
        private Encoding SjisEncoding { get; } = Encoding.GetEncoding(932);
        private List<Record> Records { get; } = new List<Record>();

        public override YukarinetteFilterPluginResult Filtering(string text, YukarinetteWordDetailData[] words)
        {
            LoadCsvFile();
            var result = new YukarinetteFilterPluginResult
            {
                Text = Run(text)
            };
            return result;
        }

        private void LoadCsvFile()
        {
            if (string.IsNullOrEmpty(Settings.Default.CsvFileName))
            {
                SettingPanelViewModel.Instance.WriteLog("[ERROR] CSVファイルの場所が設定されていません");
                return;
            }

            var fileInfo = new FileInfo(Settings.Default.CsvFileName);
            if (!fileInfo.Exists)
            {
                SettingPanelViewModel.Instance.WriteLog("[ERROR] CSVファイルが設定された場所に存在しません");
                return;
            }

            DateTime time;
            try
            {
                time = fileInfo.LastWriteTime;
            }
            catch
            {
                time = fileInfo.CreationTime;
            }

            if (time <= CacheDate)
            {
                SettingPanelViewModel.Instance.WriteLog("[INFO] CSVファイルの読み込みをスキップします");
                return;
            }

            CacheDate = time;
            Records.Clear();

            try
            {
                using (var reader = new StreamReader(Settings.Default.CsvFileName, SjisEncoding))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine() ?? "";
                        var fields = line.Replace("\"", "").Split(',');

                        if (fields.Length < 5)
                        {
                            continue;
                        }

                        var record = FieldsToRecord(fields);
                        Records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                SettingPanelViewModel.Instance.WriteLog($"[ERROR] CSVファイルが読み込めませんでした({ex.Message})");
                return;
            }

            SettingPanelViewModel.Instance.WriteLog($"[INFO] CSVファイルを読み込みました({Records.Count:N0}件)"); 
        }

        private string Run(string text)
        {
            try
            {
                SettingPanelViewModel.Instance.WriteLog($"[WORD] `{text}`");

                var record = Find(text);
                if (record == null)
                {
                    SettingPanelViewModel.Instance.WriteLog("[INFO] 該当データなし");
                    return text;
                }

                SettingPanelViewModel.Instance.WriteLog($"[FIND] {record.Keyword}");
                if (string.IsNullOrWhiteSpace(record.FileName))
                {
                    return text;
                }

                var fileInfo = new FileInfo(record.FileName);
                if (!fileInfo.Exists)
                {
                    return text;
                }

                if (record.Latency < 0)
                {
                    Thread.Sleep(-record.Latency);
                }

                try
                {
                    var extension = fileInfo.Extension.ToLower();
                    if (extension == ".mp3" || extension == ".wav" || extension == ".ogg" || extension == ".flac")
                    {
                        Play(record);
                    }

                    SettingPanelViewModel.Instance.WriteLog($"[PLAY] {record.FileName}");
                }
                catch (Exception ex)
                {
                    SettingPanelViewModel.Instance.WriteLog("[ERROR] サウンドの再生に失敗しました");
                    SettingPanelViewModel.Instance.WriteLog($"例外:{ex.Message}");
                }

                if (0 < record.Latency)
                {
                    Thread.Sleep(record.Latency);
                }

                if (record.ReplaceMode == ReplaceMode.None)
                {
                    return "";
                }

                if (record.ReplaceMode == ReplaceMode.Replace)
                {
                    return record.ReplaceWord;
                }

                return text;
            }
            catch (Exception ex)
            {
                SettingPanelViewModel.Instance.WriteLog("[ERROR] エラーが発生しました");
                SettingPanelViewModel.Instance.WriteLog($"例外:{ex.Message}");
            }

            return text;
        }

        private Record FieldsToRecord(string[] fields)
        {
            var record = new Record();
            var runMode = fields[0];
            if (runMode == "完全")
            {
                record.RunMode = RunMode.Strict;
            }
            else if (runMode == "一部")
            {
                record.RunMode = RunMode.Fuzzy;
            }

            record.Keyword = fields[1];
            record.FileName = fields[2];

            if (int.TryParse(fields[3], out var latency))
            {
                record.Latency = latency;
            }

            if (int.TryParse(fields[4], out var replaceMode))
            {
                if (replaceMode == (int)ReplaceMode.Full)
                {
                    record.ReplaceMode = ReplaceMode.Full;
                }
                else if (replaceMode == (int)ReplaceMode.Replace)
                {
                    record.ReplaceMode = ReplaceMode.Replace;
                }
                else if (replaceMode == (int)ReplaceMode.None)
                {
                    record.ReplaceMode = ReplaceMode.None;
                }
            }

            record.ReplaceWord = fields.ElementAtOrDefault(5) ?? "";

            return record;
        }

        private Record Find(string text)
        {
            var str = (text ?? "").Replace(" ", "").Replace("　", "");
            foreach (var record in Records)
            {
                if (record.RunMode == RunMode.None)
                {
                    continue;
                }

                if (record.RunMode == RunMode.Strict && str != record.Keyword)
                {
                    continue;
                }

                if (record.RunMode == RunMode.Fuzzy && str.Contains(record.Keyword))
                {
                    continue;
                }

                return record;
            }

            return null;
        }

        private void Play(Record record)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.DeviceId))
            {
                return;
            }

            var deviceId = Settings.Default.DeviceId;
            Task.Run(() =>
            {
                using (var engine = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions, deviceId))
                {
                    engine.Play2D(record.FileName);

                    while (engine.IsCurrentlyPlaying(record.FileName))
                    {
                        Thread.Sleep(1);
                    }
                }
            });
        }
    }
}