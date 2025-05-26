using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;

    private int count;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI effectStatusText;
    public GameObject winTextObject;

    public AudioSource collectSound;
    public AudioSource winSound;
    public AudioSource loseSound;
    public AudioSource backgroundMusic;
    public AudioSource hitSound;

    public GameObject winEffect;
    public GameObject collectibleEffect;

    public GameObject restartButton;

    private bool isInvincible = false;
    public GameObject enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            Destroy(Instantiate(collectibleEffect, other.transform.position, Quaternion.identity), 2.0f);

            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
            collectSound.Play();
        }
        if (other.CompareTag("Booster"))
        {
            string boosterType = other.gameObject.name;
            if (boosterType.Contains("Invincibility"))
                StartCoroutine(Invincibility());
            else if (boosterType.Contains("FreezeEnemy"))
                StartCoroutine(FreezeEnemy());

            other.gameObject.SetActive(false);
            collectSound.Play();
        }

        if (other.CompareTag("Debuff"))
        {
            string debuffType = other.gameObject.name;
            if (debuffType.Contains("Slow"))
                StartCoroutine(SlowDown());
            else if (debuffType.Contains("SpinCamera"))
                StartCoroutine(SpinCamera());

            other.gameObject.SetActive(false);
            collectSound.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isInvincible == false)
        {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            loseSound.Play();
            backgroundMusic.Stop();
            restartButton.SetActive(true);
        }

        hitSound.Play();
    }

    private void SetCountText()
    {
        if (count >= 11)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            winSound.Play();
            winEffect.SetActive(true);
            restartButton.SetActive(true);
        }
        countText.text = "Count: " + count.ToString();
    }

    IEnumerator ShowEffectStatus(string effectName, float duration)
    {
        effectStatusText.text = effectName + " (" + duration.ToString("0") + "s)";

        float remaining = duration;
        while (remaining > 0f)
        {
            effectStatusText.text = effectName + " (" + remaining.ToString("0") + "s)";
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        effectStatusText.text = "Effect: None";
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        StartCoroutine(ShowEffectStatus("Invincible", 3f));
        yield return new WaitForSeconds(3f);
        isInvincible = false;
    }

    IEnumerator FreezeEnemy()
    {
        StartCoroutine(ShowEffectStatus("Enemy Frozen", 3f));
        if (enemy != null)
        {
            var enemyScript = enemy.GetComponent<EnemyMovement>();
            if (enemyScript != null)
            {
                enemyScript.enabled = false;
                yield return new WaitForSeconds(3f);
                enemyScript.enabled = true;
            }
        }
    }

    IEnumerator SlowDown()
    {
        StartCoroutine(ShowEffectStatus("Player slowed", 3f));
        speed /= 2f;
        yield return new WaitForSeconds(3f);
        speed *= 2f;
    }

    IEnumerator SpinCamera()
    {
        StartCoroutine(ShowEffectStatus("Camera Spinning", 3f));
        CameraController camCtrl = Camera.main.GetComponent<CameraController>();
        if (camCtrl != null)
        {
            camCtrl.StartSpinning();
            yield return new WaitForSeconds(3f);
            camCtrl.StopSpinning();
        }
    }
}
