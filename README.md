# Chungus Video Downloader (CVD)

I built this program as a replacement for the many (in my opinion) inadequate YouTube video downloaders that exist. While mine may have obvious issues, they are issues that I'm okay with, as the user-experience with CVD is specifically tailored to my needs.

No binaries are provided as of now, so to use CVD you must build it from source. It's not hard, just git clone and build the project in VS.

- CVD always downloads YouTube videos at maximum quality, and then downscales/processes the result with FFmpeg, which results in crisp lower resolutions. The downside to this is that it may take a while for the videos to download and be processed.
- CVD can also trim the downloaded video to whatever length you need, and remove the audio or video tracks if so desired.

It occurs to me that CVD is an acronym for a medical condition, but I'm okay with that.

Copyright (C) 2024 Alex Archambault

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
