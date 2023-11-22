using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateWorld : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;
    public GameObject prefabGrass;
    public GameObject prefabWater;
    public GameObject prefabDryTree;
    public GameObject prefabSand;

    int mapWidth = 160;
    int mapHeight = 160;

    // Any bet 4 and 20
    float magnification = 88f;

    int xOffset = 1;
    int yOffset = -1;

    List<List<int>> noiseGrig = new List<List<int>>();
    List<List<GameObject>> tileGrid = new List<List<GameObject>>();

    public int seed = 0;

    void CreateTileSet()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefabGrass);
        tileset.Add(1, prefabWater);
        tileset.Add(2, prefabDryTree);
        tileset.Add(3, prefabSand);
    }

    void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();

        foreach (KeyValuePair<int, GameObject> prefabPair in tileset)
        {
            GameObject tileGroup = new GameObject(prefabPair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = new Vector3(0, 0, 0);
            tileGroups.Add(prefabPair.Key, tileGroup);
        }
    }

    int GetIdUsingPerlinNoise(int x, int y)
    {
        float rawPerlin = Mathf.PerlinNoise((x - (xOffset * seed)) / magnification, (y - (yOffset * seed)) / magnification);
        float clampPerlin = Mathf.Clamp(rawPerlin, 0f, 1f);
        float scaledPerlin = clampPerlin * tileset.Count;

        if (scaledPerlin == tileset.Count)
        {
            scaledPerlin -= 1;
        }

        return Mathf.FloorToInt(scaledPerlin);
    }

    void CreateTile(int tileId, int x, int y)
    {
        GameObject tilePrefab = tileset[tileId];
        GameObject tileGroup = tileGroups[tileId];
        GameObject tile = Instantiate(tilePrefab, tileGroup.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);

        tileGrid[x].Add(tile);
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            noiseGrig.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());

            for (int y = 0; y < mapHeight; y++)
            {
                int tileId = GetIdUsingPerlinNoise(x, y);
                noiseGrig[x].Add(tileId);
                CreateTile(tileId, x, y);
            }
        }
    }

    void Start()
    {
        if (seed == 0)
        {
            seed = Random.Range(-99999999, 99999999);
        }


        CreateTileSet();
        CreateTileGroups();
        GenerateMap();
    }

}
