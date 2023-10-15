using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

/* 
 * Inputs
 * Distance of 9 rays
 * Ray sees prey
 * Ray sees pred
 * Ray sees neither
 * Health
 * tillBirth
 * 
 * Outputs
 * Angular velocity -1 to 1 -> -180 to +180
 * Speed
 * Attack (Predator only)
 * 
 */

public class Creature : MonoBehaviour
{

    public float health;
    public float tillBirth;
    public float mutability;
    public bool noParent;
    public float lifespanInSec;

    public float updatePeriod;

    public float attackCooldown;
    public float lastAttack;

    public GameObject prefab;

    public Rigidbody2D body;
    public CircleCollider2D col;
    public World world;

    protected NeuralNetwork net;
    protected float timer;
    protected RaycastHit2D[] vision;
    public float[] inputVector, outputVector;
    public bool initialized;

    protected float viewDistance;
    protected int numOfViewRays;
    protected int fov;


    public enum type
    {
        prey,
        predator
    }

    public NeuralNetwork getNet()
    {
        return net;
    }

    public Rigidbody2D getBody()
    {
        return body;
    }

    protected void buildLayers()
    {
        net.addLayer(new Layer(numOfViewRays * 4 + 2, 2, Layer.activationFunctions.Linear));
        //net.addLayer(new Layer(numOfViewRays * 4 + 2, 5, Layer.activationFunctions.TanH));
        //net.addLayer(new Layer(5, 2, Layer.activationFunctions.Linear));
    }

    protected void die()
    {
        if (this.GetType() == typeof(Predator))
        {
            //Debug.Log("Predator dying " + gameObject.name);
            world.predAlive--;
        }
        if (this.GetType() == typeof(Prey))
        {
            //Debug.Log("Prey dying " + gameObject.name);
            world.preyAlive--;
        }


        Destroy(gameObject);
    }

    protected void look()
    {
        raycast();

        for (int i = 0; i < numOfViewRays; i++)
        {
            int inputPos = i * 4;

            if (vision[i].collider != null)
            {

                inputVector[inputPos] = vision[i].distance / viewDistance;

                switch (vision[i].collider.gameObject.tag)
                {
                    case "Prey":
                        inputVector[inputPos + 1] = 1f;
                        inputVector[inputPos + 2] = 0f;
                        inputVector[inputPos + 3] = 0f;
                        break;
                    case "Predator":
                        inputVector[inputPos + 1] = 0f;
                        inputVector[inputPos + 2] = 1f;
                        inputVector[inputPos + 3] = 0f;

                        break;
                    default:
                        inputVector[inputPos + 1] = 0f;
                        inputVector[inputPos + 2] = 0f;
                        inputVector[inputPos + 3] = 1f;

                        break;
                }

            }
            else
            {
                inputVector[inputPos + 1] = 0f;
                inputVector[inputPos + 2] = 0f;
                inputVector[inputPos + 3] = 0f;
            }
        }

    }

    public void init()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        net = new NeuralNetwork();
        buildLayers();
        //net.initRandomWeights();
        net.initRandomBiases();
        mutability = 1f;
        noParent = false;
        initialized = true;
    }

    public void inherit(Creature creature)
    {
        world = creature.world;
        net = creature.getNet().copy();
        //string parent = "";
        //foreach (float i in net.getLayers()[0].getWeights())
        //{
        //    parent += i;
        //}
        //mutability = creature.mutability + UnityEngine.Random.Range(-mutability, mutability);
        net.deviateOneWeight(0.01f);
        //net.deviateWeights(0.01f);
        //if (mutability < 0.001f) mutability = 0.001f;
        //string child = "";
        //foreach (float i in net.getLayers()[0].getWeights())
        //{
        //    child += i;
        //}
        //float[,] parentWeights = creature.net.getLayers()[0].getWeights();
        //float[,] childWeights = net.getLayers()[0].getWeights();
        //for(int i = 0; i < childWeights.GetLength(1); i++)
        //{
        //    if (childWeights[0,i] != parentWeights[0, i])
        //    {
        //        Debug.Log("Different weight at " + i + " Child: " + childWeights[0,i] + " Parent: " + parentWeights[0,i]);
        //    }
        //}
        //Debug.Log("Parent: " + parent + " Child: " + child);
        initialized = true;
    }

    public GameObject birth()
    {
        if((this.GetType() == typeof(Prey) && world.preyAlive <= world.maxPrey) || (this.GetType() == typeof(Predator) && world.predAlive <= world.maxPred))
        {
            tillBirth = 1;
            GameObject child = (GameObject)Instantiate(prefab);
            String[] name = gameObject.name.Split('-');

            if (this.GetType() == typeof(Prey)) { 
                world.preyAlive++;
                child.name = name[0] + "-" + name[1] + "-" + (Int32.Parse(name[2]) + 1);
                child.GetComponent<Prey>().inherit(this);
            }
            if (this.GetType() == typeof(Predator)) { 
                world.predAlive++;
                child.name = name[0] + "-" + name[1] + "-" + (Int32.Parse(name[2]) + 1);
                child.GetComponent<Predator>().inherit(this);
            }

            return child;
        }
        return null;
    }
    public bool damage(float damage)
    {
        if (health <= 0) return false;

        this.health -= damage;

        if (this.health <= 0) return true;
        else return false;
    }
    protected void raycast()
    {
        int spread = fov / numOfViewRays;
        for (int i = 0; i < numOfViewRays; i += 1)
        {
            Vector3 dir = Quaternion.AngleAxis(-(fov / 2f) + (i * spread), transform.forward * -1) * transform.up;
            Vector3 start = transform.position + (dir * (col.radius + 0.001f));
            Vector3 cast = dir * viewDistance;
            vision[i] = Physics2D.Raycast(start, cast, cast.magnitude);
            Debug.DrawRay(start, cast, Color.red, updatePeriod);
        }
    }
}
