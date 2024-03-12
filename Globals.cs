
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

namespace ChungusVideoDownloader {
    internal class Globals {
        public const string INTERMEDIATE_FILE = "intermediate";
        public const string EXTENSION_VIDEO = ".mp4";
        public const string EXTENSION_AUDIO = ".mp3";

        public const string TYPE_A = "A";
        public const string TYPE_V = "A";
        public const string TYPE_AV = "AV";

        public const string PROCESS_STATE_IDLE = "IDLE";
        public const string PROCESS_STATE_PROCESSING = "PROCESSING";

        public const string AUDIO_SAMPLE_RATE_HIGH = "48000";
        public const string AUDIO_SAMPLE_RATE_MEDIUM = "32000";
        public const string AUDIO_SAMPLE_RATE_LOW = "22050";

        public const string VIDEO_RESOLUTION_WIDTH_BEST = "best";
        public const string VIDEO_RESOLUTION_WIDTH_UHD = "3840";
        public const string VIDEO_RESOLUTION_WIDTH_SQHD = "2560";
        public const string VIDEO_RESOLUTION_WIDTH_FHD = "1920";
        public const string VIDEO_RESOLUTION_WIDTH_HD = "1280";
        public const string VIDEO_RESOLUTION_WIDTH_SD = "854";
        public const string VIDEO_RESOLUTION_WIDTH_SSD = "640";
        public const string VIDEO_RESOLUTION_WIDTH_SSSD = "426";

        public const string DEFAULT_TIMESTAMP = "00:00:00";
        public const string DEFAULT_VIDEO_NAME = "-";

        public enum AudioQuality {
            HIGH,
            MEDIUM,
            LOW
        }

        public enum VideoQuality {
            BEST,
            UHD,
            SQHD,
            FHD,
            HD,
            SD,
            SSD,
            SSSD
        }
    }
}
