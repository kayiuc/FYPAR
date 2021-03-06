﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAttack : Photon.PunBehaviour {

    private GameObject dustRing;
    private Animator animator;
    private bool isLaunchDustRing;
    private bool isNotLaunchDustRing;
    private GolemSmokeRingManager smokeRing;
    private CharacterAbility characterAbility;
    private double PASpeed, MASpeed; 

	private bool isShortAttack, isLongAttack;

	// Use this for initialization
	void Start () {
        isShortAttack = isLongAttack = isLaunchDustRing = false;
        isNotLaunchDustRing = true;

        animator = GetComponent<Animator>();
        //dustRing = Resources.Load("GolemSmokeRing", typeof(GameObject)) as GameObject;
        smokeRing = GetComponentInChildren<GolemSmokeRingManager>();

        characterAbility = GetComponent<CharacterAbility>();
        PASpeed = characterAbility.GetpSpeed();
        MASpeed = characterAbility.GetmSpeed();
	}

	public void DisableShortAttack(){
		animator.SetBool("isShortAttack", false);
		isShortAttack = false;
		Debug.Log ("DisableShortAttack is called");
	}

	public void DisableLongAttack(){
		animator.SetBool("isLongAttack", false);
		isLongAttack = false;
		Debug.Log ("DisableSLongAttack is called");
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            //if (Input.GetKey(KeyCode.J))
//			Debug.Log("isShortAttack: " + isShortAttack);
//			Debug.Log ("animator short attack " + animator.GetBool ("isShortAttack"));
			if(isShortAttack && !animator.GetBool("isShortAttack"))
            {

				Debug.Log ("attack ");
                if(PASpeed != characterAbility.GetpSpeed())
                {
                    this.photonView.RPC("PRCSetpSpeed", PhotonTargets.All, (float)characterAbility.GetpSpeed()); 
                }
                animator.SetBool("isShortAttack", true);

				//isShortAttack = false;

				//Invoke ("DisableShortAttack", 0.2f);


            }
            else
            {
                //animator.SetBool("isShortAttack", false);
            }

          //  if (Input.GetKey(KeyCode.K) && !isLaunchDustRing)
			if (isLongAttack && !animator.GetBool("isLongAttack") && !isLaunchDustRing)
            {
				Debug.Log ("long attack");
                if (MASpeed != characterAbility.GetmSpeed())
                {
                    this.photonView.RPC("PRCSetmSpeed", PhotonTargets.All, (float)characterAbility.GetmSpeed());
                }

                isLaunchDustRing = true;
                isNotLaunchDustRing = false;

                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //animator.SetBool("isLongAttack", true);
                //Invoke("LaunchDustRing", 0.9f);
                this.photonView.RPC("RPCLaunchDustRing", PhotonTargets.All);

            }
            else if(!isNotLaunchDustRing && !isLaunchDustRing)
            {
                isNotLaunchDustRing = true;
                //animator.SetBool("isLongAttack", false);
                this.photonView.RPC("PRCStopLaunchDustRing", PhotonTargets.All);
            }
        }
    }

    private void LaunchDustRing()
    {
        //Instantiate(dustRing, transform.position, Quaternion.identity);
        smokeRing.Launch();
        Invoke("ChangeIsLaunchDustRingState", 1f);
    }

    private void ChangeIsLaunchDustRingState()
    {
        isLaunchDustRing = false;

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

	public void ShortAttack()
	{
		isShortAttack = true;
		Debug.Log ("isShortAttack is called " + isShortAttack );
	}

	public void LongAttack()
	{
		isLongAttack = true;
		Debug.Log ("isLongAttack is called " + isLongAttack);
	}
    [PunRPC]
    private void RPCLaunchDustRing()
    {
        animator.SetBool("isLongAttack", true);
        Invoke("LaunchDustRing", 0.9f);
    }

    [PunRPC]
    private void PRCStopLaunchDustRing()
    {
        animator.SetBool("isLongAttack", false);
    }

    [PunRPC]
    private void PRCSetpSpeed(float _pSpeed)
    {
        PASpeed = _pSpeed;
        animator.SetFloat("GolemPASpeed", _pSpeed);
    }

    [PunRPC]
    private void PRCSetmSpeed(float _mSpeed)
    {
        MASpeed = _mSpeed;
        animator.SetFloat("GolemMASpeed", _mSpeed);
    }
}
