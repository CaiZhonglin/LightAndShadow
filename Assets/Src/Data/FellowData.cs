using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellowData : MonoBehaviour {
    public List<fellow> lightFellowInfo;
    public List<fellow> shadowFellowInfo;
    private static FellowData m_Instance;

    public static FellowData Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new FellowData();
            return m_Instance;
        }
    }
    public struct fellow
    {
        public bool isLightFellow;
        public string fellowName;
        public string fellowSkillInfo;
        public string fellowHistory;
        public int straightSpeed;
        public int curveSpeed;
        public int fellowValue;
        public int skillType;//1为额外移动回合
        public int skillValue;
        public int skillCount;
    }
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void saveFellowInfo()
    {
        lightFellowInfo = new List<fellow>();
        shadowFellowInfo = new List<fellow>();
        TextAsset FellowAll = Resources.Load<TextAsset>("config/FellowSkill");
        JSONObject json = new JSONObject(FellowAll.text);
        JSONObject lightJson = new JSONObject(FellowAll.text).GetField("lightFellow");
        JSONObject darkJson = new JSONObject(FellowAll.text).GetField("darkFellow");
        for (int i = 0; i < lightJson.Count; i++)
        {
            fellow subFellow = new fellow();
            int index = i + 1;
            JSONObject FellowInfo = lightJson.GetField(index + "");
            subFellow.isLightFellow = true;
            subFellow.fellowName = FellowInfo.GetField("name").str;
            subFellow.fellowSkillInfo = FellowInfo.GetField("skillIntro").str;
            subFellow.fellowHistory = FellowInfo.GetField("HistoryIntro").str;
            subFellow.straightSpeed = (int)FellowInfo.GetField("straightSpeed").n;
            subFellow.curveSpeed = (int)FellowInfo.GetField("curveSpeed").n;
            subFellow.fellowValue = (int)FellowInfo.GetField("valueNum").n;
            subFellow.skillType = (int)FellowInfo.GetField("skillType").n;
            subFellow.skillValue = (int)FellowInfo.GetField("skillValue").n;
            if (subFellow.skillType == 1)
            {
                subFellow.skillCount = 1;
            }
            lightFellowInfo.Add(subFellow);
        }
        for (int i = 0; i < darkJson.Count; i++)
        {
            fellow subFellow = new fellow();
            int index = i + 1;
            JSONObject FellowInfo = darkJson.GetField(index + "");
            subFellow.isLightFellow = false;
            subFellow.fellowName = FellowInfo.GetField("name").str;
            subFellow.fellowSkillInfo = FellowInfo.GetField("skillIntro").str;
            subFellow.fellowHistory = FellowInfo.GetField("HistoryIntro").str;
            subFellow.straightSpeed = (int)FellowInfo.GetField("straightSpeed").n;
            subFellow.curveSpeed = (int)FellowInfo.GetField("curveSpeed").n;
            subFellow.fellowValue = (int)FellowInfo.GetField("valueNum").n;
            subFellow.skillType = (int)FellowInfo.GetField("skillType").n;
            subFellow.skillValue = (int)FellowInfo.GetField("skillValue").n;
            if (subFellow.skillType == 1)
            {
                subFellow.skillCount = 1;
            }
            shadowFellowInfo.Add(subFellow);
        }
    } 
    public fellow getLightFellowByID(int id)
    {
        if (lightFellowInfo.Count > id)
        {
            return lightFellowInfo[id];
        }
        else
        {
            return new fellow();
        }
    }
    public fellow getShadowFellowByID(int id)
    {
        if (shadowFellowInfo.Count > id)
        {
            return shadowFellowInfo[id];
        }
        else
        {
            return new fellow();
        }
    }
    public List<fellow> getLightFellowList()
    {
        if (lightFellowInfo != null && lightFellowInfo.Count != 0)
        {
            return lightFellowInfo;
        }
        else
        {
            return null;
        }
    }
    public List<fellow> getShadowFellowList()
    {
        if (shadowFellowInfo != null && shadowFellowInfo.Count != 0)
        {
            return shadowFellowInfo;
        }
        else
        {
            return null;
        }
    }
}
