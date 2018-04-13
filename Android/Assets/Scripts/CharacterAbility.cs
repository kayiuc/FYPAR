using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterAbility : Photon.PunBehaviour
{
    public const int REWARD = 500;
    public Image healthBar;
    public enum Team { none, blue, red };
    public GameObject[] buttonList;
    public GameObject dieCanvas;
    public Text weaponText, armorText, coinsText;

    private float startHP;
    private int hp;
    private int mp;
    private int physicalAp;
    private int magicalAp;
    private int physicalDp;
    private int magicalDp;
    private double pSpeed;
    private double mSpeed;
    private double speed;
    private int coins, weaponCost, armorCost, weaponLevel, armorLevel;

    private Weapon equipWeapon;
    private Armor equipArmor;
    private PunTeams.Team team;
    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    // Use this for initialization
    void Start()
    {
        if (photonView.isMine)
        {
            this.photonView.RPC("SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam());
            dieCanvas = GameObject.Find("DieCanvas");
            dieCanvas.SetActive(false);
            coins = 0;
            weaponCost = armorCost = weaponLevel = armorLevel = 1;
            AddCoins();
        }
        else
        {
            foreach(GameObject button in buttonList)
            {
                button.SetActive(false);
            }

            coinsText.enabled = false;
        }
        

        healthBar.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            CharacterDie();
        }   
    }

	private void AddCoins()
	{
		coins += 1;
		coinsText.text = "coins: " + coins.ToString();
		Invoke("AddCoins", 1f);

	}


    private void CharacterDie()
    {
        gameObject.SetActive(false);
        Invoke("CharacterRebirth", Random.Range(10f, 20f));
        if (photonView.isMine)
        {
            //gray screen
            dieCanvas.SetActive(true);
        }
    }

    private void CharacterRebirth()
    {
        if (photonView.isMine)
        {
            dieCanvas.SetActive(false);
        }
        hp = (int)startHP;
        healthBar.fillAmount = 1;
        gameObject.SetActive(true);
        gameObject.transform.position = new Vector3(Random.Range(0, 0), 0, 0);
    }

    public void CharacterEndGame()
    {
        if (photonView.isMine)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init(int _hp, int _mp, int _physicalAp, int _magicalAp, int _physicalDp, int _magicalDp, double _speed)
    {
        startHP = hp = _hp;
        mp = _mp;
        physicalAp = _physicalAp;
        magicalAp = _magicalAp;
        physicalDp = _physicalDp;
        magicalDp = _magicalDp;
        mSpeed = pSpeed = 1;
        speed = _speed;
    }

    public void PhysicalDamage(int _ap)
    {
        int damage = (_ap - physicalDp);
        this.photonView.RPC("RPCPhysicalDamage", PhotonTargets.All, ((damage < 0) ? 0 : damage));
    }

    public void MagicalDamage(int _ap)
    {
        int damage = (_ap - magicalDp);
        this.photonView.RPC("RPCMagicalDamage", PhotonTargets.All, ((damage < 0) ? 0 : damage));
    }

    public void SetMP(int _mp)
    {
        mp -= _mp;
    }


    public int GetHP()
    {
        return hp;
    }

    public int GetMP()
    {
        return mp;
    }

    public int GetPAP()
    {
        return physicalAp;
    }

    public int GetMAP()
    {
        return magicalAp;
    }

    public int GetPDP()
    {
        return physicalDp;
    }

    public int GetMDP()
    {
        return magicalDp;
    }

    public double GetpSpeed()
    {
        return pSpeed;
    }

    public double GetmSpeed()
    {
        return mSpeed;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void AddCoins(int _coins)
    {
        coins += _coins;
        coinsText.text = "coins: " + coins.ToString();
    }

    public void ReduceCoins(int _cost)
    {
        coins -= _cost;
        coinsText.text = "coins: " + coins.ToString();
    }

    public void EquipWeapon(Weapon weapon)
    {
        this.photonView.RPC("RPCEquipWeapon", PhotonTargets.All, (int)weapon.type, weapon.apRate, weapon.speedRate);
        this.photonView.RPC("RPCWeaponBuff", PhotonTargets.All, (int)weapon.type, weapon.apRate, weapon.speedRate);

    }


    public void WeaponBuff()
    {
        if (photonView.isMine)
        {
            if (weaponLevel > 9 || weaponCost > coins)
            {
                return;
            }

            ReduceCoins(weaponCost);
            weaponCost *= 2;
            weaponLevel++;
            weaponText.text = "W " + weaponLevel;
            Debug.Log("equipArmor " + equipArmor.type);
            this.photonView.RPC("RPCWeaponBuff", PhotonTargets.All, (int)equipWeapon.type, equipWeapon.apRate, equipWeapon.speedRate);
        }
        
      
    }

    public void EquipArmor(Armor armor)
    {
        
        this.photonView.RPC("RPCEquipArmor", PhotonTargets.All, (int)armor.type, armor.pdpRate, armor.mdpRate, armor.speed);
        this.photonView.RPC("RPCArmorBuff", PhotonTargets.All, (int)armor.type, armor.pdpRate, armor.mdpRate, armor.speed);
    }

    public void ArmorBuff()
    {
        if (photonView.isMine)
        {
            if (armorLevel > 9 || armorCost > coins)
            {
                return;
            }

            ReduceCoins(armorCost);
            armorCost *= 2;
            armorLevel++;
            armorText.text = "A " + armorLevel;
            this.photonView.RPC("RPCArmorBuff", PhotonTargets.All, (int)equipArmor.type, equipArmor.pdpRate, equipArmor.mdpRate, equipArmor.speed);
        }
        
    }

    public PunTeams.Team GetTeam()
    {
        return team;
    }

    [PunRPC]
    public void RPCPhysicalDamage(int _ap)
    {
        hp = hp - _ap;
        healthBar.fillAmount = hp / startHP;
        Debug.Log("Phy damage: " + _ap);
    }

    [PunRPC]
    public void RPCMagicalDamage(int _ap)
    {
        hp = hp - _ap;
        healthBar.fillAmount = hp / startHP;
        Debug.Log("_ap " + _ap);
        Debug.Log("magicalDp" + magicalDp);
        Debug.Log("Mag damage: " + _ap);
    }

    [PunRPC]
    private void SetTeam(PunTeams.Team _team)
    {
        team = _team;
    }

    [PunRPC]
    private void RPCEquipWeapon(int type, double apRate, double speedRate)
    {
        Weapon weapon = new Weapon((WeaponAbility.Weapon)type, apRate, speedRate);
        equipWeapon = weapon;

    }

    [PunRPC]
    private void RPCWeaponBuff(int type, double apRate, double speedRate)
    {
        Debug.Log("WeaponBuff " + type);
        
        

        if ((WeaponAbility.Weapon)type == WeaponAbility.Weapon.AXE || (WeaponAbility.Weapon)type == WeaponAbility.Weapon.SWORD)
        {
            physicalAp = (int)(physicalAp + physicalAp * apRate);
            Debug.Log("WeaponBuff " + physicalAp);
        }
        else
        {
            magicalAp = (int)(magicalAp + magicalAp * apRate);
        }
        pSpeed = pSpeed + pSpeed * speedRate;
        Debug.Log("WeaponBuff " + pSpeed);
    }

    [PunRPC]
    private void RPCEquipArmor(int type, double pdpRate, double mdpRate, double speedRate)
    {
        Armor armor = new Armor((ArmorAbility.Armor)type, pdpRate, mdpRate, speedRate);
        equipArmor = armor;
        Debug.Log("equipArmor type " + equipArmor.type);
    }

    [PunRPC]
    private void RPCArmorBuff(int type, double pdpRate, double mdpRate, double speedRate)
    {
        if((ArmorAbility.Armor)type == ArmorAbility.Armor.BOOT)
        {
            speed = speed + speed * speedRate;   
            characterMovement.SetMovement((float)speed, (float)speed + 8);
            Debug.Log("Movement is called " + speed);
        }
        else
        {
            physicalDp = (int)(physicalDp + physicalDp * pdpRate);
            magicalAp = (int)(magicalDp + magicalDp * mdpRate);
        }
        
    }

}
