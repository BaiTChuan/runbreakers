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
    [SerializeField] int characterAttackPower;
    [SerializeField] int characterArmor;
    [SerializeField] int characterLuck;
    [SerializeField] float characterCastSpeed;

    [Header("----- LevelUp Stats ------")]
    [Range(1, 10)][SerializeField] int hpStatIncrease;
    [Range(1, 10)][SerializeField] float speedStatIncrease;
    [Range(1, 10)][SerializeField] int damageStatIncrease;
    [Range(1, 10)][SerializeField] int hpTier2;
    [Range(1, 10)][SerializeField] int hpTier3;
    [Range(0, 10)][SerializeField] float speedTier2;
    [Range(0, 10)][SerializeField] float speedTier3;
    [Range(1, 10)][SerializeField] int damageTier2;
    [Range(1, 10)][SerializeField] int damageTier3;

    [Header("----- Spells ------")]
    [SerializeField] private Spell spellToCast;
    [SerializeField] private Transform castPivot;
    [SerializeField] private Transform castPos;

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

    float castTimer;

    int hpOriginal;
    float speedOriginal;

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

    void CastSpell()
    {
        castTimer = 0f;

        SpellDamageChangedBasedOnATK();
        Instantiate(spellToCast, castPos.position, castPos.rotation);
    }

    void SpellDamageChangedBasedOnATK()
    {
        damage dmgScript = spellToCast.GetComponent<damage>();
        dmgScript.ChangeDmgBasedOnStats(characterAttackPower);
    }

    void AimGunToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, castPivot.position);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Vector3 dir = mouseWorldPos - castPivot.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                castPivot.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0);
            }
        }
    }

    void movement()
    {
        castTimer += Time.deltaTime;

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

        if (Input.GetButton("Fire1") && castTimer >= (spellToCast.spellToCast.castSpeed * characterCastSpeed))
        {
            CastSpell();
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
        hp -= (amount * (1-characterArmor/100));
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

                damageBuffDuration = buff.damageBuffDuration;
                damageBuffTimer = 0f;
                damageBuffed = true;
            }
            else
            {
                damageBuff = 0;

                damageBuff = buff.damageBuff;

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

    #region LevelUpFunctionss

    public void hpLevelUp0()
    {
        hpOriginal += hpStatIncrease;
        hp = hpOriginal;
    }

    public void hpLevelUp1()
    {
        hpOriginal += hpStatIncrease + hpTier2;
        hp = hpOriginal;
    }

    public void hpLevelUp2()
    {
        hpOriginal += hpStatIncrease + hpTier3;
        hp = hpOriginal;
    }

    public void damageLevelUp0()
    {
        damageOriginal += damageStatIncrease;
    }

    public void damageLevelUp1()
    {
        damageOriginal += damageStatIncrease + damageTier2;
    }

    public void damageLevelUp2()
    {
        damageOriginal += damageStatIncrease + damageTier3;
    }
    public void speedLevelUp0()
    {
        speedOriginal += speedStatIncrease;
        speed = speedOriginal;
    }

    public void speedLevelUp1()
    {
        speedOriginal += speedStatIncrease + speedTier2;
        speed = speedOriginal;
    }

    public void speedLevelUp2()
    {
        speedOriginal += speedStatIncrease + speedTier3;
        speed = speedOriginal;
    }
    #endregion
}
