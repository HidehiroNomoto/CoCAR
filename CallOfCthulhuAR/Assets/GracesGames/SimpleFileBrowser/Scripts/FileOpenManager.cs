﻿using System;
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
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
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
                string[] tmpstrs = new string[2];
                int[] tmpints = new int[7];
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
                   tmpints[0] = PlayerPrefs.GetInt("[system]マジック・ポイント"); 
                   tmpints[1] = PlayerPrefs.GetInt("[system]耐久力"); 
                   tmpints[2] = PlayerPrefs.GetInt("[system]正気度ポイント");
                    tmpints[3] = PlayerPrefs.GetInt("[system]アイデア");
                   tmpints[4] = PlayerPrefs.GetInt("[system]知識");
                   tmpints[5] = PlayerPrefs.GetInt("[system]幸運");
                    tmpints[6] = PlayerPrefs.GetInt("[system]初発狂");
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
                    PlayerPrefs.SetInt("[system]マジック・ポイント",tmpints[0]);
                    PlayerPrefs.SetInt("[system]耐久力", tmpints[1]);
                    PlayerPrefs.SetInt("[system]正気度ポイント", tmpints[2]);
                    PlayerPrefs.SetInt("[system]アイデア", tmpints[3]);
                    PlayerPrefs.SetInt("[system]知識", tmpints[4]);
                    PlayerPrefs.SetInt("[system]幸運", tmpints[5]);
                    PlayerPrefs.SetInt("[system]初発狂", tmpints[6]);
                    GameObject.Find("SelectText").GetComponent<Text>().text = "シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size>";
                    //if (Application.platform == RuntimePlatform.IPhonePlayer) { path = "/Documents/Inbox/" + System.IO.Path.GetFileName(path); }
                    PlayerPrefs.SetString("[system]進行中シナリオ",path);
                    scenarionamePath = path.Split(new char[] { '\\', '.','/' });
                    if (scenarionamePath.Length >= 2) { GameObject.Find("ScenarioName").GetComponent<Text>().text = "[シナリオ名]\n" + scenarionamePath[scenarionamePath.Length - 2]; }//アドレスからフォルダ名と拡張子を排除。.と\を区切り文字にすると拡張子が最後(Length-1)にあるので、その手前の(Length-2)が欲しい文字列。
                }
                else
                {
                    GameObject.Find("SelectText").GetComponent<Text>().text = "<color=red>シナリオ選択<size=28>\n(DLしたファイルから選ぶ)</size></color>";
                }
            }
            else if (GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey == "[system]CharacterIllstPath")
            {
                //if (Application.platform == RuntimePlatform.IPhonePlayer) { path = "/Documents/Inbox/" + System.IO.Path.GetFileName(path); }
                PlayerPrefs.SetString("[system]CharacterIllstPath", path);
                StartCoroutine(GetComponent<CSManager>().LoadChara(PlayerPrefs.GetString("[system]CharacterIllstPath", "")));
            }
        }

        public void GetFilePathWithKey(string key)
        {
            GameObject.Find("BGMManager").GetComponent<BGMManager>().saveKey = key;
            OpenFileBrowser(false);
        }
    }
}
