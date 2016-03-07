rem This is a sample. Please edit ffmpeg.exe, .h264 and .wav file path.

del movie.mp4
ffmpeg.exe -i movie.h264 -i movie.wav -vcodec copy -acodec mp3 movie.mp4