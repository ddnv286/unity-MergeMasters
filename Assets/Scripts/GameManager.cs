using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject selectedUnit;
    public Transform prefabGridCell;
    static float Y_POS = 1.14f;
    [SerializeField] int gridHeight = 3;
    [SerializeField] int gridWidth = 5;

    void Start () {
        InitiateGridcell();
    }

    void InitiateGridcell () {
        for (int i = 0; i < gridWidth; i++) {
            for (int j = 0; j < gridHeight; j++) {
                Vector3 worldPos = new Vector3(i, 0.77f, j);
                Transform obj = Instantiate(prefabGridCell, worldPos, Quaternion.identity);
                // TODO: fixing offset world position
                obj.transform.position = new Vector3 (-2f + i, 0.77f, -2.5f + j);
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.name = "Cell" + i + j;
            }
        }
    }

    void Update()
    {
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
                selectedUnit.gameObject.transform.position = new Vector3(Mathf.RoundToInt(worldPosition.x), Y_POS, Mathf.RoundToInt(worldPosition.z)+0.5f);
                CheckOverlapped(selectedUnit.GetComponent<Unit>());
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

    private void OnMouseOver () {
        // TODO: Drag unit to slot
    }

    private void OnMouseExit () {

    }
    
    void CheckOverlapped (Unit unit) {
        BoxCollider collider = unit.GetComponent<BoxCollider>();
        Collider[] otherColliders = Physics.OverlapBox(unit.transform.position, transform.localScale/2, Quaternion.identity, LayerMask.GetMask("Unit"));
        if (otherColliders.Length > 0) {
            Debug.Log("Overlapped");
            Merge(unit, otherColliders[0].GetComponent<Unit>());
        }
    }

    private void Merge (Unit unit1, Unit unit2) {
        // check if unit1 and unit2 are the same
        if (unit1.gameObject.name == unit2.gameObject.name) {
            unit1.gameObject.GetComponent<Material>().color = Color.yellow;            unit2.transform.DOScale(0f, .5f);
        }
    }
}
