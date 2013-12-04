﻿using UnityEngine;
using System.Collections;

public class BarracksRange : BuildingsBase {

    public BarracksRange()
        : base(20, 20)
    {

    }
    public override BuildingTypes type
    {
        get { return BuildingTypes.BarracksRange; }
    }

    public override Texture productionOverlay
    {
        get { throw new System.NotImplementedException(); }
    }
}