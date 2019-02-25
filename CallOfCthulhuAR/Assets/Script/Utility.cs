using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

[DefaultExecutionOrder(-1)]//utilityは他から引用されるのでstartを先行処理させる。
public class Utility : MonoBehaviour {
    public GameObject objBGM;                                  //BGMのオブジェクト
    private bool fadeFlag;                                      //フェードイン・フェードアウト中か否か
    public bool pushObjectFlag;                                 //ボタンオブジェクトのタップ(true)か画面自体（ストーリー進行）のタップ(false)かの判定
    public bool selectFlag;                                     //選択待ち中、どれかが選択されたか否かの判定
    public Sprite[] moveDice6Graphic = new Sprite[8];
    public Sprite[] dice6Graphic = new Sprite[6];
    public GameObject objDices;
    // Use this for initialization
    void Start()
    {
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        pushObjectFlag = false;
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) { GameObject.Find("TweetButton").SetActive(false); }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            float x;
            float y;
            float xy = (float)Screen.height/Screen.width;
            x= GameObject.Find("ScreenSizeChanger").GetComponent<RectTransform>().sizeDelta.x;
            if (xy >= 1.6) { y =x*(float)1.6 ; } else { y = x*xy; }
            GameObject.Find("ScreenSizeChanger").GetComponent<RectTransform>().sizeDelta = new Vector2(x,y); }
    }

	
	// Update is called once per frame
	void Update () {
		
	}


    public IEnumerator LoadSceneCoroutine(string scene)
    {
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void BGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("[system]BGMVolume", volume);
        objBGM.GetComponent<AudioSource>().volume = volume;
    }

    public void SEVolume(float volume)
    {
        PlayerPrefs.SetFloat("[system]SEVolume", volume);
    }

    public void BGMStop()
    {
        objBGM.GetComponent<AudioSource>().Stop();
    }

    public IEnumerator BGMFadeOut(int time)
    {
        while (fadeFlag == true) { yield return null; }//他でフェイドインフェイドアウト中なら待つ。
        fadeFlag = true;
        if (time > 0)
        {
            for (int i = 0; i < time; i++)
            {
                objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f) * (1.0f - (float)i / time);
                yield return null;
            }
        }
        objBGM.GetComponent<AudioSource>().volume = 0f;//最終的には０に。（for文をi<=timeにするとtime=0で０除算が発生しうる構造になるので、最後のvol=0のみfor文から隔離）
        BGMStop();
        fadeFlag = false;
        yield return null;
    }

    public IEnumerator BGMFadeIn(int time)
    {
        while (fadeFlag == true) { yield return null; }//他でフェイドインフェイドアウト中なら待つ。
        fadeFlag = true;
        if (time > 0)
        {
            for (int i = 0; i < time; i++)
            {
                objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f) * ((float)i / time);
                yield return null;
            }
        }
        objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]BGMVolume", 0.8f);//最終的には０に。（for文をi<=timeにするとtime=0で０除算が発生しうる構造になるので、最後のvol=BGMVolumeのみfor文から隔離）
        fadeFlag = false;
        yield return null;
    }

    public void BGMPlay(AudioClip bgm)
    {
        objBGM.GetComponent<AudioSource>().clip = bgm;
        objBGM.GetComponent<AudioSource>().Play();
    }

    public void SEPlay(AudioClip se)
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("[system]SEVolume", 0.8f);
        GetComponent<AudioSource>().PlayOneShot(se);
    }

    //画面が押されたかチェックするコルーチン
    public IEnumerator PushWait()
    {
        while (true)//ブレークするまでループを続ける。
        {
            yield return null;//本体に処理を返して他のオブジェクトのイベントトリガーを確認。
            if (Input.GetMouseButtonDown(0) == true)
            {
                if (pushObjectFlag == false)//フラグが立っていたらオブジェクト処理のためのタップだったと判定。
                {
                    yield break;//falseならコルーチン脱出
                }
            }
        }
    }

    public IEnumerator SelectWait()
    {
        selectFlag = false;
        while (true)//ループを続ける。
        {
            yield return null;
            if (selectFlag == true) { break; }
        }
    }

    public int DiceRoll(int diceNum,int diceMax)
    {
        int x=0;
        for (int i = 0; i < diceNum; i++) { x += Random.Range(0, diceMax)+1; }
        return x;
    }

    //URLへの遷移と、その前の演出等を見せるための待機をセットにしたコルーチン
    public IEnumerator GoToURL(string URL,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Application.OpenURL(URL);
    }

    public void PushTweetButton()
    {
        string name;
        if (PlayerPrefs.GetString("[system]ScenarioName", "") == "") { name = ""; } else { name = "シナリオ/#" + PlayerPrefs.GetString("[system]ScenarioName", ""); }
        StartCoroutine(TweetCoroutine(name));
    }

    private IEnumerator TweetCoroutine(string name)
    {
        ScreenCapture.CaptureScreenshot("CoCARcapture.png");
        yield return null;
        string imagePath = Application.persistentDataPath + "/CoCARcapture.png";
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            SocialConnector.SocialConnector.Share("\n[TRPG型ADVゲーム『クトゥルフ神話AR』] #クトゥルフ神話AR ", "\nhttp://brainmixer.net/CoCAR/linker/");
            if (objBGM.GetComponent<BGMManager>().makuma == 1) { string[] separateText=new string[2]; separateText[0] ="[system]正気度ポイント";separateText[1] ="1D6+0"; StartCoroutine(Gain(separateText)); objBGM.GetComponent<BGMManager>().makuma = 0;GameObject.Find("tweetguide").SetActive(false); }
        }
        else
        {
            SocialConnector.SocialConnector.Share("\n[TRPG型ADVゲーム『クトゥルフ神話AR』] #クトゥルフ神話AR " + name, "\nbrainmixer.net/CoCAR/linker/", imagePath);
        }
        
    }

    private IEnumerator Gain(string[] separateText)
    {
        int changeValue = 0;
        int y1, y2;
        int beforeValue = 0;
        string targetStr;
        Utility u1 = GetComponent<Utility>();
        string[] separate3Text;
        objDices.SetActive(true);
        Text text = GameObject.Find("DiceText").GetComponent<Text>();
        targetStr = separateText[0];
        if (targetStr == "[system]耐久力") { beforeValue = PlayerPrefs.GetInt("[system]耐久力", 0); }
        separate3Text = separateText[1].Split(new char[] { 'D', '+' });
        if (int.Parse(separate3Text[0]) >= 0) { y2 = 1; } else { y2 = -1; }
        for (y1 = 0; y1 < int.Parse(separate3Text[0]) * y2; y1++)
        {
            changeValue = u1.DiceRoll(1, int.Parse(separate3Text[1]));
            if (int.Parse(separate3Text[1]) != 100)
            {
                StartCoroutine(DiceEffect(1, int.Parse(separate3Text[1]), changeValue));
            }
            else
            {
                if (changeValue != 100) { StartCoroutine(DiceEffect(0, 10, changeValue / 10)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
                StartCoroutine(DiceEffect(1, 10, changeValue % 10));
            }
            for (int v = 0; v < 60; v++) { yield return null; }
            if (PlayerPrefs.GetInt(targetStr, 0) < -1 * changeValue * y2) { PlayerPrefs.SetInt(targetStr, 0); }
            else if (targetStr == "[system]耐久力" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status9", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status9", 0)); }
            else if (targetStr == "[system]マジック・ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status10", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status10", 0)); }
            else if (targetStr == "[system]正気度ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 99 - PlayerPrefs.GetInt("[system]Skill53", 0)) { PlayerPrefs.SetInt(targetStr, 99 - PlayerPrefs.GetInt("[system]Skill53", 0)); }
            else if (PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 100) { PlayerPrefs.SetInt(targetStr, 99); }
            else { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2); }
            if (y2 > 0)
            {
                text.text = "<color=blue>＜幕間の回復＞</color>\n\n\n\n\n\n\n\n\n\n";
                text.text=text.text + separateText[0].Substring(8) + "が<color=red>" + changeValue.ToString() + "</color>点上昇した。" + "\n（" + separateText[0].Substring(8) + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）";
                text.text = text.text + "\n耐久力が<color=red>全回復</color>した。";
                PlayerPrefs.SetInt("[system]耐久力", PlayerPrefs.GetInt("[system]Status9", 0));
            }
            else
            {
                text.text=separateText[0].Substring(8) + "が" + changeValue.ToString() + "点減少した。" + "\n（" + separateText[0].Substring(8) + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）";
            }
            for (int v = 0; v < 300; v++) { yield return null; }
            text.text = "";
            objDices.SetActive(false);
        }
    }

    private IEnumerator DiceEffect(int dicenum, int dicetype, int num)
    {
        AudioClip systemAudio= Resources.Load<AudioClip>("kan");
        GameObject objDice = GameObject.Find("Dice");
        if (dicetype == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                objDice.GetComponent<Image>().sprite = moveDice6Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null; }
            }
            objDice.GetComponent<Image>().sprite = dice6Graphic[num - 1];
        }
        SEPlay(systemAudio);
    }
}
