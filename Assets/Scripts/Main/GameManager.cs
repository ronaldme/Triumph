﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

/// <summary>
/// This singleton class keeps hold of the game.
/// </summary>
public class GameManager
{
    #region Singleton
    private static GameManager instance;
    private GameManager() { }
    public static GameManager Instance { 
        get 
        {
            if (instance == null) 
            { 
                instance = new GameManager();
                instance.Init();
            }
            return instance;
        }
    }
    #endregion

    public Dictionary<int, Dictionary<int, Tile>> tiles;
    public bool IsAudioOn { get; set; }
    public bool IsQuitMenuOn { get; set; }
    public bool IsDoneButtonActive { get; set; }
    public Player CurrentPlayer { get; set; }
    public bool NeedMoving { get; set; }
    public GameObject LastClickedUnitGO { get; set; }
    public Tile LastClickedUnitTile { get; set; }
    public bool IsHightlightOn { get; set; }
    public ProductionOverlayMain productionOverlayMain { get; private set; }
    public CaptureBuildings CaptureBuildings { get; private set; }
    public FogOfWarManager fowManager { get; private set; }
    public bool UnitCanAttack { get; set; }
    public float StartTime { get; set; }
    public Sounds Sounds;

    // Lists need to be accesed in GameManager because when NextPlayer method gets called we want to deactivate
    // the highlights also.
    public List<GameObject> highLightObjects = new List<GameObject>();
    public List<GameObject> attackHighLightObjects = new List<GameObject>();

    // The Player object can still be retrieved via the PlayerIndex enum.
    private SortedList<PlayerIndex, Player> players;
    public int currentTurn { get; private set; }
    private TextMesh currentTurnText;
    private TextMesh playerText;
    private TextMesh currentGold;

    /// <summary>
    /// Use this method as a constructor which is called once when the GameManager singleton is called for the first time.
    /// </summary>
    private void Init()
    {
        currentTurn = 1;
		IsAudioOn = true;
        tiles = new Dictionary<int, Dictionary<int, Tile>>();
        players = new SortedList<PlayerIndex, Player>();
        players.Add(PlayerIndex.Neutral, new Player("Neutral player", PlayerIndex.Neutral));
        players.Add(PlayerIndex.Blue, new Player("Player Blue", PlayerIndex.Blue));
        players.Add(PlayerIndex.Red, new Player("Player Red", PlayerIndex.Red));
        CurrentPlayer = players[PlayerIndex.Blue];

        CaptureBuildings = new CaptureBuildings();
        productionOverlayMain = new ProductionOverlayMain();
        fowManager = new FogOfWarManager();
        Sounds = new Sounds();
    }

    /// <summary>
    /// Add a tile to the list. This methods should only be called one when a Tile GameObject is loaded when the scene starts.
    /// </summary>
    /// <param name="tile"></param>
    public void AddTile(Tile tile)
    {
        // Check if the second dictionary exists in the list. If not then create a new dictionary and insert this in the tiles dictionary.
        if (!tiles.ContainsKey(tile.Coordinate.ColumnId))
        {
            tiles.Add(tile.Coordinate.ColumnId, new Dictionary<int, Tile>());
        }
        // Last insert the tile object into the correct spot in the dictionarys. Since we now know that both dictionary at these keys exist.
        tiles[tile.Coordinate.ColumnId].Add(tile.Coordinate.RowId, tile);
    }

    /// <summary>
    /// Removes all entrys from the tiles dictionary. Should be called before a new scene (level) starts to load. So no old references exists when new Tile are added to the list.
    /// </summary>
    public void ClearTilesDictionary()
    {
        tiles.Clear();
    }

    /// <summary>
    /// Returns the tile with via the given TileCoordinates from the tiles dictionary. Or an KeyNotFoundException if either of the keys is not found.
    /// </summary>
    /// <param name="coor"></param>
    /// <returns></returns>
    public Tile GetTile(TileCoordinates coor)
    {
        if(tiles.ContainsKey(coor.ColumnId) && tiles[coor.ColumnId].ContainsKey(coor.RowId))
        {
            return tiles[coor.ColumnId][coor.RowId];
        }

        return null;
    }

