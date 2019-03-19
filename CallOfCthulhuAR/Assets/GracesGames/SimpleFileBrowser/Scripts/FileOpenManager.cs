using System;
using UnityEngine;
using UnityEngine.UI;


namespace GracesGames.SimpleFileBrowser.Scripts
{
    public class FileOpenManager : MonoBehaviour
    {

        // Use the file browser prefab
        public GameObject FileBrowserPrefab;
        // Define a file extension
        public string[] FileExtensions;

        public bool PortraitMode;

        public string[] scenarionamePath;

        const int STATUSNUM = 12;
        const int SKILLNUM = 54;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Open the file browser using boolean parameter so it can be called in GUI
        public void OpenFileBrowser(bool saving)
        {
            OpenFileBrowser(saving ? FileBrowserMode.Save : FileBrowserMode.Load);
        }

        // Open a file browser to save and load files
        private void OpenFileBrowser(FileBrowserMode fileBrowserMode)
        {
            string startFolder="";
            if (Application.platform == RuntimePlatform.Android && GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterSheet") { startFolder = Application.persistentDataPath; }
            if (Application.platform == RuntimePlatform.IPhonePlayer && (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterSheet" || GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterIllstPath")) { startFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Documents"; }
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape,startFolder);
            if (fileBrowserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel("NoName", FileExtensions);
                // Subscribe to OnFileSelect event (call SaveFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += SaveFileUsingPath;
            }
            else
            {
                fileBrowserScript.OpenFilePanel(FileExtensions);
                // Subscribe to OnFileSelect event (call LoadFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += LoadFileUsingPath;
            }
        }

        //★ここを改造する
        private void SaveFileUsingPath(string path)
        {
            // Make sure path and _textToSave is not null or empty
            if (!String.IsNullOrEmpty(path))
            {
                //ファイルをアーカイブ化して保存
            }
            else
            {
                Debug.Log("Invalid path or empty file given");
            }
        }

        // Loads a file using a path★ここを改造する
        private void LoadFileUsingPath(string path)
        {
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]進行中シナリオ")
            {
                int[] status = new int[STATUSNUM];
                int[] skills = new int[SKILLNUM];
                string[] tmpstrs = new string[3];
                int[] tmpints = new int[8];
                int VMode = 0;
                if (path != null && path.Length != 0)
                {
                    //フラグ情報の全消去（別シナリオのフラグが生きてると意図せぬバッティングなどバグの元）
                    //残す情報を一時避難
                    for (int i = 0; i < STATUSNUM; i++)
                    {
                        status[i] = PlayerPrefs.GetInt("[system]Status" + i.ToString(), 0);
                    }
                    for (int i = 0; i < SKILLNUM; i++)
                    {
                        skills[i] = PlayerPrefs.GetInt("[system]Skill" + i.ToString(), 0);
                    }
                    tmpstrs[0]= PlayerPrefs.GetString("[system]CharacterIllstPath", "");
                    tmpstrs[1]= PlayerPrefs.GetString("[system]PlayerCharacterName", "");
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
                    PlayerPrefs.SetString("[system]CharacterIllstPath",tmpstrs[0]);
                    PlayerPrefs.SetString("[system]PlayerCharacterName", tmpstrs[1]);
                    PlayerPrefs.SetString("[system]PlayerCharacterNickName", tmpstrs[2]);
                    PlayerPrefs.SetInt("[system]マジック・ポイント",tmpints[0]);
                    PlayerPrefs.SetInt("[system]耐久力", tmpints[1]);
                    PlayerPrefs.SetInt("[system]正気度ポイント", tmpints[2]);
                    PlayerPrefs.SetInt("[system]アイデア", tmpints[3]);
                    PlayerPrefs.SetInt("[system]知識", tmpints[4]);
                    PlayerPrefs.SetInt("[system]幸運", tmpints[5]);
                    PlayerPrefs.SetInt("[system]初発狂", tmpints[6]);
                    PlayerPrefs.SetInt("[system]未決定",tmpints[7]);
                    PlayerPrefs.SetInt("[system]VMode", VMode);
                    GetComponent<TitleManager>().SelectButton.SetActive(true);
                    GetComponent<TitleManager>().DeleteButton.SetActive(true);
                    GameObject.Find("SelectText").GetComponent<Text>().text = "シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size>";
                    PlayerPrefs.SetString("[system]進行中シナリオ",path);
                    scenarionamePath = path.Split(new char[] { '\\', '.','/' });
                    if (scenarionamePath.Length >= 2) { GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n" + scenarionamePath[scenarionamePath.Length - 2]; PlayerPrefs.SetString("[system]ScenarioName", scenarionamePath[scenarionamePath.Length - 2]); }//アドレスからフォルダ名と拡張子を排除。.と\を区切り文字にすると拡張子が最後(Length-1)にあるので、その手前の(Length-2)が欲しい文字列。
                    if (PlayerPrefs.GetString("[system]進行中シナリオ", "").Contains(".zip") == false) { GameObject.Find("TitleManager").GetComponent<TitleManager>().StartButton.SetActive(false); GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n<color=red>シナリオファイルではありません</color>"; } else { GameObject.Find("TitleManager").GetComponent<TitleManager>().StartButton.SetActive(true); }
                }
                else
                {
                    GameObject.Find("SelectText").GetComponent<Text>().text = "<color=red>シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size></color>";
                }
            }
            else if (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterIllstPath")
            {
                PlayerPrefs.SetString("[system]CharacterIllstPath", path);
                StartCoroutine(GetComponent<CSManager>().LoadChara(PlayerPrefs.GetString("[system]CharacterIllstPath", "")));
            }
            else if (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]消去ファイル")
            {
                if (path.Substring(path.Length - 4) == ".png" || path.Substring(path.Length - 4) == ".PNG" || path.Substring(path.Length - 4) == ".zip" || path.Substring(path.Length - 4) == ".ZIP" || path.Substring(path.Length - 4) == ".ccs")
                {
                    System.IO.File.Delete(path);
                }
            }
            else if (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterSheet")
            {
                if (path.Contains(".ccs"))
                {
                    PlayerPrefs.SetString("[system]CharacterSheet", path);
                }
                else
                {
                    PlayerPrefs.SetString("[system]CharacterSheet", "error");
                }
            }

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TitleScene")
            {
                GameObject.Find("TitleManager").GetComponent<TitleManager>().SelectButton.SetActive(true);
                GameObject.Find("TitleManager").GetComponent<TitleManager>().DeleteButton.SetActive(true);
            }
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "CharacterSheet")
            {
                GameObject.Find("CSManager").GetComponent<CSManager>().selectButton.SetActive(true);
                GameObject.Find("CSManager").GetComponent<CSManager>().readButton.SetActive(true);
            }
        }

        public void GetFilePathWithKey(string key)
        {
            GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey = key;
            OpenFileBrowser(false);
        }
    }
}
