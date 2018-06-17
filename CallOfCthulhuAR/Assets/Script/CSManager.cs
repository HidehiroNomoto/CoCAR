using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSManager : MonoBehaviour {
    private int[] status=new int[STATUSNUM];
    private GameObject[] statusObj=new GameObject[12];

    const int STATUSNUM = 12;

	// Use this for initialization
	void Start () {

        MakeCharacter();


    }

    //キャラのステータス決定
    public void MakeCharacter()
    {

        Utility u1 = GetComponent<Utility>();
        for (int i = 0; i < STATUSNUM; i++)
        {
            statusObj[i] = GameObject.Find("statusObj" + i.ToString()).gameObject as GameObject;
        }
        status[0] = u1.DiceRoll(3, 6);
        status[1] = u1.DiceRoll(3, 6);
        status[2] = u1.DiceRoll(3, 6);
        status[3] = u1.DiceRoll(2, 6) + 6;
        status[4] = u1.DiceRoll(3, 6);
        status[5] = u1.DiceRoll(3, 6);
        status[6] = u1.DiceRoll(2, 6) + 6;
        status[7] = u1.DiceRoll(3, 6) + 3;
        if (status[1] + status[6] < 13) { status[8] = -6; }
        else if (status[0] + status[6] < 17) { status[8] = -4; }
        else if (status[0] + status[6] < 25) { status[8] = 0; }
        else if (status[0] + status[6] < 33) { status[8] = 4; }
        else if (status[0] + status[6] < 41) { status[8] = 6; }
        status[9] = (status[1] + status[6]) / 2 + (status[1] + status[6]) % 2;
        status[10] = status[5];
        status[11] = status[5] * 5;
        for (int i = 0; i < STATUSNUM; i++)
        {
            if (i == 8)
            {
                if (status[8] > 0) { statusObj[i].GetComponent<Text>().text += "+1D"; }
                if (status[8] < 0) { statusObj[i].GetComponent<Text>().text += "-1D"; }
            }
            statusObj[i].GetComponent<Text>().text += Mathf.Abs(status[i]);
        }
    }

    public void PushDecideButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MapScene");
    }


    // Update is called once per frame
    void Update () {
		
	}
}
