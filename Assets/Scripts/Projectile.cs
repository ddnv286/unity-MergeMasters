using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _shootDirection;
    public float speed = .8f;

    public void Setup(Vector3 shootDirection)
    {
        this._shootDirection = shootDirection;
        this.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(_shootDirection));
        // make projectile always face direction
        this.transform.rotation = Quaternion.LookRotation(_shootDirection);
    }

    private float GetAngleFromVector(Vector3 direction)
    {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    private void Update()
    {
        transform.position += _shootDirection * speed * Time.deltaTime;
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
