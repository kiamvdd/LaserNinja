using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LightBounce/Trick")]
public class Trick : ScriptableObject
{
    [SerializeField]
    private List<TrickConditional> m_trickConditions = new List<TrickConditional>();
    public List<TrickConditional> TrickConditions { get { return m_trickConditions; } }

    public float TimeBonus { get; set; } = 0f;
    public string Name { get; set; } = "New Trick";
}