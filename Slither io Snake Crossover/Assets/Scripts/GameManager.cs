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
    public Text endPoints;

    private BSPGenerator bsp;
    private int points;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
        //startPanel.SetActive(true);
        bsp = FindObjectOfType<BSPGenerator>();
        snake = Instantiate(snakePrefab, bsp.SpawnPoint(), Quaternion.identity);
        //snake.transform.position = bsp.SpawnPoint();
        //snake.transform.SetAsFirstSibling();
        points = -2;
        GenerateFruit();
        GenerateFruit();
    }

    public void GenerateFruit()
    {
        Instantiate(fruit, bsp.GetRandomTile(), Quaternion.identity);
        points++;
        punctuation.text = "Points: " + points.ToString();
    }

    public void StartMatch()
    {
        Time.timeScale = 1.0f;
        startPanel.SetActive(false);
    }

    public void EndMatch()
    {
        Time.timeScale = 0.0f;
        endPoints.text = punctuation.text;
        endPanel.SetActive(true);
    }

    public void ReloadScene()
    {
        endPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
