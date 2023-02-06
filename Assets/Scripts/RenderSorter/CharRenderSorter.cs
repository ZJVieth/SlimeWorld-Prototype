using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharRenderSorter : MonoBehaviour
{

    public SpriteRenderer m_characterSpriteRenderer;

    void Update()
    {
        m_characterSpriteRenderer.sortingOrder = 
            (int)(CamManager.instance.pixelHeight() - CamManager.instance.WorldToScreenPoint(transform.position).y);
    }
}
