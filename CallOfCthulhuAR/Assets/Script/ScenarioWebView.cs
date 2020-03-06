using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class ScenarioWebView : MonoBehaviour
{
    private string url = "https://brainmixer.net/CoCAR/scenario/read.cgi";
    public GameObject errorObject;
    public GameObject closeObject;
    public GameObject webWindow;
    WebViewObject webViewObject;
    bool visible=false;
    int dlWaitNum = 0;
    
    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
#else
        webViewObject =
            webWindow.AddComponent<WebViewObject>();
        webViewObject.Init((msg) => {
            StartCoroutine(FileDownload(msg));
        }
        ,enableWKWebView: true);
        webViewObject.LoadURL(url);
        webViewObject.SetMargins(0, 0, 60, 100);
        webViewObject.SetVisibility(false);
#endif
    }

    void Update()
    {
    }

    public void OpenCloseWebView()
    {
#if UNITY_STANDALONE_WIN
        Application.OpenURL("https://brainmixer.net/CoCAR/scenario/upload.cgi");
#else
        if (visible) { CloseWebView(); }
        else { OpenWebView(); }
#endif
    }

    private void OpenWebView()
    {
        errorObject.SetActive(true);
        errorObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        webViewObject.SetVisibility(true);
        closeObject.SetActive(true);
        visible = true;
    }

    private void CloseWebView()
    {
        if (dlWaitNum == 0) {
            errorObject.GetComponentInChildren<Text>().text = "";
            errorObject.SetActive(false);
        }
        webViewObject.SetVisibility(false);
        closeObject.SetActive(false);
        visible = false;
    }

    private IEnumerator FileDownload(string msg)
    {
        if (dlWaitNum > 0) { yield break; }
        WWW www = new WWW(msg);
        dlWaitNum++;
        //errorObject.GetComponentInChildren<Text>().text = "<color=yellow>ダウンロード中です</color>";
        while (!www.isDone)
        {
            errorObject.GetComponentInChildren<Text>().text = "<color=yellow>ダウンロード中 " + ((int)(www.progress*100)).ToString() + "%</color>";
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            //エラー表示
            errorObject.GetComponentInChildren<Text>().text = "ダウンロードに失敗しました";
        }
        else
        { // ダウンロードが正常に完了した
            SafeCreateDirectory(Application.persistentDataPath + "/scenario");
            File.WriteAllBytes(Application.persistentDataPath + "/scenario/" + Path.GetFileName(System.Uri.UnescapeDataString(www.url)), www.bytes);
            errorObject.GetComponentInChildren<Text>().text = "<color=white>ダウンロード完了</color>";
        }
        StartCoroutine(DeleteError());
        dlWaitNum--;
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
        if (errorObject.activeSelf == true && visible==false) {
            errorObject.GetComponentInChildren<Text>().text = "";
            errorObject.SetActive(false); }
    }
   

}
