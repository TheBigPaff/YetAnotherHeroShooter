using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace SinglePlayerMode 
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyScript : LivingEntity
    {

        public float damage = 1f;
        public ParticleSystem deathEffect;
        // public ParticleSystem hitEffect;      TO DO! ----------------
        public static event System.Action OnDeathStatic;
        private enum States { Idle, Chasing, Attacking };
        private States currentState;
        LivingEntity targetEntity;
        private Transform targetTransform;
        private NavMeshAgent pathfinder;

        [SerializeField] private float attackSpeed = 2.5f;

        private float distanceThreshold = .5f;
        private float timeBetweenAttacks = 1f;
        private float timeForNextAttack;
        private float enemyColliderRadius;
        private float targetColliderRadius;

        private bool hasTarget;


        private Material enemyMaterial;
        private Color originalMatColor;

        private void Awake()
        {
            pathfinder = GetComponent<NavMeshAgent>();

            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                hasTarget = true;

                targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
                targetEntity = targetTransform.GetComponent<LivingEntity>();

                enemyColliderRadius = GetComponent<CapsuleCollider>().radius;
                targetColliderRadius = GetComponent<CapsuleCollider>().radius;
            }
        }
        protected override void Start()
        {
            base.Start();

            if (hasTarget)
            {
                currentState = States.Chasing;
                targetEntity.OnDeath += onTargetDeath;

                StartCoroutine(UpdatePath());
            }
        }

        public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
        {
            pathfinder.speed = moveSpeed;

            if (hasTarget)
            {
                damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
            }
            startingHealth = enemyHealth;

            var main = deathEffect.main;
            main.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
            enemyMaterial = GetComponent<Renderer>().material;
            enemyMaterial.color = skinColor;
            originalMatColor = enemyMaterial.color;
        }

        public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            AudioManager.instance.PlaySound("Impact", transform.position);
            if (damage >= health)
            {
                if (OnDeathStatic != null)
                {
                    OnDeathStatic();
                    Debug.Log("You killed him!");
                }
                AudioManager.instance.PlaySound("Enemy Death", transform.position);
                Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            }
            base.TakeHit(damage, hitPoint, hitDirection);
        }

        private void onTargetDeath()
        {
            hasTarget = false;
            currentState = States.Idle;
        }

        private void Update()
        {
            if (hasTarget)
            {
                if (Time.time > timeForNextAttack)
                {
                    float sqrDistanceToTarget = (targetTransform.position - transform.position).sqrMagnitude;
                    if (sqrDistanceToTarget < ((distanceThreshold + enemyColliderRadius + targetColliderRadius) * (distanceThreshold + enemyColliderRadius + targetColliderRadius)))
                    {
                        timeForNextAttack = Time.time + timeBetweenAttacks;
                        AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                        StartCoroutine(Attack());
                    }
                }
            }
        }


        IEnumerator Attack()
        {
            currentState = States.Attacking;
            pathfinder.enabled = false;

            enemyMaterial.color = Color.red;

            Vector3 originalPosition = transform.position;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            Vector3 attackPosition = targetTransform.position - dirToTarget * enemyColliderRadius;


            float percentage = 0f;

            bool hasAppliedDamage = false;
            while (percentage <= 1)
            {
                if (percentage >= .5f && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    targetEntity.TakeDamage(damage);
                }
                percentage += Time.deltaTime * attackSpeed;
                float interpolation = (((-percentage * percentage) + percentage) * 4);
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

                yield return null;
            }

            enemyMaterial.color = originalMatColor;
            currentState = States.Chasing;
            pathfinder.enabled = true;
        }

        IEnumerator UpdatePath()
        {
            float updatePathRate = .25f;

            while (hasTarget)
            {
                if (currentState == States.Chasing)
                {
                    Vector3 targetDirection = (targetTransform.position - transform.position).normalized;
                    Vector3 targetPosition = targetTransform.position - (targetDirection * (enemyColliderRadius + targetColliderRadius + distanceThreshold / 2));
                    if (!dead)
                    {
                        pathfinder.SetDestination(targetPosition);
                    }
                }
                yield return new WaitForSeconds(updatePathRate);
            }

        }
    }
}