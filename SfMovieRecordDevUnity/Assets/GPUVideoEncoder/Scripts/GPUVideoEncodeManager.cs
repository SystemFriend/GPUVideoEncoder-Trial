using SpicyPixel.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace GPUVideoEncoder
{
    public class GPUVideoEncodeManager : ConcurrentBehaviour
    {
        private const int MaxEncoders = 2;
        private IList<MovieRecordCamera> MovieRecordCameras { get; set; }
        public int FPS { get; private set; }
        private bool needAbort = false;

        public bool IsEncoderAddable
        {
            get
            {
                return (this.MovieRecordCameras.Count < MaxEncoders);
            }
        }

        void Start()
        {
            this.FPS = 60;
            this.MovieRecordCameras = new List<MovieRecordCamera>();
            var worker = new Thread(new ThreadStart(this.FrameOutProcess));
            worker.Start();
        }
        public void OnApplicationQuit()
        {
            this.needAbort = true;
        }

        public void AddCamera(MovieRecordCamera camera)
        {
            this.MovieRecordCameras.Add(camera);
        }

        public void RemoveCamera(MovieRecordCamera camera)
        {
            this.MovieRecordCameras.Remove(camera);
        }

        private void FrameOutProcess()
        {
            var sw = new Stopwatch();
            sw.Start();
            var interval = (1000 / this.FPS);
            while (!this.needAbort)
            {
                if ((sw.ElapsedMilliseconds > interval) && (this.MovieRecordCameras.Count > 0))
                {
                    taskFactory.StartNew(() => this.FrameOutInMainThread());
                    sw.Reset();
                    sw.Start();
                }
            }
        }

        private void FrameOutInMainThread()
        {
            GL.IssuePluginEvent(SfMovieRecord.GetRenderEventFunc(), 1);
        }
    }
}
