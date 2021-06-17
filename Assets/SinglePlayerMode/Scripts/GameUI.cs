using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SinglePlayerMode
{
    public class GameUI : MonoBehaviour
    {
        public Image fadeImage;
        public GameObject gameOverUI;

        public RectTransform newWaveBanner;
        public Text newWaveTitle;
        public Text newWaveEnemyCount;
        public Text scoreUI;
        public Text GO_CurrentScoreUI;
        public RectTransform healthBar;
        Spawner spawner;
        PlayerScript player;


        private void Start()
        {
            player = FindObjectOfType<PlayerScript>();
            player.OnDeath += OnGameOver;
        }

        private void Awake()
        {
            spawner = FindObjectOfType<Spawner>();
            spawner.OnNewWave += OnNewWave;
        }
        private void Update()
        {
            scoreUI.text = ScoreKeeper.score.ToString("D6");
            float healthPercent = 0;
            if (player != null)
            {
                healthPercent = player.health / player.startingHealth;
            }
            healthBar.localScale = new Vector3(healthPercent, 1, 1);
        }
        void OnNewWave(int waveNumber)
        {
            string[] numbers = { "One", "Two", "Three", "Four", "Five" };
            newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
            string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount.ToString());
            newWaveEnemyCount.text = "Enemies: " + enemyCountString;

            StopCoroutine("AnimateNewWaveBanner");
            StartCoroutine("AnimateNewWaveBanner");
        }

        void OnGameOver()
        {
            Cursor.visible = true;
            StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .9f), 0.5f));
            scoreUI.gameObject.SetActive(false);
            healthBar.transform.parent.gameObject.SetActive(false);
            GO_CurrentScoreUI.text = scoreUI.text;
            gameOverUI.SetActive(true);
        }

        IEnumerator AnimateNewWaveBanner()
        {
            float delayTime = 1.5f;
            float speed = 3f;
            float animatePercent = 0;
            int dir = 1;

            float endDelayTime = Time.time + 1 / speed + delayTime;

            while (animatePercent >= 0)
            {
                animatePercent += Time.deltaTime * speed * dir;

                if (animatePercent >= 1)
                {
                    animatePercent = 1;
                    if (Time.time > endDelayTime)
                    {
                        dir = -1;
                    }
                }

                newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-567.06f, -288f, animatePercent);
                yield return null;
            }


        }
        IEnumerator Fade(Color from, Color to, float time)
        {
            float speed = 1 / time;
            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                fadeImage.color = Color.Lerp(from, to, percent);
                yield return null;
            }
        }

        //UI Input
        public void StartNewGame()
        {
            SceneManager.LoadScene("Hordes");
        }
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
}
