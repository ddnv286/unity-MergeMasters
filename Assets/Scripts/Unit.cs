using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour
{
    public int maxLevel = 7;
    public string unitName;
    public int unitHealth;
    public int unitMaxHealth;
    public int unitAttack;
    public int unitDefense;
    public float unitSpeed;
    public float unitRange;
    public float unitAttackDelay = 1.5f;
    public float nextAttack;
    public int level = 0;
    public bool isEnemy = false;
    public Color currentLevelColor;
    public Color[] levelColors = new Color[7];
    public enum Status { Idle, Moving, Attacking, Dead };
    public Status currentStatus = Status.Idle;
    public Vector3 lastPosition;
    public GameObject pfProjectile; // arrow
    private Unit _target;
    public Healthbar healthbar;

    private void Awake()
    {
        InitStats();
        InitLevelColors();
        nextAttack = unitAttackDelay;
    }

    void InitStats()
    {
        this.unitMaxHealth = 100;
        this.unitHealth = this.unitMaxHealth;
        this.healthbar.SetMaxHealth(this.unitMaxHealth);
        this.healthbar.SetHealth(this.unitHealth);
        this.unitDefense = 5;
        this.unitAttack = 10;
        this.unitRange = 1;
        this.unitSpeed = 1;
        this.unitName = "Level " + this.level + " Unit";
        this.name = this.unitName;
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
            this.unitName = "Level " + this.level + " Cube Unit";
            this.name = this.unitName;
            this.unitAttack += 5 * this.level;
            this.unitMaxHealth += 10 * this.level;
            this.unitHealth = this.unitMaxHealth;
            this.unitDefense += 2 * this.level;
            this.unitSpeed += .05f * this.level;
            this.unitRange += .2f * this.level;
            this.currentLevelColor = levelColors[this.level];
            this.GetComponent<Renderer>().material.DOColor(this.GetComponent<Unit>().currentLevelColor, .25f);
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

    public IEnumerator TakeDamage(int damage)
    {
        yield return new WaitForSeconds(.8f);
        // if unit defense is higher than damage, unit will not take damage
        this.unitHealth -= damage - unitDefense > 0 ? damage - unitDefense : 0;
        this.healthbar.SetHealth(this.unitHealth);
        if (this.unitHealth <= 0)
        {
            this.gameObject.transform.DOScale(0, .5f);
            this.currentStatus = Status.Dead;
            this.gameObject.SetActive(false);
        }
    }

    public void Attack(Unit target)
    {
        if (target.gameObject.activeSelf)
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
                        StartCoroutine(target.TakeDamage(this.unitAttack));
                        break;
                    }
            }
        }
        else
        {
            this.currentStatus = Status.Idle;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Destroy(collision.gameObject);
        }
    }

    private void ShootProjectile(Unit target)
    {
        // 3 ways to shoot a projectile: using transform, using collider, using raycast (raycast is like instant projectile)
        GameObject projectile = Instantiate(pfProjectile, this.transform.position + Vector3.forward * .75f, Quaternion.identity);
        projectile.transform.LookAt(target.transform);
        projectile.transform.DOMove(target.transform.position, .8f);
        // rb.AddForce(target.transform.localPosition * 100f, ForceMode.Impulse);
    }
}