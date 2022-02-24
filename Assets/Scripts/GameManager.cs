using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject selectedUnit;
    public Transform spawnedUnit;
    public List<Unit> units = new List<Unit>(); // represents the level of unit when merged (WIP)
    public GameObject raycastedObject; // check if selectedUnit is overlapped with raycastedObject
    static float Y_POS = 1.14f;
    [SerializeField] int _gridHeight = 3;
    [SerializeField] int _gridWidth = 5;
    [SerializeField] Transform _allySide;
    public GridManager gridManager;
    GridCell[,] grid;

    private void Awake()
    {
        this.grid = gridManager.grid;
    }

    void Update()
    {
        //UpdateGrid();
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
                checkCollide(selectedUnit.transform);
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

    // the idea is to move the selectedUnit to IgnoreRaycast layer, then raycast to the selectedUnit's position to check if there is any other Unit in that position
    void checkCollide (Transform unit) {
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
        }
        else
        {
            unit.gameObject.layer = 0;
            return;
        }
        // if the selectedUnit was not merged
        unit.gameObject.layer = 0;
    }

    public void Spawn ()
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
                    return;
                }
            }
        }
    }

    // keep unit1 as leveled up unit, scale unit2 down to 0, update unit1 level and material color
    void Merge (GameObject unit1, GameObject unit2)
    {
        unit2.transform.DOScale(0f, 0.25f);
        Destroy(unit2);
        unit1.GetComponent<Unit>().LevelUp();
        Debug.Log("Level up: " + unit1.GetComponent<Unit>().level);
    }
}
