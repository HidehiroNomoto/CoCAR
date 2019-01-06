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

    // Use this for initialization
    void Start () {
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        pushObjectFlag = false;
    }

	
	// Update is called once per frame
	void Update () {
		
	}


    public IEnumerator LoadSceneCoroutine(string scene)
    {
        if (SceneManager.GetActiveScene().name == "NovelScene")
        {
            string deletemp3 = GameObject.Find("NovelManager").GetComponent<ScenariosceneManager>().mp3tmpDir;
            if (deletemp3 != null && deletemp3 != "")
            {
                try { System.IO.Directory.Delete(deletemp3, true); } catch { }
            }
        }
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
        if (PlayerPrefs.GetString("[system]ScenarioName", "") == "") { name = ""; } else { name = "シナリオ/" + PlayerPrefs.GetString("[system]ScenarioName", ""); }
        SocialConnector.SocialConnector.Share("[iPhone/Android用アプリ『クトゥルフ神話AR』]" + name, "http://www.brainmixer.net/CoCAR/index.html", null);
    }

}
