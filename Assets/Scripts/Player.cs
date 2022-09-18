using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerStates
{
    Business,
    Preparation,
    BusinessTransition, SleepTransition, MurderTransition, CaughtTransition, BathubTransition, PrisonTransition, ProsecutorPanel,
    Order,
    FindTip,
    Normal,
    Murderer,
    Sleep
}
public class Player : Characters
{
    [Header("CHARACTER DATAS"), SerializeField]
    private float speed;
    //[HideInInspector]
    public PlayerStates PlayerState;
    private MeshRenderer characterRenderer;
    private Quaternion characterRotation;
    private float characterYRotation;

    [Header("SLIDER DATAS"), SerializeField]
    private Slider tiredSlider;
    [SerializeField]
    private float restTime;
    private bool isTired;
    [SerializeField]
    private GameObject tiredPanel;

    [Header("KILL DATAS")]
    [SerializeField]
    private Material killModeMaterial;
    [SerializeField]
    private bool killer;
    [HideInInspector]
    public bool IsMurderer;

    [Header("GAME DATAS"), SerializeField]
    private GameObject women;
    [SerializeField]
    private Transform bathubPlayerLocation;
    private Women womenScript;
    [SerializeField]
    private float murdererModeTime;
    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private Partner businessPartnerScript;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject homePhone;
    private Phone homePhoneScript;
    [SerializeField]
    public float timer;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private GameObject bag;
    [SerializeField]
    private GameObject bed;
    public bool isCollect;
    public bool isBagCollect;
    [SerializeField]
    private Transform playerDogum;
    [SerializeField]
    private Transform playerDogumOfis;

    void Start()
    {
        womenScript = women.GetComponent<Women>();
        gameManager.ShowThePanel();
        PlayerState = PlayerStates.Business;
        StartCoroutine(OpenTheMurdererMode());
        characterRenderer = GetComponent<MeshRenderer>();
        characterYRotation = transform.rotation.y;
        tiredSlider.maxValue = 10;
        homePhoneScript = homePhone.GetComponent<Phone>();
        if(PlayerPrefs.GetInt("Level") == 1)
        transform.position = playerDogumOfis.position;
    }

