using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Characters : MonoBehaviour
{
    internal float Speed { get; set; }

    public abstract void EnterTheCharacterData(float speed);
}
