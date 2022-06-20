
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public float health;
    public TMP_Text healthIndicator;
    Transform followAt;
    public float distanceToFollow;
    public EnemySO data;

    private NavMeshAgent mNavMeshAgent;
    private Animator mAnimator;

    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mAnimator = transform.Find("Mutant").GetComponent<Animator>();
    }

    private void Start()
    {
        health = data.health;
        mNavMeshAgent.speed = data.speed;
        followAt = GameObject.FindGameObjectWithTag("Player").transform;
    }

    bool destroying;
    private void Update()
    {
        health = Mathf.Clamp(health, 0, data.health);

        healthIndicator.text = $"HP: {health}/{data.health}";

        //mNavMeshAgent.destination = followAt.position;
        float distance = Vector3.Distance(transform.position, followAt.position);
        if (distance <= distanceToFollow)
        {
            mNavMeshAgent.isStopped = false;
            mNavMeshAgent.destination = followAt.position;
            mAnimator.SetTrigger("Walk");
        }
        else
        {
            mAnimator.SetTrigger("Stop");
            mNavMeshAgent.isStopped = true;
        }

        if (health <= 0 && !destroying)
        {
            destroying = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter " + other.name);
        var player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.health -= data.damage;
        }
    }
}
