using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject selectedUnit;
    public Transform spawnedUnit; // prefab to initiate
    public Unit[,] remainingEnemies = new Unit[5, 3]; // contains remaining enemies
    public Unit[,] remainingAllies = new Unit[5, 3]; // contains remaining allies
    public GameObject raycastedObject; // check if selectedUnit is overlapped with raycastedObject
    static float Y_POS = 1.14f;
    [SerializeField] int _gridHeight = 3;
    [SerializeField] int _gridWidth = 5;
    [SerializeField] Transform _allySide;
    public Transform prefabGridCell;
    [SerializeField] Transform _grid;
    public GridCell[,] grid = new GridCell[5, 3];

    private void Awake()
    {
        InitiateGridcell();
    }

    void Update()
    {
        UpdateUnitList();
        CheckOccupied();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedUnit == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.CompareTag("Unit"))
                    {
                        return;
                    }
                    selectedUnit = hit.collider.gameObject;
                    selectedUnit.GetComponent<Unit>().lastPosition = selectedUnit.transform.localPosition;
                    Cursor.visible = false;
                }
            }
            else
            {
                // calculate z position based on mouse position and camera points to unit's z position
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedUnit.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

                // TODO: fixing 0.5f offset (0.5f offset is temporary for easier snapping to slot)
                selectedUnit.gameObject.transform.position = new Vector3(Mathf.RoundToInt(worldPosition.x), Y_POS, Mathf.RoundToInt(worldPosition.z) + 0.5f);
                // processing outbound position
                if (isValidPosition(selectedUnit.gameObject.transform.localPosition.x, selectedUnit.gameObject.transform.localPosition.z))
                {
                    // toggle occupied statuses
                    ToggleOccupied(selectedUnit.GetComponent<Unit>().lastPosition.x, selectedUnit.GetComponent<Unit>().lastPosition.z);
                    ToggleOccupied(selectedUnit.transform.localPosition.x, selectedUnit.transform.localPosition.z);
                    CheckCollide(selectedUnit.transform);
                }
                else
                {
                    selectedUnit.gameObject.transform.DOLocalMove(selectedUnit.GetComponent<Unit>().lastPosition, .25f);
                }
                // check if placed unit is collided with another unit to merge both units
                selectedUnit = null;
                Cursor.visible = true;
            }
        }
        if (selectedUnit != null)
        {
            // calculate z position based on mouse position and camera points to unit's z position
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedUnit.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedUnit.gameObject.transform.position = new Vector3(worldPosition.x, 1.5f, worldPosition.z);
        }
    }

    bool isValidPosition(float x, float z)
    {
        return (x >= 0 && x < _gridWidth && z >= 0 && z < _gridHeight);
    }

    void InitiateGridcell()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int z = 0; z < _gridHeight; z++)
            {
                Transform obj = Instantiate(prefabGridCell, _grid);
                obj.transform.localPosition = new Vector3(x, 0, z);
                obj.transform.localScale = Vector3.one;
                obj.gameObject.layer = 2;
                obj.name = "Cell" + x + z;
                grid[x, z] = obj.GetComponent<GridCell>();
            }
        }
    }

    // check if a tile is occupied by a unit
    public void CheckOccupied()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                // .25f is chosen abitrarily
                Vector3 pos = grid[x, z].transform.position;
                var hitColliders = Physics.OverlapSphere(grid[x, z].gameObject.transform.position, .25f);
                if (hitColliders.Length > 0)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag("Unit"))
                        {
                            // Debug.Log(grid[x, z] + " is occupied");
                            grid[x, z].isOccupied = true;
                        }
                    }
                }
            }
        }
    }

    // the idea is to move the selectedUnit to IgnoreRaycast layer, then raycast to the selectedUnit's position to check if there is any other Unit in that position
    void CheckCollide(Transform unit)
    {
        // move the selectedUnit to IgnoreRaycast layer
        unit.gameObject.layer = 2;
        // raycast to the selectedUnit's position to check if there is any other Unit in that position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Unit") && unit.GetComponent<Unit>().level == hit.collider.GetComponent<Unit>().level)
            {
                // if there is other unit in that position, log the name of the unit out then merge them into new unit
                Debug.Log(hit.collider.gameObject.name);
                Merge(unit.gameObject, hit.collider.gameObject);
            }
            else if (hit.collider.tag.Equals("Unit") && unit.GetComponent<Unit>().level != hit.collider.GetComponent<Unit>().level)
            {
                unit.transform.DOLocalMove(unit.GetComponent<Unit>().lastPosition, .25f);
            }
        }
        else
        {
            unit.gameObject.layer = 0;
            return;
        }
        // if the selectedUnit was not merged
        unit.gameObject.layer = 0;
    }

    public void Spawn()
    {
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                if (!this.grid[i, j].GetComponent<GridCell>().isOccupied)
                {
                    Transform unit = Instantiate(spawnedUnit, _allySide);
                    unit.transform.localPosition = new Vector3(i, 0, j);
                    unit.transform.localScale = Vector3.one * .75f;
                    unit.name = "Unit" + i + j;
                    ToggleOccupied(i, j);
                    return;
                }
            }
        }
    }

    void ToggleOccupied(float x, float z)
    {
        grid[Mathf.RoundToInt(x), Mathf.RoundToInt(z)].isOccupied = !grid[Mathf.RoundToInt(x), Mathf.RoundToInt(z)].isOccupied;
        Debug.Log("Cell" + Mathf.RoundToInt(x) + Mathf.RoundToInt(z) + " occupied: " + grid[Mathf.RoundToInt(x), Mathf.RoundToInt(z)].isOccupied);
    }

    // keep unit1 as leveled up unit, scale unit2 down to 0 then destroy it, update unit1 level and its material color
    void Merge(GameObject unit1, GameObject unit2)
    {
        unit2.transform.DOScale(0f, 0.25f);
        ToggleOccupied(unit2.transform.localPosition.x, unit2.transform.localPosition.z);
        Destroy(unit2);
        unit1.GetComponent<Unit>().LevelUp();
        Debug.Log("Level up: " + unit1.GetComponent<Unit>().level);
    }

    public void Move()
    {
        Unit[] units = GameObject.FindObjectsOfType<Unit>();
        Unit allyUnit = null;
        Unit enemyUnit = null;
        foreach (Unit unit in units)
        {
            if (unit.isEnemy)
            {
                enemyUnit = unit;
            }
            else
            {
                allyUnit = unit;
            }
        }
        if (allyUnit != null && enemyUnit != null)
        {
            allyUnit.MoveToTarget(NearestEnemy(allyUnit));
        }
    }

    void UpdateUnitList()
    {
        // bug: as being called in Update() function, GameManager keeps adding existed units to their respective lists to infinity, lead to the issue that ally cannot find the nearest enemy
        Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            int x = Mathf.RoundToInt(unit.transform.localPosition.x);
            int z = Mathf.RoundToInt(unit.transform.localPosition.z);
            if (unit.isEnemy)
            {
                remainingEnemies[x, z] = unit;
            }
            else
            {
                remainingAllies[x, z] = unit;
                remainingAllies[x, z].currentStatus = Unit.Status.Idle;
            }
        }
    }

    // returns nearest enemy
    public Unit NearestEnemy(Unit unit)
    {
        Unit nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        foreach (Unit enemy in remainingEnemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }
        return nearestEnemy;
    }
}
