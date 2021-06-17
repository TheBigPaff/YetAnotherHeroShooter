using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class ScoreKeeper : MonoBehaviour
    {
        public static int score { get; private set; }
        float lastEnemyKilledTime;
        int streakCount;
        float streakExpireTime = 2f;

        private void Start()
        {
            EnemyScript.OnDeathStatic += OnEnemyKilled;
            FindObjectOfType<PlayerScript>().OnDeath += OnPlayerDeath;
            score = 0;
        }

        void OnEnemyKilled()
        {
            if (Time.time < lastEnemyKilledTime + streakExpireTime)
            {
                streakCount++;
            }
            else
            {
                streakCount = 0;
            }
            lastEnemyKilledTime = Time.time;
            score += 1 + (int)Mathf.Pow(2, streakCount);
        }

        void OnPlayerDeath()
        {
            EnemyScript.OnDeathStatic -= OnEnemyKilled;
        }
    }

}