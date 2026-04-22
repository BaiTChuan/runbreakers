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
    [SerializeField] public int characterAttackPower;
    [SerializeField] int characterArmor;
    [SerializeField] int characterLuck;
    [SerializeField] float characterCastSpeed;
    [SerializeField] int revive;

    [Header("----- LevelUp Stats ------")]
    [Range(1, 10)][SerializeField] int hpStatIncrease;
    [Range(1, 10)][SerializeField] float speedStatIncrease;
    [Range(1, 10)][SerializeField] int damageStatIncrease;
    [Range(1, 10)][SerializeField] int armorStatIncrease;
    [Range(0, 1)][SerializeField] float castSpeedStatIncrease;
    [Range(1, 10)][SerializeField] int luckStatIncrease;

    [Range(1, 10)][SerializeField] int hpTier2;
    [Range(0, 10)][SerializeField] float speedTier2;
    [Range(1, 10)][SerializeField] int damageTier2;
    [Range(1, 10)][SerializeField] int armorStatTier2;
    [Range(0, 1)][SerializeField] float castSpeedStatTier2;
    [Range(1, 10)][SerializeField] int luckStatTier2;

    [Range(1, 10)][SerializeField] int hpTier3;
    [Range(0, 10)][SerializeField] float speedTier3;
    [Range(1, 10)][SerializeField] int damageTier3;
    [Range(1, 10)][SerializeField] int armorStatTier3;
    [Range(0, 1)][SerializeField] float castSpeedStatTier3;
    [Range(1, 10)][SerializeField] int luckStatTier3;


    [Header("----- Spells ------")]
    [SerializeField] private List<Player_Spell> spellPrefabs = new List<Player_Spell>();
    private List<Player_Spell> spells = new List<Player_Spell>();
    [SerializeField] private Transform castPivot;
    [SerializeField] private Transform castPos;
    private int currentSpellIndex = 0;

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

#region statTrackers

    int hpOriginal;
    int hpBase;

    float speedOriginal;
    float speedBase;

    public int damageOriginal;
    int damageBase;

    float castSpeedOriginal;
    float castSpeedBase;

    int armorOriginal;
    int armorBase;

    int luckOriginal;
    int luckBase;

    int reviveOriginal;
    int reviveBase;

    int rerollOriginal;
    int rerollBase;

#endregion

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

    bool isPlayingStep;
    bool isSprinting;

    Vector3 playerVel;

    private Camera cam;

    void Awake()
    {
        // Instantiate personal copies of spells for this gameplay session
        spells = new List<Player_Spell>();
        foreach (var spellPrefab in spellPrefabs)
        {
            Player_Spell newSpell = Instantiate(spellPrefab, transform);
            spells.Add(newSpell);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statTrack();

        speedBuffed = false;
        speedDebuffed = false;
        damageBuffed = false;

        cam = Camera.main;
        updatePlayerUI();
        updateBuffUI();
    }

    void statTrack()
    {
        hpBase = hp;
        hpOriginal = hp + mainMenuManager.healthP;
        hp = hpOriginal;

        speedBase = speed;
        speedOriginal = speed + mainMenuManager.speedP;
        speed = speedOriginal;

        damageBase = characterAttackPower;
        damageOriginal = characterAttackPower + mainMenuManager.damageP;
        characterAttackPower = damageOriginal;

        castSpeedBase = characterCastSpeed;
        castSpeedOriginal = characterCastSpeed - mainMenuManager.castSpeedP;
        characterCastSpeed = castSpeedOriginal;

        luckBase = characterLuck;
        luckOriginal = characterLuck + mainMenuManager.luckP;
        characterLuck = luckOriginal;

        armorBase = armorOriginal;
        armorOriginal = characterArmor + mainMenuManager.armorP;
        characterArmor = armorOriginal;

        reviveBase = revive;
        reviveOriginal = revive + mainMenuManager.reviveP;
        revive = reviveOriginal;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        AimGunToMouse();
        dataDeletedCheck();
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
        if (spells.Count == 0)
        {
            return;
        }

        castTimer = 0f;
        spells[currentSpellIndex].Cast(castPos, castPivot.forward);
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
                castPivot.rotation = Quaternion.LookRotation(dir);
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

        if (Input.GetButton("Fire1") && castTimer >= (spells[currentSpellIndex].CastSpeed * characterCastSpeed))
        {
            CastSpell();
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            if (spells.Count > 0 && spells[currentSpellIndex] != null)
            {
                spells[currentSpellIndex].AddXp(50);
                Debug.Log("Added 50 XP to " + spells[currentSpellIndex].name);
            }
        }

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput != 0)
        {
            if (scrollWheelInput > 0)
            {
                currentSpellIndex++;
                if (currentSpellIndex >= spells.Count)
                {
                    currentSpellIndex = 0;
                }
            }
            else
            {
                currentSpellIndex--;
                if (currentSpellIndex < 0)
                {
                    currentSpellIndex = spells.Count - 1;
                }
            }
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
            characterAttackPower = damageOriginal;
            characterCastSpeed = castSpeedOriginal;
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
            if (revive > 0)
            {
                revive--;
                hp = hpOriginal;
                updatePlayerUI();
            }
            else 
            {
                Gamemanager.instance.youLose();
            }
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
                characterAttackPower += 2;
                characterCastSpeed -= 0.3f;
            }
            damageBuffDuration = buff.damageBuffDuration;
            damageBuffTimer = 0f;
            damageBuffed = true;
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

    void dataDeletedCheck()
    {
        if (mainMenuManager.instance != null)
        {
            if (mainMenuManager.instance.dataDeleted == true)
            {
                hpOriginal = hpBase;
                hp = hpOriginal;
                speedOriginal = speedBase;
                speed = speedOriginal;
                damageOriginal = damageBase;
                characterAttackPower = damageOriginal;
                castSpeedOriginal = castSpeedBase;
            }
        }
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

    public void armorLevelUp0()
    {
        characterArmor += armorStatIncrease;
    }

    public void armorLevelUp1()
    {
        characterArmor += armorStatIncrease + armorStatTier2;
    }

    public void armorLevelUp2()
    {
        characterArmor += armorStatIncrease + armorStatTier3;
    }

    public void castSpeedLevelUp0()
    {
        castSpeedOriginal -= castSpeedStatIncrease;
        characterCastSpeed = castSpeedOriginal;
    }

    public void castSpeedLevelUp1()
    {
        castSpeedOriginal -= castSpeedStatIncrease + castSpeedStatTier2;
        characterCastSpeed = castSpeedOriginal;
    }

    public void castSpeedLevelUp2()
    {
        castSpeedOriginal -= castSpeedStatIncrease + castSpeedStatTier3;
        characterCastSpeed = castSpeedOriginal;
    }

    public void luckLevelUp0()
    {
        characterLuck += luckStatIncrease;
    }

    public void luckLevelUp1()
    {
        characterLuck += luckStatIncrease + luckStatTier2;
    }

    public void luckLevelUp2()
    {
        characterLuck += luckStatIncrease + luckStatTier3;
    }
    #endregion
}
