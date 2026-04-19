using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerControl : MonoBehaviour, IDamage, IPickup
{
    // Serialized Fields, displayed in Unity explorer, easy to change the attributes
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats ------")]
    [Range(1, 30)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] float speed;
    [Range(2, 6)][SerializeField] int sprintMod;

    [Header("----- LevelUp Stats ------")]
    [Range(1, 10)][SerializeField] int hpStatIncrease;
    [Range(1, 10)][SerializeField] float speedStatIncrease;
    [Range(1, 10)][SerializeField] int damageStatIncrease;

    [Header("----- Weapons ------")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    List<int> gunAmmoList = new List<int>();
    [SerializeField] GameObject gunModel;
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
    [SerializeField] float levelUpXPRequirementIncrease;
    [SerializeField] int maxXP = 100;

    [Header("---- Audios ----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] float stepVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float hurtVol;
    [SerializeField] AudioClip[] audSwap;
    [SerializeField] float swapVol;


    public int ammoCur;
    public int ammoMax;
    int gunListPos;

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
    public int damageOriginal;
    float damageBuffTimer;
    float damageBuffDuration;

    bool isPlayingStep;
    bool isSprinting;

    Vector3 playerVel;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Keep track of orginal hp
        hpOriginal = hp;
        speedOriginal = speed;
        shootRateOriginal = shootRate;
        damageOriginal = 0;
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
        removeWeapon();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], stepVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isPlayingStep = false;
    }

    void shoot()
    {
        if (muzzleFlashEffect != null)
        {
            muzzleFlashEffect.Play();
        }

        shootTimer = 0f;

        GameObject spawnedBullet = Instantiate(bullet, shootPos.position, shootPos.rotation);
        aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        damage bulletScript = spawnedBullet.GetComponent<damage>();

        Destroy(spawnedBullet, bulletLifetime);

        ammoCur--;

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
            if (ammoCur > 0)
            {
                shoot();
            }
        }

        selectGun();

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

        if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            StartCoroutine(playStep());
    }

    void sprint()
    {
        if (currentLevel >= 3)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                speed *= sprintMod;
                isSprinting = true;
            }
            else if (Input.GetButtonUp("Sprint"))
            {
                speed /= sprintMod;
                isSprinting = false;
            }
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

        StartCoroutine(flashDamage());
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], hurtVol);

        if (hp <= 0)
        {
            Gamemanager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.damagePlayerFlash == null)
            yield break;

        Gamemanager.instance.damagePlayerFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Gamemanager.instance.damagePlayerFlash.SetActive(false);
    }

    public void updatePlayerUI()
    {
        Gamemanager.instance.playerHPBar.fillAmount = (float) hp / hpOriginal;
        Gamemanager.instance.playerXPBar.fillAmount = (float) currentXP / maxXP;
        Gamemanager.instance.updateHpText(hp);
    }

    public void hpLevelUp()
    {
        hpOriginal += hpStatIncrease;
        hp = hpOriginal;
    }

    public void damageLevelUp()
    {
        damageOriginal += damageStatIncrease;
    }

    public void speedLevelUp()
    {
        speedOriginal += speedStatIncrease;
        speed = speedOriginal;
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
            if (hp < hpOriginal)
            {
                if ((hp + buff.healAmount) > hpOriginal)
                {
                    hp = hpOriginal;
                }
                else
                {
                    hp += buff.healAmount;
                }
            }
        }
        if (buff.id == 1)
        {
            if (!speedDebuffed)
            {
                if (speedBuffed)
                {
                    speed = speedOriginal;
                    speed *= buff.speedMultiplier;
                    speedDuration = buff.speedDuration;
                    speedTimer = 0f;
                }
                else
                {
                    speed *= buff.speedMultiplier;
                    speedDuration = buff.speedDuration;
                    speedTimer = 0f;
                    speedBuffed = true;
                }
            }
            else
            {
                speed = speedOriginal;
                speedDebuffed = false;
                speed *= buff.speedMultiplier;
                speedDuration = buff.speedDuration;
                speedTimer = 0f;
                speedBuffed = true;

            }
        }
        if (buff.id == 2)
        {
            if (!damageBuffed)
            {
                damageBuff = buff.damageBuff;
                shootRate *= buff.fireRateMultiplier;
                damageBuffDuration = buff.damageBuffDuration;
                damageBuffTimer = 0f;
                damageBuffed = true;
            }
            else
            {
                damageBuff = 0;
                shootRate = shootRateOriginal;
                damageBuff = buff.damageBuff;
                shootRate *= buff.fireRateMultiplier;
                damageBuffDuration = buff.damageBuffDuration;
                damageBuffTimer = 0f;
            }
        }
        if (buff.id == 3)
        {
            if (!speedBuffed)
            {
                if (speedDebuffed)
                {
                    speed = speedOriginal;
                    speed *= buff.speedDownMultiplier;
                    speedDownDuration = buff.speedDownDuration;
                    speedDownTimer = 0f;
                }
                else
                {
                    speed *= buff.speedDownMultiplier;
                    speedDownDuration = buff.speedDownDuration;
                    speedDownTimer = 0f;
                    speedDebuffed = true;
                }
            }
            else
            {
                speed = speedOriginal;
                speedBuffed = false;
                speed *= buff.speedDownMultiplier;
                speedDownDuration = buff.speedDownDuration;
                speedDownTimer = 0f;
                speedDebuffed = true;
            }
        }
        updatePlayerUI();
    }

    public void getGold(int amount)
    {
        Gamemanager.instance.AddGold(amount);

       // GoldUI.instance.UpdateGold(Gamemanager.instance.gold);
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
        updatePlayerUI();
    }

    void CheckLevelUp()
    {
        while (currentXP >= maxXP)
        {
            currentXP -= maxXP; // carry over extra XP
            currentLevel++;
            Gamemanager.instance.LevelUp();

            Debug.Log("Player Leveled up! New Level: " + currentLevel);

            IncreaseXPThreshold();
        }
    }

    void IncreaseXPThreshold()
    {
        maxXP = Mathf.RoundToInt(maxXP * levelUpXPRequirementIncrease);

        Debug.Log("Next level requires " + maxXP + " XP");
    }

    public void getGun(gunStats gun)
    {
        if (gunList.Count < 3)
        {
            gunList.Add(gun);
            gunAmmoList.Add(gun.ammoMax);
            gunListPos = gunList.Count - 1;
            changeGun();
        }
        else if (gunList.Count >= 3)
        {
            if (gunListPos == 0)
            {
                gunList.RemoveAt(1);
                gunAmmoList.RemoveAt(1);
                gunList.Add(gun);
                gunAmmoList.Add(gun.ammoMax);
                gunListPos = gunList.Count - 1;
                changeGun();
            }
            else
            {
                gunList.RemoveAt(gunListPos);
                gunAmmoList.RemoveAt(gunListPos);
                gunList.Add(gun);
                gunAmmoList.Add(gun.ammoMax);
                gunListPos = gunList.Count - 1;
                changeGun();
            }
        }
    }

    void changeGun()
    {
        bullet = gunList[gunListPos].bullet;
        shootRate = gunList[gunListPos].shootRate;
        bulletLifetime = gunList[gunListPos].bulletLifeTime;
        ammoCur = gunAmmoList[gunListPos];
        ammoMax = gunList[gunListPos].ammoMax;

        shootRateOriginal = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunAmmoList[gunListPos] = ammoCur;
            gunListPos++;
            changeGun();
            aud.PlayOneShot(audSwap[Random.Range(0, audSwap.Length)], swapVol);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunAmmoList[gunListPos] = ammoCur;
            gunListPos--;
            changeGun();
            aud.PlayOneShot(audSwap[Random.Range(0, audSwap.Length)], swapVol);
        }
    }

    void removeWeapon() {
        if (gunList.Count > 1 && ammoCur <= 0)
        {
            gunList.RemoveAt(gunListPos);
            gunAmmoList.RemoveAt(gunListPos);

            if (gunListPos >= gunList.Count)
            {
                gunListPos = gunList.Count - 1;
            }
            changeGun();
        }
    }
}
