﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSmokeRingManager : MonoBehaviour {

    private ParticleSystem[] smokeRing;

	// Use this for initialization
	void Start () {
        smokeRing = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(Time.time - init_time >= 2.5f)
        {
            Destroy(gameObject);
        }
        */

	}

    public void Launch()
    {
        foreach (ParticleSystem particle in smokeRing)
        {
            particle.Play();
        }
    }


}
