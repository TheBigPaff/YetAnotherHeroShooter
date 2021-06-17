using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class AudioManager : MonoBehaviour
    {

        public enum AudioChannel { Master, Sfx, Music };
        public float masterVolumePercent { get; private set; }
        public float sfxVolumePercent { get; private set; }
        public float musicVolumePercent { get; private set; }

        AudioSource sfx2DSource;
        AudioSource[] musicSources;
        int activeMusicSourceIndex;

        public static AudioManager instance;

        Transform audioListener;
        Transform player;

        SoundLibrary library;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                library = GetComponent<SoundLibrary>();

                musicSources = new AudioSource[2];

                for (int i = 0; i < musicSources.Length; i++)
                {
                    GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                    musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                    newMusicSource.transform.parent = transform;
                }
                GameObject newSfx2DSource = new GameObject("Sfx 2D Source");
                sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
                newSfx2DSource.transform.parent = transform;

                audioListener = FindObjectOfType<AudioListener>().transform;
                if (FindObjectOfType<PlayerScript>() != null)
                {
                    player = FindObjectOfType<PlayerScript>().transform;
                }

                masterVolumePercent = PlayerPrefs.GetFloat("masterVolume", 1);
                sfxVolumePercent = PlayerPrefs.GetFloat("sfxVolume", 1);
                musicVolumePercent = PlayerPrefs.GetFloat("musicVolume", 1);
            }
        }
        private void Update()
        {
            if (player != null)
            {
                audioListener.position = player.position;
            }
        }
        public void SetVolume(float volumePercent, AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Master:
                    masterVolumePercent = volumePercent;
                    break;
                case AudioChannel.Sfx:
                    sfxVolumePercent = volumePercent;
                    break;
                case AudioChannel.Music:
                    musicVolumePercent = volumePercent;
                    break;
            }

            for (int i = 0; i < musicSources.Length; i++)
            {
                musicSources[i].volume = musicVolumePercent * masterVolumePercent;
            }

            PlayerPrefs.SetFloat("masterVolume", masterVolumePercent);
            PlayerPrefs.SetFloat("sfxVolume", sfxVolumePercent);
            PlayerPrefs.SetFloat("musicVolume", musicVolumePercent);
            PlayerPrefs.Save();
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1)
        {
            activeMusicSourceIndex = 1 - activeMusicSourceIndex;
            musicSources[activeMusicSourceIndex].clip = clip;
            musicSources[activeMusicSourceIndex].Play();

            StartCoroutine(AnimateMusicCrossfade(fadeDuration));
        }

        public void PlaySound(AudioClip clip, Vector3 pos)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            }
        }
        public void PlaySound(string soundname, Vector3 pos)
        {
            PlaySound(library.GetClipFromName(soundname), pos);
        }
        public void PlaySound2D(string soundName)
        {
            sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
        }
        IEnumerator AnimateMusicCrossfade(float duration)
        {

            float percent = 0;

            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / duration;
                musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
                musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
                yield return null;
            }

        }
    }
}

