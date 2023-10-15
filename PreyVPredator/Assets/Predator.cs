using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class Predator : Creature
{
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

    public int foodTillBirth;

    // Start is called before the first frame update
    void Awake()
    {
        health = 1;
        tillBirth = 1;
        lifespanInSec = 20;

        updatePeriod = 0.2f;

        attackCooldown = 1;
        lastAttack = 0;

        foodTillBirth = 2;

        viewDistance = 2;
        numOfViewRays = 11;
        fov = 160;


        initialized = false;
    }

    void Start()
    {

        if (noParent)
        {
            init();
        }

        timer = 0;
        inputVector = new float[numOfViewRays * 4 + 2];
        vision = new RaycastHit2D[numOfViewRays];
    }
    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            if (timer < updatePeriod)
            {
                timer += Time.deltaTime;
                lastAttack += Time.deltaTime;
            }
            else
            {
                if (this.health <= 0)
                {
                    die();
                }
                else
                {
                    health -= (timer / lifespanInSec);

                    timer = 0;

                    look();

                    inputVector[inputVector.Length - 2] = 0;
                    inputVector[inputVector.Length - 1] = 0;

                    //inputVector[inputVector.Length - 2] = this.health;
                    //inputVector[inputVector.Length - 1] = this.tillBirth;


                    outputVector = net.computeNetwork(inputVector);


                    body.angularVelocity = outputVector[0];
                    body.velocity = transform.up * outputVector[1];


                    if (lastAttack >= attackCooldown && vision[numOfViewRays / 2].collider != null && vision[numOfViewRays / 2].distance < .1f)
                    {
                        Prey prey = vision[numOfViewRays / 2].collider.gameObject.GetComponent<Prey>();
                        if (prey != null) {
                            lastAttack = 0;
                            if(prey.damage(.5f))
                            {
                                tillBirth -= (1f / (float)foodTillBirth) + 0.000001f;
                                if (tillBirth < 0) tillBirth = 0;
                                health += 0.2f;
                                if (health > 1) health = 1;
                            }
                        }
                    }

                    if (tillBirth <= 0)
                    {
                        birth();
                    }
                }

                
            }
        }
    }

}
