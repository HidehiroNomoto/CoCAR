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
    public GameObject StartButton;
    public GameObject CharacterButton;
    public GameObject SelectButton;
    public GameObject HelpButton;
    public GameObject JumpButton;
    public GameObject DeleteButton;
    public GameObject makumaObj;
    public GameObject titleText;
    public GameObject VButtonImage;
    public GameObject ChoiceFolder;
    public Sprite pad;
    public Sprite walk;
    GameObject objBGM;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.GetInt("[system]VMode", 0) > 0) { VButtonText.GetComponent<Text>().text = "机上プレイ\nON"; VButtonImage.GetComponent<Image>().sprite = pad; }
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
Application.platform == RuntimePlatform.OSXPlayer ||
Application.platform == RuntimePlatform.LinuxPlayer)
        {
            Screen.SetResolution(400, 800, false);
            VButton.SetActive(false);
        }
        objBGM = GameObject.Find("BGMManager");
        objBGM.GetComponent<BGMManager>().map=null;
        scenarionamePath =PlayerPrefs.GetString("[system]進行中シナリオ","").Split(new char[] {'\\' ,'.', '/'});
        if (scenarionamePath.Length >= 2) { GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n" + scenarionamePath[scenarionamePath.Length - 2];PlayerPrefs.SetString("[system]ScenarioName", scenarionamePath[scenarionamePath.Length - 2]); }//アドレスからフォルダ名と拡張子を排除。.と\を区切り文字にすると拡張子が最後(Length-1)にあるので、その手前の(Length-2)が欲しい文字列。
        if (PlayerPrefs.GetString("[system]進行中シナリオ", "") != "") { GameObject.Find("SelectText").GetComponent<Text>().text = "シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size>"; }
        if (PlayerPrefs.GetInt("[system]Status0") > 0) { GameObject.Find("CharaText").GetComponent<Text>().text = "探索者作成"; }
        //スライダーの現在位置をセーブされていた位置にする。
        GameObject.Find("SliderBGM").GetComponent<Slider>().value = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f);
        GameObject.Find("SliderSE").GetComponent<Slider>().value = PlayerPrefs.GetFloat("[system]SEVolume", 0.8f);
        //BGM再生
        objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f);
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("TitleBGM"));
        objBGM.GetComponent<BGMManager>().bgmChange(true, 0);//BGMManager内部変数の初期化
        if (PlayerPrefs.GetInt("[system]Status0", 0) ==0 || (PlayerPrefs.GetString("[system]進行中シナリオ", "").Contains(".zip")==false)) { StartButton.SetActive(false); GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n"; }
        if (objBGM.GetComponent<BGMManager>().makuma == 1 && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {makumaObj.SetActive(true); }
        StartCoroutine(SlideTitle());
    }

    // Update is called once per frame
    void Update()
    {
        timeCount++;
        if (timeCount % 100 > 90) { if (timeCount % 100 < 95) { makumaObj.GetComponent<RectTransform>().sizeDelta = new Vector2(700 + 10 * (timeCount % 100 - 90), 156 + 10 * (timeCount % 100 - 90)); } else { makumaObj.GetComponent<RectTransform>().sizeDelta = new Vector2(700 + 10 * (100 - timeCount % 100), 156 + 10 * (100 - timeCount % 100)); } }
    }

    public IEnumerator SlideTitle()
    {
        for (int i = 0; i < 100; i++)
        {
            titleText.GetComponent<RectTransform>().localPosition = new Vector2(50+i,100);
            yield return null;
        }
        titleText.GetComponent<RectTransform>().localPosition = new Vector2(150, 100);
        for (int i = 0; i < 5; i++)
        {
            titleText.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 30+i);
            yield return null;
        }
        for (int i = 0; i < 5; i++)
        {
            titleText.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 35-i);
            yield return null;
        }
        titleText.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 30);
    }



    public void PushStartButton()
    {
        PlayerPrefs.SetInt("[system]未決定",0);
        StartCoroutine(Anten());
    }
    private IEnumerator Anten()
    {
        GameObject anten= GameObject.Find("Anten");
        for (int i = 0; i < 30; i++)
        {
            anten.GetComponent<Image>().color = new Color(1, 1, 1, (float)i / 30);
#if UNITY_IOS
i++;
#endif
            yield return null;
        }
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MapScene");
    }


    public void PushCharacterButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "CharacterSheet");
    }

    public void PushSelectButton()
    {
        SelectButton.SetActive(false);DeleteButton.SetActive(false);
        GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]進行中シナリオ");
    }

    public void PushDeleteButton()
    {
        SelectButton.SetActive(false); DeleteButton.SetActive(false);
#if UNITY_ANDROID
        ChoiceFolder.SetActive(true);
#else
 GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]消去ファイル");
#endif
    }

    public void ChoiceFolderButton(int num)
    {
        if (num == 0) { GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]消去ファイル"); }
        if (num == 1) { GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]消去ファイルCS"); }
        ChoiceFolder.SetActive(false);
    }

    public void PushVButton()
    {
        if (PlayerPrefs.GetInt("[system]VMode", 0) == 0)
        {
            PlayerPrefs.SetInt("[system]VMode", 1);
            VButtonText.GetComponent<Text>().text = "机上プレイ\nON";
            VButtonImage.GetComponent<Image>().sprite = pad;
        }
        else
        {
            PlayerPrefs.SetInt("[system]VMode", 0);
            VButtonText.GetComponent<Text>().text = "机上プレイ\nOFF";
            VButtonImage.GetComponent<Image>().sprite = walk;
        }
    }

    public void PushHelpButton()
    {
        StartCoroutine(GetComponent<Utility>().GoToURL("http://www.brainmixer.net/CoCAR/CoCARguide.pdf", 0f));
    }

    public void PushJumpButton()
    {
            StartCoroutine(GetComponent<Utility>().GoToURL("http://www1366uj.sakura.ne.jp/public_html/scenario/upload/upload.cgi", 0f));
    }

}
