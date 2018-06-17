using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapScene : MonoBehaviour
{
    Sprite mapImage;
    private float intervalTime = 0.0f;
    private int width = 800;
    private int height = 600;
    private double longitude;
    private double latitude;
    private int zoom = 16;
    GameObject mapImageObj;
    GameObject obj;

    void Start()
    {
        //シナリオ進度読込。
        //シナリオ進度情報からイベント地点読込。
        obj = GameObject.Find("error").gameObject as GameObject;
        mapImageObj = GameObject.Find("mapImage").gameObject as GameObject;
        GetPos();
        GetMap();
    }
    void Update()
    {
        //毎フレーム読んでると処理が重くなるので、3秒毎に更新
        intervalTime += Time.deltaTime;
        if (intervalTime >= 3.0f)
        {
            GetPos();
            GetMap();
            intervalTime = 0.0f;
        }
    }
    void GetPos()
    {
        //GPSで取得した緯度経度を変数に代入
        StartCoroutine(GetGPS());
        longitude = Input.location.lastData.longitude;
        latitude = Input.location.lastData.latitude;
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
        //現在地マーカーはここの「&markers」以下で編集可能
        string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + "&markers=size:mid%7Ccolor:red%7C" + latitude + "," + longitude;
        WWW www = new WWW(url);
        yield return www;
        //マップの画像をTextureからspriteに変換して貼り付ける
        mapImage = Sprite.Create(www.texture, new Rect(0, 0, 640, 600), Vector2.zero);
        mapImageObj.GetComponent<Image>().sprite=mapImage;
    }
}


