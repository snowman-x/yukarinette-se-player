using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using IrrKlang;
using Microsoft.Win32;
using YukarinetteSePlayer.Annotations;
using YukarinetteSePlayer.Core;
using YukarinetteSePlayer.Data;
using YukarinetteSePlayer.Properties;

namespace YukarinetteSePlayer.ViewModel
{
    public class SettingPanelViewModel : INotifyPropertyChanged
    {
        public static SettingPanelViewModel Instance { get; private set; }

        public SettingPanelViewModel()
        {
            using (var soundDevices = new ISoundDeviceList(SoundDeviceListType.PlaybackDevice))
            {
                foreach (var i in Enumerable.Range(0, soundDevices.DeviceCount))
                {
                    var id = soundDevices.getDeviceID(i);
                    var description = soundDevices.getDeviceDescription(i);
                    var soundDevice = new SoundDevice(id, description);
                    SoundDevices.Add(soundDevice);
                }
            }

            SelectFileCommand = new RelayCommand(SelectFile);

            Instance = this;
        }

        private string _csvFileName;
        public string CsvFileName
        {
            get => _csvFileName;
            set
            {
                if (_csvFileName == value)
                {
                    return;
                }

                _csvFileName = value;
                OnPropertyChanged();
            }
        }

        private string _log;

        public string Log
        {
            get => _log;
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SoundDevice> SoundDevices { get; } = new ObservableCollection<SoundDevice>();

        private SoundDevice _selectedSoundDevice;
        public SoundDevice SelectedSoundDevice
        {
            get => _selectedSoundDevice;
            set
            {
                if (_selectedSoundDevice == value)
                {
                    return;
                }

                _selectedSoundDevice = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand SelectFileCommand { get; set; }

        private void SelectFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "CSVファイルの保存先を選択してください",
                Filter = "CSVファイル|*.csv",
            };

            if (!(dialog.ShowDialog() ?? false))
            {
                return;
            }

            CsvFileName = dialog.FileName;
            Save();
        }

        public void Save()
        {
            Settings.Default.CsvFileName = CsvFileName;
            Settings.Default.DeviceId = SelectedSoundDevice?.Id ?? string.Empty;
            Settings.Default.Save();
        }

        public void WriteLog(string log)
        {
            var message = $"[{DateTime.Now:HH:mm:ss.fff}] {log}";
            Console.WriteLine(message);
            var lines = (Log ?? "").Split('\n').ToList();
            lines.Add(message);
            while (1_000 < lines.Count)
            {
                lines.RemoveAt(0);
            }

            SettingPanel.Instance.Dispatcher.Invoke(() =>
            {
                Log = string.Join("\n", lines);
            });
        }

        [Conditional("DEBUG")]
        public void DebugLog(string log)
        {
            var message = $"[{DateTime.Now:HH:mm:ss.fff}] {log}";
            Console.WriteLine(message);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}