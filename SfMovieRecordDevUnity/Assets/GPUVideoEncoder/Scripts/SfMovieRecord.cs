using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace GPUVideoEncoder
{
    public static class SfMovieRecord
    {
        private static bool IsValidPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return true;
                default:
                    break;
            }
            throw new GPUVideoEncoderException("This plugin spport Windows x86_64 DX11 environment only.");
        }

         [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugDelegate(string message);

         [DllImport("SfMovieRecordPlugin")]
        private static extern void SfMR_SetDebugFunction(IntPtr fp);
        public static void SetDebugFunction(IntPtr fp)
        {
            if (IsValidPlatform())
            {
                SfMR_SetDebugFunction(fp);
            }
        }

        [DllImport("SfMovieRecordPlugin")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("SfMovieRecordPlugin", CharSet = CharSet.Unicode)]
        private static extern void SfMR_GetLastErrorMessage(StringBuilder buffer, uint bufferSize);
        public static string GetLastErrorMessage()
        {
            if (IsValidPlatform())
            {
                StringBuilder buffer = new StringBuilder(512);
                SfMR_GetLastErrorMessage(buffer, (uint)buffer.Capacity);
                return buffer.ToString();
            }
            return string.Empty;
        }

        [DllImport("SfMovieRecordPlugin", CharSet = CharSet.Unicode)]
        private static extern ulong SfMR_GetAudioDeviceCount();
        public static ulong GetAudioDeviceCount()
        {
            if (IsValidPlatform())
            {
                return SfMR_GetAudioDeviceCount();
            }
            return 0;
        }

        [DllImport("SfMovieRecordPlugin", CharSet = CharSet.Unicode)]
        private static extern void SfMR_GetAudioDeviceName(ulong index, StringBuilder buffer, uint bufferSize);
        public static string GetAudioDeviceName(ulong index)
        {
            if (IsValidPlatform())
            {
                StringBuilder buffer = new StringBuilder(64);
                SfMR_GetAudioDeviceName(index, buffer, (uint)buffer.Capacity);
                return buffer.ToString();
            }
            return string.Empty;
        }

        [DllImport("SfMovieRecordPlugin", CharSet = CharSet.Unicode)]
        private static extern bool SfMR_StartMovieRecord(string outputPath, string fileTitle, IntPtr texture, int fps, bool withAudio, ulong audioDeviceIndex);
        public static void StartMovieRecord(string outputPath, string fileTitle, IntPtr texture, int fps, bool withAudio, ulong audioDeviceIndex)
        {
            if (IsValidPlatform())
            {
                if (!SfMR_StartMovieRecord(outputPath, fileTitle, texture, fps, withAudio, audioDeviceIndex))
                {
                    throw new GPUVideoEncoderException(GetLastErrorMessage());
                }
            }
        }

        [DllImport("SfMovieRecordPlugin", CharSet = CharSet.Unicode)]
        private static extern bool SfMR_EndMovieRecord(string outputPath, string fileTitle);
        public static void EndMovieRecord(string outputPath, string fileTitle)
        {
            if (IsValidPlatform())
            {
                if (!SfMR_EndMovieRecord(outputPath, fileTitle))
                {
                    throw new GPUVideoEncoderException(GetLastErrorMessage());
                }
            }
        }
    }

    public class GPUVideoEncoderException : Exception
    {
        public GPUVideoEncoderException(string message) : base(message)
        {
        }
    }
}
