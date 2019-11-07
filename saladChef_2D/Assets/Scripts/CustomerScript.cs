using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerScript : MonoBehaviour
{
    private SaladChef_GameManager gameManager;
    private SladChef_CustomerSpawner customerspawner;

    private int leanTweenId = 0;

    public float timeRanout;
    [SerializeField]
    private float customerWaitTime = 20f;
    private float graceTime = 0;    
    private float percentageForGraceTime = 0;
    

    public bool isAngry = false;
    public bool correctDeliveryMade = false;

    public string saladCombo = "";

    //[SerializeField]
    public Slider waitTimeBar;
    [SerializeField]
    private float waitTimeDecrementer = 20;

    private TextMeshProUGUI combo;

    [SerializeField]
    private Color angryWaitTimeColor;

    [SerializeField]
    private Image waitTimeBarImage;
    
    private Player[] players;

    private void Awake()
    {
        customerspawner = FindObjectOfType<SladChef_CustomerSpawner>();
        gameManager = FindObjectOfType<SaladChef_GameManager>();

        players = FindObjectsOfType<Player>();

        gameManager.KillCustomers += DestroyGameObject;        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(gameManager.gameOver)
        {
            Destroy(gameObject);
        }
        else
        {
            percentageForGraceTime = gameManager.PercentageForCollectables;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Navigating the customer to his allocated seat
    /// </summary>
    /// <param name="trans"></param>
    public void GoToTheSeat(Transform trans)
    {
        LeanTween.move(gameObject, trans.position, 2f).setOnComplete(() => ChangeTheCurrentSeatToOccupied(trans));
    }

    /// <summary>
    /// Changing the state of the current seat occupied by the customer to reserved 
    /// </summary>
    /// <param name="trans"></param>
    private void ChangeTheCurrentSeatToOccupied(Transform trans)
    {        
        trans.GetComponent<SeatProperty>().reserved = true;

        gameManager.RemoveReservedSeatFromList(trans);
        EnableOrDisableTheSaladCombo(trans, true, saladCombo);
        customerspawner.InitiateTheCustomerSpawning();
        customerWaitTime = customerWaitTime * saladCombo.Length;
        leanTweenId = LeanTween.value(waitTimeBar.value, 0, customerWaitTime - waitTimeDecrementer).setOnUpdate((float value) => UpdateTheWaitMeter(value,trans)).id;
    }

    /// <summary>
    /// customer wait time calculations and Displaying in the HUD
    /// </summary>
    /// <param name="val"></param>
    /// <param name="trans"></param>
    private void UpdateTheWaitMeter(float val,Transform trans)
    {
        waitTimeBar.value = timeRanout = val;
        
        if(waitTimeBar.value == 0)
        {
            CustomerLeaves(trans);   

            for(int i = 0; i < players.Length; i++)
            {
                if (!isAngry)
                {
                    players[i].score -= 1;
                }
                else
                {
                    players[i].score -= 2;
                }

                players[i].UpdateScore(players[i].score);
            }
        }
    }

    /// <summary>
    /// customer leaving functionality
    /// </summary>
    /// <param name="trans"></param>
    private void CustomerLeaves(Transform trans)
    {   
        gameManager.AddTheFreeSeatBackToTheList(trans);
        EnableOrDisableTheSaladCombo(trans,false, "");
        LeanTween.move(gameObject, customerspawner.initialPosition.position, 2f).setOnComplete(DestroyGameObject);
    }

    /// <summary>
    /// Displaying the salad combo of the customer - HUD display
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="status"></param>
    /// <param name="vegcombo"></param>
    public void EnableOrDisableTheSaladCombo(Transform parent,bool status, string vegcombo)
    {
        if(parent != null)
        {
            parent.transform.GetChild(0).gameObject.SetActive(status);
            parent.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = vegcombo;        
        }
    }

    /// <summary>
    /// Destroying the customer prefab when he moves out of the screen
    /// </summary>
    private void DestroyGameObject()
    {
        if(gameManager.gameOver)
        {
            transform.parent.transform.GetChild(0).gameObject.SetActive(false);
            LeanTween.cancel(leanTweenId);
            Destroy(gameObject);            
        }
        else
        {
            customerspawner.InitiateTheCustomerSpawning();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Resetting the salad combo
    /// </summary>
    /// <param name="status"></param>
    public void ResetTheCombo(bool status)
    {
        if (status)
        {
            saladCombo = string.Empty;
            CustomerLeaves(transform.parent);

            LTDescr d = LeanTween.descr(leanTweenId);

            if (d != null)
            {
                LeanTween.cancel(leanTweenId);
            }

            isAngry = false;
        }
        else
        {
            waitTimeBarImage.color = angryWaitTimeColor;
            isAngry = true;
        }
    }    

    /// <summary>
    /// Gameobject disable
    /// </summary>
    private void OnDisable()
    {
        gameManager.KillCustomers -= DestroyGameObject;
    }
}
