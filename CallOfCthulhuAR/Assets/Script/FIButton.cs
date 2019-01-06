using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FIButton : MonoBehaviour
{
    public string buttonName="";
    public GameObject objBGM;
    public GameObject objTime;
    public GameObject objMapSceneManager;
    public double latitude=0;
    public double longitude=0;

    // Start is called before the first frame update
    void Start()
    {
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        objTime = GameObject.Find("TimeText").gameObject as GameObject;
        objMapSceneManager = GameObject.Find("MapSceneManager").gameObject as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ボタンを押すとイベント起動。
    public void PushFreeIventButton()
    {
    
    objBGM.GetComponent<BGMManager>().chapterName = buttonName;
    objTime.GetComponent<Text>().text = "　　　<color=red>[★イベント発生]</color>";
        objMapSceneManager.GetComponent<MapScene>().sceneChange = true;
    if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && (!Input.location.isEnabledByUser)) { Input.location.Stop(); }
    PlayerPrefs.SetFloat("[system]longitude", (float)longitude); PlayerPrefs.SetFloat("[system]latitude", (float)latitude);
        objMapSceneManager.GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "NovelScene");
    return;
    }




}
