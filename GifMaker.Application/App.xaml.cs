using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Core;

namespace GifMaker.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private const string ConfigPath = "config.yml"; // 相対パス
        private MainWindowViewModel mainWindowViewModel;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            Config config = LoadConfig();
            mainWindowViewModel = new MainWindowViewModel(config);
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var newConfig = mainWindowViewModel.GetCurrentConfig();
            StoreConfig(newConfig);
        }

        /// <summary>
        /// 設定ファイルをロードします。
        /// </summary>
        /// <returns>設定データ</returns>
        private static Config LoadConfig()
        {
            try
            {
                using (var fs = File.Open(ConfigPath, FileMode.Open))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        var deserializer = new YamlDotNet.Serialization.Deserializer();
                        return deserializer.Deserialize<Config>(reader) ?? new Config();
                    }
                }
            }
            catch (Exception exception)
            {
                return new Config();
            }
        }

        /// <summary>
        /// 設定ファイルを保存します。
        /// </summary>
        /// <param name="config">設定データ</param>
        private static void StoreConfig(Config config)
        {
            using (var fs = File.Open(ConfigPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(fs))
                {
                    var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
                    serializer.Serialize(writer, config);
                }
            }
        }
    }
}
