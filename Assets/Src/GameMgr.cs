using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour {

    public Button StartGame;

    public Button GameIntro;

    public Button FellowHistory;

    public Transform GameMenu;

    public Transform Game;

    public Transform Introduction;

    public Transform History;


    // Use this for initialization
    void Start () {
        StartGame.onClick.AddListener(() => {
            Debug.Log("点击开始游戏");
        });
        GameIntro.onClick.AddListener(() => {
            Debug.Log("点击游戏介绍");
            GameMenu.gameObject.SetActive(false);
            Introduction.gameObject.SetActive(true);
            Button btnQuit = Introduction.Find("BtnBack").GetComponent<Button>();
            btnQuit.onClick.RemoveAllListeners();
            btnQuit.onClick.AddListener(() => {
                GameMenu.gameObject.SetActive(true);
                Introduction.gameObject.SetActive(false);
                Debug.Log("点击返回至菜单");
            });
        });
        FellowHistory.onClick.AddListener(() => {
            Debug.Log("点击人物背景");
            TextAsset FellowAll = Resources.Load<TextAsset>("FellowSkill");
<<<<<<< HEAD
            JSONObject json = new JSONObject(FellowAll.text);
=======
            JSONObject lightJson = new JSONObject(FellowAll.text).GetField("lightFellow");
            JSONObject darkJson = new JSONObject(FellowAll.text).GetField("darkFellow");
>>>>>>> commit demo
            GameMenu.gameObject.SetActive(false);
            History.gameObject.SetActive(true);
            Button btnQuit = History.Find("BtnBack").GetComponent<Button>();
            btnQuit.onClick.RemoveAllListeners();
            btnQuit.onClick.AddListener(() => {
                GameMenu.gameObject.SetActive(true);
                History.gameObject.SetActive(false);
                Debug.Log("点击返回至菜单");
            });
<<<<<<< HEAD
            for(int i = 0; i < 16; i++)
            {
                int index = i + 1;
                string name = "Fellow" + index;
                GameObject btnFellow = History.Find("FellowList/List/" + name).gameObject;
                if (btnFellow != null)
                {
                    JSONObject FellowInfo = json.GetField(index + "");
                    Debug.Log(string.Format("name为{0}的btnFellow不为空", name));
                    Text fellowName = btnFellow.transform.Find("Text").gameObject.GetComponent<Text>();
                    fellowName.text = FellowInfo.GetField("name").str;
                    Button btn = btnFellow.GetComponent<Button>();
=======
            for (int i = 0; i < 15; i++)
            {
                int index = i + 1;
                string name = "Fellow" + index;
                GameObject btnLightFellow = History.Find("LightFellowList/List/" + name).gameObject;
                if (btnLightFellow != null)
                {
                    JSONObject FellowInfo = lightJson.GetField(index + "");
                    Debug.Log(string.Format("name为{0}的btnFellow不为空", name));
                    Text fellowName = btnLightFellow.transform.Find("Text").gameObject.GetComponent<Text>();
                    fellowName.text = FellowInfo.GetField("name").str;
                    Button btn = btnLightFellow.GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        ShowFellowInfo(i, FellowInfo);
                    });
                }
            }
            for (int i = 0; i < 16; i++) {
                int index = i + 1;
                string name = "Fellow" + index;
                GameObject btnDarkFellow = History.Find("ShadowFellowList/List/" + name).gameObject;
                if (btnDarkFellow != null)
                {
                    JSONObject FellowInfo = darkJson.GetField(index + "");
                    Debug.Log(string.Format("name为{0}的btnFellow不为空", name));
                    Text fellowName = btnDarkFellow.transform.Find("Text").gameObject.GetComponent<Text>();
                    fellowName.text = FellowInfo.GetField("name").str;
                    Button btn = btnDarkFellow.GetComponent<Button>();
>>>>>>> commit demo
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        ShowFellowInfo(i, FellowInfo);
                    });
<<<<<<< HEAD

=======
>>>>>>> commit demo
                }
            }
        });
    }

    void ShowFellowInfo(int i, JSONObject fellowInfo)
    {
        Debug.Log("展示第"+(i+1)+"名英雄的信息");
        string fellowSkill = fellowInfo.GetField("skillIntro").str;
        string historyIntro = fellowInfo.GetField("HistoryIntro").str;
        Text FellowSkill = History.Find("FellowHistory/List/FellowSkill").gameObject.GetComponent<Text>();
        Text HistoryIntro = History.Find("FellowHistory/List/HistoryIntro").gameObject.GetComponent<Text>();
        FellowSkill.text = "英雄技能：" + fellowSkill;
        HistoryIntro.text = "背景介绍：" + historyIntro;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