    /// <summary>
    /// Returns the player object by the given PlayerIndex enum.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Player GetPlayer(PlayerIndex index)
    {
        if(!Enum.IsDefined(typeof(PlayerIndex), index))
        {
            throw new KeyNotFoundException("The given playerIndex was not found. Give me a correct PlayerIndex or suffer the consequences.");
        }
        return players[index];
    }
	
	/// <summary>
    /// Perhaps a more cleaner system where a class holds all of the level specific data. For now this method needs to be called whenever a level scene is loaded. If loaded from
    /// the menu errors will come forth.
    /// </summary>
    public void SetupLevel()
    {
        // getting the TextMesh components and setting the player name + current turn
        playerText = GameObject.Find("PlayerName").gameObject.GetComponent<TextMesh>();
        playerText.text = "Player: " + CurrentPlayer.name;

        currentTurnText = GameObject.Find("Turn").gameObject.GetComponent<TextMesh>();
        currentTurnText.text = "Turn: " + currentTurn.ToString();

        currentGold = GameObject.Find("CurrentGold").gameObject.GetComponent<TextMesh>();
        currentGold.text = "Current gold: " + CurrentPlayer.gold;
    }

    public void NextPlayer()
    {
        ClearMovementAndHighLights();
        productionOverlayMain.DestroyAndStopOverlay();
        CaptureBuildings.CalculateCapturing();
        CurrentPlayer.IncreaseGoldBy(CurrentPlayer.GetCurrentIncome());

        // Change the currentplayer to the next player. Works with all amount of players. Ignores the Neutral player.
        bool foundPlayer = false;
        while(!foundPlayer)
        {
            int indexplayer = players.IndexOfKey(CurrentPlayer.index) + 1;
            if (indexplayer >= players.Count) { indexplayer = 0; }
            CurrentPlayer = players.Values[indexplayer];
            foundPlayer = CurrentPlayer.index != PlayerIndex.Neutral;
        }
        UpdateTextboxes();
        // Needs to be called after the CurrentTurn has increase in the UpdateTextBoxes() method. 
        fowManager.ShowOrHideFowPlayer();
    }

    public void UpdateTextboxes()
    {
        playerText.text = "Player: " + CurrentPlayer.name;
        currentGold.text = "Current gold: " + CurrentPlayer.gold;
        currentTurn++;
        currentTurnText.text = "Turn: " + currentTurn.ToString();
    }

    private void ClearMovementAndHighLights()
    {
        foreach (UnitBase unit in CurrentPlayer.ownedUnits)
        {
            unit.unitGameObject.renderer.material.color = Color.white;
            unit.hasMoved = false;
            unit.hasAttacked = false;
        }

        ClearHighlight();
    }

    public void ClearHighlight()
    {
        foreach (GameObject highlights in this.highLightObjects)
        {
            highlights.SetActive(false);
        }

        foreach (GameObject attackHighlight in this.attackHighLightObjects)
        {
            attackHighlight.SetActive(false);
        }

        this.highLightObjects.Clear();
        this.attackHighLightObjects.Clear();
        this.IsHightlightOn = false;
    }

