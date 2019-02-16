using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField][Tooltip("in one hit")]
    private int damageTaken = 2;
    [SerializeField]
    private int maxHealth = 10;
    private int currentHealth;
    //private Image healthBar;
    private Image HBLeft;
    private Image HBRight;

    
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }

        private set
        {
            currentHealth = value;
            if(HBLeft)
            {
                HBLeft.fillAmount = currentHealth / 100f;
                HBRight.fillAmount = currentHealth / 100f;
            }
            if (CurrentHealth <= 0)
                Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (CurrentHealth >= amount)
            CurrentHealth -= amount;
        else
            CurrentHealth = 0;
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
        //healthBar = transform.Find("Health Bar").Find("Health").GetComponent<Image>();     
        HBLeft = transform.Find("Health Bar").transform.Find("HBLeft").GetComponent<Image>();
        HBRight = transform.Find("Health Bar").transform.Find("HBRight").GetComponent<Image>();
        
    }

    [PunRPC]
    public void RPCHasBeenHit(int shooterId)
    {
        TakeDamage(damageTaken);
    }
}