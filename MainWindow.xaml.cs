
// ChungusVideoDownloader downloads YouTube videos.
// Copyright (C) 2024 Alex Archambault
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

using System.IO;
using System.Windows;

using VideoLibrary;

using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows.Threading;

namespace ChungusVideoDownloader {
    public partial class MainWindow : Window {
        string finalFilePath = "";

        VideoProcessor videoProcessor = new VideoProcessor();

        public MainWindow() {
            InitializeComponent();

            videoProcessor.OnVideoFound += E_OnVideoFound;
            videoProcessor.OnVideoProcessed += E_OnVideoProcessed;

            videoProcessor.SetExtension(Globals.EXTENSION_VIDEO);
        }

        protected override void OnClosed(EventArgs _args) {
            base.OnClosed(_args);
            Application.Current.Shutdown();
        }

        void SetInputState(bool _state) {
            inputButtonProcess.IsEnabled = _state;
            inputDownloadModeAV.IsEnabled = _state;
            inputDownloadModeA.IsEnabled = _state;
            inputDownloadModeV.IsEnabled = _state;
            inputTextBoxURI.IsEnabled = _state;
            inputTextBoxStart.IsEnabled = _state;
            inputTextBoxEnd.IsEnabled = _state;
            inputAudioQuality.IsEnabled = _state;
            inputVideoQuality.IsEnabled = _state;
        }

        private void InputButtonProcess_Click(object sender, RoutedEventArgs e) {
            if (!File.Exists("ffmpeg.exe")) {
                MessageBox.Show("Missing \"ffmpeg.exe\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!OpenDownloadDialog()) {
                return;
            }

            int audioIndex = inputAudioQuality.SelectedIndex;
            if (audioIndex == (int)Globals.AudioQuality.HIGH) {
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_HIGH);
            }
            else if (audioIndex == (int)Globals.AudioQuality.MEDIUM) {
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_MEDIUM);
            }
            else if (audioIndex == (int)Globals.AudioQuality.LOW) {
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_LOW);
            }

            int videoIndex = inputVideoQuality.SelectedIndex;
            if (videoIndex == (int)Globals.VideoQuality.BEST) {
                videoProcessor.SetVideoQuality("best");
            }
            else if (videoIndex == (int)Globals.VideoQuality.UHD) {
                videoProcessor.SetVideoQuality("3840");
            }
            else if (videoIndex == (int)Globals.VideoQuality.SQHD) {
                videoProcessor.SetVideoQuality("2560");
            }
            else if (videoIndex == (int)Globals.VideoQuality.FHD) {
                videoProcessor.SetVideoQuality("1920");
            }
            else if (videoIndex == (int)Globals.VideoQuality.HD) {
                videoProcessor.SetVideoQuality("1280");
            }
            else if (videoIndex == (int)Globals.VideoQuality.SD) {
                videoProcessor.SetVideoQuality("854");
            }
            else if (videoIndex == (int)Globals.VideoQuality.SSD) {
                videoProcessor.SetVideoQuality("640");
            }
            else if (videoIndex == (int)Globals.VideoQuality.SSSD) {
                videoProcessor.SetVideoQuality("426");
            }

