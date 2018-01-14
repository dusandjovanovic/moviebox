using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MovieBox;
using Windows.System;
using Windows.Storage;
using Windows.UI.Popups;

namespace MovieBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Playing : Page
    {
        public Playing()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(App.mediaPath))
                return;

            string path = App.mediaPath;
            if (path.Contains('\\'))
                path = path.Substring(0, path.LastIndexOf('\\'));

            try
            {
                StorageFolder _folder = await StorageFolder.GetFolderFromPathAsync(path);
                var name = Path.GetFileName(App.mediaPath);
                StorageFile _file = await _folder.GetFileAsync(name);
                if (_file != null)
                {
                    var randomAccessStream = await _file.OpenReadAsync();
                    Stream stream = randomAccessStream.AsStreamForRead();
                    mediaPlayer.SetSource(randomAccessStream, randomAccessStream.ContentType);
                    mediaPlayer.Play();
                }
            }
            catch (Exception)
            {
                var messageDialog = new MessageDialog("The movie can't be played, the selected path might be wrong." + "\n" + "Check containment of " + App.mediaPath);

                messageDialog.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 0;

                await messageDialog.ShowAsync();
                return;
            }

        }

        private void CommandInvokedHandler(IUICommand command)
        {
            return;
        }
    }
}