    void Update()
    {
        CharacterController();

        if (!Input.GetKey(KeyCode.W) && tiredSlider.value > tiredSlider.minValue)
        {
            if (tiredSlider.value >= tiredSlider.minValue)
                tiredSlider.value -= Time.deltaTime;

            if (tiredSlider.value <= tiredSlider.maxValue * 0.25f && tiredPanel.activeSelf)
                tiredPanel.SetActive(false);
        }
        if (women.activeSelf == true)
        {
            if (womenScript.IsDie || womenScript.ManagedToEscape)
                killer = false;
            else
                killer = Vector3.Distance(transform.position, women.transform.position) < 2;
        }


        if (PlayerState == PlayerStates.Murderer && !killer)
            Murderer();

        if (PlayerState == PlayerStates.Preparation)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else if (timer <= 0)
                timer = 0;

            timerText.text = timer.ToString("0.00");
        }
    }

    private void CharacterController()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isTired)
            {
                tiredSlider.value += Time.deltaTime;
                if (tiredSlider.value > tiredSlider.maxValue * 0.25f)
                {
                    tiredPanel.SetActive(true);

                }
                if (tiredSlider.value == tiredSlider.maxValue)
                {
                    isTired = true;
                    StartCoroutine(RecoveryTime());
                }
                transform.Translate(Vector3.forward * speed * 2f * Time.deltaTime);

            }
            else
            {
                if (tiredSlider.value > tiredSlider.minValue)
                    tiredSlider.value -= Time.deltaTime;

                if (tiredSlider.value <= tiredSlider.maxValue * 0.25f && tiredPanel.activeSelf)
                    tiredPanel.SetActive(false);

                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            characterYRotation += 180;
        }
        characterYRotation += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        characterRotation = Quaternion.Euler(transform.rotation.z, characterYRotation, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, characterRotation, 0.085f);

        TakeAction();
    }

    private void TakeAction()
    {
        switch (PlayerState)
        {
            case PlayerStates.Business:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (Vector3.Distance(transform.position, businessPartnerScript.transform.position) < 3)
                    {
                        BusinessKill();
                    }
                }
                break;
            case PlayerStates.Preparation:
                if (Vector3.Distance(transform.position, bag.transform.position) < 3 && Input.GetKeyDown(KeyCode.E))
                {
                    bag.SetActive(false);
                    isBagCollect = true;
                }
                if (Vector3.Distance(transform.position, homePhone.transform.position) < 3 && Input.GetKeyDown(KeyCode.E))
                {
                    homePhoneScript.ShowTheBag();
                }

                if (Vector3.Distance(transform.position, gameManager.CurrentTip.Object.transform.position) < 3 && Input.GetKeyDown(KeyCode.E) && isBagCollect)
                {
                    gameManager.CurrentTip.Object.SetActive(false);
                    isCollect = true;
                }

                if (timer == 0 || Input.GetKeyDown(KeyCode.Q))
                {
                    women = gameManager.womens[Random.Range(0,1)];
                    women.SetActive(true);
                    ChangeState(PlayerStates.Normal);
                }
                break;
            case PlayerStates.BusinessTransition:
                if (Input.GetKeyDown(KeyCode.F))
                {
                    transform.position = Vector3.up;
                    ChangeState(PlayerStates.Preparation);
                    //women.SetActive(true);
                }
                break;
            case PlayerStates.SleepTransition:
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                    SceneManager.LoadScene(0);
                    gameManager.CreatedTip();

                    ChangeState(PlayerStates.Preparation);
                    transform.position = playerDogum.position;
                }
                break;
            case PlayerStates.BathubTransition:
                if (Input.GetKeyDown(KeyCode.F))
                    ChangeState(PlayerStates.Sleep);
                break;
            case PlayerStates.MurderTransition:
                Murderer();
                break;
            case PlayerStates.CaughtTransition:
                break;
            case PlayerStates.PrisonTransition:
                break;
            case PlayerStates.ProsecutorPanel:
                break;
            case PlayerStates.Order:
                break;
            case PlayerStates.FindTip:
                break;
            case PlayerStates.Normal:
                if (Input.GetKey(KeyCode.E))
                {
                    if (!womenScript.IsDie)
                    {
                        ChangeState(PlayerStates.Murderer);
                        if (!killer)
                            ChangeState(PlayerStates.Murderer);
                        else
                            Kill();
                    }
                }
                break;
            case PlayerStates.Murderer:
                Murderer();
                break;
            case PlayerStates.Sleep:
                if (Vector3.Distance(transform.position, bed.transform.position) < 3 && Input.GetKeyDown(KeyCode.E))
                {
                    ChangeState(PlayerStates.SleepTransition);
                }
                break;
        }
    }

    public void ChangeState(PlayerStates newState)
    {
        PlayerState = newState;
        gameManager.ShowThePanel();


        switch (PlayerState)
        {
            case PlayerStates.BusinessTransition:
                break;
            case PlayerStates.SleepTransition:

                break;
            case PlayerStates.MurderTransition:

                break;
            case PlayerStates.CaughtTransition:

                break;
            case PlayerStates.PrisonTransition:

                break;
            case PlayerStates.ProsecutorPanel:

                break;
        }
    }

    public override void EnterTheCharacterData(float speed)
    {
        Speed = speed;
    }

    private void Murderer()
    {

        killer = true;
        characterRenderer.material = killModeMaterial;
        IsMurderer = true;
        if (Vector3.Distance(women.transform.position, transform.position) < 3 && Input.GetKeyDown(KeyCode.E))
            Kill();

        if (Input.GetKeyDown(KeyCode.F) && womenScript.WomenState == WomenStates.Die)
        {
            ChangeState(PlayerStates.BathubTransition);
            womenScript.ChangeState(WomenStates.Bathub);
            transform.position = bathubPlayerLocation.position;
        }

    }

    private void Kill()
    {
        killer = false;
        womenScript.ChangeState(WomenStates.Die);
    }

    private void BusinessKill()
    {
        businessPartnerScript.ChangeState(PartnerStates.Die);
    }

    private IEnumerator RecoveryTime()
    {
        yield return new WaitForSeconds(restTime);
        isTired = false;

    }

    private IEnumerator OpenTheMurdererMode()
    {
        if (PlayerState == PlayerStates.Normal)
        {
            yield return new WaitForSeconds(murdererModeTime);
            Murderer();
        }
        else
        {
            yield return null;
            StartCoroutine(OpenTheMurdererMode());
        }

    }
}