            videoProcessor.SetDownloadPath(finalFilePath);
            videoProcessor.SetStartTime(inputTextBoxStart.Text);
            videoProcessor.SetEndTime(inputTextBoxEnd.Text);
            if ((bool)inputDownloadModeA.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_A); }
            else if ((bool)inputDownloadModeV.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_V); }
            else if((bool)inputDownloadModeAV.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_AV); }

            SetProcessStatus(Globals.PROCESS_STATE_DOWNLOADING);

            SetInputState(false);
            videoProcessor.Download();
        }

        private void InputButtonUsage_Click(object sender, RoutedEventArgs e) {
            const string USAGE_TITLE = "Usage";
            const string USAGE_MESSAGE =
                "1. Paste a YouTube URL in the 'Source' text field.\n" +
                "2. If desired, change the start/end time in the format\n" +
                "     HH:MM:SS. (ex: 00:12:04)\n" +
                "3. Choose the download settings you need, or leave them at their\n"+
                "     defaults.\n"+
                "4. Click the 'Process' button, and choose the download location.\n" +
                "     Note: The 'Process' button will be disabled if an invalid\n" +
                "     YouTube link is provided.\n\n" +

                "It may take a while to process large videos, please be patient.\n" +
                "An audible chime will sound when it's finished.";

            MessageBox.Show(USAGE_MESSAGE, USAGE_TITLE, MessageBoxButton.OK,MessageBoxImage.Question);
        }

        private void InputButtonAbout_Click(object sender, RoutedEventArgs e) {
            const string ABOUT_TITLE = "About";
            const string ABOUT_MESSAGE =
                "This program was created as a custom replacement for\n" +
                "other similar solutions, and to test myself in a new\n" +
                "development environment.\n\n"+

                "Copyright (C) 2024 Alex Archambault\n" +
                "This program uses the GPLv3 license.\n" +
                "For more information about GPLv3, please visit:\n" +
                "https://www.gnu.org/licenses/gpl-3.0.en.html\n\n" +

                "Source code available at:\n" +
                "https://github.com/JohnMcChungus";

            MessageBox.Show(ABOUT_MESSAGE, ABOUT_TITLE, MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void InputTextBoxURI_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            videoProcessor.SetURL(inputTextBoxURI.Text);
            videoProcessor.FetchVideo();

            if (videoProcessor.IsVideoActive()) {
                inputTextBoxStart.Text = "00:00:00";
                inputTextBoxEnd.Text = videoProcessor.GetVideoDuration();
                outputInfoDuration.Text = videoProcessor.GetVideoDuration();
            }
            else {
                outputInfoName.Text = "-";
                outputInfoDuration.Text = "00:00:00";
            }
        }

        private void InputDownloadModeAV_Click(object sender, RoutedEventArgs e) {
            outputSettingAudioQuality.IsEnabled = true;
            outputSettingVideoResolution.IsEnabled = true;
            videoProcessor.SetExtension(GetExtensionFromRadioSelection());
        }

        private void InputDownloadModeA_Click(object sender, RoutedEventArgs e) {
            outputSettingAudioQuality.IsEnabled = true;
            outputSettingVideoResolution.IsEnabled = false;
            videoProcessor.SetExtension(GetExtensionFromRadioSelection());
        }

        private void InputDownloadModeV_Click(object sender, RoutedEventArgs e) {
            outputSettingAudioQuality.IsEnabled = false;
            outputSettingVideoResolution.IsEnabled = true;
            videoProcessor.SetExtension(GetExtensionFromRadioSelection());
        }

        void E_OnVideoFound(object _sender, YouTubeVideo _video) {
            inputButtonProcess.IsEnabled = _video != null;

            if (_video == null) {
                return;
            }

            string title = _video.Title;
            if (title.Length > 19) {
                outputInfoName.Text = $"{_video.Title.Substring(0, 16)}...";
            }
            else {
                outputInfoName.Text = _video.Title;
            }
        }

        void E_OnVideoProcessed(object _sender, EventArgs _args) {
            SetProcessStatus(Globals.PROCESS_STATE_IDLE);
            SetInputState(true);
            System.Media.SystemSounds.Exclamation.Play();
        }

        string GetExtensionFromRadioSelection() {
            if (inputDownloadModeAV.IsChecked == true || inputDownloadModeV.IsChecked == true) {
                return Globals.EXTENSION_VIDEO;
            }
            else if (inputDownloadModeA.IsChecked == true) {
                return Globals.EXTENSION_AUDIO;
            }

            // .mp4 is used as a generic catch-all, just in case.
            return ".mp4";
        }

        bool OpenDownloadDialog() {
            string ext = GetExtensionFromRadioSelection();
            string desc = "Files";
            string title = videoProcessor.GetVideoTitle();

            foreach (char c in Path.GetInvalidFileNameChars()) {
                title = title.Replace(c, '_');
            }

            if (ext == Globals.EXTENSION_VIDEO) {
                desc = "Video Files";
            }
            else if (ext == Globals.EXTENSION_AUDIO) {
                desc = "Audio Files";
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Set Download Path";
            saveFileDialog.Filter = $"{desc} | *{ext}";
            saveFileDialog.InitialDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
            saveFileDialog.FileName = title + ext;

            bool? success = saveFileDialog.ShowDialog();
            if (success == true) {
                finalFilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        void SetProcessStatus(string _status) {
            Thread thread = new Thread(() => SetProcessStatusTask(_status));
            thread.Start();
        }

        void SetProcessStatusTask(string _status) {
            Dispatcher.Invoke(() => {
                outputProcessState.Text = _status;
            });
        }
    }
}
