using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapScene : MonoBehaviour
{
    Sprite mapImage;
    private int width = 1000;
    private int height = 1000;
    public float longitude;
    public float latitude;
    private double longitudeMap;
    private double latitudeMap;
    private int zoom = 16;
    private float targetX = 0;
    private float targetY = 0;
    private string[] mapData;
    public bool sceneChange = false;
    private bool mapLoad = false;
    public GameObject mapImageObj;
    public GameObject target;
    GameObject obj;
    GameObject objBGM;
    GameObject objTime;
    List<GameObject> objSP = new List<GameObject>();
    public GameObject objPreSP;
    public GameObject parentObject;
    string _FILE_HEADER;
    private double zoomPow = 1;
    private int VMode = 0;
    public GameObject VStick;
    public GameObject VStext;
    FixedJoystick stick;
    private bool zoomNow = false;
    public GameObject objErrorBack;
    private float beforeLatitude;//イベント直前に保存する用
    private float beforeLongitude;
    private bool getmapflag = false;

    public GameObject objTitleBack;

    void Start()
    {
        PlayerPrefs.Save();
        zoom = PlayerPrefs.GetInt("[system]Zoom", 16);
        VMode = PlayerPrefs.GetInt("[system]VMode", 0);
        if (VMode == 0) { VStick.SetActive(false); VStext.SetActive(true); } else { VStext.SetActive(false); stick = VStick.GetComponent<FixedJoystick>(); }
        zoomPow = Math.Pow(2, zoom)*0.4266666666;
        GetComponent<Utility>().BGMFadeIn(2);
        if ((VMode == 0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        { GetComponent<Utility>().BGMStop(); }
        else
        { GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("MapBGM")); }
        _FILE_HEADER = PlayerPrefs.GetString("[system]進行中シナリオ", "");                      //ファイル場所の頭
        if (_FILE_HEADER == null || _FILE_HEADER == "") { GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene"); }
        longitude = PlayerPrefs.GetFloat("[system]longitude", 135.768738f); latitude = PlayerPrefs.GetFloat("[system]latitude", 35.010348f);
        obj = GameObject.Find("error").gameObject as GameObject;
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        objTime = GameObject.Find("TimeText").gameObject as GameObject;
        LoadMapData("[system]mapdata[system].txt");
        if ((VMode == 0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) { Input.location.Start(); }
        beforeLongitude = longitude;
        beforeLatitude = latitude;
        longitudeMap = longitude;
        latitudeMap = latitude;
        GetPos();
        if (objBGM.GetComponent<BGMManager>().map!=null)
        {
            latitudeMap = objBGM.GetComponent<BGMManager>().latitudeMap;
            longitudeMap = objBGM.GetComponent<BGMManager>().longitudeMap;
            mapImage= objBGM.GetComponent<BGMManager>().map;
            mapImageObj.GetComponent<Image>().sprite = mapImage;
            target.GetComponent<RectTransform>().localPosition=new Vector2(0,0);
            targetX = (float)((longitude - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
            targetY = (float)((latitude - latitudeMap) * 2.05993652344 * zoomPow);
            mapImageObj.GetComponent<RectTransform>().localPosition = new Vector2(-targetX, -targetY);
            StartCoroutine(SpotMakeCo());
        }
        else
        {
            GetMap();
        }
    }

    void Update()
    {
        if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) || Input.location.status == LocationServiceStatus.Running || VMode > 0)
        {
            if (mapLoad == true)
            {
                if (!zoomNow) { GetPos(); }
                 IventCheck();
            }
        }
        objBGM.GetComponent<Text>().text = longitude.ToString() + "," + latitude.ToString();
        //地図の更新は、マップ範囲から出た時かつ時間が相当経過している時に。（時間変数入れないと、場所によってはGPS誤差でマップ連続読込になりかねない）
        if (((longitude > longitudeMap + 750 / zoomPow) ||
        (longitude < longitudeMap - 750 / zoomPow) ||
        (latitude < latitudeMap - 600 / zoomPow) ||
        (latitude > latitudeMap + 600 / zoomPow)))
        {
            GetMap();
        }
    }



    //マップデータテキストを読み込んで、現在位置と時刻が一致するイベントがあれば読込。
    void IventCheck()
    {
        DateTime dt;
        dt = DateTime.UtcNow;
        dt = dt.AddHours(9);//アンドロイドがローカル時間周りで妙な動きをするので、UTCで出してからJSTに変換してやる。

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
                    objBGM.GetComponent<BGMManager>().latitudeMap= latitudeMap;
                    objBGM.GetComponent<BGMManager>().longitudeMap= longitudeMap;
                    objBGM.GetComponent<BGMManager>().map = mapImage;
                    objBGM.GetComponent<BGMManager>().chapterName = data[11];
                    objTime.GetComponent<Text>().text = "　　　<color=red>[★イベント発生]</color>";
                    sceneChange = true;
                    if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && (!Input.location.isEnabledByUser)) { Input.location.Stop(); }
                    PlayerPrefs.SetFloat("[system]longitude", (float)beforeLongitude); PlayerPrefs.SetFloat("[system]latitude", (float)beforeLatitude);
                    GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "NovelScene");
                    return;
                }
            }
        }
        beforeLatitude = latitude;
        beforeLongitude = longitude;
        if (sceneChange == false) { objTime.GetComponent<Text>().text = "　　　" + dt.ToString("MM/dd  HH:mm") + "\n" + "<size=48>緯度：" + Math.Round(latitude, 4).ToString() + "　,　経度：" + Math.Round(longitude, 4).ToString().ToString() + "</size>"; }
    }

    void GetPos()
    {
        //★①スマートフォン版（ＧＰＳから位置情報を拾う）
        if ((VMode == 0) && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        {
            //GPSで取得した緯度経度を変数に代入
            StartCoroutine(GetGPS());
        }
        //★②PC版（キー入力で動かす）
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (Input.GetKey(KeyCode.DownArrow)) { latitude -= (float)(0.65536 / zoomPow); }
            if (Input.GetKey(KeyCode.UpArrow)) { latitude += (float)(0.65536 / zoomPow); }
            if (Input.GetKey(KeyCode.LeftArrow)) { longitude -= (float)(0.65536 / zoomPow); }
            if (Input.GetKey(KeyCode.RightArrow)) { longitude += (float)(0.65536 / zoomPow); }
        }
        //★③机上プレイ（バーチャルスティックから）
        if (VMode > 0)
        {
            longitude += (float)(stick.Horizontal * 0.65536 / zoomPow);
            latitude += (float)(stick.Vertical * 0.65536 / zoomPow);
        }
        targetX = (float)((longitude - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
        targetY = (float)((latitude - latitudeMap) * 2.05993652344 * zoomPow);
        mapImageObj.GetComponent<RectTransform>().localPosition = new Vector3(-targetX, -targetY, 0);
    }

    void GetMap()
    {
        //マップを取得
        if (getmapflag == false)
        {
            getmapflag = true;
            StartCoroutine(GetStreetViewImage(latitude, longitude, zoom));
        }
    }

    private IEnumerator GetGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            obj.GetComponent<Text>().text = ("[エラー]\n位置情報の使用が許可されていません");
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
            obj.GetComponent<Text>().text = ("[エラー]\nタイムアウトが発生しました");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            obj.GetComponent<Text>().text = ("[エラー]\n位置情報が使用できません");
            yield break;
        }
        else if (Input.location.status == LocationServiceStatus.Running)
        {
            longitude = Input.location.lastData.longitude;
            latitude = Input.location.lastData.latitude;
        }
    }

    private IEnumerator ZoomWait(bool UPFlag)
    {
        //while (getmapflag) { yield return null; }
        if (UPFlag == true)
        {
            ZoomUpButton();
        }
        else
        {
            ZoomDownButton();
        }
        yield return null;
    }

    public void ZoomUpButton()
    {
        if (zoom >= 21) { return; }
        if (zoomNow == true) { return; }
        //if (getmapflag == true) { StartCoroutine(ZoomWait(true)); return; }
        zoomNow = true;
        zoom++;
        PlayerPrefs.SetInt("[system]Zoom", zoom);
        zoomPow = Math.Pow(2, zoom) * 0.4266666666;
        for (int i = 0; i < objSP.Count; i++) { Destroy(objSP[i]); }
        objSP.Clear();
        StartCoroutine(ZoomEffect(true));
    }
    public void ZoomDownButton()
    {
        if (zoom <= 10) { return; }
        if (zoomNow == true) { return; }
        //if (getmapflag == true) { StartCoroutine(ZoomWait(false)); return; }
        zoom--;
        PlayerPrefs.SetInt("[system]Zoom", zoom);
        zoomNow = true;
        zoomPow = Math.Pow(2, zoom) * 0.4266666666;
        for (int i = 0; i < objSP.Count; i++) { Destroy(objSP[i]); }
        objSP.Clear();
        StartCoroutine(ZoomEffect(false));
    }

    private IEnumerator ZoomEffect(bool UPFlag)
    {
        for (int i = 0; i < objSP.Count; i++) { Destroy(objSP[i]); }
        objSP.Clear();
        getmapflag = true;
        StartCoroutine(GetStreetViewImage(latitude, longitude, zoom));
        if (UPFlag)
        {
            for (int i = 4000; i < 8000; i += 40)
            {
                mapImageObj.GetComponent<RectTransform>().localScale = new Vector2((float)i/4000, (float)i/4000);
                mapImageObj.GetComponent<RectTransform>().localPosition = new Vector2(-targetX*i/4000, -targetY*i/4000);
                yield return null;
            }
        }
        else
        {
            for (int i = 4000; i > 2000; i -= 20)
            {
                mapImageObj.GetComponent<RectTransform>().localScale = new Vector2((float)i/4000, (float)i/4000);
                mapImageObj.GetComponent<RectTransform>().localPosition = new Vector2(-targetX * i / 4000, -targetY * i / 4000);
                yield return null;
            }
        }
        while (getmapflag) { yield return null; }
        mapImageObj.GetComponent<RectTransform>().localScale = new Vector2(1,1);
        zoomNow = false;
    }

    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom)
    {
        string url = "";
        //map中心の確定
        double longitudeM = longitude;
        double latitudeM = latitude;
        //地図の中心の緯度経度を保存
        yield return null;yield return null;
        while (sceneChange) { yield return null; }
#if UNITY_IOS
        url = "https://map.yahooapis.jp/map/V1/static?" + Secret.SecretString.iPhoneKey + "&lat=" + latitudeM.ToString() + "&lon=" + longitudeM.ToString() + "&z=" + ((int)zoom-1).ToString() + "&width=" + width.ToString() + "&height=" + height.ToString();
#else
        url = "https://map.yahooapis.jp/map/V1/static?" + Secret.SecretString.androidKey + "&lat=" + latitudeM.ToString() + "&lon=" + longitudeM.ToString() + "&z=" + ((int)zoom-1).ToString() + "&width=" + width.ToString() + "&height=" + height.ToString();
#endif
        WWW www = new WWW(url);
        yield return www;
        //マップの画像をTextureからspriteに変換して貼り付ける
        mapImage = Sprite.Create(www.texture, new Rect(0, 0, width, height), Vector2.zero);
        getmapflag = false;
        while (zoomNow) { yield return null; }
        mapImageObj.GetComponent<Image>().sprite = mapImage;
        target.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        longitudeMap = longitudeM;
        latitudeMap = latitudeM;
        targetX = (float)((longitude - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
        targetY = (float)((latitude - latitudeMap) * 2.05993652344 * zoomPow);
        mapImageObj.GetComponent<RectTransform>().localPosition = new Vector2(-targetX,-targetY);
        SpotMake();
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
            if ((tempBool == false) && (PlayerPrefs.GetInt(data[11].Substring(0, data[11].Length - 4) + "Flag", 0) == 0))
            {
                objSP.Insert(0, Instantiate(objPreSP) as GameObject);
                objSP[0].transform.SetParent(parentObject.transform, false);
                x = (float)((float.Parse(data[1]) - longitudeMap) * 2.05993652344 * zoomPow * Math.Cos(latitude * (Math.PI / 180)));
                y = (float)((float.Parse(data[0]) - latitudeMap) * 2.05993652344 * zoomPow);
                objSP[0].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
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

                // 読み込んだ目次テキストファイルからstring配列を作成する
                mapData = text.Split('\n');
                //閉じる
                sr.Close();
                reader.Close();
                mapLoad = true;
            }
            else
            {
                obj.GetComponent<Text>().text = ("[エラー]\nシナリオファイルの異常");
                ErrorBack();
                mapData = new string[0];
            }
            //閉じる
            zf.Close();
        }
        catch
        {
            obj.GetComponent<Text>().text = ("[エラー]\nシナリオファイルの異常");
            ErrorBack();
            mapData = new string[0];
        }
    }

    private void ErrorBack()
    {
        objErrorBack.SetActive(true);
    }

    public void TitleBack()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void TitleBackButton()
    {
        objTitleBack.SetActive(true);
    }

    public void TitleBackDecide()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
    }

    public void TitleBackCancel()
    {
        objTitleBack.SetActive(false);
    }

    private IEnumerator SpotMakeCo()
    {
        yield return null;
        SpotMake();
    }
}


