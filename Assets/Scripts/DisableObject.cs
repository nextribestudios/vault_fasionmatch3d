using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    [SerializeField] float timeDelay = 0.3f;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("DisableGameObject", timeDelay);
    }

    void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
