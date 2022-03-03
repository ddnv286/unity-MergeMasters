using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < this.levelColors.Length; i++) {
            this.levelColors[i].a = .65f;
        }
        this.unitRange = 1;
    }
}
