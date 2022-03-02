using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Awake()
    {
        this.gameObject.tag = "Projectile";
        Destroy(this.gameObject, .8f);
    }
}
