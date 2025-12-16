using UnityEngine;

public struct StatStruct
{
    public int att;
    public int hp;
    public int def;
    public int spd;

    public StatStruct(int att, int hp, int def, int spd)
    {
        this.att = att;
        this.hp = hp;
        this.def = def;
        this.spd = spd;
    }
}
