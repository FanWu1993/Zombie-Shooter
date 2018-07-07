using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text timer_text;
    public Text kills_text;
    public Text spawn_text;
    private float killCount = 0;
    private float Timer;
	// Use this for initialization
	void Start () {
        Timer = 0f;
	}

	// Update is called once per frame
	void Update () {
        Timer += Time.deltaTime;
        timer_text.text = "Time: " + (int)Timer;
	}
    //void TimerReset()
    //{
    //}

    void spawnRate(int spawnRate)
    {
        spawn_text.text = "Spawn Rate: " + spawnRate + "s";
    }

    void CountKills()
    {
        killCount++;
        kills_text.text = "Kills: " + killCount;
    }
}
