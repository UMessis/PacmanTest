using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Portals")]
    public bool isPortal;
    public GameObject portalReciever;

    [Header("Dots")]
    public bool isDot;
    public bool isBigDot;
    public bool isConsumed;
}