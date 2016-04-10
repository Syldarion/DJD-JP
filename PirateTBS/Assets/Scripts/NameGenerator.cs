﻿using UnityEngine;
using System.Collections;
using System.IO;

public class NameGenerator : MonoBehaviour
{
    [HideInInspector]
    public static NameGenerator Instance;

    string[] english_ports;
    string[] spanish_ports;
    string[] dutch_ports;
    string[] french_ports;

    string[] ship_name_adjectives;
    string[] ship_name_nouns;

	void Start()
    {
        Instance = this;

        english_ports = File.ReadAllLines("Assets/Text/EnglishPorts.txt");
        spanish_ports = File.ReadAllLines("Assets/Text/SpanishPorts.txt");
        dutch_ports = File.ReadAllLines("Assets/Text/DutchPorts.txt");
        french_ports = File.ReadAllLines("Assets/Text/FrenchPorts.txt");

        ship_name_adjectives = File.ReadAllLines("Assets/Text/ShipNameAdjectives.txt");
        ship_name_nouns = File.ReadAllLines("Assets/Text/ShipNameNouns.txt");
    }
	
	void Update()
    {

	}

    public string GetPortName(int nation_index)
    {
        string port_name;

        switch(nation_index)
        {
            case 0:
                port_name = english_ports[Random.Range(0, english_ports.Length)];
                break;
            case 1:
                port_name = spanish_ports[Random.Range(0, spanish_ports.Length)];
                break;
            case 2:
                port_name = dutch_ports[Random.Range(0, dutch_ports.Length)];
                break;
            case 3:
            default:
                port_name = french_ports[Random.Range(0, french_ports.Length)];
                break;
        }

        return port_name;
    }

    public string GetShipName()
    {
        string ship_name;
        int format = Random.Range(0, 3); //0 - [Blank], 1 - The [Blank] [Blank], 2 - [Blank] of [Blank]

        switch(format)
        {
            case 0:
                int noun_or_adj = Random.Range(0, 2);
                if (noun_or_adj == 0)
                    ship_name = ship_name_nouns[Random.Range(0, ship_name_nouns.Length)];
                else
                    ship_name = ship_name_adjectives[Random.Range(0, ship_name_adjectives.Length)];
                break;
            case 1:
                ship_name = string.Format("The {0} {1}",
                    ship_name_adjectives[Random.Range(0, ship_name_adjectives.Length)],
                    ship_name_nouns[Random.Range(0, ship_name_nouns.Length)]);
                break;
            case 2:
            default:
                ship_name = string.Format("{0} of {1}",
                    ship_name_nouns[Random.Range(0, ship_name_nouns.Length)],
                    GetPortName(Random.Range(0, 4)));
                break;
        }

        return ship_name;
    }
}
