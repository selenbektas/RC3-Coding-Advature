using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmAgent : MonoBehaviour
{

    public Vector3 position;

    public Vector3 velocity;

    public Vector3 acceleration;

  

    public float radius;

    public void Init(Vector3 position,float radius)
    {
        this.position = position;
        this.radius = radius;

        transform.position = position;
        velocity = Random.onUnitSphere;
    }

    public float DistanceTo(SwarmAgent other)
    {
        return Vector3.Distance(position, other.position);
    }

    public void UpdatePosition(float delta)
    {
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, 2.0f);
        position += velocity*delta;
        acceleration *= 0;
        transform.position = position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity),delta);
    }

    public void CalculateBorders(float sizeX,float sizeY,float sizeZ)
    {
        if (position.x < radius)
        {
            velocity.x = -velocity.x;
            position.x = radius;
        }
            
        if (position.y < radius)
        {
            velocity.y = -velocity.y;
            position.y = radius;
        }
        
        if (position.z < radius)
        {
            velocity.z = -velocity.z;
            position.z = radius;
        }
            
        if (position.x > sizeX - radius)
        {
            velocity.x = -velocity.x;
            position.x = sizeX - radius;
        }
          
        if (position.y > sizeY - radius)
        {
            velocity.y = -velocity.y;
            position.y = sizeY - radius;
        }
           
        if (position.z > sizeZ - radius)
        {
            velocity.z = -velocity.z;
            position.z = sizeZ - radius;
        }
           
    }

}

