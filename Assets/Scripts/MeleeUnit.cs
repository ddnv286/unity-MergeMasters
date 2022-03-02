using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    // Start is called before the first frame update
    void Awake ()
    {
        base.InitStats();
        base.InitLevelColors();
        for (int i = 0; i < this.levelColors.Length; i++) {
            this.levelColors[i].a = 1;
        }
        this.unitRange = 1;
    }
}
