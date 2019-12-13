using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class ScenarioWebView : MonoBehaviour
{
    private string url = "https://wp026.wappy.ne.jp/brainmixer.net/CoCAR/scenario/read.cgi";
    public GameObject errorObject;
    public GameObject closeObject;
    public GameObject webWindow;
    WebViewObject webViewObject;
    bool visible=false;
    
    void Start()
    {
        webViewObject =
            webWindow.AddComponent<WebViewObject>();
        webViewObject.Init((msg) => {
            StartCoroutine(FileDownload(msg));
        });
        webViewObject.LoadURL(url);
        webViewObject.SetMargins(0, 0, 60, 60);
        webViewObject.SetVisibility(false);
    }

    public void OpenCloseWebView()
    {
        if (visible) { CloseWebView(); }
        else { OpenWebView(); }
    }

    private void OpenWebView()
    {
        errorObject.SetActive(true);
        webViewObject.SetVisibility(true);
        closeObject.SetActive(true);
        visible = true;
        GetComponentInChildren<Text>().text = "<size=48>戻る</size>";
    }

    private void CloseWebView()
    {
        errorObject.SetActive(false);
        webViewObject.SetVisibility(false);
        closeObject.SetActive(false);
        visible = false;
        GetComponentInChildren<Text>().text = "投稿サイトからシナリオを取得";
    }

    private IEnumerator FileDownload(string msg)
    {
        WWW www = new WWW(msg);

        while (!www.isDone)
        { 
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            //エラー表示
            errorObject.GetComponentInChildren<Text>().text = "ダウンロードに失敗しました";
            StartCoroutine(DeleteError());
        }
        else
        { // ダウンロードが正常に完了した
            SafeCreateDirectory(Application.persistentDataPath + "/scenario");
            File.WriteAllBytes(Application.persistentDataPath + "/scenario/" + Path.GetFileName(System.Uri.UnescapeDataString(www.url)), www.bytes);
            errorObject.GetComponentInChildren<Text>().text = "<color=white>ダウンロード完了</color>";
            StartCoroutine(DeleteError());
        }
    }

        public static DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }
            return Directory.CreateDirectory(path);
        }

    private IEnumerator DeleteError()
    {
        for (int i = 0; i < 100; i++) { yield return null; }
        errorObject.GetComponentInChildren<Text>().text = "";
    }
   

}
