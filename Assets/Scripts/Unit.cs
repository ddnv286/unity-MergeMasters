using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

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
    private float _unitAttackDelay = 1.5f;
    private float _nextAttack;
    public int level = 0;
    public bool isEnemy = false;
    public Color currentLevelColor;
    public Color[] levelColors = new Color[7];
    public Transform[] unitUpgrades = new Transform[7];
    public enum Status { Idle, Moving, Attacking, Dead };
    public Status currentStatus = Status.Idle;
    public Vector3 lastPosition;
    public GameObject pfProjectile; // arrow
    public GameObject floatingText;
    private Unit _target;
    public Healthbar healthbar;
    protected GameManager _manager;

    protected virtual void Awake()
    {
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        InitStats();
        InitLevelColors();
        _nextAttack = _unitAttackDelay;
    }

    protected void InitStats()
    {
        this.unitMaxHealth = 100;
        this.unitHealth = this.unitMaxHealth;
        this.unitDefense = 5;
        this.unitAttack = 10;
        this.unitRange = 1;
        this.unitSpeed = 1;
        this.unitName = "Level " + this.level + " Unit";
        //this.name = this.unitName;
        this.healthbar.SetMaxHealth(this.unitMaxHealth);
        this.healthbar.SetHealth(this.unitHealth);
    }

    protected void InitLevelColors()
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
            // this.unitRange += .2f * this.level;
            // TODO: instead of changing material color based on level, instantiate new unit prefab based on level
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
            if (_nextAttack > 0)
            {
                _nextAttack -= Time.deltaTime;
            }
            else if (_nextAttack <= 0)
            {
                this.currentStatus = Status.Attacking;
                Debug.Log(_target.name + "'s remaining HP: " + _target.unitHealth);
                Attack(_target);
                _nextAttack = _unitAttackDelay;
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
        // only add gold on enemy hit, not ally hit
        if (this.isEnemy)
        {
            _manager.AddGold(this.unitMaxHealth);
            var _floatingText = Instantiate(floatingText, this.transform);
            _floatingText.GetComponent<TextMeshPro>().SetText("+" + this.unitMaxHealth);
            _floatingText.GetComponent<TextMeshPro>().transform.Rotate(Vector3.up*-180);
            // TODO: calculating max height based on bounds, then set text to desired height before applying animation
            _floatingText.transform.position = new Vector3(_floatingText.transform.position.x, 1f, _floatingText.transform.position.z);
            _floatingText.transform.DOMoveY(1.5f, .5f);
            _floatingText.GetComponent<TextMeshPro>().DOFade(0, .5f);
            Destroy(_floatingText, 1f);
        }
        if (this.unitHealth <= 0)
        {
            this.currentStatus = Status.Dead;
            this.gameObject.SetActive(false);
        }
    }

    protected virtual void Attack(Unit target)
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
                        // ShootProjectile(target);
                        StartCoroutine(target.TakeDamage(this.unitAttack));
                        break;
                    }
            }
        }
        else if (_manager.remainingEnemies.Count != 0)
        {
            Unit nearest = _manager.NearestEnemy(this);
            MoveToTarget(nearest);
        }
        else
        {
            this.currentStatus = Status.Idle;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        // this doesn't work for some reason
        // both unit and projectile have box collider, unit has triggered rigidbody (?)
        if (collision.gameObject.tag == "Projectile")
        {
            Destroy(collision.gameObject);
        }
    }

    public void ShootProjectile(Unit target)
    {
        // 3 ways to shoot a projectile: using transform, using collider, using raycast (raycast is like instant projectile)
        GameObject projectile = Instantiate(pfProjectile, this.transform.position + Vector3.forward * .75f, Quaternion.identity);
        // rotate the projectile to make it always facing the target
        projectile.transform.LookAt(target.transform);
        projectile.transform.DOMove(target.transform.position, .8f);
    }
}