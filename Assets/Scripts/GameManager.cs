using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public int playerHealth = 20;
    public TextMesh playerHealthUI;

    private int playerScore;
    public TextMesh playerScoreUI;

    public int maxChargeup = 5;
    [HideInInspector]
    public int currentChargeup;
    //public TextMesh chargeupUI;

    public Slider chargeupSlider1;
    public Slider chargeupSlider2;

    [HideInInspector]
    public bool powerReady = false;

    public GameObject enemy;
    public int maxNrOfEnemies;
    List<GameObject> enemyPool;

    public GameObject innerR;
    public GameObject outerR;
    float innerRadius;
    float outerRadius;

    public GameObject powerup;
    private GameObject instantiatedNova;

    // Sounds
    public AudioSource bgmusic;
    [HideInInspector] public AudioSource gunshot;
    [HideInInspector] public AudioSource swordhit;
    public AudioSource swordnova;
    [HideInInspector] public AudioSource zombieapproach;
    [HideInInspector] public AudioSource zombieattack;
    [HideInInspector] public AudioSource zombiedeath;

    public AudioClip clipGunshot;
    public AudioClip clipSwordhit;
    public AudioClip clipSwordnova;
    public AudioClip clipZombieapproach;
    public AudioClip clipZombieattack;
    public AudioClip clipZombiedeath;

    private void InitMusic()
    {
        bgmusic.        GetComponent<AudioSource>();
        //gunshot.        GetComponent<AudioSource>();
        //swordhit.       GetComponent<AudioSource>();
        swordnova.      GetComponent<AudioSource>();
        //zombieapproach. GetComponent<AudioSource>();
        //zombieattack.   GetComponent<AudioSource>();
        //zombiedeath.    GetComponent<AudioSource>();
        bgmusic.loop = true;
        bgmusic.Play();
    }

    private void OnEnable()
    {
        EnemyController.OnDeath += EnemyDie;
        EnemyController.OnPlayerAttack += DamagePlayer;
        PowerupActivation.OnGMPowerNotify += PowerReset;
    }

    private void OnDisable()
    {
        EnemyController.OnDeath -= EnemyDie;
        EnemyController.OnPlayerAttack -= DamagePlayer;
        PowerupActivation.OnGMPowerNotify -= PowerReset;
    }

    void PowerReset(Vector3 pos)
    {
        swordnova.PlayOneShot(clipSwordnova);
        instantiatedNova = Instantiate(powerup, pos, powerup.transform.rotation);
        powerReady = false;
        currentChargeup = 0;
        //chargeupUI.text = "Powerup: " + currentChargeup + "/" + maxChargeup;
        chargeupSlider1.value = currentChargeup;
        chargeupSlider2.value = currentChargeup;
    }

    void DamagePlayer()
    {
        playerHealth--;
        playerHealthUI.text = "HP: " + playerHealth;
    }

    void EnemyDie(int instanceID)
    {
        foreach(var enemy in enemyPool.ToArray())
        {
            if(enemy.GetInstanceID() == instanceID)
            {
                enemyPool.Remove(enemy);
                playerScore++;
                playerScoreUI.text = "Score: " + playerScore;
                if (currentChargeup < maxChargeup && instantiatedNova == null)
                {
                    currentChargeup++;
                    //chargeupUI.text = "Powerup: " + currentChargeup + "/" + maxChargeup;
                    chargeupSlider1.value = currentChargeup;
                    chargeupSlider2.value = currentChargeup;
                }
                GenerateEnemy(true);
            }
        }
    }

    Vector3 GenerateEnemy(bool instantiate)
    {
        int tries = 0;
        do
        {
            Vector3 offset = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
            offset.z = offset.y;
            offset.y = 0.0f;
            enemy.transform.position = player.transform.position + offset;
            tries++;
        } while ((enemy.transform.position - player.transform.position).magnitude < outerRadius &&
                 (enemy.transform.position - player.transform.position).magnitude > innerRadius &&
                 tries < 20);
        if(instantiate)
        {
            GameObject e = Instantiate(enemy, enemy.transform.position, enemy.transform.rotation);
            enemyPool.Add(e.gameObject);
        }
        return enemy.transform.position;
    }

    void InitialEnemySpawn()
    {
        for(int i = 0; i < maxNrOfEnemies; ++i)
        {
            GenerateEnemy(true);
        }
    }

    void Start()
    {
        enemyPool = new List<GameObject>();
        innerRadius = (player.transform.position + innerR.transform.position).magnitude;
        outerRadius = (player.transform.position + outerR.transform.position).magnitude;
        InitialEnemySpawn();
        playerHealthUI.text = "HP: " + playerHealth;
        chargeupSlider1.maxValue = maxChargeup;
        chargeupSlider2.maxValue = maxChargeup;
        currentChargeup = 0;
        //chargeupUI.text = "Powerup: " + currentChargeup + "/" + maxChargeup;
        chargeupSlider1.value = currentChargeup;
        chargeupSlider2.value = currentChargeup;
        playerScore = 0;
        playerScoreUI.text = "Score: " + playerScore;
        InitMusic();
    }

    void Update()
    {
        if(playerHealth <= 0)
        {
            Debug.Log("GAMEOVER");
            SceneManager.LoadScene(1);
        }
        if(currentChargeup >= maxChargeup)
        {
            powerReady = true;
            Debug.Log("power ready to use");
        }
    }
}
