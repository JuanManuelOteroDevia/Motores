using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int maxLives = 3;
    private int currentHealth;
    private static int currentLivesStatic;

    [Header("UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI livesText;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Scene Reset")]
    public GameObject[] objectsToSave;
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;
    public float fadeHoldTime = 0.8f;
    public GameObject gameOverPrefab;

    private AudioSource audioSource;
    private bool isDead = false;

    class SavedState
    {
        public Vector3 pos;
        public Quaternion rot;
        public bool active;
        public Transform parent;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }

    private Dictionary<GameObject, SavedState> saved = new Dictionary<GameObject, SavedState>();

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        currentHealth = maxHealth;
        if (currentLivesStatic == 0) currentLivesStatic = maxLives;

        if (fadeCanvas != null) fadeCanvas.alpha = 0f;
        if (gameOverPrefab != null) gameOverPrefab.SetActive(false);

        CaptureInitialStates();
    }

    void Start()
    {
        currentLivesStatic = Mathf.Clamp(currentLivesStatic, 0, maxLives);
        UpdateUI();
    }

    void CaptureInitialStates()
    {
        saved.Clear();
        foreach (var go in objectsToSave)
        {
            if (go == null || go.GetComponent<PlayerHealth>() != null) continue;

            var st = new SavedState
            {
                pos = go.transform.position,
                rot = go.transform.rotation,
                active = go.activeSelf,
                parent = go.transform.parent
            };

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                st.velocity = rb.velocity;
                st.angularVelocity = rb.angularVelocity;
            }

            saved[go] = st;
        }
    }

    public void TakeDamage(int amount, Vector3 sourcePos)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("⚡ Daño recibido. Vida restante: " + currentHealth);

        if (hitSound) audioSource.PlayOneShot(hitSound);

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Vector3 dir = (transform.position - sourcePos).normalized;
            rb.AddForce(dir * 5f, ForceMode.Impulse);
        }

        UpdateUI();

        if (currentHealth <= 0)
        {
            currentLivesStatic--;
            StartCoroutine(DieCoroutine());
        }
    }

    IEnumerator DieCoroutine()
    {
        if (isDead) yield break;
        isDead = true;

        Debug.Log("☠ Muerte. Vidas restantes: " + currentLivesStatic);

        if (deathSound) audioSource.PlayOneShot(deathSound);

        if (fadeCanvas != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
                yield return null;
            }
        }

        yield return new WaitForSeconds(fadeHoldTime);

        RestoreSavedObjects();
        TeleportToSpawn();

        if (fadeCanvas != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
                yield return null;
            }
        }

        if (currentLivesStatic > 0)
        {
            currentHealth = maxHealth;
            isDead = false;
            UpdateUI();

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.Log("☠ GAME OVER: sin vidas.");
            if (gameOverPrefab != null) gameOverPrefab.SetActive(true);
            UpdateUI();

            FindObjectOfType<GameManager>()?.TriggerGameOver(); // ✅ Avisar al GameManager

            Time.timeScale = 0f;
        }
    }

    void RestoreSavedObjects()
    {
        foreach (var kv in saved)
        {
            var go = kv.Key;
            var st = kv.Value;
            if (go == null || go.GetComponent<PlayerHealth>() != null) continue;

            go.SetActive(st.active);
            go.transform.SetParent(st.parent);
            go.transform.position = st.pos;
            go.transform.rotation = st.rot;

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }

            var anim = go.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }

            var resettable = go.GetComponent<IResettable>();
            if (resettable != null) resettable.ResetState();
        }
    }

    void TeleportToSpawn()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            var ip = GetComponent<InitialPosition>();
            if (ip != null)
            {
                transform.position = ip.startPos;
                transform.rotation = ip.startRot;
            }
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"Vida: {currentHealth}/{maxHealth}";

        if (livesText != null)
            livesText.text = $"Vidas: {currentLivesStatic}";
    }

    public static int GetCurrentLives()
    {
        return currentLivesStatic;
    }
}

public interface IResettable
{
    void ResetState();
}

public class InitialPosition : MonoBehaviour
{
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }
}
