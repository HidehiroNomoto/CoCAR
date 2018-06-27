using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosceneManager : MonoBehaviour
{
    public string[] scenarioText = new string[100];          //シナリオテキスト保存変数
    public AudioClip[] scenarioAudio = new AudioClip[40];    //シナリオＢＧＭ・ＳＥ保存変数
    public Sprite[] scenarioGraphic = new Sprite[100];       //シナリオ画像保存変数
    public string[] scenarioFilePath = new string[100];      //シナリオ用ファイルのアドレス
    public bool sentenceEnd=false;                           //文の処理が終了したか否か
    public bool pushObjectFlag = false;
    GameObject obj;
    GameObject objText;
    GameObject objTextBox;
    GameObject[] objCharacter = new GameObject[5];
    GameObject objBackImage;
    GameObject objBackText;
    GameObject objItem;
    GameObject objCanvas;
    const string _FILE_HEADER = "C:\\Users\\hoto\\Documents\\GitHub\\CoCAR\\CallOfCthulhuAR\\Assets\\Scenario\\";                      //ファイル場所の頭
    const int CHARACTER_Y = -300;

    // Use this for initialization
    void Start()
    {
        obj = GameObject.Find("error").gameObject as GameObject;
        for (int i = 0; i < 5; i++) { objCharacter[i] = GameObject.Find("Chara" + (i + 1).ToString()).gameObject as GameObject; objCharacter[i].gameObject.SetActive(false); }
        objText = GameObject.Find("MainText").gameObject as GameObject;
        objTextBox = GameObject.Find("TextBox").gameObject as GameObject;
        objBackImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBackText = GameObject.Find("BackText").gameObject as GameObject; objBackText.gameObject.SetActive(false);
        objItem = GameObject.Find("Item").gameObject as GameObject; objItem.gameObject.SetActive(false);
        objCanvas = GameObject.Find("CanvasDraw").gameObject as GameObject;
        StartCoroutine(MainCoroutine());
    }


    // Update is called once per frame
    void Update()
    {

    }

    //ノベルゲーム処理
    private IEnumerator NovelGame()
    {
        for (int i = 1; i < 10000; i++)
        {
            sentenceEnd = false;
            if (scenarioText[i] == "[END]") { break; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Text:") { TextDraw(scenarioText[i].Substring(5)); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "BackText:") { BackTextDraw(scenarioText[i].Substring(9)); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Back:") { BackDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 4 && scenarioText[i].Substring(0, 4) == "BGM:") { BGMIn(int.Parse(scenarioText[i].Substring(4, 4))); BGMPlay(int.Parse(scenarioText[i].Substring(9))); sentenceEnd = true; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "BGMStop") { BGMOut(int.Parse(scenarioText[i].Substring(8, 4)));sentenceEnd = true; }
            if (scenarioText[i].Length > 3 && scenarioText[i].Substring(0, 3) == "SE:") { SEPlay(int.Parse(scenarioText[i].Substring(3))); sentenceEnd = true; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 5) == "Chara") { CharacterDraw(int.Parse(scenarioText[i].Substring(9)), int.Parse(scenarioText[i].Substring(5, 1))); StartCoroutine(CharacterMove(int.Parse(scenarioText[i].Substring(5, 1)), scenarioText[i].Substring(7, 1))); }//Chara2ならポジション2、Chara5ならポジション5...。:の後（6文字目以降）は立ち絵の指定
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Item:") { ItemDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Shake") { StartCoroutine(ShakeScreen()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Jump:") { StartCoroutine(CharacterJump(int.Parse(scenarioText[i].Substring(5, 1)))); }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Hantei:") { Hantei(50); StartCoroutine(PushWait()); }
            while (sentenceEnd == false) { yield return null; }
            objBackText.gameObject.SetActive(false);//背景テキストは出っ放しにならない
        }
    }

    private int Hantei(int target)
    {
        int dice;
        Utility u1 = GetComponent<Utility>();
        objTextBox.gameObject.SetActive(true);
        dice =u1.DiceRoll(1, 100);
        if (dice > target)
        {
            objText.GetComponent<Text>().text = "<color=#ff0000ff>DiceRoll:1D100→  " + dice.ToString() + " > " + target.ToString() + "   （失敗）</color>";
            if (dice > 95)
            {
                objText.GetComponent<Text>().text = "<color=#990000ff>DiceRoll:1D100→  " + dice.ToString() + " >> " + target.ToString() + "   （大失敗）</color>";
                return -2;
            }
            return -1;
        }
        if (dice <= target)
        {
            objText.GetComponent<Text>().text = "<color=#000099ff>DiceRoll:1D100→  " + dice.ToString() + " <= " + target.ToString() + "   （成功）</color>";
            objText.GetComponent<Text>().color = new Color(0.2f, 0.2f, 1.0f);
            if (dice <= 5)
            {
                objText.GetComponent<Text>().text = "<color=#0000ffff>DiceRoll:1D100→  " + dice.ToString() + " << " + target.ToString() + "   （大成功）</color>";
                objText.GetComponent<Text>().color = new Color(0.4f, 0.4f, 1.0f);
                return 2;
            }
            return 1;
        }
        return 0;
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

    private void CharacterDraw(int character, int position)
    {
        if (character == -1) { objCharacter[position - 1].gameObject.SetActive(false); return; }
        objCharacter[position - 1].gameObject.SetActive(true);
        objCharacter[position - 1].GetComponent<Image>().sprite = scenarioGraphic[character];
    }

    private void ItemDraw(int item)
    {
        if (item == -1) { objItem.gameObject.SetActive(false); return; }
        objItem.gameObject.SetActive(true);
        objItem.GetComponent<Image>().sprite = scenarioGraphic[item];
    }

    private void BGMPlay(int bgm)
    {
        Utility u1 = GetComponent<Utility>();
        u1.BGMPlay(scenarioAudio[bgm]);
    }

    private void BGMIn(int time)
    {
        Utility u1 = GetComponent<Utility>();
        StartCoroutine(u1.BGMFadeIn(time));
    }

    private void BGMOut(int time)
    {
        Utility u1 = GetComponent<Utility>();
        StartCoroutine(u1.BGMFadeOut(time));
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


    private IEnumerator CharacterMove(int position, string lr)
    {
        for (int i = 0; i < 5; i++)//キャラクター移動
        {
            if (lr == "L")
            {
                objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 + i * 6 - 300 - 30, CHARACTER_Y, 0);
            }
            if (lr == "R")
            {
                objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - i * 6 - 300 + 30, CHARACTER_Y, 0);
            }
            if (lr == "N") {; }//Nなら動きなし
            yield return null;
        }
        objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y, 0);
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
            objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y + i * 2, 1);
            yield return null;
        }
        for (int i = 7; i > 0; i--)
        {
            objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y + i * 2, 1);
            yield return null;
        }
        objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y, 1);
        sentenceEnd = true;
    }


    private IEnumerator MainCoroutine()
    {
        //シナリオデータ読込
        yield return StartCoroutine(LoadScenario("loadfile.txt", PlayerPrefs.GetInt("Chapter", 0)));
        //シナリオ処理
        yield return StartCoroutine(NovelGame());
    }



    //目次ファイルを読み込み、進行度に合わせてファイルを拾ってくる。
    private IEnumerator LoadScenario(string path, int chapter)
    {
        // 目次ファイルが無かったら終わる
        if (!System.IO.File.Exists(_FILE_HEADER + path))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルが見当たりません。" + _FILE_HEADER + path);
            yield break;
        }

        // 目次ファイルをロードする
        WWW request = new WWW(_FILE_HEADER + path);

        while (!request.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        // テキストを取り出す
        string text = request.text;

        // 読み込んだ目次テキストファイルからstring配列を作成する
        scenarioFilePath = text.Split('\n');

        //chapterに対応する場所から取り出したアドレスから各ファイルをロード※１チャプター１００行
        for (int i = chapter * 100; i < (chapter + 1) * 100; i++)
        {
            if (scenarioFilePath[i] == "[END]") { break; }
            yield return StartCoroutine(LoadFile(scenarioFilePath[i].Replace("\r", "").Replace("\n", "")));
        }
    }


    private IEnumerator LoadFile(string path)
    {
        int i;
        // ファイルが無かったら終わる
        if (!System.IO.File.Exists(_FILE_HEADER + path))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルに問題があります。" + _FILE_HEADER + path);
        }

        // 指定したファイルをロードする
        WWW request = new WWW(_FILE_HEADER + path);

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
        }

        //pngファイルの場合
        if (path.Substring(path.Length - 4) == ".png")
        {
            // 画像を取り出す
            Texture2D texture = request.texture;

            // 読み込んだ画像からSpriteを作成する
            for (i = 0; i < 100; i++) { if (scenarioGraphic[i] == null) { break; } }
            scenarioGraphic[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        //mp3ファイルの場合
        if (path.Substring(path.Length - 4) == ".mp3" || path.Substring(path.Length - 4) == ".ogg")
        {
            yield return StartCoroutine(LoadBGM(request));
        }
        yield return null;
    }

    private IEnumerator LoadBGM(WWW request)
    {
        int i;
        for (i = 0; i < 10; i++) { if (scenarioAudio[i] == null) { break; } }
        scenarioAudio[i] = request.GetAudioClip(false, true);
        while (scenarioAudio[i].loadState == AudioDataLoadState.Loading)
        {
            // ロードが終わるまで待つ
            yield return new WaitForEndOfFrame();
        }

        if (scenarioAudio[i].loadState != AudioDataLoadState.Loaded)
        {
            // 読み込み失敗
            Debug.Log("Failed to Load!");
            yield break;
        }
    }
    //画面が押されたかチェックするコルーチン
    public IEnumerator PushWait()
    {
        while (true)//ブレークするまでループを続ける。
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                yield return null;//本体に処理を返して他のオブジェクトのイベントトリガーを確認。
                if (pushObjectFlag == false)//フラグが立っていたらオブジェクト処理のためのタップだったと判定。
                {
                    sentenceEnd = true;
                    yield break;//falseならコルーチン脱出
                }
                else
                {
                    yield return null;//trueならコルーチン継続
                }
            }
            else
            {
                yield return null;
            }
        }
    }
    //データを２重で読み込んでる。なぜ？



}