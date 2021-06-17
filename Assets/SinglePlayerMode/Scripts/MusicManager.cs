using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinglePlayerMode
{
    public class MusicManager : MonoBehaviour
    {
        public AudioClip hordesTheme;
        public AudioClip menuTheme;

        string sceneName;

        private void Awake()
        {
            SceneManager.activeSceneChanged += newLevel;
        }

        private void newLevel(Scene replacedScene, Scene newScene)
        {
            string newSceneName = SceneManager.GetActiveScene().name;

            if (newSceneName != sceneName)
            {
                sceneName = newSceneName;
                Invoke("PlayMusic", .2f);
            }
        }

        void PlayMusic()
        {
            AudioClip clipToPlay = null;

            if (sceneName == "Menu")
            {
                clipToPlay = menuTheme;
            }
            else if (sceneName == "Hordes")
            {
                clipToPlay = hordesTheme;
            }

            if (clipToPlay != null)
            {
                AudioManager.instance.PlayMusic(clipToPlay, 2);
                Invoke("PlayMusic", clipToPlay.length);
            }
        }
    }

}