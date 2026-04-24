using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static MainMenu;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 5f;
    public float thrustForce = 2f;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;
    Rigidbody2D rb;
    public UIDocument uiDocument;
    private Label scoreText;
    private Button restartButton;
    public GameObject explosionEffect; 
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 0.25f;
    private float lastFireTime = 0f;
    private int bonusScore = 0;
    public int asteroidsDestroyed = 0;
    public static bool IsPlayerAlive = true; 
    public GameObject borderParent;
    private Label PRLabel;
    private Label highScoreText;
    private Label shopText;
    private Label item1;
    private Label item2;
    private Label item3;
    private Label item4;
    private Button pauseButton;
    private Button optionsButton;
    private bool isPaused = false;
    private Button backButton;
    private Button muteButton;
    private bool isMuted = false;
    public UIDocument GameUI;
    public UIDocument PauseMenuUI;
    public UIDocument ShopUI;

    private Button shopButton;
    private Button shopBackButton;

    private Button p1Button;
    private Button p2Button;
    private VisualElement shopBackground;
    public GameObject mrSPrefab;
    public GameObject mrBPrefab;
    public GameObject shieldVFXPrefab;
    public GameObject shieldHitEffectPrefab;
    private GameObject activeShieldVFX;
    private ParticleSystem shieldParticles;

    public bool shieldActive = false;
    public float shieldCooldown = 10f;
    private VisualElement shopContainer; 
    private VisualElement gameUIContainer; 
    private VisualElement pausemenuContainer;

    private Button p3Button;
    private Button p4Button;

    private int atkSpeedLevel = 0;
    private int maxAtkSpeedLevel = 5;

    private bool hasDash = false;
    private float dashCooldown = 1f;
    private float dashForce = 10f;
    private bool canDash = true;

    private float timeScore = 0f;
    private int spentScore = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        var gameRoot = GameUI.rootVisualElement;
        var pauseRoot = PauseMenuUI.rootVisualElement;
        var shopRoot = ShopUI.rootVisualElement;
        // var root = uiDocument.rootVisualElement;

        // shopContainer = shopRoot.Q<VisualElement>("ShopContainer");
        // gameUIContainer = gameRoot.Q<VisualElement>("GameUIContainer");
        // pausemenuContainer = pauseRoot.Q<VisualElement>("PauseMenuContainer");

        scoreText = gameRoot.Q<Label>("ScoreLabel");
        highScoreText = gameRoot.Q<Label>("HighScoreLabel");
        pauseButton = gameRoot.Q<Button>("PauseButton");

        backButton = pauseRoot.Q<Button>("BackButton");
        muteButton = pauseRoot.Q<Button>("MuteButton");
        optionsButton = pauseRoot.Q<Button>("OptionsButton");
        shopButton = pauseRoot.Q<Button>("ShopButton");
        restartButton = pauseRoot.Q<Button>("RestartButton");

        shopBackButton = shopRoot.Q<Button>("ShopBackButton");
        shopBackground = shopRoot.Q<VisualElement>("ShopBackground");
        shopText = shopRoot.Q<Label>("ShopText");
        item1 = shopRoot.Q<Label>("Item1");
        item2 = shopRoot.Q<Label>("Item2");
        item3 = shopRoot.Q<Label>("Item3");
        item4 = shopRoot.Q<Label>("Item4");
        p1Button = shopRoot.Q<Button>("Purchase1Button");
        p2Button = shopRoot.Q<Button>("Purchase2Button");
        p3Button = shopRoot.Q<Button>("Purchase3Button");
        p4Button = shopRoot.Q<Button>("Purchase4Button");
        
        // Shop Buttons
        shopBackButton.style.display = DisplayStyle.None;
        shopBackground.style.display = DisplayStyle.None;
        shopText.style.display = DisplayStyle.None;
        item1.style.display = DisplayStyle.None;
        item2.style.display = DisplayStyle.None;
        item3.style.display = DisplayStyle.None;
        item4.style.display = DisplayStyle.None;
        p1Button.style.display = DisplayStyle.None;
        p2Button.style.display = DisplayStyle.None;
        p3Button.style.display = DisplayStyle.None;
        p4Button.style.display = DisplayStyle.None;
        
        // PauseMenu Buttons
        shopButton.style.display = DisplayStyle.None;
        optionsButton.style.display = DisplayStyle.None;
        muteButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        backButton.style.display = DisplayStyle.None;

        // Actions for buttons
        optionsButton.clicked += OpenOptions;
        restartButton.clicked += ReloadScene;
        pauseButton.clicked += TogglePause;
        backButton.clicked += CloseOptions;
        muteButton.clicked += ToggleAudio;
        shopButton.clicked += OpenShop;
        shopBackButton.clicked += CloseShop;
        p1Button.clicked += () => BuyMrS();
        p2Button.clicked += () => BuyMrB();
        p3Button.clicked += () => BuyAtkSpeed();
        p4Button.clicked += () => BuyDash();

        IsPlayerAlive = true;
        
        PlayerPrefs.SetInt("AtkSpeedLevel", 0);

        float highScore = PlayerPrefs.GetFloat("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;

        if (shieldVFXPrefab != null)
        {
            activeShieldVFX = Instantiate(shieldVFXPrefab, transform.position, Quaternion.identity, transform);
            shieldParticles = activeShieldVFX.GetComponent<ParticleSystem>();

            // Start OFF until purchased
            activeShieldVFX.SetActive(false);
        }
        atkSpeedLevel = PlayerPrefs.GetInt("AtkSpeedLevel", 0);
        
        // Reapply attack speed upgrades
        for (int i = 0; i < atkSpeedLevel; i++)
        {
            fireCooldown *= 0.8f;
        }

        if (atkSpeedLevel > 0)
        {
            p3Button.text = "Atk Speed Lv " + atkSpeedLevel;
        }

        if (atkSpeedLevel >= maxAtkSpeedLevel)
        {
            p3Button.text = "Maxed!";
            p3Button.SetEnabled(false);
        }
        hasDash = false;
        canDash = true;

        p4Button.text = "Unlock: 1000";
        p4Button.SetEnabled(true);

        if (hasDash)
        {
            p4Button.text = "Dash Unlocked!";
            p4Button.SetEnabled(false);
        }

    }
    void Update()
    {
    if (!IsPlayerAlive || isPaused) return; // Check if player alive
        UpdateScore();
        MovePlayer();

    if (Keyboard.current.spaceKey.isPressed && Time.time > lastFireTime + fireCooldown)
    {
        Shoot();
        lastFireTime = Time.time;
    }
    if (hasDash && Keyboard.current.leftShiftKey.wasPressedThisFrame && canDash)
    {
    StartCoroutine(Dash());
    }
    }

    void UpdateScore()
    {
        elapsedTime += Time.deltaTime;

        timeScore = Mathf.FloorToInt(elapsedTime * scoreMultiplier);

        score = timeScore + bonusScore - spentScore;

        scoreText.text = "Score: " + score;

        float highScore = PlayerPrefs.GetFloat("HighScore", 0);

        if (score > highScore)
        {
            PlayerPrefs.SetFloat("HighScore", score);
            PlayerPrefs.Save();
            highScoreText.text = "NEW HIGH SCORE!!!: " + Mathf.FloorToInt(score);
        }
    }
    public void AddScore(int amount)
    {
        bonusScore += amount;
        asteroidsDestroyed++;
    }

    void MovePlayer()
    {
        
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }

        // Velocity 
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
        rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Animation for booster
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }

        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (!IsPlayerAlive) return;

        // Shield logic
        if (shieldActive)
        {
            if (collision.gameObject.CompareTag("Asteroid"))
            {
                Destroy(collision.gameObject); // absorb hit
                BreakShield(); // trigger cooldown + VFX change
                Vector2 contactPoint = collision.GetContact(0).point;

                Instantiate(shieldHitEffectPrefab, contactPoint, Quaternion.identity);
            }
            return; // prevent death
        }

        // Player death
        pauseButton.style.display = DisplayStyle.None;

        IsPlayerAlive = false;

        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score", 0) + Mathf.FloorToInt(score));
        PlayerPrefs.Save();

        SaveHighScore();
        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        borderParent.SetActive(false);
        gameObject.SetActive(false);



    }
    void Shoot()
    {
        Vector2 direction = transform.up;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, transform.rotation);

        projectile.GetComponent<Projectile>().Fire(direction);
        // rb.AddForce(-transform.up * 0.2f, ForceMode2D.Impulse);  Added force from projectile to player still testing satisfaction 
    }
    void SaveHighScore()
    {
    float currentHighScore = PlayerPrefs.GetFloat("HighScore", 0);

    if (score > currentHighScore)
    {
        PlayerPrefs.SetFloat("HighScore", score);
        PlayerPrefs.Save();
    }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

            pauseButton.text = "Unpause";
            shopButton.style.display = DisplayStyle.Flex;
            optionsButton.style.display = DisplayStyle.Flex;
            restartButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1f;

            pauseButton.text = "Pause";
            shopButton.style.display = DisplayStyle.None;
            optionsButton.style.display = DisplayStyle.None;
            restartButton.style.display = DisplayStyle.None;
        }
    }
    void OpenOptions()
    {
        // Hide main menu buttons
        optionsButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        pauseButton.style.display = DisplayStyle.None;
        shopButton.style.display = DisplayStyle.None;

        // Show options buttons
        backButton.style.display = DisplayStyle.Flex;
        muteButton.style.display = DisplayStyle.Flex;
    }

    void CloseOptions()
    {
        // Show mute button
        optionsButton.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;
        pauseButton.style.display = DisplayStyle.Flex;
        shopButton.style.display = DisplayStyle.Flex;
        

        // Hide options buttons
        backButton.style.display = DisplayStyle.None;
        muteButton.style.display = DisplayStyle.None;
    }

    void ToggleAudio()
    {
        isMuted = !isMuted;

        AudioListener.volume = isMuted ? 0f : 1f;
        muteButton.text = isMuted ? "SFX: Unmute" : "SFX: Mute";
    }
    void OpenShop()
    {
        Time.timeScale = 0f;

        // Hides Game Buttons
        // scoreText.style.display = DisplayStyle.None;
        highScoreText.style.display = DisplayStyle.None;
        pauseButton.style.display = DisplayStyle.None;
        // Hide Menu Buttons
        shopButton.style.display = DisplayStyle.None;
        optionsButton.style.display = DisplayStyle.None;
        muteButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        backButton.style.display = DisplayStyle.None;

        // Open Shop Buttons
        shopBackButton.style.display = DisplayStyle.Flex;
        p1Button.style.display = DisplayStyle.Flex;
        p2Button.style.display = DisplayStyle.Flex;
        p3Button.style.display = DisplayStyle.Flex;
        p4Button.style.display = DisplayStyle.Flex;
        shopBackground.style.display = DisplayStyle.Flex;
        shopText.style.display = DisplayStyle.Flex;
        item1.style.display = DisplayStyle.Flex;
        item2.style.display = DisplayStyle.Flex;
        item3.style.display = DisplayStyle.Flex;
        item4.style.display = DisplayStyle.Flex;
//        gameUIContainer.style.display = DisplayStyle.None;
//        shopContainer.style.display = DisplayStyle.Flex;
    }

    void CloseShop()
    {
        
        // Open Game Buttons
        // scoreText.style.display = DisplayStyle.Flex;
        highScoreText.style.display = DisplayStyle.Flex;
        pauseButton.style.display = DisplayStyle.Flex;

        // Open Menu Buttons
        shopButton.style.display = DisplayStyle.Flex;
        optionsButton.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;

        // Hide Shop Buttons
        shopBackButton.style.display = DisplayStyle.None;
        p1Button.style.display = DisplayStyle.None;
        p2Button.style.display = DisplayStyle.None;
        p3Button.style.display = DisplayStyle.None;
        p4Button.style.display = DisplayStyle.None;
        shopBackground.style.display = DisplayStyle.None;
        shopText.style.display = DisplayStyle.None;
        item1.style.display = DisplayStyle.None;
        item2.style.display = DisplayStyle.None;
        item3.style.display = DisplayStyle.None;
        item4.style.display = DisplayStyle.None;
        UpdateScore();
//        shopContainer.style.display = DisplayStyle.None;
//        gameUIContainer.style.display = DisplayStyle.Flex;
    }

    void BuyMrS()
    {
        int cost = 1000;

        if (score < cost)
        {
            Debug.Log("Not enough score!");
            return;
        }
        score -= cost;
        PlayerPrefs.Save();
        spentScore += cost; // track spending       
        //  // Unlock
        PlayerPrefs.SetInt("HasMrS", 1);
        PlayerPrefs.Save();

        ActivateShield();
        
        Debug.Log("Purchased Mr.S!");
        p1Button.text = "Purchased Mr.S!";

        // Spawn immediately
        GameObject mrS = Instantiate(mrSPrefab, transform.position, Quaternion.identity);
        mrS.GetComponent<OrbitAroundPlayer>().player = transform;

        // Disable button after purchase
        p1Button.SetEnabled(false);
    }
    void BuyMrB()
    {
        int cost = 1000;

        if (score < cost)
        {
            Debug.Log("Score not high enough!");
            return;
        }

        score -= cost;
        spentScore += cost; // track spending
        PlayerPrefs.SetInt("HasMrB", 1);
        PlayerPrefs.Save();

        Debug.Log("Purchased Mr.B!");
        p2Button.text = "Purchased Mr.B!";

        GameObject mrB = Instantiate(mrBPrefab, transform.position, Quaternion.identity);
        mrB.GetComponent<OrbitAroundPlayer>().player = transform;

        p2Button.SetEnabled(false);
    }
    public void ActivateShield()
    {
        shieldActive = true;
        gameObject.layer = LayerMask.NameToLayer("Shield");
        if (activeShieldVFX != null)
            activeShieldVFX.SetActive(true);
    
        if (shieldParticles != null)
            shieldParticles.Play();
    }

    public void BreakShield()
    {
        if (!shieldActive) return;

        shieldActive = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        // Stop VFX
        if (shieldParticles != null)
            shieldParticles.Stop();
        else if (activeShieldVFX != null)
            activeShieldVFX.SetActive(false);

        // Cancel any existing cooldown (important!)
        CancelInvoke(nameof(ActivateShield));

        // Start cooldown
        Invoke(nameof(ActivateShield), shieldCooldown);
    }

    void BuyAtkSpeed()
    {
    int cost = 500;

    if (atkSpeedLevel >= maxAtkSpeedLevel)
    {
        Debug.Log("Max attack speed reached!");
        return;
    }

    if (score < cost)
    {
        Debug.Log("Not enough score!");
        return;
    }
    score -= cost;
    spentScore += cost; // track spending

    atkSpeedLevel++;

    // Reduce fire cooldown
    fireCooldown = Mathf.Max(0.05f, fireCooldown * 0.8f);

    PlayerPrefs.SetInt("AtkSpeedLevel", atkSpeedLevel);
    // PlayerPrefs.Save();

    p3Button.text = "Atk Speed Lv " + atkSpeedLevel;

    if (atkSpeedLevel >= maxAtkSpeedLevel)
    {
        p3Button.text = "Maxed!";
        p3Button.SetEnabled(false);
    }

    Debug.Log("Attack speed upgraded to level " + atkSpeedLevel);
    }


    void BuyDash()
    {
    int cost = 1000;

    if (hasDash)
    {
        Debug.Log("Dash already unlocked!");
        return;
    }

    if (score < cost)
    {
        Debug.Log("Not enough score!");
        return;
    }

    score -= cost;
    spentScore += cost; // track spending

    hasDash = true;

    p4Button.text = "Dash Unlocked!";
    p4Button.SetEnabled(false);

    Debug.Log("Dash ability unlocked!");
    }
    IEnumerator Dash()
    {
        canDash = false;

        Vector2 dashDirection = transform.up;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(dashCooldown);

        canDash = true;
    }
}
