using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SladChef_CustomerSpawner : MonoBehaviour
{
    SaladChef_GameManager gamemamnager;

    public Transform initialPosition;

    [SerializeField]
    private GameObject customerPrefab;
    private GameObject customerPrefab_Parent;    

    private void Awake()
    {
        gamemamnager = FindObjectOfType<SaladChef_GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (gamemamnager.gameOver == false)
        {
            InitiateTheCustomerSpawning();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Spawning the customer

    /// <summary>
    /// Initiate the customer spawning
    /// </summary>
    public void InitiateTheCustomerSpawning()
    {
        if (gamemamnager.gameOver == false)
        {
            if (gamemamnager.tempTransList.Count > 0)
            {
                StartCoroutine(SpawnTheCustomer());
            }
        }
    }   

    /// <summary>
    /// Spawning the customer, assigning its parent and scale
    /// Assigning a seat to the customer 
    /// Assigning a salad combo to the customer
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTheCustomer()
    {        
        yield return new WaitForSeconds(Random.Range(1,5));
        Transform parent = gamemamnager.Alot_A_Seat_For_The_Customer();        
        GameObject customer = Instantiate(customerPrefab, initialPosition.position, Quaternion.identity);

        if (gamemamnager.gameOver == false)
        {
            if (parent != null)
            {
                if (parent.transform.childCount > 1)
                {
                    Destroy(customer);
                }
                else
                {
                    customer.transform.SetParent(parent, false);
                    customer.transform.localScale = new Vector3(20, 20, 20);
                    customer.GetComponent<CustomerScript>().GoToTheSeat(parent);
                    AssignTheVegetableCombo(customer);
                }
            }
        }
    }   
    
    /// <summary>
    /// Assign a salad combo to the spawned customer
    /// </summary>
    /// <param name="customer"></param>
    private void AssignTheVegetableCombo(GameObject customer)
    {
        int randomNo = Random.Range(1, 4);
        int val = 0;

        for(int i = 0; i < randomNo; i++)
        {
            val = Random.Range(0, 6);
            customer.GetComponent<CustomerScript>().saladCombo += gamemamnager.VegetableList[val];            
        }
    }

    #endregion
}
