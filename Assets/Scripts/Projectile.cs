using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    
    void Awake () {
        this.gameObject.tag = "Projectile";
        Destroy(this.gameObject, .8f);
    }
}
