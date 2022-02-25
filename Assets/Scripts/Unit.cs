using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitHealth;
    public int unitAttack;
    public int unitDefense;
    public int unitSpeed;
    public int unitRange;
    public int level = 0;
    public Color currentLevel;
    public Color[] levelColors = new Color[7];

    public Vector3 lastPosition;

    private void Awake()
    {
        InitLevelColors();
    }   

    void InitLevelColors ()
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

    public void LevelUp ()
    {
        if (this.level + 1 < levelColors.Length)
        {
            this.level++;
            this.unitName = levelColors[this.level].ToString() + " Cube Unit";
            this.currentLevel = levelColors[this.level];
            this.GetComponent<Renderer>().material.DOColor(this.GetComponent<Unit>().currentLevel, .25f);
        } else
        {
            Debug.Log("Level Maxed.");
            this.gameObject.transform.DOLocalMove(this.lastPosition, .25f);
        }
    }
}