using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _shootDirection;
    public float speed = .8f;

    public void Setup(Vector3 shootDirection)
    {
        this._shootDirection = shootDirection;
        // make projectile always face direction
        this.transform.rotation = Quaternion.LookRotation(_shootDirection);
    }

    private void Update()
    {
        // using transform
        // transform.position += _shootDirection * speed * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = _shootDirection * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // destroy the projectile whenever it hit a unit (regardless of unit side)
        if (collision.collider.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }
}
