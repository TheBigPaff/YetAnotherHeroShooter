using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class MuzzleFlash : MonoBehaviour
    {
        public GameObject flashHolder;
        public Sprite[] flashSprites;
        public SpriteRenderer[] spriteRenderers;

        public float flashTime;

        private void Start()
        {
            DeActivate();
        }

        public void Activate()
        {
            flashHolder.SetActive(true);
            int flashSpritesIndex = Random.Range(0, flashSprites.Length);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = flashSprites[flashSpritesIndex];
            }

            Invoke("DeActivate", flashTime);
        }
        void DeActivate()
        {
            flashHolder.SetActive(false);

        }
    }
}
