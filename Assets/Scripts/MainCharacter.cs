using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCharacter : Characters
{
    #region Character Datas
    [SerializeField]
    private float speed;
    private Quaternion characterRotation;
    private float characterYRotation;
    [SerializeField]
    private Slider tiredSlider;
    [SerializeField]
    private float restTime;
    private bool isTired;
    [SerializeField]
    private GameObject tiredPanel;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        EnterTheCharacterData(speed);
        characterRotation = Quaternion.Euler(Vector3.zero);
        characterYRotation = transform.rotation.y;
        tiredSlider.maxValue = 10;
    }

    // Update is called once per frame
    void Update()
    {
        CharacterController();

        if (!Input.GetKey(KeyCode.W) && tiredSlider.value > tiredSlider.minValue)
        {
            if (tiredSlider.value >= tiredSlider.minValue)
                tiredSlider.value -= Time.deltaTime;

            if (tiredSlider.value <= tiredSlider.maxValue * 0.25f)
                tiredPanel.SetActive(false);
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

                if (tiredSlider.value <= tiredSlider.maxValue * 0.25f)
                    tiredPanel.SetActive(false);

                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            characterYRotation += 180;

        }
        if (Input.GetKey(KeyCode.D))
        {
            characterYRotation += Time.deltaTime * 90;
        }
        if (Input.GetKey(KeyCode.A))
        {
            characterYRotation -= Time.deltaTime * 90;
        }
        characterRotation = Quaternion.Euler(transform.rotation.z, characterYRotation, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, characterRotation, 0.085f);
    }


    public override void EnterTheCharacterData(float speed)
    {
        Speed = speed;
    }

    private IEnumerator RecoveryTime()
    {
        yield return new WaitForSeconds(5);
        isTired = false;
    }
}
