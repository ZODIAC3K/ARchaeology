using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    [SerializeField]
    private GameObject qrUI;

    [SerializeField]
    private GameObject arUI;

    public void AR_SceneLoader(){
        qrUI.SetActive(false);

        arUI.SetActive(true);
    }

    public void QR_SceneLoader(){
        arUI.SetActive(false);

        qrUI.SetActive(true);
    }
}
