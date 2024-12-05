using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 
/// ShipScript manages the behavior of a ship, including its position, rotation, interactions with tiles, 
/// and visual feedback like flashing colors. It also tracks hits to determine if the ship is sunk
/// 
public class ShipScript : MonoBehaviour
{
    
    public float xOffset = 0;
    public float zOffset = 0;
    private float nextZRotation = 90f;
    private GameObject clickedTile;
    int hitCount = 0;
    public int shipSize;

    public int TouchTilesCount()
{
    return touchTiles.Count;
}

    private Material[] allMaterials;

    List<GameObject> touchTiles = new List<GameObject>();
    List<Color> allColors = new List<Color>();

    private void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        for (int i = 0; i < allMaterials.Length; i++)
            allColors.Add(allMaterials[i].color);
    }

    // Triggered when the ship collides with a tile. Adds the tile to the touch list
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(collision.gameObject);
                    Debug.Log($"Tile added to touchTiles. Current count: {touchTiles.Count}");
        }
    }

    // clears the list of tiles the ship is currently touching
    public void ClearTileList()
    {
        touchTiles.Clear();
    }


    // calculates the ship's position based on the tile position and offset values
    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }

    // rotates the ship and updates its position to match the rotation
    public void RotateShip()
    {
        if (clickedTile == null) return;
        touchTiles.Clear();
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1;

        // swap xOffset and zOffset to adjust position after rotation
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;

        // update the position based on the clicked tile
        SetPosition(clickedTile.transform.position);
    }

    // ets the ship's position relative to a new vector
    public void SetPosition(Vector3 newVec)
    {
        ClearTileList();
        transform.localPosition = new Vector3(newVec.x + xOffset, 2, newVec.z + zOffset);
    }

    // sets the clicked tile for positioning the ship
    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    // checks if the ship is correctly placed on the game board
    public bool OnGameBoard()
    {
        Debug.Log($"OnGameBoard called. touchTiles.Count: {touchTiles.Count}, shipSize: {shipSize}");
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public void FlashColor(Color tempColor)
    {
        foreach(Material mat in allMaterials)
        {
            mat.color = tempColor;
        }
        Invoke("ResetColor", 0.5f);
    }

    private void ResetColor()
    {
        int i = 0; 
        foreach(Material mat in allMaterials)
        {
            mat.color = allColors[i++];
        }
    }
}