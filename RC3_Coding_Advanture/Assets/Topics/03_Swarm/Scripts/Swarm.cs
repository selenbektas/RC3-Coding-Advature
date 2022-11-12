using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// reference to swarm intelligence https://en.wikipedia.org/wiki/Swarm_intelligence
/// </summary>
public class Swarm : MonoBehaviour
{
    public SwarmAgent agentPrefab;
    public SwarmAgent predatorPrefab;
    public Food foodPrefab;


    public int agentCount = 100;
    public int predatorCount = 5;

    public float foodDropRate = 15f;

    public float agentRadius = 1.0f;

    public float agentMaxDistance = 5.0f;

    public float stiffness = 10;

    public Vector3 boundsXYZ = new Vector3(3, 3, 3);


    List<Food> foods;
    List<SwarmAgent> agents;
    List<SwarmAgent> predators;

    // Start is called before the first frame update
    void Start()
    {
        PopulateAgents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBehaviors(agentRadius, agentMaxDistance);
        UpdateAgents();

        if (Time.time % foodDropRate < 0.01f)
        {
            DropFood();
        }

    }

    void DropFood()
    {
        var food = Instantiate(foodPrefab, transform);
        float x = Random.Range(0, boundsXYZ.x);
        float y = Random.Range(0, boundsXYZ.y);
        float z = Random.Range(0, boundsXYZ.z);

        food.transform.position = new Vector3(x, y, z);
        food.UpdateVelocity();
        foods.Add(food);
    }

    void PopulateAgents()
    {
        agents = new List<SwarmAgent>();
        predators = new List<SwarmAgent>();
        foods = new List<Food>();
        for (int i = 0; i < agentCount; i++)
        {
            var agt = Instantiate(agentPrefab, transform);

            float x = Random.Range(0, boundsXYZ.x);
            float y = Random.Range(0, boundsXYZ.y);
            float z = Random.Range(0, boundsXYZ.z);

            agt.Init(new Vector3(x, y, z), agentRadius);
            agents.Add(agt);
        }

        for (int i = 0; i < predatorCount; i++)
        {
            var prd = Instantiate(predatorPrefab, transform);

            float x = Random.Range(0, boundsXYZ.x);
            float y = Random.Range(0, boundsXYZ.y);
            float z = Random.Range(0, boundsXYZ.z);

            prd.Init(new Vector3(x, y, z), agentRadius);
            predators.Add(prd);
        }
    }

    public void UpdateAgents()
    {
        foreach (SwarmAgent a in agents)
        {
            a.UpdatePosition(Time.deltaTime * stiffness);
            a.CalculateBorders(boundsXYZ.x, boundsXYZ.y, boundsXYZ.z);
        }

        foreach (SwarmAgent a in predators)
        {
            a.UpdatePosition(Time.deltaTime * stiffness);
            a.CalculateBorders(boundsXYZ.x, boundsXYZ.y, boundsXYZ.z);
        }
    }


