using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosceneManager : MonoBehaviour {
    public string[] scenarioText = new string[10000];           //シナリオテキスト保存変数
    public AudioClip[] scenarioAudio = new AudioClip[40];       //シナリオＢＧＭ・ＳＥ保存変数
    public Sprite[] scenarioGraphic = new Sprite[100];          //シナリオ画像保存変数
    public string[] scenarioFilePath = new string[150000];      //シナリオ用ファイルのアドレス
    public bool sentenceEnd;                                    //文の処理が終了したか否か
    GameObject obj;
    GameObject objText;
    GameObject objTextBox;
    GameObject[] objCharacter = new GameObject[5];
    GameObject objBackImage;
    GameObject objBackText;
    GameObject objItem;
    GameObject objCanvas;
    const string _FILE_HEADER = "file://";                      //ファイル場所の頭
    const int CHARACTER_Y = 300;


    // Use this for initialization
    void Start() {
        obj = GameObject.Find("error").gameObject as GameObject;
        for (int i = 0; i < 5; i++) { objCharacter[i] = GameObject.Find("Chara" + i.ToString()).gameObject as GameObject; objCharacter[i].gameObject.SetActive(false); }
        objText = GameObject.Find("MainText").gameObject as GameObject;
        objTextBox = GameObject.Find("TextBox").gameObject as GameObject;
        objBackImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBackText = GameObject.Find("BackText").gameObject as GameObject; objBackText.gameObject.SetActive(false);
        objItem = GameObject.Find("Item").gameObject as GameObject; objItem.gameObject.SetActive(false);
        objCanvas = GameObject.Find("Canvas").gameObject as GameObject;
        //シナリオデータ読込
        LoadScenario("loadfile.txt", PlayerPrefs.GetInt("Chapter", 0));
        //シナリオ処理
        StartCoroutine(NovelGame());
    }


    // Update is called once per frame
    void Update() {

    }

    //ノベルゲーム処理
    private IEnumerator NovelGame()
    {
        for (int i=0; i < 10000; i++)
        {
            sentenceEnd = false;
            if (scenarioText[i] == "") { break; }
            if (scenarioText[i].Substring(0, 5) == "Text:") { TextDraw(scenarioText[i].Substring(5)); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 9) == "BackText:") { BackTextDraw(scenarioText[i].Substring(5)); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 5) == "Back:") { BackDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 4) == "BGM:") { BGMPlay(int.Parse(scenarioText[i].Substring(4))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 6) == "BGMIn:") { BGMIn(int.Parse(scenarioText[i].Substring(6))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 7) == "BGMOut:") { BGMOut(int.Parse(scenarioText[i].Substring(7))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 7) == "BGMStop") { BGMStop(); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 4) == "SE:") { SEPlay(int.Parse(scenarioText[i].Substring(3))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 5) == "Chara"){CharacterDraw(int.Parse(scenarioText[i].Substring(7)), int.Parse(scenarioText[i].Substring(5, 1))); sentenceEnd = true; }//Chara2ならポジション2、Chara5ならポジション5...。:の後（6文字目以降）は立ち絵の指定
            if (scenarioText[i].Substring(0, 5) == "Item:") { ItemDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Substring(0, 6) == "Shake:") { StartCoroutine(ShakeScreen()); }
            if (scenarioText[i].Substring(0, 5) == "Jump:") { StartCoroutine(CharacterJump(int.Parse(scenarioText[i].Substring(5, 1)))); }
            if (scenarioText[i].Substring(0, 4) == "Move") { StartCoroutine(CharacterMove(int.Parse(scenarioText[i].Substring(4, 1)), scenarioText[i].Substring(6, 1))); }
            yield return null;
            while(sentenceEnd==false) { yield return null; }


            objBackText.gameObject.SetActive(false);//背景テキストは出っ放しにならない
        }
    }

    private void TextDraw(string text)
    {
        objTextBox.gameObject.SetActive(true);
        objText.GetComponent<Text>().text = text;
    }

    private void BackTextDraw(string text)
    {
        //背景テキスト表示の際は通常テキスト欄は消す
        objTextBox.gameObject.SetActive(false);
        objBackText.gameObject.SetActive(true);
        objBackText.GetComponent<Text>().text = text;
    }

    private void BackDraw(int back)
    {
        objBackImage.GetComponent<Image>().sprite = scenarioGraphic[back];
    }

    private void CharacterDraw(int character,int position)
    {
        if (character == -1) { objCharacter[position].gameObject.SetActive(false); return; }
        objCharacter[position].gameObject.SetActive(true);
        objCharacter[position].GetComponent<Image>().sprite = scenarioGraphic[character];
    }

    private void ItemDraw(int item)
    {
        if (item == -1) { objItem.GetComponent<Image>().enabled = false; return; }
        objItem.GetComponent<Image>().enabled = true;
        objItem.GetComponent<Image>().sprite = scenarioGraphic[item];
    }

    private void BGMPlay(int bgm)
    {
        Utility u1 =GetComponent<Utility>();
        u1.BGMPlay(scenarioAudio[bgm]);
    }

    private void BGMIn(int time)
    {
        Utility u1 = GetComponent<Utility>();
        u1.BGMFadeIn(time);
    }

    private void BGMOut(int time)
    {
        Utility u1 = GetComponent<Utility>();
        u1.BGMFadeOut(time);
    }

    private void BGMStop()
    {
        Utility u1 = GetComponent<Utility>();
        u1.BGMStop();
    }

    private void SEPlay(int se)
    {
        Utility u1 = GetComponent<Utility>();
        u1.SEPlay(scenarioAudio[se]);
    }


    private IEnumerator CharacterMove(int position,string lr)
    {
        for (int i = 0; i < 5; i++)//キャラクター移動
        {
            if (lr == "L")
            {
                objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100 + i * 20 - 100, CHARACTER_Y, 0);
            }
            if (lr == "R")
            {
                objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100 + 100 - i * 20, CHARACTER_Y, 0);
            }
            yield return null;
        }
        objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100, CHARACTER_Y, 0);
        sentenceEnd = true;
    }

    //画面振動する関数
    private IEnumerator ShakeScreen()
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 - 5 + 10 * (i % 2));
            yield return null;
        }
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        sentenceEnd = true;
    }

    //キャラクターの小ジャンプ
    private IEnumerator CharacterJump(int position)
    {
        for (int i = 0; i < 7; i++)
        {
            objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100, CHARACTER_Y + i * 2, 1);
            yield return null;
        }
        for (int i = 7; i > 0; i--)
        {
            objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100, CHARACTER_Y + i * 2, 1);
            yield return null;
        }
        objCharacter[position].GetComponent<RectTransform>().localPosition = new Vector3(position * 100, CHARACTER_Y, 1);
        sentenceEnd = true;
    }






    //目次ファイルを読み込み、進行度に合わせてファイルを拾ってくる。
    private void LoadScenario(string path, int chapter)
    {
        // 目次ファイルが無かったら終わる
        if (!System.IO.File.Exists(path))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルに問題があります。" + _FILE_HEADER + path);
            return;
        }

        // 目次ファイルをロードする
        WWW request = new WWW(_FILE_HEADER + path);

        // テキストを取り出す
        string text = request.text;

        //アドレス変数の初期化
        for (int i = 0; i < 150 ; i++)
        {
            scenarioFilePath[i] = "";
        }

        // 読み込んだ目次テキストファイルからstring配列を作成する
        scenarioFilePath = text.Split('\n');

        //chapterに対応する場所から取り出したアドレスから各ファイルをロード※１チャプター１５０行
        for (int i=chapter*150; i < (chapter+1)*150 && scenarioFilePath[i] !="" ; i++)
        {
            StartCoroutine(LoadFile(scenarioFilePath[i]));
        }
    }


        private IEnumerator LoadFile(string path)
    {
        int i;
        // ファイルが無かったら終わる
        if (!System.IO.File.Exists(path))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルに問題があります。" + _FILE_HEADER + path);
            yield break;
        }

        // 指定したファイルをロードする
        WWW request = new WWW(_FILE_HEADER + path);

        // ロードが終わるまで待つ
        while (!request.isDone)
        {
            yield return new WaitForEndOfFrame();
        }


        //txtファイルの場合
        if (path.Substring(path.Length - 4) == ".txt")
        {
            // テキストを取り出す
            string text = request.text;

            // 読み込んだテキストファイルからstring配列を作成する
            scenarioText = text.Split('\n');
            yield break;
        }

        //pngファイルの場合
        if (path.Substring(path.Length - 4) == ".png")
        {
            // 画像を取り出す
            Texture2D texture = request.texture;

            // 読み込んだ画像からSpriteを作成する
            for (i = 0; i < 100; i++) { if (scenarioGraphic[i]==null) { break; } }
            scenarioGraphic[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            yield break;
        }

        //mp3ファイルの場合
        if (path.Substring(path.Length-4)==".mp3")
        {
            for (i = 0; i < 10; i++) { if (scenarioAudio[i] == null) { break; } }
            scenarioAudio[i] = request.GetAudioClip(false, true);
            while (scenarioAudio[i].loadState == AudioDataLoadState.Loading)
            {
                // ロードが終わるまで待つ
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        
    }





}
