using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMonster : MonoBehaviour
{
    public MonsterHealth health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            health.GetDamage(collision.GetComponent<Projectile>().damage);
        }
    }
}
