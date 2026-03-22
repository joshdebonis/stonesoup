using UnityEngine;

namespace Lam
{
    public class ZoomRecorder : Tile
    {
        private AudioClip recordedClip;
        private AudioSource audioSource;

        private bool isRecording = false;
        private bool isPlaying = false;

        private string micDevice;
        private int sampleRate = 44100;
        private int maxLengthSeconds = 30;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            if (Microphone.devices.Length > 0)
            {
                micDevice = Microphone.devices[0];
            }
            else
            {
                Debug.LogError("No microphone detected!");
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!isPlaying)
                {
                    StartPlayback();
                }
                else
                {
                    StopPlayback();
                }
            }
        }

        public override void useAsItem(Tile tileUsingUs)
        {
            base.useAsItem(tileUsingUs);
            
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        void StartRecording()
        {
            if (micDevice == null) return;

            if (isPlaying)
                StopPlayback();

            Debug.Log("Recording Started");

            recordedClip = Microphone.Start(
                micDevice,
                false,
                maxLengthSeconds,
                sampleRate
            );

            isRecording = true;
        }

        void StopRecording()
        {
            if (!isRecording) return;

            int position = Microphone.GetPosition(micDevice);
            Microphone.End(micDevice);

            float[] samples = new float[position];
            recordedClip.GetData(samples, 0);

            AudioClip trimmedClip = AudioClip.Create(
                "RecordedClip",
                position,
                1,
                sampleRate,
                false
            );

            trimmedClip.SetData(samples, 0);

            recordedClip = trimmedClip;

            isRecording = false;

            Debug.Log("Recording Stopped");
        }

        void StartPlayback()
        {
            if (recordedClip == null) return;

            Debug.Log("Playback Started");

            audioSource.clip = recordedClip;
            audioSource.Play();

            isPlaying = true;
        }

        void StopPlayback()
        {
            audioSource.Stop();
            isPlaying = false;

            Debug.Log("Playback Stopped");
        }
    }
}