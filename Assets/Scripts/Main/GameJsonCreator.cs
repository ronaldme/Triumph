﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SimpleJSON;

public class GameJsonCreator
{
    public static UnitBase CreateUnit(UnitGameObject ug, bool isHero, UnitTypes type)
    {
        string jsonString = Resources.Load<TextAsset>("JSON/Units/" + type.ToString()).text;
        JSONNode jsonUnit = JSON.Parse(jsonString);

        int attackRange = jsonUnit["attackRange"].AsInt;
        int moveRange = jsonUnit["moveRange"].AsInt;
        bool canAttackAfterMove = jsonUnit["canAttackAfterMove"].AsBool;
        int maxHealth = jsonUnit["maxHealth"].AsInt;
        float damage = jsonUnit["damage"].AsFloat;
        int cost = jsonUnit["cost"].AsInt;
        int fowLos = jsonUnit["fowLos"].AsInt;
        int baseLoot = jsonUnit["baseLoot"].AsInt;
        JSONArray a = jsonUnit["unitModifiers"].AsArray;

        Dictionary<UnitTypes, float> modifiers = new Dictionary<UnitTypes, float>();

        foreach (UnitTypes suit in (UnitTypes[])Enum.GetValues(typeof(UnitTypes)))
        {
            foreach (JSONNode item in a)
            {
                if (item[suit.ToString()] != null && item[suit.ToString()] != "" && item[suit.ToString()] != suit.ToString())
                {
                    modifiers.Add(suit, item[suit.ToString()].AsFloat);
                }
            }
        }

        return new UnitBase(ug, isHero, attackRange, moveRange, canAttackAfterMove, maxHealth, damage, cost, fowLos, baseLoot, modifiers);
    }

    public static BuildingsBase CreateBuilding(BuildingGameObject bg, BuildingTypes type)
    {
        string jsonString = Resources.Load<TextAsset>("JSON/Buildings/" + type.ToString()).text;
        JSONNode jsonBuilding = JSON.Parse(jsonString);

        int income = jsonBuilding["income"].AsInt;
        int capturePoints = jsonBuilding["capturePoints"].AsInt;
        bool canProduce = jsonBuilding["canProduce"].AsBool;
        float damageToCapturingUnit = jsonBuilding["damageToCapturingUnit"].AsFloat;
        int fowLos = jsonBuilding["fowLos"].AsInt;
        int attackRange = jsonBuilding["attackRange"].AsInt;
        float damage = jsonBuilding["damage"].AsFloat;
        JSONArray a = jsonBuilding["unitModifiers"].AsArray;

        Dictionary<UnitTypes, float> modifiers = new Dictionary<UnitTypes, float>();

        foreach (UnitTypes suit in (UnitTypes[])Enum.GetValues(typeof(UnitTypes)))
        {
            foreach (JSONNode item in a)
            {
                if (item[suit.ToString()] != null && item[suit.ToString()] != "")
                {
                    modifiers.Add(suit, item[suit.ToString()].AsFloat);
                }
            }
        }

        return new BuildingsBase(bg, income, capturePoints, canProduce, damageToCapturingUnit, fowLos, attackRange, damage, modifiers);
    }

    public static EnvironmentBase CreateEnvironment(EnvironmentGameObject eg, EnvironmentTypes type)
    {
        string jsonString = Resources.Load<TextAsset>("JSON/Environments/" + type.ToString()).text;
        JSONNode jsonEnvironment = JSON.Parse(jsonString);

        bool isWalkable = jsonEnvironment["isWalkable"].AsBool;

        JSONArray a = jsonEnvironment["unitModifiers"].AsArray;
        
        Dictionary<UnitTypes, float> modifiers = new Dictionary<UnitTypes,float>();

        foreach (UnitTypes suit in (UnitTypes[])Enum.GetValues(typeof(UnitTypes)))
        {
            foreach (JSONNode item in a)
            {
                if (item[suit.ToString()] != null && item[suit.ToString()] != "")
                {
                    modifiers.Add(suit, item[suit.ToString()].AsFloat);
                }
            }
        }
        
        return new EnvironmentBase(eg, isWalkable, modifiers);
    }
}