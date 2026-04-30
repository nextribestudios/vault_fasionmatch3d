using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public int itemID;
    public Rigidbody rb;
    public bool isClickable = true;
    public bool isFan = false;
    public int posIndex = 0;
    [SerializeField] Collider[] colliders;
    [SerializeField] Material normalMat;
    [SerializeField] Material outLineMat;
    [SerializeField] Renderer renderers;
    Animator anim;
    float randomX;
    float randomY;
    float randomZ;
    Vector3 randomVect;

    public float journeyTime = 1.0f;
    private float startTime;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        //    randomX = Random.Range(-90, 90);
        //    randomY = Random.Range(-90, 90);
        //    randomZ = Random.Range(-90, 90);
        //    randomVect = new Vector3(randomX, randomY, randomZ);
        if (isFan)
        {
            rb.AddForce(randomVect * 1f, ForceMode.Impulse);
            //float fracComplete = (Time.time - startTime) / journeyTime;
            //transform.position = Vector3.Slerp(startPos, endPos, fracComplete);
        }
    }

    public void SetShuffle()
    {
        randomX = 0;//Random.Range(-0.9f, 0.9f);
        randomY = 0.2f;
        randomZ = 0;// Random.Range(-1.5f, 1f);
        randomVect = new Vector3(randomX, randomY, randomZ);
        //startTime = Time.time;
        //startPos = transform.position;
        //endPos = randomVect;
        //DecreaseDrag();
        isFan = true;

        //rb.AddForce(Vector3.up * 20);
    }

    public void IncreaseDrag()
    {
        rb.linearDamping = 1;
    }

    public void DecreaseDrag()
    {
        rb.linearDamping = 0;
    }

    //IEnumerator ShuffleItem()
    //{

    //    iTween.MoveTo(gameObject, iTween.Hash("position", targetPos, "time", durationMove, "delay", delayTime, "easeType", easeType));
    //}

    public void ActivateSlotImage()
    {
        Invoke("DelayAtivateSlotImage", 0.4f);
    }

    void DelayAtivateSlotImage()
    {
        GameManager.Instance.AtivateSlotImage();
    }
    public void DestroyObject(float _time)
    {
        Destroy(gameObject, _time);
    }

    public void FinishAnimation()
    {
        anim.enabled = true;
        anim.SetTrigger("Combine");
    }

    public void BurnAnimation()
    {
        anim.enabled = true;
        anim.SetTrigger("Burn");
        DestroyObject(0.5f);
    }

    public void DisableClickable()
    {
        isClickable = false;
        rb.isKinematic = true;
        foreach (Collider col in colliders)
            col.enabled = false;
    }

    public void EnableClickable()
    {
        isClickable = true;
        rb.isKinematic = false;
        anim.enabled = false;
        foreach (Collider col in colliders)
            col.enabled = true;
    }

    public void SetNormalMaterial()
    {
        var newMats = new Material[renderers.materials.Length];
        for (int i = 0; i < renderers.materials.Length; i++)
        {
            newMats[i] = normalMat;
        }
        renderers.materials = newMats;
    }

    public void SetOutLineMaterial()
    {
        var newMats = new Material[renderers.materials.Length];
        for (int i = 0; i < renderers.materials.Length; i++)
        {
            newMats[i] = outLineMat;
        }
        renderers.materials = newMats;
        //renderers.materials[2] = outLineMat;
    }
}
