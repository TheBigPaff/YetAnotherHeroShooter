using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float damage;
    public LayerMask enemyLayer;
    //public Color trailColor;

    private float speed = 10f;
    private float lifetime = 4f;
    private float distance; //Move Distance

    private float skinWidth = .1f;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] collisions = Physics.OverlapSphere(transform.position, .5f, enemyLayer);
        if (collisions.Length > 0)
        {
            OnGameObjectHit(collisions[0], transform.position);
        }
        //GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    private void Update()
    {
        distance = speed * Time.deltaTime;
        CheckCollision(distance);
        transform.Translate(Vector3.forward * distance);
    }



    private void CheckCollision(float distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance + skinWidth, enemyLayer, QueryTriggerInteraction.Collide)) //We use "QueryTriggenInteraction.Collide" because the enemy collider has isTrigger = true, and it only works with colliders usually
        {
            OnGameObjectHit(hit.collider, hit.point);
        }

    }

    private void OnGameObjectHit(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject.Destroy(gameObject);
    }
}

