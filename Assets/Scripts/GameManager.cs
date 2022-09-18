using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<Tips> tips = new List<Tips>();
    [SerializeField]
    private Player playerScript;
    [HideInInspector]
    public bool TipCreated;
    public Tips CurrentTip;
    [SerializeField]
    private GameObject businessTransitionPanel; //ortaðýný öldürdükten sonra gelecek panel
    [SerializeField]
    private GameObject phoneTransitionPanel; //evde telefonu bulduktan sonra sonra gelecek panel
    [SerializeField]
    private GameObject sleepTransitionPanel; // uyuyyup uyandýktan sonra gelecek panel
    [SerializeField]
    private GameObject murderTransitionPanel; // birini öldürdükten sonra gelecek panel
    [SerializeField]
    private GameObject caughtTransitionPanel; // tüm cinayetleri iþledikten sonra vyea hata yaptýktan sonra gelecek panel
    [SerializeField]
    private GameObject prosecutorTransitionPanel; // hüküm giyerken gelecek panel
    [SerializeField]
    private GameObject bathubTransitionPanel; // hüküm giyerken gelecek panel
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private GameObject playerDogum;
    [SerializeField]
    private GameObject womenDogum;
    [SerializeField]
    public GameObject[] womens;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
        else if (PlayerPrefs.GetInt("Level") != 1)
            CreatedTip();
    }
    public void CreatedTip()
    {
        playerScript.timer = 60;
        playerScript.gameObject.transform.position = playerDogum.transform.position;
        playerScript.IsMurderer = false;

        CurrentTip = tips[0];
        CurrentTip.BeingUsed = true;

        CurrentTip.Object.CompareTag("Tip");
        descriptionText.text = CurrentTip.Title;
        if (PlayerPrefs.GetInt("Level") != 1)
        {
            playerScript.ChangeState(PlayerStates.BusinessTransition);
            playerScript.ChangeState(PlayerStates.Preparation);

        }
    }

    public void ShowThePanel()
    {
        //Debug.Log(CurrentTip.);
        HideAllPanel();

        if (playerScript.PlayerState == PlayerStates.Sleep)
            descriptionText.text = "Uyu";

        switch (playerScript.PlayerState)
        {
            case PlayerStates.Preparation:
                if (PlayerPrefs.GetInt("Level") != 1)
                    CreatedTip();
                CurrentTip.Object.CompareTag("Tip");
                descriptionText.text = CurrentTip.Title;
                break;
            case PlayerStates.BusinessTransition:
                if (PlayerPrefs.GetInt("Level") != 1)
                    playerScript.ChangeState(PlayerStates.Preparation);
                else
                    CreatedTip();
                businessTransitionPanel.SetActive(true);
                break;
            case PlayerStates.SleepTransition:
                CreatedTip();
                phoneTransitionPanel.SetActive(true);
                break;
            case PlayerStates.MurderTransition:
                sleepTransitionPanel.SetActive(true);
                break;
            case PlayerStates.CaughtTransition:
                murderTransitionPanel.SetActive(true);
                break;
            case PlayerStates.PrisonTransition:
                caughtTransitionPanel.SetActive(true);
                break;
            case PlayerStates.ProsecutorPanel:
                prosecutorTransitionPanel.SetActive(true);
                break;
            case PlayerStates.BathubTransition:
                bathubTransitionPanel.SetActive(true);
                break;
            default:
                HideAllPanel();
                break;
        }
    }

    public void HideAllPanel()
    {
        businessTransitionPanel.SetActive(false);
        phoneTransitionPanel.SetActive(false);
        sleepTransitionPanel.SetActive(false);
        murderTransitionPanel.SetActive(false);
        caughtTransitionPanel.SetActive(false);
        prosecutorTransitionPanel.SetActive(false);
        bathubTransitionPanel.SetActive(false);
    }


}
