using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLocker : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 48;
    }
}
