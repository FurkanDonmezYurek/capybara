using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELımage : MonoBehaviour
{
    public GameObject elImage;

    public void Goster()
    {
        elImage.SetActive(true);
    }

    public void Gizle()
    {
        elImage.SetActive(false);
    }
}