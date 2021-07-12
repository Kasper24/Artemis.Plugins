using System.Threading;
using OpenTK.Audio.OpenAL;
using ALAudioCapture = OpenTK.Audio.AudioCapture;

namespace Artemis.Plugins.Audio.LayerEffects.AudioCapture
{
    public class OpenALAudioInput : IAudioInput
    {
        #region Properties & Fields

        private ALAudioCapture _audioCapture;
        private Timer _captureTimer;
        private short[] _buffer;

        private int _bufferLength => SampleRate * 50;

        public int SampleRate => 44100;
        public float MasterVolume => 1;

        #endregion

        #region Event

        public event AudioData DataAvailable;

        #endregion

        #region Methods

        public void Initialize()
        {
            _buffer = new short[_bufferLength];
            var recorders = ALAudioCapture.AvailableDevices;

            _audioCapture = new ALAudioCapture(recorders[0], SampleRate, ALFormat.Stereo16, _bufferLength);
            _audioCapture.Start();

            if (_captureTimer == null) 
            {
                _captureTimer = new Timer(OnRecording, null, 1, 1);
            }
        }

        public void Dispose()
        {
            _audioCapture.Stop();
            _audioCapture.Dispose();
            _audioCapture = null;
            _captureTimer.Dispose();
            _captureTimer = null;
        }

        private void OnRecording(object state)
        {
            var samplesAvailable = _audioCapture.AvailableSamples;
            _audioCapture.ReadSamples(_buffer, samplesAvailable);

            short left = 0;
            for (var i = 0; i < samplesAvailable; i++)
            {
                if (i % 2 == 0)
                {
                    left = _buffer[i];
                }
                else
                {
                    DataAvailable?.Invoke(left, _buffer[i]);
                }
            }
        }

        #endregion
    }
}