using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float damageInterval = 0.5f;
    private float lastDamageTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamageImmediate(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DealDamageOverTime(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamageImmediate(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DealDamageOverTime(collision.collider);
    }

    private void DealDamageImmediate(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        ApplyDamage();
    }

    private void DealDamageOverTime(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (damageInterval <= 0f || Time.time - lastDamageTime >= damageInterval)
        {
            ApplyDamage();
        }
    }

    private void ApplyDamage()
    {
        if (HealthManager.instance != null)
        {
            HealthManager.instance.HurtPlayer();
            lastDamageTime = Time.time;
        }
        else
        {
            Debug.LogError("Chưa tìm thấy HealthManager trong Scene!");
        }
    }
}