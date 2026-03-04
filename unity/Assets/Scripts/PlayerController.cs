using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHP = 100;
    public int currentHP;

    private Vector2 input;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        HandleInput();
        Move();
    }

    void HandleInput()
    {
        // Replace with on-screen joystick in mobile build
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrimaryAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dodge();
        }
    }

    void Move()
    {
        Vector3 delta = new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;
        transform.position += delta;
    }

    public void PrimaryAttack()
    {
        // Local attack animation + send action to DuelManager/network
        Debug.Log("Attack");
    }

    public void Dodge()
    {
        // Short invulnerability frame handled by DuelManager
        Debug.Log("Dodge");
    }

    public void ApplyDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);
    }
}
