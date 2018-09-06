using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

//文字入力（シナリオ部）、呪文（戦闘用）、アーカイブ読込（作成は別プログラム）


public class ScenariosceneManager : MonoBehaviour
{
    public string[] scenarioText = new string[100];          //シナリオテキスト保存変数
    public AudioClip[] scenarioAudio = new AudioClip[40];    //シナリオＢＧＭ・ＳＥ保存変数
    public Sprite[] scenarioGraphic = new Sprite[100];       //シナリオ画像保存変数
    public string[] scenarioFilePath = new string[100];      //シナリオ用ファイルのアドレス
    public bool sentenceEnd=false;                           //文の処理が終了したか否か
    public bool pushObjectFlag = false;
    public int battleFlag=-10; 
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
    GameObject objBGM;
    GameObject[] objDice = new GameObject[2];
    GameObject[] objBox=new GameObject[4];
    public AudioClip[] systemAudio = new AudioClip[10];
    public Sprite[] moveDice10Graphic = new Sprite[7];
    public Sprite[] dice10Graphic = new Sprite[10];
    public Sprite[] moveDice6Graphic = new Sprite[8];
    public Sprite[] dice6Graphic = new Sprite[6];
    public Sprite[] moveDice4Graphic = new Sprite[7];
    public Sprite[] dice4Graphic = new Sprite[4];
    public int selectNum=1;
    const string _FILE_HEADER = "C:\\Users\\hoto\\Documents\\GitHub\\CoCAR\\CallOfCthulhuAR\\Assets\\Scenario\\";                      //ファイル場所の頭
    const int CHARACTER_Y = -300;

