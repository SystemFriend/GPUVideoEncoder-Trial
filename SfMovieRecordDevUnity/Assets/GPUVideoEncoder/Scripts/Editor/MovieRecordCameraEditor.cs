using UnityEditor;

namespace GPUVideoEncoder
{
    [CustomEditor(typeof(MovieRecordCamera))]
    public sealed class MovieRecordCameraEditor : Editor
    {
        private static string[] audioDeviceNames;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MovieRecordCamera mrc = (this.target as MovieRecordCamera);

            var count = SfMovieRecord.GetAudioDeviceCount();
            //Debug.Log(string.Format("{0}", count));
            audioDeviceNames = new string[count];
            for (ulong i = 0; i < count; i++)
            {
                audioDeviceNames[i] = SfMovieRecord.GetAudioDeviceName(i);
                //Debug.Log(string.Format("{0}", name.ToString()));
            }
            var index = EditorGUILayout.Popup("Audio Device", mrc.audioDeviceIndex, audioDeviceNames);
            if (index != mrc.audioDeviceIndex)
            {
                Undo.RecordObject(mrc, "AudioDeviceIndex Change");
                mrc.audioDeviceIndex = index;
                EditorUtility.SetDirty(mrc);
            }
        }
    }
}