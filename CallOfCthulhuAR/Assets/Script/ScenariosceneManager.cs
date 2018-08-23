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
    GameObject objRollText;
    GameObject objName;
    GameObject[] objDice = new GameObject[2];
    GameObject[] objBox=new GameObject[4];
    public Sprite[] moveDice10Graphic = new Sprite[7];
    public Sprite[] dice10Graphic = new Sprite[10];
    public Sprite[] moveDice6Graphic = new Sprite[8];
    public Sprite[] dice6Graphic = new Sprite[6];
    public int selectNum=1;
    public string[] buttonText = new string[4];
    public string[] hanteiText = new string[2];
    public string[] serifuText = new string[2];
    const string _FILE_HEADER = "C:\\Users\\hoto\\Documents\\GitHub\\CoCAR\\CallOfCthulhuAR\\Assets\\Scenario\\";                      //ファイル場所の頭
    const int CHARACTER_Y = -300;

    // Use this for initialization
    void Start()
    {
        objName= GameObject.Find("CharacterName").gameObject as GameObject;
        objRollText = GameObject.Find("Rolltext").gameObject as GameObject; objRollText.gameObject.SetActive(false);
        obj = GameObject.Find("error").gameObject as GameObject;
        for (int i = 0; i < 5; i++) { objCharacter[i] = GameObject.Find("Chara" + (i + 1).ToString()).gameObject as GameObject; objCharacter[i].gameObject.SetActive(false); }
        objText = GameObject.Find("MainText").gameObject as GameObject;
        objTextBox = GameObject.Find("TextBox").gameObject as GameObject;
        objBackImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBackText = GameObject.Find("BackText").gameObject as GameObject; objBackText.gameObject.SetActive(false);
        objItem = GameObject.Find("Item").gameObject as GameObject; objItem.gameObject.SetActive(false);
        objCanvas = GameObject.Find("CanvasDraw").gameObject as GameObject;
        for (int i = 0; i < 4; i++) { objBox[i] = GameObject.Find("select" + (i + 1).ToString()).gameObject as GameObject; objBox[i].gameObject.SetActive(false); }
        for (int i = 0; i < 2; i++) { objDice[i] = GameObject.Find("Dice" + (i + 1).ToString()).gameObject as GameObject; objDice[i].gameObject.SetActive(false); }
        StartCoroutine(MainCoroutine());
    }


    // Update is called once per frame
    void Update()
    {

    }

    //ノベルゲーム処理
    private IEnumerator NovelGame()
    {
        for (int i = 1; i < 100; i++)
        {
            for (int j = 0; j < 4; j++) { buttonText[j] = null; }
            sentenceEnd = false;
            if (scenarioText[i] == "[END]") { break; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Text:") {serifuText=scenarioText[i].Substring(5).Split(','); ; TextDraw(serifuText[0],serifuText[1]); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "BackText:") { BackTextDraw(scenarioText[i].Substring(9)); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Back:") { BackDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 4 && scenarioText[i].Substring(0, 4) == "BGM:") { BGMIn(int.Parse(scenarioText[i].Substring(4, 4))); BGMPlay(int.Parse(scenarioText[i].Substring(9))); sentenceEnd = true; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "BGMStop") { BGMOut(int.Parse(scenarioText[i].Substring(8, 4)));sentenceEnd = true; }
            if (scenarioText[i].Length > 3 && scenarioText[i].Substring(0, 3) == "SE:") { SEPlay(int.Parse(scenarioText[i].Substring(3))); sentenceEnd = true; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 5) == "Chara") { CharacterDraw(int.Parse(scenarioText[i].Substring(9)), int.Parse(scenarioText[i].Substring(5, 1))); StartCoroutine(CharacterMove(int.Parse(scenarioText[i].Substring(5, 1)), scenarioText[i].Substring(7, 1))); }//Chara2ならポジション2、Chara5ならポジション5...。:の後（6文字目以降）は立ち絵の指定
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Item:") { ItemDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Shake") { StartCoroutine(ShakeScreen()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Jump:") { StartCoroutine(CharacterJump(int.Parse(scenarioText[i].Substring(5, 1)))); }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Select:") { buttonText=scenarioText[i].Substring(7).Split(','); StartCoroutine(Select(buttonText[0],buttonText[1],buttonText[2],buttonText[3].Replace("\r", "").Replace("\n", ""))); while (sentenceEnd == false) { yield return null; };i += selectNum;continue; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "NextFile:") { yield return StartCoroutine(LoadFile(scenarioText[i].Substring(9).Replace("\r", "").Replace("\n", "")));i = 0;sentenceEnd = true; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Hantei:") { hanteiText=scenarioText[i].Substring(7).Split(','); i += Hantei(hanteiText[0],int.Parse(hanteiText[1].Replace("\r", "").Replace("\n", ""))); while (sentenceEnd == false) { yield return null; };sentenceEnd = false; StartCoroutine(PushWait()); }
            while (sentenceEnd == false) { yield return null; }
            objBackText.gameObject.SetActive(false);//背景テキストは出っ放しにならない
            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
        }
    }

    public void SelePush(int selectnum)
    {
        selectNum=selectnum;
    }

    private IEnumerator Select(string choiceA,string choiceB,string choiceC,string choiceD)
    {
        objBox[0].gameObject.SetActive(true); objBox[0].GetComponentInChildren<Text>().text = choiceA;
        if (choiceB.Length>0) { objBox[1].gameObject.SetActive(true); objBox[1].GetComponentInChildren<Text>().text = choiceB; }
        if (choiceC.Length>0) { objBox[2].gameObject.SetActive(true); objBox[2].GetComponentInChildren<Text>().text = choiceC; }
        if (choiceD.Length>0) { objBox[3].gameObject.SetActive(true); objBox[3].GetComponentInChildren<Text>().text = choiceD; }
        //ボタンがクリックされるまでループ。
        selectNum = -1;
        while (selectNum == -1)
        {
            yield return null;
        }
        for (int i = 0; i < 4; i++) { objBox[i].gameObject.SetActive(false); }
        sentenceEnd = true;
    }

    private int Hantei(string targetStr,int bonus)
    {
        int dice;
        int target=0;
        string bonusStr="";
        if (targetStr == "言いくるめ") { target=PlayerPrefs.GetInt("Skill0",target); }
        if (targetStr == "医学") { target=PlayerPrefs.GetInt("Skill1", target); }
        if (targetStr == "運転") { target=PlayerPrefs.GetInt("Skill2", target); }
        if (targetStr == "応急手当") { target=PlayerPrefs.GetInt("Skill3", target); }
        if (targetStr == "オカルト") { target=PlayerPrefs.GetInt("Skill4", target); }
        if (targetStr == "回避") { target=PlayerPrefs.GetInt("Skill5", target); }
        if (targetStr == "化学") { target=PlayerPrefs.GetInt("Skill6", target); }
        if (targetStr == "鍵開け") { target=PlayerPrefs.GetInt("Skill7", target); }
        if (targetStr == "隠す") { target=PlayerPrefs.GetInt("Skill8", target); }
        if (targetStr == "隠れる") { target=PlayerPrefs.GetInt("Skill9", target); }
        if (targetStr == "機械修理") { target=PlayerPrefs.GetInt("Skill10", target); }
        if (targetStr == "聞き耳") { target=PlayerPrefs.GetInt("Skill11", target); }
        if (targetStr == "芸術") { target=PlayerPrefs.GetInt("Skill12", target); }
        if (targetStr == "経理") { target=PlayerPrefs.GetInt("Skill13", target); }
        if (targetStr == "考古学") { target=PlayerPrefs.GetInt("Skill14", target); }
        if (targetStr == "コンピューター") { target=PlayerPrefs.GetInt("Skill15", target); }
        if (targetStr == "忍び歩き") { target=PlayerPrefs.GetInt("Skill16", target); }
        if (targetStr == "写真術") { target=PlayerPrefs.GetInt("Skill17", target); }
        if (targetStr == "重機械操作") { target=PlayerPrefs.GetInt("Skill18", target); }
        if (targetStr == "乗馬") { target=PlayerPrefs.GetInt("Skill19", target); }
        if (targetStr == "信用") { target=PlayerPrefs.GetInt("Skill20", target); }
        if (targetStr == "心理学") { target=PlayerPrefs.GetInt("Skill21", target); }
        if (targetStr == "人類学") { target=PlayerPrefs.GetInt("Skill22", target); }
        if (targetStr == "水泳") { target=PlayerPrefs.GetInt("Skill23", target); }
        if (targetStr == "製作") { target=PlayerPrefs.GetInt("Skill24", target); }
        if (targetStr == "精神分析") { target=PlayerPrefs.GetInt("Skill25", target); }
        if (targetStr == "生物学") { target=PlayerPrefs.GetInt("Skill26", target); }
        if (targetStr == "説得") { target=PlayerPrefs.GetInt("Skill27", target); }
        if (targetStr == "操縦") { target=PlayerPrefs.GetInt("Skill28", target); }
        if (targetStr == "地質学") { target=PlayerPrefs.GetInt("Skill29", target); }
        if (targetStr == "跳躍") { target=PlayerPrefs.GetInt("Skill30", target); }
        if (targetStr == "追跡") { target=PlayerPrefs.GetInt("Skill31", target); }
        if (targetStr == "電気修理") { target=PlayerPrefs.GetInt("Skill32", target); }
        if (targetStr == "電子工学") { target=PlayerPrefs.GetInt("Skill33", target); }
        if (targetStr == "天文学") { target=PlayerPrefs.GetInt("Skill34", target); }
        if (targetStr == "投擲") { target=PlayerPrefs.GetInt("Skill35", target); }
        if (targetStr == "登攀") { target=PlayerPrefs.GetInt("Skill36", target); }
        if (targetStr == "図書館") { target=PlayerPrefs.GetInt("Skill37", target); }
        if (targetStr == "ナビゲート") { target=PlayerPrefs.GetInt("Skill38", target); }
        if (targetStr == "値切り") { target=PlayerPrefs.GetInt("Skill39", target); }
        if (targetStr == "博物学") { target=PlayerPrefs.GetInt("Skill40", target); }
        if (targetStr == "物理学") { target=PlayerPrefs.GetInt("Skill41", target); }
        if (targetStr == "変装") { target=PlayerPrefs.GetInt("Skill42", target); }
        if (targetStr == "法律") { target=PlayerPrefs.GetInt("Skill43", target); }
        if (targetStr == "ほかの言語") { target=PlayerPrefs.GetInt("Skill44", target); }
        if (targetStr == "母国語") { target=PlayerPrefs.GetInt("Skill45", target); }
        if (targetStr == "マーシャルアーツ") { target=PlayerPrefs.GetInt("Skill46", target); }
        if (targetStr == "目星") { target=PlayerPrefs.GetInt("Skill47", target); }
        if (targetStr == "薬学") { target=PlayerPrefs.GetInt("Skill48", target); }
        if (targetStr == "歴史") { target=PlayerPrefs.GetInt("Skill49", target); }
        if (targetStr == "火器") { target=PlayerPrefs.GetInt("Skill50", target); }
        if (targetStr == "格闘") { target=PlayerPrefs.GetInt("Skill51", target); }
        if (targetStr == "武器術") { target=PlayerPrefs.GetInt("Skill52", target); }
        if (targetStr == "クトゥルフ神話") { target=PlayerPrefs.GetInt("Skill53", target); }

        if (bonus > 0) { bonusStr = " + " + bonus.ToString(); }
        if (bonus < 0) { bonusStr = " - " + (-1*bonus).ToString(); }
        objRollText.gameObject.SetActive(true);
        objRollText.GetComponent<Text>().text = targetStr + "\n" + "<color=#88ff88ff>" + target.ToString() + "</color>";
        Utility u1 = GetComponent<Utility>();
        objTextBox.gameObject.SetActive(true);
        dice =u1.DiceRoll(1, 100);
        if (dice != 100) { StartCoroutine(DiceEffect(0, 10, dice / 10)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
        StartCoroutine(DiceEffect(1, 10, dice % 10));
        StartCoroutine(DiceText(dice, target, bonus,targetStr,bonusStr));
        if (dice > target + bonus)
        {
            if (dice > 95)
            {
                return 3;
            }
            return 2;
        }
        if (dice <= target+ bonus)
        {
            if (dice <= 5)
            {
                return 0;
            }
            return 1;
        }
        return 0;
    }

    private IEnumerator DiceText(int dice, int target, int bonus,string targetStr,string bonusStr)
    {
        for (int j = 0; j < 50; j++) { yield return null; }
        if (dice > target + bonus)
        {
            objText.GetComponent<Text>().text = "<color=#ff0000ff>[DiceRoll]\n1D100→　" + dice.ToString() + " > " + target.ToString() + bonusStr + " (<" + targetStr + ">" + bonusStr + ")\n<size=72>（失敗）</size></color>";
            if (dice > 95)
            {
                objText.GetComponent<Text>().text = "<color=#990000ff>DiceRoll:1D100→  " + dice.ToString() + " >> " + target.ToString() + bonusStr + "\n　　　　　　　　" + targetStr + bonusStr + "   （大失敗）</color>";
            }
        }
        if (dice <= target + bonus)
        {
            objText.GetComponent<Text>().text = "<color=#000099ff>DiceRoll:1D100→  " + dice.ToString() + " <= " + target.ToString() + bonusStr + "\n　　　　　　　　" + targetStr + bonusStr + "   （成功）</color>";
            if (dice <= 5)
            {
                objText.GetComponent<Text>().text = "<color=#0000ffff>DiceRoll:1D100→  " + dice.ToString() + " << " + target.ToString() + bonusStr + "\n　　　　　　　　" + targetStr + bonusStr + "   （大成功）</color>";
            }
        }
        sentenceEnd = true;
    }


        private IEnumerator DiceEffect(int dicenum,int dicetype,int num)
    {
        objDice[dicenum].gameObject.SetActive(true);

        if (dicetype == 10)
        {
            for (int i = 0; i < 7; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice10Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null; }
            }
            objDice[dicenum].GetComponent<Image>().sprite = dice10Graphic[num];

        }
        if (dicetype == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice6Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null; }
            }
            objDice[dicenum].GetComponent<Image>().sprite = dice6Graphic[num];
        }
    }




    private void TextDraw(string name,string text)
    {
        objTextBox.gameObject.SetActive(true);
        objText.GetComponent<Text>().text = text;
        objName.GetComponent<Text>().text = name;
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



}