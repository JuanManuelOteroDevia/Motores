using UnityEngine;

public class Fall : MonoBehaviour
{
    [Header("Spawn")]
    public Transform spawnPoint; 

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>()
                           ?? collision.collider.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            Debug.Log("☠ El jugador tocó el bloque de muerte");

            int dañoLetal = PlayerHealth.GetCurrentLives() > 1 ? player.maxHealth : player.maxHealth + 1;
            player.TakeDamage(dañoLetal, transform.position);

            if (spawnPoint != null)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }
        }
    }
}
