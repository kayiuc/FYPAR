﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallManager : Photon.PunBehaviour {

    private float initTime;
    private int magicalAp;
    private PunTeams.Team team;
    private bool isHitPlayer, isHitPlanet;
    private int viewID;


    // Use this for initialization
    void Start () {

        if (photonView.isMine)
        {
            Debug.Log("fire ball team " + PhotonNetwork.player.GetTeam());
            team = PhotonNetwork.player.GetTeam();
        }

        initTime = Time.timeSinceLevelLoad;
        isHitPlanet = isHitPlayer = false;
        //magicalAp = 100;
    }
	
	// Update is called once per frame
	void Update () {
        
        
		if(Time.timeSinceLevelLoad - initTime >= 2f)
        {        
            PhotonNetwork.Destroy(gameObject);
        }
        
	}

    private void DisableHitPlayer()
    {
        isHitPlayer = false;
    }

    private void DisableHitPlanet()
    {
        isHitPlanet = false;
    }

    public void InitFireBall(int _magicalAp, int _viewID)
    {
        viewID = _viewID;
        magicalAp = _magicalAp;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (photonView.isMine)
        {
            if (other.tag == "Player" && other.GetComponent<CharacterAbility>().GetTeam() != team && !isHitPlayer)
            {
                isHitPlayer = true;
                Invoke("DisableHitPlayer", 0.5f);
                Debug.Log("particle hit name " + other.name);
                int otherID = other.GetPhotonView().viewID;
                other.GetComponent<CharacterAbility>().MagicalDamage(magicalAp);
                this.photonView.RPC("RPCOnParticleCollision", PhotonTargets.All, otherID);


                if (other.GetComponent<CharacterAbility>().GetHP() <= 0)
                {
                    PhotonView.Find(viewID).GetComponent<CharacterAbility>().AddCoins(CharacterAbility.REWARD);
                }
               

                PhotonNetwork.Destroy(gameObject);
            }
            else if (other.tag == "Planet" && other.GetComponent<PlanetAbility>().GetTeam() != team)
            {
                if (other.GetComponent<PlanetAbility>().GetTeam() != team && !isHitPlanet)
                {
                    isHitPlanet = true;
                    Invoke("DisableHitPlanet", 0.5f);
                    other.GetComponent<PlanetAbility>().MagicalDamage(magicalAp);
                    this.photonView.RPC("RPCOnParticleCollision", PhotonTargets.All, other.name, team);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        

    }

    [PunRPC]
    private void RPCOnParticleCollision(int otherID)
    {
        GameObject other = PhotonView.Find(otherID).gameObject;
        //other.GetComponent<CharacterAbility>().MagicalDamage(_magicalAp);
        other.GetComponent<Rigidbody>().AddForce(transform.forward * 500);
        

    }

    [PunRPC]
    private void RPCOnParticleCollision(string otherName, PunTeams.Team _team)
    {
        PlanetAbility other = GameObject.Find(otherName).GetComponent<PlanetAbility>();
        //other.MagicalDamage(_magicalAp);
        if (other.GetHP() <= 0)
            other.SetTeam(_team);
    }

}
