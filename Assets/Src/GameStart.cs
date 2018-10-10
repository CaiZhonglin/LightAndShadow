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

    public List<int[]> lightSpot = new List<int[]>();
    public List<int[]> shadowSpot = new List<int[]>();
    public Transform canvas;
    public List<aa> Light;






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

        lightTurn = true;
        ShowTurnMsg();
        DrawChessBoard();
        int[] demoSpot1 = { 2, 3 };
        int[] demoSpot2 = { 3, 4 };
        int[] demoSpot3 = { 8, 9 };
        int[] demoSpot4 = { 7, 8 };
        lightSpot.Add(demoSpot1);
        lightSpot.Add(demoSpot2);
        shadowSpot.Add(demoSpot3);
        shadowSpot.Add(demoSpot4);
        lightFellowNum = lightSpot.Count;
        shadowFellowNum = shadowSpot.Count;
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
        Text turn = canvas.Find("Game/GameUI/GrpFrame/Turn").gameObject.GetComponent<Text>();
        if (lightTurn)
        {
            turn.text = "Light Turn";
        }
        else
        {
            turn.text = "Shadow Turn";
        }

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
        for (int a = 0; a < 10; a++)
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
        for(int i = 0; i < lightSpot.Count; i++)
        {
            if(lightSpot[i] != null)
            {
                int[] playerSpot = lightSpot[i];
                int index = i;
                Debug.Log("111spotState[playerSpot]为" + spotState[(playerSpot[0] + "" + playerSpot[1])]);
                spotState[(playerSpot[0] + "" + playerSpot[1])].FellowType = 1;
                spotState[(playerSpot[0] + "" + playerSpot[1])].FellowId = index;
                Debug.Log("222spotState[playerSpot]为" + spotState[(playerSpot[0] + "" + playerSpot[1])]);
                int x = playerSpot[0];
                int y = playerSpot[1];
                string name = "spot" + x + y;
                GameObject player = FindElementGo(chessBoard, name);
                if (player != null)
                {
                    Image icon = player.transform.Find("player").gameObject.GetComponent<Image>();
                    icon.sprite = Resources.Load<Sprite>("UIRes/fellowIcon/LightIcon1");
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

        for (int i = 0; i < shadowSpot.Count; i++)
        {
            if (shadowSpot[i] != null)
            {
                int[] playerSpot2 = shadowSpot[i];
                int index = i;
                Debug.Log("111spotState[playerSpot2]为" + spotState[(playerSpot2[0] + "" + playerSpot2[1])]);
                spotState[(playerSpot2[0] + "" + playerSpot2[1])].FellowType = 2;
                spotState[(playerSpot2[0] + "" + playerSpot2[1])].FellowId = index;
                Debug.Log("222spotState[playerSpot2]为" + spotState[(playerSpot2[0] + "" + playerSpot2[1])]);
                int x = playerSpot2[0];
                int y = playerSpot2[1];
                string name2 = "spot" + x + y;
                GameObject player2 = FindElementGo(chessBoard, name2);
                if (player2 != null)
                {
                    Image icon = player2.transform.Find("player").gameObject.GetComponent<Image>();
                    icon.sprite = Resources.Load<Sprite>("UIRes/fellowIcon/ShadowIcon1");
                    player2.transform.Find("CannotGo").gameObject.SetActive(false);
                    player2.transform.Find("player").gameObject.SetActive(true);
                    Button btnPlayer = player2.GetComponent<Button>();
                    if (!lightTurn)
                    {
                        player2.transform.Find("CanGo").gameObject.SetActive(true);
                        btnPlayer.onClick.AddListener(() =>
                        {
                            OnClickPlayer(x, y, index);
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
    }

    void OnClickPlayer(int x, int y, int index)
    {
        int straightSpeed = 3;
        int curveSpeed = 1;
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
                    if (i == x && j == y || (lightTurn && spotState[spotString].FellowType == 1) || (!lightTurn && spotState[spotString].FellowType == 2))
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
                                Debug.Log("spotState长度为" + spotState.Count);
                                if (spotState[spotString].FellowType == 2)
                                {
                                    OnKillPlayer(x, y, xIndex, yIndex, index);
                                }
                                else if (spotState[spotString].FellowType == 0)
                                {
                                    OnMovePlayer(x, y, xIndex, yIndex, index);
                                }
                            }
                            else
                            {
                                if (spotState[spotString].FellowType == 1)
                                {
                                    OnKillPlayer(x, y, xIndex, yIndex, index);
                                }
                                else if (spotState[spotString].FellowType == 0)
                                {
                                    OnMovePlayer(x, y, xIndex, yIndex, index);
                                }
                            }
                        });
                    }
                    else
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(true);
                        subBtn.onClick.AddListener(() =>
                        {
                            CleanMapWithoutSingleSpot(x, y);
                            DrawPlayer();
                        });

                    }

                }
            }
        }
    }

    void OnKillPlayer(int xBefore, int yBefore, int xAfter, int yAfter, int index)
    {
        string name = xAfter + "" + yAfter;
        int killedFellow = spotState[name].FellowId;
        if (lightTurn)
        {
            shadowSpot[killedFellow] = null;
            shadowFellowNum--;
        }
        else
        {
            lightSpot[killedFellow] = null;
            lightFellowNum--;
        }
        OnMovePlayer(xBefore, yBefore, xAfter, yAfter, index);
        CheckGameEnd();
    }
    void CheckGameEnd()
    {
        if(lightFellowNum == 0 || shadowFellowNum == 0)
        {
            OnFinishGame();
        }
    }
    void OnFinishGame()
    {
        if (shadowFellowNum == 0)
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


    void OnMovePlayer(int xBefore, int yBefore, int xAfter, int yAfter, int index)
    {
        string name = "spot" + xBefore + yBefore;
        GameObject player = FindElementGo(chessBoard, name);
        if (player != null)
        {
            player.transform.Find("player").gameObject.SetActive(false);
        }
        int[] after = { xAfter, yAfter };
        if (lightTurn)
        {
            lightSpot[index] = after;
        }
        else
        {
            shadowSpot[index] = after;
        }
        lightTurn = !lightTurn;
        ShowTurnMsg();
        //string name = "spot" + xBefore + yBefore;
        //GameObject player = FindElementGo(chessBoard, name);
        //if (player != null)
        //{
        //    player.transform.Find("CanGo").gameObject.SetActive(false);
        //    player.transform.Find("CannotGo").gameObject.SetActive(false);
        //    player.transform.Find("player").gameObject.SetActive(false);
        //}
        //Button btnPlayer = player.GetComponent<Button>();
        //btnPlayer.onClick.RemoveAllListeners();

        //name = "spot" + xAfter + yAfter;
        //player = FindElementGo(chessBoard, name);
        //if (player != null)
        //{
        //    player.transform.Find("CanGo").gameObject.SetActive(true);
        //    player.transform.Find("CannotGo").gameObject.SetActive(false);
        //    player.transform.Find("player").gameObject.SetActive(true);
        //}
        //btnPlayer = player.GetComponent<Button>();
        //btnPlayer.onClick.AddListener(() =>
        //{
        //    OnClickPlayer(xAfter, yAfter);
        //});
        CleanMapWithoutSingleSpot(xAfter, yAfter);
        DrawPlayer();
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
                        subBtn.onClick.RemoveAllListeners();
                    }
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
    public int FellowType = 0;//该点是否由英雄及英雄所在阵营 0为无英雄，1为光阵容英雄，2为影阵容英雄
    public int FellowId = 0;
}
