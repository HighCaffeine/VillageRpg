using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    private void Awake()
    {
        SceneController.Instance.loadImage = transform.GetComponent<Image>();
    }
}
