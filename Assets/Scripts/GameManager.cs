using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public float goldAmount;
    public Text goldDisplay;
    public Text meleeCostDisplay;
    public Text rangedCostDisplay;
    private int _meleeCost = 0;
    private int _rangedCost = 0;
    public GameObject selectedUnit;
    public Transform spawnedUnit; // prefab to initiate
    public List<Unit> remainingEnemies = new List<Unit>(); // contains remaining enemies
    public List<Unit> remainingAllies = new List<Unit>(); // contains remaining allies
    public GameObject raycastedObject; // check if selectedUnit is overlapped with raycastedObject
    static float Y_POS = 1.14f;
    [SerializeField] int _gridHeight = 3;
    [SerializeField] int _gridWidth = 5;
    [SerializeField] Transform _allySide;
    public Transform prefabGridCell;
    [SerializeField] Transform _grid;
    public GridCell[,] grid = new GridCell[5, 3];
    [SerializeField] Formation _formation;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        this.goldAmount = Mathf.Infinity;
        InitiateGridcell();
        UpdateUnitList();
        InitFormation();
        UpdateCostDisplay();
        UpdateGoldDisplay();
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
        CheckWinCondition();
    }

    public void InitFormation()
    {
        // WIP
        this._formation = new Formation();
        this._formation.allies = new Unit[remainingAllies.Count];
        this._formation.allyCoordinates = new Vector2Int[remainingAllies.Count];
        for (int i = 0; i < remainingAllies.Count; i++)
        {
            this._formation.allies[i] = remainingAllies[i];
            this._formation.allyCoordinates[i] = new Vector2Int(Mathf.RoundToInt(remainingAllies[i].transform.localPosition.x), Mathf.RoundToInt(remainingAllies[i].transform.localPosition.z));
        }
        Debug.Log("Formation initiated.");
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
                // if there is other unit with same level in that position, log the name of the unit out then merge them into new unit
                // move the selectedUnit back to it last position in case the level is maxed out
                if (unit.GetComponent<Unit>().level + 1 == unit.GetComponent<Unit>().maxLevel)
                {
                    unit.gameObject.transform.DOLocalMove(unit.GetComponent<Unit>().lastPosition, .25f);
                }
                else
                {
                    Debug.Log(hit.collider.gameObject.name);
                    Merge(unit.gameObject, hit.collider.gameObject);
                }
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

    void UpdateCostDisplay()
    {
        this.meleeCostDisplay.text = "Spawn Melee: " + _meleeCost;
        this.rangedCostDisplay.text = "Spawn Ranged: " + _rangedCost;
    }

    public void SpawnMelee()
    {
        if (goldAmount >= _meleeCost)
        {
            goldAmount -= _meleeCost;
            _meleeCost += 3000;
            Spawn("Melee");
        }
    }

    public void SpawnRanged()
    {
        if (goldAmount >= _rangedCost)
        {
            goldAmount -= _rangedCost;
            _rangedCost += 2000;
            Spawn("Ranged");
        }
    }

    public void Spawn(string type)
    {
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                if (!this.grid[i, j].GetComponent<GridCell>().isOccupied)
                {
                    if (type == "Melee")
                    {
                        Transform unit = Instantiate(spawnedUnit, _allySide);
                        unit.transform.localPosition = new Vector3(i, 0, j);
                        unit.transform.localScale = Vector3.one * .75f;
                        unit.name = "Melee Unit" + i + j;
                        ToggleOccupied(i, j);
                        UpdateGoldDisplay();
                        UpdateCostDisplay();
                        return;
                    }
                    else if (type == "Ranged")
                    {
                        Transform unit = Instantiate(spawnedUnit, _allySide);
                        unit.transform.localPosition = new Vector3(i, 0, j);
                        unit.transform.localScale = Vector3.one * .75f;
                        unit.name = "Ranged Unit" + i + j;
                        ToggleOccupied(i, j);
                        UpdateGoldDisplay();
                        UpdateCostDisplay();
                        return;
                    }
                }
            }
        }
    }

    public void AddGold(int amount)
    {
        this.goldAmount += amount;
        this.goldDisplay.text = "Gold: " + goldAmount;
    }

    public void UpdateGoldDisplay()
    {
        this.goldDisplay.text = "Gold: " + goldAmount;
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
        if (remainingAllies.Count != 0)
        {
            // hide the gridlines when Move button is pressed.
            _grid.transform.DOScale(0, 0);
            foreach (Unit ally in remainingAllies)
            {
                Unit nearest = NearestEnemy(ally);
                ally.MoveToTarget(nearest);
            }
        }
    }

    void UpdateUnitList()
    {
        // prevent adding both list to infinity
        remainingAllies.Clear();
        remainingEnemies.Clear();
        Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            if (unit.isEnemy)
            {
                remainingEnemies.Add(unit);
            }
            else
            {
                remainingAllies.Add(unit);
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
            float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    public void CheckWinCondition()
    {
        if (remainingEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated");
        }
    }
}

public class Formation
{
    public Unit[] allies;
    public Vector2Int[] allyCoordinates;
}