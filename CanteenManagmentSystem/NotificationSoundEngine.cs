using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CSCore;
using CSCore.SoundOut;
using CSCore.CoreAudioAPI;

namespace CanteenManagmentSystem
{
    public class NotificationSoundEngine:Component
    {
        public IWaveSource waveSource;
        public ISoundOut soundOut;

        public EventHandler<PlaybackStoppedEventArgs> PlayBackStopped;


        public PlaybackState PlaybackState
        {
            get
            {
                return soundOut != null ? soundOut.PlaybackState : PlaybackState.Stopped;
            }
        }
        public void OpenMedia(string FileName)
        {
            try
            {
                CleanupPlayback();
                waveSource = CSCore.Codecs.CodecFactory.Instance.GetCodec(FileName)
               .ToSampleSource()
               .ToMono()
               .ToWaveSource();

                soundOut = new WasapiOut() { Latency = 100, Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia), Volume = 1 };
                soundOut.Initialize(waveSource);
                if (PlayBackStopped != null) soundOut.Stopped += PlayBackStopped;
                if (soundOut != null)
                    soundOut.Play();
            }
            catch (Exception) {  }
        }

        public void CleanupPlayback()
        {
            if (soundOut != null)
            {
                soundOut.Dispose();
                soundOut = null;
            }

            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}
