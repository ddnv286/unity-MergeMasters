using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject selectedUnit;
    public Vector3[,] coordinates = new Vector3[5, 3];
    static float Y_POS = 1.14f;

    void Start()
    {
        InitCoordinates();
    }

    void InitCoordinates()
    {
        coordinates[0, 0] = new Vector3(-1.8f, Y_POS, -2.55f);
        coordinates[1, 0] = new Vector3(-0.9f, Y_POS, -2.55f);
        coordinates[2, 0] = new Vector3(0, Y_POS, -2.55f);
        coordinates[3, 0] = new Vector3(0.9f, Y_POS, -2.55f);
        coordinates[4, 0] = new Vector3(1.8f, Y_POS, -2.55f);

        coordinates[0, 1] = new Vector3(-1.8f, Y_POS, -1.65f);
        coordinates[1, 1] = new Vector3(-0.9f, Y_POS, -1.65f);
        coordinates[2, 1] = new Vector3(0, Y_POS, -1.65f);
        coordinates[3, 1] = new Vector3(0.9f, Y_POS, -1.65f);
        coordinates[4, 1] = new Vector3(1.8f, Y_POS, -1.65f);

        coordinates[0, 2] = new Vector3(-1.8f, Y_POS, -0.75f);
        coordinates[1, 2] = new Vector3(-0.9f, Y_POS, -0.75f);
        coordinates[2, 2] = new Vector3(0, Y_POS, -0.75f);
        coordinates[3, 2] = new Vector3(0.9f, Y_POS, -0.75f);
        coordinates[4, 2] = new Vector3(1.8f, Y_POS, -0.75f);
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
                selectedUnit.gameObject.transform.position = new Vector3(worldPosition.x, Y_POS, worldPosition.z);
                selectedUnit = null;
                Cursor.visible = true;
            }
        }
        if (selectedUnit != null)
        {
            // calculate z position based on mouse position and camera points to unit's z position
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedUnit.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedUnit.gameObject.transform.position = new Vector3(worldPosition.x, 1.3f, worldPosition.z);
        }
    }

    void SnapToGrid(GameObject unit, Vector3 position)
    {
        
    }
}
