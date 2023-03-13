using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Items : MonoBehaviour
{
    abstract public bool CanPick();
    abstract public void Pick();
}
