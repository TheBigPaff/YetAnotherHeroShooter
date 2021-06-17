using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class Spawner : MonoBehaviour
    {
        public bool devMode;

        public Wave[] waves;
        public EnemyScript enemy;

        private Wave currentWave;
        float nextSpawnTime;
        int enemiesRemaingAlive;
        int currentWaveNumber = 0;
        int remainingEnemiesToSpawn;
        int randomSeedForEnemySpawning = 10;


        LivingEntity playerEntity;
        Transform playerT;

        MapGenerator map;

        float timeBetweenCampingChecks = 2f;
        float campThresholdDistance = 1.5f;
        float nextCampCheckTime;
        Vector3 campPositionOld;
        bool isCamping;
        bool isDisabled;

        public event System.Action<int> OnNewWave;
        private void Start()
        {
            playerEntity = FindObjectOfType<PlayerScript>();
            playerEntity.OnDeath += OnPlayerDeath;
            playerT = playerEntity.transform;
            nextCampCheckTime = timeBetweenCampingChecks + Time.time;
            campPositionOld = playerT.position;
            map = FindObjectOfType<MapGenerator>();
            NextWave();

        }

        IEnumerator SpawnEnemy()
        {
            int randomNumber = Random.Range(0, 100);
            float spawnFlashDelay = 1;
            float tileFlashSpeed = 4;

            Transform spawnTile = map.GetRandomOpenTile();
            if (isCamping)
            {
                if (randomNumber < 10)
                {

                    spawnTile = map.GetTileFromPosition(new Vector3(playerT.position.x + 1, playerT.position.y, playerT.position.z));
                }
                else if (randomNumber < 20 && randomNumber > 10)
                {
                    spawnTile = map.GetTileFromPosition(new Vector3(playerT.position.x - 1, playerT.position.y, playerT.position.z));
                }
                else if (randomNumber < 30 && randomNumber > 20)
                {
                    spawnTile = map.GetTileFromPosition(new Vector3(playerT.position.x, playerT.position.y, playerT.position.z + 1));
                }
                else if (randomNumber < 40 && randomNumber > 30)
                {
                    spawnTile = map.GetTileFromPosition(new Vector3(playerT.position.x, playerT.position.y, playerT.position.z - 1));
                }
                else
                {
                    spawnTile = map.GetTileFromPosition(playerT.position);
                }
            }
            Material tileMat = spawnTile.GetComponent<Renderer>().material;
            Color initialColor = Color.white;
            Color flashColor = Color.red;
            float spawnTimer = 0;

            while (spawnTimer < spawnFlashDelay)
            {
                tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
                spawnTimer += Time.deltaTime;

                yield return null;
            }

            EnemyScript spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as EnemyScript;
            spawnedEnemy.OnDeath += OnEnemyDeath; //Delegate and events thingy
            spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
        }

        private void Update()
        {


            if (!isDisabled)
            {

                //checks if is camping
                if (Time.time > nextCampCheckTime)
                {
                    nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                    isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                    campPositionOld = playerT.position;
                }


                if ((remainingEnemiesToSpawn > 0 || currentWave.infinite) && (Time.time >= nextSpawnTime))
                {
                    remainingEnemiesToSpawn--;
                    nextSpawnTime = Time.time + currentWave.timeBtwnSpawns;
                    StartCoroutine("SpawnEnemy");
                }

                if (devMode)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        StopCoroutine("SpawnEnemy");
                        foreach (EnemyScript enemy in FindObjectsOfType<EnemyScript>())
                        {
                            GameObject.Destroy(enemy.gameObject);
                        }
                        NextWave();
                    }
                }
            }



        }
        void OnPlayerDeath()
        {
            isDisabled = true;
        }

        void OnEnemyDeath()
        {
            enemiesRemaingAlive--;
            if (enemiesRemaingAlive == 0)
            {
                NextWave();
            }
        }

        void ResetPlayerPosition()
        {
            playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2;
        }

        void NextWave()
        {
            if (currentWaveNumber > 0)
            {
                AudioManager.instance.PlaySound2D("Level Complete");
            }
            currentWaveNumber++;
            if ((currentWaveNumber - 1) < waves.Length)
            {
                currentWave = waves[currentWaveNumber - 1];
                remainingEnemiesToSpawn = currentWave.enemyCount;
                nextSpawnTime = currentWave.timeBtwnSpawns;
                enemiesRemaingAlive = currentWave.enemyCount;

                if (OnNewWave != null)
                {
                    OnNewWave(currentWaveNumber);
                }
            }
            ResetPlayerPosition();

        }

        [System.Serializable]
        public class Wave
        {
            public bool infinite;
            public int enemyCount;
            public float timeBtwnSpawns;

            public float moveSpeed;
            public int hitsToKillPlayer;
            public float enemyHealth;
            public Color skinColor;

        }
    }

}