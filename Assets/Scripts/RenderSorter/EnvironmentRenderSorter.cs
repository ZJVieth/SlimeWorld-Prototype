using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRenderSorter : MonoBehaviour
{

    void Start()
    {
        foreach(Transform child in transform) {
            foreach(Transform childObj in child) {
                childObj.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 
                    (int)(CamManager.instance.pixelHeight() - CamManager.instance.WorldToScreenPoint(childObj.position).y);
            }
        }
    }

    void Update()
    {
        foreach(Transform child in transform) {
            if (child.gameObject.name.Contains("dynamic")) {
                foreach(Transform childObj in child) {
                    childObj.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 
                        (int)(CamManager.instance.pixelHeight() - CamManager.instance.WorldToScreenPoint(childObj.position).y);
                }
            }
        }
    }
}
