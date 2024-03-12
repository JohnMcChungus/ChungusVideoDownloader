
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
using System.Diagnostics;
using System.Windows;
using System.IO;

using VideoLibrary;
using YoutubeExplode;
using YoutubeExplode.Converter;
using System.Threading.Tasks;

namespace ChungusVideoDownloader {
    internal class VideoProcessor : Window {
        YouTube youtube = YouTube.Default;
        YouTubeVideo activeVideo = null;

        string videoUrl = "";

        string downloadPath = "";
        string startTime = "";
        string endTime = "";

        string extension = "";
        string type = "";

        string videoQuality = "";
        string audioQuality = "";

        public bool IsVideoActive() {
            return activeVideo != null;
        }

        public void SetDownloadType(string _type) { type = _type; }
        public void SetAudioQuality(string _audioQuality) { audioQuality = _audioQuality; }
        public void SetVideoQuality(string _videoQuality) { videoQuality = _videoQuality; }
        public void SetDownloadPath(string _path) { downloadPath = _path; }
        public void SetExtension(string _extension) { extension = _extension; }
        public void SetStartTime(string _startTime) { startTime = _startTime; }
        public void SetEndTime(string _endTime) { endTime = _endTime; }
        public void SetURL(string _url) { videoUrl = _url; }

        public void FetchVideo() {
            activeVideo = null;

            try {
                var video = youtube.GetVideo(videoUrl);
                activeVideo = video;
            }
            catch (Exception _e) {
                Trace.WriteLine(_e.Message);
            }
        }

        public string GetVideoTitle() {
            if (activeVideo == null) {
                return Globals.DEFAULT_VIDEO_NAME;
            }
            return activeVideo.Title;
        }

        public string GetVideoDuration() {
            if (activeVideo == null) {
                return Globals.DEFAULT_TIMESTAMP;
            }

            int durationSeconds = activeVideo.Info.LengthSeconds ?? default;
            int durationMinutes = durationSeconds / 60;
            durationSeconds -= durationMinutes * 60;
            int durationHours = durationMinutes / 60;
            durationMinutes -= durationHours * 60;

            return $"{FormatIntDurationToString(durationHours)}:{FormatIntDurationToString(durationMinutes)}:{FormatIntDurationToString(durationSeconds)}";
        }

        string FormatIntDurationToString(int _duration) {
            return _duration < 10 ? $"0{_duration}" : $"{_duration}";
        }

        string GetDownloadedVideoResolution() {
            var ffprobe = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "ffprobe.exe",
                    Arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{Globals.INTERMEDIATE_FILE + extension}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            ffprobe.Start();

            string resolution = "";
            while (!ffprobe.StandardOutput.EndOfStream) {
                resolution = ffprobe.StandardOutput.ReadLine();
            }

            ffprobe.Close();
            return resolution;
        }

        async public Task Download() {
            if (activeVideo == null) {
                return;
            }

            try {
                PurgeIntermediateFile();

                YoutubeClient client = new YoutubeClient();
                await client.Videos.DownloadAsync(videoUrl, Globals.INTERMEDIATE_FILE + extension);
            }
            catch (Exception _e) {
                MessageBox.Show(_e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Process() {
            if (!File.Exists(Globals.INTERMEDIATE_FILE + extension)) {
                MessageBox.Show("Intermediate file not found. Aborting.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try {
                string startTimeIn = startTime != "" ? startTime : Globals.DEFAULT_TIMESTAMP;
                string[] startTimeStrings = startTimeIn.Split(':');
                int[] startTimeInts = new int[startTimeStrings.Length];

                for (int i = 0; i < startTimeStrings.Length; i++) {
                    startTimeInts[i] = int.Parse(startTimeStrings[i]);

                    if (startTimeInts[i] < 0 || startTimeInts[i] > 59) {
                        startTimeInts[i] = 0;
                    }
                }

                string startTimeFormatted = $"{startTimeInts[0]}:{startTimeInts[1]}:{startTimeInts[2]}";

                string endTimeIn = endTime == "" ? GetVideoDuration() : endTime;
                string[] endTimeStrings = endTimeIn.Split(':');
                int[] endTimeInts = new int[endTimeStrings.Length];

                for (int i = 0; i < endTimeStrings.Length; i++) {
                    endTimeInts[i] = int.Parse(endTimeStrings[i]);

                    if (endTimeInts[i] < 0 || endTimeInts[i] > 59) {
                        endTimeInts[i] = 0;
                    }
                }

                int[] durations = new int[endTimeInts.Length];
                for (int i = 0; i < durations.Length; i++) {
                    durations[i] = (endTimeInts[i] - startTimeInts[i] >= 0) ? endTimeInts[i] - startTimeInts[i] : 60 + endTimeInts[i] - startTimeInts[i];
                }
                string duration = $"{durations[0]}:{durations[1]}:{durations[2]}";


                string audioQualityParam = $"-ar {audioQuality}";
                string videoQualityParam = "";
                string copyParam = "";
                string videoOnlyParam = "";

                if (extension != Globals.EXTENSION_AUDIO) {
                    if (videoQuality != "best") {
                        string width = GetDownloadedVideoResolution().Split('x')[0];
                        int currentWidthInt = int.Parse(width);
                        int targetWidthInt = int.Parse(videoQuality);
                        string setWidth = currentWidthInt > targetWidthInt ? videoQuality : width;

                        videoQualityParam = $"-vf scale={setWidth}:-2,setsar=1:1 -c:v libx264";
                    }
                    else {
                        string width = GetDownloadedVideoResolution().Split('x')[0];
                        videoQualityParam = $"-vf scale={width}:-2,setsar=1:1 -c:v libx264";
                    }
                }
                else {
                    copyParam = "-c copy";
                }

                if (type == Globals.TYPE_V) {
                    videoOnlyParam = "-an";
                }

                string args = "";
                if (type != Globals.TYPE_A) {
                    args = $"-y -ss {startTimeFormatted} -i \"{Globals.INTERMEDIATE_FILE + extension}\" -t {duration} {audioQualityParam} {videoQualityParam} {videoOnlyParam} {copyParam} \"{downloadPath}\"";
                }
                else {
                    args = $"-y -ss {startTimeFormatted} -i \"{Globals.INTERMEDIATE_FILE + extension}\" -t {duration} {audioQualityParam} \"{downloadPath}\"";
                }

                Trace.WriteLine("Starting ffmpeg process.");
                var ffmpeg = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "ffmpeg.exe",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                ffmpeg.Start();

                ffmpeg.WaitForExit(); // This causes the UI to become blocked. I might want to revisit this, but it works for now.
                PurgeIntermediateFile();
            }
            catch (Exception _e) {
                MessageBox.Show(_e.Message, "Processing Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void PurgeIntermediateFile() {
            if (File.Exists(Globals.INTERMEDIATE_FILE + extension)) {
                File.Delete(Globals.INTERMEDIATE_FILE + extension);
            }
        }
    }
}
