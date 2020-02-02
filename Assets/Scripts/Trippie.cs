using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class Trippie : MonoBehaviour
{
    public List<GameObject> Objects = new List<GameObject>();
    public GameObject[] Players;
    public GameObject[] RefPlayers;
    public GameObject[] RedTeam;
    public GameObject[] BlueTeam;
    public GameObject[] Spectators;
    public GameObject HostPlayer;
    public int RedTeamPoints = 0;
    public int BlueTeamPoints = 0;
    public Text RedTeamText;
    public Text BlueTeamText;
    public Text GameTimerUI;
    public Text PointsUI;
    public GameObject TeamPanel;

    void Start()
    {
        UpdatePlayerList();
    }

    void Update()
    {
        RefPlayers = null;
        RefPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (RefPlayers.Length != Players.Length)
        {
            UpdatePlayerList();
        }

        foreach (GameObject Object in Objects)
        {
            LookerFunction(Object);
        }

        BlueTeamPoints = CountPoints(BlueTeam);
        RedTeamPoints = CountPoints(RedTeam);

        RedTeamText.GetComponent<Text>().text = RedTeamPoints.ToString();
        BlueTeamText.GetComponent<Text>().text = BlueTeamPoints.ToString();
    }

    private void LookerFunction(GameObject _Object)
    {
        var _Visible = _Object.GetComponent<Spefication>().VisibleState;
        if (_Object.GetComponent<Spefication>().Looker.GetComponent<Renderer>().isVisible)
        {
            if (!_Visible)
            {
                ChangeObjectState(_Object);
            }
            _Object.GetComponent<Spefication>().VisibleState = true;
        }
        else
        {
            _Object.GetComponent<Spefication>().VisibleState = false;
        }
    }

    private void ChangeObjectState(GameObject _Object)
    {
        if (_Object.GetComponent<Spefication>().TrippieState)
        {
            if (_Object.GetComponent<Spefication>().TrueMaterialFalsePosition == true)
            {
                _Object.GetComponent<Renderer>().material = _Object.GetComponent<Spefication>().Color2;
            }
            else
            {
                _Object.transform.position = new Vector3(_Object.transform.position.x, _Object.GetComponent<Spefication>().ChangeY, _Object.transform.position.z);
            }
        }
        else
        {
            if (_Object.GetComponent<Spefication>().TrueMaterialFalsePosition == true)
            {
                _Object.GetComponent<Renderer>().material = _Object.GetComponent<Spefication>().Color1;
            }
            else
            {
                _Object.transform.position = new Vector3(_Object.transform.position.x, _Object.GetComponent<Spefication>().OriginalY, _Object.transform.position.z);
            }
        }

        _Object.GetComponent<Spefication>().TrippieState = !_Object.GetComponent<Spefication>().TrippieState;
    }

    public void AddPoint(int Points)
    {
        BlueTeamPoints += Points;
    }

    public void UpdatePlayerList()
    {
        Players = null;
        Players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Player Joined!");
        MakeTeams();
        foreach (GameObject Player in RedTeam)
        {
            Player.GetComponent<Player>().Team = "Red";
        }
        foreach (GameObject Player in BlueTeam)
        {
            Player.GetComponent<Player>().Team = "Blue";
        }
    }

    public void MakeTeams()
    {
        RedTeam = Players.Take(Mathf.RoundToInt(Players.Length / 2)).ToArray();
        BlueTeam = Players.Skip(Mathf.RoundToInt(Players.Length / 2)).Take(Players.Length - Mathf.RoundToInt(Players.Length / 2)).ToArray();
    }

    public int CountPoints(GameObject[] CountList)
    {
        int OverallPoints = 0;
        foreach (GameObject Player in CountList)
        {
            OverallPoints += Player.GetComponent<Player>().Points;
        }
        return OverallPoints;
    }
}
