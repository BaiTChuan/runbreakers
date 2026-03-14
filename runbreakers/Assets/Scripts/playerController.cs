using Unity.VisualScripting;
using UnityEngine;

public class playerControl : MonoBehaviour, IDamage
{
    // Serialized Fields, displayed in Unity explorer, easy to change the attributes
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats ------")]
    [Range(1, 10)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 6)][SerializeField] int sprintMod;

    [Header("----- Weapons ------")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform gunPivot;
    [SerializeField] ParticleSystem muzzleFlashEffect;

    int hpOriginal;

    float shootTimer;

    Vector3 playerVel;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Keep track of orginal hp
        hpOriginal = hp;
        cam = Camera.main;
        updatePlayerUI();
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

        if (hp <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float) hp / hpOriginal;
    }
}
