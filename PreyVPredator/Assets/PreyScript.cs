using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class PreyScript : MonoBehaviour
{



    public float health;
    public float tillBirth;
    public float mutability;

    public Rigidbody2D body;
    public float updatePeriod;
    
    private NeuralNetwork net;
    private float timer;
    private RaycastHit2D[] vision;
    private float[] inputVector, outputVector;


    /* 
     * Inputs
     * Distance of 9 rays
     * Ray sees prey
     * Ray sees pred
     * Ray sees neither
     * Health
     * SecTillBirth
     * 
     * Outputs
     * Angular velocity -1 to 1 -> -180 to +180
     * Speed
     * Attack (Predator only)
     * 
     */

    public PreyScript(bool randomize)
    {
        net = new NeuralNetwork();
        buildLayers();
        if (randomize) net.initRandomWeights();
    }

    public PreyScript(NeuralNetwork parent)
    {
        net = parent;
        buildLayers();
        net.deviateWeights(mutability);
    }

    private void buildLayers()
    {
        net.addLayer(new Layer(38, 5, Layer.activationFunctions.TanH));
        net.addLayer(new Layer(5, 4, Layer.activationFunctions.TanH));
        net.addLayer(new Layer(4, 3, Layer.activationFunctions.Linear));
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 1;
        tillBirth = 1;
        mutability = 1;

        updatePeriod = 1 / 10;
        updatePeriod = 1 / 10;

        timer = 0;
        inputVector = new float[38];
        vision = new RaycastHit2D[9];
        Debug.Log("Starting!");
    }

    // Update is called once per frame
    void Update()
    {

        
        if(timer < updatePeriod)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;

            raycast();
            
            for(int i = 0; i < 9; i++)
            {
                if (vision[i].collider != null)
                {

                    inputVector[i] = vision[i].distance;

                    switch (vision[i].collider.gameObject.tag)
                    {
                        case "Prey":
                            inputVector[i + 9] = 1f;
                            inputVector[i + 10] = 0f;
                            inputVector[i + 11] = 0f;
                            break;
                        case "Predator":
                            inputVector[i + 9] = 0f;
                            inputVector[i + 10] = 1f;
                            inputVector[i + 11] = 0f;
                            break;
                        default:
                            inputVector[i + 9] = 0f;
                            inputVector[i + 10] = 0f;
                            inputVector[i + 11] = 1f;
                            break;
                    }



                }
                else
                {
                    inputVector[i] = 0f;
                    inputVector[i + 9] = 0f;
                }
            }


            inputVector[18] = this.health;
            inputVector[19] = this.tillBirth;

            Debug.Log("Input is " + inputVector);
        }
    }

    private void raycast()
    {
        for (int i = 0; i < 9; i += 1)
        {
            Vector3 dir = Quaternion.AngleAxis(-120 + (i * 30), transform.forward * -1) * transform.up;
            Vector3 start = transform.position + (dir / 10);
            Vector3 cast = dir / 10;
            Debug.Log("Magnitude is " + cast.magnitude);
            vision[i] = Physics2D.Raycast(start, cast, cast.magnitude);
            Debug.DrawRay(start, cast, Color.red, updatePeriod);
        }
    }


}
