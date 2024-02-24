using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MenuUIHandler : MonoBehaviour
{
    public static MenuUIHandler menuUIHandler;
    //Definimos la variable input text mesh pro
    public TMP_InputField inputField;

    //Serializamos el string player para compartirlo
    [SerializeField] string playerName;
    public TMP_Text bestScore;
    [SerializeField] string bestPlayer;
    [SerializeField] int bestRecord;


    void Awake()
    {
        LoadRecord();
        bestScore.text = "Best Score: " + bestPlayer + " " + bestRecord;
    }

    public void PlayerName()
    {
        playerName = inputField.text;
        Debug.Log("playerName is " + playerName);
    }
    public void StartNew()
    {
        SaveRecord();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif    
       // MainManager.Instance.SaveColor();
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
        //reaste una nueva instancia de los datos guardados y rellenaste su miembro de la clase
        SaveData data = new SaveData();
        data.newPlayer = playerName;
        data.name = bestPlayer;
        data.record = bestRecord;

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
            bestPlayer = data.name;
            bestRecord= data.record;
        }
    }

}
