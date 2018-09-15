using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MapScene : MonoBehaviour
{
    Sprite mapImage;
    private float intervalTime = 0.0f;
    private float intervalTime2 = 0.0f;
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
    GameObject mapImageObj;
    GameObject obj;
    GameObject objTarget;
    GameObject objBGM;
    GameObject objTime;
    string _FILE_HEADER; 

    void Start()
    {
        _FILE_HEADER = PlayerPrefs.GetString("進行中シナリオ","");                      //ファイル場所の頭
        if (_FILE_HEADER==null || _FILE_HEADER == "") { GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene"); }
        longitude = 135.768738;
        latitude = 35.010348;
        obj = GameObject.Find("error").gameObject as GameObject;
        objTarget = GameObject.Find("Target").gameObject as GameObject;
        mapImageObj = GameObject.Find("mapImage").gameObject as GameObject;
        objBGM= GameObject.Find("BGMManager").gameObject as GameObject;
        objTime = GameObject.Find("TimeText").gameObject as GameObject;
        LoadMapData("mapdata.txt");
        GetPos();
        GetMap();
    }

    void Update()
    {
        //毎フレーム読んでると処理が重くなるので、3秒毎にGPS読込
        intervalTime += Time.deltaTime; intervalTime2 += Time.deltaTime;
        if (intervalTime >= 0.03f)//★スマホ版は3.0f
        {
            GetPos();
            intervalTime = 0.0f;
        }
        if (intervalTime2 > 2.0f) { IventCheck(); }
        objBGM.GetComponent<Text>().text = longitude.ToString() + "," + latitude.ToString();
        //地図の更新は、マップ範囲から出た時かつ時間が相当経過している時に。（時間変数入れないと、場所によってはGPS誤差でマップ連続読込になりかねない）
            if (((longitude>longitudeMap+0.003) ||
            (longitude < longitudeMap - 0.003) ||
            (latitude < latitudeMap - 0.005) ||
            (latitude > latitudeMap + 0.005)) &&
            (intervalTime2 >= 5.0f)
            )
        {
            GetMap();
            intervalTime2 = 0.0f;
        }
    }

    //マップデータテキストを読み込んで、現在位置と時刻が一致するイベントがあれば読込。
    void IventCheck()
    {
        DateTime dt;
        dt = DateTime.Now;
        string[] data;
        for (int i = 0; i < mapData.Length; i++)
        {
            if (mapData[i] == "[END]") { break; }
            data =mapData[i].Split(',');
            if ((data[0] == "" || double.Parse(data[0]) > latitude - 0.0001 && double.Parse(data[0]) < latitude + 0.001) &&
                (data[1] == "" || double.Parse(data[1]) > longitude - 0.0001 && double.Parse(data[1]) < longitude + 0.001) &&
                (data[2] == "" || (int.Parse(data[2]) >= dt.Month)) &&
                (data[3] == "" || (int.Parse(data[3]) >= dt.Day) || (int.Parse(data[2]) > dt.Month)) &&
                (data[4] == "" || (int.Parse(data[4]) >= dt.Hour) || (int.Parse(data[3]) > dt.Day) || (int.Parse(data[2]) > dt.Month)) &&
                (data[5] == "" || (int.Parse(data[5]) >= dt.Minute) || (int.Parse(data[4]) > dt.Hour) || (int.Parse(data[3]) > dt.Day) || (int.Parse(data[2]) > dt.Month)) &&
                (data[6] == "" || (int.Parse(data[6]) <= dt.Month)) &&
                (data[7] == "" || (int.Parse(data[7]) <= dt.Day) || (int.Parse(data[6]) < dt.Month)) &&
                (data[8] == "" || (int.Parse(data[8]) <= dt.Hour) || (int.Parse(data[7]) < dt.Day) || (int.Parse(data[6]) < dt.Month)) &&
                (data[9] == "" || (int.Parse(data[9]) <= dt.Minute) || (int.Parse(data[8]) < dt.Hour) || (int.Parse(data[7]) < dt.Day) || (int.Parse(data[6]) < dt.Month)) &&
                (data[10] == "" || PlayerPrefs.GetInt(data[10], 0) > 0))
            {
                objBGM.GetComponent<BGMManager>().chapterName = data[11].Replace("\r", "").Replace("\n", "");
                objTime.GetComponent<Text>().text = "[★イベント発生]";
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "NovelScene");
            }
        }
        objTime.GetComponent<Text>().text= dt.ToString("MM/dd  HH:mm") + "\n" + "<size=48>緯度：" + Math.Round(latitude, 4).ToString() + "　,　経度：" + Math.Round(longitude,4).ToString().ToString() + "</size>";
    }

    void GetPos()
    {
        //★①スマートフォン版（ＧＰＳから位置情報を拾う）
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
        //GPSで取得した緯度経度を変数に代入
        StartCoroutine(GetGPS());
        longitude = Input.location.lastData.longitude;
        latitude = Input.location.lastData.latitude;
        }
        //★②PC版（キー入力で動かす）
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (Input.GetKey(KeyCode.DownArrow)) { latitude -= 0.00001; }
            if (Input.GetKey(KeyCode.UpArrow)) { latitude += 0.00001; }
            if (Input.GetKey(KeyCode.LeftArrow)) { longitude -= 0.00001; }
            if (Input.GetKey(KeyCode.RightArrow)) { longitude += 0.00001; }
        }
        objTarget.GetComponent<RectTransform>().localPosition = new Vector3(targetX, targetY, 0);
        targetX = (float)((longitude - longitudeMap) * 80000);
        targetY= (float)((latitude - latitudeMap) * (100000 + ((latitude - 27) / 80)*100000));
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
            yield break;
        }
        Input.location.Start();
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
            obj.GetComponent<Text>().text = ("エラー。位置情報が使用できません。");
            yield break;
        }
        else
        {
                  print("現在座標: " +
                  Input.location.lastData.latitude + " " +
                  Input.location.lastData.longitude + " " +
                  Input.location.lastData.altitude + " " +
                  Input.location.lastData.horizontalAccuracy + " " +
                  Input.location.lastData.timestamp);
        }
        Input.location.Stop();
    }


    private IEnumerator GetStreetViewImage(double latitude, double longitude, double zoom)
    {
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height;
        WWW www = new WWW(url);
        yield return www;
        //マップの画像をTextureからspriteに変換して貼り付ける
        mapImage = Sprite.Create(www.texture, new Rect(0, 0, 640, 640), Vector2.zero);
        mapImageObj.GetComponent<Image>().sprite=mapImage;

        //地図の中心の緯度経度を保存
        longitudeMap = longitude;
        latitudeMap = latitude;

        //targetの位置を中心に
        targetX = 0;targetY = 0;
    }

    //目次ファイルを読み込み、進行度に合わせてファイルを拾ってくる。
    private void LoadMapData(string path)
    {
        //閲覧するエントリ
        string extractFile = path;

        //ZipFileオブジェクトの作成
        ICSharpCode.SharpZipLib.Zip.ZipFile zf =
            new ICSharpCode.SharpZipLib.Zip.ZipFile(PlayerPrefs.GetString("進行中シナリオ",""));
        zf.Password = "I_change_the_world";
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
        }
        else
        {
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "TitleScene");
        }
        //閉じる
        zf.Close();
        /*ファイルをそのまま展開する時のバージョン
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
        mapData = text.Split('\n');
        */
    }
}


