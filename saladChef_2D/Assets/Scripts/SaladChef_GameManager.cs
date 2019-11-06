using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using MiniJSON;
using System.IO;
using System.Linq;

public class SaladChef_GameManager : MonoBehaviour
{
    public int ScorePoints = 1;

    public float TimeLeft = 0;
    public float BonusTime = 5f;

    public bool gameOver = false;

    public Text PlayerA_Timer;
    public Text PlayerB_Timer;
    public Text ResultText;

    public GameObject EndScreen;
    public GameObject HighScoreUI;
    public GameObject ScoreUI;

    public Transform CustomerTable;
    public Transform ScrollRectContentParent;

    public List<string> VegetableList = new List<string>();

    private Player[] players;

    public List<Transform> SeatPositions = new List<Transform>();
    public List<Transform> tempTransList;

    [SerializeField]
    private List<int> tempIntList = new List<int>();
    [SerializeField]
    private List<Highscore> highscoreList = new List<Highscore>();
    private List<KeyValuePair<string, object>> kvpList;

    public delegate void DestroyCustomers();
    public event DestroyCustomers KillCustomers;

    private void Awake()
    {
        tempTransList = new List<Transform>(SeatPositions);
        players = FindObjectsOfType<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadTheHighScoresOnGameStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Allocating a seat to the customer
    /// </summary>
    /// <returns></returns>
    public Transform Alot_A_Seat_For_The_Customer()
    {
        if (tempTransList.Count > 0)
        {
            int randomNo = Random.Range(0, tempTransList.Count);
            return tempTransList[randomNo];
        }

        return null;
    }

    /// <summary>
    /// Remove the seat occupied from the templist 
    /// trans - customer position object
    /// </summary>
    /// <param name="trans"></param>
    public void RemoveReservedSeatFromList(Transform trans)
    {
        tempTransList.Remove(trans);
    }

    /// <summary>
    /// Adding back the seat that the customer left to the temporary seat list
    /// </summary>
    /// <param name="trans"></param>
    public void AddTheFreeSeatBackToTheList(Transform trans)
    {
        tempTransList.Add(trans);
    }

    /// <summary>
    /// Checking if the delivered salad combo to the customer is correct
    /// </summary>
    /// <param name="player"></param>
    /// <param name="customer"></param>
    public void CheckingTheDeliveredSaladCombo(GameObject player, GameObject customer)
    {
        string playerCombo = player.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        string customercombo = customer.transform.GetChild(1).GetComponent<CustomerScript>().saladCombo;

        if (playerCombo != string.Empty)
        {
            if (playerCombo == customercombo)
            {
                player.GetComponent<Player>().UpdateScore(player.GetComponent<Player>().score + ScorePoints);
                //ResetTheCombosOfPlayerAndCustomer(player, customer, true);
            }
            else
            {
                //ResetTheCombosOfPlayerAndCustomer(player, customer, false);
            }
        }
    }

    /// <summary>
    /// Resetting the salad combo that the customer and the player has oncw the delivery is done
    /// </summary>
    /// <param name="player"></param>
    /// <param name="customer"></param>
    /// <param name="status"></param>
    //private void ResetTheCombosOfPlayerAndCustomer(GameObject player, GameObject customer, bool status)
    //{
    //    player.GetComponent<Player>().ResetTheCombo(status);
    //    customer.transform.GetChild(1).GetComponent<CustomerScript>().ResetTheCombo(status);
    //}

    /// <summary>
    /// Countdown timer for both the players
    /// </summary>
    private void CountDownTimer()
    {
        TimeLeft -= Time.deltaTime;
        PlayerA_Timer.text = PlayerB_Timer.text = (TimeLeft).ToString("0");

        if (TimeLeft < 0)
        {
            gameOver = true;
        }
    }

    /// <summary>
    /// Check if the game has completed and show the results
    /// </summary>
    //public void CheckIfGameOver()
    //{
    //    if (players[0].timeRanOut && players[1].timeRanOut)
    //    {
    //        gameOver = true;
    //        KillCustomers();
    //        EndScreen.SetActive(true);
    //        if (players[0].score > players[1].score)
    //        {
    //            ResultText.text = players[0].name + " Wins \n score : " + players[0].score;
    //            SaveTheHighScore(players[0].gameObject);
    //        }
    //        else
    //        {
    //            ResultText.text = players[1].name + " Wins \n score : " + players[1].score;
    //            SaveTheHighScore(players[1].gameObject);
    //        }
    //    }
    //}

    /// <summary>
    /// Restart the game
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// Save the scores 
    /// </summary>
    /// <param name="player"></param>
    //private void SaveTheHighScore(GameObject player)
    //{
    //    Dictionary<string, object> dict_1 = new Dictionary<string, object>();
    //    Highscore newhighscore = new Highscore();

    //    highscoreList.Add(newhighscore);

    //    Dictionary<string, object> dict_2 = new Dictionary<string, object>();

    //    for (int i = 0; i < highscoreList.Count; i++)
    //    {
    //        dict_2.Add(i.ToString(), highscoreList[i].score);
    //    }

    //    dict_1.Add("highscores", dict_2);
    //    string highscoreJson = Json.Serialize(dict_1);

    //    string path = "Assets/Resources/savedata.txt";

    //    if (!File.Exists(path))
    //    {
    //        File.WriteAllText(path, highscoreJson);
    //    }
    //    else
    //    {
    //        File.WriteAllText(path, highscoreJson);
    //    }
    //}

    /// <summary>
    /// Loading the highscore when the highscore button is pressed at the end screen
    /// </summary>
    /// <param name="status"></param>
    public void LoadHighScoreOnGameEnd(bool status)
    {
        //Clearing the high score UI
        if (ScrollRectContentParent.childCount > 0)
        {
            for (int i = 0; i < ScrollRectContentParent.childCount; i++)
            {
                Destroy(ScrollRectContentParent.GetChild(i).gameObject);
            }
        }

        if (status)
        {
            if (tempIntList.Count == 0)
            {
                for (int i = 0; i < highscoreList.Count; i++)
                {
                    tempIntList[i] = highscoreList[i].score;
                }

                for (int i = 0; i < tempIntList.Count; i++)
                {
                    for (int j = i + 1; j < tempIntList.Count; j++)
                    {
                        if (tempIntList[j] > tempIntList[i])
                        {
                            int tempscore = tempIntList[i];
                            tempIntList[i] = tempIntList[j];
                            tempIntList[j] = tempscore;
                        }
                    }
                }
            }

            HighScoreUI.SetActive(status);

            for (int i = 0; i < 10; i++)
            {
                GameObject go = Instantiate(ScoreUI);
                go.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                go.transform.GetChild(1).GetComponent<Text>().text = tempIntList[i].ToString();
                go.transform.SetParent(ScrollRectContentParent, false);
            }
        }
        else
        {
            HighScoreUI.SetActive(status);
        }
    }

    /// <summary>
    /// Loading the highscore on gamestart and saving them in a class for serializing them in json
    /// </summary>
    //private void LoadTheHighScoresOnGameStart()
    //{
    //    string path = "Assets/Resources/savedata.txt";

    //    if (File.Exists(path))
    //    {
    //        StreamReader reader = new StreamReader(path);
    //        string highscore = reader.ReadToEnd();
    //        reader.Close();

    //        var dict_1 = Json.Deserialize(highscore) as Dictionary<string, object>;
    //        kvpList = dict_1.ToList();

    //        foreach (KeyValuePair<string, object> kvp in kvpList)
    //        {
    //            Dictionary<string, object> dict_2 = new Dictionary<string, object>();
    //            dict_2 = kvp.Value as Dictionary<string, object>;

    //            var kvpDict_2 = dict_2.ToList();

    //            foreach (KeyValuePair<string, object> kvp2 in kvpDict_2)
    //            {
    //                int h_score = (int.Parse)((kvp2.Value).ToString());
    //                tempIntList.Add(h_score);
    //            }
    //        }

    //        SortingHighScore();
    //    }
    //    else
    //    {
    //        Debug.Log("The given path is invalid");
    //    }
    //}

    /// <summary>
    /// Sorting the highscores for display
    /// </summary>
    private void SortingHighScore()
    {
        for (int i = 0; i < tempIntList.Count; i++)
        {
            for (int j = i + 1; j < tempIntList.Count; j++)
            {
                if (tempIntList[j] > tempIntList[i])
                {
                    int tempscore = tempIntList[i];
                    tempIntList[i] = tempIntList[j];
                    tempIntList[j] = tempscore;
                }
            }
        }

        for (int i = 0; i < tempIntList.Count; i++)
        {
            Highscore hscore = new Highscore();
            hscore.score = tempIntList[i];
            highscoreList.Add(hscore);
        }
    }
}

/// <summary>
/// Class to save the scores
/// </summary>
[System.Serializable]
public class Highscore
{
    public int score = 0;
}
