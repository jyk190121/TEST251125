using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    public int maxHP = 3;
    private int currentHP;

    private RoomController room;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void SetupRoom(RoomController roomController)
    {
        room = roomController;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        if (room != null)
        {
            room.ClearDungeon(gameObject);
        }

        gameObject.SetActive(false);
    }


}
