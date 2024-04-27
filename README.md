# Chungus Video Downloader (CVD)

I built this program as a replacement for the many (in my opinion) inadequate YouTube video downloaders that exist. While mine may have obvious issues, they are issues that I'm okay with, as the user-experience with CVD is specifically tailored to my needs.

No binaries are provided as of now, so to use CVD you must build it from source. It's not hard, just git clone and build the project in VS.
CVD requires 'ffmpeg.exe' and 'ffprobe.exe'. It seems that these must be in the binary's working directory, as the program doesn't recognise them in PATH due to my inability to care about fixing this.

- CVD always downloads YouTube videos at maximum quality, and then downscales/processes the result with FFmpeg, which results in crisp lower resolutions. The downside to this is that it may take a while for the videos to download and be processed.
- CVD can also trim the downloaded video to whatever length you need, and remove the audio or video tracks if so desired.

It occurs to me that CVD is an acronym for a medical condition, but I'm okay with that.
