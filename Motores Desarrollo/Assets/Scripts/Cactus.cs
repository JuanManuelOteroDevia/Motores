using UnityEngine;

public class Cactus : MonoBehaviour
{
    [Header("Daño y empuje")]
    public int damage = 20;
    public float pushForce = 6f;
    public float hitCooldown = 1f;

    private float lastHitTime = -999f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time - lastHitTime > hitCooldown)
        {
            lastHitTime = Time.time;
            Debug.Log("🌵 El cactus dañó al jugador");

            // Aplicar daño
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, transform.position);
            }

            // Aplicar empujón
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }
        }
    }
}
