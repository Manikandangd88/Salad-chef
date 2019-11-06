using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    SaladChef_GameManager gamemanager;

    private readonly int Max_Vegetables_Hold_Limit = 2;     //Maximum no of vegetables a player can hold

    public int score = 0;

    [SerializeField]
    private float timeLeft = 20f;
    public float speed = 2f;

    public bool timeRanOut = false;
    private bool canMove = true;
    private bool hitWithWall = false;            //flag to check if the player has touched the wall    
    private bool hitWithVegetables = false;      //flag to check if the player has touched the vegetables    
    private bool hitWithChopTable = false;       //flag to check if the player has touched the chop table
    private bool hitWithTrashCan = false;        //flag to check if the player has touched the trashcan
    [SerializeField]
    private bool hitWithCustomer = false;        //flag to check if the player has touched the customer

    private string vegetableName = string.Empty;    //currently touched vegetable 
    [SerializeField]
    private string horizontalInput = string.Empty;
    [SerializeField]
    private string verticalInput = string.Empty;

    public KeyCode interactKey;

    private Vector3 collisionPos = Vector3.zero;

    private Rigidbody2D r_Body;                   //rigidbody of this player

    private GameObject VegetablePlateObject = null;

    public Text ScoreText;
    public Text TimeText;

    public TextMeshProUGUI UnchoppedVegetables_Text = null;
    public TextMeshProUGUI ChoppedVegetables_Text = null;
    public TextMeshProUGUI ChopTableVegetable_Text = null;

    [SerializeField]
    private List<string> vegetableList = new List<string>();       //the collected vegetables name from the vegetable plate
    [SerializeField]
    private List<string> choppedVegetables = new List<string>();   //The list of chopped vegetables

    private void Awake()
    {
        r_Body = GetComponent<Rigidbody2D>();
        gamemanager = FindObjectOfType<SaladChef_GameManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw(horizontalInput) != 0 || Input.GetAxisRaw(verticalInput) != 0)
        {
            if (canMove)
            {
                Move();
            }
        }

        if (timeRanOut == false)
        {
            CountDownTimer();
        }
    }

    /// <summary>
    /// Move the player
    /// </summary>
    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw(horizontalInput), Input.GetAxisRaw(verticalInput), 0).normalized;
        transform.position += moveDir * speed * Time.deltaTime;
    }

    #region TriggerEnter, TriggerStay, TriggerExit
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "time")
        {
            timeLeft += gamemanager.BonusTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "vegetable")
        {
            VegetablePlateObject = collision.gameObject;
            vegetableName = VegetablePlateObject.GetComponent<VegetablePlate>().VegetableName;
            hitWithVegetables = true;
        }
        else if (collision.tag == "wall")
        {

        }
        else if (collision.tag == "choptable")
        {
            hitWithChopTable = true;
        }
        else if (collision.tag == "customer")
        {
            hitWithCustomer = true;
        }
        else if (collision.tag == "trash")
        {
            hitWithTrashCan = true;
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (hitWithVegetables)
            {
                AddVegetablesToPlate();
            }
            else if (hitWithChopTable)
            {
                ChopVegetables(collision.gameObject);
            }
            else if (hitWithCustomer)
            {
                gamemanager.CheckingTheDeliveredSaladCombo(this.gameObject, collision.gameObject);
            }
            else if (hitWithTrashCan)
            {
                TrashingTheVegetables();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "vegetable")
        {
            hitWithVegetables = false;
        }
        else if (collision.tag == "wall")
        {

        }
        else if (collision.tag == "choptable")
        {
            hitWithChopTable = false;
        }
        else if (hitWithCustomer)
        {
            hitWithCustomer = false;
        }
        else if (collision.tag == "trash")
        {
            hitWithTrashCan = false;
        }
    }
    #endregion

    /// <summary>
    /// Adding the vegetables from the plate to the player
    /// </summary>
    private void AddVegetablesToPlate()
    {
        if (vegetableList.Count < Max_Vegetables_Hold_Limit)
        {
            vegetableList.Add(vegetableName);

            if (UnchoppedVegetables_Text.text != "")
            {
                //UnchoppedVegetables_Text.text += " " + vegetableName;
                UnchoppedVegetables_Text.text += vegetableName;
            }
            else
            {
                UnchoppedVegetables_Text.text = vegetableName;
            }
        }
        else
        {

        }
    }

    #region ChoppingVegetables
    /// <summary>
    /// This method is responsible for chopping the vegetables
    /// </summary>
    private void ChopVegetables(GameObject choptable)
    {
        if (vegetableList.Count > 0)
        {
            canMove = false;
            Remove_Vegetables_From_Player_Holding_VegetableList(choptable);

            if (canMove == false)
            {
                StartCoroutine(DelaytimeForChopping(choptable));
            }
        }
        else
        {

        }
    }

    /// <summary>
    /// Time taken for chopping the vegetables
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaytimeForChopping(GameObject choptable)
    {
        yield return new WaitForSeconds(2f);
        ChoppedVegetables_Text.text += choptable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        choptable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        canMove = true;
    }
    #endregion

    /// <summary>
    /// Removing the vegetable from the player and placing them in the chop board for chopping
    /// </summary>
    private void Remove_Vegetables_From_Player_Holding_VegetableList(GameObject choptable)
    {
        string currentVegetable = string.Empty;

        TextMeshProUGUI txtmesh = choptable.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        currentVegetable = txtmesh.text = vegetableList[0];
        choppedVegetables.Add(currentVegetable);
        vegetableList.Remove(currentVegetable);

        if (vegetableList.Count > 0)
        {
            UnchoppedVegetables_Text.text = vegetableList[vegetableList.Count - 1];
        }
        else
        {
            UnchoppedVegetables_Text.text = "";
        }
    }

    /// <summary>
    /// This method is responsible for 
    /// </summary>
    private void TrashingTheVegetables()
    {
        if (choppedVegetables.Count > 0)
        {
            choppedVegetables.Clear();
            ChoppedVegetables_Text.text = "";
        }
    }

    /// <summary>
    /// Resetting the salad combo on delivery
    /// </summary>
    /// <param name="status"></param>
    public void ResetTheCombo(bool status)
    {
        if (status)
        {
            UnchoppedVegetables_Text.text = "";
            ChoppedVegetables_Text.text = "";
        }
        else
        {

        }
    }

    /// <summary>
    /// Updating the player score
    /// </summary>
    /// <param name="val"></param>
    public void UpdateScore(int val)
    {
        score = val;
        ScoreText.text = score.ToString();
    }

    /// <summary>
    /// Countdown timer of the player
    /// </summary>
    public void CountDownTimer()
    {
        timeLeft -= Time.deltaTime;
        TimeText.text = timeLeft.ToString("0");

        if (timeLeft < 0)
        {
            timeRanOut = true;
            //gamemanager.CheckIfGameOver();
        }
    }

    /// <summary>
    /// Gameobject Disable
    /// </summary>
    private void OnDisable()
    {

    }
}
