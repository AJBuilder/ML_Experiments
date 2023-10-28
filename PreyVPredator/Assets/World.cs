using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public float simSpeed = 1;
    public int preyToSpawn, predToSpawn;
    public int maxPrey, maxPred;
    public GameObject prey, predator;
    public int worldSize;

    public GameObject left,right,top,bottom;
    public Text info;

    public int preyAlive, predAlive;
    public int spawns;

    private float timer;
    public float updatePeriod;


    private void spawnWorld()
    {

        left.transform.position = transform.position + new Vector3(-worldSize / 2, 0, 0);
        right.transform.position = transform.position + new Vector3(worldSize / 2, 0, 0);
        top.transform.position = transform.position + new Vector3(0, worldSize / 2, 0);
        bottom.transform.position = transform.position + new Vector3(0, -worldSize / 2, 0);

        top.transform.eulerAngles = new Vector3(0,0,-90);
        bottom.transform.eulerAngles = new Vector3(0,0,90);

        left.transform.localScale = new Vector3(1, worldSize + 1, 1);
        right.transform.localScale = new Vector3(1, worldSize + 1, 1);
        top.transform.localScale = new Vector3(1, worldSize + 1, 1);
        bottom.transform.localScale = new Vector3(1, worldSize + 1, 1);

        int predDiv = (int)Math.Floor(Math.Sqrt(predToSpawn)) + 1;
        float predScale = (worldSize * .9f) / predDiv;
        for (int i = 0; i < predToSpawn; i++)
        {
            GameObject clone = Instantiate(predator, transform.position + new Vector3(i % predDiv * predScale - (predScale * predDiv / 2), i / predDiv * predScale - (predScale * predDiv / 2), 0), Quaternion.identity);
            clone.name = "Predator-" + i.ToString() + "-0";
            clone.GetComponent<Predator>().init();
            predAlive++;
        }

        int preyDiv = (int)Math.Floor(Math.Sqrt(preyToSpawn)) + 1;
        float preyScale = (worldSize * .9f) / preyDiv;
        for (int i = 0; i < preyToSpawn; i++)
        {
            GameObject clone = Instantiate(prey, transform.position + new Vector3((i % preyDiv) * preyScale + 0.0001f - (preyScale * preyDiv / 2), i / preyDiv * preyScale - (preyScale * preyDiv / 2), 0), Quaternion.identity);
            clone.name = "Prey-" + i.ToString() + "-0";
            clone.GetComponent<Prey>().init();
            preyAlive++;
        }

        spawns++;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawns = 0;
        predAlive = 0;
        preyAlive = 0;
        updatePeriod = 20;
        timer = updatePeriod;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(simSpeed < 0)
        {
            simSpeed = 0;
        }
        Time.timeScale = simSpeed;



        if (timer < updatePeriod)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;

            info.text = "FPS: " + Time.captureFramerate + '\n' +
                "Prey: " + preyAlive + '\n' +
                "Pred: " + predAlive + '\n';
            Debug.Log("preyAlive " + preyAlive);

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }


            if (preyAlive <= 0 || predAlive <= 0)
            {

                GameObject[] preyObj = GameObject.FindGameObjectsWithTag("Prey");
                GameObject[] predObj = GameObject.FindGameObjectsWithTag("Predator");

                foreach (GameObject go in preyObj)
                {
                    Destroy(go);

                }
                foreach (GameObject go in predObj)
                {
                    Destroy(go);
                }

                predAlive = 0;
                preyAlive = 0;
                spawnWorld();

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject wall = collision.GetContact(0).otherCollider.gameObject;
        if(wall == left)
        {
            collision.gameObject.transform.position += new Vector3((worldSize - (collision.gameObject.GetComponent<CircleCollider2D>().radius + 0.1f)), 0, 0);
        }
        else if(wall == right)
        {
            collision.gameObject.transform.position += new Vector3(-(worldSize - (collision.gameObject.GetComponent<CircleCollider2D>().radius + 0.1f)), 0, 0);
        }
        else if (wall == top)
        {
            collision.gameObject.transform.position += new Vector3(0, -(worldSize - (collision.gameObject.GetComponent<CircleCollider2D>().radius + 0.1f)), 0);
        }
        else if (wall == bottom)
        {
            collision.gameObject.transform.position += new Vector3(0, (worldSize - (collision.gameObject.GetComponent<CircleCollider2D>().radius + 0.1f)), 0);
        }
    }

}


