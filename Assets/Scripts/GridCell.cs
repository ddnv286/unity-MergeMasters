using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool isOccupied;
    public Unit occupiedUnit;

    private void Awake()
    {
        
    }

    public GridCell ()
    {
        isOccupied = false;
    }
}
