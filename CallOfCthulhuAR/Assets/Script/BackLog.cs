using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class BackLog : MonoBehaviour {
    public GameObject obj,obj2,obj3,obj4,obj5,obj6,obj7,obj8;
    GameObject objGame;
    private bool backLog = false;
    private bool itemFlag = false;
    public int startLog = 0;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	}

    public IEnumerator BackLogScroll(string[] logText,int startLog2)
    {
        Vector3 mousePos=Vector3.zero;
        int startLog3=startLog2;
        while (backLog == true)
        {
            //
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            mousePos.z = 0;
            if (Input.GetMouseButton(0))
            {
                
                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;

                //タッチ対応デバイス向け、1本目の指にのみ反応
                if (Input.touchSupported)
                {
                    diff = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) - mousePos;
                }
                diff.z = 0;
                if (diff.y > 0.2f) { if (startLog < startLog2) { startLog++; }; mousePos.y +=0.2f; }
                if (diff.y < -0.2f) { if (startLog > 0) { startLog--; }; mousePos.y -=0.2f; }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePos = Vector3.zero;
            }
            if (startLog3 != startLog)
            {
                startLog3 = startLog;
                obj2.GetComponent<Text>().text = "";
                for (int j = startLog; j < startLog2; j++)
                {
                    obj2.GetComponent<Text>().text+=logText[j];
                }
            }
            yield return null;
        }
        if (SceneManager.GetActiveScene().name=="NovelScene")
        {
            GameObject.Find("NovelManager").gameObject.GetComponent<ScenariosceneManager>().backLogCSFlag = false;
        }
    }
    
    public void BackLogButton()
    {
        BackLogButtonIn();
    }

    private void BackLogButtonIn()
    {
        int logNum = 0;
        int logNum2 = 0;
        string[] logText = new string[1000];
        int startLog2 = 0;
        if (SceneManager.GetActiveScene().name == "NovelScene")
        {
           
            GameObject.Find("NovelManager").gameObject.GetComponent<ScenariosceneManager>().backLogCSFlag = true;
           
        }
        if (backLog == false)
        {
            obj.gameObject.SetActive(true);
            obj2.gameObject.SetActive(true);
            obj4.gameObject.SetActive(true);
            obj8.gameObject.SetActive(false);
            obj2.GetComponent<Text>().text = "";
            logNum2 = PlayerPrefs.GetInt("[system]最新ログ番号", 0);
            obj3.GetComponent<Text>().text = "戻る";
            if (PlayerPrefs.GetString("[system]バックログ" + (logNum2 + 1).ToString(), "[NoLog!]") == "[NoLog!]") { logNum = 0; } else { logNum = logNum2 + 1; if (logNum >= 1000) { logNum = 0; } }
            for (int i = 0; i < 1000; i++)
            {
                logText[i] = PlayerPrefs.GetString("[system]バックログ" + logNum.ToString(), "") + "\n";
                logNum++;
                if (logNum >= 1000) { logNum = 0; }
            }
            if (logNum2 >= 4) { startLog = logNum2 - 4; } else { startLog = 0; }
            startLog2 = startLog + 4;
            backLog = true;
            StartCoroutine(BackLogScroll(logText, startLog2));
        }
        else
        {
            obj.gameObject.SetActive(false);
            obj2.gameObject.SetActive(false);
            obj4.gameObject.SetActive(false);
            obj8.gameObject.SetActive(true);
            obj3.GetComponent<Text>().text = "BackLog";
            backLog = false;
        }
    }

    public void ItemButton()
    {
        ItemButtonIn();
    }
    private void ItemButtonIn()
    {
        if (SceneManager.GetActiveScene().name == "NovelScene")
        {
           
            GameObject.Find("NovelManager").gameObject.GetComponent<ScenariosceneManager>().backLogCSFlag = true;
           
        }
        if (itemFlag == false)
        {
            obj5.gameObject.SetActive(true);
            obj4.GetComponentInChildren<Text>().text = "戻る";
            itemFlag = true;
            StartCoroutine(ItemSee());
        }
        else
        {
            obj5.gameObject.SetActive(false);
            obj4.GetComponentInChildren<Text>().text = "取得情報";
            itemFlag = false;
        }  
    }
    private IEnumerator ItemSee()
    {
        int itemNum;
        string str = "";
        bool changeFlag = false;
        Vector3 mousePos = Vector3.zero;
        int itemNumMax = 100;
        itemNum=PlayerPrefs.GetInt("[system]NowItem",-1);
        str = PlayerPrefs.GetString("[system]Item" + itemNum.ToString(), "");
        if (str == "")
        {
            if (PlayerPrefs.GetString("[system]Item0", "") == "")
            {
                obj7.GetComponent<Text>().text = "NotFind";
            }
            else//アイテムはあるが、まだ情報取得画面を一度も開いたことがない場合。
            {
                obj7.GetComponent<Text>().text = "<size=36>左に\nスライド</size>";
            }
        }
        else { obj7.GetComponent<Text>().text = itemNum.ToString(); LoadItem(str); }
        while (itemFlag == true)
        {
            //
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            mousePos.z = 0;
            if (Input.GetMouseButton(0))
            {

                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;

                //タッチ対応デバイス向け、1本目の指にのみ反応
                if (Input.touchSupported)
                {
                    diff = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) - mousePos;
                }
                diff.z = 0;
                if (changeFlag==false && diff.x < - 0.2f) {  for (itemNum++; itemNum < itemNumMax; itemNum++) { str = PlayerPrefs.GetString("[system]Item" + itemNum.ToString(), ""); if (str != "") { obj7.GetComponent<Text>().text = itemNum.ToString(); LoadItem(str); PlayerPrefs.SetInt("[system]NowItem", itemNum); changeFlag = true; break; } } if (itemNum >= itemNumMax) { itemNum = itemNumMax-1; } } 
                if (changeFlag==false && diff.x > + 0.2f) {  for (itemNum--; itemNum >=0; itemNum--) { str = PlayerPrefs.GetString("[system]Item" + itemNum.ToString(), ""); if (str != "") { obj7.GetComponent<Text>().text = itemNum.ToString(); LoadItem(str); PlayerPrefs.SetInt("[system]NowItem", itemNum); changeFlag = true; break; } } if (itemNum < 0) { itemNum = 0; }  }
            }
            if (Input.GetMouseButtonUp(0))
            {
                mousePos = Vector3.zero;
                changeFlag = false;
            }
            yield return null;  
        }
    }

    //アイテム画像ファイルを拾ってくる。
    private void LoadItem(string path)
    {
        byte[] buffer;
        try
        {
            //閲覧するエントリ
            string extractFile = path;
            ICSharpCode.SharpZipLib.Zip.ZipFile zf;
            //ZipFileオブジェクトの作成
            zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(PlayerPrefs.GetString("[system]進行中シナリオ", ""));//説明に書かれてる以外のエラーが出てる。

            zf.Password = Secret.SecretString.zipPass;
            //展開するエントリを探す
            ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zf.GetEntry(extractFile);

            if (ze != null)
            {
                //pngファイルの場合
                if (path.Substring(path.Length - 4) == ".png")
                {
                    //閲覧するZIPエントリのStreamを取得
                    Stream fs = zf.GetInputStream(ze);
                    buffer = ReadBinaryData(fs);//bufferにbyte[]になったファイルを読み込み

                    // 画像を取り出す
                    //横サイズ
                    int pos = 16;
                    int width = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        width = width * 256 + buffer[pos++];
                    }
                    //縦サイズ
                    int height = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        height = height * 256 + buffer[pos++];
                    }
                    //byteからTexture2D作成
                    Texture2D texture = new Texture2D(width, height);
                    texture.LoadImage(buffer);

                    // 読み込んだ画像からSpriteを作成する
                    obj6.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
                    obj6.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    //閉じる
                    fs.Close();
                }
            }
            //閉じる
            zf.Close();
        }
        catch
        {
        }
    }
    // ストリームからデータを読み込み、バイト配列に格納
    static public byte[] ReadBinaryData(Stream st)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            st.CopyTo(ms);
            return ms.ToArray();
        }
    }


}
