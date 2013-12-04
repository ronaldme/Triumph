﻿using UnityEngine;
using System.Collections;

public abstract class BuildingsBase {

    protected BuildingsBase(int income, int capturePoints)
    {
        this.income = income;
        this.capturePoints = capturePoints;
    }

    public int income { get; set; }
    public int capturePoints { get; set; }
    public abstract BuildingTypes type { get; }
    /// <summary>
    /// Returns the production overlay. Don't know if this needs to be a Texture or Texture2D.
    /// </summary>
    public abstract Texture productionOverlay { get; }

    public void ShowProductionOverlay()
    {
        // spawn below the screen and move it up by x amount of units. Get the texture via the above property.
    }

}