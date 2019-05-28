using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_bulletIcon;
    [SerializeField]
    private float m_spacing = 0.1f;
    private Stack<SpriteRenderer> m_bulletIcons = new Stack<SpriteRenderer>();
    [SerializeField]
    private float m_flashTime = 1;
    [SerializeField]
    private float m_fadeoutTime = 1;
    private IEnumerator m_flashAnimation = null;
    private float m_xOffset;

    private void Awake()
    {
        m_xOffset = transform.localPosition.x;
    }

    public void SetAmmoCount(int amount)
    {
        ModifyAmmoCountBy(amount - m_bulletIcons.Count);
    }

    public void ModifyAmmoCountBy(int amount)
    {
        if (amount == 0)
            return;
        else if (amount > 0)
            AddAmmo(amount);
        else
            RemoveAmmo(Mathf.Abs(amount));

        UpdatePosition();
    }

    private void AddAmmo(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            InstantiateIcon();
        }
    }

    private void RemoveAmmo(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (m_bulletIcons.Count <= 0)
                break;

            SpriteRenderer icon = m_bulletIcons.Pop();
            Destroy(icon.gameObject);
        }
    }

    public void FlashIcons()
    {
        if (m_flashAnimation != null)
            StopCoroutine(m_flashAnimation);

        m_flashAnimation = AnimateFlash();
        StartCoroutine(m_flashAnimation);
    }

    private IEnumerator AnimateFlash()
    {
        float timer = m_flashTime;
        foreach (SpriteRenderer renderer in m_bulletIcons)
        {
            Color color = renderer.color;
            color.a = 1;
            renderer.color = color;
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        timer = m_fadeoutTime;

        while (timer > 0)
        {
            foreach (SpriteRenderer renderer in m_bulletIcons)
            {
                Color color = renderer.color;
                color.a = timer / m_fadeoutTime;
                renderer.color = color;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        m_flashAnimation = null;
    }

    private void InstantiateIcon()
    {
        SpriteRenderer icon = Instantiate(m_bulletIcon);
        icon.transform.SetParent(transform);
        icon.transform.localPosition = new Vector3(m_bulletIcons.Count * m_spacing, 0, 0);
        m_bulletIcons.Push(icon);
    }

    private void UpdatePosition()
    {
        Vector3 pos = transform.localPosition;
        pos.x = m_xOffset - (m_bulletIcons.Count - 1) * m_spacing * 0.5f;
        transform.localPosition = pos;
    }
}
