using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    private int timeCount;                                           //シーン開始からのフレーム数

    // Use this for initialization
    void Start()
    {
        //スライダーの現在位置をセーブされていた位置にする。
        GameObject.Find("SliderBGM").GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GameObject.Find("SliderSE").GetComponent<Slider>().value = PlayerPrefs.GetFloat("SEVolume", 0.8f);
        //BGM再生
        DontDestroyOnLoad(GameObject.Find("BGMManager"));//BGMマネージャーのオブジェクトはタイトル画面で作ってゲーム終了までそれを使用。
        GameObject.Find("BGMManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("TitleBGM"));
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(true, 0);//BGMManager内部変数の初期化
    }

    // Update is called once per frame
    void Update()
    {
        timeCount++;
    }

    public void PushStartButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MapScene");
    }

    public void PushCharacterButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "CharacterSheet");
    }

    public void PushMaskButton()
    {
            StartCoroutine(GetComponent<Utility>().GoToURL("https://play.google.com/store/apps/details?id=com.brainmixer.", 0f));
    }


}
