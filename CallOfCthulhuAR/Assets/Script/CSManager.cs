﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CSManager : MonoBehaviour {
    private int[] status = new int[STATUSNUM];
    private int[] skills = new int[SKILLNUM];
    private int buyPoint;
    private GameObject[] statusObj = new GameObject[STATUSNUM];
    private GameObject objSkillSheet1;
    private GameObject objSkillSheet2;
    private GameObject objSkillButton;
    private GameObject objBuyPoint;
    private GameObject objChara;
    private GameObject objCS;
    private GameObject[] objSkill = new GameObject[SKILLNUM];
    private bool skill;
    private int[] skillDefault = new int[SKILLNUM];
    private int nowHP;
    private int nowMP;
    private int nowSAN;
    private bool loadedChara = false;
    const int STATUSNUM = 12;
    const int SKILLNUM = 54;
    public GameObject inputBox;
    public GameObject selectButton;
    public GameObject decideText;
    public GameObject readButton;
    private string[] mapData;
    string _FILE_HEADER;
    List<GameObject> objFI = new List<GameObject>();
    public GameObject parentObject;
    public GameObject objFreeIvent;
    public GameObject objFIField;
    public GameObject objFIOpenButton;

    // Use this for initialization
    void Start() {
        objSkillSheet1 = GameObject.Find("SkillSheet1/2").gameObject as GameObject;
        objSkillSheet2 = GameObject.Find("SkillSheet2/2").gameObject as GameObject;
        objChara = GameObject.Find("CharacterImage").gameObject as GameObject;
        objSkillButton = GameObject.Find("SkillButton").gameObject as GameObject;
        if (SceneManager.GetActiveScene().name == "CharacterSheet")
        {
            objBuyPoint = GameObject.Find("BuyPoint").gameObject as GameObject;
        }
        for (int i = 0; i < STATUSNUM; i++)
        {
            statusObj[i] = GameObject.Find("statusObj" + i.ToString()).gameObject as GameObject;
        }
        for (int i = 0; i < SKILLNUM / 2; i++)
        {
            objSkill[i] = GameObject.Find("Skill" + (i + 1).ToString()).gameObject as GameObject;
        }
        for (int i = SKILLNUM / 2; i < SKILLNUM; i++)
        {
            objSkill[i] = GameObject.Find("2Skill" + (i - SKILLNUM / 2 + 1).ToString()).gameObject as GameObject;
        }
        objCS = GameObject.Find("CS") as GameObject;
        DefaultMake();
        SeeCharacter();
        if (SceneManager.GetActiveScene().name == "CharacterSheet")
        {
            if (Application.platform != RuntimePlatform.Android) { inputBox.GetComponent<Text>().raycastTarget = false; }
            GameObject.Find("InputField").GetComponent<InputField>().text = PlayerPrefs.GetString("[system]PlayerCharacterName", "");
            if (loadedChara == true)
            {
                decideText.GetComponent<Text>().text = "決定済";
            }
        }
        else
        {
            _FILE_HEADER = PlayerPrefs.GetString("[system]進行中シナリオ", "");
            GameObject.Find("PlayerCharacterName").GetComponent<Text>().text = PlayerPrefs.GetString("[system]PlayerCharacterName", "――");
            objCS.gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().name == "MapScene")
            {
                objFIOpenButton.SetActive(false);
                GetFreeIvent("[system]mapdata[system].txt");
                objFIField.SetActive(false);
            }           
        }
    }


    public void DefaultMake()
    {
        skill = false;
        StartCoroutine(LoadChara(PlayerPrefs.GetString("[system]CharacterIllstPath", "")));

        //技能初期値設定
        skillDefault[0] = 5; skillDefault[1] = 5; skillDefault[2] = 20; skillDefault[3] = 30;
        skillDefault[4] = 5; skillDefault[5] = status[2] * 2; skillDefault[6] = 1; skillDefault[7] = 1;
        skillDefault[8] = 15; skillDefault[9] = 10; skillDefault[10] = 20; skillDefault[11] = 25;
        skillDefault[12] = 5; skillDefault[13] = 10; skillDefault[14] = 1; skillDefault[15] = 1;
        skillDefault[16] = 10; skillDefault[17] = 10; skillDefault[18] = 1; skillDefault[19] = 5;
        skillDefault[20] = 15; skillDefault[21] = 5; skillDefault[22] = 1; skillDefault[23] = 25;
        skillDefault[24] = 5; skillDefault[25] = 1; skillDefault[26] = 1; skillDefault[27] = 15;
        skillDefault[28] = 1; skillDefault[29] = 1; skillDefault[30] = 25; skillDefault[31] = 10;
        skillDefault[32] = 10; skillDefault[33] = 1; skillDefault[34] = 1; skillDefault[35] = 25;
        skillDefault[36] = 40; skillDefault[37] = 25; skillDefault[38] = 10; skillDefault[39] = 5;
        skillDefault[40] = 10; skillDefault[41] = 1; skillDefault[42] = 1; skillDefault[43] = 5;
        skillDefault[44] = 1; skillDefault[45] = status[7] * 5; skillDefault[46] = 1; skillDefault[47] = 25;
        skillDefault[48] = 1; skillDefault[49] = 20; skillDefault[50] = 30; skillDefault[51] = 50;
        skillDefault[52] = 50; skillDefault[53] = 0;
    }


    public void SeeCharacter()
    {
        string[][] str = new string[STATUSNUM][];
        string[] str2;
        string str3 = "";
        for (int i = 0; i < STATUSNUM; i++)
        {
            status[i] = PlayerPrefs.GetInt("[system]Status" + i.ToString(), 0);
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            skills[i] = PlayerPrefs.GetInt("[system]Skill" + i.ToString(), 0);
        }
        nowHP = PlayerPrefs.GetInt("[system]耐久力", 0);
        nowMP = PlayerPrefs.GetInt("[system]マジック・ポイント", 0);
        nowSAN = PlayerPrefs.GetInt("[system]正気度ポイント", 0);
        for (int i = 0; i < STATUSNUM; i++)
        {
            str[i] = statusObj[i].GetComponent<Text>().text.Split(':');
            if (i == 8)
            {
                if (status[8] > 0) { str3 = "+1D"; }
                if (status[8] < 0) { str3 = "-1D"; }
            }
            if (i == 9) { str3 = nowHP.ToString() + "/"; }
            if (i == 10) { str3 = nowMP.ToString() + "/"; }
            if (i == 11) { str3 = nowSAN.ToString() + "/"; }
            statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + Mathf.Abs(status[i]);
            if (i == 11) { statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + (99 - skills[53]).ToString(); }
            str3 = "";
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            str2 = objSkill[i].GetComponent<Text>().text.Split('：');
            objSkill[i].GetComponent<Text>().text = str2[0] + '：' + skills[i].ToString();
        }
        //キャラがいなければ作成
        if (status[0] == 0) { MakeCharacter(); } else { loadedChara = true;  }
    }


    //キャラのステータス決定
    public void MakeCharacter()
    {
        loadedChara = false;
        Utility u1 = GetComponent<Utility>();
        string str3 = "";
        string[][] str = new string[STATUSNUM][];
        status[0] = u1.DiceRoll(3, 6);
        status[1] = u1.DiceRoll(3, 6);
        status[2] = u1.DiceRoll(3, 6);
        status[3] = u1.DiceRoll(2, 6) + 6;
        status[4] = u1.DiceRoll(3, 6);
        status[5] = u1.DiceRoll(3, 6);
        status[6] = u1.DiceRoll(2, 6) + 6;
        status[7] = u1.DiceRoll(3, 6) + 3;
        if (status[1] + status[6] < 13) { status[8] = -6; }
        else if (status[0] + status[6] < 17) { status[8] = -4; }
        else if (status[0] + status[6] < 25) { status[8] = 0; }
        else if (status[0] + status[6] < 33) { status[8] = 4; }
        else if (status[0] + status[6] < 41) { status[8] = 6; }
        status[9] = (status[1] + status[6]) / 2 + (status[1] + status[6]) % 2;
        status[10] = status[5];
        status[11] = status[5] * 5;
        nowHP = status[9];
        nowMP = status[10];
        nowSAN = status[11];
        for (int i = 0; i < STATUSNUM; i++)
        {
            str[i] = statusObj[i].GetComponent<Text>().text.Split(':');
            if (i == 8)
            {
                if (status[8] > 0) { str3 = "+1D"; }
                if (status[8] < 0) { str3 = "-1D"; }
            }
            if (i == 9) { str3 = nowHP.ToString() + "/"; }
            if (i == 10) { str3 = nowMP.ToString() + "/"; }
            if (i == 11) { str3 = nowSAN.ToString() + "/"; }
            statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + Mathf.Abs(status[i]);
            if (i == 11) { statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + (99 - skills[53]).ToString(); }
            str3 = "";
        }

        //技能初期値の入力
        skillDefault[5] = status[2] * 2; skillDefault[45] = status[7] * 5;
        SkillReset();
    }

    public void SkillReset()
    {
        string[] str;
        if (loadedChara == false)
        {
            for (int i = 0; i < SKILLNUM; i++)
            {
                skills[i] = skillDefault[i];
                str = objSkill[i].GetComponent<Text>().text.Split('：');
                objSkill[i].GetComponent<Text>().text = str[0] + '：' + skills[i].ToString();
            }
            buyPoint = status[3] * 10 + status[7] * 20;
            objBuyPoint.GetComponent<Text>().text = "残：" + buyPoint.ToString() + "P";
        }
        else
        {
            objBuyPoint.GetComponent<Text>().text = "決定済";
        }
    }

    private IEnumerator SkillMove()
    {
        if (skill == false)
        {
            skill = true;
            for (int i = 0; i <= 20; i++)
            {
                objSkillSheet2.GetComponent<RectTransform>().localPosition = new Vector3(800 - i * 40, 50, 0);
                yield return null;
            }
            if (SceneManager.GetActiveScene().name == "CharacterSheet")
            {
                if (loadedChara == false)
                {
                    objBuyPoint.GetComponent<Text>().text = "残：" + buyPoint.ToString() + "P";
                }
                else
                {
                    objBuyPoint.GetComponent<Text>().text = "決定済";
                }
            }
            yield break;
        }

        if (skill == true)
        {
            skill = false;
            for (int i = 0; i <= 20; i++)
            {
                objSkillSheet2.GetComponent<RectTransform>().localPosition = new Vector3(i * 40, 50, 0);
                yield return null;
            }
            yield break;
        }

    }

    public void SkillButton()
    {
        SkillButtonIn();
    }

    private void SkillButtonIn()
    {
        if (skill == false) { objSkillButton.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f); }
        if (skill == true) { objSkillButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f); }
        StartCoroutine(SkillMove());
    }

    public IEnumerator LoadChara(string path)
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
            objChara.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
            objChara.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void SkillChangeButton1()
    {
        StartCoroutine(SkillChange(1));
    }

    public void SkillChangeButton2()
    {
        StartCoroutine(SkillChange(2));
    }

    public IEnumerator SkillChange(int number)
    {
        if (number == 2)
        {
            for (int i = 0; i <= 20; i++)
            {
                objSkillSheet1.GetComponent<RectTransform>().localPosition = new Vector3(800 - i * 40, 0, 0);
                yield return null;
            }
            yield break;
        }

        if (number == 1)
        {
            for (int i = 0; i <= 20; i++)
            {
                objSkillSheet1.GetComponent<RectTransform>().localPosition = new Vector3(i * 40, 0, 0);
                yield return null;
            }
            yield break;
        }
    }

    public void SkillAdd(int number)
    {
        StartCoroutine("SkillAddCor", number);
    }

    public void SkillAddEnd(int number)
    {
        StopCoroutine("SkillAddCor");
    }

    public IEnumerator SkillAddCor(int number)
    {
        string[] str;
        if (loadedChara == false)
        {
            str = objSkill[number].GetComponent<Text>().text.Split('：');
            for (int i = 0; i < 2; i++)
            {
                skills[number]++; buyPoint--;
                if (skills[number] == 100 || buyPoint < 0) { buyPoint += (skills[number] - skillDefault[number]); skills[number] = skillDefault[number]; }
                objSkill[number].GetComponent<Text>().text = str[0] + '：' + skills[number].ToString();
                objBuyPoint.GetComponent<Text>().text = "残：" + buyPoint.ToString() + "P";
                yield return new WaitForSeconds(1.0f);
            }
            while (true)
            {
                skills[number]++; buyPoint--;
                if (skills[number] == 100 || buyPoint < 0) { buyPoint += (skills[number] - skillDefault[number]); skills[number] = skillDefault[number]; }
                objSkill[number].GetComponent<Text>().text = str[0] + '：' + skills[number].ToString();
                objBuyPoint.GetComponent<Text>().text = "残：" + buyPoint.ToString() + "P";
                yield return null; yield return null;
            }
        }
        else
        {
            objBuyPoint.GetComponent<Text>().text = "決定済";
        }
    }

    public void PushDecideButton()
    {
        PushDecideButtonIn();
    }

    private void PushDecideButtonIn()
    {
        for (int i = 0; i < STATUSNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Status" + i.ToString(), status[i]);
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            PlayerPrefs.SetInt("[system]Skill" + i.ToString(), skills[i]);
        }
        PlayerPrefs.SetInt("[system]耐久力", nowHP);
        PlayerPrefs.SetInt("[system]マジック・ポイント", nowMP);
        PlayerPrefs.SetInt("[system]正気度ポイント", nowSAN);
        PlayerPrefs.SetString("[system]PlayerCharacterName", GameObject.Find("PlayerCharacterName").GetComponent<Text>().text);
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
    }

    public void PushRediceButton()
    {
        PushRediceButtonIn();
    }

    private void PushRediceButtonIn()
    {
        decideText.GetComponent<Text>().text = "";
        int adnum;
        adnum=Random.Range(0,5);
        if (adnum == 0) { Application.OpenURL("https://www.melonbooks.com/index.php?main_page=maker_affiliate_go&affiliate_id=AF0000028251"); }
        if (adnum == 1) { Application.OpenURL("https://www.melonbooks.com/index.php?main_page=maker_affiliate_go&affiliate_id=AF0000027414"); }
        if (adnum == 2) { Application.OpenURL("https://www.melonbooks.com/index.php?main_page=maker_affiliate_go&affiliate_id=AF0000028253"); }
        if (adnum == 3) { Application.OpenURL("https://www.melonbooks.com/index.php?main_page=maker_affiliate_go&affiliate_id=AF0000029264"); }
        if (adnum == 4) { Application.OpenURL("https://www.amazon.co.jp/gp/product/4047294640/ref=as_li_tl?ie=UTF8&camp=247&creative=1211&creativeASIN=4047294640&linkCode=as2&tag=permanentpapy-22&linkId=44241ba025c599790c39a2ee2d7a3774"); }
        for (int i = 0; i < SKILLNUM; i++)
        {
            skills[i] = 0;
        }
        MakeCharacter();
    }

    public void CSButton()
    {
        CSButtonIn();
    }

    private void CSButtonIn()
    {
        SeeCharacter();
        if (objCS.activeSelf == false)
        {
            if (SceneManager.GetActiveScene().name == "NovelScene")
            {

                GameObject.Find("NovelManager").gameObject.GetComponent<ScenariosceneManager>().backLogCSFlag = true;
            }
            else if(SceneManager.GetActiveScene().name == "MapScene")
            {
                objFIOpenButton.SetActive(true);
            }
            objCS.gameObject.SetActive(true);
            transform.GetChild(0).gameObject.GetComponent<Text>().text = "戻る";
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "NovelScene")
            {
                GameObject.Find("NovelManager").gameObject.GetComponent<ScenariosceneManager>().backLogCSFlag = false;
               
            }
            else if (SceneManager.GetActiveScene().name == "MapScene")
            {
                objFIOpenButton.SetActive(false);
                objFIField.SetActive(false);
            }
            objCS.gameObject.SetActive(false);
            transform.GetChild(0).gameObject.GetComponent<Text>().text = "Character\nsheet";
        }
    }

    public void CharacterIllustButton()
    {
        selectButton.SetActive(false);readButton.SetActive(false);
        GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]CharacterIllstPath");
    }

    public void PushReadButton()
    {
        string path;
        selectButton.SetActive(false); readButton.SetActive(false);
        GetComponent<GracesGames.SimpleFileBrowser.Scripts.FileOpenManager>().GetFilePathWithKey("[system]CharacterSheet");
        path = PlayerPrefs.GetString("[system]CharacterSheet", "");
        if (path != "")
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(path);
                //内容をすべて読み込む
                string text = sr.ReadToEnd();
                //閉じる
                sr.Close();
                string[] texts = text.Split('\n');
                int j = 0;
                for (int i = 0; i < STATUSNUM; i++)
                {
                    int x;
                    if (int.TryParse(texts[j], out x)) { status[i] = x; } else { status[i] = 3; }
                    j++;
                }
                for (int i = 0; i < SKILLNUM; i++)
                {
                    int x;
                    if (int.TryParse(texts[j], out x)) { skills[i] = x; } else { skills[i] = 0; }
                    j++;
                }
                int y;
                if (int.TryParse(texts[j], out y)) { nowHP = y; } else { nowHP = 3; }
                j++;
                if (int.TryParse(texts[j], out y)) { nowMP = y; } else { nowMP = 1; }
                j++;
                if (int.TryParse(texts[j], out y)) { nowSAN = y; } else { nowSAN = 1; }
                j++;
                GameObject.Find("PlayerCharacterName").GetComponent<Text>().text = texts[j];
                GameObject.Find("InputField").GetComponent<InputField>().text = texts[j];
                j++;
                PlayerPrefs.SetString("[system]CharacterIllstPath", texts[j]);
                StartCoroutine(LoadChara(PlayerPrefs.GetString("[system]CharacterIllstPath", "")));

                //数字を表示に適用。
                string[][] str = new string[STATUSNUM][];
                string[] str2;
                string str3 = "";
                for (int i = 0; i < STATUSNUM; i++)
                {
                    str[i] = statusObj[i].GetComponent<Text>().text.Split(':');
                    if (i == 8)
                    {
                        if (status[8] > 0) { str3 = "+1D"; }
                        if (status[8] < 0) { str3 = "-1D"; }
                    }
                    if (i == 9) { str3 = nowHP.ToString() + "/"; }
                    if (i == 10) { str3 = nowMP.ToString() + "/"; }
                    if (i == 11) { str3 = nowSAN.ToString() + "/"; }
                    statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + Mathf.Abs(status[i]);
                    if (i == 11) { statusObj[i].GetComponent<Text>().text = str[i][0] + ':' + str3 + (99 - skills[53]).ToString(); }
                    str3 = "";
                }
                for (int i = 0; i < SKILLNUM; i++)
                {
                    str2 = objSkill[i].GetComponent<Text>().text.Split('：');
                    objSkill[i].GetComponent<Text>().text = str2[0] + '：' + skills[i].ToString();
                }
                loadedChara = true;
                decideText.GetComponent<Text>().text = "決定済";
            }
            catch { }
        }
    }

    public void PushWriteButton()
    {
        string tmp="";
        string filename = "";
        for (int i = 0; i < STATUSNUM; i++)
        {
            tmp+= status[i] + "\n";
        }
        for (int i = 0; i < SKILLNUM; i++)
        {
            tmp+=skills[i] + "\n";
        }
        tmp+=nowHP+"\n";
        tmp+=nowMP+"\n";
        tmp+=nowSAN+"\n";
        if (GameObject.Find("PlayerCharacterName").GetComponent<Text>().text == "") { filename = "未決定"; } else { filename = GameObject.Find("PlayerCharacterName").GetComponent<Text>().text; }
        tmp +=filename + "\n";
        tmp+=PlayerPrefs.GetString("[system]CharacterIllstPath", "");
        try
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/" + filename + ".ccs", false);
            //TextBox1.Textの内容を書き込む
            sw.Write(tmp);
            //閉じる
            sw.Close();
        }
        catch { }
    }

    //startのタイミングでフリーイベントの一覧表を取得し、条件を満たしているものはボタンを作成（Mapシーン限定）
    private void GetFreeIvent(string path)
    {
        try
        {
            //閲覧するエントリ
            string extractFile = path;

            //ZipFileオブジェクトの作成
            ICSharpCode.SharpZipLib.Zip.ZipFile zf =
                new ICSharpCode.SharpZipLib.Zip.ZipFile(PlayerPrefs.GetString("[system]進行中シナリオ", ""));
            zf.Password = Secret.SecretString.zipPass;
            //展開するエントリを探す
            ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zf.GetEntry(extractFile);

            if (ze != null)
            {
                //閲覧するZIPエントリのStreamを取得
                System.IO.Stream reader = zf.GetInputStream(ze);
                //文字コードを指定してStreamReaderを作成
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    reader, System.Text.Encoding.GetEncoding("UTF-8"));
                // テキストを取り出す
                string text = sr.ReadToEnd();
                text = text.Replace("[system]任意イベント", "[system]任意イベントCS");
                // 読み込んだ目次テキストファイルからstring配列を作成する
                mapData = text.Split('\n');
                //閉じる
                sr.Close();
                reader.Close();
            }
            else
            {
                SceneManager.LoadScene("TitleScene");
            }
            //閉じる
            zf.Close();
        }
        catch
        {
        }
    }


    public void PushFreeIventButton()
    {
        if (objFIField.activeSelf == true)
        {
            objFIField.SetActive(false);
        }
        else
        {
            objFIField.SetActive(true);
            PlayerPrefs.SetInt("[system]任意イベントCS", 1);
            System.DateTime dt;
            dt = System.DateTime.UtcNow;
            dt = dt.AddHours(9);//アンドロイドがローカル時間周りで妙な動きをするので、UTCで出してからJSTに変換してやる。
            double latitude = GameObject.Find("MapSceneManager").GetComponent<MapScene>().latitude;
            double longitude = GameObject.Find("MapSceneManager").GetComponent<MapScene>().longitude;
            for (int i = 0; i < objFI.Count; i++) { Destroy(objFI[i]); }
            objFI.Clear();
            int k = 0;
            for (int i = 0; i < mapData.Length; i++)
            {
                bool tempBool = false;
                string[] dataFlag;
                string[] data;
                if (mapData[i] == "[END]") { break; }
                data = mapData[i].Replace("\r", "").Replace("\n", "").Split(',');
                dataFlag = data[10].Replace("　", " ").Split(' ');
                for (int j = 0; j < dataFlag.Length; j++) { if (dataFlag[j] != "" && PlayerPrefs.GetInt(dataFlag[j], 0) <= 0) { tempBool = true; } }
                if (data[5] != "" && data[9] != "" && int.Parse(data[9]) < int.Parse(data[5])) { if (dt.Minute >= int.Parse(data[5])) { data[9] = (int.Parse(data[9]) + 60).ToString(); if (data[8] != "") { data[8] = (int.Parse(data[8]) - 1).ToString(); } } else { data[5] = (int.Parse(data[5]) - 60).ToString(); if (data[4] != "") { data[4] = (int.Parse(data[4]) + 1).ToString(); } } }
                if (data[4] != "" && data[8] != "" && int.Parse(data[8]) < int.Parse(data[4])) { if (dt.Hour >= int.Parse(data[4])) { data[8] = (int.Parse(data[8]) + 24).ToString(); if (data[7] != "") { data[7] = (int.Parse(data[7]) - 1).ToString(); } } else { data[4] = (int.Parse(data[4]) - 24).ToString(); if (data[3] != "") { data[3] = (int.Parse(data[3]) + 1).ToString(); } } }
                if (data[3] != "" && data[7] != "" && int.Parse(data[7]) < int.Parse(data[3])) { if (dt.Day >= int.Parse(data[3])) { data[7] = (int.Parse(data[7]) + 31).ToString(); if (data[6] != "") { data[6] = (int.Parse(data[6]) - 1).ToString(); } } else { data[3] = (int.Parse(data[3]) - 31).ToString(); if (data[4] != "") { data[2] = (int.Parse(data[2]) + 1).ToString(); } } }
                if (data[2] != "" && data[6] != "" && int.Parse(data[6]) < int.Parse(data[2])) { if (dt.Month >= int.Parse(data[2])) { data[6] = (int.Parse(data[6]) + 12).ToString(); } else { data[2] = (int.Parse(data[2]) - 12).ToString(); } }

                if ((data[0] == "" || double.Parse(data[0]) > latitude - 0.0005 && double.Parse(data[0]) < latitude + 0.0005) &&
                    (data[1] == "" || double.Parse(data[1]) > longitude - 0.0005 && double.Parse(data[1]) < longitude + 0.0005) &&
                    (data[2] == "" || (int.Parse(data[2]) <= dt.Month)) &&
                    (data[3] == "" || (int.Parse(data[3]) <= dt.Day)) &&
                    (data[4] == "" || (int.Parse(data[4]) <= dt.Hour)) &&
                    (data[5] == "" || (int.Parse(data[5]) <= dt.Minute)) &&
                    (data[6] == "" || (int.Parse(data[6]) >= dt.Month)) &&
                    (data[7] == "" || (int.Parse(data[7]) >= dt.Day)) &&
                    (data[8] == "" || (int.Parse(data[8]) >= dt.Hour)) &&
                    (data[9] == "" || (int.Parse(data[9]) >= dt.Minute)) &&
                    (tempBool == false) &&
                    (PlayerPrefs.GetInt(data[11].Substring(0, data[11].Length - 4) + "Flag", 0) == 0))
                {
                    //ボタン追加
                        string iventName = data[11].Substring(0, data[11].Length - 4);
                        objFI.Add(Instantiate(objFreeIvent) as GameObject);
                        objFI[k].transform.SetParent(parentObject.transform, false);
                        objFI[k].GetComponentInChildren<Text>().text = iventName;
                        objFI[k].GetComponent<FIButton>().buttonName = iventName + ".txt";
                        objFI[k].GetComponent<FIButton>().latitude = latitude;
                        objFI[k].GetComponent<FIButton>().longitude = longitude;
                    k++;
                }
            }
        }
    }



    // Update is called once per frame
    void Update () {
		
	}
}
