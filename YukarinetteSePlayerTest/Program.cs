using System;
using System.Linq;
using YukarinetteSePlayer;
using YukarinetteSePlayer.ViewModel;

namespace YukarinetteSePlayerTest
{
    internal class Program
    {
        [STAThread]
        static void Main()
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

            // viewModel.CsvFileName = @"CSV FILE PATH";
            viewModel.Save();

            var keyword = "KEYWORD";
            player.Filtering(keyword, null);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
