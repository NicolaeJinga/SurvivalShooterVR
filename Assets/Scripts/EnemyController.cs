using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private bool nearPlayer = false;
    private bool isDead = false;

    public int health = 5;

    public int attackDamage = 1;
    public float attackSpeed = 1.0f;
    private float timeUntilNextAttack = 0.0f;

    public ParticleSystem effectOnReceivingDamage;

    public delegate void DeathNotify(int instanceID);
    public static event DeathNotify OnDeath;

    public delegate void PlayerAttack();
    public static event PlayerAttack OnPlayerAttack;

    private AudioSource sourceZombieDeath;
    private AudioSource sourceZombieApproach; //zombieHurt actually
    private AudioSource sourceSwordHit;
    private AudioSource sourceZombieAttack;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerBullet")
        {
            Destroy(other.gameObject);
            if (health > 0)
            {
                Instantiate(effectOnReceivingDamage, other.transform.position, effectOnReceivingDamage.transform.rotation);
                health--;
                sourceZombieApproach.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipZombieapproach);
            }
            Debug.Log("enemy hit by gun. health remaining = " + health);
        }
        else if(other.tag == "PlayerSword")
        {
            if (health > 0)
            {
                Instantiate(effectOnReceivingDamage, transform.position, effectOnReceivingDamage.transform.rotation);
                health--;
                sourceZombieApproach.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipZombieapproach);
                sourceSwordHit.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipSwordhit);
            }
            Debug.Log("enemy hit with sword. health remaining = " + health);
        }
        else if(other.tag == "Nova")
        {
            health = 0;
            Debug.Log("enemy hit by nova.");
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nearPlayer = false;
        isDead = false;
        health = 5;
        timeUntilNextAttack = 0.0f;
        sourceZombieDeath    = GetComponent<AudioSource>();
        sourceZombieApproach = GetComponent<AudioSource>();
        sourceSwordHit       = GetComponent<AudioSource>();
        sourceZombieAttack   = GetComponent<AudioSource>();
    }

    private bool playAttackSoundOnce = false;

    void Update()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        distanceToPlayer.y = 0;

        Vector3 pos = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
        Transform t = player.transform;
        t.position = pos;
        
        transform.LookAt(t);

        if(distanceToPlayer.magnitude < 1.0f)
        {
            nearPlayer = true;
            GetComponentInChildren<Animator>().SetBool("isNearPlayer", true);
            if(!playAttackSoundOnce)
            {
                playAttackSoundOnce = true;
                sourceZombieAttack.loop = true;
                sourceZombieAttack.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipZombieattack);
            }
        }
        else
        {
            nearPlayer = false;
            GetComponentInChildren<Animator>().SetBool("isNearPlayer", false);
        }
        if (health > 0)
        {
            if (!nearPlayer)
            {
                //follow player
                transform.position += distanceToPlayer.normalized * speed * Time.deltaTime;
            }
            else
            {
                //attack player
                if (timeUntilNextAttack > attackSpeed)
                {
                    OnPlayerAttack?.Invoke();
                    timeUntilNextAttack = 0.0f;
                }
                timeUntilNextAttack += Time.deltaTime;
            }
        }

        if(health <= 0)
        {
            OnDeath?.Invoke(gameObject.GetInstanceID());
            StartCoroutine(Die());
        }
    }
    bool playDeathSoundOnce = false;
    private IEnumerator Die()
    {
        Debug.Log("enemy died");
        attackDamage = 0;
        isDead = true;
        if (!playDeathSoundOnce)
        {
            sourceZombieDeath.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipZombiedeath);
            playDeathSoundOnce = true;
        }
        GetComponentInChildren<Animator>().SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + 2.0f);
        transform.position += -transform.up * speed * Time.deltaTime;
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
