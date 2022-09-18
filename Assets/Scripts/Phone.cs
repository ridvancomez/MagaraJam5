using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    [SerializeField]
    private GameObject bag;

    public void ShowTheBag()
    {
        bag.SetActive(true);
    }
}
