using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    public int maxLevel = 7;
    public string unitName;
    public int unitHealth;
    public int unitAttack;
    public int unitDefense;
    public float unitSpeed;
    public float unitRange;
    public float unitAttackDelay = 1.5f;
    public float nextAttack;
    public int level = 0;
    public bool isEnemy = false;
    public Color currentLevel;
    public Color[] levelColors = new Color[7];
    public enum Status { Idle, Moving, Attacking, Dead };
    public Status currentStatus = Status.Idle;
    public Vector3 lastPosition;
    public GameObject projectile; // arrow
    public float shootForce, upwardForce;
    private Unit _target;

    private void Awake()
    {
        InitStats();
        InitLevelColors();
        nextAttack = unitAttackDelay;
    }

    void InitStats()
    {
        this.unitHealth = 100;
        this.unitDefense = 5;
        this.unitAttack = 10;
    }

    void InitLevelColors()
    {
        levelColors[0] = Color.white;
        levelColors[1] = Color.yellow;
        levelColors[2] = Color.cyan;
        levelColors[3] = Color.blue;
        levelColors[4] = Color.magenta;
        levelColors[5] = Color.red;
        levelColors[6] = Color.black;
        for (int i = 0; i < levelColors.Length; i++)
        {
            levelColors[i].a = .65f;
        }
    }

    public void LevelUp()
    {
        if (this.level + 1 < levelColors.Length)
        {
            this.level++;
            this.unitName = levelColors[this.level].ToString() + " Cube Unit";
            this.currentLevel = levelColors[this.level];
            this.GetComponent<Renderer>().material.DOColor(this.GetComponent<Unit>().currentLevel, .25f);
        }
        else
        {
            Debug.Log("Level Maxed.");
            this.gameObject.transform.DOLocalMove(this.lastPosition, .25f);
        }
    }

    public void Update()
    {
        if (this.currentStatus == Status.Moving)
        {
            this.unitSpeed = 1f;
            this.unitRange = 5f;
            {
                if (Vector3.Distance(this.transform.position, _target.transform.position) > this.unitRange)
                {
                    // keep moving until the target is in range
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, _target.gameObject.transform.position, this.unitSpeed * Time.deltaTime);
                }
                else
                {
                    // attack enemy target if in range
                    this.currentStatus = Status.Attacking;
                }
            }
        }
        if (this.currentStatus == Status.Attacking)
        {
            // processing attack delay here
            if (nextAttack > 0)
            {
                nextAttack -= Time.deltaTime;
            }
            else if (nextAttack <= 0)
            {
                this.currentStatus = Status.Attacking;
                Debug.Log(_target.name + "'s remaining HP: " + _target.unitHealth);
                Attack(_target);
                nextAttack = unitAttackDelay;
            }
        }
    }

    public void MoveToTarget(Unit target)
    {
        _target = target;
        this.currentStatus = Status.Moving;
    }

    public void TakeDamage(int damage)
    {
        // if unit defense is higher than damage, unit will not take damage
        this.unitHealth -= damage - unitDefense > 0 ? damage - unitDefense : 0;
        if (this.unitHealth <= 0)
        {
            this.gameObject.transform.DOScale(0, .5f);
            this.currentStatus = Status.Dead;
            this.gameObject.SetActive(false);
        }
    }

    public void Attack(Unit target)
    {
        if (target.gameObject.active)
        {
            switch (this.currentStatus)
            {
                case Status.Idle:
                    break;
                case Status.Moving:
                    {
                        MoveToTarget(target);
                        break;
                    }
                case Status.Attacking:
                    {
                        this.currentStatus = Status.Attacking;
                        ShootProjectile(target);
                        target.TakeDamage(this.unitAttack);
                        break;
                    }
            }
        }
        else
        {
            this.currentStatus = Status.Idle;
        }
    }

    void ShootProjectile(Unit target)
    {
        GameObject tempObj = Instantiate(projectile, this.transform.position + Camera.main.transform.forward*1.5f, Quaternion.identity);
        //Set position of the bullet in front of the player
        //tempObj.transform.position = transform.position + Camera.main.transform.forward*1.5f;
        //Get the Rigidbody that is attached to that instantiated bullet
        //Shoot the Bullet 
        //tempObj.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * shootForce;
        tempObj.GetComponent<Rigidbody>().AddForce(tempObj.transform.forward * shootForce);
        tempObj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * upwardForce);
    }
}