using System.Windows.Controls;
using YukarinetteSePlayer.Properties;
using YukarinetteSePlayer.ViewModel;

namespace YukarinetteSePlayer
{
    /// <summary>
    /// SettingPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingPanel
    {
        private static SettingPanel _instance;
        public static SettingPanel Instance => _instance ?? (_instance = new SettingPanel());

        public SettingPanel()
        {
            InitializeComponent();
            Initialize();
        }

        private SettingPanelViewModel ViewModel => DataContext as SettingPanelViewModel;

        private void Initialize()
        {
            ViewModel.CsvFileName = Settings.Default.CsvFileName;

            // var guid = Settings.Default.SoundDeviceGuid;
            var deviceId = Settings.Default.DeviceId;
            foreach (var device in ViewModel.SoundDevices)
            {

                if (deviceId == device.Id)
                // if (device.Guid != guid)
                // if (device.ManufacturerGuid != guid)
                {
                    continue;
                }

                ViewModel.SelectedSoundDevice = device;
                break;
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.Save();
        }
    }
}
