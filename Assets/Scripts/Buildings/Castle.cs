﻿using UnityEngine;
using System.Collections;

public class Castle : BuildingsBase{

    public Castle() 
        : base(50, 40)
    {

    }
    public override BuildingTypes type
    {
        get { return BuildingTypes.Castle; }
    }

    public override Texture productionOverlay
    {
        get { throw new System.NotImplementedException(); }
    }
}