using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GridCell[,] grid = new GridCell[5, 3];
    [SerializeField] int _gridHeight = 3;
    [SerializeField] int _gridWidth = 5;
    [SerializeField] Transform _grid;
    public Transform prefabGridCell;

    private void Awake()
    {
        InitiateGridcell();
    }

    private void Update()
    {
        checkOccupied();
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

    // if a unit standing on a gridcell is hit by the raycast, then that gridcell is occupied
    // TODO: -fix- raycast did not detect any unit standing on corresponding cell
    public void checkOccupied()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(grid[x, z].transform.position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Unit"))
                    {
                        grid[x, z].isOccupied = true;
                        Debug.Log(grid[x, z] + " is occupied.");
                    }
                }
            }
        }
    }
}
