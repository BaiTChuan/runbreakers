using UnityEngine;
using System.Collections;

public class bossRoomManager : MonoBehaviour
{
    public static bossRoomManager instance;

    [Header("---- Boss Room ----")]
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] bossAI boss;

    [Header("---- Doors ----")]
    [SerializeField] door[] doors;

    [Header("---- Transition ----")]
    [SerializeField] float transitionDelay = 1.5f;

    bool fightStarted;

    void Awake()
    {
        instance = this;
        fightStarted = false;
    }

    public void StartBossFight()
    {
        if (fightStarted) return;
        fightStarted = true;
        StartCoroutine(bossFightSequence());
    }

    IEnumerator bossFightSequence()
    {
        if (Gamemanager.instance != null)
            Gamemanager.instance.showWaveTransition(0);

        yield return new WaitForSeconds(transitionDelay);

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            CharacterController cc = Gamemanager.instance.player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            Gamemanager.instance.player.transform.position = playerSpawnPoint.position;
            if (cc != null) cc.enabled = true;
        }

        foreach (door d in doors)
        {
            if (d != null)
                d.LockDoor();
        }

        if (boss != null)
            boss.ActivateBoss();

        if (Gamemanager.instance != null)
            Gamemanager.instance.setBossText();
    }

    public void OnBossDefeated()
    {
        foreach (door d in doors)
        {
            if (d != null)
                d.UnlockDoor();
        }
    }
}
