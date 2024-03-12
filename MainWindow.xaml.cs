
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

using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ChungusVideoDownloader {
    public partial class MainWindow : Window {
        VideoProcessor videoProcessor = new VideoProcessor();
        string finalFilePath = "";

        public MainWindow() {
            InitializeComponent();
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

        async private void InputButtonProcess_Click(object _sender, RoutedEventArgs _args) {
            if (!File.Exists("ffmpeg.exe")) {
                MessageBox.Show("Missing \"ffmpeg.exe\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!File.Exists("ffprobe.exe")) {
                MessageBox.Show("Missing \"ffprobe.exe\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!OpenDownloadDialog()) {
                return;
            }

            switch (inputAudioQuality.SelectedIndex) {
            case (int)Globals.AudioQuality.HIGH:
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_HIGH);
                break;
            case (int)Globals.AudioQuality.MEDIUM:
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_MEDIUM);
                break;
            case (int)Globals.AudioQuality.LOW:
                videoProcessor.SetAudioQuality(Globals.AUDIO_SAMPLE_RATE_LOW);
                break;
            }

            switch (inputVideoQuality.SelectedIndex) {
            case (int)Globals.VideoQuality.BEST:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_BEST);
                break;
            case (int)Globals.VideoQuality.UHD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_UHD);
                break;
            case (int)Globals.VideoQuality.SQHD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_SQHD);
                break;
            case (int)Globals.VideoQuality.FHD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_FHD);
                break;
            case (int)Globals.VideoQuality.HD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_HD);
                break;
            case (int)Globals.VideoQuality.SD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_SD);
                break;
            case (int)Globals.VideoQuality.SSD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_SSD);
                break;
            case (int)Globals.VideoQuality.SSSD:
                videoProcessor.SetVideoQuality(Globals.VIDEO_RESOLUTION_WIDTH_SSSD);
                break;
            }

            videoProcessor.SetDownloadPath(finalFilePath);
            videoProcessor.SetStartTime(inputTextBoxStart.Text);
            videoProcessor.SetEndTime(inputTextBoxEnd.Text);
            videoProcessor.SetExtension(GetExtensionFromRadioSelection());

            if ((bool)inputDownloadModeA.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_A); }
            else if ((bool)inputDownloadModeV.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_V); }
            else if ((bool)inputDownloadModeAV.IsChecked) { videoProcessor.SetDownloadType(Globals.TYPE_AV); }

            SetInputState(false);
            outputProcessState.Text = Globals.PROCESS_STATE_PROCESSING;

            await videoProcessor.Download();
            videoProcessor.Process();

            outputProcessState.Text = Globals.PROCESS_STATE_IDLE;
            SetInputState(true);
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void InputButtonUsage_Click(object _sender, RoutedEventArgs _args) {
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

        private void InputButtonAbout_Click(object _sender, RoutedEventArgs _args) {
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

        private void InputTextBoxURI_TextChanged(object _sender, System.Windows.Controls.TextChangedEventArgs _args) {
            videoProcessor.SetURL(inputTextBoxURI.Text);
            videoProcessor.FetchVideo();

            inputButtonProcess.IsEnabled = videoProcessor.IsVideoActive() != false;

            if (videoProcessor.IsVideoActive()) {
                inputTextBoxStart.Text = Globals.DEFAULT_TIMESTAMP;
                inputTextBoxEnd.Text = videoProcessor.GetVideoDuration();
                outputInfoDuration.Text = videoProcessor.GetVideoDuration();

                const int MAX_TITLE_LENGTH = 19;
                string title = videoProcessor.GetVideoTitle();
                if (title.Length > MAX_TITLE_LENGTH) {
                    outputInfoName.Text = $"{title.Substring(0, MAX_TITLE_LENGTH - 3)}...";
                }
                else {
                    outputInfoName.Text = title;
                }
            }
            else {
                inputTextBoxStart.Text = Globals.DEFAULT_TIMESTAMP;
                inputTextBoxEnd.Text = Globals.DEFAULT_TIMESTAMP;
                outputInfoDuration.Text = Globals.DEFAULT_TIMESTAMP;
                outputInfoName.Text = Globals.DEFAULT_VIDEO_NAME;
            }
        }

        private void InputDownloadModeAV_Click(object _sender, RoutedEventArgs _args) {
            outputSettingAudioQuality.IsEnabled = true;
            outputSettingVideoResolution.IsEnabled = true;
        }

        private void InputDownloadModeA_Click(object _sender, RoutedEventArgs _args) {
            outputSettingAudioQuality.IsEnabled = true;
            outputSettingVideoResolution.IsEnabled = false;
        }

        private void InputDownloadModeV_Click(object _sender, RoutedEventArgs _args) {
            outputSettingAudioQuality.IsEnabled = false;
            outputSettingVideoResolution.IsEnabled = true;
        }

        string GetExtensionFromRadioSelection() {
            if (inputDownloadModeA.IsChecked != true) {
                return Globals.EXTENSION_VIDEO;
            }
            else {
                return Globals.EXTENSION_AUDIO;
            }
        }

        bool OpenDownloadDialog() {
            string title = videoProcessor.GetVideoTitle();
            foreach (char c in Path.GetInvalidFileNameChars()) {
                title = title.Replace(c, '_');
            }

            string desc = "";
            string extension = GetExtensionFromRadioSelection();
            if (extension == Globals.EXTENSION_VIDEO) {
                desc = "Video Files";
            }
            else if (extension == Globals.EXTENSION_AUDIO) {
                desc = "Audio Files";
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Set Download Path";
            saveFileDialog.Filter = $"{desc} | *{extension}";
            saveFileDialog.InitialDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
            saveFileDialog.FileName = title + extension;

            bool? success = saveFileDialog.ShowDialog();
            if (success == true) {
                finalFilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }
    }
}
