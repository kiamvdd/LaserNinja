using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLocker : MonoBehaviour
{
    public int TargetFramerate = 60;
    void Update()
    {
        Application.targetFrameRate = TargetFramerate;
    }
}
