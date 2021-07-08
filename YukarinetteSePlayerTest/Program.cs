using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukarinetteSePlayer;
using YukarinetteSePlayer.ViewModel;

namespace YukarinetteSePlayerTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var player = new SePlayer();
            var panel = SettingPanel.Instance;
            var viewModel = SettingPanelViewModel.Instance;
            foreach (var i in Enumerable.Range(0, viewModel.SoundDevices.Count))
            {
                var device = viewModel.SoundDevices[i];
                var index = i + 1;
                Console.WriteLine($"{index}: {device.Description}\t{device.Id}");
                // Console.WriteLine($"{index}: {device.Description}\t{device.Guid}");
            }

            var str = Console.ReadLine();
            if (!int.TryParse(str, out var number))
            {
                return;
            }

            foreach (var i in Enumerable.Range(0, viewModel.SoundDevices.Count))
            {
                if ((number - 1) == i)
                {
                    viewModel.SelectedSoundDevice = viewModel.SoundDevices[i];
                }
            }

            viewModel.CsvFileName = @"D:\Application\ゆかりネット\音源連携.csv";
            viewModel.Save();
            player.Filtering("出陣2", null);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
