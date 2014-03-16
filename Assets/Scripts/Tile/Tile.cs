﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This class is used to find Tiles in the tile list. This makes it easier than using two variables e.g. int ColumndId and int RowId.
/// </summary>
public class TileCoordinates
{
    public int ColumnId { get; private set; }
    public int RowId { get; private set; }
    public TileCoordinates(int _ColumnId, int _RowId)
    {
        ColumnId = _ColumnId;
        RowId = _RowId;
    }

    public override string ToString()
    {
        return "ColumnId: " + ColumnId + ". RowId: " + RowId + ".";
    }
}

/// <summary>
/// This script is put on a prefab. Some information needs to be set within Unity. The environment object is a prefab. That needs to be dragged ontop of the environmentGameObject variable.
/// This is also true for possible units and buildings.
/// So the tile has a reference to the stuff that is on it.
/// </summary>
public class Tile : MonoBehaviour
{
    // ColumnId and RowId are now calculated dynamicly based on the position. Which elimanates the need to manually set the coordinates.
    //public int ColumnId;
    //public int RowId;
    // The three gameobjects below are prefabs of corresponding objects. ArcherRed-Prefab, ArcherBlue-Prefab, Grass-Prefab, etc.
    // Each of these prefabs should have the corresponding script attached e.g. BuildingGameObject, UnitGameObject or EnvironmentGameObject.
    // And be set within the unity environment.
    public EnvironmentGameObject environmentGameObject;
    public BuildingGameObject buildingGameObject;
    public UnitGameObject unitGameObject;
    public GameObject FogOfWar { get; private set; }
    public bool IsFogShown { get; set; }
    public HighlightObject highlight { get; private set; }
    public TileCoordinates Coordinate { get; private set; }
    public Vector2 Vector2 { get; set; }
    public Loot Loot { get; set; }

    void Awake()
    {
        // The world starts at 0,0 and goes in the x in the positive and the y in the negative. It always needs to be this way other wise the calculation will not work.
        int cId = (((int)gameObject.transform.position.x) / 2) + 1;
        int rId = Mathf.Abs((((int)gameObject.transform.position.y) / 2) - 1);

        Coordinate = new TileCoordinates(cId, rId);
        Vector2 = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
        TileHelper.AddTile(this);
        IsFogShown = false;
        InitHighlights();
    }

    private void InitHighlights()
    {
        FogOfWar = (GameObject)GameObject.Instantiate(Resources.Load(FileLocations.fogOfWar));
        FogOfWar.transform.position = this.transform.position;
        FogOfWar.transform.parent = this.transform;
        Color color = FogOfWar.renderer.material.color;
        color.a = 0f;
        FogOfWar.renderer.material.color = color;

        GameObject highlight = ((GameObject)GameObject.Instantiate(Resources.Load(FileLocations.highlight)));
        highlight.transform.parent = this.transform;
        highlight.transform.position = this.transform.position;
        this.highlight = highlight.GetComponent<HighlightObject>();
        this.highlight.ChangeHighlight(HighlightTypes.highlight_none);        
    }

    public bool HasBuilding()
    {
        return buildingGameObject != null;
    }

    public bool HasUnit()
    {
        return unitGameObject != null;
    }

    public bool HasLoot()
    {
        return Loot != null;
    }
}