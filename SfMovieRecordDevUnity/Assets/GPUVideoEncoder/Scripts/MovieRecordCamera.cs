using SpicyPixel.Threading;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace GPUVideoEncoder
{
    public class MovieRecordCamera : ConcurrentBehaviour
    {
        public RenderTexture texture;
        public string outputFilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public string outputFileTitle = "movie";
        public bool withAudio = true;
        [HideInInspector]
        public int audioDeviceIndex;

        private int fps = 60;
        private bool isRecording = false;
        private Thread wokerThread = null;

        static void SfMovieRecordPluginDebugCallBack(string message)
        {
            Debug.Log("GPUVideoEncoder : " + message);
        }

        void Start()
        {
            var debugDelegate = new SfMovieRecord.DebugDelegate(SfMovieRecordPluginDebugCallBack);
            var functionPointer = Marshal.GetFunctionPointerForDelegate(debugDelegate);
            SfMovieRecord.SetDebugFunction(functionPointer);
            this.wokerThread = null;
        }

        public void OnApplicationQuit()
        {
            this.EndMovieRecord();
            SfMovieRecord.SetDebugFunction(IntPtr.Zero);
        }

        public void StartMovieRecord()
        {
            if (!this.isRecording && (this.texture.GetNativeTexturePtr() != System.IntPtr.Zero))
            {
                SfMovieRecord.StartMovieRecord(this.outputFilePath, this.outputFileTitle, this.texture.GetNativeTexturePtr(), this.fps, this.withAudio, (ulong)this.audioDeviceIndex);
                this.isRecording = true;
                this.wokerThread = new Thread(new ThreadStart(this.FrameOutProcess));
                this.wokerThread.Start();
            }
        }

        public void EndMovieRecord()
        {
            this.isRecording = false;
            this.StartCoroutine(this.FrameOutFinalize());
        }

        private void FrameOutProcess()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int interval = (1000 / this.fps);
            while (this.isRecording)
            {
                if (sw.ElapsedMilliseconds > interval)
                {
                    taskFactory.StartNew(() => this.FrameOutInMainThread());
                    sw.Reset();
                    sw.Start();
                }
            }
            sw.Stop();
        }

        private void FrameOutInMainThread()
        {
            if (this.isRecording)
            {
                GL.IssuePluginEvent(SfMovieRecord.GetRenderEventFunc(), 1);
            }
        }

        private IEnumerator FrameOutFinalize()
        {
            if (this.wokerThread != null)
            {
                this.wokerThread.Abort();
                this.wokerThread = null;
            }
            yield return new WaitForEndOfFrame();
            SfMovieRecord.EndMovieRecord();
        }
    }
}

