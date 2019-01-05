using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapScene : MonoBehaviour
{
    Sprite mapImage;
    private int width = 640;
    private int height = 640;
    private double longitude;
    private double latitude;
    private double longitudeMap;
    private double latitudeMap;
    private int zoom = 16;
    private float targetX=0;
    private float targetY=0;
    private string[] mapData;
    private bool sceneChange = false;
    private bool mapLoad = false;
    public GameObject mapImageObj;
    GameObject obj;
    public GameObject objTarget;
    GameObject objBGM;
    GameObject objTime;
    List<GameObject> objSP = new List<GameObject>();
    public GameObject objPreSP;
    public GameObject parentObject;
    string _FILE_HEADER;
    private bool moveStop = false;
    private bool imageChange=false;
    private double zoomPow = 1;
    private int VMode = 0;
    public GameObject VStick;
    FixedJoystick stick;
    private bool zoomNow = false;

    void Start()
    {
        PlayerPrefs.Save();
        zoom = PlayerPrefs.GetInt("[system]Zoom", 16);
        VMode = PlayerPrefs.GetInt("[system]VMode",0);
        if (VMode == 0) { VStick.SetActive(false); } else { stick = VStick.GetComponent<FixedJoystick>(); }
        zoomPow = Math.Pow(2, zoom);
        GetComponent<Utility>().BGMFadeIn(2);
        if ((VMode == 0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        { GetComponent<Utility>().BGMStop(); }
        else
        { GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("MapBGM")); }
        _FILE_HEADER = PlayerPrefs.GetString("[system]進行中シナリオ","");                      //ファイル場所の頭
        if (_FILE_HEADER==null || _FILE_HEADER == "") { GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene"); }
        longitude=PlayerPrefs.GetFloat("[system]longitude",135.768738f); latitude = PlayerPrefs.GetFloat("[system]latitude", 35.010348f);
        obj = GameObject.Find("error").gameObject as GameObject;
        objBGM= GameObject.Find("BGMManager").gameObject as GameObject;
        objTime = GameObject.Find("TimeText").gameObject as GameObject;
        LoadMapData("[system]mapdata[system].txt");
        if ((VMode==0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) { Input.location.Start(); }
        GetPos();
        GetMap();
    }

    void Update()
    {
        if (moveStop == false) { GetPos(); }
        if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) || Input.location.status == LocationServiceStatus.Running || VMode>0) { if (mapLoad==true) { IventCheck(); } }
        objBGM.GetComponent<Text>().text = longitude.ToString() + "," + latitude.ToString();
        //地図の更新は、マップ範囲から出た時かつ時間が相当経過している時に。（時間変数入れないと、場所によってはGPS誤差でマップ連続読込になりかねない）
            if (((longitude>longitudeMap+196/zoomPow) ||
            (longitude < longitudeMap - 196/ zoomPow) ||
            (latitude < latitudeMap - 262/zoomPow) ||
            (latitude > latitudeMap + 262/zoomPow)))
        {
            GetMap();
        }
    }


    //マップデータテキストを読み込んで、現在位置と時刻が一致するイベントがあれば読込。
    void IventCheck()
    {
        DateTime dt;
        dt = DateTime.UtcNow;
        dt=dt.AddHours(9);//アンドロイドがローカル時間周りで妙な動きをするので、UTCで出してからJSTに変換してやる。

        for (int i = 0; i < mapData.Length; i++)
        {
            if (sceneChange == false)
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
                    objBGM.GetComponent<BGMManager>().chapterName = data[11];
                    objTime.GetComponent<Text>().text = "<color=red>[★イベント発生]</color>";
                    sceneChange = true;
                    if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && (!Input.location.isEnabledByUser)) { Input.location.Stop(); }
                    PlayerPrefs.SetFloat("[system]longitude", (float)longitude - 0.0011f); PlayerPrefs.SetFloat("[system]latitude", (float)latitude);
                    GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "NovelScene");
                    return;
                }
            }
        }
        if (sceneChange == false) { objTime.GetComponent<Text>().text = dt.ToString("MM/dd  HH:mm") + "\n" + "<size=48>緯度：" + Math.Round(latitude, 4).ToString() + "　,　経度：" + Math.Round(longitude, 4).ToString().ToString() + "</size>"; }
    }

    void GetPos()
    {
        //★①スマートフォン版（ＧＰＳから位置情報を拾う）
        if((VMode==0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        {
        //GPSで取得した緯度経度を変数に代入
        StartCoroutine(GetGPS());
        }
        //★②PC版（キー入力で動かす）
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
                if (Input.GetKey(KeyCode.DownArrow)) { latitude -= 0.65536 / zoomPow; }
                if (Input.GetKey(KeyCode.UpArrow)) { latitude += 0.65536 / zoomPow; }
                if (Input.GetKey(KeyCode.LeftArrow)) { longitude -= 0.65536 / zoomPow; }
                if (Input.GetKey(KeyCode.RightArrow)) { longitude += 0.65536 / zoomPow; }
        }
        //★③机上プレイ（バーチャルスティックから）
        if (VMode > 0)
        {
                longitude += stick.Horizontal * 0.65536 / zoomPow;
                latitude += stick.Vertical * 0.65536 / zoomPow;      
        }
        targetX = (float)((longitude - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
        targetY = (float)((latitude - latitudeMap) * 2.05993652344 * zoomPow);
        objTarget.GetComponent<RectTransform>().localPosition = new Vector3(targetX, targetY, 0);
    }

    void GetMap()
    {
        //マップを取得
        StartCoroutine(GetStreetViewImage(latitude, longitude, zoom));
    }

    private IEnumerator GetGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            obj.GetComponent<Text>().text = ("エラー。位置情報の使用が許可されていません。スマホの「設定」にて位置情報機能の使用を許可してください。");
            yield break;
        }
            int maxWait = 120;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            obj.GetComponent<Text>().text = ("エラー。タイムアウトが発生しました。");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            obj.GetComponent<Text>().text = ("エラー。何らかの理由で位置情報が使用できません。");
            yield break;
        }
        else if(Input.location.status == LocationServiceStatus.Running)
        {
            longitude = Input.location.lastData.longitude;
            latitude = Input.location.lastData.latitude;
        }
    }

    public void ZoomUpButton()
    {
        if (imageChange==true || zoom >= 21) { return; }
        if (zoomNow == true) { return; }
        zoomNow = true;
        zoom++;
        PlayerPrefs.SetInt("[system]Zoom",zoom);
        zoomPow = Math.Pow(2, zoom);
        StartCoroutine(ZoomEffect(true));
    }
    public void ZoomDownButton()
    {
        if (imageChange==true || zoom <= 10) { return; }
        if (zoomNow == true) { return; }
        zoom--;
        PlayerPrefs.SetInt("[system]Zoom", zoom);
        zoomNow = true;
        zoomPow = Math.Pow(2, zoom);
        StartCoroutine(ZoomEffect(false));
    }

    private IEnumerator ZoomEffect(bool UPFlag)
    {
        if (UPFlag)
        {
            for (int i = 1500; i < 3000; i += 20)
            {
                mapImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(i, i);
                yield return null;
            }
        }
        else
        {
            for (int i = 1500; i > 750; i-=10)
            {
                mapImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(i, i);
                yield return null;
            }
        }
        imageChange = false;
        StartCoroutine(GetStreetViewImage(latitude, longitude, zoom));
        while (!imageChange) { yield return null; }
        mapImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1500, 1500);
        zoomNow = false;
    }

    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom)
    {
        string url="";
        moveStop = true;
        //地図の中心の緯度経度を保存
        longitudeMap = longitude;
        latitudeMap = latitude;
        if (Application.platform == RuntimePlatform.IPhonePlayer) { url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + Secret.SecretString.iPhoneKey; }
        if (Application.platform == RuntimePlatform.Android) { url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + Secret.SecretString.androidKey; }
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) { url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + Secret.SecretString.androidKey; ; }
        WWW www = new WWW(url);
        yield return www;
        //マップの画像をTextureからspriteに変換して貼り付ける
        mapImage = Sprite.Create(www.texture, new Rect(0, 0, 640, 640), Vector2.zero);
        mapImageObj.GetComponent<Image>().sprite=mapImage;
        //targetの位置を中心に
        targetX = 0; targetY = 0;
        moveStop = false;
        imageChange = true;
        SpotMake();
        yield return null;
        imageChange = false;
    }

    private void SpotMake()
    {
        for (int i = 0; i < objSP.Count; i++) { Destroy(objSP[i]); }
        objSP.Clear();
        float x;
        float y;
        for (int i = 0; i < mapData.Length; i++)
        {
            bool tempBool = false;
            string[] dataFlag;
            string[] data;
            if (mapData[i] == "[END]") { break; }
            data = mapData[i].Replace("\r", "").Replace("\n", "").Split(',');
            dataFlag = data[10].Replace("　", " ").Split(' ');
            for (int j = 0; j < dataFlag.Length; j++) { if (dataFlag[j] != "" && PlayerPrefs.GetInt(dataFlag[j], 0) <= 0) { tempBool = true; } }
            if ((tempBool == false) &&(PlayerPrefs.GetInt(data[11].Substring(0, data[11].Length - 4) + "Flag", 0) == 0))
            {
                objSP.Insert(0, Instantiate(objPreSP) as GameObject);
                objSP[0].transform.SetParent(parentObject.transform, false);
                    x = (float)((float.Parse(data[1]) - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
                    y = (float)((float.Parse(data[0]) - latitudeMap) * 2.05993652344 * zoomPow);
                objSP[0].GetComponent<RectTransform>().localPosition = new Vector3(x,y,0);
                objSP[0].GetComponent<RectTransform>().sizeDelta = new Vector2((float)(0.0015258789 * zoomPow), (float)(0.0015258789 * zoomPow));
            }
        }
    }

    //目次ファイルを読み込み、進行度に合わせてファイルを拾ってくる。
    private void LoadMapData(string path)
    {
        try
        {
        //閲覧するエントリ
        string extractFile = path;

        //ZipFileオブジェクトの作成
        ICSharpCode.SharpZipLib.Zip.ZipFile zf =
            new ICSharpCode.SharpZipLib.Zip.ZipFile(PlayerPrefs.GetString("[system]進行中シナリオ",""));
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

            // 読み込んだ目次テキストファイルからstring配列を作成する
            mapData = text.Split('\n');
            //閉じる
            sr.Close();
            reader.Close();
            mapLoad = true;
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
            obj.GetComponent<Text>().text = ("エラー。シナリオファイルの形式が不適合です。" + _FILE_HEADER + "\\" + path);
            SceneManager.LoadScene("TitleScene");
        }
    }
}


