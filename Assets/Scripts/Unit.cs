using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitHealth;
    public int unitAttack;
    public int unitDefense;
    public float unitSpeed;
    public float unitRange;
    public int level = 0;
    public bool isEnemy = false;
    public Color currentLevel;
    public Color[] levelColors = new Color[7];
    public enum Status { Idle, Moving, Attacking, Dead };
    public Status currentStatus = Status.Idle;
    public Vector3 lastPosition;
    private Unit _target;

    private void Awake()
    {
        InitLevelColors();
    }

    void InitStats()
    {
        this.unitHealth = 100;
        this.unitAttack = 10;
        this.unitDefense = 5;
        this.unitSpeed = 1f;
        this.unitRange = 1.5f;
    }

    void UpdateStats()
    {
        this.unitHealth += 100 * (this.level + 1);
        this.unitAttack += 10 * (this.level + 1);
        this.unitDefense += 5 * (this.level + 1);
        // speed and range remain unchanged
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
            {
                if (Vector3.Distance(this.transform.position, _target.transform.position) > this.unitRange)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, _target.gameObject.transform.position, this.unitSpeed * Time.deltaTime);
                }
                else
                {
                    this.currentStatus = Status.Idle;
                }
            }
        }
        else if (this.currentStatus == Status.Attacking)
        {
            Attack(_target);
        }
    }

    public void Attack(Unit target)
    {
        this.currentStatus = Status.Attacking;
        target.unitHealth -= this.unitAttack - target.unitDefense;
        Debug.Log(target.name + " remaining HP: " + target.unitHealth);
        if (target.unitHealth <= 0)
        {
            target.currentStatus = Status.Dead;
            Debug.Log(target.name + " is " + target.currentStatus);
            target.gameObject.transform.DOScale(0, .5f);
            Destroy(target);
        }
        this.currentStatus = Status.Idle;
    }

    public void MoveToTarget(Unit target)
    {
        _target = target;
        this.currentStatus = Status.Moving;
    }
}