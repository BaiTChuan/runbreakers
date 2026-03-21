using Unity.VisualScripting;
using UnityEngine;

public class playerControl : MonoBehaviour, IDamage, IBuff
{
    // Serialized Fields, displayed in Unity explorer, easy to change the attributes
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats ------")]
    [Range(1, 10)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] float speed;
    [Range(2, 6)][SerializeField] int sprintMod;

    [Header("----- Weapons ------")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float bulletLifetime;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;
    [SerializeField] ParticleSystem muzzleFlashEffect;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    [Header("---- XP ----")]
    [SerializeField] int currentXP;
    [SerializeField] int currentLevel = 1;
    [SerializeField] int maxXP = 100;

    int hpOriginal;
    float speedOriginal;
    float shootRateOriginal;

    float shootTimer;

    float speedTimer;
    float speedDuration;

    float speedDownTimer;
    float speedDownDuration;

    bool speedBuffed;
    bool speedDebuffed;
    bool damageBuffed;

    public int damageBuff;
    float damageBuffTimer;
    float damageBuffDuration;

    Vector3 playerVel;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Keep track of orginal hp
        hpOriginal = hp;
        speedOriginal = speed;
        shootRateOriginal = shootRate;
        speedBuffed = false;
        speedDebuffed = false;
        damageBuffed = false;
        cam = Camera.main;
        updatePlayerUI();
        updateBuffUI();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        AimGunToMouse();
    }

    void shoot()
    {
        if (muzzleFlashEffect != null)
        {
            muzzleFlashEffect.Play();
        }

        shootTimer = 0f;

        GameObject spawnedBullet = Instantiate(bullet, shootPos.position, shootPos.rotation);

        damage bulletScript = spawnedBullet.GetComponent<damage>();

        Destroy(spawnedBullet, bulletLifetime);

        if (bulletScript != null)
        {
            Vector3 bulletDir = gunPivot.right;
            bulletScript.SetDirection(bulletDir);
        }
    }

    void AimGunToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, gunPivot.position);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Vector3 dir = mouseWorldPos - gunPivot.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                gunPivot.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0);
            }
        }
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (speedBuffed == true)
        {
            speedTimer += Time.deltaTime;
            updateBuffUI();
        }
        if (speedDebuffed == true)
        {
            speedDownTimer += Time.deltaTime;
            updateBuffUI();
        }
        if (damageBuffed == true)
        {
            damageBuffTimer += Time.deltaTime;
            updateBuffUI();
        }

        playerVel.y = -10;

        float h = -Input.GetAxis("Horizontal");
        float v = -Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(h, 0f, v).normalized;

        controller.Move(moveDir * speed * Time.deltaTime);
        controller.Move(playerVel * Time.deltaTime);

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }

        if (speedTimer >= speedDuration && speedBuffed == true)
        {
            speed = speedOriginal;
            speedBuffed = false;
        }

        if (speedDownTimer >= speedDownDuration && speedDebuffed == true)
        {
            speed = speedOriginal;
            speedDebuffed = false;
        }

        if (damageBuffTimer >= damageBuffDuration && damageBuffed == true)
        {
            damageBuff = 0;
            shootRate = shootRateOriginal;
            damageBuffed = false;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        updatePlayerUI();

        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        if (hp <= 0)
        {
            Gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        Gamemanager.instance.playerHPBar.fillAmount = (float) hp / hpOriginal;
    }

    public void updateBuffUI()
    {
        if (speedBuffed == false)
        {
            Gamemanager.instance.speedBuffBar.fillAmount = 0;
        }
        else
        {
            Gamemanager.instance.speedBuffBar.fillAmount = (speedDuration - speedTimer) / speedDuration;
        }

        if (speedDebuffed == false)
        {
            Gamemanager.instance.speedDebuffBar.fillAmount = 0;
        }
        else
        {
            Gamemanager.instance.speedDebuffBar.fillAmount = (speedDownDuration - speedDownTimer) / speedDownDuration;
        }

        if (damageBuffed == false)
        {
            Gamemanager.instance.damageBuffBar.fillAmount = 0;
        }
        else
        {
            Gamemanager.instance.damageBuffBar.fillAmount = (damageBuffDuration - damageBuffTimer) / damageBuffDuration;
        }
    }

    public void getBuff(buffStats buff)
    {
        if (buff.id == 0)
        {
            hp += buff.healAmount;
        }
        if (buff.id == 1)
        {
            speed *= buff.speedMultiplier;
            speedDuration = buff.speedDuration;
            speedTimer = 0f;
            speedBuffed = true;
        }
        if (buff.id == 2)
        {
            damageBuff = buff.damageBuff;
            shootRate *= buff.fireRateMultiplier;
            damageBuffDuration = buff.damageBuffDuration;
            damageBuffTimer = 0f;
            damageBuffed = true;
        }
        if (buff.id == 3)
        {
            speed *= buff.speedDownMultiplier;
            speedDownDuration = buff.speedDownDuration;
            speedDownTimer = 0f;
            speedDebuffed = true;
        }
        updatePlayerUI();
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log("Player gained " + amount + " XP. Total XP: " + currentXP);

        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= maxXP)
        {
            currentXP -= maxXP; // carry over extra XP
            currentLevel++;

            Debug.Log("Player Leveled up! New Level: " + currentLevel);

            IncreaseXPThreshold();
        }
    }

    void IncreaseXPThreshold()
    {
        maxXP = Mathf.RoundToInt(maxXP * 1.2f);

        Debug.Log("Next level requires " + maxXP + " XP");
    }
}
