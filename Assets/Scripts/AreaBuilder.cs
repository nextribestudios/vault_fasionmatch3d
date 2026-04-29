using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBuilder : MonoBehaviour
{
    [SerializeField] GameObject[] particleEffects;
    [SerializeField] Animator[] anims;

    public void EnableEffects()
    {
        foreach (GameObject part in particleEffects)
            part.SetActive(true);

        foreach (Animator anim in anims)
            anim.enabled = true;
    }

    public void DisableEffects()
    {
        foreach (GameObject part in particleEffects)
            part.SetActive(false);

        foreach (Animator anim in anims)
            anim.enabled = false;
    }
}
