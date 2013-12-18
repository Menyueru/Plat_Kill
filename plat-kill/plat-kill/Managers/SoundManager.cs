using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using plat_kill.Helpers.States;
using System.Collections.Generic;
using System;

namespace plat_kill.Managers
{
    public class SoundManager
    {
        #region Propierties
        private PKGame Game;

        private List<Song> Songs;
        private int currentSongIndex;

        private Dictionary<SoundEffects, SoundEffectInstance> SoundFXs;
        private SoundEffect GunShot;

        private float MasterVolume;

        #endregion

        public SoundManager(PKGame game, int MasterVolume) 
        {
            this.Game = game;
            this.MasterVolume = MasterVolume;
            this.Songs = new List<Song>();
            this.SoundFXs = new Dictionary<SoundEffects, SoundEffectInstance>();

            MediaPlayer.Volume = MasterVolume/10;

            this.LoadSounds(game.Content);
        }

        #region Methods
        private void LoadSounds(ContentManager content) 
        {
            //Loading Music
            Songs.Add(content.Load<Song>("Sounds\\Music\\portals"));
            Songs.Add(content.Load<Song>("Sounds\\Music\\tristram"));
            Songs.Add(content.Load<Song>("Sounds\\Music\\wreckingBall"));
            
            Random r = new Random();
            this.currentSongIndex = r.Next(Songs.Count);

            //Loading SoundFXs
            this.SoundFXs.Add(SoundEffects.Dodge, content.Load<SoundEffect>("Sounds\\SoundFX\\Dodge").CreateInstance());
            this.SoundFXs.Add(SoundEffects.GunShot, content.Load<SoundEffect>("Sounds\\SoundFX\\gunShot").CreateInstance());
            this.SoundFXs.Add(SoundEffects.MeleeSwing, content.Load<SoundEffect>("Sounds\\SoundFX\\MeleeSwing").CreateInstance());
            this.SoundFXs.Add(SoundEffects.Step, content.Load<SoundEffect>("Sounds\\SoundFX\\Step").CreateInstance());
            this.SoundFXs.Add(SoundEffects.YouLoose, content.Load<SoundEffect>("Sounds\\SoundFX\\YouLoose").CreateInstance());
            this.SoundFXs.Add(SoundEffects.YouWin, content.Load<SoundEffect>("Sounds\\SoundFX\\YouWin").CreateInstance());
            this.SoundFXs.Add(SoundEffects.Reload, content.Load<SoundEffect>("Sounds\\SoundFX\\Reload").CreateInstance());
            this.GunShot = content.Load<SoundEffect>("Sounds\\SoundFX\\gunShot");

            foreach(var soundFX in this.SoundFXs)
            {
                soundFX.Value.Volume = (MasterVolume/100);
            }

        }

        private void PlayNextSong() 
        {
            if ((currentSongIndex + 1) < Songs.Count)
            {
                currentSongIndex++;
            }
            else 
            {
                currentSongIndex = 0;
            }

            this.PlaySong();
        }

        private void PlaySong()
        {
            MediaPlayer.Play(Songs[currentSongIndex]);
        }

        public void StartBackgroundMusic() 
        {
            this.PlaySong();
        }

        public void StopBackgroundMusic() 
        {
            MediaPlayer.Stop();
        }

        public void PlaySoundFX(SoundEffects effect) 
        {
            if (effect.Equals(SoundEffects.GunShot))
            {
                this.GunShot.Play();
            }
            else 
            {
                if (this.SoundFXs[effect].State == SoundState.Stopped)
                {
                    this.SoundFXs[effect].Play();
                }
            }
        }

        public void StopSoundFX(SoundEffects effect)
        {
            if (this.SoundFXs[effect].State == SoundState.Playing)
            {
                this.SoundFXs[effect].Stop();
            }
        }

        public void Update() 
        {
            TimeSpan time = MediaPlayer.PlayPosition;
            TimeSpan songTime = Songs[currentSongIndex].Duration;

            if(time.Equals(songTime))
            {
                PlayNextSong();
            }
        }

        #endregion

    }
}
