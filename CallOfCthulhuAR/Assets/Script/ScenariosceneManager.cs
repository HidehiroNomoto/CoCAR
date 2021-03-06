﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ScenariosceneManager : MonoBehaviour
{
    const int STATUSNUM = 12;
    const int SKILLNUM = 54;
    public string[] scenarioText = new string[100];          //シナリオテキスト保存変数
    public AudioClip[] scenarioAudio = new AudioClip[40];    //シナリオＢＧＭ・ＳＥ保存変数
    public Sprite[] scenarioGraphic = new Sprite[100];       //シナリオ画像保存変数
    private string[] scenarioGraphicToPath = new string[100];//シナリオ画像番号と画像パスを連携させるための変数（Item関連に使う）
    public string[] scenarioFilePath = new string[140];      //シナリオ用ファイルのアドレス
    public bool sentenceEnd=false;                           //文の処理が終了したか否か
    public int battleFlag=-1;
    public int SANCheckFlag = -1;
    GameObject obj;
    GameObject objText;
    GameObject objTextBox;
    GameObject[] objCharacter = new GameObject[5];
    GameObject objBackImage;
    GameObject objBackText;
    GameObject objCanvas;
    GameObject objRollText;
    GameObject objName;
    GameObject objBGM;
    GameObject[] objDice = new GameObject[2];
    GameObject[] objBox=new GameObject[4];
    GameObject objInput;
    GameObject objSkip;
    public GameObject ScreenSizeChanger;
    public GameObject objTitleBack;
    public GameObject objStatusHP;
    public GameObject objStatusSAN;
    public GameObject objStatusName;
    public GameObject objMad;
    public AudioClip[] systemAudio = new AudioClip[11];
    public AudioClip mad;
    public Sprite[] moveDice10Graphic = new Sprite[7];
    public Sprite[] dice10Graphic = new Sprite[10];
    public Sprite[] moveDice6Graphic = new Sprite[8];
    public Sprite[] dice6Graphic = new Sprite[6];
    public Sprite[] moveDice4Graphic = new Sprite[7];
    public Sprite[] dice4Graphic = new Sprite[4];
    public Sprite skip;
    public Sprite play;
    public GameObject objSkipImage;
    public GameObject objDiceButton;
    public GameObject objProxySkillButton;
    public bool skipFlag = false;
    public bool skipFlag2 = false;
    public bool backLogCSFlag = false;
    public int selectNum=1;
    private int backNum=-1;
    private int logNum=0;
    string _FILE_HEADER;
    const int CHARACTER_Y = 585;
    private int gNum = 0;
    private int sNum = 0;
    private bool pushButton;
    private string sectionName="";
    public List<string> flagname=new List<string>();//イベントが異常終了した際にフラグ等をイベント前まで戻すための一時保存
    public List<string> flagvalue = new List<string>();//イベントが異常終了した際にフラグ等をイベント前まで戻すための一時保存
    public GameObject inputBox;
    public AudioClip mp3Dammy;
    public List<string> tmpMP3Path = new List<string>();
    public GameObject objReview;
    private int ask = 0;
    private int hanteiDice=1;
    public int hanteikekka = 2;
    private bool hanteiWait = true;
    private string proxySkill;
    private string proxyBase;
    private int baseBonus;
    private int basemulti;

    // Use this for initialization
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android) { inputBox.GetComponent<Text>().raycastTarget = false; }
        _FILE_HEADER = PlayerPrefs.GetString("[system]進行中シナリオ", "");                      //ファイル場所の頭
        if (_FILE_HEADER == null || _FILE_HEADER == "") {  GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene"); }
        logNum = PlayerPrefs.GetInt("[system]最新ログ番号", 0);
        objSkip = GameObject.Find("SkipText").gameObject as GameObject;
        objStatusName.GetComponent<Text>().text = PlayerPrefs.GetString("[system]PlayerCharacterName", "名無し");
        if (PlayerPrefs.GetInt("[system]skipFlag", 0) == 1) { skipFlag = true; objSkip.GetComponent<Text>().text = "<color=red>既読Skip\n<ON></color>";objSkipImage.GetComponent<Image>().sprite = skip; }
        systemAudio[0] = Resources.Load<AudioClip>("kan"); systemAudio[1] = Resources.Load<AudioClip>("correct1");
        systemAudio[2] = Resources.Load<AudioClip>("incorrect1"); systemAudio[3] = Resources.Load<AudioClip>("switch1");
        systemAudio[4] = Resources.Load<AudioClip>("gun1"); systemAudio[5] = Resources.Load<AudioClip>("punch-high1");
        systemAudio[6] = Resources.Load<AudioClip>("sword-slash5"); systemAudio[7] = Resources.Load<AudioClip>("punch-swing1");
        systemAudio[8] = Resources.Load<AudioClip>("highspeed-movement1"); systemAudio[9] = Resources.Load<AudioClip>("sword-clash4");
        systemAudio[10] = Resources.Load<AudioClip>("magic");
        objName = GameObject.Find("CharacterName").gameObject as GameObject;
        objRollText = GameObject.Find("Rolltext").gameObject as GameObject; objRollText.gameObject.SetActive(false);
        obj = GameObject.Find("error").gameObject as GameObject;
        for (int i = 0; i < 5; i++) { objCharacter[i] = GameObject.Find("Chara" + (i + 1).ToString()).gameObject as GameObject; objCharacter[i].gameObject.SetActive(false); }
        objInput= GameObject.Find("Input").gameObject as GameObject; objInput.gameObject.SetActive(false);
        objText = GameObject.Find("MainText").gameObject as GameObject;
        objTextBox = GameObject.Find("TextBoxImage").gameObject as GameObject;
        objBackImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBackText = GameObject.Find("BackText").gameObject as GameObject; objBackText.gameObject.SetActive(false);
        objCanvas = GameObject.Find("BackImage").gameObject as GameObject;
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        for (int i = 0; i < 4; i++) { objBox[i] = GameObject.Find("select" + (i + 1).ToString()).gameObject as GameObject; objBox[i].gameObject.SetActive(false); }
        for (int i = 0; i < 2; i++) { objDice[i] = GameObject.Find("Dice" + (i + 1).ToString()).gameObject as GameObject; objDice[i].gameObject.SetActive(false); }
        StartCoroutine(LoadPlayerChara(PlayerPrefs.GetString("[system]CharacterIllstPath", "")));
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
        string[] battleText = new string[13];
        string[] separateText = new string[2];
        string[] separate3Text = new string[3];
        DateTime dt;
        StartCoroutine(Status(PlayerPrefs.GetInt("[system]耐久力", 0), 0));
        StartCoroutine(StatusSAN(PlayerPrefs.GetInt("[system]正気度ポイント", 0), 0));
        for (int i = 1; i < 100; i++)
        {
            for (int j = 0; j < 4; j++) { buttonText[j] = null; }
            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
            ScreenSizeChanger.SetActive(true);
            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
            PlayerPrefs.Save();
            skipFlag2 = false;
            sentenceEnd = false;
            if (scenarioText[i].Replace("\r", "").Replace("\n", "") == "[END]" || scenarioText[i].Replace("\r","").Replace("\n","") == "" || scenarioText[i] == null) { break; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Wait:"){ for (int k = 0; k <double.Parse(scenarioText[i].Substring(5).Replace("\r", "").Replace("\n", "")) * 60; k++) { yield return null; }sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Text:") { separate3Text = scenarioText[i].Substring(5).Replace("\r","").Replace("\n","").Split(',');string[] tmp =TextReplace(separate3Text[0], separate3Text[1]); TextDraw(tmp[0], tmp[1]);if (PlayerPrefs.GetInt("[system]" + sectionName + i.ToString(), 0)==1) { skipFlag2 = true; } if (separate3Text[2] == "true") { StartCoroutine(PushWait()); } else { sentenceEnd = true; } PlayerPrefs.SetInt("[system]" + sectionName + i.ToString(),1);if (skipFlag2 == false) {   PlayerPrefs.SetString("[system]バックログ" + logNum.ToString(),"<" + tmp[0] + ">\r\n" + tmp[2]); logNum++; if (logNum >= 1000) { logNum = 0; } PlayerPrefs.SetInt("[system]最新ログ番号", logNum); skipFlag2 = false; }}
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "BackText:") {separateText=scenarioText[i].Substring(9).Replace("\r","").Replace("\n","").Split(',');string[] tmp = TextReplace("",separateText[0]); BackTextDraw(tmp[1]); if (PlayerPrefs.GetInt("[system]" + sectionName + i.ToString(), 0) == 1) { skipFlag2 = true; } if (separateText[1] == "true") { StartCoroutine(PushWait()); } else { sentenceEnd = true; }  PlayerPrefs.SetInt("[system]" + sectionName + i.ToString(), 1);if (skipFlag2 == false) {  PlayerPrefs.SetString("[system]バックログ" + logNum.ToString(), tmp[2]); logNum++; if (logNum >= 1000) { logNum = 0; } PlayerPrefs.SetInt("[system]最新ログ番号", logNum); skipFlag2 = false; }}
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Back:") { BackDraw(int.Parse(scenarioText[i].Substring(5))); sentenceEnd = true; }
            if (scenarioText[i].Length > 4 && scenarioText[i].Substring(0, 4) == "BGM:") { separateText = scenarioText[i].Substring(4).Split(','); BGMIn(int.Parse(separateText[1].Replace("\r", "").Replace("\n", ""))); BGMPlay(int.Parse(separateText[0])); sentenceEnd = true; }
            if (scenarioText[i].Length > 8 && scenarioText[i].Substring(0, 8) == "BGMStop:") { BGMOut(int.Parse(scenarioText[i].Substring(8))); sentenceEnd = true; }
            if (scenarioText[i].Length > 3 && scenarioText[i].Substring(0, 3) == "SE:") { SEPlay(int.Parse(scenarioText[i].Substring(3).Replace("\r", "").Replace("\n", ""))); sentenceEnd = true; }
            if (scenarioText[i].Length > 6 && scenarioText[i].Substring(0, 6) == "Chara:") { separate3Text = scenarioText[i].Substring(6).Split(','); CharacterDraw(int.Parse(separate3Text[0]), int.Parse(separate3Text[1])); StartCoroutine(CharacterMove(int.Parse(separate3Text[1]), separate3Text[2].Replace("\r", "").Replace("\n", ""))); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Item:") { ItemDraw(int.Parse(scenarioText[i].Substring(5).Replace("\r", "").Replace("\n", ""))); sentenceEnd = false; StartCoroutine(PushWait()); while (sentenceEnd == false) { yield return null; } if (backNum>=0) { objBackImage.GetComponent<Image>().sprite = scenarioGraphic[backNum]; }sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Shake") { StartCoroutine(ShakeScreen()); }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Jump:") { StartCoroutine(CharacterJump(int.Parse(scenarioText[i].Substring(5, 1).Replace("\r", "").Replace("\n", "")))); }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Select:") { buttonText = scenarioText[i].Substring(7).Split(','); StartCoroutine(Select(buttonText[0], buttonText[1], buttonText[2], buttonText[3].Replace("\r", "").Replace("\n", ""),false)); while (sentenceEnd == false) { yield return null; }; SystemSEPlay(systemAudio[3]); i += selectNum; continue; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "NextFile:") { LoadFile(scenarioText[i].Substring(9).Replace("\r", "").Replace("\n", "")); i = 0; sentenceEnd = true; continue; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Hantei:") { separateText = scenarioText[i].Substring(7).Split(','); yield return StartCoroutine(Hantei(separateText[0], int.Parse(separateText[1].Replace("\r", "").Replace("\n", ""))));i += hanteikekka; for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }objRollText.gameObject.SetActive(false);PlayerPrefs.Save();skipFlag2 = false; continue; }
            if (scenarioText[i].Length > 7 && scenarioText[i].Substring(0, 7) == "Battle:") { battleText = scenarioText[i].Substring(7).Split(',');int attacktype = 0; try { attacktype=int.Parse(battleText[13].Replace("\r", "").Replace("\n", "")); } catch { } battleFlag = -1; StartCoroutine(Battle(int.Parse(battleText[0]), int.Parse(battleText[1]), int.Parse(battleText[2]), int.Parse(battleText[3]), int.Parse(battleText[4]), int.Parse(battleText[5]), int.Parse(battleText[6]),bool.Parse(battleText[7]), battleText[8], battleText[9], int.Parse(battleText[10]), int.Parse(battleText[11]), bool.Parse(battleText[12].Replace("\r", "").Replace("\n", "")),attacktype)); while (battleFlag == -1) { yield return null; }; i += battleFlag;continue; }
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "FlagBranch:") { separateText = scenarioText[i].Substring(11).Replace("\r", "").Replace("\n", "").Split(','); if (PlayerPrefs.GetInt(separateText[0], 0) < int.Parse(separateText[1]) ) { i++; }continue; }
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "FlagChange:") { sentenceEnd = true; separate3Text = scenarioText[i].Substring(11).Replace("\r", "").Replace("\n","").Split(','); if (flagname.Contains(separate3Text[0])) { } else { flagname.Add(separate3Text[0]); flagvalue.Add(PlayerPrefs.GetInt(separate3Text[0]).ToString()); } if (separate3Text[1] == "") { FlagChange(separate3Text[0], 0, separate3Text[2].Replace("\r", "").Replace("\n", ""), true); } else { FlagChange(separate3Text[0], int.Parse(separate3Text[1]), "", false); }  }
            if (scenarioText[i].Length > 8 && scenarioText[i].Substring(0, 8) == "GetTime:"){ dt = DateTime.Now; PlayerPrefs.SetInt("[system]Month", dt.Month); PlayerPrefs.SetInt("[system]Day", dt.Day); PlayerPrefs.SetInt("[system]Hour",dt.Hour); PlayerPrefs.SetInt("[system]Minute", dt.Minute); sentenceEnd = true; }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "FlagCopy:"){ separateText = scenarioText[i].Substring(9).Split(',');if (flagname.Contains(separateText[1].Replace("\r", "").Replace("\n", ""))) { } else { flagname.Add(separateText[1].Replace("\r", "").Replace("\n", "")); flagvalue.Add(PlayerPrefs.GetInt(separateText[1].Replace("\r", "").Replace("\n", "")).ToString()); } PlayerPrefs.SetInt(separateText[1].Replace("\r", "").Replace("\n", ""), PlayerPrefs.GetInt(separateText[0], 0));  sentenceEnd = true; }//フラグを別名で保存する
            if (scenarioText[i].Length > 11 && scenarioText[i].Substring(0, 11) == "Difference:"){separate3Text = scenarioText[i].Substring(11).Split(',');i+=Difference(separate3Text);continue; }
            if (scenarioText[i].Length > 13 && scenarioText[i].Substring(0, 13) == "StatusChange:"){separateText = scenarioText[i].Substring(13).Split(',');if (flagname.Contains(SkillList(separateText[0]))) { } else { flagname.Add(SkillList(separateText[0]));flagvalue.Add(PlayerPrefs.GetInt(SkillList(separateText[0])).ToString()); }  StartCoroutine(StatusChange(separateText,true)); while (sentenceEnd == false) { yield return null; }; sentenceEnd = false; StartCoroutine(PushWait());  }//「StatusChange:正気度,-2D6」のように①変動ステータス、②変動値（○D○または固定値どちらでもプログラム側で適切な解釈をしてくれる）
            if (scenarioText[i].Length > 6 && scenarioText[i].Substring(0, 6) == "Input:") { if (flagname.Contains(scenarioText[i].Substring(6).Replace("\r", "").Replace("\n", ""))) { } else { flagname.Add(scenarioText[i].Substring(6).Replace("\r", "").Replace("\n", ""));flagvalue.Add("[system]String" + PlayerPrefs.GetString(scenarioText[i].Substring(6).Replace("\r", "").Replace("\n", ""), "")); } StartCoroutine(InputText(scenarioText[i].Substring(6).Replace("\r", "").Replace("\n", ""))); }
            if (scenarioText[i].Length > 6 && scenarioText[i].Substring(0, 6) == "Equal:"){  separateText = scenarioText[i].Substring(6).Split(','); i += Equal(PlayerPrefs.GetString(separateText[0], ""), separateText[1].Replace("\r", "").Replace("\n", ""));  continue; }
            if (scenarioText[i].Length > 12 && scenarioText[i].Substring(0, 12) == "PlaceChange:") { if (flagname.Contains("[system]latitude") && flagname.Contains("[system]longitude")) { } else { flagname.Add("[system]latitude"); flagname.Add("[system]longitude"); } flagvalue.Add(PlayerPrefs.GetFloat("[system]latitude",0).ToString()); flagvalue.Add(PlayerPrefs.GetFloat("[system]longitude",0).ToString()); separateText = scenarioText[i].Substring(12).Split(','); PlayerPrefs.SetFloat("[system]latitude", float.Parse(separateText[0].Replace("\r", "").Replace("\n", ""))); PlayerPrefs.SetFloat("[system]longitude", float.Parse(separateText[1].Replace("\r", "").Replace("\n", "")));  sentenceEnd = true; }
            if (scenarioText[i].Length > 5 && scenarioText[i].Substring(0, 5) == "Lost:") { StartCoroutine(CharaLost()); }
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "SANCheck:") { separateText = scenarioText[i].Substring(9).Replace("\r","").Replace("\n","").Split(',');SANCheckFlag = -1; StartCoroutine(SANCheck(separateText)); while (SANCheckFlag == -1) { yield return null; }i += SANCheckFlag;continue; }
            if (scenarioText[i].Length > 10 && scenarioText[i].Substring(0, 10) == "FlagReset:") { yield return StartCoroutine(Grow()); FlagReset();}
            if (scenarioText[i].Length > 9 && scenarioText[i].Substring(0, 9) == "BlackOut:") { buttonText = scenarioText[i].Substring(9).Split(','); StartCoroutine(BlackOut(int.Parse(buttonText[0]),int.Parse(buttonText[1]),int.Parse(buttonText[2]),int.Parse(buttonText[3].Replace("\r", "").Replace("\n", ""))));}
            if (scenarioText[i].Length > 6 && scenarioText[i].Substring(0, 6) == "Title:") { if (LostCheck()) { GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene"); } }
            if (scenarioText[i].Length > 4 && scenarioText[i].Substring(0, 4) == "Map:") { if (LostCheck()) { if (scenarioText[i].Substring(4, 4).Replace("\r", "").Replace("\n", "") == "Once") { PlayerPrefs.SetInt(objBGM.GetComponent<BGMManager>().chapterName.Substring(0, objBGM.GetComponent<BGMManager>().chapterName.Length - 4) + "Flag", 1); } GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MapScene"); } }
            while (sentenceEnd == false) { yield return null; }
            StoreLink(scenarioText[i]);
        }
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");

    }


    private bool LostCheck()
    {
        if (PlayerPrefs.GetInt("[system]正気度ポイント", 0)<=0 || PlayerPrefs.GetInt("[system]耐久力", 0) <= 2 || PlayerPrefs.GetInt("[system]Status0", 0) <= 0 || PlayerPrefs.GetInt("[system]Status1", 0) <= 0 || PlayerPrefs.GetInt("[system]Status2", 0) <= 0 || PlayerPrefs.GetInt("[system]Status3", 0) <= 0 || PlayerPrefs.GetInt("[system]Status6", 0) <= 0 || PlayerPrefs.GetInt("[system]Status7", 0) <= 0)
        {
            StartCoroutine(CharaLost());
            return false;
        }
        return true;
    }

    private IEnumerator SANCheck(string[] dice)
    {
        int kekka = 0;
        string[] str=new string[2];
        string[] tmp;
        int tmpint;
        //現在正気度を保存
        int nowSAN = PlayerPrefs.GetInt("[system]正気度ポイント",0);
        sentenceEnd = false;
        //正気度ポイントで判定して成功か失敗か
        yield return StartCoroutine(Hantei("正気度ポイント",0)); 
        kekka = hanteikekka;
        sentenceEnd = false;
        //成功時の減少
        if (kekka < 2)
        {
            str[0] = "正気度ポイント";
            tmp=dice[0].Split('+');
            if (!dice[0].Contains("D"))
            {
                if (tmp.Length > 1)
                {
                    if (int.TryParse(tmp[1], out tmpint)) { if (tmpint == 0) { SANCheckFlag = 0; sentenceEnd = true; yield break; } }
                }
            }
            //if (tmp.Length > 1) { if (int.TryParse(tmp[1], out tmpint)) { if (tmpint >= 0) { dice[0] = dice[0].Replace("+", "+-"); } else { dice[0] = dice[0].Replace("+-", "+"); } } }
            if (dice[0].Contains("D")) { str[1] = "-" + dice[0]; } else { str[1] = "-" + dice[0]; }
            StartCoroutine(StatusChange(str,true));
        }
        //失敗時の減少
        if(kekka==2)
        {
            str[0] = "正気度ポイント";
            tmp = dice[1].Split('+');
            if (!dice[1].Contains("D"))
            {
                if (tmp.Length > 1)
                {
                    if (int.TryParse(tmp[1], out tmpint)) { if (tmpint == 0) { SANCheckFlag = 0; sentenceEnd = true; yield break; } }
                }
            }
            //if (tmp.Length > 1) { if (int.TryParse(tmp[1], out tmpint)) { if (tmpint >= 0) { dice[1] = dice[1].Replace("+", "+-"); } else { dice[1] = dice[1].Replace("+-", "+"); } } }
            if (dice[1].Contains("D")) { str[1] = "-" + dice[1]; } else { str[1] = "-" + dice[1]; }
            StartCoroutine(StatusChange(str,true));
        }
        while (sentenceEnd == false) { yield return null; }
        sentenceEnd = false; StartCoroutine(PushWait()); while (sentenceEnd == false) { yield return null; }
        sentenceEnd = false;
        //現在正気度をさっき保存したのと比較
        if (PlayerPrefs.GetInt("[system]正気度ポイント", 0) > nowSAN - 5) { SANCheckFlag = 0; }
        //アイデア判定
        if (SANCheckFlag == -1)
        {
            yield return StartCoroutine(Hantei("アイデア", 0));
            kekka = hanteikekka; 
            sentenceEnd = false; StartCoroutine(PushWait()); while (sentenceEnd == false) { yield return null; }
            sentenceEnd = false;
            //失敗なら何事もなく
            if (kekka == 2) { SANCheckFlag = 0; }
            //成功なら発狂へ
            if (kekka < 2) {
                StartCoroutine(Mad());
                if (PlayerPrefs.GetInt("[system]初発狂", 0) == 0)
                {
                    str[0] = "クトゥルフ神話";
                    str[1] = "5";
                    StartCoroutine(StatusChange(str,true)); PlayerPrefs.SetInt("[system]初発狂", 1);
                    while (sentenceEnd == false) { yield return null; }
                    sentenceEnd = false; StartCoroutine(PushWait()); while (sentenceEnd == false) { yield return null; }
                }
                SANCheckFlag = 1; }
        }
        sentenceEnd = true;
    }
    private IEnumerator Mad()
    {
        objMad.SetActive(true);
        SystemSEPlay(mad);
        for (int i=0; i < 600; i++)
        {
            yield return null;
#if UNITY_IOS
i++;
#endif
        }
        objMad.SetActive(false);
    }


    private int Equal(string a,string b)
    {
        if (a == b) { return 0; }
        return 1;
    }


    private void FlagChange(string flagname,int value,string changevalue,bool changevalueflag)
    {
        if (changevalueflag==false) { PlayerPrefs.SetInt(flagname, value); }
        if (changevalueflag==true) {
            if (changevalue.Contains("D"))
            {
                string[] tmp=new string[2];
                tmp[0] = flagname;
                tmp[1] = changevalue;
                sentenceEnd = false;
                StartCoroutine(StatusChange(tmp,false));
            }
            else
            {
                PlayerPrefs.SetInt(flagname, PlayerPrefs.GetInt(flagname, 0) + int.Parse(changevalue));
            }
        }
    }

    private IEnumerator BlackOut(int r,int g,int b,int time)
    {
        Image bo = GameObject.Find("BlackOut").GetComponent<Image>();
        bo.enabled = true;
        for (int i = 0; i < time * 60; i++)
        {
            bo.color = new Color((float)r/255,(float)g/255,(float)b/255,(float)i/(time*60));
#if UNITY_IOS
i++;
#endif
            yield return null;
        }
        bo.enabled = false;
        sentenceEnd = true;
    }

    //仕様上、既読フラグやバックログも一緒に消えるので注意。（キャラシは残る）
    private void FlagReset()
    {
        int[] status=new int[STATUSNUM];
        int[] skills=new int[SKILLNUM];
        string[] tmpstrs=new string[3];
        int[] tmpints = new int[8];
        int VMode = 0;
        string nowPlay;
        objBGM.GetComponent<BGMManager>().makuma = 1;
        //残す情報を一時避難
        for(int i=0;i<STATUSNUM;i++)
        {
            status[i] = PlayerPrefs.GetInt("[system]Status" + i.ToString(), 0);
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            skills[i] = PlayerPrefs.GetInt("[system]Skill" + i.ToString(), 0);
        }
        nowPlay= PlayerPrefs.GetString("[system]進行中シナリオ", "");
        tmpstrs[0] = PlayerPrefs.GetString("[system]CharacterIllstPath", "");
        tmpstrs[1] = PlayerPrefs.GetString("[system]PlayerCharacterName", "");
        tmpstrs[2] = PlayerPrefs.GetString("[system]PlayerCharacterNickName", "");
        tmpints[0] = PlayerPrefs.GetInt("[system]マジック・ポイント");
        tmpints[1] = PlayerPrefs.GetInt("[system]耐久力");
        tmpints[2] = PlayerPrefs.GetInt("[system]正気度ポイント");
        tmpints[3] = PlayerPrefs.GetInt("[system]アイデア");
        tmpints[4] = PlayerPrefs.GetInt("[system]知識");
        tmpints[5] = PlayerPrefs.GetInt("[system]幸運");
        tmpints[6] = PlayerPrefs.GetInt("[system]初発狂");
        tmpints[7] = PlayerPrefs.GetInt("[system]未決定");
        VMode = PlayerPrefs.GetInt("[system]VMode");

        //セーブデータを全部消す
        PlayerPrefs.DeleteAll();
        //残す情報を再書き込み
        for (int i = 0; i < STATUSNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Status" + i.ToString(), status[i]);
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Skill" + i.ToString(), skills[i]);
        }
        PlayerPrefs.SetString("[system]CharacterIllstPath", tmpstrs[0]);
        PlayerPrefs.SetString("[system]PlayerCharacterName", tmpstrs[1]);
        PlayerPrefs.SetString("[system]PlayerCharacterNickName", tmpstrs[2]);
        PlayerPrefs.SetInt("[system]マジック・ポイント", tmpints[0]);
        PlayerPrefs.SetInt("[system]耐久力", tmpints[1]);
        PlayerPrefs.SetInt("[system]正気度ポイント", tmpints[2]);
        PlayerPrefs.SetInt("[system]アイデア", tmpints[3]);
        PlayerPrefs.SetInt("[system]知識", tmpints[4]);
        PlayerPrefs.SetInt("[system]幸運", tmpints[5]);
        PlayerPrefs.SetInt("[system]初発狂", tmpints[6]);
        PlayerPrefs.SetInt("[system]未決定",tmpints[7]);
        PlayerPrefs.SetInt("[system]VMode",VMode);
        logNum = 0;
        PlayerPrefs.SetString("[system]進行中シナリオ",nowPlay);
        if (skipFlag == true) { PlayerPrefs.SetInt("[system]skipFlag", 1); }
        sentenceEnd=false;
        StartCoroutine(Review());
    }

    private IEnumerator Grow()
    {
        TextDraw(" ","＜技能成長＞");
        sentenceEnd = false;
        StartCoroutine(PushWait());
        while (!sentenceEnd) { yield return null; }
        for (int i = 0; i < 53; i++)
        {
            if (PlayerPrefs.GetInt("s[system]Skill" + i.ToString(), 0) > 0)
            {
                string skilltmp=ReverseSkillList("[system]Skill" + i.ToString());
                yield return StartCoroutine(Hantei(skilltmp,0,true));
                if (hanteikekka > 1) {
                    string[] tmp = new string[2];
                    tmp[0] = skilltmp;
                    tmp[1] = "1D10";
                    yield return StartCoroutine(StatusChange(tmp,true));
                    if (SkillCheck(tmp[0])>=90)
                    {
                        tmp[0] = "正気度ポイント";
                        tmp[1] = "2D6";
                        yield return StartCoroutine(StatusChange(tmp, true));
                        sentenceEnd = false;
                        StartCoroutine(PushWait());
                        while (sentenceEnd) { yield return null; }
                    }
                }
                PlayerPrefs.SetInt("s[system]Skill" + i.ToString(), 0);
            }
        }
        sentenceEnd = false;
        StartCoroutine(PushWait());
        while (sentenceEnd) { yield return null; }
        sentenceEnd = false;
    }

    private IEnumerator Review()
    {
        ask = 0;
#if UNITY_IOS
        {
            if (!UnityEngine.iOS.Device.RequestStoreReview())
            {
                objReview.SetActive(true);
                while (ask==0) { yield return null; }
                if (ask == 1)
                {
                    string url = "itms-apps://itunes.apple.com/jp/app/id1445032217?mt=8&action=write-review";
                    Application.OpenURL(url);
                }
            }
        }
#endif
#if UNITY_ANDROID
        objReview.SetActive(true);
            while (ask == 0) { yield return null; }
            if (ask == 1)
            {
                string url = "market://details?id=com.brainmixer.CoCAR";
                Application.OpenURL(url);
            }
#endif
        objReview.SetActive(false);
        sentenceEnd = true;
        yield return null;


    }

    public void ReviewButton(int asknum)
    {
        ask = asknum;
    }

    public void StoreLink(string str)
    {
        str = str.Replace("：",":");
        if (str.Contains("https://booth.pm/"))
        {
            System.Text.RegularExpressions.Match matche = System.Text.RegularExpressions.Regex.Match(str, "https://booth.pm/" + "[ -~]*");
            Application.OpenURL(matche.Value);
        }
    }





    private IEnumerator CharaLost()
    {
        //ロスト状態キャラシを見せる
        GameObject.Find("Button (2)").gameObject.GetComponent<CSManager>().CSButton();
        GameObject.Find("Lost").gameObject.GetComponent<Text>().text= "-LOST-";
        while (Input.GetMouseButtonDown(0) == false){ yield return null; }
        //キャラクターデータを全て消し、タイトル画面に送り返す。
        for (int i = 0; i < STATUSNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Status" + i.ToString(), 0);
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Skill" + i.ToString(), 0);
        }
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
    }

    public void SkipButton()
    {
        SkipButtonIn();
    }

    private void SkipButtonIn()
    {
        if (skipFlag == false)
        {
            skipFlag = true;
            PlayerPrefs.SetInt("[system]skipFlag", 1);
            objSkip.GetComponent<Text>().text = "<color=red>既読Skip\n<ON></color>";
            objSkipImage.GetComponent<Image>().sprite = skip;
        }
        else
        {
            skipFlag = false;
            PlayerPrefs.SetInt("[system]skipFlag", 0);
            objSkip.GetComponent<Text>().text = "既読Skip\n<OFF>";
            objSkipImage.GetComponent<Image>().sprite = play;
        }
    }

    private IEnumerator InputText(string str)
    {
        selectNum = -1;
        objInput.gameObject.SetActive(true);
        InputField inputField = objInput.GetComponent<InputField>();
        inputField.text = "";
        inputField.ActivateInputField();
        objBox[3].gameObject.SetActive(true);
        objBox[3].GetComponentInChildren<Text>().text = "決定";
        SelectBoxMake(0, 0, 0, 2, false);
        while (selectNum==-1) { yield return null; }
        PlayerPrefs.SetString(str, inputField.text);
        objInput.gameObject.SetActive(false);
        objBox[3].gameObject.SetActive(false);
        sentenceEnd = true;
    }

    private IEnumerator StatusChange(string[] separateText,bool textsee)
    {
        int changeValue = 0;
        int changeValue2 = 0;
        int changeValue3 = 0;
        string cvtext = "";
        int x1,x2, y1, y2;
int beforeValue=0;
        string targetStr;
        Utility u1 = GetComponent<Utility>();
        string[] separate3Text;
        targetStr=SkillList(separateText[0]);
if (targetStr == "[system]耐久力") {beforeValue=PlayerPrefs.GetInt("[system]耐久力", 0);}
        if (targetStr == "[system]正気度ポイント") { beforeValue = PlayerPrefs.GetInt("[system]正気度ポイント", 0); }
        if (int.TryParse(separateText[1].Replace("\r", "").Replace("\n", "").Replace("+",""), out x1))
        {
            x2 = x1;
            if (PlayerPrefs.GetInt(targetStr, 0) < -x1) { x1 = -1 * PlayerPrefs.GetInt(targetStr, 0); }
            if (PlayerPrefs.GetInt(targetStr, 0) + x1 >=100) { x1 = 99 - PlayerPrefs.GetInt(targetStr, 0); }
            if (targetStr == "[system]耐久力" && PlayerPrefs.GetInt(targetStr, 0) + x1 >= PlayerPrefs.GetInt("[system]Status9", 0)) { x1 = PlayerPrefs.GetInt("[system]Status9", 0)- PlayerPrefs.GetInt(targetStr, 0); }
            if (targetStr == "[system]マジック・ポイント" && PlayerPrefs.GetInt(targetStr, 0) + x1 >= PlayerPrefs.GetInt("[system]Status10", 0)) { x1 = PlayerPrefs.GetInt("[system]Status10", 0) - PlayerPrefs.GetInt(targetStr, 0); }
            if (targetStr == "[system]正気度ポイント" && PlayerPrefs.GetInt(targetStr, 0) + x1 >= 99 - PlayerPrefs.GetInt("[system]Skill53", 0)) {x1= 99 - PlayerPrefs.GetInt("[system]Skill53", 0) - PlayerPrefs.GetInt(targetStr, 0); }
            PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + x1);
            if (x2 > 0)
            {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                if (textsee) { TextDraw("", separateText[0] + "の能力が" + x2.ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
            }
            else
            {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                    if (textsee) { TextDraw("", separateText[0] + "の能力が" + (-1 * x2).ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
            }
            for (int v = 0; v < 60; v++)
            {
#if UNITY_IOS
v++;
#endif
                yield return null; }
            }
        else
        {
            separate3Text = separateText[1].Split(new char[] { 'D','+' });
            if (int.Parse(separate3Text[0]) >= 0) { y2 = 1; } else { y2 = -1; }
            if (int.Parse(separate3Text[0]) * y2 == 2 && int.Parse(separate3Text[1])!=100)
            {
                changeValue = u1.DiceRoll(1, int.Parse(separate3Text[1]));
                changeValue2 = u1.DiceRoll(1, int.Parse(separate3Text[1]));
                if (separate3Text.Length>2){ int.TryParse(separate3Text[2],out changeValue3); }
                StartCoroutine(DiceEffect(0, int.Parse(separate3Text[1]), changeValue));
                StartCoroutine(DiceEffect(1, int.Parse(separate3Text[1]), changeValue2));
                for (int v = 0; v < 60; v++) {
#if UNITY_IOS
v++;
#endif
                    yield return null; }
                if (PlayerPrefs.GetInt(targetStr, 0) < -1 * (changeValue + changeValue2 + changeValue3) * y2) { PlayerPrefs.SetInt(targetStr, 0); }
                else if (targetStr == "[system]耐久力" && PlayerPrefs.GetInt(targetStr, 0) + (changeValue + changeValue2 + changeValue3) * y2 >= PlayerPrefs.GetInt("[system]Status9", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status9", 0)); }
                else if (targetStr == "[system]マジック・ポイント" && PlayerPrefs.GetInt(targetStr, 0) + (changeValue + changeValue2 + changeValue3) * y2 >= PlayerPrefs.GetInt("[system]Status10", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status10", 0)); }
                else if (targetStr == "[system]正気度ポイント" && PlayerPrefs.GetInt(targetStr, 0) + (changeValue + changeValue2 + changeValue3) * y2 >= 99 - PlayerPrefs.GetInt("[system]Skill53", 0)) { PlayerPrefs.SetInt(targetStr, 99 - PlayerPrefs.GetInt("[system]Skill53", 0)); }
                else if (PlayerPrefs.GetInt(targetStr, 0) + (changeValue + changeValue2 + changeValue3) * y2 >= 100) { PlayerPrefs.SetInt(targetStr, 99); }
                else { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + (changeValue + changeValue2 + changeValue3) * y2); }
                if (changeValue3 != 0) { cvtext = "+" + changeValue3.ToString(); }
                if (y2 > 0)
                {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                    if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                            if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "+" + changeValue2.ToString() + cvtext + "=" + (changeValue + changeValue2 + changeValue3).ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                }
                else
                {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                    if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                                if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "+" + changeValue2.ToString() + cvtext + "=" + (changeValue + changeValue2 + changeValue3).ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                }
                for (int v = 0; v < 60; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
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
                    for (int v = 0; v < 60; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                    }
                    if (PlayerPrefs.GetInt(targetStr, 0) < -1 * changeValue * y2) { PlayerPrefs.SetInt(targetStr, 0); }
                    else if (targetStr == "[system]耐久力" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status9", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status9", 0)); }
                    else if (targetStr == "[system]マジック・ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status10", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status10", 0)); }
                    else if (targetStr == "[system]正気度ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 99 - PlayerPrefs.GetInt("[system]Skill53", 0)) { PlayerPrefs.SetInt(targetStr, 99 - PlayerPrefs.GetInt("[system]Skill53", 0)); }
                    else if (PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 100) { PlayerPrefs.SetInt(targetStr, 99); }
                    else { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2); }
                    if (y2 > 0)
                    {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                        if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                                        if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                    }
                    else
                    {
                if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue,beforeValue-PlayerPrefs.GetInt("[system]耐久力", 0))); }
                        if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                                            if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                    }
                    for (int v = 0; v < 60; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                    }
                }
                if (separate3Text.Length > 2)
                {
                    if (int.TryParse(separate3Text[2], out changeValue))
                    {
                        if (PlayerPrefs.GetInt(targetStr, 0) < -1 * changeValue * y2) { PlayerPrefs.SetInt(targetStr, 0); }
                        else if (targetStr == "[system]耐久力" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status9", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status9", 0)); }
                        else if (targetStr == "[system]マジック・ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= PlayerPrefs.GetInt("[system]Status10", 0)) { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt("[system]Status10", 0)); }
                        else if (targetStr == "[system]正気度ポイント" && PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 99 - PlayerPrefs.GetInt("[system]Skill53", 0)) { PlayerPrefs.SetInt(targetStr, 99 - PlayerPrefs.GetInt("[system]Skill53", 0)); }
                        else if (PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2 >= 100) { PlayerPrefs.SetInt(targetStr, 99); }
                        else { PlayerPrefs.SetInt(targetStr, PlayerPrefs.GetInt(targetStr, 0) + changeValue * y2); }
                        if (y2 > 0)
                        {
                            if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]耐久力", 0))); }
                            if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                                                    if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点上昇した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                        }
                        else
                        {
                            if (targetStr == "[system]耐久力") { StartCoroutine(Status(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]耐久力", 0))); }
                            if (targetStr == "[system]正気度ポイント") { StartCoroutine(StatusSAN(beforeValue, beforeValue - PlayerPrefs.GetInt("[system]正気度ポイント", 0))); }
                                                        if (textsee) { TextDraw("", separateText[0] + "の能力が" + changeValue.ToString() + "点減少した。" + "\n（" + separateText[0] + "：" + PlayerPrefs.GetInt(targetStr, 0).ToString() + "）"); }
                        }
                        for (int v = 0; v < 60; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                        }
                    }
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
                if (x1 >= x2 + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 0; }
            }
            else
            {
                if (x1 >= PlayerPrefs.GetInt(separate3Text[1], 0) + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 0; }
            }
        }
        else
        {
            if (int.TryParse(separate3Text[1], out x2))
            {
                if (PlayerPrefs.GetInt(separate3Text[0], 0) >= x2 + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) { return 0; }
            }
            else
            {
                if (PlayerPrefs.GetInt(separate3Text[0], 0) >= PlayerPrefs.GetInt(separate3Text[1], 0) + int.Parse(separate3Text[2].Replace("\r", "").Replace("\n", ""))) {return 0; }
            }
        }
        return 1;
    }

    private void BattleBegin(int enemyGraph,int enemyNum,int HP,int playerHP,ref int[] enemyHP,ref int enemyMaxHP)
    {
        TextDraw(" ", "<size=72>戦闘開始</size>");
        for (int i = 0; i < 5; i++)
        {
            objCharacter[i].gameObject.SetActive(false);
        }
            for (int i = 0; i < enemyNum; i++)
        {
            objCharacter[i].gameObject.SetActive(true);
            objCharacter[i].GetComponent<Image>().sprite = scenarioGraphic[enemyGraph];
            enemyHP[i] = HP;
            ObjSizeChangeToGraph(i, scenarioGraphic[enemyGraph]);
        }
        enemyMaxHP = HP;
        if (enemyNum == 1) { objCharacter[0].GetComponent<RectTransform>().localPosition =new Vector3(0, CHARACTER_Y, 1); }
        if (enemyNum == 2) { objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(1 * 150 - 300, CHARACTER_Y, 1); objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(3 * 150 - 300, CHARACTER_Y, 1); }
        if (enemyNum == 3) { objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(2 * 150 - 300, CHARACTER_Y, 1); objCharacter[2].GetComponent<RectTransform>().localPosition = new Vector3(4 * 150 - 300, CHARACTER_Y, 1); }
    }

    //戦闘処理
    private IEnumerator Battle(int enemyGraph, int enemyNum, int HP, int DEX, int AttackPercent, int ATDiceNum, int ATDice,bool humanFlag,string tokusyu,string tokusyuSkill,int tokusyuSkillBonus,int maxTurn,bool maxTurnWin,int attacktype)
    {
        int[] enemyHP = new int[enemyNum];
        int enemyMaxHP=0;
        int kill = 0;
        int sleep = 0;
        int playerHP = PlayerPrefs.GetInt("[system]耐久力", 0);
        int playerDEX = PlayerPrefs.GetInt("[system]Status2", 3);
        int damage = 0;
        int avoid = 2;
        int detailAct = 0;
        int tmpDice;
        bool cutFlag = false;

        BattleBegin(enemyGraph,enemyNum,HP,playerHP,ref enemyHP,ref enemyMaxHP);

        Utility u1 = GetComponent<Utility>();
        for (int x=0;x<maxTurn || maxTurn==-1;x++)
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
                if (PlayerPrefs.GetInt("火器", 0)>0) { StartCoroutine(Select("火器", "格闘", "武器術", "戻る", true)); }
                else{ StartCoroutine(Select("投石", "格闘", "武器術", "戻る", true)); }
                while (selectNum == -1) { yield return null; }; SystemSEPlay(systemAudio[3]);
                if (selectNum == 3) { goto firstSelect; }
                if (selectNum == 2) {cutFlag=true; }
                detailAct =selectNum;selectNum = 0;
                TextDraw("", "");
            }
            for (int v = 0; v < 50; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
            }
            if (DEX <= playerDEX && selectNum==0 && playerHP>2)
            {
                yield return StartCoroutine(PlayerBattle(detailAct, enemyHP, humanFlag, enemyNum,enemyMaxHP));
                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
            }//攻撃１（相手より早い場合）

            if (DEX <= playerDEX && selectNum==2 && playerHP>2)
            {
                yield return StartCoroutine(Catcher(enemyNum, humanFlag, enemyHP));
                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
            }//拘束１（相手より早い場合）
            for (int i = 0; i < enemyNum && playerHP>2; i++)
            {
                if ((enemyHP[i] > 0 && humanFlag==false) || enemyHP[i]>2)
                {
                    tmpDice = u1.DiceRoll(1, 100);
                    if (tmpDice < AttackPercent)
                    {
                        if (humanFlag==true && cutFlag==true && attacktype<2)
                        {
                            sentenceEnd = false;
                            StartCoroutine(Hantei("武器術", 0));
                            objRollText.GetComponent<Text>().text = "受け流し\n（武器術）" + objRollText.GetComponent<Text>().text.Substring(3);
                            while (sentenceEnd == false) { yield return null; }
                            avoid = hanteikekka;
                            if (avoid >= 1) { cutFlag = false; }
                            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                            if (avoid <=1 )
                            {
                                sentenceEnd = false;
                                StartCoroutine(Cut(i,enemyNum));
                                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                                }
                                continue;
                            }
                        }
                        if (selectNum==1)
                        {
                            yield return StartCoroutine(Hantei("回避", 0));
                            avoid = hanteikekka;
                            for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                            objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                            if (avoid >= 2) { selectNum = -1; }         
                            if (avoid <= 1) { sentenceEnd = false;for (int j = 0; j < 10; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                                }
                                StartCoroutine(Avoid(i,enemyNum)); }
                        }
                        if (selectNum !=1 )
                        {
                            bool special = false;
                            if (tmpDice <= AttackPercent / 5){ special = true; }
                            damage = u1.DiceRoll(ATDiceNum, ATDice);
                            sentenceEnd = false;
                            StartCoroutine(Status(playerHP, damage));
                            yield return StartCoroutine(EnemyHit(i, enemyNum, damage, special, attacktype));
                            playerHP -= damage;
                            if (playerHP <= 2) { break; }
                            if (tmpDice<=AttackPercent/5) { i--; }
                        }
                        if (selectNum==1 && avoid!=0) { selectNum = -1; }//スペシャルなら回避を連続でできる。
                    }
                    else
                    {
                        sentenceEnd = false;
                        if (attacktype==2) { StartCoroutine(EnemyMiss(i, enemyNum, true)); } else { StartCoroutine(EnemyMiss(i, enemyNum, false)); }
                    }
                    for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                    }
                }
            }//敵の攻撃
            if(selectNum == 0 && playerHP>2)
            {
                yield return StartCoroutine(PlayerBattle(detailAct, enemyHP, humanFlag, enemyNum,enemyMaxHP));
                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
            }//攻撃２（相手より遅い場合）
            if (selectNum == 2 && playerHP>2)
            {
                yield return StartCoroutine(Catcher(enemyNum,humanFlag,enemyHP));
                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
            }//拘束２（相手より遅い場合）
            if(selectNum==3 && playerHP>2)
            {
                yield return StartCoroutine(Hantei(tokusyuSkill, tokusyuSkillBonus));
                if (hanteikekka < 2)
                {
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    battleFlag = 0; yield return StartCoroutine(BattleEnd(playerHP)); yield break;
                }
                while (sentenceEnd == false) { yield return null; }
                for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                }
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
            }//特殊行動に成功したら戦闘終了
            //戦闘終了判定
            for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag==true) { sleep++; } if (enemyHP[i] <= 0) { kill++; if (humanFlag == true) { sleep--; } } }
            if (sleep + kill == enemyNum)
            {
                yield return StartCoroutine(BattleEnd(playerHP));
                if (kill == enemyNum) { battleFlag = 1; yield break; }//皆殺し勝利
                if (sleep == enemyNum) { battleFlag = 3; yield break; }//全員生存勝利
                battleFlag = 2; yield break;//生存・死亡両方あり勝利
            }//勝ち
            if (playerHP <= 2)
            {
                yield return StartCoroutine(BattleEnd(playerHP));
                if (playerHP <= 0) { battleFlag = 5; yield break; }//死亡敗北
                battleFlag = 4; yield break;//生存敗北
            }//負け
            sleep = 0;kill = 0;
        }
        //戦闘終了判定
        for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag==true) { sleep++; } if (enemyHP[i] <= 0) { kill++; if (humanFlag == true) { sleep--; } } }
        yield return StartCoroutine(BattleEnd(playerHP));
        if (maxTurnWin == true)
        {
            if (kill == enemyNum) { battleFlag = 1; yield break; }//皆殺し勝利
            if (kill==0) { battleFlag = 3; yield break; }//全員生存勝利
            battleFlag = 2; yield break;//生存・志望両方あり勝利
        }
        else
        {
            if (playerHP <= 0) { battleFlag = 5; yield break; }//死亡敗北
            battleFlag = 4; yield break;//生存敗北
        }
    }

    private IEnumerator BattleEnd(int playerHP)
    {
        PlayerPrefs.SetInt("[system]耐久力", playerHP);
        TextDraw(" ", "<size=72>戦闘終了</size>");
        yield return StartCoroutine(PushWait());
        for (int i = 0; i < 5; i++)
        {
            objCharacter[i].GetComponentInChildren<Text>().text = "";
            objCharacter[i].GetComponent<Image>().enabled = true;
            objCharacter[i].GetComponent<RectTransform>().localPosition = new Vector3(i * 150 - 300, CHARACTER_Y, 1);
            objCharacter[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayerBattle(int detailAct,int[] enemyHP,bool humanFlag,int enemyNum,int enemyMaxHP)
    {
        int damage;
        int y,z;
        int playerDB;
        int attack = 2;
        bool tmp=false;
        z = 3;
        Utility u1 = GetComponent<Utility>();
        if (detailAct == 0)
        {
            if (PlayerPrefs.GetInt("火器", 0) > 0)
            {
                for (int x = 0; x < z; x++)
                {
                    yield return StartCoroutine(Hantei("火器", 0));
                    attack = hanteikekka;
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (attack == 2)
                    {
                        sentenceEnd = false;
                        for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                        StartCoroutine(PlayerMiss(y, enemyNum));
                        objText.GetComponent<Text>().text = objText.GetComponent<Text>().text + "\n(" + (x + 1).ToString() + "発目/" + z.ToString() + "発)";
                        for (int i = 0; i < 100; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                    }
                    else
                    {
                        damage = u1.DiceRoll(1, 10);
                        sentenceEnd = false;
                        for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                        yield return StartCoroutine(DiceEffect(0, 10, damage));
                        for (int i = 0; i < 20; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                        enemyHP[y] -= damage + PlayerPrefs.GetInt("火器", 0);
                        StartCoroutine(PlayerHit(y, enemyNum, damage,0, PlayerPrefs.GetInt("火器", 0), 0, enemyHP, humanFlag, enemyMaxHP));
                        objText.GetComponent<Text>().text = objText.GetComponent<Text>().text + "\n(" + (x+1).ToString() + "発目/"+ z.ToString() +"発)";
                        if (attack == 0) { z++; objText.GetComponent<Text>().text = objText.GetComponent<Text>().text + "★追加攻撃"; }
                        for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                        objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                        if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].GetComponent<Image>().enabled=false; }
                        for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                        for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 0 && (enemyHP[i] > 2 || humanFlag == false)) { break; }if (i == enemyNum - 1) { tmp = true; } }
                        if (tmp == true) { break; }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 1; x++)
                {
                    yield return StartCoroutine(Hantei("投擲", 0));
                    attack = hanteikekka;
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (attack == 2)
                    {
                        sentenceEnd = false;
                        for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                        StartCoroutine(PlayerMiss(y, enemyNum));
                    }
                    else
                    {
                        damage = u1.DiceRoll(1, 10);
                        sentenceEnd = false;
                        for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                       yield return StartCoroutine(DiceEffect(0, 10, damage));
                        for (int i = 0; i < 20; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                        enemyHP[y] -= damage;
                        StartCoroutine(PlayerHit(y, enemyNum, damage,0, 0, 0, enemyHP, humanFlag, enemyMaxHP));
                        for (int v = 0; v < 60; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
                        }
                        if (attack == 0) { x--; }
                        for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                        objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                        if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].GetComponent<Image>().enabled = false; }
                        for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                        for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 0 && (enemyHP[i] > 2 || humanFlag == false)) { break; } if (i == enemyNum - 1) { tmp = true; } }
                        if (tmp == true) { break; }
                    }
                }
            }
        }
        if (detailAct == 1)
        {
            for (int x = 0; x < 1; x++)
            {
                yield return StartCoroutine(Hantei("格闘", 0));
                attack = hanteikekka;
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                if (attack == 0) { x--; }
                if (attack == 2)
                {
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    StartCoroutine(PlayerMiss(y,enemyNum));
                }
                else
                {
                    damage = u1.DiceRoll(1, 6);
                    yield return StartCoroutine(DiceEffect(0, 6, damage));
                    if (hanteiDice< PlayerPrefs.GetInt("[system]Skill46", 1))
                    {
                        int damage2;
                        damage2 = u1.DiceRoll(1, 6);
                        yield return StartCoroutine(AttackEffect(1));
                        yield return StartCoroutine(DiceEffect(1, 6, damage2));
                        damage += damage2;
                        for(int i = 0; i < 30; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                        }
                    }
                    playerDB = 0;
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == 6) { playerDB = u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == 4) { playerDB = u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == -4) { playerDB = -u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, -playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == -6) { playerDB = -u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, -playerDB)); }
                    for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                    }
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    if (damage + playerDB > 0) { enemyHP[y] -= damage + playerDB; }
                    StartCoroutine(PlayerHit(y,enemyNum, damage, playerDB,0, detailAct, enemyHP, humanFlag, enemyMaxHP));
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].GetComponent<Image>().enabled = false; }
                    for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                    }
                    for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 0 && (enemyHP[i] > 2 || humanFlag == false)) { break; } if (i == enemyNum - 1) { tmp = true; } }
                    if (tmp == true) { break; }
                }
            }
        }
        if (detailAct == 2)
        {
            for (int x = 0; x < 1; x++)
            {
                yield return StartCoroutine(Hantei("武器術", 0));
                attack = hanteikekka;
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                if (attack ==0) {x--; }
                if (attack == 2)
                {
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    StartCoroutine(PlayerMiss(y,enemyNum));
                }
                else
                {
                    damage = u1.DiceRoll(1, 10);
                    if (damage < 10) { StartCoroutine(DiceEffect(0, 10, damage)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
                    playerDB = 0;
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == 6) { playerDB = u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == 4) { playerDB = u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == -4) { playerDB = -u1.DiceRoll(1, 4); StartCoroutine(DiceEffect(1, 4, -playerDB)); }
                    if (PlayerPrefs.GetInt("[system]Status8", 0) == -6) { playerDB = -u1.DiceRoll(1, 6); StartCoroutine(DiceEffect(1, 6, -playerDB)); }
                    for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                    }
                    sentenceEnd = false;
                    for (y = 0; y < enemyNum - 1; y++) { if (enemyHP[y] >= 3 || (enemyHP[y] > 0 && humanFlag == false)) { break; } }
                    if (damage + playerDB > 0) { enemyHP[y] -= damage + playerDB + PlayerPrefs.GetInt("武器", 0); }
                    StartCoroutine(PlayerHit(y,enemyNum, damage, playerDB, PlayerPrefs.GetInt("武器", 0), detailAct,enemyHP,humanFlag,enemyMaxHP));
                    for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                    objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                    if (enemyHP[y] <= 0 || (enemyHP[y] <= 2 && humanFlag == true)) { objCharacter[y].GetComponent<Image>().enabled = false; }
                    for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                    }
                    for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 0 && (enemyHP[i] > 2 || humanFlag == false)) { break; } if (i == enemyNum - 1) { tmp = true; } }
                    if (tmp == true) { break; }
                }
            }
        }
        selectNum = -1;
    }

    /// <summary>
    /// effect:MA=1,Gun=2,Spell=3
    /// </summary>
    private IEnumerator AttackEffect(int effect)
    {
        Image bo = GameObject.Find("BlackOut").GetComponent<Image>();
        bo.enabled = true;
        Sprite sp= bo.sprite;
        string graph="MAEffect";
        if (effect == 1) { graph = "MAEffect"; }
        if (effect == 2) { graph = "GunEffect"; }
        if (effect == 3) { graph = "SpellEffect"; }
        bo.sprite = Resources.Load<Sprite>(graph);
        for (int i = 0; i < 60; i++)
        {
            if (i < 50) { bo.color = new Color(1, 1, 1, 1); }
            else
            {
                bo.color = new Color(1, 1, 1, (float)(59-i) / 9);
            }
#if UNITY_IOS
i++;
#endif
            yield return null;
        }
        bo.sprite = sp;
        bo.enabled = false;
    }

    private IEnumerator Status(int playerHP,int damage)
    {
        int maxHP = PlayerPrefs.GetInt("[system]Status9", 0);
        string color1="";
        string color2="";
        if(playerHP<=0){ color1 = "<color=red>";color2 = "</color>"; }else if (playerHP < 2) { color1 = "<color=#ff4000ff>";color2 = "</color>"; }else if(playerHP<=maxHP/2){ color1="<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
        objStatusHP.GetComponent<Text>().text = "耐久力　<size=48><b>" + color1 + playerHP.ToString() + color2 + "</b></size>/" + maxHP.ToString();
        for (int i = 0; i < 6; i++) { yield return null; }
        while (damage > 0)
        {
            playerHP--; damage--;
            if (playerHP <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerHP < 2) { color1 = "<color=#ff4000ff>"; color2 = "</color>"; } else if (playerHP <= maxHP / 2) { color1 = "<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
            objStatusHP.GetComponent<Text>().text = "耐久力　<size=48><b>" + color1 + playerHP.ToString() + color2 + "</b></size>/" + maxHP.ToString();
            for (int i=0;i<6;i++) { yield return null; }
        }
        while (damage < 0 && playerHP<maxHP)
        {
            playerHP++; damage++;
            if (playerHP <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerHP < 2) { color1 = "<color=#ff4000ff>"; color2 = "</color>"; } else if (playerHP <= maxHP / 2) { color1 = "<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
            objStatusHP.GetComponent<Text>().text = "耐久力　<size=48><b>" + color1 + playerHP.ToString() + color2 + "</b></size>/" + maxHP.ToString();
            for (int i = 0; i < 6; i++) { yield return null; }
        }
    }
    private IEnumerator StatusSAN(int playerSAN, int damage)
    {
        int maxSAN = 99-PlayerPrefs.GetInt("[system]Skill53", 0);
        string color1 = "";
        string color2 = "";
        if (playerSAN <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerSAN < 20) { color1 = "<color=#ff4000ff>"; color2 = "</color>"; } else if (playerSAN < 40) { color1 = "<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
        objStatusSAN.GetComponent<Text>().text = "正気度ポイント　<size=48><b>" + color1 + playerSAN.ToString() + color2 + "</b></size>/" + maxSAN.ToString();
        for (int i = 0; i < 6; i++) { yield return null; }
        while (damage > 0)
        {
            playerSAN--; damage--;
            if (playerSAN <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerSAN < 20) { color1 = "<color=#ff4000ff>"; color2 = "</color>"; } else if (playerSAN < 40) { color1 = "<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
            objStatusSAN.GetComponent<Text>().text = "正気度ポイント　<size=48><b>" + color1 + playerSAN.ToString() + color2 + "</b></size>/" + maxSAN.ToString();
            for (int i = 0; i < 6; i++) { yield return null; }
        }
        while (damage < 0 && playerSAN < maxSAN)
        {
            playerSAN++; damage++;
            if (playerSAN <= 0) { color1 = "<color=red>"; color2 = "</color>"; } else if (playerSAN < 20) { color1 = "<color=#ff4000ff>"; color2 = "</color>"; } else if (playerSAN < 40) { color1 = "<color=#ff8000ff>"; color2 = "</color>"; } else { color1 = ""; color2 = ""; }
            objStatusSAN.GetComponent<Text>().text = "正気度ポイント　<size=48><b>" + color1 + playerSAN.ToString() + color2 + "</b></size>/" + maxSAN.ToString();
            for (int i = 0; i < 6; i++) { yield return null; }
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
        if (humanFlag == false) {  TextDraw("", "拘束できる相手ではない……！"); for (int i = 0; i < 80; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
            }
        }//人外は拘束できない
        else
        {
            for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] <= 2 && humanFlag == true) { sleep++; } if (enemyHP[i] <= 0) { kill++; sleep--; } }
            for (catcherNum = 0; catcherNum < enemyNum - sleep - kill; catcherNum++)
            {
                objCharacter[y].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                yield return StartCoroutine(Hantei("格闘/2", 0));
                catcher = hanteikekka;
                for (int k = 0; k < 2; k++) { objDice[k].gameObject.SetActive(false); }
                objRollText.gameObject.SetActive(false);//ダイスは出っ放しにならない
                objCharacter[y].GetComponent<Image>().color = new Color(1, 1, 1);
                y++;
                for (int i = 0; i < 40; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
                }
                if (catcher == 0) { TextDraw("", "スペシャル成功！\r\nあなたは絶妙な動きで敵を次々と縛り付けた。\r\n"); catcherNum +=99;break; }
                if (catcher == 2) { TextDraw("", "失敗！\r\nあなたが手間取る隙に、全ての敵が拘束から抜け出した。\r\n"); break; }
                if (catcher == 1) { TextDraw("", "成功！\r\nあなたは一人を拘束した。\r\n残り：" + (enemyNum - sleep - kill-catcherNum-1).ToString() + "人"); continue; }
            }
            for (int i = 0; i < 60; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
            }
            if (enemyNum - sleep - kill <= catcherNum) { TextDraw("", "<color=blue>全ての敵を捕縛した！</color>"); for (int i = 0; i < enemyNum; i++) { if (enemyHP[i] > 2) { enemyHP[i] = 2; } } }//全員捕獲した場合のみ、それらのHPを２にして戦闘終了処理へ
            else
            {
                TextDraw("", "<color=red>敵の捕縛に失敗した……。</color>"); 
            }
        }
        selectNum = -1;
    }

    private IEnumerator PlayerMiss(int target,int enemyNum)
    {
        SystemSEPlay(systemAudio[7]);
        TextDraw("", "攻撃を外した！");
        for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
        }
    }

    private IEnumerator EnemyMiss(int target,int enemyNum,bool gunflag)
    {
        int targetGra=target;
        if (enemyNum == 1 && target == 0) { targetGra = 2; }
        if (enemyNum == 2) { if (target == 0) { targetGra = 1; } else { targetGra = 3; } }
        if (enemyNum == 3) { if (target == 0) { targetGra = 0; } else if (target==1) { targetGra = 2; } else { targetGra = 4; } }
        if (gunflag==true) { SystemSEPlay(systemAudio[4]); } else { SystemSEPlay(systemAudio[7]); }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "相手の攻撃は当たらなかった。");
        for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
        }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y, 0);
    }

    private IEnumerator PlayerHit(int target,int enemyNum,int damage,int db,int bonus,int detailAct,int[] enemyHP,bool humanflag,int enemyMaxHP)
    {
        int targetGra = target;
        int toutekiSound=0;
        if (PlayerPrefs.GetInt("火器", 0) <=0 && detailAct==0) { toutekiSound++; }
        if (enemyNum == 1 && target == 0) { targetGra = 2; }
        if (enemyNum == 2) { if (target == 0) { targetGra = 1; } else { targetGra = 3; } }
        if (enemyNum == 3) { if (target == 0) { targetGra = 0; } else if (target == 1) { targetGra = 2; } else { targetGra = 4; } }

        SystemSEPlay(systemAudio[4+detailAct+toutekiSound]);
        objCharacter[target].GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
        string bonusStr;
        if (bonus == 0) { bonusStr = ""; }
        else{ bonusStr = "+" + bonus.ToString(); }
        if (damage + db + bonus > 0)
        {
            if (db > 0)
            {
                TextDraw("", "damage→" + damage.ToString() + "+" + db.ToString() + bonusStr + "\n" + (damage + db+bonus).ToString() + "点のダメージを与えた。");
            }
            else if (db < 0)
            {
                TextDraw("", "damage→" + damage.ToString() + db.ToString() + bonusStr + "\n" + (damage + db+bonus).ToString() + "点のダメージを与えた。");
            }
            else
            {
                TextDraw("", "damage→" + damage.ToString() + bonusStr + "\n" + (damage + db+bonus).ToString() + "点のダメージを与えた。");
            }
            int tmp=0;
            int.TryParse((objCharacter[target].GetComponentInChildren<Text>().text).Replace("<color=yellow>","").Replace("</color>",""),out tmp);
            int totaldamage= tmp + damage + db + bonus;
            if (enemyHP[target] <= 0) { objCharacter[target].GetComponentInChildren<Text>().text = "<color=red>死亡</color>"; }
            else if (enemyHP[target] <= 2 && humanflag == true) { objCharacter[target].GetComponentInChildren<Text>().text = "<color=orange><size=48>戦闘不能</size></color>"; }
            else if (enemyHP[target]<=enemyMaxHP/2) { objCharacter[target].GetComponentInChildren<Text>().text = "<color=yellow>" + totaldamage.ToString() + "</color>"; }
            else { objCharacter[target].GetComponentInChildren<Text>().text = totaldamage.ToString(); }
        }
        else
        {
            if (db > 0)
            {
                TextDraw("", "damage→" + damage.ToString() + "+" + db.ToString() + bonusStr + "\n" + "ダメージを与えられない！");
            }
            else if (db < 0)
            {
                TextDraw("", "damage→" + damage.ToString() + db.ToString() + bonusStr + "\n" + "ダメージを与えられない！");
            }
            else
            {
                TextDraw("", "damage→" + damage.ToString() + bonusStr + "\n" + "ダメージを与えられない！");
            }
        }
        for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
        }
        objCharacter[target].GetComponent<Image>().color = new Color(1, 1, 1);
    }

    private IEnumerator EnemyHit(int target, int enemyNum, int damage, bool special, int attacktype)
    {
        int wait = 100;if (attacktype == 2 || attacktype == 3) { wait = 45; }
        int targetGra = target;
        if (enemyNum == 1 && target == 0) { targetGra = 2; }
        if (enemyNum == 2) { if (target == 0) { targetGra = 1; } else { targetGra = 3; } }
        if (enemyNum == 3) { if (target == 0) { targetGra = 0; } else if (target == 1) { targetGra = 2; } else { targetGra = 4; } }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y - 100, 0);
        if (special == false) { TextDraw("", damage.ToString() + "点のダメージを受けた。");
        }
        else { TextDraw("","<color=red>スペシャル攻撃！</color>\n" + damage.ToString() + "点のダメージを受けた。\n敵は追加攻撃を行う。"); }
        yield return StartCoroutine(AttackTypeEffect(attacktype));
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y, 0);
        for (int i = 0; i < wait; i++) { yield return null;
#if UNITY_IOS
i++;
#endif
        }
    }

    private IEnumerator Cut(int target,int enemyNum)
    {
        int targetGra = target;
        if (enemyNum == 1 && target == 0) { targetGra = 2; }
        if (enemyNum == 2) { if (target == 0) { targetGra = 1; } else { targetGra = 3; } }
        if (enemyNum == 3) { if (target == 0) { targetGra = 0; } else if (target == 1) { targetGra = 2; } else { targetGra = 4; } }

        SystemSEPlay(systemAudio[9]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "攻撃を受け流した。");
        for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
        }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y, 0);
    }

    private IEnumerator DamageShake()
    {
        for (int v = 0; v < 30; v++)
        {
           objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 5 * (v % 2), 0); 
            yield return null;
#if UNITY_IOS
v++;
#endif
        }
    }

    private IEnumerator AttackTypeEffect(int attacktype)
    {
        StartCoroutine(DamageShake());
        if (attacktype == 0) { SystemSEPlay(systemAudio[5]); }
        if (attacktype == 1) { SystemSEPlay(systemAudio[6]); }
        if (attacktype < 2)
        {
            Image bo = GameObject.Find("BlackOut").GetComponent<Image>();
            bo.enabled = true;
            for (int i = 0; i < 5; i++)
            {
                bo.color = new Color(1, 0, 0, (float)(5 - i) / 5);
                yield return null;
            }
            bo.enabled = false;
        }
        if (attacktype == 2)
        {
            SystemSEPlay(systemAudio[4]);
            yield return StartCoroutine(AttackEffect(2));
        }
        if (attacktype == 3)
        {
            SystemSEPlay(systemAudio[10]);
            yield return StartCoroutine(AttackEffect(3));
        }
    }

    private IEnumerator Avoid(int target,int enemyNum)
    {
        int targetGra = target;
        if (enemyNum == 1 && target == 0) { targetGra = 2; }
        if (enemyNum == 2) { if (target == 0) { targetGra = 1; } else { targetGra = 3; } }
        if (enemyNum == 3) { if (target == 0) { targetGra = 0; } else if (target == 1) { targetGra = 2; } else { targetGra = 4; } }

        SystemSEPlay(systemAudio[8]);
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y - 100, 0);
        TextDraw("", "攻撃を回避した。");
        for (int v = 0; v < 100; v++) { yield return null;
#if UNITY_IOS
v++;
#endif
        }
        objCharacter[target].GetComponent<RectTransform>().localPosition = new Vector3(targetGra * 150 - 300, CHARACTER_Y, 0);
    }

    private void SelectBoxMake(int choiceA, int choiceB, int choiceC, int choiceD,bool inBattleFlag)
    {
        if (inBattleFlag == false)
        {
            if (choiceA > 0 && choiceB > 0 && choiceC > 0 && choiceD > 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 500, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); objBox[3].GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0); for (int i = 0; i < 4; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB > 0 && choiceC > 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(0, 50, 0); for (int i = 0; i < 3; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB > 0 && choiceC == 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 350, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); for (int i = 0; i < 2; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
            if (choiceA > 0 && choiceB == 0 && choiceC == 0 && choiceD == 0) { objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 200, 0); for (int i = 0; i < 1; i++) { objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(660, 100); } }
        }
        else
        {
            objBox[0].GetComponent<RectTransform>().localPosition = new Vector3(-200, -300, 0); objBox[1].GetComponent<RectTransform>().localPosition = new Vector3(-200, -450, 0); objBox[2].GetComponent<RectTransform>().localPosition = new Vector3(200, -300, 0); objBox[3].GetComponent<RectTransform>().localPosition = new Vector3(200, -450, 0);
            for(int i=0;i<4;i++){ objBox[i].GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100); }
        }
    }

    public void SelePush(int selectnum)
    {
        selectNum=selectnum;
    }

    private IEnumerator Select(string choiceA,string choiceB,string choiceC,string choiceD,bool inBattleFlag)
    {
        objBox[0].gameObject.SetActive(true); objBox[0].GetComponentInChildren<Text>().text = choiceA;
        if (choiceB.Length>0) { objBox[1].gameObject.SetActive(true); objBox[1].GetComponentInChildren<Text>().text = choiceB; }
        if (choiceC.Length>0) { objBox[2].gameObject.SetActive(true); objBox[2].GetComponentInChildren<Text>().text = choiceC; }
        if (choiceD.Length>0) { objBox[3].gameObject.SetActive(true); objBox[3].GetComponentInChildren<Text>().text = choiceD; }
        SelectBoxMake(choiceA.Length, choiceB.Length, choiceC.Length, choiceD.Length,inBattleFlag);
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
        string target = targetStr;
        if (targetStr == "言いくるめ") { target = "[system]Skill0"; }
        if (targetStr == "医学") { target = "[system]Skill1"; }
        if (targetStr == "運転") { target = "[system]Skill2"; }
        if (targetStr == "応急手当") { target = "[system]Skill3"; }
        if (targetStr == "オカルト") { target = "[system]Skill4"; }
        if (targetStr == "回避") { target = "[system]Skill5"; }
        if (targetStr == "化学") { target = "[system]Skill6"; }
        if (targetStr == "鍵開け") { target = "[system]Skill7"; }
        if (targetStr == "隠す") { target = "[system]Skill8"; }
        if (targetStr == "隠れる") { target = "[system]Skill9"; }
        if (targetStr == "機械修理") { target = "[system]Skill10"; }
        if (targetStr == "聞き耳") { target = "[system]Skill11"; }
        if (targetStr == "芸術") { target = "[system]Skill12"; }
        if (targetStr == "経理") { target = "[system]Skill13"; }
        if (targetStr == "考古学") { target = "[system]Skill14"; }
        if (targetStr == "コンピューター") { target = "[system]Skill15"; }
        if (targetStr == "忍び歩き") { target = "[system]Skill16"; }
        if (targetStr == "写真術") { target = "[system]Skill17"; }
        if (targetStr == "重機械操作") { target = "[system]Skill18"; }
        if (targetStr == "乗馬") { target = "[system]Skill19"; }
        if (targetStr == "信用") { target = "[system]Skill20"; }
        if (targetStr == "心理学") { target = "[system]Skill21"; }
        if (targetStr == "人類学") { target = "[system]Skill22"; }
        if (targetStr == "水泳") { target = "[system]Skill23"; }
        if (targetStr == "製作") { target = "[system]Skill24"; }
        if (targetStr == "精神分析") { target = "[system]Skill25"; }
        if (targetStr == "生物学") { target = "[system]Skill26"; }
        if (targetStr == "説得") { target = "[system]Skill27"; }
        if (targetStr == "操縦") { target = "[system]Skill28"; }
        if (targetStr == "地質学") { target = "[system]Skill29"; }
        if (targetStr == "跳躍") { target = "[system]Skill30"; }
        if (targetStr == "追跡") { target = "[system]Skill31"; }
        if (targetStr == "電気修理") { target = "[system]Skill32"; }
        if (targetStr == "電子工学") { target = "[system]Skill33"; }
        if (targetStr == "天文学") { target = "[system]Skill34"; }
        if (targetStr == "投擲") { target = "[system]Skill35"; }
        if (targetStr == "登ハン") { target = "[system]Skill36"; }
        if (targetStr == "図書館") { target = "[system]Skill37"; }
        if (targetStr == "ナビゲート") { target = "[system]Skill38"; }
        if (targetStr == "値切り") { target = "[system]Skill39"; }
        if (targetStr == "博物学") { target = "[system]Skill40"; }
        if (targetStr == "物理学") { target = "[system]Skill41"; }
        if (targetStr == "変装") { target = "[system]Skill42"; }
        if (targetStr == "法律") { target = "[system]Skill43"; }
        if (targetStr == "ほかの言語") { target = "[system]Skill44"; }
        if (targetStr == "母国語") { target = "[system]Skill45"; }
        if (targetStr == "マーシャルアーツ") { target = "[system]Skill46"; }
        if (targetStr == "目星") { target = "[system]Skill47"; }
        if (targetStr == "薬学") { target = "[system]Skill48"; }
        if (targetStr == "歴史") { target = "[system]Skill49"; }
        if (targetStr == "火器") { target = "[system]Skill50"; }
        if (targetStr == "格闘") { target = "[system]Skill51"; }
        if (targetStr == "武器術") { target = "[system]Skill52"; }
        if (targetStr == "クトゥルフ神話") { target = "[system]Skill53"; }
        if (targetStr == "STR") { target = "[system]Status0"; }
        if (targetStr == "DEX") { target = "[system]Status2"; }
        if (targetStr == "CON") { target = "[system]Status1"; }
        if (targetStr == "POW") { target = "[system]Status5"; }
        if (targetStr == "INT") { target = "[system]Status3"; }
        if (targetStr == "EDU") { target = "[system]Status7"; }
        if (targetStr == "SIZ") { target = "[system]Status6"; }
        if (targetStr == "APP") { target = "[system]Status4"; }
        if (targetStr == "最大マジック・ポイント") { target = "[system]Status10"; }
        if (targetStr == "最大耐久力") { target = "[system]Status9"; }
        if (targetStr == "マジック・ポイント") { target = "[system]マジック・ポイント"; }
        if (targetStr == "耐久力") { target = "[system]耐久力"; }
        if (targetStr == "正気度ポイント") { target = "[system]正気度ポイント"; }
        if (targetStr == "アイデア") { PlayerPrefs.SetInt("[system]アイデア", PlayerPrefs.GetInt("[system]Status3")*5);target = "[system]アイデア"; }
        if (targetStr == "知識") { PlayerPrefs.SetInt("[system]知識", PlayerPrefs.GetInt("[system]Status7") * 5); target = "[system]知識"; }
        if (targetStr == "幸運") { PlayerPrefs.SetInt("[system]幸運", PlayerPrefs.GetInt("[system]Status5") * 5); target = "[system]幸運"; }
        return target;
    }

    private string ReverseSkillList(string targetStr)
    {
        string target = targetStr;
        if (targetStr == "[system]Skill0") { target = "言いくるめ"; }
        if (targetStr == "[system]Skill1") { target = "医学"; }
        if (targetStr == "[system]Skill2") { target = "運転"; }
        if (targetStr == "[system]Skill3") { target = "応急手当";} 
        if (targetStr == "[system]Skill4") { target = "オカルト";}
        if (targetStr == "[system]Skill5") { target = "回避";}
        if (targetStr == "[system]Skill6") { target = "化学";}
        if (targetStr == "[system]Skill7") { target = "鍵開け";}
        if (targetStr == "[system]Skill8") { target = "隠す";} 
        if (targetStr == "[system]Skill9") { target = "隠れる";}
        if (targetStr == "[system]Skill10") { target = "機械修理";}
        if (targetStr == "[system]Skill11") { target = "聞き耳";}
        if (targetStr == "[system]Skill12") { target = "芸術";} 
        if (targetStr == "[system]Skill13") { target = "経理";} 
        if (targetStr == "[system]Skill14") { target = "考古学";} 
        if (targetStr == "[system]Skill15") { target = "コンピューター";} 
        if (targetStr == "[system]Skill16") { target = "忍び歩き";} 
        if (targetStr == "[system]Skill17") { target = "写真術";} 
        if (targetStr == "[system]Skill18") { target = "重機械操作";} 
        if (targetStr == "[system]Skill19") { target = "乗馬";}
        if (targetStr == "[system]Skill20") { target = "信用";}
        if (targetStr == "[system]Skill21") { target = "心理学";} 
        if (targetStr == "[system]Skill22") { target = "人類学";} 
        if (targetStr == "[system]Skill23") { target = "水泳";} 
        if (targetStr == "[system]Skill24") { target = "製作";}
        if (targetStr == "[system]Skill25") { target = "精神分析";} 
        if (targetStr == "[system]Skill26") { target = "生物学";} 
        if (targetStr == "[system]Skill27") { target = "説得";}
        if (targetStr == "[system]Skill28") { target = "操縦";} 
        if (targetStr == "[system]Skill29") { target = "地質学";}
        if (targetStr == "[system]Skill30") { target = "跳躍";}
        if (targetStr == "[system]Skill31") { target = "追跡";} 
        if (targetStr == "[system]Skill32") { target = "電気修理";} 
        if (targetStr == "[system]Skill33") { target = "電子工学";} 
        if (targetStr == "[system]Skill34") { target = "天文学";} 
        if (targetStr == "[system]Skill35") { target = "投擲";} 
        if (targetStr == "[system]Skill36") { target = "登ハン";}
        if (targetStr == "[system]Skill37") { target = "図書館";} 
        if (targetStr == "[system]Skill38") { target = "ナビゲート";} 
        if (targetStr == "[system]Skill39") { target = "値切り";}
        if (targetStr == "[system]Skill40") { target = "博物学";}
        if (targetStr == "[system]Skill41") { target = "物理学";}
        if (targetStr == "[system]Skill42") { target = "変装";}
        if (targetStr == "[system]Skill43") { target = "法律";} 
        if (targetStr == "[system]Skill44") { target = "ほかの言語";} 
        if (targetStr == "[system]Skill45") { target = "母国語";}
        if (targetStr == "[system]Skill46") { target = "マーシャルアーツ";} 
        if (targetStr == "[system]Skill47") { target = "目星";} 
        if (targetStr == "[system]Skill48") { target = "薬学";} 
        if (targetStr == "[system]Skill49") { target = "歴史";} 
        if (targetStr == "[system]Skill50") { target = "火器";} 
        if (targetStr == "[system]Skill51") { target = "格闘";} 
        if (targetStr == "[system]Skill52") { target = "武器術";} 
        return target;
    }



    public void HanteiDiceRoll()
    {
        hanteiWait = false;
        objDiceButton.SetActive(false);
    }

    private IEnumerator Hantei(string targetStr,int bonus,bool growflag=false)
    {
        int target=0;
        string bonusStr="";
        TextDraw("判定", "　");
        target =SkillCheck(targetStr);
        if (bonus > 0) { bonusStr = " + " + bonus.ToString(); }
        if (bonus < 0) { bonusStr = " - " + (-1*bonus).ToString(); }
        objRollText.gameObject.SetActive(true);
        objDiceButton.SetActive(true);
        if (target > -bonus) { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + (target + bonus).ToString() + "</color>"; } else { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + "自動失敗" + "</color>"; }
        Utility u1 = GetComponent<Utility>();
        objTextBox.gameObject.SetActive(true);
        hanteiWait = true;
        proxySkill = targetStr;
        proxyBase = targetStr;
        baseBonus = bonus;
        if (targetStr==SkillList(targetStr) || proxyBase.Contains("回避") || proxyBase.Contains("投擲") || proxyBase.Contains("火器") || proxyBase.Contains("武器術") || proxyBase.Contains("格闘") || proxyBase.Contains("幸運") || proxyBase.Contains("知識") || proxyBase.Contains("アイデア") || proxyBase.Contains("正気度ポイント") || proxyBase.Contains("耐久力") || proxyBase.Contains("マジック・ポイント") || proxyBase.Contains("最大耐久力") || proxyBase.Contains("最大マジック・ポイント") ||
            proxyBase.Contains("APP") || proxyBase.Contains("SIZ") || proxyBase.Contains("EDU") || proxyBase.Contains("INT") || proxyBase.Contains("POW") || proxyBase.Contains("CON") || proxyBase.Contains("DEX") || proxyBase.Contains("STR") || growflag==true) { objProxySkillButton.SetActive(false); }
        else
        {
            objProxySkillButton.SetActive(true);
        }
        while (hanteiWait) { yield return null; }
        objProxySkillButton.SetActive(false);
        targetStr = proxySkill;
        target = SkillCheck(targetStr);
        hanteiDice =u1.DiceRoll(1, 100);
        if (hanteiDice != 100) { StartCoroutine(DiceEffect(0, 10, hanteiDice / 10)); } else { StartCoroutine(DiceEffect(0, 10, 0)); }
        yield return StartCoroutine(DiceEffect(1, 10, hanteiDice % 10));
        yield return StartCoroutine(DiceText(hanteiDice, target, bonus,targetStr,bonusStr,growflag));
        string success = SkillList(targetStr);
        if (hanteiDice > target + bonus)
        {
            hanteikekka=2;
            yield break;
        }
        else
        {
            if (hanteiDice <= (target+bonus)/5)
            {
                hanteikekka=0;
                if (success != targetStr) { PlayerPrefs.SetInt("s" + success, 1); }
                yield break;
            }
            hanteikekka=1;
            if (success != targetStr) { PlayerPrefs.SetInt("s" + success, 1); }
            yield break;
        }
    }

    //代用技能ボタンを押した時
    public void ProxySkillButton()
    {
        objRollText.SetActive(false);
        StartCoroutine(ProxySkillButtonCor());    
    }

    private IEnumerator ProxySkillButtonCor()
    {
        int target = 0;
        int beforeselect;
        string bonusStr = "";
        string targetStr = "";
        string[] proxy = new string[3];
        MakeProxy(ref proxy);
        beforeselect=selectNum;
        if (baseBonus > 0) { bonusStr = " + " + baseBonus.ToString(); }
        if (baseBonus < 0) { bonusStr = " - " + (-1 * baseBonus).ToString(); }
        yield return StartCoroutine(Select(proxyBase+bonusStr, proxy[0]+bonusStr, proxy[1]+bonusStr, proxy[2]+bonusStr, false));
        if (selectNum == 0) { targetStr = proxyBase; }//等倍技能
        if (selectNum == 1) { targetStr = proxy[0]; }//代用１
        if (selectNum == 2) { targetStr = proxy[1]; }//代用２
        if (selectNum == 3) { targetStr = proxy[2]; }//代用３
        selectNum = beforeselect;
        sentenceEnd=false;
        target = SkillCheck(targetStr);
        objRollText.SetActive(true);
        if (target > -baseBonus) { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + (target + baseBonus).ToString() + "</color>"; } else { objRollText.GetComponent<Text>().text = targetStr + bonusStr + "\n" + "<color=#88ff88ff>" + "自動失敗" + "</color>"; }
        proxySkill = targetStr;
    }

    private void MakeProxy(ref string[] proxy)
    {
        if (proxyBase.Contains("言いくるめ")) { proxy[0] = "説得/2"; proxy[1] = "信用/2"; proxy[2] = "値切り/2"; }
        if (proxyBase.Contains("医学")) { proxy[0] = "応急手当/5"; proxy[1] = "生物学/5"; proxy[2] = "薬学/5"; }
        if (proxyBase.Contains("運転")) { proxy[0] = "操縦/2"; proxy[1] = "重機械操作/2"; proxy[2] = "乗馬/5"; }
        if (proxyBase.Contains("応急手当")) { proxy[0] = "医学"; proxy[1] = "薬学/3"; proxy[2] = "生物学/5"; }
        if (proxyBase.Contains("オカルト")) { proxy[0] = "歴史/4"; proxy[1] = "考古学/2"; proxy[2] = "人類学/2"; }
        if (proxyBase.Contains("化学")) { proxy[0] = "生物学/2"; proxy[1] = "地質学/2"; proxy[2] = "医学/4"; }
        if (proxyBase.Contains("鍵開け")) { proxy[0] = "機械修理/2"; proxy[1] = "隠す/5"; proxy[2] = "製作/2"; }
        if (proxyBase.Contains("隠す")) { proxy[0] = "鍵開け/2"; proxy[1] = "隠れる/5"; proxy[2] = "DEX"; }
        if (proxyBase.Contains("隠れる")) { proxy[0] = "変装"; proxy[1] = "隠す/5"; proxy[2] = "忍び歩き/3"; }
        if (proxyBase.Contains("機械修理")) { proxy[0] = "電気修理/2"; proxy[1] = "製作/2"; proxy[2] = "重機械操作/2"; }
        if (proxyBase.Contains("聞き耳")) { proxy[0] = "INT"; proxy[1] = "マーシャルアーツ/2"; proxy[2] = "POW"; }
        if (proxyBase.Contains("芸術")) { proxy[0] = "製作/2"; proxy[1] = "INT"; proxy[2] = "POW"; }
        if (proxyBase.Contains("経理")) { proxy[0] = "法律/2"; proxy[1] = "EDU"; proxy[2] = "値切り/2"; }
        if (proxyBase.Contains("考古学")) { proxy[0] = "歴史/2"; proxy[1] = "オカルト/5"; proxy[2] = "博物学/5"; }
        if (proxyBase.Contains("コンピューター")) { proxy[0] = "電子工学/2"; proxy[1] = "INT"; proxy[2] = "EDU"; }
        if (proxyBase.Contains("忍び歩き")) { proxy[0] = "隠れる/2"; proxy[1] = "変装/2"; proxy[2] = "DEX"; }
        if (proxyBase.Contains("写真術")) { proxy[0] = "INT"; proxy[1] = "POW"; proxy[2] = "EDU"; }
        if (proxyBase.Contains("重機械操作")) { proxy[0] = "運転/5"; proxy[1] = "操縦/5"; proxy[2] = "INT"; }
        if (proxyBase.Contains("乗馬")) { proxy[0] = "DEX"; proxy[1] = "操縦/5"; proxy[2] = "マーシャルアーツ/5"; }
        if (proxyBase.Contains("信用")) { proxy[0] = "法律/5"; proxy[1] = "変装"; proxy[2] = "説得/5"; }
        if (proxyBase.Contains("心理学")) { proxy[0] = "精神分析/2"; proxy[1] = "変装/3"; proxy[2] = "POW"; }
        if (proxyBase.Contains("人類学")) { proxy[0] = "心理学/2"; proxy[1] = "変装/2"; proxy[2] = "EDU"; }
        if (proxyBase.Contains("水泳")) { proxy[0] = "DEX"; proxy[1] = "CON"; proxy[2] = "STR"; }
        if (proxyBase.Contains("製作")) { proxy[0] = "機械修理/2"; proxy[1] = "電気修理/2"; proxy[2] = "芸術/2"; }
        if (proxyBase.Contains("精神分析")) { proxy[0] = "人類学/5"; proxy[1] = "心理学/5"; proxy[2] = "芸術/5"; }
        if (proxyBase.Contains("生物学")) { proxy[0] = "薬学/2"; proxy[1] = "医学/4"; proxy[2] = "化学/2"; }
        if (proxyBase.Contains("説得")) { proxy[0] = "言いくるめ/2"; proxy[1] = "信用/2"; proxy[2] = "値切り/5"; }
        if (proxyBase.Contains("操縦")) { proxy[0] = "運転/2"; proxy[1] = "重機械操作/2"; proxy[2] = "乗馬/5"; }
        if (proxyBase.Contains("地質学")) { proxy[0] = "化学/5"; proxy[1] = "考古学/5"; proxy[2] = "博物学/2"; }
        if (proxyBase.Contains("跳躍")) { proxy[0] = "登ハン/5"; proxy[1] = "DEX"; proxy[2] = "乗馬/5"; }
        if (proxyBase.Contains("追跡")) { proxy[0] = "ナビゲート/2"; proxy[1] = "変装/5"; proxy[2] = "INT"; }
        if (proxyBase.Contains("電気修理")) { proxy[0] = "機械修理/3"; proxy[1] = "電子工学/2"; proxy[2] = "物理学/3"; }
        if (proxyBase.Contains("電子工学")) { proxy[0] = "電気修理/5"; proxy[1] = "物理学/2"; proxy[2] = "EDU"; }
        if (proxyBase.Contains("天文学")) { proxy[0] = "物理学/2"; proxy[1] = "歴史/5"; proxy[2] = "EDU"; }
        if (proxyBase.Contains("登ハン")) { proxy[0] = "跳躍/2"; proxy[1] = "DEX"; proxy[2] = "乗馬/5"; }
        if (proxyBase.Contains("図書館")) { proxy[0] = "EDU"; proxy[1] = "博物学/5"; proxy[2] = "ナビゲート/5"; }
        if (proxyBase.Contains("ナビゲート")) { proxy[0] = "追跡/4"; proxy[1] = "天文学/5"; proxy[2] = "POW"; }
        if (proxyBase.Contains("値切り")) { proxy[0] = "言いくるめ/3"; proxy[1] = "説得/4"; proxy[2] = "信用/5"; }
        if (proxyBase.Contains("博物学")) { proxy[0] = "地質学/3"; proxy[1] = "人類学/4"; proxy[2] = "考古学/5"; }
        if (proxyBase.Contains("物理学")) { proxy[0] = "博物学/5"; proxy[1] = "天文学/4"; proxy[2] = "INT"; }
        if (proxyBase.Contains("変装")) { proxy[0] = "APP"; proxy[1] = "DEX/2"; proxy[2] = "隠す/9"; }
        if (proxyBase.Contains("法律")) { proxy[0] = "言いくるめ/5"; proxy[1] = "EDU"; proxy[2] = "経理/5"; }
        if (proxyBase.Contains("ほかの言語")) { proxy[0] = "EDU/2"; proxy[1] = "母国語/9"; proxy[2] = "信用/5"; }
        if (proxyBase.Contains("母国語")) { proxy[0] = "知識"; proxy[1] = "EDU"; proxy[2] = "歴史"; }
        if (proxyBase.Contains("マーシャルアーツ")) { proxy[0] = "格闘/5"; proxy[1] = "乗馬/5"; proxy[2] = "武器術/5"; }
        if (proxyBase.Contains("目星")) { proxy[0] = "聞き耳/5"; proxy[1] = "INT"; proxy[2] = "POW"; }
        if (proxyBase.Contains("薬学")) { proxy[0] = "化学/2"; proxy[1] = "医学/3"; proxy[2] = "生物学/2"; }
        if (proxyBase.Contains("歴史")) { proxy[0] = "考古学/2"; proxy[1] = "博物学/2"; proxy[2] = "人類学/2"; }
        if (proxyBase.Contains("クトゥルフ神話")) { proxy[0] = "考古学/9"; proxy[1] = "POW/9"; proxy[2] = "オカルト/9"; }

        string[] tmp2;
        int x1, x2;
        char[] tmpchar = { '*', '/' };
        x2 = 1;
        if (proxyBase.Contains("*") || proxyBase.Contains("/")) { tmp2 = proxyBase.Split(tmpchar); int.TryParse(tmp2[1], out x2); }

        for (int i = 0; i < 3; i++)
        {
            string[] tmp;
            if (proxy[i].Contains("/"))
            {
                tmp = proxy[i].Split('/');
                int.TryParse(tmp[1], out x1);
                if (proxyBase.Contains("*"))
                {
                    if (x2 >= x1) { proxy[i] = tmp[0] + "*" + (x2 / x1).ToString(); }
                    else { proxy[i] = tmp[0] + "/" + (x1 / x2).ToString(); }
                }
                if (proxyBase.Contains("/"))
                {
                    proxy[i] = tmp[0] + "/" + (x1 * x2).ToString();
                }
            }
            else {
                if (proxyBase.Contains("/"))
                {
                    proxy[i] = proxy[i] + "/" + x2.ToString();
                }
                if (proxyBase.Contains("*"))
                {
                    proxy[i] = proxy[i] + "*" + x2.ToString();
                }
            }
        }
    }






    private IEnumerator DiceText(int dice, int target, int bonus,string targetStr,string bonusStr,bool growflag)
    {
        //for (int j = 0; j < 50; j++) { yield return null; }
        if (dice > target + bonus)
        {
            if (growflag)
            {
                objText.GetComponent<Text>().text = "<color=#0000ffff>[DiceRoll]\n1D100→　" + dice.ToString() + " > " + (target + bonus).ToString() + " (<" + targetStr + ">" + bonusStr + ")\n<size=72>（1D10成長）</size></color>";
                for (int j = 0; j < 40; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
                SystemSEPlay(systemAudio[1]);
            }
            else
            {
                objText.GetComponent<Text>().text = "<color=#ff0000ff>[DiceRoll]\n1D100→　" + dice.ToString() + " > " + (target + bonus).ToString() + " (<" + targetStr + ">" + bonusStr + ")\n<size=72>（失敗）</size></color>";
                for (int j = 0; j < 40; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
                SystemSEPlay(systemAudio[2]);
            }
        }
        if (dice <= target + bonus)
        {
            if (growflag)
            {
                objText.GetComponent<Text>().text = "<color=#ff0000ff>DiceRoll:1D100→  " + dice.ToString() + " <= " + (target + bonus).ToString() + "\n　　　　　　　　" + targetStr + bonusStr + "   （成長せず）</color>";
                for (int j = 0; j < 40; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
                SystemSEPlay(systemAudio[2]);
            }
            else
            {
                objText.GetComponent<Text>().text = "<color=#000099ff>DiceRoll:1D100→  " + dice.ToString() + " <= " + (target + bonus).ToString() + "\n　　　　　　　　" + targetStr + bonusStr + "   （成功）</color>";
                if (dice <= (target + bonus) / 5)
                {
                    objText.GetComponent<Text>().text = "<color=#0000ffff>DiceRoll:1D100→  " + dice.ToString() + " << " + (target + bonus).ToString() + "\n　　　　　　　　" + targetStr + bonusStr + "   （スペシャル）</color>";
                }
                for (int j = 0; j < 40; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
                SystemSEPlay(systemAudio[1]);
            }
        }
        yield return StartCoroutine(PushWait());    
    }


        private IEnumerator DiceEffect(int dicenum,int dicetype,int num)
    {
        objDice[dicenum].gameObject.SetActive(true);

        if (dicetype == 10)
        {
            for (int i = 0; i < 7; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice10Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
            }
            if (num >= 10) { num = 0; }
            objDice[dicenum].GetComponent<Image>().sprite = dice10Graphic[num];
        }
        if (dicetype == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice6Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
            }
            objDice[dicenum].GetComponent<Image>().sprite = dice6Graphic[num-1];
        }
        if (dicetype == 4)
        {
            for (int i = 0; i < 7; i++)
            {
                objDice[dicenum].GetComponent<Image>().sprite = moveDice4Graphic[i];
                for (int j = 0; j < 6; j++) { yield return null;
#if UNITY_IOS
j++;
#endif
                }
            }
            objDice[dicenum].GetComponent<Image>().sprite = dice4Graphic[num-1];
        }
        SystemSEPlay(systemAudio[0]);
    }

    private string[] TextReplace(string name,string text)
    {
        string[] backtext = new string[3];
        string yourName;
        string yourNickName;
        if (PlayerPrefs.GetString("[system]PlayerCharacterName", "") == "") { yourName = "名無し"; } else { yourName = PlayerPrefs.GetString("[system]PlayerCharacterName", "名無し"); }
        if (PlayerPrefs.GetString("[system]PlayerCharacterNickName", "") == "") { yourNickName = yourName; } else { yourNickName = PlayerPrefs.GetString("[system]PlayerCharacterNickName", "名無し"); }
        text = text.Replace("[system]改行", "\r\n").Replace("[PC]", yourNickName).Replace("<PC>", yourNickName);

        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("<FLAG：.+?>");
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(text, "<FLAG：.+?>");
        while (match.Success)
        {
            string tmpstr = PlayerPrefs.GetInt(match.ToString().Replace("<FLAG：", "").Replace(">", ""), 0).ToString();
            text = System.Text.RegularExpressions.Regex.Replace(text,match.ToString(), tmpstr);
            match = match.NextMatch();
        }
        backtext[1] = text.Replace(" ", "\u00A0");
        string text2=text;
        System.Text.RegularExpressions.Regex reg2 = new System.Text.RegularExpressions.Regex("<size=.+?>");
        System.Text.RegularExpressions.Match match2 = System.Text.RegularExpressions.Regex.Match(text, "<size=.+?>");
        while (match2.Success)
        {
            text2 = reg2.Replace(text2,"");
            match2 = match2.NextMatch();
        }
        backtext[2]=text2.Replace("</size>","").Replace(" ", "\u00A0") + "\r\n";
        if (name == "[PC]" || name == "<PC>")
            {
                backtext[0] =  yourName.Replace(" ", "\u00A0");
            }
            else
            {
                backtext[0] = name.Replace(" ", "\u00A0");
            }
        return backtext;
    }

    private void TextDraw(string name,string text)
    {
        objBackText.gameObject.SetActive(false);
        objTextBox.gameObject.SetActive(true);
        objText.GetComponent<Text>().text=text;
        objName.GetComponent<Text>().text = " " + name;
    }

    private void BackTextDraw(string text)
    {
        //背景テキスト表示の際は通常テキスト欄は消す
        objTextBox.gameObject.SetActive(false);
        objBackText.gameObject.SetActive(true);
        objBackText.GetComponent<Text>().text = text;
        if (text == "") {ScreenSizeChanger.SetActive(false);}
    }

    private void BackDraw(int back)
    {
        objBackImage.GetComponent<Image>().sprite = scenarioGraphic[back];
        backNum = back;
    }

    private void CharacterDraw(int character, int position)
    {
        if (character == -1) { objCharacter[position - 1].gameObject.SetActive(false); return; }
        objCharacter[position - 1].gameObject.SetActive(true);
        objCharacter[position - 1].GetComponent<Image>().sprite = scenarioGraphic[character];
        ObjSizeChangeToGraph(position-1,scenarioGraphic[character]);
    }

    private void ItemDraw(int item)
    {
        int back;
        int itemNumMax = 100;
        back = backNum;
        BackDraw(item);
        backNum = back;
        BackTextDraw("");
        for (int i = 0; i < itemNumMax; i++) { if (PlayerPrefs.GetString("[system]Item" + i.ToString())==scenarioGraphicToPath[item]) { return; } }//既に持ってたらセーブしない。
        PlayerPrefs.SetString("[system]Item" + PlayerPrefs.GetInt("[system]所持アイテム数", 0).ToString(),scenarioGraphicToPath[item]);
        PlayerPrefs.SetInt("[system]所持アイテム数", PlayerPrefs.GetInt("[system]所持アイテム数", 0) + 1);
    }

    private void BGMPlay(int bgm)
    {
        Utility u1 = GetComponent<Utility>();
        u1.BGMPlay(scenarioAudio[bgm]);
    }

    private void BGMIn(int time)
    {
        Utility u1 = GetComponent<Utility>();
        StartCoroutine(u1.BGMFadeIn(time*60));
    }

    private void BGMOut(int time)
    {
        Utility u1 = GetComponent<Utility>();
        StartCoroutine(u1.BGMFadeOut(time*60));
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
        objBackText.gameObject.SetActive(false);
        objTextBox.gameObject.SetActive(true);
        objText.GetComponent<Text>().text = "　";
        objName.GetComponent<Text>().text = "　";
            for (int i = 0; i < 5; i++)//キャラクター移動
        {
            if (lr == "L" && objCharacter[position-1].activeSelf)
            {
                objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 + i * 6 - 300 - 30, CHARACTER_Y, 0);
            }
            if (lr == "R" && objCharacter[position - 1].activeSelf)
            {
                objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - i * 6 - 300 + 30, CHARACTER_Y, 0);
            }
            if (lr == "N") {; }//Nなら動きなし
            yield return null;
#if UNITY_IOS
i++;
#endif
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
#if UNITY_IOS
i++;
#endif
        }
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        sentenceEnd = true;
    }

    //キャラクターの小ジャンプ
    private IEnumerator CharacterJump(int position)
    {
        objBackText.gameObject.SetActive(false);
        objTextBox.gameObject.SetActive(true);
        for (int i = 0; i < 7; i++)
        {
            objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y + i * 2, 1);
            yield return null;
#if UNITY_IOS
i++;
#endif
        }
        for (int i = 7; i > 0; i--)
        {
            objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y + i * 2, 1);
            yield return null;
#if UNITY_IOS
i--;
#endif
        }
        objCharacter[position - 1].GetComponent<RectTransform>().localPosition = new Vector3((position - 1) * 150 - 300, CHARACTER_Y, 1);
        sentenceEnd = true;
    }


    private IEnumerator MainCoroutine()
    {
        BGMManager b1 = objBGM.GetComponent<BGMManager>();
        //シナリオデータ読込
        LoadScenario(b1.chapterName);
        //シナリオ処理
        yield return StartCoroutine(NovelGame());
    }



    //目次ファイルを読み込み、ファイルを拾ってくる。
    private void LoadScenario(string path)
    {
        // 目次ファイルが無かったら終わる
        if (!File.Exists(_FILE_HEADER))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルが見当たりません。" + _FILE_HEADER + "\\" + path);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
        }

        try
        {
            //閲覧するエントリ
            string extractFile = path;
            ICSharpCode.SharpZipLib.Zip.ZipFile zf;
            //ZipFileオブジェクトの作成
            zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(_FILE_HEADER);//説明に書かれてる以外のエラーが出てる。

            zf.Password = Secret.SecretString.zipPass;
            //展開するエントリを探す
            ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
            ze = zf.GetEntry(extractFile);

            if (ze != null)
            {
                //閲覧するZIPエントリのStreamを取得
                Stream reader = zf.GetInputStream(ze);
                //文字コードを指定してStreamReaderを作成
                StreamReader sr = new StreamReader(
                    reader, System.Text.Encoding.GetEncoding("UTF-8"));
                // テキストを取り出す
                string text = sr.ReadToEnd();

                // 読み込んだ目次テキストファイルからstring配列を作成する
                scenarioFilePath = text.Split('\n');

                //アドレスから各ファイルをロード
                for (int i = 0; i < scenarioFilePath.Length; i++)
                {
                    if (scenarioFilePath[i] == "[END]") { break; }
                    LoadFile(scenarioFilePath[i].Replace("\r", "").Replace("\n", ""), zf);
                }
                //閉じる
                sr.Close();
                reader.Close();
            }
            else
            {
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
            }
            //閉じる
            zf.Close();
        }
        catch
        {
            obj.GetComponent<Text>().text = ("エラーZIP。シナリオファイルの形式が不適合です。" + _FILE_HEADER + "\\" + path);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
        }
    }

    //引数pathのみのバージョン。テキストデータの読み込みのみ対応。（スクリプトからの直呼び出し『NextFile:』用）
    private void LoadFile(string path)
    {
        // 目次ファイルが無かったら終わる
        if (!File.Exists(_FILE_HEADER))
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルが見当たりません。" + _FILE_HEADER + "\\" + path);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
        }
        try
        {
            //閲覧するエントリ
            string extractFile = path;

            //ZipFileオブジェクトの作成
            ICSharpCode.SharpZipLib.Zip.ZipFile zf =
                new ICSharpCode.SharpZipLib.Zip.ZipFile(_FILE_HEADER);
            zf.Password = Secret.SecretString.zipPass;
            //展開するエントリを探す
            ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zf.GetEntry(extractFile);

            if (ze != null)
            {
                //閲覧するZIPエントリのStreamを取得
                Stream reader = zf.GetInputStream(ze);
                //文字コードを指定してStreamReaderを作成
                StreamReader sr = new StreamReader(
                    reader, System.Text.Encoding.GetEncoding("UTF-8"));
                // テキストを取り出す
                string text = sr.ReadToEnd();

                // 読み込んだ目次テキストファイルからstring配列を作成する
                scenarioText = text.Split('\n');

                //閉じる
                sr.Close();
                reader.Close();
                sectionName = path;
            }
            else
            {
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
            }
            //閉じる
            zf.Close();
        }
        catch
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルの形式が不適合です。" + _FILE_HEADER + "\\" + path);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
        }

    }

    private void LoadFile(string path, ICSharpCode.SharpZipLib.Zip.ZipFile zf)
    {
        byte[] buffer;
        if (path.Replace("\r", "").Replace("\n", "") == "g") { gNum++;return; }
        if (path.Replace("\r", "").Replace("\n", "") == "s") { sNum++;return; }
        try
        {
            //閲覧するエントリ
            string extractFile = path;
            //展開するエントリを探す
            ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zf.GetEntry(extractFile);

            if (ze != null)
            {
                //txtファイルの場合
                if (path.Substring(path.Length - 4) == ".txt")
                {
                    //閲覧するZIPエントリのStreamを取得
                    Stream reader = zf.GetInputStream(ze);
                    //文字コードを指定してStreamReaderを作成
                    StreamReader sr = new StreamReader(
                        reader, System.Text.Encoding.GetEncoding("UTF-8"));
                    // テキストを取り出す
                    string text = sr.ReadToEnd();

                    // 読み込んだ目次テキストファイルからstring配列を作成する
                    scenarioText = text.Split('\n');

                    //閉じる
                    sr.Close();
                    reader.Close();
                    sectionName = path;
                }

                //pngファイルの場合
                if (path.Substring(path.Length - 4) == ".png" || path.Substring(path.Length - 4) == ".PNG")
                {
                    //閲覧するZIPエントリのStreamを取得
                    Stream fs = zf.GetInputStream(ze);
                    buffer = ReadBinaryData(fs);//bufferにbyte[]になったファイルを読み込み

                    // 画像を取り出す
                    /*
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
                    */
                    //byteからTexture2D作成
                    Texture2D texture = new Texture2D(1,1);
                    texture.LoadImage(buffer);

                    // 読み込んだ画像からSpriteを作成する
                    scenarioGraphic[gNum] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    scenarioGraphicToPath[gNum] = path;
                    //閉じる
                    fs.Close();
                    gNum++;
                }

                //jpgファイルの場合
                if (path.Substring(path.Length - 4) == ".jpg" || path.Substring(path.Length - 4) == ".JPG" || path.Substring(path.Length - 5) == ".jpeg" || path.Substring(path.Length - 5) == ".JPEG")
                {
                    //閲覧するZIPエントリのStreamを取得
                    Stream fs = zf.GetInputStream(ze);
                    buffer = ReadBinaryData(fs);//bufferにbyte[]になったファイルを読み込み

                    // 画像を取り出す
                    /*
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
                    */
                    //byteからTexture2D作成
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(buffer);

                    // 読み込んだ画像からSpriteを作成する
                    scenarioGraphic[gNum] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    scenarioGraphicToPath[gNum] = path;
                    //閉じる
                    fs.Close();
                    gNum++;
                }

                //wavファイルの場合
                if (path.Substring(path.Length - 4) == ".wav" || path.Substring(path.Length - 4) == ".WAV")
                {
                    //閲覧するZIPエントリのStreamを取得
                    Stream fs = zf.GetInputStream(ze);
                    buffer = ReadBinaryData(fs);//bufferにbyte[]になったファイルを読み込み
                    scenarioAudio[sNum] = WavUtility.ToAudioClip(buffer);
                    //閉じる
                    fs.Close();
                    sNum++;
                }

                }
            else
            {
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
            }
        }
        catch
        {
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルの形式が不適合です。" + _FILE_HEADER + "\\" + path);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
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

    //画像サイズに合わせて立ち絵サイズを変更
    private void ObjSizeChangeToGraph(int position,Sprite sprite)
    {
        objCharacter[position].GetComponent<RectTransform>().sizeDelta=new Vector2(sprite.pixelsPerUnit * sprite.bounds.size.x, sprite.pixelsPerUnit * sprite.bounds.size.y);
    }

    //画像の100番にはPC立ち絵を。
    public IEnumerator LoadPlayerChara(string path)
    {
        // 指定したファイルをロードする
        WWW request = new WWW("file://" + path);

        while ((!request.isDone) || (!string.IsNullOrEmpty(request.error)))
        {
            yield return new WaitForEndOfFrame();
        }
        if (request.texture != null)
        {
            // 画像を取り出す
            Texture2D texture = request.texture;

            // 読み込んだ画像からSpriteを作成する
            scenarioGraphic[99] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }


    //画面が押されたかチェックするコルーチン
    public IEnumerator PushWait()
    {
        pushButton = false;
        while (true)//ブレークするまでループを続ける。
        {
            yield return null;
            if (pushButton==true || (skipFlag == true && skipFlag2 == true))
            {
                if (backLogCSFlag == false)
                {
                    sentenceEnd = true;
                    yield break;//falseならコルーチン脱出
                }
            }
        }
    }

    public void PushNextGo()
    {
        pushButton = true;
    }

    public void TitleBackButton()
    {
        backLogCSFlag = true;
        objTitleBack.SetActive(true);
    }

    public void TitleBackDecide()
    {
        int x;
        float y;
        for (int i = 0; i < flagname.Count; i++) { if (int.TryParse(flagvalue[i], out x)) { PlayerPrefs.SetInt(flagname[i], x); } else if (float.TryParse(flagvalue[i], out y)) { PlayerPrefs.SetFloat(flagname[i], y); } else { PlayerPrefs.SetString(flagname[i], flagvalue[i].Replace("[system]String", "")); } }
        PlayerPrefs.Save();
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
    }

    public void TitleBackCancel()
    {
        objTitleBack.SetActive(false);
        backLogCSFlag = false;
    }

    public void OnApplicationQuit()
    {
        int x;
        float y;
        for (int i=0;i<flagname.Count;i++){ if (int.TryParse(flagvalue[i], out x)) { PlayerPrefs.SetInt(flagname[i],x); } else if (float.TryParse(flagvalue[i], out y)) { PlayerPrefs.SetFloat(flagname[i],y); } else { PlayerPrefs.SetString(flagname[i], flagvalue[i].Replace("[system]String",""));  } }
        PlayerPrefs.Save();
    }
}