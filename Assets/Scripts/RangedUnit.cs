using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RangedUnit : Unit
{
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < this.levelColors.Length; i++)
        {
            this.levelColors[i].a = 1f;
        }
        this.unitRange = 5;
    }

    protected override void Attack(Unit target)
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
}
