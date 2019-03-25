using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "LightBounce/Trick")]
public class Trick : ScriptableObject
{
    [SerializeField]
    private List<TrickConditional> m_trickConditions = new List<TrickConditional>();
    public List<TrickConditional> TrickConditions { get { return m_trickConditions; } }

    [SerializeField]
    private float m_timeBonus = 0f;
    public float TimeBonus { get { return m_timeBonus; } }

    [SerializeField]
    private string m_name = "New Trick";
    public string Name { get { return m_name; } }
}