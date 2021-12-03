using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RubyController : MonoBehaviour
{
    public Text gameoverText;
    public Text ammoText;
    public Text scoreText;
    public Text progressText;
    public float speed = 4.0f;
    public float boostTimer;
    public bool boosting;
    public int maxHealth = 5;
    public int minHealth = 0;
    public int ammo;
    public int score;
    public int progress;
    public GameObject projectilePrefab;
    public GameObject hiteffectPrefab;
    public GameObject healtheffectPrefab;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip powerupSound;
    public AudioClip ammopickupSound;
    public int health { get { return currentHealth; } }
    int currentHealth;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    AudioSource audioSource;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        boostTimer = 0;

        boosting = false;

        audioSource = GetComponent<AudioSource>();

        gameoverText.text = "";

        ammo = 4;
        SetAmmoText();

        score = 0;
        SetScoreText();

        progressText.text = "";
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    
                    if (gameoverText.text == "Talk to Jambi to visit Stage 2!")
                    {
                        transform.position = new Vector2(26.4f, 0.0f);
                        gameoverText.text = "";
                    }
                }
                CatController cat = hit.collider.GetComponent<CatController>();
                if (cat != null)
                {
                    cat.DisplayDialog();
                    
                    progress = 0;
                    SetProgressText();
                }
            }
        }

        if (boosting)
        {
            boostTimer += Time.deltaTime;
            if (boostTimer >= 4)
            {
                speed = 4.0f;
                boostTimer = 0;
                boosting = false;
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            score = score + 2;
            SetScoreText();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameOver();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hiteffectObject = Instantiate(hiteffectPrefab, rigidbody2d.position + Vector2.up * .75f, Quaternion.identity);

            PlaySound(hitSound);

            animator.SetTrigger("Hit");
        }

        if (health == minHealth)
        {
            GameOver();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        if (ammo > 0 && health > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            ammo = ammo - 1;
            SetAmmoText();

            PlaySound(throwSound);
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "HealthCollectable")
        {
            if (health < maxHealth)
            {
                GameObject healtheffectObject = Instantiate(healtheffectPrefab, rigidbody2d.position + Vector2.up * 1f, Quaternion.identity);
            }
        }

        if (collider.tag == "AmmoPickUp")
        {
            ammo = ammo + 4;
            SetAmmoText();

            audioSource.clip = ammopickupSound;
            audioSource.Play();

            Destroy(collider.gameObject);
        }

        if (collider.tag == "Dog")
        {
            progress = progress + 1;
            SetProgressText();

            Destroy(collider.gameObject);
        }

        if (collider.tag == "Power-Up")
        {
            boosting = true;
            speed = 8.0f;

            audioSource.clip = powerupSound;
            audioSource.Play();

            Destroy(collider.gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void SetAmmoText()
    {
        ammoText.text = "Cogs: " + ammo.ToString();
    }

    void SetScoreText()
    {
        scoreText.text = "Robots Fixed: " + score.ToString();

        if (score == 2)
        {
            gameoverText.text = "Talk to Jambi to visit Stage 2!";

            audioSource.clip = winSound;
            audioSource.Play();
            audioSource.loop = false;
        }

        if (score >= 6 && progress >= 3)
        {
            gameoverText.text = "You Win! - A Game by Alejandro Morales Press ESC to Quit";

            speed = 0;

            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;

            audioSource.clip = winSound;
            audioSource.Play();
            audioSource.loop = false;
        }
    }

    void SetProgressText()
    {
        progressText.text = "Dogs Helped: " + progress.ToString();
    }
    
    void GameOver()
    {
        gameoverText.text = "You Lose! - Press ESC to Quit";
            
        Destroy(gameObject.GetComponent<Collider>());
        isInvincible = true;
        invincibleTimer = 9999;

        speed = 0;

        rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;

        audioSource.clip = loseSound;
        audioSource.Play();
        audioSource.loop = false;
    }
}