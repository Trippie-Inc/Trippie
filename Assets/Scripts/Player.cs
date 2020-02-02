using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public GameObject Laser;
    public Animator Anim;
    public GameObject GameManager;
    public GameObject AudioManager;
    public GameObject NetworkManager;
    public Text PointsUI;
    public Text PlayerNameUI;
    public Image TeamColorUI;
    public float ShootRange = 100.0f;
    public Camera PlayerCam;
    private float translation;
    private float straffe;
    private float Minutes;
    private float Seconds;
    [SyncVar] public GameObject Killed;
    [SyncVar] public float GameTime;
    [SyncVar] public int Points;
    [SyncVar] public string Player_Name;
    [SyncVar] public string Team;
    public bool GameTimerPause = false;
    public Vector3 jump;
    public float jumpForce = 2.0f;
    private float Speed;
    public float WalkSpeed = 10.0f;
    public float SprintSpeed = 10.0f;
    public float StraffeSpeed = 5.0f;
    public float BackwardSpeed = 5.0f;
    public bool isGrounded;
    Rigidbody rb;
    private NetworkStartPosition[] AllSpawnPoints;

    void Start()
    {
        AudioManager = GameObject.Find("AudioManager");
        NetworkManager = GameObject.FindGameObjectWithTag("Network Manager");
        Player_Name = NetworkManager.GetComponent<NetworkManagerCustom>().playername;
        CmdPlayer(Player_Name);
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        Cursor.lockState = CursorLockMode.Locked;
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        PointsUI = GameObject.FindGameObjectWithTag("Counter").GetComponent<Text>();
        GameManager.GetComponent<Trippie>().HostPlayer = GameManager.GetComponent<Trippie>().Players[0];
        if (isServer)
        {
            StartTimer(300f);
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.collider.gameObject.name == "Ground")
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        AllSpawnPoints = FindObjectsOfType<NetworkStartPosition>();
        Minutes = Mathf.Floor(GameTime / 60);
        Seconds = Mathf.Floor(GameTime - (Mathf.Floor(GameTime / 60) * 60));
        if (Seconds < 10)
        {
            GameManager.GetComponent<Trippie>().GameTimerUI.text = Minutes.ToString() + ":" + 0.ToString() + Seconds.ToString();
        }
        else
        {
            GameManager.GetComponent<Trippie>().GameTimerUI.text = Minutes.ToString() + ":" + Seconds.ToString();
        }

        if (!isServer)
        {
            GameTime = GameManager.GetComponent<Trippie>().HostPlayer.GetComponent<Player>().GameTime;
        }

        foreach (GameObject Player in GameManager.GetComponent<Trippie>().Players)
        {
            PlayerNameUI = Player.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>();
            PlayerNameUI.text = Player.GetComponent<Player>().Player_Name;

            if (isServer)
            {
                CheckForRespawn(Player);
            }

            TeamColorUI = Player.transform.Find("Canvas").transform.Find("Team").GetComponent<Image>();

            if (Player.GetComponent<Player>().Team == "Blue")
            {
                TeamColorUI.color = new Color32(0, 62, 225, 255);
            }
            if (Player.GetComponent<Player>().Team == "Red")
            {
                TeamColorUI.color = new Color32(255, 0, 0, 255);
            }
        }

        if (Team == "Red")
        {
            GameManager.GetComponent<Trippie>().TeamPanel.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        }
        else
        {
            GameManager.GetComponent<Trippie>().TeamPanel.GetComponent<Image>().color = new Color32(0, 62, 225, 255);
        }

        PointsUI.text = Points.ToString();

        translation = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * StraffeSpeed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (straffe > 0)
        {
            Anim.SetBool("D", true);
            //Right
        }
        else
        {
            Anim.SetBool("D", false);
        }

        if (straffe < 0)
        {
            Anim.SetBool("A", true);
            //Left
        }
        else
        {
            Anim.SetBool("A", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Anim.SetBool("S", true);
            //Backwards
            Speed = BackwardSpeed;
        }
        else
        {
            Anim.SetBool("S", false);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {

            FindObjectOfType<AudioManager>().PlaySound("Walk");
        }

        if (Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            FindObjectOfType<AudioManager>().StopSound("Walk");
        }

        if (Input.GetKey(KeyCode.W))
        {
            Anim.SetBool("W", true);
            //Walking
            Speed = WalkSpeed;
        }
        else
        {
            Anim.SetBool("W", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            FindObjectOfType<AudioManager>().PlaySound("Run");
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            FindObjectOfType<AudioManager>().StopSound("Run");
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            //Sprinting
            Speed = SprintSpeed;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Anim.SetBool("Shift", true);
        }
        else
        {
            Anim.SetBool("Shift", false);
        }

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetGame();
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            FindObjectOfType<AudioManager>().PlaySound("Jump");
            isGrounded = false;
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void ResetGame()
    {
        foreach (GameObject _Player in GameManager.GetComponent<Trippie>().Players)
        {
            _Player.GetComponent<Player>().MoveTo(AllSpawnPoints[Random.Range(0, AllSpawnPoints.Length)].transform.position);
        }
    }

    void CheckForRespawn(GameObject _Player)
    {
        if (_Player.GetComponent<Player>().Killed != null)
        {
            if (_Player.GetComponent<Player>().Killed == gameObject)
            {
                gameObject.transform.position = AllSpawnPoints[Random.Range(0, AllSpawnPoints.Length)].transform.position;
                _Player.GetComponent<Player>().Killed = null;
                Debug.Log("Killed Host");
            }
            else
            {
                _Player.GetComponent<Player>().Killed.GetComponent<Player>().MoveTo(AllSpawnPoints[Random.Range(0, AllSpawnPoints.Length)].transform.position);
                _Player.GetComponent<Player>().Killed = null;
                Debug.Log("Killed Client");
            }
        }
    }

    void StartTimer(float _Time)
    {
        GameTime = _Time;
        Timer();
    }

    void PauseTimer()
    {
        GameTimerPause = !GameTimerPause;
    }

    void ResetTimer(float _Time)
    {
        GameTime = _Time;
        ResetGame();
        Timer();
    }

    void Timer()
    {
        if (!GameTimerPause)
        {
            if (GameTime < 0)
            {
                ResetTimer(300f);
            }
            else
            {
                StartCoroutine(Wait1Second());
            }
        }
    }

    IEnumerator Wait1Second()
    {
        yield return new WaitForSeconds(1);
        GameTime -= 1;
        Timer();
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(GameObject.FindGameObjectWithTag("Laser"), 0);
    }

    void Shoot()
    {
        FindObjectOfType<AudioManager>().PlaySound("Shoot");

        RaycastHit hit;
        if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out hit, ShootRange))
        {
            GameObject _Laser = Instantiate(Laser);
            _Laser.GetComponent<LineRenderer>().SetPosition(0, PlayerCam.transform.position);
            _Laser.GetComponent<LineRenderer>().SetPosition(1, hit.point);

            StartCoroutine(WaitForDestroy());

            Debug.Log(hit.transform.name);
            if (hit.transform.tag == "Player")
            {
                if (hit.transform.GetComponent<Player>().Team != Team)
                {
                    FindObjectOfType<AudioManager>().PlaySound("Kill");

                    Points += 1;
                    if (!isServer)
                    {
                        CmdPoints(Points);
                    }

                    Killed = hit.transform.gameObject;

                    if (!isServer)
                    {
                        CmdRespawn(Killed);
                    }
                }
            }
        }
    }

    [Command]
    public void CmdPoints(int CMDPoints)
    {
        Points = CMDPoints;
    }

    [Command]
    public void CmdPlayer(string CMDPlayerName)
    {
        Player_Name = CMDPlayerName;
    }

    [Command]
    public void CmdRespawn(GameObject CMDNeedRespawn)
    {
        Killed = CMDNeedRespawn;
    }

    [Server]
    public void MoveTo(Vector3 newPosition)
    {
        transform.position = newPosition;
        FindObjectOfType<AudioManager>().PlaySound("Death");
        RpcMoveTo(newPosition);
    }

    [ClientRpc]
    void RpcMoveTo(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}