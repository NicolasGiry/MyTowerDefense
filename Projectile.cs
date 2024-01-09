using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 shootDir;
    public float damage;
    public void Setup(Vector3 dir)
    {
        shootDir = dir.normalized;
        transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Destroy(gameObject);
        }
    }
}
