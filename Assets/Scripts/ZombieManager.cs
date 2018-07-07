using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour {

    public GameObject zombie_to_spawn;
    public float SpawnRate = 5f;

    public Vector3[] spawnPoint;
    public Vector3 mySpawnPoint;
    private GameObject zombie;


    //public float change_Rate = 10f;
    //private float nextChange = 10.0f;
    //private float Timer;
    // Use this for initialization
    void Awake () {
        //Timer = 0f;
        SpawnRate = 5f;
        GameObject.Find("UIManager").SendMessage("spawnRate", (int)SpawnRate);
        InvokeRepeating("UpdateSpawning", 0.0f, SpawnRate);
    }

    void reSetInvoke()
    {
        CancelInvoke();
        SpawnRate--;
        GameObject.Find("UIManager").SendMessage("spawnRate",(int)SpawnRate);
        InvokeRepeating("UpdateSpawning", 0.0f, SpawnRate);
    }
    // Update is called once per frame
    void Update()
    {
        /*
        Timer += Time.deltaTime;
        if ( Timer > nextChange && SpawnRate > 1)
        {
            reSetInvoke();
            nextChange += change_Rate;
        }
        */
    }

    void UpdateSpawning () {
        int temp;
        temp = Random.Range(0,4);
        zombie = (GameObject)Instantiate(zombie_to_spawn, spawnPoint[temp], Quaternion.Euler(new Vector3(0, 0, 1)));
        zombie.GetComponent<ZombieAI>().mySpawnPoint = spawnPoint[temp];
    }
}
