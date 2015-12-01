using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : NetworkBehaviour
{
<<<<<<< HEAD
    public string Name { get; set; }
=======
    public string Name { get; private set; }
>>>>>>> origin/master
    public Nation Nationality { get; private set; }
    public int TotalGold { get; private set; }
    public int TotalShips { get; private set; }
    public int TotalCrew { get; private set; }
    public List<FleetScript> Fleets { get; private set; }

    //english, spanish, dutch, french
    public int[] Reputation { get; private set; }

    void Start()
    {
        TotalGold = 0;
        TotalShips = 0;
        TotalCrew = 0;
        Fleets = new List<FleetScript>();
        Reputation = new int[4]{ 50, 50, 50, 50};
	}
	
	void Update()
    {

	}

    //new_nation should actually be set to a nation that already exists in game
    public void ChangeNationality(Nation new_nation)
    {
        //make sure you aren't trying to join your own nation and that you aren't trying to join a nation that your current nation is at war with
        //maybe you should be able to join nations at war, but for now, whatever
        if(new_nation != Nationality && !Nationality.Enemies.Contains(new_nation))
        {
            Nationality = new_nation;
            foreach (Nation n in new_nation.Allies)
                ModifyReputation(n.Name, 10);
            foreach (Nation n in new_nation.Enemies)
                ModifyReputation(n.Name, -10);
        }
    }

    public void ModifyReputation(Nationality nation, int modifier)
    {
        Reputation[(int)nation] = Mathf.Clamp(Reputation[(int)nation] + modifier, 0, 100);
    }
}