    // Use this for initialization
    void Start()
    {
        systemAudio[0] = Resources.Load<AudioClip>("kan"); systemAudio[1] = Resources.Load<AudioClip>("correct1");
        systemAudio[2] = Resources.Load<AudioClip>("incorrect1"); systemAudio[3] = Resources.Load<AudioClip>("switch1");
        systemAudio[4] = Resources.Load<AudioClip>("gun1"); systemAudio[5] = Resources.Load<AudioClip>("punch-high1");
        systemAudio[6] = Resources.Load<AudioClip>("sword-slash5"); systemAudio[7] = Resources.Load<AudioClip>("punch-swing1");
        systemAudio[8] = Resources.Load<AudioClip>("highspeed-movement1"); systemAudio[9] = Resources.Load<AudioClip>("sword-clash4");
        objName = GameObject.Find("CharacterName").gameObject as GameObject;
        objRollText = GameObject.Find("Rolltext").gameObject as GameObject; objRollText.gameObject.SetActive(false);
        obj = GameObject.Find("error").gameObject as GameObject;
        for (int i = 0; i < 5; i++) { objCharacter[i] = GameObject.Find("Chara" + (i + 1).ToString()).gameObject as GameObject; objCharacter[i].gameObject.SetActive(false); }
        objText = GameObject.Find("MainText").gameObject as GameObject;
        objTextBox = GameObject.Find("TextBox").gameObject as GameObject;
        objBackImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBackText = GameObject.Find("BackText").gameObject as GameObject; objBackText.gameObject.SetActive(false);
        objItem = GameObject.Find("Item").gameObject as GameObject; objItem.gameObject.SetActive(false);
        objCanvas = GameObject.Find("CanvasDraw").gameObject as GameObject;
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
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
        string[] buttonText = new string[4];
        string[] battleText = new string[14];
        string[] separateText = new string[2];
        string[] separate3Text = new string[3];
        DateTime dt;
        for (int i = 1; i < 100; i++)
        {
            for (int j = 0; j < 4; j++) { buttonText[j] = null; }
            sentenceEnd = false;
            if (scenarioText[i] == "[END]") { break; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Text:") { separateText = scenarioText[i].Substring(5).Split(','); TextDraw(separateText[0], separateText[1]); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "BackText:") { BackTextDraw(scenarioText[i].Substring(9)); StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Back:") { BackDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 4 && scenarioText[i].Substring(0, 4) == "BGM:") { BGMIn(int.Parse(scenarioText[i].Substring(4, 4))); BGMPlay(int.Parse(scenarioText[i].Substring(9))); sentenceEnd = true; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "BGMStop") { BGMOut(int.Parse(scenarioText[i].Substring(8, 4))); sentenceEnd = true; }
            if (scenarioText[i].Length > 3 && scenarioText[i].Substring(0, 3) == "SE:") { SEPlay(int.Parse(scenarioText[i].Substring(3))); sentenceEnd = true; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 5) == "Chara") { CharacterDraw(int.Parse(scenarioText[i].Substring(9)), int.Parse(scenarioText[i].Substring(5, 1))); StartCoroutine(CharacterMove(int.Parse(scenarioText[i].Substring(5, 1)), scenarioText[i].Substring(7, 1))); }//Chara2ならポジション2、Chara5ならポジション5...。:の後（6文字目以降）は立ち絵の指定
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Item:") { ItemDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Shake") { StartCoroutine(ShakeScreen()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Jump:") { StartCoroutine(CharacterJump(int.Parse(scenarioText[i].Substring(5, 1)))); }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Select:") { buttonText = scenarioText[i].Substring(7).Split(','); StartCoroutine(Select(buttonText[0], buttonText[1], buttonText[2], buttonText[3].Replace("\r", "").Replace("\n", ""),false)); while (sentenceEnd == false) { yield return null; }; SystemSEPlay(systemAudio[3]); i += selectNum; continue; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "NextFile:") { yield return StartCoroutine(LoadFile(scenarioText[i].Substring(9).Replace("\r", "").Replace("\n", ""))); i = 0; sentenceEnd = true; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Hantei:") { separateText = scenarioText[i].Substring(7).Split(','); i += Hantei(separateText[0], int.Parse(separateText[1].Replace("\r", "").Replace("\n", ""))); while (sentenceEnd == false) { yield return null; }; sentenceEnd = false; StartCoroutine(PushWait()); }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Battle:") { battleText = scenarioText[i].Substring(7).Split(','); battleFlag = -1; StartCoroutine(Battle(int.Parse(battleText[0]), int.Parse(battleText[1]), int.Parse(battleText[2]), int.Parse(battleText[3]), int.Parse(battleText[4]), int.Parse(battleText[5]), int.Parse(battleText[6]), int.Parse(battleText[7]), bool.Parse(battleText[8]), battleText[9], battleText[10], int.Parse(battleText[11]), int.Parse(battleText[12]), bool.Parse(battleText[13].Replace("\r", "").Replace("\n", "")))); while (battleFlag == -1) { yield return null; }; i += battleFlag;sentenceEnd = true; }
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "FlagBranch:") { i+=PlayerPrefs.GetInt(scenarioText[i].Substring(11).Replace("\r", "").Replace("\n", ""),0);sentenceEnd = true; }
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "FlagChange:"){ separateText = scenarioText[i].Substring(11).Split(','); PlayerPrefs.SetInt(separateText[0],int.Parse(separateText[1].Replace("\r", "").Replace("\n", ""))); sentenceEnd = true; }
            if (scenarioText[i].Length > 8 && scenarioText[i].Substring(0, 8) == "GetTime:"){ dt = DateTime.Now; PlayerPrefs.SetInt("Month", dt.Month); PlayerPrefs.SetInt("Day", dt.Day); PlayerPrefs.SetInt("Hour",dt.Hour); PlayerPrefs.SetInt("Minute", dt.Minute); sentenceEnd = true; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "FlagName:"){ separateText = scenarioText[i].Substring(9).Split(','); PlayerPrefs.SetInt(separateText[1].Replace("\r", "").Replace("\n", ""), PlayerPrefs.GetInt(separateText[0], 0)); sentenceEnd = true; }//フラグを別名で保存する
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "Difference:"){separate3Text = scenarioText[i].Substring(11).Split(',');i+=Difference(separate3Text);sentenceEnd = true; }
            if (scenarioText[i].Length > 13 && scenarioText[i].Substring(0, 13) == "StatusChange:"){separateText = scenarioText[i].Substring(13).Split(',');StartCoroutine(StatusChange(separateText));while (sentenceEnd == false) { yield return null; }; sentenceEnd = false; StartCoroutine(PushWait()); }//「StatusChange:正気度,-2D6」のように①変動ステータス、②変動値（○D○または固定値どちらでもプログラム側で適切な解釈をしてくれる）


            while (sentenceEnd == false) { yield return null; }
            objBackText.gameObject.SetActive(false);//背景テキストは出っ放しにならない
            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
        }
    }

    private IEnumerator StatusChange(string[] separateText)
    {
        int changeValue = 0;
        int changeValue2 = 0;
        int x1, y1, y2;
        string targetStr;
        Utility u1 = GetComponent<Utility>();
        string[] separate3Text;
        targetStr=SkillList(separateText[0]);
        if (int.TryParse(separateText[1].Replace("\r", "").Replace("\n", ""), out x1))
        {
            PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + x1);
            if (x1 >= 0)
            {
                TextDraw("", separateText[0] + "の能力が" + x1.ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
            }
            else
            {
                TextDraw("", separateText[0] + "の能力が" + x1.ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
            }
            for (int v = 0; v < 60; v++) { yield return null; }
        }
        else
        {
            separate3Text = separateText[1].Split('D');
            if (int.Parse(separate3Text[0]) >= 0) { y2 = 1; } else { y2 = -1; }
            if (int.Parse(separate3Text[0]) * y2 == 2 && int.Parse(separate3Text[1])!=100)
            {
                changeValue = u1.DiceRoll(1, int.Parse(separate3Text[1]));
                changeValue2 = u1.DiceRoll(1, int.Parse(separate3Text[1]));
                StartCoroutine(DiceEffect(0, int.Parse(separate3Text[1]), changeValue));
                StartCoroutine(DiceEffect(1, int.Parse(separate3Text[1]), changeValue2));
                for (int v = 0; v < 60; v++) { yield return null; }
                PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2);
                if (y2 > 0)
                {
                    TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "+" + changeValue2.ToString() + "=" + (changeValue + changeValue2).ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
                }
                else
                {
                    TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "+" + changeValue2.ToString() + "=" + (changeValue + changeValue2).ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
                }
                for (int v = 0; v < 60; v++) { yield return null; }
            }
            else
            {
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
                    PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2);
                    if (y2 > 0)
                    {
                        TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
                    }
                    else
                    {
                        TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）");
                    }
                    for (int v = 0; v < 60; v++) { yield return null; }
                }
            }
        }
        sentenceEnd = true;
    }

    private int Difference(string[] separate3Text)
    {
        int x1, x2;
        if (int.TryParse(separate3Text[0], out x1))
        {
            if (int.TryParse(separate3Text[1], out x2))
            {
                if (x1 >= x2 + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 1; }
            }
            else
            {
                if (x1 >= PlayerPrefs.GetInt(separate3Text[1], 0) + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 1; }
            }
        }
        else
        {
            if (int.TryParse(separate3Text[1], out x2))
            {
                if (PlayerPrefs.GetInt(separate3Text[0], 0) >= x2 + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 1; }
            }
            else
            {
                if (PlayerPrefs.GetInt(separate3Text[0], 0) >= PlayerPrefs.GetInt(separate3Text[1], 0) + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) {return 1; }
            }
        }
        return 0;
    }

    //戦闘処理
    private IEnumerator Battle(int enemyGraph, int enemyNum, int HP, int DEX, int AP, int ATDiceNum, int ATDice, int DB,bool humanFlag,string tokusyu,string tokusyuSkill,int tokusyuSkillBonus,int maxTurn,bool maxTurnWin)
    {
        int[] enemyHP = new int[enemyNum];
        int kill = 0;
        int sleep = 0;
        int playerHP = PlayerPrefs.GetInt("Status9", 3);
        int playerDEX = PlayerPrefs.GetInt("Status2", 3);
        int damage = 0;
        int avoid = 2;
        int detailAct = 0;
        bool cutFlag = false;
        for (int i = 0; i < enemyNum; i++)
        {
            objCharacter[i].gameObject.SetActive(true);
            objCharacter[i].GetComponent<Image>().sprite = scenarioGraphic[enemyGraph];
            enemyHP[i] = HP;
            StartCoroutine(Status(playerHP,0));
        }
        Utility u1 = GetComponent<Utility>();
        for (int x=0;x<maxTurn;x++)
        {
            firstSelect:
            cutFlag = false;
            selectNum = -1;
            TextDraw("1.行動選択", "");
            StartCoroutine(Select("攻撃", "回避","拘束", tokusyu,true)); while (selectNum==-1) { yield return null; }; SystemSEPlay(systemAudio[3]);
            if (selectNum == 0)
            {
                selectNum = -1;
                TextDraw("1.行動選択→2.攻撃選択", "");
                StartCoroutine(Select("火器", "格闘", "武器術", "戻る",true)); while (selectNum == -1) { yield return null; }; SystemSEPlay(systemAudio[3]);
                if (selectNum == 3) { goto firstSelect; }
                if (selectNum == 2) {cutFlag=true; }
                detailAct =selectNum;selectNum = 0;
                TextDraw("", "");
            }
            for (int v = 0; v < 50; v++) { yield return null; }
            if (DEX <= playerDEX && selectNum==0)
            {
                StartCoroutine(PlayerBattle(detailAct, enemyHP, humanFlag, enemyNum));
                while (selectNum == 0) { yield return null; }
                for (int v = 0; v < 100; v++) { yield return null; }
            }//攻撃１（相手より早い場合）

            if (DEX <= playerDEX && selectNum==2)
            {
                StartCoroutine(Catcher(enemyNum, humanFlag, enemyHP));
                while (selectNum == 2) { yield return null; }
                for (int v = 0; v < 100; v++) { yield return null; }
            }//拘束１（相手より早い場合）
            for (int i = 0; i < enemyNum; i++)
            {
                if ((enemyHP[i] > 0 && humanFlag==false) || enemyHP[i]>2)
                {
                    if (u1.DiceRoll(1, 100) < AP)
                    {
                        if (humanFlag==true && cutFlag==true)
                        {
                            sentenceEnd = false;
                            avoid = Hantei("武器術", 0);
                            objRollText.GetComponent<Text>().text = "受け流し\n（武器術）" + objRollText.GetComponent<Text>().text.Substring(3);
                            if (avoid >= 1) { cutFlag = false; }
                            while (sentenceEnd == false) { yield return null; }
                            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                            if (avoid <=1 )
                            {
                                sentenceEnd = false;
                                StartCoroutine(Cut(i));
                                for (int v = 0; v < 100; v++) { yield return null; }
                                continue;
                            }
                        }
                        if (selectNum==1)
                        {
                            sentenceEnd = false;
                            avoid = Hantei("回避", 0);
                            while (sentenceEnd == false) { yield return null; }
                            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                            if (avoid >= 2) { selectNum = -1; }         
                            if (avoid <= 1) { sentenceEnd = false; StartCoroutine(Avoid(i)); }
                        }
                        if (selectNum !=1 )
                        {
                            damage = u1.DiceRoll(ATDiceNum, ATDice);
                            sentenceEnd = false;
                            StartCoroutine(EnemyHit(i, damage));
                            StartCoroutine(Status(playerHP, damage));
                            playerHP -= damage;
                            if (playerHP <= 2) { break; }
                        }
                        if (selectNum==1 && avoid!=0) { selectNum = -1; }//スペシャルなら回避を連続でできる。
                    }
                    else
                    {
                        sentenceEnd = false;
                        StartCoroutine(EnemyMiss(i));
                    }
                    for (int v = 0; v < 100; v++) { yield return null; }
                }
            }//敵の攻撃
            if(selectNum == 0)
            {
                StartCoroutine(PlayerBattle(detailAct, enemyHP, humanFlag, enemyNum));
                while (selectNum == 0) {  yield return null; }
                for (int v = 0; v < 100; v++) { yield return null; }
            }//攻撃２（相手より遅い場合）
            if (selectNum == 2)
            {
                StartCoroutine(Catcher(enemyNum,humanFlag,enemyHP));
                while (selectNum == 2) { yield return null; }
                for (int v = 0; v < 100; v++) { yield return null; }
            }//拘束２（相手より遅い場合）
            if(selectNum==3)
            {
                sentenceEnd = false;
                if (Hantei(tokusyuSkill, tokusyuSkillBonus) < 2)
                {
                    while (sentenceEnd == false) { yield return null; }
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    battleFlag = 0; BattleEnd(); yield break;
                }
                while (sentenceEnd == false) { yield return null; }
                for (int v = 0; v < 100; v++) { yield return null; }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
            }//特殊行動に成功したら戦闘終了
            //戦闘終了判定
            for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag==true) { sleep++; } if (enemyHP[i] <= 0) { kill++; sleep--; } }
            if (sleep + kill == enemyNum)
            {
                BattleEnd();
                if (kill == enemyNum) { battleFlag = 1; yield break; }//皆殺し勝利
                if (sleep == enemyNum) { battleFlag = 3; yield break; }//全員捕縛勝利
                battleFlag = 2; yield break;//捕縛者あり勝利
            }//勝ち
            if (playerHP <= 2)
            {
                BattleEnd();
                if (playerHP <= 0) { battleFlag = 5; yield break; }//死亡敗北
                battleFlag = 4; yield break;//生存敗北
            }//負け
            sleep = 0;kill = 0;
        }
        //戦闘終了判定
        for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag==true) { sleep++; } if (enemyHP[i] <= 0) { kill++; sleep--; } }
        BattleEnd();
        if (maxTurnWin == true)
        {
            if (kill == enemyNum) { battleFlag = 1; yield break; }//皆殺し勝利
            if (sleep == enemyNum) { battleFlag = 3; yield break; }//全員捕縛勝利
            battleFlag = 2; yield break;//捕縛者あり勝利
        }
        else
        {
            if (playerHP <= 0) { battleFlag = 5; yield break; }//死亡敗北
            battleFlag = 4; yield break;//生存敗北
        }
    }

    private void BattleEnd()
    {
        for (int i = 0; i < 5; i++)
        {
            objCharacter[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayerBattle(int detailAct,int[] enemyHP,bool humanFlag,int enemyNum)
    {
        int damage;
        int y;
        int playerDB;
        int attack = 2;
        Utility u1 = GetComponent<Utility>();
        if (detailAct == 0)
        {
            for(int x=0;x<3;x++)
            {
                sentenceEnd = false;
                attack = Hantei("火器", 0);
                while (sentenceEnd == false) { yield return null; }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                if (attack == 2)
                {
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum-1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    StartCoroutine(PlayerMiss(y));
                }
                else
                {
                    damage = u1.DiceRoll(1, 10);
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum-1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    enemyHP[y] -= damage;
                    StartCoroutine(PlayerHit(y, damage, 0, 0));
                    for (int v = 0; v < 60; v++) { yield return null; }
                    if (attack == 0){x--;}
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)){ objCharacter[y].gameObject.SetActive(false); }
                    for (int i = 0; i < 60; i++) { yield return null; }
                }
            }
        }
        if (detailAct == 1)
        {
            for (int x = 0; x < 1; x++)
            {
                sentenceEnd = false;
                attack = Hantei("格闘", 0);
                while (sentenceEnd == false) { yield return null; }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                if (attack == 0) { x--; }
                if (attack == 2)
                {
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    StartCoroutine(PlayerMiss(y));
                }
                else
                {
                    damage = u1.DiceRoll(1, 6);
                    StartCoroutine(DiceEffect(0, 6, damage));
                    playerDB = 0;
                    if (PlayerPrefs.GetInt("Status8", 0) == 6) { playerDB = u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == 4) { playerDB = u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == -4) { playerDB = -u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, -playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == -6) { playerDB = -u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, -playerDB)); }
                    for (int i = 0; i < 60; i++) { yield return null; }
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    if (damage + playerDB > 0) { enemyHP[y] -= damage + playerDB; }
                    StartCoroutine(PlayerHit(y, damage, playerDB, detailAct));
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].gameObject.SetActive(false); }
                    for (int i = 0; i < 60; i++) { yield return null; }
                }
            }
        }
        if (detailAct == 2)
        {
            for (int x = 0; x < 1; x++)
            {
                sentenceEnd = false;
                attack = Hantei("武器術", 0);
                while (sentenceEnd == false) { yield return null; }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                if (attack ==0) {x--; }
                if (attack == 2)
                {
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    StartCoroutine(PlayerMiss(y));
                }
                else
                {
                    damage = u1.DiceRoll(1, 10);
                    if (damage < 10) { StartCoroutine(DiceEffect(0, 10, damage)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
                    playerDB = 0;
                    if (PlayerPrefs.GetInt("Status8", 0) == 6) { playerDB = u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == 4) { playerDB = u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == -4) { playerDB = -u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, -playerDB)); }
                    if (PlayerPrefs.GetInt("Status8", 0) == -6) { playerDB = -u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, -playerDB)); }
                    for (int i = 0; i < 60; i++) { yield return null; } 
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    if (damage + playerDB > 0) { enemyHP[y] -= damage + playerDB; }
                    StartCoroutine(PlayerHit(y, damage, playerDB, detailAct));
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].gameObject.SetActive(false); }
                    for (int i = 0; i < 60; i++) { yield return null; }
                }
            }
        }
        selectNum = -1;
    }
    private IEnumerator Status(int playerHP,int damage)
    {
        int maxHP = PlayerPrefs.GetInt("Status9", 0);
        string color1="";
        string color2="";
        if(playerHP<=0){ color1 = "<color=red>";color2 = "</color>"; }else if (playerHP < 2) { color1 = "<color=orange>";color2 = "</color>"; }else if(playerHP<=maxHP/2){ color1="<color=yellow>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
        objTextBox.GetComponent<Text>().text = color1 + "耐久力：" + playerHP.ToString() + " ／ " + maxHP.ToString() + color2;
        for (int i = 0; i < 6; i++) { yield return null; }
        while (damage > 0)
        {
            playerHP--; damage--;
            if (playerHP <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerHP < 2) { color1 = "<color=orange>"; color2 = "</color>"; } else if (playerHP <= maxHP/2) { color1 = "<color=yellow>"; color2 = "</color>"; } else { color1 = "";color2 = ""; }
            objTextBox.GetComponent<Text>().text =color1 + "耐久力：" + playerHP.ToString() + " ／ " + maxHP.ToString() + color2;
            for (int i=0;i<6;i++) { yield return null; }
        }
    }

    private IEnumerator Catcher(int enemyNum,bool humanFlag,int[] enemyHP)
    {
        int sleep=0;
        int kill=0;
        int catcher;
        int catcherNum;
        int y;
        for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] > 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
        if (humanFlag == false) {  TextDraw("", "拘束できる相手ではない……！"); for (int i = 0; i < 80; i++) { yield return null; } }//人外は拘束できない
        else
        {

            for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag == true) { sleep++; } if (enemyHP[i] <= 0) { kill++; sleep--; } }
            for (catcherNum = 0; catcherNum < enemyNum - sleep - kill; catcherNum++)
            {
                objCharacter[y].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                sentenceEnd = false;
                catcher = Hantei("格闘", +250);
                while (sentenceEnd == false) { yield return null; }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                objCharacter[y].GetComponent<Image>().color = new Color(1, 1, 1);
                y++;
                if (catcher == 2) { break; }
                if (catcher == 1) { catcherNum++; break; }
            }
            if (enemyNum - sleep - kill <= catcherNum) { for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 3) { enemyHP[i] = 2; } } }//全員捕獲した場合のみ、それらのHPを２にして戦闘終了処理へ
            else
            {
                TextDraw("", "<color=red>全員拘束には至らなかった……。</color>"); for (int i = 0; i < 80; i++) { yield return null; }
            }
        }
        selectNum = -1;
    }

    private IEnumerator PlayerMiss(int target)
    {
        SystemSEPlay(systemAudio[7]);
        TextDraw("", "攻撃を外した！");
        for (int v = 0; v < 100; v++) { yield return null; }
    }

    private IEnumerator EnemyMiss(int target)
    {
        SystemSEPlay(systemAudio[7]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "相手の攻撃は当たらなかった。");
        for (int v = 0; v < 100; v++) { yield return null; }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y, 0);
    }

    private IEnumerator PlayerHit(int target,int damage,int db,int detailAct)
    {
        SystemSEPlay(systemAudio[4+detailAct]);
        objCharacter[target].GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
        if (damage + db > 0)
        {
            TextDraw("", "damage→" + damage.ToString() + "+" + db.ToString() + "\n" + (damage + db).ToString() + "点のダメージを与えた。"); 
        }
        else
        {
            TextDraw("","ダメージを与えられない！"); 
        }
        for (int v = 0; v < 100; v++) { yield return null; }
        objCharacter[target].GetComponent<Image>().color = new Color(1, 1, 1);
    }

    private IEnumerator EnemyHit(int target, int damage)
    {
        SystemSEPlay(systemAudio[5]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", damage.ToString() + "点のダメージを受けた。");
        for (int v = 0; v < 100; v++)
        {
            if (v < 30) { objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 5 * (v % 2), 0); }
            yield return null;
        }
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y, 0);
    }

    private IEnumerator Cut(int target)
    {
        SystemSEPlay(systemAudio[9]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "攻撃を受け流した。");
        for (int v = 0; v < 100; v++) { yield return null; }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y, 0);
    }

    private IEnumerator Avoid(int target)
    {
        SystemSEPlay(systemAudio[8]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "攻撃を回避した。");
        for (int v = 0; v < 100; v++) { yield return null; }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(target * 150 - 300, CHARACTER_Y, 0);
    }

    private void SelectBoxMake(int choiceA, int choiceB, int choiceC, int choiceD,bool battleFlag)
    {
        if (battleFlag == false)
        {
            if (choiceA > 0 && choiceB > 0 && choiceC > 0 && choiceD > 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 500, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); objBox[3].GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0); for (int i = 0; i < 4; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB > 0 && choiceC > 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0); for (int i = 0; i < 3; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB > 0 && choiceC == 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); for (int i = 0; i < 2; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB == 0 && choiceC == 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); for (int i = 0; i < 1; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
        }
        else
        {
            objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, -250, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(-200, -400, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(200, -250, 0); objBox[3].GetComponent<RectTransform>().localPosition = new Vector3(200, -400, 0);
            for(int i=0;i<4;i++){ objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100); }
        }
    }

    public void SelePush(int selectnum)
    {
        selectNum=selectnum;
    }

    private IEnumerator Select(string choiceA,string choiceB,string choiceC,string choiceD,bool battleFlag)
    {
        objBox[0].gameObject.SetActive(true); objBox[0].GetComponentInChildren<Text>().text = choiceA;
        if (choiceB.Length>0) { objBox[1].gameObject.SetActive(true); objBox[1].GetComponentInChildren<Text>().text = choiceB; }
        if (choiceC.Length>0) { objBox[2].gameObject.SetActive(true); objBox[2].GetComponentInChildren<Text>().text = choiceC; }
        if (choiceD.Length>0) { objBox[3].gameObject.SetActive(true); objBox[3].GetComponentInChildren<Text>().text = choiceD; }
        SelectBoxMake(choiceA.Length, choiceB.Length, choiceC.Length, choiceD.Length,battleFlag);
        //ボタンがクリックされるまでループ。
        selectNum = -1;
        while (selectNum == -1)
        {
            yield return null;
        }
        for (int i = 0; i < 4; i++) { objBox[i].gameObject.SetActive(false); }
        sentenceEnd = true;
    }

    private void SystemSEPlay(AudioClip audio)
    {
        Utility u1 = GetComponent<Utility>();
        u1.SEPlay(audio);
    }

    private int SkillCheck(string targetStr)
    {
        int target = 0;
        int num = 0;
        string[] separate = new string[2];
        separate[0] = "";
        separate[1] = "";
        if (targetStr.Contains("*"))
        {
            separate = targetStr.Split('*');
        }
        else if (targetStr.Contains("/"))
        {
            separate = targetStr.Split('/');
        }
        else
        {
            separate[0] = targetStr;
        }
        target = PlayerPrefs.GetInt(SkillList(separate[0]), target); 
        int.TryParse(separate[1], out num);
        if (targetStr.Contains("*"))
        {
            target = target * num;
        }
        if (targetStr.Contains("/"))
        {
            target = target / num;
        }
        return target;
    }

    private string SkillList(string targetStr)
    {
        string target = "";
        if (targetStr == "言いくるめ") { target = "Skill0"; }
        if (targetStr == "医学") { target = "Skill1"; }
        if (targetStr == "運転") { target = "Skill2"; }
        if (targetStr == "応急手当") { target = "Skill3"; }
        if (targetStr == "オカルト") { target = "Skill4"; }
        if (targetStr == "回避") { target = "Skill5"; }
        if (targetStr == "化学") { target = "Skill6"; }
        if (targetStr == "鍵開け") { target = "Skill7"; }
        if (targetStr == "隠す") { target = "Skill8"; }
        if (targetStr == "隠れる") { target = "Skill9"; }
        if (targetStr == "機械修理") { target = "Skill10"; }
        if (targetStr == "聞き耳") { target = "Skill11"; }
        if (targetStr == "芸術") { target = "Skill12"; }
        if (targetStr == "経理") { target = "Skill13"; }
        if (targetStr == "考古学") { target = "Skill14"; }
        if (targetStr == "コンピューター") { target = "Skill15"; }
        if (targetStr == "忍び歩き") { target = "Skill16"; }
        if (targetStr == "写真術") { target = "Skill17"; }
        if (targetStr == "重機械操作") { target = "Skill18"; }
        if (targetStr == "乗馬") { target = "Skill19"; }
        if (targetStr == "信用") { target = "Skill20"; }
        if (targetStr == "心理学") { target = "Skill21"; }
        if (targetStr == "人類学") { target = "Skill22"; }
        if (targetStr == "水泳") { target = "Skill23"; }
        if (targetStr == "製作") { target = "Skill24"; }
        if (targetStr == "精神分析") { target = "Skill25"; }
        if (targetStr == "生物学") { target = "Skill26"; }
        if (targetStr == "説得") { target = "Skill27"; }
        if (targetStr == "操縦") { target = "Skill28"; }
        if (targetStr == "地質学") { target = "Skill29"; }
        if (targetStr == "跳躍") { target = "Skill30"; }
        if (targetStr == "追跡") { target = "Skill31"; }
        if (targetStr == "電気修理") { target = "Skill32"; }
        if (targetStr == "電子工学") { target = "Skill33"; }
        if (targetStr == "天文学") { target = "Skill34"; }
        if (targetStr == "投擲") { target = "Skill35"; }
        if (targetStr == "登攀") { target = "Skill36"; }
        if (targetStr == "図書館") { target = "Skill37"; }
        if (targetStr == "ナビゲート") { target = "Skill38"; }
        if (targetStr == "値切り") { target = "Skill39"; }
        if (targetStr == "博物学") { target = "Skill40"; }
        if (targetStr == "物理学") { target = "Skill41"; }
        if (targetStr == "変装") { target = "Skill42"; }
        if (targetStr == "法律") { target = "Skill43"; }
        if (targetStr == "ほかの言語") { target = "Skill44"; }
        if (targetStr == "母国語") { target = "Skill45"; }
        if (targetStr == "マーシャルアーツ") { target = "Skill46"; }
        if (targetStr == "目星") { target = "Skill47"; }
        if (targetStr == "薬学") { target = "Skill48"; }
        if (targetStr == "歴史") { target = "Skill49"; }
        if (targetStr == "火器") { target = "Skill50"; }
        if (targetStr == "格闘") { target = "Skill51"; }
        if (targetStr == "武器術") { target = "Skill52"; }
        if (targetStr == "クトゥルフ神話") { target = "Skill53"; }
        if (targetStr == "STR") { target = "Status0"; }
        if (targetStr == "DEX") { target = "Status2"; }
        if (targetStr == "CON") { target = "Status1"; }
        if (targetStr == "POW") { target = "Status5"; }
        if (targetStr == "INT") { target = "Status3"; }
        if (targetStr == "EDU") { target = "Status7"; }
        if (targetStr == "SIZ") { target = "Status6"; }
        if (targetStr == "APP") { target = "Status4"; }
        if (targetStr == "MP") { target = "Status10"; }
        if (targetStr == "HP") { target = "Status9"; }
        return target;
    }

    private int Hantei(string targetStr,int bonus)
    {
        int dice;
        int target=0;
        string bonusStr="";
        target=SkillCheck(targetStr);
        if (bonus > 0) { bonusStr = " + " + bonus.ToString(); }
        if (bonus < 0) { bonusStr = " - " + (-1*bonus).ToString(); }
        objRollText.gameObject.SetActive(true);
        if (target > -bonus) { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + (target + bonus).ToString() + "</color>"; } else { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + "自動失敗" + "</color>"; }
        Utility u1 = GetComponent<Utility>();
        objTextBox.gameObject.SetActive(true);
        dice =u1.DiceRoll(1, 100);
        if (dice != 100) { StartCoroutine(DiceEffect(0, 10, dice / 10)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
        StartCoroutine(DiceEffect(1, 10, dice % 10));
        StartCoroutine(DiceText(dice, target, bonus,targetStr,bonusStr));
        if (dice > target + bonus)
        {
            return 2;
        }
        if (dice <= target+ bonus)
        {
            if (dice <= (target+bonus)/5)
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
            objText.GetComponent<Text>().text = "<color=#ff0000ff>[DiceRoll]\n1D100→　" + dice.ToString() + " > " + (target + bonus).ToString() + " (<" + targetStr + ">" + bonusStr + ")\n<size=72>（失敗）</size></color>";
            for (int j = 0; j < 40; j++) { yield return null; }
            SystemSEPlay(systemAudio[2]);
        }
        if (dice <= target + bonus)
        {
            objText.GetComponent<Text>().text = "<color=#000099ff>DiceRoll:1D100→  " + dice.ToString() + " <= " + (target + bonus).ToString() + "\n　　　　　　　　" + targetStr + bonusStr + "   （成功）</color>";
            if (dice <= (target+bonus)/5)
            {
                objText.GetComponent<Text>().text = "<color=#0000ffff>DiceRoll:1D100→  " + dice.ToString() + " << " + (target + bonus).ToString() + "\n　　　　　　　　" + targetStr + bonusStr + "   （スペシャル）</color>";
            }
            for (int j = 0; j < 40; j++) { yield return null; }
            SystemSEPlay(systemAudio[1]);
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
            objDice[dicenum].GetComponent<Image>().sprite = dice6Graphic[num-1];
        }
        if (dicetype == 4)
        {
            for (int i = 0; i < 7; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice4Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null; }
            }
            objDice[dicenum].GetComponent<Image>().sprite = dice4Graphic[num-1];
        }
        SystemSEPlay(systemAudio[0]);
    }




    private void TextDraw(string name,string text)
    {
        objTextBox.gameObject.SetActive(true);
        objText.GetComponent<Text>().text = text;
        objName.GetComponent<Text>().text = "　" + name;
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
        BGMManager b1 = objBGM.GetComponent<BGMManager>();
        //シナリオデータ読込
        yield return StartCoroutine(LoadScenario(b1.scenarioName));
        //シナリオ処理
        yield return StartCoroutine(NovelGame());
    }



    //目次ファイルを読み込み、進行度に合わせてファイルを拾ってくる。
    private IEnumerator LoadScenario(string path)
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

        //アドレスから各ファイルをロード※１チャプター１００行
        for (int i = 0; i < scenarioFilePath.Length; i++)
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