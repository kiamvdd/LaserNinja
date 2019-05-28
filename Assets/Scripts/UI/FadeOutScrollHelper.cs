using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutScrollHelper : MonoBehaviour
{
    public FadeOutButtonScrollList FadeOutList;

    public void OnInputDown()
    {
        FadeOutList.ReceiveChildInput(transform);
    }
}
