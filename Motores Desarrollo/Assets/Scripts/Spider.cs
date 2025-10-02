using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Spider : MonoBehaviour
{
    [Header("Target / Movement")]
    public Transform target;
    public float moveSpeed = 3.5f;
    public float detectionRange = 6f;
    public float stoppingDistance = 1.2f;

    [Header("Attack")]
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;
    private float lastAttackTime = -999f;

    const string PARAM_ISWALKING = "isWalking";
    const string PARAM_ATTACK = "Attack";
    const string PARAM_HIT = "Hit";
    const string PARAM_DEAD = "Dead";

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (isDead || target == null) return;

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

        // ❌ Eliminamos la rotación para que no se dé vuelta
        // El Spider se mueve sin mirar al jugador (como si lo persiguiera con la cola)

        animator.SetBool(PARAM_ISWALKING, true);
    }

    void StopMoving()
    {
        animator.SetBool(PARAM_ISWALKING, false);
    }

    void OnCollisionStay(Collision collision)
    {
        if (isDead || Time.time - lastAttackTime < attackCooldown) return;

        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>()
                           ?? collision.collider.GetComponentInParent<PlayerHealth>();

        if (player != null && collision.transform == target)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger(PARAM_ATTACK);
            Debug.Log("🕷 Spider golpeó al jugador");
            player.TakeDamage(attackDamage, transform.position);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        animator.SetTrigger(PARAM_HIT);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetBool(PARAM_DEAD, true);
        rb.isKinematic = true;
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        Destroy(gameObject, 3f);
    }
}
