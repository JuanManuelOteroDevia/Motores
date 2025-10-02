using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class WhipController : MonoBehaviour
{
    [Header("Configuración")]
    public float whipRange = 20f;
    public LayerMask grappleMask;
    public LayerMask enemyMask;
    public LineRenderer lineRenderer;
    public Transform whipOrigin;
    public int whipDamage = 20;
    public float lineWidth = 0.05f;
    public float grappleSpeed = 15f;

    [Header("Sonidos")]
    public AudioClip hitSound;
    public AudioClip grabSound;
    public AudioClip grappleSound;

    private bool isSwinging = false;
    private Vector3 grapplePoint;
    private Rigidbody rb;
    private GameObject crosshair;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // LineRenderer setup
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
            if (lineRenderer.material == null)
            {
                var mat = new Material(Shader.Find("Unlit/Color"));
                mat.color = Color.yellow;
                lineRenderer.material = mat;
            }
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

        // AudioSource setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        CreateCrosshair();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));

        if (Input.GetMouseButtonDown(0)) TryWhip(ray);
        if (Input.GetMouseButtonUp(0) && isSwinging) StopSwing();

        // Actualizar línea solo al columpiar
        if (lineRenderer != null &&
            lineRenderer.enabled &&
            whipOrigin != null &&
            isSwinging)
        {
            lineRenderer.SetPosition(0, whipOrigin.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }

        // Movimiento hacia el grapplePoint
        if (isSwinging)
        {
            Vector3 dir = (grapplePoint - transform.position).normalized;
            rb.MovePosition(transform.position + dir * grappleSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, grapplePoint) < 1f)
                StopSwing();
        }
    }

    void TryWhip(Ray ray)
    {
        var mask = enemyMask | grappleMask;
        RaycastHit[] hits = Physics.RaycastAll(ray, whipRange, mask);
        if (hits.Length == 0)
        {
            Debug.Log("[Whip] No golpeó nada");
            return;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var h in hits)
        {
            // 1) Enemigo (solo Tag)
            if (h.collider.CompareTag("Enemy"))
            {
                // Llamar a TakeDamage en cualquier script que lo maneje
                h.collider.SendMessage("TakeDamage", whipDamage, SendMessageOptions.DontRequireReceiver);
                if (hitSound != null)
                    audioSource.PlayOneShot(hitSound);
                StartCoroutine(ShowWhipLine(h.point));
                return;
            }

            // 2) Empujar Objetos
            if (h.collider.CompareTag("Object"))
            {
                Rigidbody objRb = h.collider.attachedRigidbody;
                if (objRb != null)
                {
                    Vector3 dir = (whipOrigin.position - h.point).normalized;
                    objRb.AddForce(dir * 8f, ForceMode.Impulse);
                    if (grabSound != null)
                        audioSource.PlayOneShot(grabSound);
                    StartCoroutine(ShowWhipLine(h.point));
                }
                return;
            }

            // 3) GrapplePoint
            if (h.collider.CompareTag("GrapplePoint"))
            {
                grapplePoint = h.point;
                isSwinging = true;
                if (lineRenderer != null)
                    lineRenderer.enabled = true;
                if (grappleSound != null)
                    audioSource.PlayOneShot(grappleSound);
                Debug.Log("[Whip] Enganchado al GrapplePoint.");
                return;
            }
        }
    }

    void StopSwing()
    {
        isSwinging = false;
        if (lineRenderer != null)
            lineRenderer.enabled = false;
        Debug.Log("[Whip] Dejaste de usar el whip.");
    }

    private IEnumerator ShowWhipLine(Vector3 target, float duration = 0.1f)
    {
        if (lineRenderer == null || whipOrigin == null)
            yield break;

        lineRenderer.SetPosition(0, whipOrigin.position);
        lineRenderer.SetPosition(1, target);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(duration);

        if (!isSwinging)
            lineRenderer.enabled = false;
    }

    void CreateCrosshair()
    {
        crosshair = new GameObject("Crosshair");
        var canvas = crosshair.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        crosshair.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        crosshair.AddComponent<GraphicRaycaster>();

        var dot = new GameObject("Dot");
        dot.transform.SetParent(crosshair.transform, false);
        var img = dot.AddComponent<Image>();
        img.color = new Color(0.4f, 0.26f, 0.13f);
        img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        img.type = Image.Type.Simple;
        img.preserveAspect = true;

        var rt = dot.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(12, 12);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }
}
