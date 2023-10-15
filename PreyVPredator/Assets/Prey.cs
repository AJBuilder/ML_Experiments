using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class Prey : Creature
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

    public int secsBetweenBirths;

    void Awake()
    {
        health = 1;
        tillBirth = 1;

        updatePeriod = 0.2f;

        attackCooldown = 5;
        lastAttack = 0;

        secsBetweenBirths = 10;

        viewDistance = 2;
        numOfViewRays = 11;
        fov = 200;

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
                if(health <= 0)
                {
                    die();
                }
                else
                {

                    tillBirth -= (timer / secsBetweenBirths);
                    if(tillBirth < 0 ) tillBirth = 0;

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
                        Predator predator = vision[numOfViewRays / 2].collider.gameObject.GetComponent<Predator>();
                        if (predator != null)
                        {
                            lastAttack = 0;
                            predator.damage(.2f);
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
