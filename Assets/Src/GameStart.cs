using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct aa
{
    public int[] bb;

}

public class GameStart : MonoBehaviour {

    public List<int[]> lightFellowSpot = new List<int[]>();
    public List<int[]> shadowFellowSpot = new List<int[]>();
    public List<int[]> lightCampSpot = new List<int[]>();
    public List<int[]> shadowCampSpot = new List<int[]>();
    public Transform canvas;
    public List<aa> LightSpot;
    public List<aa> ShadowSpot;
    public FellowData fellow;
    public List<FellowData.fellow> lightFellowList;
    public List<FellowData.fellow> shadowFellowList;
    public int LightValueNum;
    public int ShadowValueNum;
    public List<aa> LightCampSpot;
    public List<aa> ShadowCampSpot;



    GameObject canvasPart;
    GameObject chessBoard;
    bool lightTurn = false;
    int lightFellowNum = 0;
    int shadowFellowNum = 0;
    Dictionary<string, spotInfo> spotState = new Dictionary<string, spotInfo>();
    // Use this for initialization
    void Start() {
        Initialise();
    }

    void Initialise()
    {
        Button BtnBack = canvas.Find("Game/BtnBack").gameObject.GetComponent<Button>();
        BtnBack.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("init");
        });

        fellow = new FellowData();
        fellow.saveFellowInfo();
        lightFellowList = fellow.getLightFellowList();
        shadowFellowList = fellow.getShadowFellowList();
        lightTurn = true;
        //int[] demoSpot1 = { 2, 3 };
        //int[] demoSpot2 = { 3, 4 };
        //int[] demoSpot3 = { 8, 9 };
        //int[] demoSpot4 = { 7, 8 };
        //lightSpot.Add(demoSpot1);
        //lightSpot.Add(demoSpot2);
        //shadowSpot.Add(demoSpot3);
        //shadowSpot.Add(demoSpot4);
        for (int i = 0; i < LightSpot.Count; i++)
        {
            int[] demoSpot = LightSpot[i].bb;
            lightFellowSpot.Add(demoSpot);
        }
        for (int i = 0; i < ShadowSpot.Count; i++)
        {
            int[] demoSpot = ShadowSpot[i].bb;
            shadowFellowSpot.Add(demoSpot);
        }

        for (int i = 0; i < LightCampSpot.Count; i++)
        {
            int[] demoSpot = LightCampSpot[i].bb;
            lightCampSpot.Add(demoSpot);
        }
        for (int i = 0; i < ShadowCampSpot.Count; i++)
        {
            int[] demoSpot = ShadowCampSpot[i].bb;
            shadowCampSpot.Add(demoSpot);
        }
        lightFellowNum = lightFellowSpot.Count;
        shadowFellowNum = shadowFellowSpot.Count;
        DrawChessBoard();
        DrawPlayer();
    }

    void DrawChessBoard()
    {
        chessBoard = canvas.Find("Game/ChessBoard").gameObject;
        GameObject spot = canvas.Find("Game/ChessBoard/spot").gameObject;
        canvasPart = canvas.gameObject;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string name = "spot" + i + j;
                GameObject subSpot = FindElementGo(canvasPart, "Game/ChessBoard/" + name);
                if (subSpot == null)
                {
                    subSpot = GameObject.Instantiate(spot);
                    subSpot.transform.SetParent(chessBoard.transform);
                    subSpot.transform.localScale = Vector3.one;
                    subSpot.name = name;
                    subSpot.transform.localPosition = new Vector3(-270 + i * 60, -270 + j * 60, 0);
                    subSpot.SetActive(true);
                }
            }
        }
        spot.SetActive(false);
        
    }

    void ShowTurnMsg()
    {
        Text turn = canvas.Find("Game/GameUI/GrpFrame/List/Turn").gameObject.GetComponent<Text>();
        Text fellowName = canvas.Find("Game/GameUI/GrpFrame/List/FellowName").gameObject.GetComponent<Text>();
        Text skill = canvas.Find("Game/GameUI/GrpFrame/List/Skill").gameObject.GetComponent<Text>();
        Text lightValue = canvas.Find("Game/GameUI/GrpFrame/List/LightValueNum").gameObject.GetComponent<Text>();
        Text shadowValue = canvas.Find("Game/GameUI/GrpFrame/List/ShadowValueNum").gameObject.GetComponent<Text>();
        Text skillType = canvas.Find("Game/GameUI/GrpFrame/List/SkillType").gameObject.GetComponent<Text>();
        Text skillValue = canvas.Find("Game/GameUI/GrpFrame/List/SkillValue").gameObject.GetComponent<Text>();
        Text skillCount = canvas.Find("Game/GameUI/GrpFrame/List/SkillCount").gameObject.GetComponent<Text>();
        if (lightTurn)
        {
            turn.text = "Light Turn";
        }
        else
        {
            turn.text = "Shadow Turn";
        }
        fellowName.text = "";
        skill.text = "";
        skillType.text = "";
        skillValue.text = "";
        skillCount.text = "";
        lightValue.text = "light:"+ LightValueNum + "祭";
        shadowValue.text = "shadow:" + ShadowValueNum + "祭";
    }

    public GameObject FindElementGo(GameObject go, string name)
    {
        if (go == null) return null;
        GameObject subGo = null;
        Transform trans = go.transform.Find(name);
        if (trans != null) subGo = trans.gameObject;
        return subGo;
    }

    void DrawPlayer()
    {
        for (int a = 0; a < 10; a++) //初始化地图位置
        {
            for (int b = 0; b < 10; b++)
            {
                string defaultSpot = a + "" + b;
                if (spotState.ContainsKey(defaultSpot))
                {
                    spotState[defaultSpot].FellowType = 0;
                }
                else
                {
                    spotInfo spot = new spotInfo();
                    spotState.Add(defaultSpot, spot);
                }
            }
        }
        for(int i = 0; i < lightFellowSpot.Count; i++)
        {
            if(lightFellowSpot[i] != null)
            {
                int[] playerSpot = lightFellowSpot[i];
                int index = i;
                spotState[(playerSpot[0] + "" + playerSpot[1])].FellowType = 1;
                spotState[(playerSpot[0] + "" + playerSpot[1])].FellowId = index;
                int x = playerSpot[0];
                int y = playerSpot[1];
                string name = "spot" + x + y;
                GameObject player = FindElementGo(chessBoard, name);
                if (player != null)
                {
                    Image icon = player.transform.Find("player").gameObject.GetComponent<Image>();
                    icon.sprite = Resources.Load<Sprite>("UIRes/fellowIcon/LightIcon"+ (index+1));
                    player.transform.Find("CannotGo").gameObject.SetActive(false);
                    player.transform.Find("player").gameObject.SetActive(true);
                    Button btnPlayer = player.GetComponent<Button>();
                    if (lightTurn)
                    {
                        player.transform.Find("CanGo").gameObject.SetActive(true);
                        btnPlayer.onClick.AddListener(() =>
                        {
                            OnClickPlayer(x, y, index);
                        });
                    }
                    else
                    {
                        player.transform.Find("CanGo").gameObject.SetActive(false);
                        btnPlayer.onClick.RemoveAllListeners();
                    }
                }
            }
        }

        for (int i = 0; i < shadowFellowSpot.Count; i++)
        {
            if (shadowFellowSpot[i] != null)
            {
                int[] playerSpot2 = shadowFellowSpot[i];
                int ClickedPlayerId = i;
                spotState[(playerSpot2[0] + "" + playerSpot2[1])].FellowType = 2;
                spotState[(playerSpot2[0] + "" + playerSpot2[1])].FellowId = ClickedPlayerId;
                int x = playerSpot2[0];
                int y = playerSpot2[1];
                string name2 = "spot" + x + y;
                GameObject player2 = FindElementGo(chessBoard, name2);
                if (player2 != null)
                {
                    Image icon = player2.transform.Find("player").gameObject.GetComponent<Image>();
                    icon.sprite = Resources.Load<Sprite>("UIRes/fellowIcon/ShadowIcon"+ (ClickedPlayerId + 1));
                    player2.transform.Find("CannotGo").gameObject.SetActive(false);
                    player2.transform.Find("player").gameObject.SetActive(true);
                    Button btnPlayer = player2.GetComponent<Button>();
                    if (!lightTurn)
                    {
                        player2.transform.Find("CanGo").gameObject.SetActive(true);
                        btnPlayer.onClick.AddListener(() =>
                        {
                            OnClickPlayer(x, y, ClickedPlayerId);
                        });
                    }
                    else
                    {
                        player2.transform.Find("CanGo").gameObject.SetActive(false);
                        btnPlayer.onClick.RemoveAllListeners();
                    }
                }
            }
        }
        for (int i = 0; i < lightCampSpot.Count; i++)
        {
            int[] campSpot = lightCampSpot[i];
            int ClickedCampId = i;
            spotState[(campSpot[0] + "" + campSpot[1])].FellowType = 3;
            int x = campSpot[0];
            int y = campSpot[1];
            string name = "spot" + x + y;
            GameObject player = FindElementGo(chessBoard, name);
            if (player != null)
            {
                Image icon = player.transform.Find("player").gameObject.GetComponent<Image>();
                if(icon.sprite == null)
                {
                    int num = Random.Range(1, 5);
                    icon.sprite = Resources.Load<Sprite>("UIRes/buildingIcon/lightBuilding" + num);
                }
                player.transform.Find("CanGo").gameObject.SetActive(false);
                player.transform.Find("CannotGo").gameObject.SetActive(false);
                player.transform.Find("player").gameObject.SetActive(true);
                Button btnPlayer = player.GetComponent<Button>();
                btnPlayer.onClick.RemoveAllListeners();
            }
        }
        for (int i = 0; i < shadowCampSpot.Count; i++)
        {
            int[] campSpot = shadowCampSpot[i];
            int ClickedCampId = i;
            spotState[(campSpot[0] + "" + campSpot[1])].FellowType = 4;
            int x = campSpot[0];
            int y = campSpot[1];
            string name = "spot" + x + y;
            GameObject player = FindElementGo(chessBoard, name);
            if (player != null)
            {
                Image icon = player.transform.Find("player").gameObject.GetComponent<Image>();
                if (icon.sprite == null)
                {
                    int num = Random.Range(1, 5);
                    icon.sprite = Resources.Load<Sprite>("UIRes/buildingIcon/shadowBuilding" + num);
                }
                player.transform.Find("CanGo").gameObject.SetActive(false);
                player.transform.Find("CannotGo").gameObject.SetActive(false);
                player.transform.Find("player").gameObject.SetActive(true);
                Button btnPlayer = player.GetComponent<Button>();
                btnPlayer.onClick.RemoveAllListeners();
            }
        }
        ShowTurnMsg();
    }
    
    void OnClickPlayer(int x, int y, int ClickedPlayerId,bool canCancle = true,bool usingSkill = false)//canCancle为此次移动是否可以通过点击其他位置取消，usingSkill为此次移动是否是通过主动技能进行的额外移动
    {
        int straightSpeed;
        int curveSpeed;
        
        Text fellowName = canvas.Find("Game/GameUI/GrpFrame/List/FellowName").gameObject.GetComponent<Text>();
        Text skill = canvas.Find("Game/GameUI/GrpFrame/List/Skill").gameObject.GetComponent<Text>();
        Text skillType = canvas.Find("Game/GameUI/GrpFrame/List/SkillType").gameObject.GetComponent<Text>();
        Text skillValue = canvas.Find("Game/GameUI/GrpFrame/List/SkillValue").gameObject.GetComponent<Text>();
        Text skillCount = canvas.Find("Game/GameUI/GrpFrame/List/SkillCount").gameObject.GetComponent<Text>();
        if (lightTurn)
        {
            fellowName.text = lightFellowList[ClickedPlayerId].fellowName;
            skill.text = lightFellowList[ClickedPlayerId].fellowSkillInfo;
            if (!usingSkill)
            {
                straightSpeed = lightFellowList[ClickedPlayerId].straightSpeed;
                curveSpeed = lightFellowList[ClickedPlayerId].curveSpeed;
            }
            else
            {
                straightSpeed = lightFellowList[ClickedPlayerId].skillValue;
                curveSpeed = lightFellowList[ClickedPlayerId].skillValue;
            }
            skillType.text = "skillType:" + lightFellowList[ClickedPlayerId].skillType;
            skillValue.text = "skilValue:" + lightFellowList[ClickedPlayerId].skillValue;
            skillCount.text = "skillCount:" + lightFellowList[ClickedPlayerId].SkillCount;
        }
        else
        {
            fellowName.text = shadowFellowList[ClickedPlayerId].fellowName;
            skill.text = shadowFellowList[ClickedPlayerId].fellowSkillInfo;
            if (!usingSkill)
            {
                straightSpeed = shadowFellowList[ClickedPlayerId].straightSpeed;
                curveSpeed = shadowFellowList[ClickedPlayerId].curveSpeed;
            }
            else
            {
                straightSpeed = shadowFellowList[ClickedPlayerId].skillValue;
                curveSpeed = shadowFellowList[ClickedPlayerId].skillValue;
            }
            skillType.text = "skillType:" + shadowFellowList[ClickedPlayerId].skillType.ToString();
            skillValue.text = "skilValue:" + shadowFellowList[ClickedPlayerId].skillValue.ToString();
            skillCount.text = "skillCount:" + shadowFellowList[ClickedPlayerId].SkillCount.ToString();
        }
        int xMax = Mathf.Min(x + straightSpeed, 9);
        int xMin = Mathf.Max(x - straightSpeed, 0);
        int yMax = Mathf.Min(y + straightSpeed, 9);
        int yMin = Mathf.Max(y - straightSpeed, 0);
        int xCurveMax = Mathf.Max(x + curveSpeed, 9);
        int xCurveMin = Mathf.Min(x - curveSpeed, 0);
        int yCurveMax = Mathf.Max(y + curveSpeed, 9);
        int yCurveMin = Mathf.Min(y - curveSpeed, 0);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string name = "spot" + i + j;
                GameObject subSpot = FindElementGo(canvasPart, "Game/ChessBoard/" + name);
                Button subBtn = subSpot.GetComponent<Button>();
                int xIndex = i;
                int yIndex = j;
                string spotString = xIndex + "" + yIndex;
                if (subSpot != null)
                {
                    if (i == x && j == y || (lightTurn && (spotState[spotString].FellowType == 1 ||spotState[spotString].FellowType == 3)) || (!lightTurn && (spotState[spotString].FellowType == 2 || spotState[spotString].FellowType == 4)))
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                        subSpot.transform.Find("player").gameObject.SetActive(true);
                        subBtn.onClick.RemoveAllListeners();
                    }
                    else if ((i == x && j <= yMax && j >= yMin && j != y) || (i != x && i <= xMax && i >= xMin && j == y) || (((i - x == j - y) || (i - x == y - j)) && (i - x) * (i - x) <= curveSpeed * curveSpeed && i - x != 0))
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(true);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                        subBtn.onClick.AddListener(() =>
                        {
                            if (lightTurn)
                            {
                                if (spotState[spotString].FellowType == 2)
                                {
                                    OnKillPlayer(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                                else if(spotState[spotString].FellowType == 4)
                                {
                                    OnAttackBuilding(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                                else if (spotState[spotString].FellowType == 0)
                                {
                                    OnMovePlayer(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                            }
                            else
                            {
                                if (spotState[spotString].FellowType == 1)
                                {
                                    OnKillPlayer(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                                else if (spotState[spotString].FellowType == 3)
                                {
                                    OnAttackBuilding(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                                else if (spotState[spotString].FellowType == 0)
                                {
                                    OnMovePlayer(x, y, xIndex, yIndex, ClickedPlayerId);
                                    subBtn.onClick.RemoveAllListeners();
                                }
                            }
                        });
                    }
                    else
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(true);
                        if (canCancle)
                        {
                            subBtn.onClick.AddListener(() =>
                            {
                                CleanMapWithoutSingleSpot(x, y);
                                DrawPlayer();
                            });
                        }
                        else
                        {
                            subBtn.onClick.RemoveAllListeners();
                        }
                    }
                }
            }
        }
    }

    
    bool CheckCanRevive(int killedFellow)
    {
        if (lightTurn)
        {
            int valueCost = shadowFellowList[killedFellow].fellowValue;
            if (ShadowValueNum >= valueCost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            int valueCost = lightFellowList[killedFellow].fellowValue;
            if (LightValueNum >= valueCost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    void OnKillPlayer(int xBefore, int yBefore, int xAfter, int yAfter, int index)
    {
        string name = xAfter + "" + yAfter;
        int killedFellow = spotState[name].FellowId;
        bool canRevive = CheckCanRevive(killedFellow);
        GameObject ReviveCheck = FindElementGo(canvasPart, "Game/GameUI/ReviveCheck");
        if (canRevive)
        {
            ReviveCheck.SetActive(true);
            Button btnYes = FindElementGo(ReviveCheck, "BtnYes").GetComponent<Button>();
            Button btnNo = FindElementGo(ReviveCheck, "BtnNo").GetComponent<Button>();
            btnYes.onClick.RemoveAllListeners();
            btnNo.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener(() =>
            {
                ReviveCheck.SetActive(false);
                if (lightTurn)
                {
                    int valueCost = shadowFellowList[killedFellow].fellowValue;
                    ShadowValueNum = ShadowValueNum - valueCost;
                    shadowFellowSpot[killedFellow] = null;
                    shadowFellowNum--;
                }
                else
                {
                    int valueCost = lightFellowList[killedFellow].fellowValue;
                    LightValueNum = LightValueNum - valueCost;
                    lightFellowSpot[killedFellow] = null;
                    lightFellowNum--;
                }
                Debug.Log("OnKillPlayer:lightFellowNum:" + lightFellowNum + " shadowFellowNum" + shadowFellowNum);
                OnMovePlayer(xBefore, yBefore, xAfter, yAfter, index);
                CheckGameEnd();
                ChooseSpotToRevive(killedFellow);
            });
            btnNo.onClick.AddListener(() =>
            {
                ReviveCheck.SetActive(false);
                if (lightTurn)
                {
                    shadowFellowSpot[killedFellow] = null;
                    shadowFellowNum--;
                }
                else
                {
                    lightFellowSpot[killedFellow] = null;
                    lightFellowNum--;
                }
                OnMovePlayer(xBefore, yBefore, xAfter, yAfter, index);
                CheckGameEnd();
            });
        }
        else
        {
            ReviveCheck.SetActive(false);
            if (lightTurn)
            {
                shadowFellowSpot[killedFellow] = null;
                shadowFellowNum--;
            }
            else
            {
                lightFellowSpot[killedFellow] = null;
                lightFellowNum--;
            }
            OnMovePlayer(xBefore, yBefore, xAfter, yAfter, index);
            CheckGameEnd();
        }
        
    }
    void ChooseSpotToRevive(int id)
    {
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                int x = i;
                int y = j;
                int[] reviveLocation = { x, y };
                string name = i + "" +j;
                Button spot = FindElementGo(canvasPart,"Game/ChessBoard/spot"+name).GetComponent<Button>();
                GameObject canGo = FindElementGo(canvasPart, "Game/ChessBoard/spot" + name + "/CanGo");
                GameObject cannotGo = FindElementGo(canvasPart, "Game/ChessBoard/spot" + name + "/CannotGo");
                if (spotState[name].FellowType == 0)
                {
                    spot.onClick.RemoveAllListeners();
                    spot.onClick.AddListener(() =>
                    {
                        if (lightTurn)
                        {
                            lightFellowSpot[id] = reviveLocation;
                            lightFellowNum++;
                        }
                        else
                        {
                            shadowFellowSpot[id] = reviveLocation;
                            shadowFellowNum++;
                        }
                        Debug.Log("ChooseSpotToRevive:lightFellowNum:" + lightFellowNum+ " shadowFellowNum" + shadowFellowNum);
                        CleanMap();
                        DrawPlayer();
                    });
                    canGo.SetActive(true);
                    cannotGo.SetActive(false);
                }
                else
                {
                    spot.onClick.RemoveAllListeners();
                    canGo.SetActive(false);
                    cannotGo.SetActive(true);
                }
            }
        }
    }
    void CheckGameEnd()
    {
        if(lightFellowNum == 0 || shadowFellowNum == 0 || LightValueNum < 0 || ShadowValueNum < 0)
        {
            OnFinishGame();
        }
    }
    void OnFinishGame()
    {
        if (shadowFellowNum == 0 || ShadowValueNum < 0)
        {
            GameObject lightWin = FindElementGo(canvasPart, "Game/GameUI/LightWin");
            lightWin.SetActive(true);
        }
        else
        {
            GameObject ShadowWin = FindElementGo(canvasPart, "Game/GameUI/ShadowWin");
            ShadowWin.SetActive(true);
        }
        GameObject Cover = FindElementGo(canvasPart, "Game/Cover");
        Cover.SetActive(true);
        Button btnCover = Cover.GetComponent<Button>();
        btnCover.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("init");
        });
    }


    void OnMovePlayer(int xBefore, int yBefore, int xAfter, int yAfter, int ClickedPlayerId)
    {
        string name = "spot" + xBefore + yBefore;
        GameObject player = FindElementGo(chessBoard, name);
        if (player != null)
        {
            player.transform.Find("player").gameObject.SetActive(false);
        }
        int[] after = { xAfter, yAfter };
        bool canUseSkill = false;
        if (lightTurn)
        {
            lightFellowSpot[ClickedPlayerId] = after;
            if (lightFellowList[ClickedPlayerId].SkillCount > 1)
            {
                canUseSkill = true;
            }
        }
        else
        {
            shadowFellowSpot[ClickedPlayerId] = after;
            if (shadowFellowList[ClickedPlayerId].SkillCount > 1)
            {
                canUseSkill = true;
            }
        }
        if (canUseSkill)
        {
            CheckCanUseSkill(after, ClickedPlayerId);
        }
        else
        {
            lightTurn = !lightTurn;
            CleanMapWithoutSingleSpot(xAfter, yAfter);
            DrawPlayer();
        }
    }

    void OnAttackBuilding(int xBefore, int yBefore, int xAfter, int yAfter, int ClickedPlayerId)
    {
        string name = "spot" + xBefore + yBefore;
        GameObject player = FindElementGo(chessBoard, name);
        if (player != null)
        {
            player.transform.Find("player").gameObject.SetActive(false);
        }
        string buildingnName = xAfter + "" + yAfter;
        name = "spot" + xAfter + yAfter;
        GameObject fire = FindElementGo(canvasPart, "Game/ChessBoard/" + name + "/fire");
        if(spotState[buildingnName].burning == false)
        {
            fire.SetActive(true);
            spotState[buildingnName].burning = true;
        }
        else
        {
            fire.SetActive(false);
            spotState[buildingnName].burning = false;
            if (lightTurn)
            {
                ShadowValueNum = ShadowValueNum - 3;
            }
            else
            {
                LightValueNum = LightValueNum - 3;
            }
        }
        bool canUseSkill = false;
        if (lightTurn)
        {
            int[] after = LightSpot[ClickedPlayerId].bb;
            lightFellowSpot[ClickedPlayerId] = after;
            if (lightFellowList[ClickedPlayerId].SkillCount > 1)
            {
                canUseSkill = true;
            }
            if (canUseSkill)
            {
                CheckCanUseSkill(after, ClickedPlayerId);
            }
            else
            {
                lightTurn = !lightTurn;
                CleanMapWithoutSingleSpot(xAfter, yAfter);
                DrawPlayer();
            }
        }
        else
        {
            int[] after = ShadowSpot[ClickedPlayerId].bb;
            shadowFellowSpot[ClickedPlayerId] = after;
            if (shadowFellowList[ClickedPlayerId].SkillCount > 1)
            {
                canUseSkill = true;
            }
            if (canUseSkill)
            {
                CheckCanUseSkill(after, ClickedPlayerId);
            }
            else
            {
                lightTurn = !lightTurn;
                CleanMapWithoutSingleSpot(xAfter, yAfter);
                DrawPlayer();
            }
        }
        CheckGameEnd();
    }

    void CheckCanUseSkill(int[]after, int ClickedPlayerId)
    {
        int skillType;
        int skillValue;
        int skillCount;
        if (lightTurn)
        {
            skillType = lightFellowList[ClickedPlayerId].skillType;
            skillValue = lightFellowList[ClickedPlayerId].skillValue;
            skillCount = lightFellowList[ClickedPlayerId].SkillCount;
        }
        else
        {
            skillType = shadowFellowList[ClickedPlayerId].skillType;
            skillValue = shadowFellowList[ClickedPlayerId].skillValue;
            skillCount = shadowFellowList[ClickedPlayerId].SkillCount;
        }
        GameObject SkillCheck = FindElementGo(canvasPart, "Game/GameUI/SkillCheck");
        SkillCheck.SetActive(true);
        Button btnYes = FindElementGo(SkillCheck, "BtnYes").GetComponent<Button>();
        Button btnNo = FindElementGo(SkillCheck, "BtnNo").GetComponent<Button>();
        btnYes.onClick.RemoveAllListeners();
        btnNo.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            SkillCheck.SetActive(false);
            if (lightTurn)
            {
                if (lightFellowList[ClickedPlayerId].SkillCount > 0)
                {
                    CleanMap();
                    DrawPlayer();
                    OnClickPlayer(after[0],after[1], ClickedPlayerId,false,true);
                    lightFellowList[ClickedPlayerId].SkillCount = lightFellowList[ClickedPlayerId].SkillCount -1;
                }
                else
                {
                    Debug.LogError("出问题啦");
                }
            }
            else
            {
                if (shadowFellowList[ClickedPlayerId].skillCount > 0)
                {
                    CleanMap();
                    DrawPlayer();
                    OnClickPlayer(after[0], after[1], ClickedPlayerId,false,true);
                    shadowFellowList[ClickedPlayerId].SkillCount = shadowFellowList[ClickedPlayerId].SkillCount - 1;
                }
                else
                {
                    Debug.LogError("出问题啦");
                }
            }
        });
        btnNo.onClick.AddListener(() =>
        {
            SkillCheck.SetActive(false);
            lightTurn = !lightTurn;
            CleanMap();
            DrawPlayer();
        });
    }

    void CleanMapWithoutSingleSpot(int x, int y)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string name = "spot" + i + j;
                GameObject subSpot = FindElementGo(canvasPart, "Game/ChessBoard/" + name);
                Button subBtn = subSpot.GetComponent<Button>();
                if (subSpot != null)
                {
                    if (i == x && j == y)
                    {

                    }
                    else
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                        subSpot.transform.Find("player").gameObject.SetActive(false);
                        subBtn.onClick.RemoveAllListeners();
                    }
                }
            }
        }
    }
    void CleanMap()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string name = "spot" + i + j;
                GameObject subSpot = FindElementGo(canvasPart, "Game/ChessBoard/" + name);
                Button subBtn = subSpot.GetComponent<Button>();
                if (subSpot != null)
                {
                    subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                    subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                    subSpot.transform.Find("player").gameObject.SetActive(false);
                    subBtn.onClick.RemoveAllListeners();
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
public class spotInfo
{
    public int FellowType = 0;//该点是否由英雄及英雄所在阵营 0为无英雄，1为光阵容英雄，2为影阵容英雄,3为光阵容基地，4为影阵容基地
    public int FellowId = 0;
    public bool burning = false;
}
