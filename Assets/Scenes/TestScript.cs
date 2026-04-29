using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float remainingTime;
    public DateTime startTime;
    public TimeSpan timeSpan;

    //public int startLife;

    void Start()
    {
        //startTime = PlayerPrefs.GetFloat("StartTime");
        //remainingTime = PlayerPrefs.GetFloat("RemainTime");
        //startLife = PlayerPrefs.GetInt("StartLife");
        timeSpan = DateTime.Now - startTime;
    }

    //private void Update()
    //{
    //if (Input.GetMouseButtonDown(0))
    //{
    // here I want to move object x position when mouse button press
    //target.transform.position.x += 1;
    //    c.Insert(5, 20);
    //    foreach (int q in c)
    //        Debug.LogError(q);
    //}

    //a++;
    //if (a = 10)
    //{
    //    Debug.Log("print a");
    //}
    //}

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    float _time = Time.time;
        //    float minutes = Mathf.FloorToInt(_time / 60);
        //    float seconds = Mathf.FloorToInt(_time % 60);
        //}

        //if (startLife == 1)
        //{
        //    if (remainingTime > 0)
        //    {
        //        remainingTime -= Time.deltaTime;
        //    }
        //    else
        //    {

        //    }
        //}
    }

    public void LifeStart()
    {
        startTime = DateTime.Now;
        PlayerPrefs.SetString("StartTime", startTime.ToString());
        remainingTime += 15;
    }

    private void OnApplicationQuit()
    {
        //PlayerPrefs.SetFloat("RemainTime", remainingTime);
        //PlayerPrefs.SetFloat("StartTime", startTime.ToString());
    }
}
