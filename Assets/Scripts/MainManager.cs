using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Data;
using System.Drawing;


public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    public TMP_Text bestScoreText;
    [SerializeField] string actualPlayer;
    [SerializeField] string bestPlayer;
    [SerializeField] int maxPoints;


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        LoadRecord();
        bestScoreText.text = "Best Score: " + bestPlayer + " " + maxPoints;
    }

    private void Update()
    {

        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            SaveRecord();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;

        if (m_Points > maxPoints)
        {
            bestPlayer = actualPlayer;
            maxPoints = m_Points;
            bestScoreText.text = "Best Score : " + bestPlayer + " " + maxPoints;
            ScoreText.text = "Score " + bestPlayer + " : " + maxPoints;
        }
        else
        {
            bestScoreText.text = "Best Score: " + bestPlayer + " " + maxPoints;
            ScoreText.text = "Score " + actualPlayer + ": " + m_Points;
        }
    }
    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    [System.Serializable]
    class SaveData
    {
        //[System.Serializable] encima de ella. Esta línea es requerida para JsonUtility
        public int record;
        public string name;
        public string newPlayer;
    }

    public void SaveRecord()
    {
        //rehase una nueva instancia de los datos guardados y rellenaste su miembro de la clase
        SaveData data = new SaveData();
        data.newPlayer = actualPlayer;
        data.name = bestPlayer;
        data.record = maxPoints;
        //transformaste esa instancia de JSON con JsonUtility.ToJson
        string json = JsonUtility.ToJson(data);
        //usaste el método especial File.WriteAllText para escribir una secuencia de caracteres a un archivo
        //Usaste un método de Unity llamado Application.persistentDataPath que te dará una carpeta donde podrás guardar datos que sobrevivirán
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadRecord()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        //Utiliza el método File.Exists para verificar si existe un archivo .json
        if (File.Exists(path))
        {
            //Si el archivo existe, entonces el método leerá su contenido con File.ReadAllText
            string json = File.ReadAllText(path);
            //dará el texto resultante a JsonUtility.FromJson para transformarlo nuevamente en una instancia de SaveData
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            //Por último, definiremos el TeamColor al color guardado en ese SaveData
            actualPlayer = data.newPlayer;
            bestPlayer = data.name;
            maxPoints = data.record;
        }
    }
}
