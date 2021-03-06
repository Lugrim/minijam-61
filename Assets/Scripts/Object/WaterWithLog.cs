﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWithLog : MonoBehaviour
{

    private Vector3 waterDest;
    private Vector3 waterInit;
    private GameObject log;

    // Start is called before the first frame update
    void Start()
    {
        waterDest = transform.GetChild(0).gameObject.transform.position;
        log = transform.GetChild(1).gameObject;
        waterInit = log.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Reset()
    {
        log.transform.position = waterInit;
    }

    private void OnEnable()
    {
        GameManager.ResetPrefabs += Reset;
    }

    private void OnDisable()
    {
        GameManager.ResetPrefabs += Reset;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Rain")
        {
            log.transform.position = Vector2.Lerp(log.transform.position, waterDest, 0.005f);
        }
    }
}
