﻿using UnityEngine;
using System.Collections;

public class Headquarters : BuildingsBase{

    public Headquarters() 
        : base(50, 30)
    {

    }
    public override BuildingTypes type
    {
        get { return BuildingTypes.Headquarters; }
    }

    public override Texture productionOverlay
    {
        get { throw new System.NotImplementedException(); }
    }
}