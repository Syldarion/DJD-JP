using System.Collections;
using System.Collections.Generic;

public enum Nationality
{
    English,
    Spanish,
    Dutch,
    French,
    Pirate
}

public class Nation
{
    public Nationality Name { get; private set; }
    public List<Nation> Allies { get; private set; }
    public List<Nation> Enemies { get; private set; }

    public Nation(Nationality nationality)
    {
        Name = nationality;
        Allies = new List<Nation>();
        Enemies = new List<Nation>();
    }

    public void GoToWarWith(Nation nation)
    {
        if (!Enemies.Contains(nation))
            Enemies.Add(nation);
        if (Allies.Contains(nation))
            Allies.Remove(nation);
    }

    public void MakePeaceWith(Nation nation)
    {
        if (!Allies.Contains(nation))
            Allies.Add(nation);
        if (Enemies.Contains(nation))
            Enemies.Remove(nation);
    }
}
