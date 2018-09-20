using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    private int timeCount;                                           //シーン開始からのフレーム数
    public GameObject FileBrowserPrefab;
    private string[] scenarionamePath;

    // Use this for initialization
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        scenarionamePath=PlayerPrefs.GetString("進行中シナリオ","").Split(new char[] {'\\' ,'.'});
        if (scenarionamePath.Length >= 2) { GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n" + scenarionamePath[scenarionamePath.Length - 2]; }//アドレスからフォルダ名と拡張子を排除。.と\を区切り文字にすると拡張子が最後(Length-1)にあるので、その手前の(Length-2)が欲しい文字列。
        if (PlayerPrefs.GetString("進行中シナリオ", "") != "") { GameObject.Find("SelectText").GetComponent<Text>().text = "シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size>"; }
        if (PlayerPrefs.GetInt("Status0") > 0) { GameObject.Find("CharaText").GetComponent<Text>().text = "探索者作成"; }
        //スライダーの現在位置をセーブされていた位置にする。
        GameObject.Find("SliderBGM").GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GameObject.Find("SliderSE").GetComponent<Slider>().value = PlayerPrefs.GetFloat("SEVolume", 0.8f);
        //BGM再生
        DontDestroyOnLoad(GameObject.Find("BGMManager"));//BGMマネージャーのオブジェクトはタイトル画面で作ってゲーム終了までそれを使用。
        GameObject.Find("BGMManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("TitleBGM"));
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(true, 0);//BGMManager内部変数の初期化
        if (PlayerPrefs.GetInt("Status0", 0) ==0) { GameObject.Find("StartButton").SetActive(false); }
    }

    // Update is called once per frame
    void Update()
    {
        timeCount++;
    }

    public void PushStartButton()
    {
        if (PlayerPrefs.GetInt("開始フラグ", 0) == 0)
        {
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "NovelScene");
        }
        else
        {
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MapScene");
        }
    }

    public void PushCharacterButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "CharacterSheet");
    }

    public void PushSelectButton()
    {
        GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("進行中シナリオ");
    }

    public void PushMaskButton()
    {
            StartCoroutine(GetComponent<Utility>().GoToURL("https://play.google.com/store/apps/details?id=com.brainmixer.", 0f));
    }


}