    public void UpdateBehaviors(float minDist, float maxDist)
    {
        int agentCount = agents.Count;

        // Calculate agents behaviors
        for (int i = 0; i < agentCount; i++)
        {
            var agent = agents[i];

           

            // setup 3 vectors for seperation, cohesion, and alignment
            Vector3 seperate = new Vector3(0, 0, 0);
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 align = new Vector3(0, 0, 0);

            float seperateCount = 0;
            float cohesionCount = 0;

            //calculate the behavior relates to the foods, which are attracting all agents
            for (int k = 0; k < foods.Count; k++)
            {
                var food = foods[k];
                float d = Vector3.Distance(agent.position, food.transform.position);

                Vector3 dir = food.transform.position - agent.position;

                if (d < minDist)
                {
                    dir = -dir;
                    food.Consume(Random.Range(0, 0.3f));
                }


                if ( d < maxDist*2)
                {
                    //add food position to the center, (from vector A to vector B = B-A)
                    center += food.transform.position;
                    //add direction towards the food to the alignment factor
                    align += dir;
                    cohesionCount++;
                }
            }

            // behaviors between predators, which all agents should avoid and keep away from
            for (int m = 0; m < predators.Count; m++)
            {
                var pdt = predators[m];
                float d = agent.DistanceTo(pdt);

                if (d < maxDist*1.5f)
                {
                    //direction from predator to agent, (from vector A to vector B = B-A)
                    var dir = agent.position - pdt.position;
                    seperate += dir / (d * d);
                    align -= dir;
                    seperateCount++;
                    
                }
            }


            // behaviors between other agents, which agents should keep between min and max distance, stay together while not getting too close
            for (int j = i + 1; j < agentCount; j++)
            {

                var other = agents[j];
                float dist = agent.DistanceTo(other);

                // seperate when distance too close
                if (dist < minDist)
                {
                    Vector3 dir = agent.position - other.position;

                    dir.Normalize();
                    seperate += (dir / dist);
                    seperateCount++;
                }

                // keep together and move at similar direction with other agents in range, but ignore far away agents
                if (dist >= minDist && dist < maxDist)
                {
                    center += other.position;
                    align += other.velocity;
                    cohesionCount++;
                }
            }

            if (seperateCount > 0)
            {
                seperate /= (seperateCount);
                seperate.Normalize();
                seperate *= (2);

                seperate -= agent.velocity;

                seperate = Vector3.ClampMagnitude(seperate, 0.1f) * 2;
                agent.acceleration += seperate;
            }

            if (cohesionCount > 0)
            {
                align /= cohesionCount;
                align.Normalize();
                align *= 2;
                var steer0 = align - agent.velocity;

                steer0 = Vector3.ClampMagnitude(steer0, 0.03f);
                agent.acceleration += (steer0);

                center /= (cohesionCount);
                var dir = center - agent.position;

                dir.Normalize();
                dir *= 2;

                var steer = dir - agent.velocity;

                steer = Vector3.ClampMagnitude(steer, 0.03f);

                agent.acceleration += (steer);
            }
        }

        for (int i = 0; i < predatorCount; i++)
        {
            var predator = predators[i];
            Vector3 seperate = new Vector3(0, 0, 0);
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 align = new Vector3(0, 0, 0);

            float seperateCount = 0;
            float cohesionCount = 0;

            for (int j = i + 1; j < predatorCount; j++)
            {

                var other = predators[j];
                float d = predator.DistanceTo(other);

                // seperate when distance too close
                if (d < minDist)
                {
                    Vector3 dir = predator.position - other.position;

                    dir.Normalize();
                    seperate += (dir / d);
                    seperateCount++;
                }

                // keep together and move at similar direction with other agents in range, but ignore far away agents
                if (d >= minDist && d < maxDist)
                {
                    center += other.position;
                    align += other.velocity;
                    cohesionCount++;
                }
            }


            for (int j = 0; j < agentCount; j++)
            {

                var other = agents[j];
                float distance = predator.DistanceTo(other);


                if (distance < maxDist*2)
                {
                    center += other.position;
                    align += other.velocity;
                    cohesionCount++;
                }
            }

            if (seperateCount > 0)
            {
                seperate /= (seperateCount);
                seperate.Normalize();
                seperate *= (2);

                seperate -= predator.velocity;

                seperate = Vector3.ClampMagnitude(seperate, 0.1f) * 2;
                predator.acceleration += seperate;
            }

            if (cohesionCount < 1)
            {
                align = Random.onUnitSphere;
                center = predator.position + (predator.velocity + align) * 0.5f * maxDist;
                cohesionCount = 1;
            }


            if (cohesionCount > 0)
            {
                align /= cohesionCount;
                align.Normalize();
                align *= 2;
                center /= (cohesionCount);

                var steer0 = align - predator.velocity;

                steer0 = Vector3.ClampMagnitude(steer0, 0.03f);
                predator.acceleration += (steer0);


                var dir = center - predator.position;

                dir.Normalize();
                dir *= 2;

                var steer = dir - predator.velocity;
                steer = Vector3.ClampMagnitude(steer, 0.03f);
                predator.acceleration += (steer);
            }
        }


        for (int i = 0; i < foods.Count; i++)
        {
            var food = foods[i];
            food.UpdatePosition(boundsXYZ, Time.deltaTime * stiffness);
            if (food.IsEmpty)
            {
                DestroyImmediate(food.gameObject);
            }
        }

        foods.RemoveAll(f => f == null);


    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(0.5f * boundsXYZ, boundsXYZ);
    }
}
