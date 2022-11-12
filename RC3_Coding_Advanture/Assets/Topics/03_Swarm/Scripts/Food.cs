using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float amount = 100;

    public Vector3 velocity;

    public void Consume(float consume)
    {
        amount -= consume;
    }

    public bool IsEmpty
    {
        get => amount <= 0;
    }

    public void UpdatePosition(Vector3 boundXYZ,float delta)
    {
        var newPos = transform.position + velocity * delta;

        newPos.x = Mathf.Clamp(newPos.x,0, boundXYZ.x);
        newPos.y = Mathf.Clamp(newPos.y, 0, boundXYZ.y);
        newPos.z = Mathf.Clamp(newPos.z, 0, boundXYZ.z);

        transform.position = newPos;

        if (Time.frameCount % 1000 == 0)
        {
            UpdateVelocity();
        }

    }

    public void UpdateVelocity()
    {
        velocity = Random.onUnitSphere;
    }
 
}
