﻿using UnityEngine;
using System;
using Grid = Assets.Scripts.Grid.Grid;

namespace Library
{
    [Serializable]

    public class Map : MonoBehaviour
    {
        private int mapSize = 50;

        private Grid grid;

        private BaseTile[,] tiles;
        public float budget { get; set; }
        public int zoneSize { get; set; } = 3;
        public float zoneBrightness { get; set; } = 0.5f;

        [SerializeField] private Grid gridPrefab;
        [SerializeField] private GrassTile grassTilePrefab;
        [SerializeField] private WaterTile waterTilePrefab;
        [SerializeField] private AsphaltTile asphaltTilePrefab;

        public Map Initialize(int size)
        {
            mapSize = size;

            init();

            return this;
        }

        private void init()
        {
            tiles = new BaseTile[mapSize, mapSize];

            CreateGrid();
        }

        private void CreateGrid()
        {
            grid = Instantiate(gridPrefab, transform).Initialize(mapSize);
        }

        public void GenerateGrassMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    tiles[i, j] = Instantiate(grassTilePrefab, grid.GetTransform(i, j));
                    tiles[i, j].transform.position = new Vector3(i, 0, j);
                    tiles[i, j].Coordinate = new Vector2(i, j);
                    grid.SetAvailable(i, j, false);
                }
            }
        }

        public void GenerateStandardMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if ((i == 2 || i == 3) && (j == 2 || j == 3) || ((i == 3 || i == 4) && (j == 4 || j == 5))
                                                                 || (i == 45 && j < 50 && j > 43) ||
                                                                 (j == 43 && i > 45 && i < 50) || (i == 45 && j == 43)
                                                                 || ((i == 28 || i == 29 || i == 30) &&
                                                                     (j == 23 || j == 24 || j == 25)))
                    {
                        tiles[i, j] = Instantiate(waterTilePrefab, grid.GetTransform(i, j));
                    }
                    else if (i == 7 || i == 8 || i == 15 || i == 16 || i == 25 || i == 26 || i == 40 || i == 41
                             || j == 10 || j == 11 || j == 20 || j == 21 || j == 30 || j == 31 || j == 40 || j == 41
                             || (i == 35 && j < 10 && j >= 0) || (j == 6 && i > 35 && i < 40) ||
                             (j == 5 && (i > 16 && i < 25))
                             || (i == 33 && (j > 10 && j < 20)))

                    {
                        tiles[i, j] = Instantiate(asphaltTilePrefab, grid.GetTransform(i, j));
                    }
                    else
                    {
                        tiles[i, j] = Instantiate(grassTilePrefab, grid.GetTransform(i, j));
                    }

                    tiles[i, j].transform.position = new Vector3(i, 0, j);
                    tiles[i, j].Coordinate = new Vector2(i, j);
                    grid.SetAvailable(i, j, false);
                }
            }
        }

        public void SetTile(BaseTile tile)
        {
            tiles[(int)tile.Coordinate.x, (int)tile.Coordinate.y] = tile;
        }

        public BaseTile GetTile(Vector2 coordinate)
        {
            return tiles[(int)coordinate.x, (int)coordinate.y];
        }

        public BaseTile GetTileByCoordinates(int x, int y)
        {
            return tiles[x, y];
        }

        public void CreateBaseTile(BaseTile prefab, GridCell cell)
        {
            Instantiate(prefab, cell.transform).Initialize(cell.GridCoordinate);
            cell.IsAvailable = false;
        }

        public void UpdateTileType(BaseTile selectedTile, BaseTile newTile)
        {
            Transform parent = selectedTile.transform.parent;
            Vector2 position = selectedTile.Coordinate;

            foreach (var child in parent.transform.GetComponentsInChildren<BaseTile>())
            {
                Destroy(child.gameObject);
            }

            BaseTile tile = Instantiate(newTile, parent).Initialize(position);

            SetTile(tile);

            Debug.Log("UpdateTileType: "
                      + tile.Coordinate.x + ", "
                      + tile.Coordinate.y);
        }

        public void SetInactiveTile(Vector2 coordinate, State state1, State state2)
        {
            if (IsZoneSizeEven())
            {
                BuildZoneEvenSize(zoneSize / 2, coordinate.x, coordinate.y, state1, state2);
            }
            else
            {
                BuildZoneOddSize((zoneSize - 1) / 2, coordinate.x, coordinate.y, state1, state2);
            }
        }

        public void BuildZoneEvenSize(int size, float x, float y, State state1, State state2)
        {
            for (int i = -size; i < size; i++)
            {
                for (int j = -size; j < size; j++)
                {
                    int coordinateX = (int) x + i;
                    int coordinateY = (int) y + j;

                    if (!OutsideGrid(coordinateX, coordinateY))
                    {
                        UpdateZoneState(coordinateX, coordinateY, state1, state2);

                    }
                }
            }
        }

        public void BuildZoneOddSize(int size, float x, float y, State state1, State state2)
        {
            for (int i = -size; i <= size; i++)
            {
                for (int j = -size; j <= size; j++)
                {
                    int coordinateX = (int) x + i;
                    int coordinateY = (int) y + j;

                    if (!OutsideGrid (coordinateX, coordinateY))
                    {
                        UpdateZoneState(coordinateX, coordinateY, state1, state2);

                    }
                }
            }
        }

        private bool OutsideGrid(int coordinateX, int coordinateY)
        {
            return coordinateX < 0 || coordinateX > (mapSize - 1) || coordinateY < 0 || coordinateY > (mapSize - 1);
        }

        private void UpdateZoneState(int coordinateX, int coordinateY, State state1, State state2)
        {
            BaseTile tile = GetTileByCoordinates(coordinateX, coordinateY);
            if (tile.State == state1)
            {
                tile.GetComponentInChildren<Renderer>().material.color *= zoneBrightness;
                GetTileByCoordinates(coordinateX, coordinateY).State = state2;
            }
        }

        private bool IsZoneSizeEven()
        {
            return zoneSize % 2 == 0;
        }
    }
}
