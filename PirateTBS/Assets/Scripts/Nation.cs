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
    public Nationality Name;                //Name of nation
    public List<Nation> Allies;             //List of nations this nation is allied with
    public List<Nation> Enemies;            //List of nations this nation is at war with

    /// <summary>
    /// Creates a new nation
    /// </summary>
    /// <param name="nationality">Nationality of new nation</param>
    public Nation(Nationality nationality)
    {
        Name = nationality;
        Allies = new List<Nation>();
        Enemies = new List<Nation>();
    }

    /// <summary>
    /// Go to war with another nation
    /// </summary>
    /// <param name="nation">Nation to go to war with</param>
    public void GoToWarWith(Nation nation)
    {
        if (!Enemies.Contains(nation))
            Enemies.Add(nation);
        if (Allies.Contains(nation))
            Allies.Remove(nation);
    }

    /// <summary>
    /// Make peace with another nation
    /// </summary>
    /// <param name="nation">Nation to make peace with</param>
    public void MakePeaceWith(Nation nation)
    {
        if (!Allies.Contains(nation))
            Allies.Add(nation);
        if (Enemies.Contains(nation))
            Enemies.Remove(nation);
    }
}
