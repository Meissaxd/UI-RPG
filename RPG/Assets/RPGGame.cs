using UnityEngine;
using UnityEngine.UI;

public class RPGGame : MonoBehaviour
{
    public Text playerStatsText;
    public Text enemyStatsText;
    public Text gameLogText;

    public Button attackButton;
    public Toggle shieldToggle;

    private Player player;
    private Enemy enemy;

    void Start()
    {
        player = new Player();
        enemy = new Enemy();

        StartNewEnemy();
        UpdateUI();

        attackButton.onClick.AddListener(PlayerAttack);
        shieldToggle.onValueChanged.AddListener(delegate { ToggleShield(shieldToggle.isOn); });
    }

    void StartNewEnemy()
    {
        enemy.GenerateNewCharacter();
        gameLogText.text += "A new enemy appears!\n";
    }

    void UpdateUI()
    {
        playerStatsText.text = $"Player: Health {player.GetHealth()}, Shield {player.GetShield()}";
        enemyStatsText.text = $"Enemy: Health {enemy.GetHealth()}";
    }

    void PlayerAttack()
    {
        if (player.GetHealth() <= 0)
            return;

        int baseDamage = Random.Range(10, 21);
        player.Attack(enemy, baseDamage); 

        if (enemy.GetHealth() <= 0)
        {
            gameLogText.text += "You defeated the enemy!\n";
            StartNewEnemy();
        }
        else
        {
            EnemyAttack();
        }

        UpdateUI();
    }

    void EnemyAttack()
    {
        int damageToPlayer = enemy.GetDamage();

        if (player.IsShieldActive && player.GetShield() > 0)
        {
            damageToPlayer /= 2;
            player.ReduceShield(Random.Range(10, 21));

            if (player.GetShield() <= 0)
            {
                shieldToggle.isOn = false;
                gameLogText.text += "Your shield is destroyed!\n";
            }
        }

        player.TakeDamage(damageToPlayer);
        gameLogText.text += $"The enemy attacks you for {damageToPlayer} damage.\n";

        if (player.GetHealth() <= 0)
        {
            gameLogText.text += "You have been defeated. Game Over.\n";
            attackButton.interactable = false;
        }
    }

    void ToggleShield(bool isOn)
    {
        if (player.GetShield() > 0)
        {
            player.IsShieldActive = isOn;
            gameLogText.text += isOn ? "Shield activated.\n" : "Shield deactivated.\n";
        }
        else
        {
            shieldToggle.isOn = false;
        }
    }
}



public abstract class Character
{
    private int health;
    private int damage;

    public int GetHealth() => health;
    public void SetHealth(int value) => health = Mathf.Max(0, value);
    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;

    public void TakeDamage(int amount) => SetHealth(GetHealth() - amount);

    public abstract void GenerateNewCharacter();
    public virtual void Attack(Character target)
    {
        target.TakeDamage(GetDamage());
    }
}

public class Player : Character
{
    private int shield = 50;
    public bool IsShieldActive { get; set; } = false;

    public Player()
    {
        SetHealth(100);
    }

    public int GetShield() => shield;
    public void ReduceShield(int amount)
    {
        shield -= amount;
        if (shield <= 0)
        {
            shield = 0;
            IsShieldActive = false;
        }
    }

    public void Attack(Character target, int bonusDamage)
    {
        target.TakeDamage(bonusDamage);
        Debug.Log($"Player attacks with {bonusDamage} damage!");
    }

    public override void GenerateNewCharacter()
    {
        //enemy generation jāpabeidz
    }
}

public class Enemy : Character
{
    public override void GenerateNewCharacter()
    {
        SetHealth(Random.Range(50, 101));
        SetDamage(Random.Range(5, 16));
    }
}