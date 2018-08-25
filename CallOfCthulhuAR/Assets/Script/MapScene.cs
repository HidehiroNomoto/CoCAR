using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    GameObject mapImageObj;
    GameObject obj;
    GameObject objTarget;

    //★緯度経度を６０進法にする関数を組む


    void Start()
    {
        //シナリオ進度読込。
        //シナリオ進度情報からイベント地点読込。
        longitude = 135.768738;
        latitude = 35.010348;
        obj = GameObject.Find("error").gameObject as GameObject;
        objTarget = GameObject.Find("Target").gameObject as GameObject;
        mapImageObj = GameObject.Find("mapImage").gameObject as GameObject;
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
    //PC版（キー入力で移動するエミュレータ版）
    void GetPos()
    {
        if (Input.GetKey(KeyCode.DownArrow)) { latitude -= 0.00001; targetY -= (float)(1.0 + (latitude - 27) / 80);  }
        if (Input.GetKey(KeyCode.UpArrow)) { latitude += 0.00001; targetY += (float)(1.0 + (latitude - 27) / 80); }
        if (Input.GetKey(KeyCode.LeftArrow)) { longitude -= 0.00001;targetX -= (float)0.8;  }
        if (Input.GetKey(KeyCode.RightArrow)) { longitude += 0.00001; targetX += (float)0.8; }
        objTarget.GetComponent<RectTransform>().localPosition = new Vector3(targetX, targetY, 0);

    }
    /*★スマートフォン版
    void GetPos()
    {
        //GPSで取得した緯度経度を変数に代入
        StartCoroutine(GetGPS());
        longitude = Input.location.lastData.longitude;
        latitude = Input.location.lastData.latitude;
    }
    */

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
}


