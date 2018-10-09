using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour {

    public List<int[]> spot = new List<int[]>();
    public Transform canvas;
    GameObject canvasPart;
    GameObject chessBoard;

    // Use this for initialization
    void Start () {
        Initialise();
	}
	
    void Initialise()
    {
        Button BtnBack = canvas.Find("Game/BtnBack").gameObject.GetComponent<Button>();
        BtnBack.onClick.AddListener(() =>
        {
            Debug.Log("点击开始游戏");
            SceneManager.LoadScene("init");
        });
        DrawChessBoard();
        int[] demoSpot = { 2, 3 };
        spot.Add(demoSpot);
        DrawPlayer();
    }

    void DrawChessBoard()
    {
        chessBoard = canvas.Find("Game/ChessBoard").gameObject;
        GameObject spot = canvas.Find("Game/ChessBoard/spot").gameObject;
        canvasPart = canvas.gameObject;
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                string name = "spot" + i + j;
                GameObject subSpot = FindElementGo(canvasPart, "Game/ChessBoard/" + name);
                if(subSpot == null)
                {
                    subSpot = GameObject.Instantiate(spot);
                    subSpot.transform.SetParent(chessBoard.transform);
                    subSpot.transform.localScale = Vector3.one;
                    subSpot.name = name;
                    subSpot.transform.localPosition = new Vector3(-270+i*60,-270+j*60,0);
                    subSpot.SetActive(true);
                }
            }
        }
        spot.SetActive(false);

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
        int[] playerSpot = spot[0];
        int x = playerSpot[0];
        int y = playerSpot[1];
        string name = "spot" + x + y;
        GameObject player = FindElementGo(chessBoard, name);
        if (player != null)
        {
            player.transform.Find("CanGo").gameObject.SetActive(true);
            player.transform.Find("CannotGo").gameObject.SetActive(false);
            player.transform.Find("player").gameObject.SetActive(true);
        }
        Button btnPlayer = player.GetComponent<Button>();
        btnPlayer.onClick.AddListener(() =>
        {
            OnClickPlayer(x,y);
        });
    }

    void OnClickPlayer(int x,int y)
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
                if (subSpot != null)
                {
                    if(i == x && j == y)
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                        subSpot.transform.Find("player").gameObject.SetActive(true);
                        subBtn.onClick.RemoveAllListeners();
                    }
                    else if((i == x && j <= yMax && j >= yMin && j != y) || (i != x && i <= xMax && i >= xMin && j == y)||( ((i - x == j - y)||(i - x == y - j)) && (i - x) * (i - x) <= curveSpeed * curveSpeed && i - x != 0))
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(true);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(false);
                        subSpot.transform.Find("player").gameObject.SetActive(false);
                        subBtn.onClick.AddListener(() =>
                        {
                            OnMovePlayer(x, y, xIndex, yIndex);
                        });
                    }
                    else
                    {
                        subSpot.transform.Find("CanGo").gameObject.SetActive(false);
                        subSpot.transform.Find("CannotGo").gameObject.SetActive(true);
                        subSpot.transform.Find("player").gameObject.SetActive(false);
                        subBtn.onClick.AddListener(() =>
                        {
                            DrawPlayer();
                            CleanMapWithoutSingleSpot(x, y);
                        });

                    }
                    
                }
            }
        }
    }

    void OnMovePlayer(int xBefore, int yBefore, int xAfter, int yAfter)
    {
        string name = "spot" + xBefore + yBefore;
        GameObject player = FindElementGo(chessBoard, name);
        if (player != null)
        {
            player.transform.Find("player").gameObject.SetActive(false);
        }
        int[] after = { xAfter, yAfter };
        spot[0] = after;
        DrawPlayer();
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
