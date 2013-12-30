﻿using UnityEngine;
using System.Collections;

public class Big_Rocks : EnvironmentBase
{
    public Big_Rocks(EnvironmentGameObject game)
        : base(game)
    {

    }
    public override EnvironmentTypes type
    {
        get { return EnvironmentTypes.Big_Rocks; }
    }

    public override bool IsWalkable
    {
        get { return true; }
    }
}
