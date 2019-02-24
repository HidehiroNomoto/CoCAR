using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour {

    //BGM再生に伴うフラグを管理するスクリプト（BGMManager本体がdontdestroyオブジェクトなのでシーンをまたぐ際に使うフラグがある）
    public bgmFlag b1;
    //
    public string chapterName="start.txt";
    public string saveKey = "[system]進行中シナリオ";
    //マルチプレイのフラグ
    public int multiPlay = 0;
    public int makuma = 0;

    public struct bgmFlag
    {
        public bool bgmChangeFlag;//新しいシーンでBGMを新たに再生するか
        public int bgmNum;//新しいシーンでも流れ続ける場合のBGMの番号（scenario.csのbgmリスト番号）※スキップの場合には新たに流す必要があるため
        public bgmFlag(bool flag, int num) { this.bgmChangeFlag = flag; this.bgmNum = num; }
    }

    // Use this for initialization
    void Start () {

    }

	
	// Update is called once per frame
	void Update () {
		
	}

    public void bgmChange(bool flag,int num)
    {
        b1=new bgmFlag(flag,num);
    }


        // 現在存在しているオブジェクト実体の記憶領域
        static BGMManager _instance = null;

        // オブジェクト実体の参照（初期参照時、実体の登録も行う）
        static BGMManager instance
        {
            get { return _instance ?? (_instance = FindObjectOfType<BGMManager>()); }
        }

        void Awake()
        {

            // ※オブジェクトが重複していたらここで破棄される

            // 自身がインスタンスでなければ自滅
            if (this != instance)
            {
                Destroy(gameObject);
                return;
            }

            // 以降破棄しない
            DontDestroyOnLoad(gameObject);

        }

        void OnDestroy()
        {

            // ※破棄時に、登録した実体の解除を行なっている

            // 自身がインスタンスなら登録を解除
            if (this == instance) _instance = null;

        }



}
