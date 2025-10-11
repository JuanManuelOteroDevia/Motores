using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Skeleton : MonoBehaviour
{
    [Header("Target / Movement")]
    public Transform target;
    public float moveSpeed = 3f;
    public float detectionRange = 7f;
    public float stoppingDistance = 1.5f;

    [Header("Damage")]
    public int contactDamage = 10;
    public float pushForce = 5f;
    public float damageCooldown = 1f;

    [Header("Health")]
    public int maxHealth = 30;
    private int currentHealth;

    private Animator animator;
    private Rigidbody rb;
    private float lastDamageTime = -999f;

    const string PARAM_ISRUNNING = "IsRuning"; 

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (target == null || IsDead()) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= detectionRange)
        {
            if (dist > stoppingDistance)
                MoveTowards(target.position);
            else
                StopMoving();
        }
        else
        {
            StopMoving();
        }
    }

    void MoveTowards(Vector3 destination)
    {
        Vector3 dir = destination - transform.position;
        dir.y = 0f;
        Vector3 move = dir.normalized * moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, look, 10f * Time.fixedDeltaTime));
        }

        animator.SetBool(PARAM_ISRUNNING, true);
    }

    void StopMoving()
    {
        animator.SetBool(PARAM_ISRUNNING, false);
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsDead() || Time.time - lastDamageTime < damageCooldown) return;

        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>()
                           ?? collision.collider.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            lastDamageTime = Time.time;
            player.TakeDamage(contactDamage, transform.position);

            Rigidbody rbPlayer = player.GetComponent<Rigidbody>();
            if (rbPlayer != null)
            {
                Vector3 dir = (player.transform.position - transform.position).normalized;
                rbPlayer.velocity = Vector3.zero;
                rbPlayer.AddForce(dir * pushForce, ForceMode.Impulse);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead()) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StopMoving();
        Destroy(gameObject);
    }

    bool IsDead()
    {
        return currentHealth <= 0;
    }
}
