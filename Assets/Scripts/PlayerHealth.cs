using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerController))]

public class PlayerHealth : MonoBehaviour
{
    // Manages player defenses and collision actions
    // ---------------------------------------------

    [SerializeField] GameObject spriteHandle;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] GameObject bubbleHandle;
    [SerializeField] BubbleAction bubbleAction;
    private PlayerController playerController;
    private readonly int baseHealth = 1;
    private int health;
    private bool isAlive = true;
    private bool invincible = false;

    [SerializeField] GameObject boxParticle;
    [SerializeField] GameObject sawBloodParticle;
    [SerializeField] GameObject mineBloodParticle;
    [SerializeField] GameObject explosionParticle;
    [SerializeField] ParticleSystem windTrailParticle;
    [SerializeField] DigitParticleSpawner digitParticle;

    [SerializeField] PlayerSFX playerSFX;

    private int jumpCombo = 0;
    private float totalBonus = 0f;
    public float baseBoxBonus = 0.5f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Check if player lost all defenses
        if (isAlive && health <= 0)
        {
            Debug.Log("Awarded +$" + totalBonus);
            float money = PlayerPrefs.GetFloat("Money");
            money += totalBonus;
            PlayerPrefs.SetFloat("Money", money);

            Messenger.Broadcast(GameEvent.PLAYER_DIED);
            Messenger<float>.Broadcast(GameEvent.UI_UPDATE_BONUS, totalBonus);

            StopWindTrailParticles();
            playerSFX.StopRunSFX();

            playerSprite.enabled = false;

            isAlive = false;
        }
    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_HEALTH, SetHealth);
        Messenger.AddListener(GameEvent.START_RUN, StartWindTrailParticles);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_HEALTH, SetHealth);
        Messenger.RemoveListener(GameEvent.START_RUN, StartWindTrailParticles);
    }

    void PlayerHit()
    {
        health--;
        if (health > 0) StartCoroutine(ShieldInvincibility(0.1f, 8)); // = 1.6 seconds
        Messenger.Broadcast(GameEvent.UI_DECREMENT_BUBBLE);
    }

    // Set defense quantity (1 hit plus Bubble Shield layers), -- from GameManager
    void SetHealth(int defense)
    {
        health = baseHealth + defense;

        if (health > 1)
        {
            EnableBubbleShield();
            bubbleAction.RestartSpin();
        }
    }

    void StartWindTrailParticles()
    {
        var mainModule = windTrailParticle.main;
        mainModule.maxParticles = 1000;
    }

    void StopWindTrailParticles()
    {
        var mainModule = windTrailParticle.main;
        mainModule.maxParticles = 0;
    }

    void EnableBubbleShield()
    {
        bubbleHandle.SetActive(true);
    }

    public void PlayerLanded()
    {
        Debug.Log("Landing combo: +$" + baseBoxBonus * jumpCombo * (jumpCombo + 1));
        totalBonus += baseBoxBonus * jumpCombo * (jumpCombo + 1);
        jumpCombo = 0;
        digitParticle.ResetCombo();
    }

    public void ShrinkBubble()
    {
        Vector3 scale = bubbleHandle.transform.localScale;
        scale.x = 0.5f;
        scale.y = 0.5f;
        bubbleHandle.transform.localScale = scale;
    }

    public void RestoreBubble()
    {
        Vector3 scale = bubbleHandle.transform.localScale;
        scale.x = 1f;
        scale.y = 1f;
        bubbleHandle.transform.localScale = scale;
    }

    // Decide what to do when colliding with an obstacle
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive)
        {
            // If we collide with a Box..
            if (collision.gameObject.CompareTag("Box"))
            {
                BoxCollider2D box = collision.GetComponent<BoxCollider2D>();
                Vector2 playerPos = new(transform.position.x, transform.position.y);

                // Since the sprite handle position on approach is always some distance behind the rigidbody
                // (it's being lerped for smooth framerate), its position serves as a check in cases when
                // the rigidbody is detected after passing through a collider by continuous collision detection
                if (box != null && spriteHandle.transform.position.y > box.bounds.max.y)
                {
                    float baseHeight = collision.GetComponent<BoxCollider2D>().bounds.max.y;
                    playerController.ApplyJump(baseHeight, playerController.jumpForce / 2);

                    Instantiate(boxParticle, new Vector3(collision.ClosestPoint(playerPos).x, collision.ClosestPoint(playerPos).y, 0f), boxParticle.transform.rotation);
                    playerSFX.PlayBoxBreakSFX();

                    // Player is above box, destroy box
                    Destroy(collision.gameObject);
                    jumpCombo++;
                    digitParticle.OnBounceComboIncrement();
                }
                else if (!invincible)
                {
                    // Player hit box from side or below, lose defense
                    PlayerHit();

                    Instantiate(boxParticle, new Vector3(collision.ClosestPoint(playerPos).x, collision.ClosestPoint(playerPos).y, 0f), boxParticle.transform.rotation);
                    Instantiate(explosionParticle, collision.transform.position, explosionParticle.transform.rotation);
                    playerSFX.PlayBoxBreakSFX();
                    playerSFX.PlayMineExploSFX();

                    if (health < 1)
                    {
                        Instantiate(mineBloodParticle, new Vector3(collision.ClosestPoint(playerPos).x, transform.position.y + 0.8f, 0f), mineBloodParticle.transform.rotation);
                    }

                    Destroy(collision.gameObject);
                }

                return;
            }

            // If we collide with a Saw..
            if (collision.gameObject.CompareTag("Saw") && !invincible)
            {
                // Player sliced up by Saw, lose defense
                PlayerHit();

                Vector2 playerPos = new(transform.position.x, transform.position.y);
                if (health < 1)
                {
                    Instantiate(sawBloodParticle, new Vector3(collision.ClosestPoint(playerPos).x, collision.ClosestPoint(playerPos).y, 0f), sawBloodParticle.transform.rotation);
                    playerSFX.PlaySawSliceSFX();
                }

                // Destroy(collision.gameObject);
                if (collision.TryGetComponent<CircleCollider2D>(out var collider))
                {
                    collider.enabled = false;
                }

                return;
            }

            // If we collide with a Mine's proximity detector..
            if (collision.gameObject.CompareTag("Mine") && !invincible)
            {
                // Player blown up by Mine, lose defense
                PlayerHit();
                Instantiate(explosionParticle, collision.transform.position, explosionParticle.transform.rotation);
                playerSFX.PlayMineExploSFX();

                Vector2 playerPos = new(transform.position.x, transform.position.y);
                if (health < 1)
                {
                    Instantiate(mineBloodParticle, new Vector3(collision.ClosestPoint(playerPos).x, transform.position.y + 0.8f, 0f), mineBloodParticle.transform.rotation);
                }

                Destroy(collision.gameObject);
                return;
            }

            // If we are hit by a Turret laser..
            if (collision.gameObject.CompareTag("Turret") && !invincible)
            {
                // Player fried by Turret laser, lose defense
                PlayerHit();

                Vector2 playerPos = new(transform.position.x, transform.position.y);
                if (health < 1)
                {
                    Instantiate(mineBloodParticle, new Vector3(collision.ClosestPoint(playerPos).x, transform.position.y + 0.8f, 0f), mineBloodParticle.transform.rotation);
                }

                if (collision.TryGetComponent<BoxCollider2D>(out var collider))
                {
                    collider.enabled = false;
                }
            }
        }
    }

    private IEnumerator ShieldInvincibility(float interval, int count)
    {
        invincible = true;
        playerSFX.PlayShieldHitSFX();

        bubbleAction.isBroken = true;
        StartCoroutine(bubbleAction.FlickerRoutine());

        for (int i = 0; i < count; i++)
        {
            playerSprite.enabled = false;
            yield return new WaitForSeconds(interval);
            playerSprite.enabled = true;
            yield return new WaitForSeconds(interval);
        }

        invincible = false;
        playerSFX.PlayShieldPopSFX();

        bubbleAction.isBroken = false;

        if (health > 1) bubbleAction.RestartSpin(); else bubbleHandle.SetActive(false);
    }
}
