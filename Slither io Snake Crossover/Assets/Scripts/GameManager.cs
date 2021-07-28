using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject snakePrefab;
    public GameObject snake;
    public GameObject fruit;
    public Text punctuation;

    public GameObject startPanel;
    public GameObject endPanel;
    public GameObject connectingPanel;
    public GameObject connectionFailedPanel;
    public Text endPoints;
    public Text maxPointsText;

    public BSPGenerator bsp;
    private int points;
    public int maxPoints;

    private static bool isLogged = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        ServerLogin();
        Time.timeScale = 0.0f;
        //snake.transform.position = bsp.SpawnPoint();
        //snake.transform.SetAsFirstSibling();
    }

    private void Start()
    {
        points = -2;
        GenerateFruit();
        GenerateFruit();
        snake = Instantiate(snakePrefab, bsp.SpawnPoint(), Quaternion.identity);
        if (bsp.FirstRoomIsWider())
        {
            snake.GetComponent<Snake>().LookRight();
        }
        else
        {
            snake.GetComponent<Snake>().LookUpwards();
        }
    }

    public void GenerateFruit()
    {
        Instantiate(fruit, bsp.GetRandomTile(), Quaternion.identity);
        points++;
        punctuation.text = "Points: " + points.ToString();
    }

    public void StartMatch()
    {
        if (isLogged)
        {
            Time.timeScale = 1.0f;
            startPanel.SetActive(false);
            PlayFabManager.instance.GetStatistics();
        }
    }

    public void EndMatch()
    {
        PlayFabManager.instance.SubmitStatistics(points);
        Time.timeScale = 0.0f;
        endPoints.text = punctuation.text;
        endPanel.SetActive(true);
        maxPointsText.text = "Highscore: " + maxPoints;
    }

    public void ReloadScene()
    {
        endPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ServerLogin()
    {
        if (!isLogged)
        {
            connectingPanel.SetActive(true);
            PlayFabManager.instance.AndroidLogin(OnAndroidLoginSuccess, OnAndroidLoginFailed);
        }
    }

    private void OnAndroidLoginSuccess(LoginResult loginResult)
    {
        connectingPanel.SetActive(false);
        Debug.Log("Android Login: " + loginResult.PlayFabId);
        isLogged = true;
        PlayFabManager.instance.GetStatistics();
    }
    private void OnAndroidLoginFailed(PlayFabError error)
    {
        Debug.Log("Android Login: " + error.GenerateErrorReport());
        PlayFabManager.instance.Login(OnLoginSuccess, OnLoginFailed);
    }

    private void OnLoginSuccess(LoginResult loginResult)
    {
        connectingPanel.SetActive(false);
        Debug.Log("Login: " + loginResult.PlayFabId);
        isLogged = true;
        PlayFabManager.instance.GetStatistics();
    }
    private void OnLoginFailed(PlayFabError error)
    {
        connectionFailedPanel.SetActive(true);
        connectingPanel.SetActive(false);
        Debug.Log("Login: " + error.GenerateErrorReport());
        isLogged = false;
    }

}
