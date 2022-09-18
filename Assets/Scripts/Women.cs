using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum WomenStates { Patrol, Escape, Die, Succed, Bathub }
[RequireComponent(typeof(NavMeshAgent))]
public class Women : Characters
{

    [Header("CHARACTER DATAS"), SerializeField]
    private float speed;
    //[HideInInspector]
    public WomenStates WomenState;
    private NavMeshAgent agent;
    [SerializeField]
    private float hitSize;
    private RaycastHit hit;

    [Header("DESTINATION DATAS"), SerializeField]
    private float waitTime;
    private Transform destination;
    [SerializeField]
    private List<Transform> destinations;
    [SerializeField]
    private Transform door;
    [SerializeField]
    private bool escape;
    private bool escaped;

    [Header("GAME DATAS"), SerializeField]
    private Player playerScript;
    private bool workOnce;
    [HideInInspector]
    public bool IsDie, ManagedToEscape;
    //[SerializeField]
    //private Transform bathubPlayerLocation;
    private void Start()
    {
        WomenState = WomenStates.Patrol;
        destination = destinations[Random.Range(0, destinations.Count)];
        agent = GetComponent<NavMeshAgent>();
        EnterTheCharacterData(speed);
        StartCoroutine(GoToDestionation());
    }

    private void Update()
    {
        switch (WomenState)
        {
            case WomenStates.Patrol:
                Patrol();
                break;
            case WomenStates.Escape:
                Escape();
                break;
            case WomenStates.Die:
                Die();
                break;
            case WomenStates.Bathub:
                Bathub();
            break;
            case WomenStates.Succed:
                gameObject.SetActive(false);
                break;
        }
    }
    private void Patrol()
    {
        if (agent.remainingDistance < 2 && !workOnce && !escape)
        {
            workOnce = true;
            StartCoroutine(WaitAndGo());
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, hitSize))
        {
            if (hit.collider.CompareTag("PlayerFront") && playerScript.IsMurderer && !IsDie)
            {
                ChangeState(WomenStates.Escape);
                escape = true;
            }
            Debug.DrawLine(transform.position, hit.point, Color.green);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * hitSize, Color.red);
        }
    }

    private void Escape()
    {
        if (escape)
        {
            EnterTheCharacterData(speed * 2);
            agent.SetDestination(door.position);

            if (agent.remainingDistance < 1 && escaped)
            {
                ManagedToEscape = true;
                ChangeState(WomenStates.Succed);
            }
        }
    }

    private void Die()
    {
        EnterTheCharacterData(0);
        IsDie = true;
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        transform.position -= new Vector3(0, 0.5f, 0);
    }

    private void Bathub()
    {
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        transform.position = new Vector3(-48.5f, 2.25f, -18.25f);
    }
    public void ChangeState(WomenStates newState)
    {
        WomenState = newState;
        if(WomenState == WomenStates.Bathub)
            Bathub();
    }

    public override void EnterTheCharacterData(float speed)
    {
        Speed = speed;
        agent.speed = Speed;
    }

    IEnumerator GoToDestionation()
    {
        destination = destinations[Random.Range(0, destinations.Count)];
        agent.SetDestination(destination.position);
        workOnce = false;
        yield return null;
    }
    IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(GoToDestionation());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Door" && escape)
            escaped = true;
    }
}
