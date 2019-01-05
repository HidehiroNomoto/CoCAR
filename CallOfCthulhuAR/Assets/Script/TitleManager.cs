using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    private int timeCount;                                           //シーン開始からのフレーム数
    public GameObject FileBrowserPrefab;
    private string[] scenarionamePath;
    public GameObject VButtonText;
    public GameObject VButton;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.GetInt("[system]VMode", 0) > 0) { VButtonText.GetComponent<Text>().text = "机上プレイ\nON"; }
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
Application.platform == RuntimePlatform.OSXPlayer ||
Application.platform == RuntimePlatform.LinuxPlayer)
        {
            Screen.SetResolution(400, 800, false);
            VButton.SetActive(false);
        }
        scenarionamePath =PlayerPrefs.GetString("[system]進行中シナリオ","").Split(new char[] {'\\' ,'.', '/'});
        if (scenarionamePath.Length >= 2) { GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n" + scenarionamePath[scenarionamePath.Length - 2]; }//アドレスからフォルダ名と拡張子を排除。.と\を区切り文字にすると拡張子が最後(Length-1)にあるので、その手前の(Length-2)が欲しい文字列。
        if (PlayerPrefs.GetString("[system]進行中シナリオ", "") != "") { GameObject.Find("SelectText").GetComponent<Text>().text = "シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size>"; }
        if (PlayerPrefs.GetInt("[system]Status0") > 0) { GameObject.Find("CharaText").GetComponent<Text>().text = "探索者作成"; }
        //スライダーの現在位置をセーブされていた位置にする。
        GameObject.Find("SliderBGM").GetComponent<Slider>().value = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f);
        GameObject.Find("SliderSE").GetComponent<Slider>().value = PlayerPrefs.GetFloat("[system]SEVolume", 0.8f);
        //BGM再生
        DontDestroyOnLoad(GameObject.Find("BGMManager"));//BGMマネージャーのオブジェクトはタイトル画面で作ってゲーム終了までそれを使用。
        GameObject.Find("BGMManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f);
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("TitleBGM"));
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(true, 0);//BGMManager内部変数の初期化
        if (PlayerPrefs.GetInt("[system]Status0", 0) ==0) { GameObject.Find("StartButton").SetActive(false); }
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

    public void PushSelectButton()
    {
        GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]進行中シナリオ");
    }

    public void PushVButton()
    {
        if (PlayerPrefs.GetInt("[system]VMode", 0) == 0)
        {
            PlayerPrefs.SetInt("[system]VMode", 1);
            VButtonText.GetComponent<Text>().text = "机上プレイ\nON";
        }
        else
        {
            PlayerPrefs.SetInt("[system]VMode", 0);
            VButtonText.GetComponent<Text>().text = "机上プレイ\nOFF";
        }
    }

    public void PushMaskButton()
    {
            StartCoroutine(GetComponent<Utility>().GoToURL("https://play.google.com/store/apps/details?id=com.brainmixer.", 0f));
    }

}
