using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    enum TileMapperGround
    {
        DoubleFlatUpLeftPath = 0,
        FlatUpPath = 1,
        DoubleFlatUpRightPath = 2,
        FlatLeftPath = 3,
        FullGrass = 4,
        FlatRightPath = 5,
        DoubleFlatDownLeftPath = 6,
        FlatDownPath = 7,
        DoubleFlatDownRightPath = 8,
        CornerDownRightPath = 9,
        CornerDownLeftPath = 11,
        FullPath = 13,
        CornerUpRightPath = 15,
        CornerUpLeftPath = 17
    }
    enum TileMapperWater
    {
        DoubleFlatUpLeftWater = 0,
        FlatUpWater = 1,
        DoubleFlatUpRightWater = 2,
        FlatLeftWater = 3,
        FlatRightWater = 4,
        DoubleFlatDownLeftWater = 5,
        FlatDownWater = 6,
        DoubleFlatDownRightWater = 7,
        CornerDownRightWater = 8,
        CornerDownLeftWater = 10,
        FullWater = 12,
        CornerUpRightWater = 14,
        CornerUpLeftWater = 16
    }
    enum ChunkType
    {
        Ocean,
        Ground
    }
    enum BiomeType
    {
        None,
        Plain,
        Forest,
        Path
    }
    enum CellType
    {
        None,
        Ground,
        Water,
        Transition,
        Path
    }
    enum TransitionType
    {
        Full,
        FlatUp,
        FlatDown,
        FlatRight,
        FlatLeft,
        DoubleFlatUpRight,
        DoubleFlatUpLeft,
        DoubleFlatDownRight,
        DoubleFlatDownLeft,
        CornerUpRight,
        CornerUpLeft,
        CornerDownLeft,
        CornerDownRight
    }

    public List<GameObject> PlainObjects;
    public List<GameObject> ForestObjects;
    public List<GameObject> PathObjects;

    public List<float> PlainProbabilities;
    public List<float> ForestProbabilities;
    public List<float> PathProbabilities;

    public float ForestBiomeRarity = 0.3f; // how often is biome generated
    public float PlainsBiomeRarity = 0.3f; // how often is biome generated
    public float PathBiomeRarity = 0.4f; // how often is biome generated

    public Tilemap GroundGrid;
    public Tilemap WaterGrid;
    public List<Tile> GroundTiles;
    public List<Tile> WaterTiles;

    public float ExpansionProbability = 0.6f;
    public int ChunkSize = 16;
    public int MapSizeInChunks = 11;

    List<List<ChunkType>> chunkMap;
    List<List<BiomeType>> biomeMap;
    List<List<CellType>> cellMap;

    int MapSizeInTiles
    {
        get { return MapSizeInChunks * ChunkSize; }
        set { MapSizeInTiles = value; }
    }

    int tilesOffset
    {
        get { return (ChunkSize * (MapSizeInChunks / 2)) + (ChunkSize / 2); }
        set { tilesOffset = value; }
    }

    void SetGroundTile(int x, int y, TileMapperGround tileType)
    {
        x -= tilesOffset;
        y -= tilesOffset;
        GroundGrid.SetTile(new Vector3Int(x, y, 0), GroundTiles[(int)tileType]);
    }

    void SetWaterTile(int x, int y, TileMapperWater tileType)
    {
        x -= tilesOffset;
        y -= tilesOffset;
        WaterGrid.SetTile(new Vector3Int(x, y, 0), WaterTiles[(int)tileType]);
    }

    void SetTile(int x, int y, CellType cellType, Tile tile)
    {
        x -= tilesOffset;
        y -= tilesOffset;
        if (cellType == CellType.Water)
        {
            WaterGrid.SetTile(new Vector3Int(x, y, 0), tile);
        }
        else if (cellType == CellType.Path)
        {
            GroundGrid.SetTile(new Vector3Int(x, y, 0), tile);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeChunks();
        GenerateGroundAndWater();
        GenerateBiomeTypes();
        GeneratePaths();
        FillTransitions();
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < MapSizeInChunks; i++)
        {
            for (int j = 0; j < MapSizeInChunks; j++)
            {
                if (biomeMap[i][j] == BiomeType.Plain)
                {
                    GeneratePlainObjects(i, j);
                }
                else if (biomeMap[i][j] == BiomeType.Forest)
                {
                    GenerateForestObjects(i, j);
                }
                else if (biomeMap[i][j] == BiomeType.Path)
                {
                    GeneratePathObjects(i, j);
                }
            }
        }
    }

    void GeneratePlainObjects(int i, int j)
    {
        for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
        {
            for (var y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
            {
                if (cellMap[x][y] == CellType.Ground)
                {
                    var randomNumber = Random.Range(0f, 100f);
                    for (int k = 0; k < PlainProbabilities.Count; k++)
                    {
                        if (randomNumber < PlainProbabilities[k])
                        {
                            var go = Instantiate(PlainObjects[k]);
                            go.transform.position = new Vector3(x - tilesOffset + 0.5f, y - tilesOffset + 0.5f, 0f);
                            k = PlainProbabilities.Count;
                        }
                        else
                        {
                            randomNumber -= PlainProbabilities[k];
                        }
                    }
                }
            }
        }
    }

    void GenerateForestObjects(int i, int j)
    {
        for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
        {
            for (var y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
            {
                if (cellMap[x][y] == CellType.Ground)
                {
                    var randomNumber = Random.Range(0f, 100f);
                    for (int k = 0; k < ForestProbabilities.Count; k++)
                    {
                        if (randomNumber < ForestProbabilities[k])
                        {
                            var go = Instantiate(ForestObjects[k]);
                            go.transform.position = new Vector3(x - tilesOffset + 0.5f, y - tilesOffset + 0.5f, 0f);
                            k = ForestProbabilities.Count;
                        }
                        else
                        {
                            randomNumber -= ForestProbabilities[k];
                        }
                    }
                }
            }
        }
    }

    void GeneratePathObjects(int i, int j)
    {
        for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
        {
            for (var y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
            {
                if (cellMap[x][y] == CellType.Ground)
                {
                    var randomNumber = Random.Range(0f, 100f);
                    for (int k = 0; k < PathProbabilities.Count; k++)
                    {
                        if (randomNumber < PathProbabilities[k])
                        {
                            var go = Instantiate(PathObjects[k]);
                            go.transform.position = new Vector3(x - tilesOffset + 0.5f, y - tilesOffset + 0.5f, 0f);
                            k = PathProbabilities.Count;
                        }
                        else
                        {
                            randomNumber -= PathProbabilities[k];
                        }
                    }
                }
            }
        }
    }

    void GeneratePaths()
    {
        for (int i = 0; i < MapSizeInChunks; i++)
        {
            for (int j = 0; j < MapSizeInChunks; j++)
            {
                if (biomeMap[i][j] == BiomeType.Path)
                {
                    // make center cell to be path
                    var x = (i * ChunkSize) + (ChunkSize / 2);
                    var y = (j * ChunkSize) + (ChunkSize / 2);
                    cellMap[x][y] = CellType.Path;
                    SetGroundTile(x, y, TileMapperGround.FullPath);
                    cellMap[x + 1][y] = CellType.Transition;  
                    cellMap[x + 1][y - 1] = CellType.Transition;  
                    cellMap[x + 1][y + 1] = CellType.Transition;  
                    cellMap[x - 1][y + 1] = CellType.Transition;  
                    cellMap[x - 1][y] = CellType.Transition;  
                    cellMap[x - 1][y - 1] = CellType.Transition;  
                    cellMap[x][y - 1] = CellType.Transition;  
                    cellMap[x][y + 1] = CellType.Transition;

                    if (i > 0 && biomeMap[i - 1][j] == BiomeType.Path)
                    {
                        var neighbourY = (j * ChunkSize) + (ChunkSize / 2);
                        for (var neighbourX = (i * ChunkSize); neighbourX < (i * ChunkSize) + (ChunkSize / 2); neighbourX++)
                        {
                            cellMap[neighbourX][neighbourY] = CellType.Path;
                            SetGroundTile(neighbourX, neighbourY, TileMapperGround.FullPath);
                            cellMap[neighbourX][neighbourY - 1] = CellType.Transition;
                            cellMap[neighbourX][neighbourY + 1] = CellType.Transition;
                        }
                    }
                    if (i < MapSizeInChunks - 1 && biomeMap[i + 1][j] == BiomeType.Path)
                    {
                        var neighbourY = (j * ChunkSize) + (ChunkSize / 2);
                        for (var neighbourX = (i * ChunkSize) + (ChunkSize / 2) + 1; neighbourX < (i + 1) * ChunkSize; neighbourX++)
                        {
                            cellMap[neighbourX][neighbourY] = CellType.Path;
                            SetGroundTile(neighbourX, neighbourY, TileMapperGround.FullPath);
                            cellMap[neighbourX][neighbourY - 1] = CellType.Transition;
                            cellMap[neighbourX][neighbourY + 1] = CellType.Transition;
                        }
                    }
                    if (j > 0 && biomeMap[i][j - 1] == BiomeType.Path)
                    {
                        var neighbourX = (i * ChunkSize) + (ChunkSize / 2);
                        for (var neighbourY = (j * ChunkSize); neighbourY < (j * ChunkSize) + (ChunkSize / 2); neighbourY++)
                        {
                            cellMap[neighbourX][neighbourY] = CellType.Path;
                            SetGroundTile(neighbourX, neighbourY, TileMapperGround.FullPath);
                            cellMap[neighbourX - 1][neighbourY] = CellType.Transition;
                            cellMap[neighbourX + 1][neighbourY] = CellType.Transition;
                        }
                    }
                    if (j < MapSizeInChunks - 1 && biomeMap[i][j + 1] == BiomeType.Path)
                    {
                        var neighbourX = (i * ChunkSize) + (ChunkSize / 2);
                        for (var neighbourY = (j * ChunkSize) + (ChunkSize / 2) + 1; neighbourY < (j + 1) * ChunkSize; neighbourY++)
                        {
                            cellMap[neighbourX][neighbourY] = CellType.Path;
                            SetGroundTile(neighbourX, neighbourY, TileMapperGround.FullPath);
                            cellMap[neighbourX - 1][neighbourY] = CellType.Transition;
                            cellMap[neighbourX + 1][neighbourY] = CellType.Transition;
                        }
                    }
                }
            }
        }
    }

    void InitializeChunks()
    {
        // fill all with an ocean
        chunkMap = new List<List<ChunkType>>();
        for (int i = 0; i < MapSizeInChunks; i++)
        {
            chunkMap.Add(new List<ChunkType>());
            for (int j = 0; j < MapSizeInChunks; j++)
            {
                chunkMap[i].Add(ChunkType.Ocean);
            }
        }

        // start the rucusrive ground filling function
        FillGround(MapSizeInChunks / 2, MapSizeInChunks / 2, 1f);
    }

    void FillGround(int x, int y, float probability)
    {
        if (x <= 0 || x >= MapSizeInChunks - 1 || y <= 0 || y >= MapSizeInChunks - 1
            || chunkMap[x][y] == ChunkType.Ground)
        {
            return;
        }
        if (Random.Range(0f, 1f) >= probability)
        {
            return;
        }

        chunkMap[x][y] = ChunkType.Ground;

        FillGround(x - 1, y, ExpansionProbability);
        FillGround(x + 1, y, ExpansionProbability);
        FillGround(x, y - 1, ExpansionProbability);
        FillGround(x, y + 1, ExpansionProbability);
    }

    void GenerateGroundAndWater()
    {
        // fill all tiles with the None type
        cellMap = new List<List<CellType>>();
        for (int i = 0; i < MapSizeInTiles; i++)
        {
            cellMap.Add(new List<CellType>());
            for (int j = 0; j < MapSizeInTiles; j++)
            {
                cellMap[i].Add(CellType.None);
            }
        }

        // fill cell types based on the chunk type
        for (int i = 0; i < MapSizeInChunks; i++)
        {
            for (int j = 0; j < MapSizeInChunks; j++)
            {
                switch (chunkMap[i][j])
                {
                    case ChunkType.Ground:
                        {
                            for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
                            {
                                for (int y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
                                {
                                    cellMap[x][y] = CellType.Ground;
                                    SetGroundTile(x, y, TileMapperGround.FullGrass);
                                }
                            }
                        }
                        break;
                    case ChunkType.Ocean:
                        {
                            for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
                            {
                                for (int y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
                                {
                                    cellMap[x][y] = CellType.Water;
                                }
                            }

                            FillTransitionTypeCellsInOceanChunk(i, j);

                            for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
                            {
                                for (int y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
                                {
                                    if (cellMap[x][y] == CellType.Water)
                                    {
                                        SetWaterTile(x, y, TileMapperWater.FullWater);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    void GenerateBiomeTypes()
    {
        biomeMap = new List<List<BiomeType>>();
        for (int i = 0; i < MapSizeInChunks; i++)
        {
            biomeMap.Add(new List<BiomeType>());
            {
                for (int j = 0; j < MapSizeInChunks; j++)
                {
                    biomeMap[i].Add(BiomeType.None);
                }
            }
        }

        List<float> biomeTypesProbability = new List<float>();
        biomeTypesProbability.Add(PlainsBiomeRarity);
        biomeTypesProbability.Add(ForestBiomeRarity);
        biomeTypesProbability.Add(PathBiomeRarity);
        var totalProbability = PlainsBiomeRarity + ForestBiomeRarity + PathBiomeRarity;

        for (int i = 0; i < MapSizeInChunks; i++)
        {
            for (int j = 0; j < MapSizeInChunks; j++)
            {
                biomeMap[i][j] = BiomeType.None;
                if (chunkMap[i][j] == ChunkType.Ground)
                {
                    var randomNumber = Random.Range(0f, totalProbability);

                    int z = 0;
                    while(biomeMap[i][j] == BiomeType.None)
                    {
                        if (randomNumber <= biomeTypesProbability[z])
                        {
                            switch(z)
                            {
                                case 0:
                                    biomeMap[i][j] = BiomeType.Plain;
                                    break;
                                case 1:
                                    biomeMap[i][j] = BiomeType.Forest;
                                    break;
                                case 2:
                                    biomeMap[i][j] = BiomeType.Path;
                                    break;
                            }
                        }
                        randomNumber -= biomeTypesProbability[z];
                        z++;
                    }
                }
            }
        }
    }

    void FillTransitionTypeCellsInOceanChunk(int i, int j)
    {
        if (chunkMap[i][j] != ChunkType.Ocean)
        {
            return;
        }

        if (i > 0 && chunkMap[i - 1][j] == ChunkType.Ground)
        {
            var x = i * ChunkSize;
            for (int y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
            {
                cellMap[x][y] = CellType.Transition;
            }
        }

        if (i < MapSizeInChunks - 1 && chunkMap[i + 1][j] == ChunkType.Ground)
        {
            var x = ((i + 1) * ChunkSize) - 1;
            for (int y = j * ChunkSize; y < (j + 1) * ChunkSize; y++)
            {
                cellMap[x][y] = CellType.Transition;
            }
        }

        if (j > 0 && chunkMap[i][j - 1] == ChunkType.Ground)
        {
            var y = j * ChunkSize;
            for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
            {
                cellMap[x][y] = CellType.Transition;
            }
        }

        if (j < MapSizeInChunks - 1 && chunkMap[i][j + 1] == ChunkType.Ground)
        {
            var y = ((j + 1) * ChunkSize) - 1;
            for (int x = i * ChunkSize; x < (i + 1) * ChunkSize; x++)
            {
                cellMap[x][y] = CellType.Transition;
            }
        }

        if (j < MapSizeInChunks - 1 && i < MapSizeInChunks - 1 && chunkMap[i + 1][j + 1] == ChunkType.Ground)
        {
            var x = ((i + 1) * ChunkSize) - 1;
            var y = ((j + 1) * ChunkSize) - 1;
            cellMap[x][y] = CellType.Transition;
        }

        if (j > 0 && i < MapSizeInChunks - 1 && chunkMap[i + 1][j - 1] == ChunkType.Ground)
        {
            var x = ((i + 1) * ChunkSize) - 1;
            var y =  j * ChunkSize;
            cellMap[x][y] = CellType.Transition;
        }

        if (j < MapSizeInChunks - 1 && i > 0 && chunkMap[i - 1][j + 1] == ChunkType.Ground)
        {
            var x = i * ChunkSize;
            var y = ((j + 1) * ChunkSize) - 1;
            cellMap[x][y] = CellType.Transition;
        }

        if (j > 0 && i > 0 && chunkMap[i - 1][j - 1] == ChunkType.Ground)
        {
            var x = i * ChunkSize;
            var y = j * ChunkSize;
            cellMap[x][y] = CellType.Transition;
        }
    }

    void FillTyleTypeForTransitionCell(int x, int y, CellType cellType)
    {
        var s = new TransitionStorage(cellType, this);

        SetGroundTile(x, y, TileMapperGround.FullGrass);

        // double flat
        if (cellMap[x - 1][y] == cellType && cellMap[x][y + 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.DoubleFlatUpLeft));
            return;
        }
        if (cellMap[x + 1][y] == cellType && cellMap[x][y + 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.DoubleFlatUpRight));
            return;
        }
        if (cellMap[x - 1][y] == cellType && cellMap[x][y - 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.DoubleFlatDownLeft));
            return;
        }
        if (cellMap[x + 1][y] == cellType && cellMap[x][y - 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.DoubleFlatDownRight));
            return;
        }

        // flat
        if (cellMap[x - 1][y] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.FlatLeft));
            return;
        }
        if (cellMap[x + 1][y] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.FlatRight));
            return;
        }
        if (cellMap[x][y - 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.FlatDown));
            return;
        }
        if (cellMap[x][y + 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.FlatUp));
            return;
        }

        // corner
        if (cellMap[x - 1][y - 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.CornerDownLeft));
            return;
        }
        if (cellMap[x + 1][y - 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.CornerDownRight));
            return;
        }
        if (cellMap[x + 1][y + 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.CornerUpRight));
            return;
        }
        if (cellMap[x - 1][y + 1] == cellType)
        {
            SetTile(x, y, cellType, s.GetTransitionTile(TransitionType.CornerUpLeft));
            return;
        }
    }

    void FillTransitions()
    {
        for (int x = 0; x < MapSizeInTiles; x++)
        {
            for (int y = 0; y < MapSizeInTiles; y++)
            {
                if (cellMap[x][y] == CellType.Transition &&
                    (x > 0 && y > 0 && x < MapSizeInTiles - 1 && y < MapSizeInTiles - 1))
                {
                    var cellType = biomeMap[x / ChunkSize][y / ChunkSize] == BiomeType.Path ? CellType.Path : CellType.Water;
                    FillTyleTypeForTransitionCell(x, y, cellType);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    class TransitionStorage
    {
        CellType cellType;
        List<Tile> GroundTiles;
        List<Tile> WaterTiles;

        public TransitionStorage(CellType cellType, MapGenerator mapGenerator)
        {
            this.cellType = cellType;
            this.WaterTiles = mapGenerator.WaterTiles;
            this.GroundTiles = mapGenerator.GroundTiles;
        }

        public Tile GetTransitionTile(TransitionType transitionType)
        {
            var tile = new Tile();
            switch (transitionType)
            {
                case TransitionType.FlatUp:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.FlatUpWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.FlatUpPath];
                        }
                    }
                    break;
                case TransitionType.FlatDown:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.FlatDownWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.FlatDownPath];
                        }
                    }
                    break;
                case TransitionType.FlatRight:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.FlatRightWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.FlatRightPath];
                        }
                    }
                    break;
                case TransitionType.FlatLeft:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.FlatLeftWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.FlatLeftPath];
                        }
                    }
                    break;
                case TransitionType.DoubleFlatUpRight:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.DoubleFlatUpRightWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.DoubleFlatUpRightPath];
                        }
                    }
                    break;
                case TransitionType.DoubleFlatUpLeft:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.DoubleFlatUpLeftWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.DoubleFlatUpLeftPath];
                        }
                    }
                    break;
                case TransitionType.DoubleFlatDownRight:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.DoubleFlatDownRightWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.DoubleFlatDownRightPath];
                        }
                    }
                    break;
                case TransitionType.DoubleFlatDownLeft:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.DoubleFlatDownLeftWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.DoubleFlatDownLeftPath];
                        }
                    }
                    break;
                case TransitionType.CornerUpRight:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.CornerUpRightWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.CornerUpRightPath];
                        }
                    }
                    break;
                case TransitionType.CornerUpLeft:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.CornerUpLeftWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.CornerUpLeftPath];
                        }
                    }
                    break;
                case TransitionType.CornerDownLeft:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.CornerDownLeftWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.CornerDownLeftPath];
                        }
                    }
                    break;
                case TransitionType.CornerDownRight:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.CornerDownRightWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.CornerDownRightPath];
                        }
                    }
                    break;
                case TransitionType.Full:
                    {
                        if (cellType == CellType.Water)
                        {
                            tile = WaterTiles[(int)TileMapperWater.FullWater];
                        }
                        else if (cellType == CellType.Path)
                        {
                            tile = GroundTiles[(int)TileMapperGround.FullPath];
                        }
                    }
                    break;
            }
            return tile;
        }
    }
}
