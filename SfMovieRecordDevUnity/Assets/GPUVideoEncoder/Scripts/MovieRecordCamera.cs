using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GPUVideoEncoder
{
    public class MovieRecordCamera : MonoBehaviour
    {
        public RenderTexture texture;
        public string outputFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public string outputFileTitle = "movie";
        public bool withAudio = true;
        [HideInInspector]
        public int audioDeviceIndex;

        private bool isRecording = false;
        private GPUVideoEncodeManager Manager { get; set; }

        static void SfMovieRecordPluginDebugCallBack(string message)
        {
            Debug.Log("GPUVideoEncoder : " + message);
        }

        void Start()
        {
            var manager = GameObject.FindObjectOfType(typeof(GPUVideoEncodeManager));
            if (manager == null)
            {
                manager = (GameObject)Instantiate(Resources.Load("Prefabs/GPUVideoEncodeManager"));
                manager.name = "GPUVideoEncodeManager";
                this.Manager = (manager as GameObject).GetComponent<GPUVideoEncodeManager>();
            }
            else
            {
                this.Manager = (manager as GPUVideoEncodeManager);
            }

            var debugDelegate = new SfMovieRecord.DebugDelegate(SfMovieRecordPluginDebugCallBack);
            var functionPointer = Marshal.GetFunctionPointerForDelegate(debugDelegate);
            SfMovieRecord.SetDebugFunction(functionPointer);
        }

        public void OnApplicationQuit()
        {
            this.EndMovieRecord();
            SfMovieRecord.SetDebugFunction(IntPtr.Zero);
        }

        public void StartMovieRecord()
        {
            if (!this.isRecording && (this.texture.GetNativeTexturePtr() != IntPtr.Zero) && this.Manager.IsEncoderAddable)
            {
                SfMovieRecord.StartMovieRecord(this.outputFilePath, this.outputFileTitle, this.texture.GetNativeTexturePtr(), this.Manager.FPS, this.withAudio, (ulong)this.audioDeviceIndex);
                this.Manager.AddCamera(this);
                this.isRecording = true;
            }
        }

        public void EndMovieRecord()
        {
            if (this.isRecording)
            {
                this.StartCoroutine(this.FrameOutFinalize());
            }
        }

        private IEnumerator FrameOutFinalize()
        {
            this.Manager.RemoveCamera(this);
            yield return new WaitForEndOfFrame();
            SfMovieRecord.EndMovieRecord(this.outputFilePath, this.outputFileTitle);
            this.isRecording = false;
        }
    }
}