    /// <summary>
    /// This method returns all the tiles that are within a certain range calculated from the Tile which you specify.
    /// It takes into account out of bounds and doesn't add the tile the which has the same coordinates as the given center point. 
    /// The return type is the same as the tile collection in the GameManager for consistancy.
    /// The movement and attack classes / implementations should be responsible for further calculations.
    /// So if you want to highlight possible attack locations the attack class / method should loop through all tiles in range
    /// and only hightlight the tiles on which an enemy unit is placed.
    /// </summary>
    /// <param name="centerPointTileCoordinate">The tilecoordinate from which the calculation is done. </param>
    /// <param name="range">The range from which tiles get returned</param>
    public Dictionary<int, Dictionary<int, Tile>> GetAllTilesWithinRange(TileCoordinates centerPointTileCoordinate, int range)
    {
        // Check if the range is 0 or smaller.
        if (range <= 0)
        {
            throw new ArgumentOutOfRangeException("range", "The entered range is 0 or smaller. Please use a correct range");
        }

        if (!tiles.ContainsKey(centerPointTileCoordinate.ColumnId) || !tiles[centerPointTileCoordinate.ColumnId].ContainsKey(centerPointTileCoordinate.RowId))
        {
            throw new ArgumentOutOfRangeException("centerPointTileCoordinate", "The given center tile does not exist. Please give a valid TileCoordinate");
        }

        // collection for holding the possible tiles that are within range.
        Dictionary<int, Dictionary<int, Tile>> possibleLocations = new Dictionary<int, Dictionary<int, Tile>>();

        int columnId = centerPointTileCoordinate.ColumnId;
        int rowId = centerPointTileCoordinate.RowId;

        // The row size in which it goes up and down.
        int size = 0;

        int beginColumnId = columnId - range;
        int endColumnId = columnId + range;
        int currentColumnId = beginColumnId;

        while (currentColumnId <= endColumnId)
        {
            // If the current tilecoordinate falls outside the level dont bother getting it.
            if (!tiles.ContainsKey(currentColumnId))
            {
                currentColumnId++;
                size++;
                continue;
            }

            int beginRowId = rowId - size;
            int endRowId = rowId + size;
            int currentRowid = beginRowId;

            while (currentRowid <= endRowId)
            {
                // If the current tilecoordinate falls outside the level dont bother getting it.
                // And if the current tilecoordinate is on the same place as the original coordinate dont get it.
                if (!tiles[currentColumnId].ContainsKey(currentRowid) ||
                    (currentColumnId == centerPointTileCoordinate.ColumnId &&
                    currentRowid == centerPointTileCoordinate.RowId))
                {
                    currentRowid++;
                    continue;
                }
                // Get the tile from the tile list and add it to the return list.
                Tile t = GetTile(new TileCoordinates(currentColumnId, currentRowid));
                if (!possibleLocations.ContainsKey(currentColumnId))
                {
                    possibleLocations.Add(currentColumnId, new Dictionary<int, Tile>());
                }
                possibleLocations[currentColumnId].Add(currentRowid, t);
                currentRowid++;
            }
            currentColumnId++;
            // Determine if the currentColumnId has reached the center tile columnid, ifso start making the size smaller.
            size = currentColumnId <= columnId ? size += 1 : size -= 1;
        }
        return possibleLocations;
    }

    /// <summary>
    /// This method is called whenever a unit needs to be destroyed. For example when a unit is unit is killed.
    /// In here needs to be all of the code to remove the references that the game has to any of these objects (and childs).
    /// </summary>
    /// <param name="unitToDestroy">The UnitGameObject to destroy</param>
    public void DestroyUnitGameObjects(UnitGameObject unitToDestroy)
    {
        unitToDestroy.tile.unitGameObject = null;
        unitToDestroy.tile = null;
        GameManager.Instance.GetPlayer(unitToDestroy.index).RemoveUnit(unitToDestroy.unitGame);
        GameObject.Destroy(unitToDestroy.gameObject);
    }

    /// <summary>
    /// This method is called whenever a building needs to be destroyed. For example when a building is captured.
    /// In here needs to be all of the code to remove the references that the game has to any of these objects (and childs).
    /// </summary>
    /// <param name="unitToDestroy">The UnitGameObject to destroy</param>
    public void DestroyBuildingGameObjects(BuildingGameObject buildingToDestroy)
    {
        buildingToDestroy.tile.buildingGameObject = null;
        buildingToDestroy.tile = null;
        this.GetPlayer(buildingToDestroy.index).RemoveBuilding(buildingToDestroy.buildingGame);
        GameObject.Destroy(buildingToDestroy.gameObject);
    }
}
