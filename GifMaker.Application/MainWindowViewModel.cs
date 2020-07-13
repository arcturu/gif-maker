using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifMaker.Application
{
    class MainWindowViewModel : IDisposable
    {
        public ReactivePropertySlim<string> TargetDirectory { get; }
        public ReactivePropertySlim<string> OutputPath { get; }
        public ReactivePropertySlim<float> FrameRate { get; }
        public ReactivePropertySlim<string> ConvertStatus { get; }
        public ICommand ConvertCommand { get; }

        public MainWindowViewModel(Config config)
        {
            TargetDirectory = new ReactivePropertySlim<string>(config.TargetDirectory);
            disposables.Add(TargetDirectory);
            OutputPath = new ReactivePropertySlim<string>(config.OutputPath);
            disposables.Add(OutputPath);
            FrameRate = new ReactivePropertySlim<float>(config.FrameRate);
            disposables.Add(FrameRate);
            ConvertStatus = new ReactivePropertySlim<string>("Ready");
            disposables.Add(ConvertStatus);

            ConvertCommand = new Command(async () =>
            {
                ConvertStatus.Value = "Converting...";
                await ConvertToAnimatedGifFileAsync(TargetDirectory.Value, OutputPath.Value, FrameRate.Value);
                ConvertStatus.Value = "Ready";
            });
        }

        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        public Config GetCurrentConfig()
        {
            return new Config
            {
                TargetDirectory = TargetDirectory.Value,
                OutputPath = OutputPath.Value,
                FrameRate = FrameRate.Value,
            };
        }

        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private async Task ConvertToAnimatedGifFileAsync(string targetDirectory, string outputPath, float frameRate)
        {
            // 画像ファイルを名前昇順で全取得
            var filePaths = Directory.GetFiles(targetDirectory)
                .Where(name => name.EndsWith(".jpg") || name.EndsWith(".png"))
                .OrderBy(name => name);

            await Task.Run(() =>
            {
                using (var collection = new ImageMagick.MagickImageCollection())
                {
                    foreach (var filePath in filePaths)
                    {
                        collection.Add(filePath);
                    }
                    foreach (var item in collection)
                    {
                        item.AnimationDelay = (int)(1000.0f / frameRate / 10.0f); // 10 ms 単位
                        // item.Resize(100, (int)(100.0f / item.Width * item.Height));
                    }
                    collection.Optimize();
                    collection.Write(outputPath);
                }
            }).ConfigureAwait(false);
        }
    }
}
