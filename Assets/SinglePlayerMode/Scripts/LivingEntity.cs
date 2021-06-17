using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class LivingEntity : MonoBehaviour, IDamageable
    {
        public float startingHealth;
        public event System.Action OnDeath;

        public float health { get; protected set; }
        public bool dead;

        protected virtual void Start()
        {
            health = startingHealth;
        }

        public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            //Do some stuff with hit var later on
            TakeDamage(damage);
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0 && !dead)
            {
                Die();
            }
        }
        [ContextMenu("Self Descruct")]
        public virtual void Die()
        {
            GameObject.Destroy(gameObject);
            OnDeath();
            dead = true;
        }
    }
}
