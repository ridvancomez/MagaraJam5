using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PartnerStates { Follow, Die }

[RequireComponent(typeof(NavMeshAgent))]
public class Partner : Characters
{
    [Header("CHARACTER DATAS"), SerializeField]
    private float speed;
    public PartnerStates PartnerState;
    private NavMeshAgent agent;

    [Header("GAME DATAS"), SerializeField]
    private GameObject player;
    private Player playerScript;
    private bool switchScreenWorked;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerScript = player.GetComponent<Player>();
    }
    private void Update()
    {
        switch (PartnerState)
        {
            case PartnerStates.Follow:
                agent.SetDestination(player.transform.position - Vector3.forward * 2);
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                break;
            case PartnerStates.Die:
                transform.rotation = Quaternion.Euler(-90, 0, 0);
                transform.position -= new Vector3(0, 0.5f, 0);
                if (!switchScreenWorked)
                {
                    playerScript.ChangeState(PlayerStates.BusinessTransition);
                    switchScreenWorked = true;
                }
                break;
        }
    }

    public override void EnterTheCharacterData(float speed)
    {
        Speed = speed;
        agent.speed = Speed;
    }

    public void ChangeState(PartnerStates newState)
    {
        PartnerState = newState;
    }
}
