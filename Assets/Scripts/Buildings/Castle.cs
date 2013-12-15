﻿using UnityEngine;
using System.Collections;

public class Castle : BuildingsBase{

    public Castle(BuildingGameObject game)
        : base(game, 50, 40)
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

    public override Sprite sprite
    {
        get
        {
            throw new System.NotImplementedException();
        }
        protected set
        {
            throw new System.NotImplementedException();
        }
    }
}
