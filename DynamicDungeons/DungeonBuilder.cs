using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons
{
    public class DungeonBuilder
    {
        private static Random _Random = new Random();
        private const float _ChanceToStartAlive = 0.4f;
        private const int _DeathLimit = 3;
        private const int _BirthLimit = 4;
        private const int _SimulationSteps = 30;

        public int Seed { get; private set; }
        private bool[,] Map;
        private Color[,] CMap;
        private Random Seeder;
        private int Width;
        private int Height;
        private int Floor;
        private double Difficulty;
        public DungeonBuilder(double difficulty, int floor)
        {
            this.Difficulty = difficulty;
            this.Floor = floor;
            this.Seed = _Random.Next();
            this.GenerateMap();
        }
        public DungeonBuilder(double difficulty, int floor, int seed)
        {
            this.Difficulty = difficulty;
            this.Floor = floor;
            this.Seed = seed;
            this.GenerateMap();
        }
        private void Report(string message)
        {
            DynamicDungeonsMod.SMonitor.Log("DDGenerator: " + message, StardewModdingAPI.LogLevel.Info);
        }
        private void ColorConvert()
        {
            this.CMap = new Color[this.Width, this.Height];
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    this.CMap[x, y] = this.Map[x, y] ? Color.Black : Color.White;
        }
        private void GenerateMap()
        {
            Report("Trying to generate from seed: " + this.Seed.ToString("X8"));
            // Setup the seeder that this map will be using
            this.Seeder = new Random(this.Seed);
            // Define the width & height
            this.Width = this.Seeder.Next(40, 48);
            this.Height = this.Seeder.Next(40, 48);
            // Setup the map
            this.Map = new bool[this.Width, this.Height];
            // Seed the initial automaton state
            Report("Creating seed map...");
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    if(this.Seeder.NextDouble() < _ChanceToStartAlive)
                        this.Map[x, y] = true;
            Report("Simulating map...");
            this.Simulate(1);
        }
        private void Simulate(int attempt)
        {
            Report("Attempt "+attempt+": Simulation part 1...");
            for (int s = 0; s < _SimulationSteps; s++)
                NextStep();
            Report("Attempt " + attempt + ": Simulation part 2...");
            for (int s = 0; s < _SimulationSteps * 4; s++)
                NextClean();
            Report("Attempt " + attempt + ": Converting structure...");
            ColorConvert();
            // Perform flood filling to find & isolate a large dungeon region
            Report("Attempt " + attempt + ": Trying to validate map...");
            if (!FloodFill())
            {
                if (attempt < 3)
                {
                    Report("Attempt " + attempt + " failed to validate, trying again...");
                    this.Simulate(attempt + 1);
                }
                else
                {
                    Report("Attempt " + attempt + " failed to validate, retry limit reached, discarding map and starting over...");
                    this.GenerateMap();
                }
            }
            else
            {
                Report("Attempt " + attempt + ": Validation success!");
                Report("Y-Scaling...");
                this.Upscale();
                Report("Encapsulating...");
                this.Encapsulate();
                Report("Removing wall-locked tiles...");
                this.WipeBounds();
                Report("Map generation complete");
            }
        }
        private Color GetTileColor(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
                return Color.Black;
            else
                return this.CMap[x, y];
        }
        private int GetColorCount(Color color)
        {
            int count = 0;
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    if (this.CMap[x, y] == color)
                        count++;
            return count;
        }
        private Color GetNeighborsColor(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
                return Color.Transparent;
            else
                return this.CMap[x, y];
        }
        private int GetNeighborCountColor(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (GetTileColor(x + i, y + j)!=Color.White)
                        count++;
                }
            return count;
        }
        private bool GetNeighbor(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
                return true;
            else
                return this.Map[x, y];
        }
        private int GetNeighborCount(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (GetNeighbor(x + i, y + j))
                        count++;
                }
            return count;
        }
        private int GetNeighborCountX(int x, int y)
        {
            int count = 0;
            if (GetNeighbor(x - 1, y))
                count++;
            if (GetNeighbor(x + 1, y))
                count++;
            return count;
        }
        private int GetNeighborCountY(int x, int y)
        {
            int count = 0;
            if (GetNeighbor(x, y - 1))
                count++;
            if (GetNeighbor(x, y + 2))
                count++;
            return count;
        }
        private void NextStep()
        {
            bool[,] updated = new bool[this.Width, this.Height];
            Parallel.For(0, this.Width, (x) =>{
                Parallel.For(0, this.Height, (y) =>
                {
                    int count = this.GetNeighborCount(x, y);
                    if (this.Map[x, y])
                    {
                        if (count < _DeathLimit)
                            updated[x, y] = false;
                        else
                            updated[x, y] = true;
                    }
                    else
                    {
                        if (count > _BirthLimit)
                            updated[x, y] = true;
                        else
                            updated[x, y] = false;
                    }
                });
            });
            /*
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                {
                }
            */
            this.Map = updated;
        }
        private void NextClean()
        {
            bool[,] updated = new bool[this.Width, this.Height];
            Parallel.For(0, this.Width, (x) =>
            {
                Parallel.For(0, this.Height, (y) =>
                {
                    if (this.Map[x, y])
                    {
                        int xc = this.GetNeighborCountX(x, y);
                        int yc = this.GetNeighborCountY(x, y);
                        if (xc + yc < 3 && (xc == 0 || yc == 0))
                            updated[x, y] = false;
                        else
                            updated[x, y] = true;
                    }
                    else
                    {
                        updated[x, y] = false;
                    }
                });
            });
            this.Map = updated;
        }
        private bool FloodFill()
        {
            for(int x=0;x<this.Width;x++)
                for(int y=0;y<this.Height;y++)
                {
                    if (this.CMap[x,y]==Color.White)
                    {
                        int floods=Flood(x, y);
                        if (floods > ((this.Width * this.Height) / 3))
                        {
                            FloodAway(Color.White);
                            FloodAway(Color.Red, Color.White);
                            Report("Validation found a valid region: " + floods);
                            return true;
                        }
                        else
                        {
                            Report("Validation failed to find a valid region, retrying...");
                            FloodAway(Color.Red);
                            return FloodFill();
                        }
                    }
                }
            Report("Validation attempt failed, returning control to the map generator...");
            this.Seed = this.Seeder.Next();
            return false;
        }
        private void FloodAway(Color color, Color? replacement=null)
        {
            Color outColor = replacement ?? Color.Black;
            Color[,] updated = new Color[this.Width, this.Height];
            Parallel.For(0, this.Width, (x) =>
            {
                Parallel.For(0, this.Height, (y) =>
                {
                    if (this.CMap[x, y] == color)
                        updated[x, y] = outColor;
                    else
                        updated[x, y] = this.CMap[x, y];
                });
            });
            this.CMap = updated;
        }
        private int Flood(int x, int y)
        {
            this.CMap[x, y] = Color.Red;
            int floods = 1;
            if (GetTileColor(x - 1, y) == Color.White)
                floods += Flood(x - 1, y);
            if (GetTileColor(x + 1, y) == Color.White)
                floods += Flood(x + 1, y);
            if (GetTileColor(x, y - 1) == Color.White)
                floods += Flood(x, y - 1);
            if (GetTileColor(x, y + 1) == Color.White)
                floods += Flood(x, y + 1);
            return floods;
        }
        private void Encapsulate()
        {
            Color[,] updated = new Color[this.Width+8, this.Height+8];
            Parallel.For(0, this.Width + 8, (x) =>
            {
                Parallel.For(0, this.Height + 8, (y) =>
                {
                    updated[x, y] = Color.Black;
                });
            });
            Parallel.For(0, this.Width, (x) =>
            {
                Parallel.For(0, this.Height, (y) =>
                {
                    updated[x + 4, y + 4] = this.CMap[x, y];
                });
            });
            this.Width += 8;
            this.Height += 8;
            this.CMap = updated;
        }
        private void WipeBounds()
        {
            Color[,] updated = new Color[this.Width, this.Height];
            Parallel.For(0, this.Width, (x) =>
            {
                Parallel.For(0, this.Height, (y) =>
                {
                    if (this.CMap[x, y] != Color.White && this.GetNeighborCountColor(x, y) == 8)
                        updated[x, y] = Color.Transparent;
                    else
                        updated[x, y] = this.CMap[x, y];
                });
            });
            this.CMap = updated;
        }
        private void Upscale()
        {
            Color[,] updated = new Color[this.Width*2, this.Height*2];
            Parallel.For(0, this.Width * 2, (x) =>
            {
                Parallel.For(0, this.Height * 2, (y) =>
                {
                    updated[x, y] = this.CMap[(int)Math.Floor(x / 2f), (int)Math.Floor(y / 2f)];
                });
            });
            this.Height *= 2;
            this.Width *= 2;
            this.CMap = updated;
        }
        private byte GetSurroundingWalls(int x, int y)
        {
            byte count = 0;
            byte add = 128;
            for (int j = -1; j < 2; j++)
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (GetTileColor(x+i , y+j)!=Color.White)
                        count += add;
                    add /= 2;
                }
            return count;
        }
        private Point? GetRandomGroundPoint(ref Layer floor, ref Layer wall)
        {
            for (int x = this.Seeder.Next(0, this.Width-1); x < this.Width; x++)
                for (int y = this.Seeder.Next(0, this.Height-1); y < this.Height; y++)
                {
                    if (floor.Tiles[x, y] != null && floor.Tiles[x, y].TileIndex == 138 && wall.Tiles[x,y]==null)
                        return new Point(x, y);
                }
            return null;
        }
        private bool FindRegionOfSize(ref Map map, int width, int height, int weight, out Point point)
        {
            point = default(Point);
            Point? origin = null;
            Layer floor = map.GetLayer("Back");
            Layer wall = map.GetLayer("Buildings");
            for (float t = 0; t <= weight; t++)
            {
                origin = GetRandomGroundPoint(ref floor, ref wall);
                if (origin != null)
                    return FindRegionOfSize(ref map, width, height, weight, (Point)origin, out point);
            }
            return false;
        }
        private bool FindRegionOfSize(ref Map map, int width, int height, int weight, Point start, out Point point)
        {
            point = default(Point);
            Layer floor = map.GetLayer("Back");
            Layer wall = map.GetLayer("Buildings");
            bool invalid = false;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (floor.Tiles[start.X + i, start.Y + j] == null || floor.Tiles[start.X + i, start.Y + j].TileIndex != 138 || wall.Tiles[start.X + i, start.Y + j] != null)
                        invalid = true;
                    if (invalid)
                        break;
                }
            if (!invalid)
            {
                point = start;
                return true;
            }
            return false;
        }
        private void BuildStructure(ref Map map, string name, int width, int height, int weight, int minSpawn, int maxSpawn, ITile[] tiles)
        {
            BuildStructure(ref map, name, width, height, weight, minSpawn, maxSpawn, () => tiles);
        }
        private void BuildStructure(ref Map map, string name, int width, int height, int weight, int minSpawn, int maxSpawn, Func<ITile[]> tiles)
        {
            if (minSpawn != maxSpawn)
                minSpawn = this.Seeder.Next(0, minSpawn);
            else
                minSpawn = 0;
            for (int c = minSpawn; c < maxSpawn; c++)
                if (FindRegionOfSize(ref map, width, height, weight, out var point))
                {
                    Report("Structure spawned: "+name);
                    var mytiles = tiles();
                    Parallel.ForEach(mytiles, tile => {
                        if (tile is PTile)
                            return;
                        tile.Layer.Tiles[point.X + tile.X, point.Y + tile.Y] = tile.Get();
                    });
                    Parallel.ForEach(mytiles, tile => {
                        if(tile is PTile)
                            tile.Layer.Tiles[point.X + tile.X, point.Y + tile.Y].Properties.Add(((PTile)tile).Key, ((PTile)tile).Value);
                    });
                }
                else
                    Report("Spawn attempt failed: " + name);
        }
        private Color[,] GeneratePatch(int width=7, int height=7)
        {
            List<(int, int)> GetAdjacentTiles(ref Color[,] area, int x, int y)
            {
                List<(int, int)> points = new List<(int, int)>();
                if (x - 1 >= 0 && area[x - 1, y] != Color.White)
                    points.Add((x - 1, y));
                if (x + 1 < area.GetLength(0) && area[x + 1, y] != Color.White)
                    points.Add((x + 1, y));
                if (y - 1 >= 0 && area[x, y - 1] != Color.White)
                    points.Add((x, y - 1));
                if (y + 1 < area.GetLength(1) && area[x, y + 1] != Color.White)
                    points.Add((x, y + 1));
                return points;
            }
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            int GetNeighborCount(ref Color[,] area, int x, int y)
            {
                int count = 0;
                for (int i = -1; i < 2; i++)
                    for (int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) == Color.White)
                            count++;
                    }
                return count;
            }
            void RemoveObstructedWalls(ref Color[,] area)
            {
                for (int x = 0; x < area.GetLength(0); x++)
                    for (int y = 0; y < area.GetLength(1); y++)
                        if (GetNeighborCount(ref area, x, y) == 0)
                            area[x, y] = Color.Transparent;
            }
            var reg = new Color[width, height];
            for (int x = 0; x < reg.GetLength(0); x++)
                for (int y = 0; y < reg.GetLength(1); y++)
                    reg[x, y] = Color.Black;
            reg[width/2, height/2] = Color.White;
            List<(int, int)> pos = new List<(int, int)>();
            for (int c = 0; c < this.Seeder.Next(width*height/4, width*height/3); c++)
            {
                pos.Clear();
                for (int x = 0; x < reg.GetLength(0); x++)
                    for (int y = 0; y < reg.GetLength(1); y++)
                    {
                        if (reg[x, y] == Color.White)
                            pos.AddRange(GetAdjacentTiles(ref reg, x, y));
                    }
                var match = pos[this.Seeder.Next(0, pos.Count)];
                reg[match.Item1, match.Item2] = Color.White;
            }
            for (int c = 0; c < 10; c++)
            {
                var copy = new Color[reg.GetLength(0), reg.GetLength(1)];
                for (int x = 0; x < reg.GetLength(0); x++)
                    for (int y = 0; y < reg.GetLength(1); y++)
                    {
                        if (reg[x, y] != Color.White && GetNeighborCount(ref reg, x, y) > 4)
                            copy[x, y] = Color.White;
                        else
                            copy[x, y] = reg[x, y];
                    }
                reg = copy;
            }
            var mapping = new Color[reg.GetLength(0)+2,reg.GetLength(1)+2];
            for (int x = 0; x < mapping.GetLength(0); x++)
                for (int y = 0; y < mapping.GetLength(1); y++)
                    if (y > 0 && y < 8 && x > 0 && x < 8)
                        mapping[y, x] = GetNeighbor(ref reg, x - 1, y - 1);
                    else
                        mapping[y, x] = Color.Black;
            RemoveObstructedWalls(ref mapping);
            return mapping;
        }
        private ITile[] GetRandomWater(ref Layer back, ref Layer wall, ref TileSheet sheet)
        {
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            byte GetSurroundingWalls(ref Color[,] area, int x, int y)
            {
                byte count = 0;
                byte add = 128;
                for (int j = -1; j < 2; j++)
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) != Color.White)
                            count += add;
                        add /= 2;
                    }
                return count;
            }
            var mapping = GeneratePatch();
            List<ITile> tiles = new List<ITile>();
            for (int x = 0; x < mapping.GetLength(0); x++)
                for (int y = 0; y < mapping.GetLength(1); y++)
                {
                    if (mapping[x, y] == Color.Transparent)
                        continue;
                    if (this.Seeder.NextDouble() < 0.2f)
                            tiles.Add(new STile(x, y, back, sheet, 260 + ((x + y) % 2) * 16));
                    else if (this.Seeder.NextDouble() < 0.1f)
                        tiles.Add(new STile(x, y, back, sheet, 277));
                    else
                            tiles.Add(new STile(x, y, back, sheet, 275));
                    if (mapping[x, y] == Color.Black)
                    {
                        switch (GetSurroundingWalls(ref mapping, x, y))
                        {
                            // Top-center wall
                            case 0b111_1_1_000:
                            case 0b111_1_1_100:
                            case 0b111_1_1_101:
                            case 0b111_1_1_001:
                                tiles.Add(new STile(x, y, wall, sheet, 244));
                                break;
                            // Bottom-center wall
                            case 0b000_1_1_111:
                            case 0b100_1_1_111:
                            case 0b101_1_1_111:
                            case 0b001_1_1_111:
                                tiles.Add(new STile(x, y, wall, sheet, 279));
                                break;
                            // Left-center wall
                            case 0b110_1_0_110:
                            case 0b111_1_0_110:
                            case 0b111_1_0_111:
                            case 0b110_1_0_111:
                                tiles.Add(new STile(x, y, wall, sheet, 259));
                                break;
                            // Right-center wall
                            case 0b011_0_1_011:
                            case 0b111_0_1_011:
                            case 0b111_0_1_111:
                            case 0b011_0_1_111:
                                tiles.Add(new STile(x, y, wall, sheet, 261));
                                break;
                            // top-left inner corner
                            case 0b111_1_0_000:
                            case 0b111_1_0_100:
                            case 0b110_1_0_100:
                            case 0b110_1_0_000:
                                tiles.Add(new STile(x, y, wall, sheet, 249));
                                break;
                            // top-right inner corner
                            case 0b111_0_1_000:
                            case 0b111_0_1_001:
                            case 0b011_0_1_001:
                            case 0b011_0_1_000:
                                tiles.Add(new STile(x, y, wall, sheet, 250));
                                break;
                            // bottom-right inner corner
                            case 0b001_0_1_111:
                            case 0b000_0_1_111:
                            case 0b000_0_1_011:
                            case 0b001_0_1_011:
                                tiles.Add(new STile(x, y, wall, sheet, 267));
                                break;
                            // bottom-left inner corner
                            case 0b100_1_0_111:
                            case 0b000_1_0_111:
                            case 0b000_1_0_110:
                            case 0b100_1_0_110:
                                tiles.Add(new STile(x, y, wall, sheet, 251));
                                break;
                            case 0b111_1_1_011:
                                tiles.Add(new STile(x, y, wall, sheet, 245));
                                break;
                            case 0b111_1_1_110:
                                tiles.Add(new STile(x, y, wall, sheet, 243));
                                break;
                            case 0b011_1_1_111:
                                tiles.Add(new STile(x, y, wall, sheet, 280));
                                break;
                            case 0b110_1_1_111:
                                tiles.Add(new STile(x, y, wall, sheet, 278));
                                break;
                        }
                    }
                }
            return tiles.ToArray();
        }
        private ITile[] GetRandomDirt(ref Layer back, ref Layer wall, ref TileSheet sheet)
        {
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            byte GetSurroundingWalls(ref Color[,] area, int x, int y)
            {
                byte count = 0;
                byte add = 128;
                for (int j = -1; j < 2; j++)
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) != Color.White)
                            count += add;
                        add /= 2;
                    }
                return count;
            }
            var mapping = GeneratePatch();
            List<ITile> tiles = new List<ITile>();
            for (int x = 0; x < mapping.GetLength(0); x++)
                for (int y = 0; y < mapping.GetLength(1); y++)
                {
                    if (mapping[x, y] == Color.Transparent)
                        continue;
                    if(mapping[x,y]==Color.White)
                        if (this.Seeder.NextDouble() < 0.2f)
                            tiles.Add(new STile(x, y, back, sheet, 165 + ((x + y) % 2) * 16));
                        else
                            tiles.Add(new STile(x, y, back, sheet, 183));
                    else if(mapping[x,y]==Color.Black)
                    {
                        switch(GetSurroundingWalls(ref mapping, x, y))
                        {
                            // Top-center wall
                            case 0b111_1_1_000:
                            case 0b111_1_1_100:
                            case 0b111_1_1_101:
                            case 0b111_1_1_001:
                                    tiles.Add(new STile(x, y, back, sheet, 167));
                                break;
                            // Bottom-center wall
                            case 0b000_1_1_111:
                            case 0b100_1_1_111:
                            case 0b101_1_1_111:
                            case 0b001_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 199));
                                break;
                            // Left-center wall
                            case 0b110_1_0_110:
                            case 0b111_1_0_110:
                            case 0b111_1_0_111:
                            case 0b110_1_0_111:
                                tiles.Add(new STile(x, y, back, sheet, 182));
                                break;
                            // Right-center wall
                            case 0b011_0_1_011:
                            case 0b111_0_1_011:
                            case 0b111_0_1_111:
                            case 0b011_0_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 184));
                                break;
                            // top-left inner corner
                            case 0b111_1_0_000:
                            case 0b111_1_0_100:
                            case 0b110_1_0_100:
                            case 0b110_1_0_000:
                                tiles.Add(new STile(x, y, back, sheet, 152));
                                break;
                            // top-right inner corner
                            case 0b111_0_1_000:
                            case 0b111_0_1_001:
                            case 0b011_0_1_001:
                            case 0b011_0_1_000:
                                tiles.Add(new STile(x, y, back, sheet, 151));
                                break;
                            // bottom-right inner corner
                            case 0b001_0_1_111:
                            case 0b000_0_1_111:
                            case 0b000_0_1_011:
                            case 0b001_0_1_011:
                                tiles.Add(new STile(x, y, back, sheet, 150));
                                break;
                            // bottom-left inner corner
                            case 0b100_1_0_111:
                            case 0b000_1_0_111:
                            case 0b000_1_0_110:
                            case 0b100_1_0_110:
                                tiles.Add(new STile(x, y, back, sheet, 149));
                                break;
                            case 0b111_1_1_011:
                                tiles.Add(new STile(x, y, back, sheet, 168));
                                break;
                            case 0b111_1_1_110:
                                tiles.Add(new STile(x, y, back, sheet, 166));
                                break;
                            case 0b011_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 200));
                                break;
                            case 0b110_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 198));
                                break;
                        }
                    }
                }
            return tiles.ToArray();
        }
        private ITile[] GetRandomWood(ref Layer back, ref Layer wall, ref TileSheet sheet)
        {
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            byte GetSurroundingWalls(ref Color[,] area, int x, int y)
            {
                byte count = 0;
                byte add = 128;
                for (int j = -1; j < 2; j++)
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) != Color.White)
                            count += add;
                        add /= 2;
                    }
                return count;
            }
            var mapping = GeneratePatch(5,5);
            List<ITile> tiles = new List<ITile>();
            for (int x = 0; x < mapping.GetLength(0); x++)
                for (int y = 0; y < mapping.GetLength(1); y++)
                {
                    if (mapping[x, y] == Color.Transparent)
                        continue;
                    if (mapping[x, y] == Color.White)
                            tiles.Add(new STile(x, y, back, sheet, 257));
                    else if (mapping[x, y] == Color.Black)
                    {
                        switch (GetSurroundingWalls(ref mapping, x, y))
                        {
                            // Top-center wall
                            case 0b111_1_1_000:
                            case 0b111_1_1_100:
                            case 0b111_1_1_101:
                            case 0b111_1_1_001:
                                tiles.Add(new STile(x, y, back, sheet, 241));
                                break;
                            // Bottom-center wall
                            case 0b000_1_1_111:
                            case 0b100_1_1_111:
                            case 0b101_1_1_111:
                            case 0b001_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 273));
                                break;
                            // Left-center wall
                            case 0b110_1_0_110:
                            case 0b111_1_0_110:
                            case 0b111_1_0_111:
                            case 0b110_1_0_111:
                                tiles.Add(new STile(x, y, back, sheet, 256));
                                break;
                            // Right-center wall
                            case 0b011_0_1_011:
                            case 0b111_0_1_011:
                            case 0b111_0_1_111:
                            case 0b011_0_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 258));
                                break;
                            // inner corners
                            case 0b111_1_0_000:
                            case 0b111_1_0_100:
                            case 0b110_1_0_100:
                            case 0b110_1_0_000:
                            case 0b111_0_1_000:
                            case 0b111_0_1_001:
                            case 0b011_0_1_001:
                            case 0b011_0_1_000:
                            case 0b001_0_1_111:
                            case 0b000_0_1_111:
                            case 0b000_0_1_011:
                            case 0b001_0_1_011:
                            case 0b100_1_0_111:
                            case 0b000_1_0_111:
                            case 0b000_1_0_110:
                            case 0b100_1_0_110:
                                tiles.Add(new STile(x, y, back, sheet, 257));
                                break;
                            case 0b111_1_1_011:
                                tiles.Add(new STile(x, y, back, sheet, 242));
                                break;
                            case 0b111_1_1_110:
                                tiles.Add(new STile(x, y, back, sheet, 240));
                                break;
                            case 0b011_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 274));
                                break;
                            case 0b110_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 272));
                                break;
                        }
                    }
                }
            return tiles.ToArray();
        }
        private ITile[] GetRandomCobble(ref Layer back, ref Layer wall, ref TileSheet sheet)
        {
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            byte GetSurroundingWalls(ref Color[,] area, int x, int y)
            {
                byte count = 0;
                byte add = 128;
                for (int j = -1; j < 2; j++)
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) != Color.White)
                            count += add;
                        add /= 2;
                    }
                return count;
            }
            var mapping = GeneratePatch(5, 5);
            List<ITile> tiles = new List<ITile>();
            for (int x = 0; x < mapping.GetLength(0); x++)
                for (int y = 0; y < mapping.GetLength(1); y++)
                {
                    if (mapping[x, y] == Color.Transparent)
                        continue;
                    if (mapping[x, y] == Color.White)
                        tiles.Add(new STile(x, y, back, sheet, 18));
                    else if (mapping[x, y] == Color.Black)
                    {
                        switch (GetSurroundingWalls(ref mapping, x, y))
                        {
                            // Top-center wall
                            case 0b111_1_1_000:
                            case 0b111_1_1_100:
                            case 0b111_1_1_101:
                            case 0b111_1_1_001:
                                tiles.Add(new STile(x, y, back, sheet, 2));
                                break;
                            // Bottom-center wall
                            case 0b000_1_1_111:
                            case 0b100_1_1_111:
                            case 0b101_1_1_111:
                            case 0b001_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 34));
                                break;
                            // Left-center wall
                            case 0b110_1_0_110:
                            case 0b111_1_0_110:
                            case 0b111_1_0_111:
                            case 0b110_1_0_111:
                                tiles.Add(new STile(x, y, back, sheet, 17));
                                break;
                            // Right-center wall
                            case 0b011_0_1_011:
                            case 0b111_0_1_011:
                            case 0b111_0_1_111:
                            case 0b011_0_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 19));
                                break;
                            // inner corners
                            case 0b111_1_0_000:
                            case 0b111_1_0_100:
                            case 0b110_1_0_100:
                            case 0b110_1_0_000:
                            case 0b111_0_1_000:
                            case 0b111_0_1_001:
                            case 0b011_0_1_001:
                            case 0b011_0_1_000:
                            case 0b001_0_1_111:
                            case 0b000_0_1_111:
                            case 0b000_0_1_011:
                            case 0b001_0_1_011:
                            case 0b100_1_0_111:
                            case 0b000_1_0_111:
                            case 0b000_1_0_110:
                            case 0b100_1_0_110:
                                tiles.Add(new STile(x, y, back, sheet, 18));
                                break;
                            case 0b111_1_1_011:
                                tiles.Add(new STile(x, y, back, sheet, 3));
                                break;
                            case 0b111_1_1_110:
                                tiles.Add(new STile(x, y, back, sheet, 1));
                                break;
                            case 0b011_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 35));
                                break;
                            case 0b110_1_1_111:
                                tiles.Add(new STile(x, y, back, sheet, 33));
                                break;
                        }
                    }
                }
            return tiles.ToArray();
        }
        private void GenerateTrack(Map map, TileSheet sheet)
        {
            Color GetNeighbor(ref Color[,] area, int x, int y)
            {
                if (x < 0 || y < 0 || x >= area.GetLength(0) || y >= area.GetLength(1))
                    return Color.Black;
                else
                    return area[x, y];
            }
            byte GetSurroundingWalls(ref Color[,] area, int x, int y)
            {
                byte count = 0;
                byte add = 128;
                for (int j = -1; j < 2; j++)
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (GetNeighbor(ref area, x + i, y + j) != Color.White)
                            count += add;
                        add /= 2;
                    }
                return count;
            }
            if (FindRegionOfSize(ref map, 1, 1, 500, out var start) && FindRegionOfSize(ref map, 1, 1, 500, new Point(start.X+this.Seeder.Next(-25,25),start.Y+this.Seeder.Next(-25,25)), out var end))
            {
                var backLayer = map.GetLayer("Back");
                var buildingsLayer = map.GetLayer("Buildings");
                List<Point> nodes=null;
                Color[,] mapping = new Color[backLayer.LayerWidth, backLayer.LayerHeight];
                var task = Task.Run(() => nodes = PathFinder.FindPath(map, start, end));
                Parallel.For(0, backLayer.LayerWidth, x =>
                {
                    Parallel.For(0, backLayer.LayerHeight, y => {
                        mapping[x, y] = Color.White;
                    });
                });
                task.Wait();
                if(nodes==null)
                {
                    Report("Spawn attempt failed: DynamicTrack (Path not found)");
                    return;
                }
                if(nodes.Count>50)
                {
                    Report("Spawn attempt failed: DynamicTrack (Path too long)");
                    return;
                }
                Report($"Structure spawned: DynamicTrack (from [{start.X},{start.Y}] to [{end.X},{end.Y}] over {nodes.Count} tiles)");
                Parallel.ForEach(nodes, node =>
                {
                    mapping[node.X, node.Y] = Color.Black;
                });
                Parallel.For(0, backLayer.LayerWidth, x =>
                {
                    Parallel.For(0, backLayer.LayerHeight, y => {
                        if(mapping[x,y]==Color.Black)
                        {
                            byte info = GetSurroundingWalls(ref mapping, x, y);
                            // Horizontal
                            if((info & 0b00_1_1_000) == 0b00_1_1_000)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 21 : 1);
                            // Vertical
                            else if ((info & 0b010_0_0_010) == 0b010_0_0_010)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 12 : 10);
                            // Top-left corner
                            else if ((info & 0b000_0_1_010) == 0b000_0_1_010)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, 0);
                            // Bottom-left corner
                            else if ((info & 0b010_0_1_000) == 0b010_0_1_000)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, 20);
                            // Top-right corner
                            else if ((info & 0b000_1_0_010) == 0b000_1_0_010)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, 2);
                            // Bottom-right corner
                            else if ((info & 0b010_1_0_000) == 0b010_1_0_000)
                                backLayer.Tiles[x, y] = new StaticTile(backLayer, sheet, BlendMode.Additive, 22);
                            // Bottom ending
                            else if ((info & 0b010_0_0_000) == 0b010_0_0_000)
                                buildingsLayer.Tiles[x, y] = new StaticTile(buildingsLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 6 : 16);
                            // Top ending
                            else if ((info & 0b000_0_0_010) == 0b000_0_0_010)
                                buildingsLayer.Tiles[x, y] = new StaticTile(buildingsLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 4 : 14);
                            // Left ending
                            else if ((info & 0b000_0_1_000) == 0b000_0_1_000)
                                buildingsLayer.Tiles[x, y] = new StaticTile(buildingsLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 3 : 13);
                            // Right ending
                            else if ((info & 0b000_1_0_000) == 0b000_1_0_000)
                                buildingsLayer.Tiles[x, y] = new StaticTile(buildingsLayer, sheet, BlendMode.Additive, (x + y) % 4 == 0 ? 5 : 15);
                            else
                                backLayer.Tiles[x, y] = null;
                        }
                    });
                });
            }
            else
                Report("Spawn attempt failed: DynamicTrack");
        }
        public Texture2D GetMiniMap()
        {
            Color[] mapping = new Color[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.CMap[x, y] == Color.White)
                        mapping[y * this.Width + x] = new Color(221, 148, 84);
                    else if (this.CMap[x, y] == Color.Black)
                        mapping[y * this.Width + x] = new Color(34, 17, 34);
                    else
                        mapping[y * this.Width + x] = this.CMap[x, y];
                }
            Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, this.Width, this.Height);
            texture.SetData(mapping);
            return texture;
        }
        public Map GetMap()
        {
            Report("Generating map data...");
            int[] floorTiles = { 137, 139, 140, 153, 154, 155, 169, 170, 171 };
            Map map = new Map("DynamicDungeons_" + this.Seed.ToString("X8"));
            map.Properties.Add("ViewportFollowPlayer", "True");
            map.Properties.Add("Outdoors", "True");
            TileSheet sheet = new TileSheet(map, "Mines\\mine", new Size(16, 18), new Size(16, 16));
            sheet.TileIndexProperties[165].Add("Diggable", "T");
            sheet.TileIndexProperties[181].Add("Diggable", "T");
            sheet.TileIndexProperties[183].Add("Diggable", "T");
            sheet.TileIndexProperties[275].Add("Water", "T");
            sheet.TileIndexProperties[276].Add("Water", "T");
            sheet.TileIndexProperties[277].Add("Water", "T");
            sheet.TileIndexProperties[260].Add("Water", "T");
            map.AddTileSheet(sheet);
            var bsheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_boulder.png"), new Size(6, 3), new Size(16, 16));
            map.AddTileSheet(bsheet);
            var csheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_cavein.png"), new Size(2, 2), new Size(16, 16));
            map.AddTileSheet(csheet);
            var s1sheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_stalagmite1.png"), new Size(1, 2), new Size(16, 16));
            map.AddTileSheet(s1sheet);
            var s2sheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_stalagmite2.png"), new Size(2, 3), new Size(16, 16));
            map.AddTileSheet(s2sheet);
            var vsheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_dwarfvendor.png"), new Size(8, 3), new Size(16, 16));
            map.AddTileSheet(vsheet);
            var sWalls = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_walls.png"), new Size(9, 24), new Size(16, 16));
            map.AddTileSheet(sWalls);
            var rsheet = new TileSheet(map, DynamicDungeonsMod.SHelper.Content.GetActualAssetKey("sheet_tracks.png"), new Size(10, 3), new Size(16, 16));
            rsheet.TileIndexProperties[3].Add("Passable", "T");
            rsheet.TileIndexProperties[4].Add("Passable", "T");
            rsheet.TileIndexProperties[5].Add("Passable", "T");
            rsheet.TileIndexProperties[6].Add("Passable", "T");
            rsheet.TileIndexProperties[7].Add("Passable", "T");
            rsheet.TileIndexProperties[8].Add("Passable", "T");
            rsheet.TileIndexProperties[9].Add("Passable", "T");
            rsheet.TileIndexProperties[13].Add("Passable", "T");
            rsheet.TileIndexProperties[14].Add("Passable", "T");
            rsheet.TileIndexProperties[15].Add("Passable", "T");
            rsheet.TileIndexProperties[16].Add("Passable", "T");
            rsheet.TileIndexProperties[17].Add("Passable", "T");
            rsheet.TileIndexProperties[18].Add("Passable", "T");
            rsheet.TileIndexProperties[19].Add("Passable", "T");
            rsheet.TileIndexProperties[27].Add("Passable", "T");
            rsheet.TileIndexProperties[28].Add("Passable", "T");
            rsheet.TileIndexProperties[29].Add("Passable", "T");
            map.AddTileSheet(rsheet);
            Layer floor = new Layer("Back", map, new Size(this.Width, this.Height), new Size(64, 64));
            map.AddLayer(floor);
            Layer wall = new Layer("Buildings", map, new Size(this.Width, this.Height), new Size(64, 64));
            map.AddLayer(wall);
            Layer front = new Layer("Front", map, new Size(this.Width, this.Height), new Size(64, 64));
            map.AddLayer(front);
            Layer afront = new Layer("AlwaysFront", map, new Size(this.Width, this.Height), new Size(64, 64));
            map.AddLayer(afront);
            Report("Filling in floors and walls...");
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    // If not a void tile
                    if (this.CMap[x, y] != Color.Transparent)
                    {
                        if(floor.Tiles[x,y]==null)
                            floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 138);
                        // If a wall tile
                        if (this.CMap[x, y] == Color.Black)
                        {
                            byte wallState = GetSurroundingWalls(x, y);
                            switch (wallState)
                            {
                                // Bottom straight wall
                                case 0b000_11_111:
                                case 0b100_11_111:
                                case 0b001_11_111:
                                case 0b101_11_111:
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 77);
                                    front.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 77);
                                    front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 214 + (x) % 2);
                                    break;
                                // Top straight wall
                                case 0b111_11_000:
                                case 0b111_11_100:
                                case 0b111_11_001:
                                case 0b111_11_101:
                                    if (this.Seeder.NextDouble() < 0.1)
                                    {
                                            if ((x + y) % 5 == 0)
                                            {
                                                front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 73 + (x % 3));
                                                front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 98);
                                                front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 114);
                                                wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 121 + (x % 3));
                                            }
                                            else
                                            {
                                                front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 7 + (x % 2));
                                                front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 23 + (x % 2));
                                                front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 39 + (x % 2));
                                                wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 55 + (x % 2));
                                            }
                                    }
                                    else if(this.Seeder.NextDouble()<0.3)
                                    {

                                        if (this.Seeder.NextDouble() < this.Difficulty/25)
                                        {
                                            front.Tiles[x, y - 3] = new AnimatedTile(wall, new StaticTile[]{
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 108 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 144 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 180 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 144 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 108 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 108 + (x % 9))
                                                }, 250); ;
                                            front.Tiles[x, y - 2] = new AnimatedTile(wall, new StaticTile[]{
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 117 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 153 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 189 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 153 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 117 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 117 + (x % 9))
                                                }, 250); ;
                                            front.Tiles[x, y - 1] = new AnimatedTile(wall, new StaticTile[]{
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 126 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 162 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 198 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 162 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 126 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 126 + (x % 9))
                                                }, 250); ;
                                            wall.Tiles[x, y] = new AnimatedTile(wall, new StaticTile[]{
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 135 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 171 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 207 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 171 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 135 + (x % 9)),
                                                    new StaticTile(wall, sWalls, BlendMode.Additive, 135 + (x % 9))
                                                }, 250); ;
                                        }
                                        else if (this.Seeder.NextDouble() < Math.Min(0.6, 0.05 + (this.Difficulty / 18)))
                                        {
                                            front.Tiles[x, y - 3] = new StaticTile(wall, sWalls, BlendMode.Additive, 72 + (x % 9));
                                            front.Tiles[x, y - 2] = new StaticTile(wall, sWalls, BlendMode.Additive, 81 + (x % 9));
                                            front.Tiles[x, y - 1] = new StaticTile(wall, sWalls, BlendMode.Additive, 90 + (x % 9));
                                            wall.Tiles[x, y] = new StaticTile(wall, sWalls, BlendMode.Additive, 99 + (x % 9));
                                        }
                                        else if (this.Seeder.NextDouble() < Math.Min(0.8, 0.1 + (this.Difficulty / 14)))
                                        {
                                            front.Tiles[x, y - 3] = new StaticTile(wall, sWalls, BlendMode.Additive, 36 + (x % 9));
                                            front.Tiles[x, y - 2] = new StaticTile(wall, sWalls, BlendMode.Additive, 45 + (x % 9));
                                            front.Tiles[x, y - 1] = new StaticTile(wall, sWalls, BlendMode.Additive, 54 + (x % 9));
                                            wall.Tiles[x, y] = new StaticTile(wall, sWalls, BlendMode.Additive, 63 + (x % 9));
                                        }
                                        else
                                        {
                                            front.Tiles[x, y - 3] = new StaticTile(wall, sWalls, BlendMode.Additive, 0 + (x % 9));
                                            front.Tiles[x, y - 2] = new StaticTile(wall, sWalls, BlendMode.Additive, 9 + (x % 9));
                                            front.Tiles[x, y - 1] = new StaticTile(wall, sWalls, BlendMode.Additive, 18 + (x % 9));
                                            wall.Tiles[x, y] = new StaticTile(wall, sWalls, BlendMode.Additive, 27 + (x % 9));
                                        }
                                    }
                                    else
                                    {
                                        front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 73 + (x % 3));
                                        front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 89 + (x % 3));
                                        front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 105 + (x % 3));
                                        wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 121 + (x % 3));
                                    }
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 218);
                                    floor.Tiles[x, y + 1] = new StaticTile(floor, sheet, BlendMode.Additive, 234);
                                    break;
                                // Top-left outer corner wall
                                case 0b111_11_110:
                                    front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 68);
                                    front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 84);
                                    wall.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 100);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 116);
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 218);
                                    floor.Tiles[x, y + 1] = new StaticTile(floor, sheet, BlendMode.Additive, 234);
                                    break;
                                // Top-right outer corner wall
                                case 0b111_11_011:
                                    front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 111);
                                    front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 127);
                                    wall.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 143);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 159);
                                    break;
                                // Right straight wall
                                case 0b011_01_011:
                                case 0b011_01_111:
                                case 0b111_01_111:
                                case 0b111_01_011:
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 175 + (16 * (y % 4)));
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 218);
                                    floor.Tiles[x - 1, y] = new StaticTile(floor, sheet, BlendMode.Additive, 217);
                                    if (wallState==0b111_01_011)
                                        floor.Tiles[x - 1, y] = new StaticTile(floor, sheet, BlendMode.Additive, 185);
                                    break;
                                // Left straight wall
                                case 0b110_10_110:
                                case 0b110_10_111:
                                case 0b111_10_110:
                                case 0b111_10_111:
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 132 + (16 * (y % 4)));
                                    break;
                                // Bottom-right inner corner wall
                                case 0b110_1_0_000:
                                case 0b110_1_0_001:
                                    front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 71);
                                    front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 87);
                                    front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 103);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 119);
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 219);
                                    floor.Tiles[x, y + 1] = new StaticTile(floor, sheet, BlendMode.Additive, 235);
                                    break;
                                // Bottom-left inner corner wall
                                case 0b011_0_1_000:
                                case 0b011_0_1_001:
                                    front.Tiles[x, y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 72);
                                    front.Tiles[x, y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 88);
                                    front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 104);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 120);
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 218);
                                    floor.Tiles[x - 1, y] = new StaticTile(floor, sheet, BlendMode.Additive, 217);
                                    floor.Tiles[x - 1, y + 1] = new StaticTile(floor, sheet, BlendMode.Additive, 233);
                                    floor.Tiles[x, y + 1] = new StaticTile(floor, sheet, BlendMode.Additive, 234);
                                    break;
                                // Top-left inner corner wall
                                case 0b000_0_1_011:
                                case 0b100_0_1_011:
                                    front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 220);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 236);
                                    floor.Tiles[x - 1, y] = new StaticTile(floor, sheet, BlendMode.Additive, 201);
                                    floor.Tiles[x, y] = new StaticTile(floor, sheet, BlendMode.Additive, 202);
                                    break;
                                // Top-right inner corner wall
                                case 0b000_1_0_110:
                                case 0b001_1_0_110:
                                    front.Tiles[x, y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 197);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 148);
                                    break;
                                // Top-left outer corner wall
                                case 0b110_1_1_111:
                                    front.Tiles[x, y-2] = new StaticTile(wall, sheet, BlendMode.Additive, 180);
                                    front.Tiles[x, y-1] = new StaticTile(wall, sheet, BlendMode.Additive, 196);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 77);
                                    break;
                                // Top-right outer corner wall
                                case 0b011_1_1_111:
                                    front.Tiles[x, y-2] = new StaticTile(wall, sheet, BlendMode.Additive, 223);
                                    front.Tiles[x, y-1] = new StaticTile(wall, sheet, BlendMode.Additive, 206);
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 77);
                                    break;
                                // Undefined wall type
                                default:
                                    wall.Tiles[x, y] = new StaticTile(wall, sheet, BlendMode.Additive, 275);
                                    break;
                            }
                        }
                    }
            Report("Spawning in structures....");
            // Try to spawn structures
            this.BuildStructure(ref map, "DirtPatch", 9, 9, 50, 2, 4,() => this.GetRandomDirt(ref floor, ref wall, ref sheet));
            this.BuildStructure(ref map, "WaterPatch", 9, 9, 50, 1,2, () => this.GetRandomWater(ref floor, ref wall, ref sheet));
            this.BuildStructure(ref map, "WoodPatch", 7, 7, 50, 1, 2, () => this.GetRandomWood(ref floor, ref wall, ref sheet));
            this.BuildStructure(ref map, "CobblePatch", 7, 7, 50, 1, 2, () => this.GetRandomCobble(ref floor, ref wall, ref sheet));
            this.BuildStructure(ref map, "CoalCart", 7, 7, 8, 2, 3, new ITile[] {
                new STile(2,0,front, sheet, 205),
                new STile(3,0,front,sheet,215),
                new STile(4,0,front,sheet,215),
                new STile(5,0,front,sheet,197),
                new STile(2,1,wall,sheet,72),
                new STile(3,1,wall,sheet,73),
                new STile(4,1,wall,sheet,74),
                new STile(5,1,wall,sheet,71),
                new STile(2,2,wall,sheet,88),
                new STile(3,2,wall,sheet,130),
                new STile(4,2,wall,sheet,131),
                new STile(5,2,wall,sheet,87),
                new STile(0,3,floor,sheet,193),
                new STile(1,3,floor,sheet,226),
                new STile(2,3,wall,sheet,104),
                new STile(3,3,wall,sheet,146),
                new STile(4,3,wall,sheet,147),
                new STile(5,3,wall,sheet,103),
                new STile(1,4,floor,sheet,228),
                new STile(2,4,wall,sheet,120),
                new STile(2,4,front,sheet,179),
                new STile(3,4,wall,sheet,162),
                new STile(3,4,front,sheet,208),
                new STile(4,4,wall,sheet,163),
                new STile(5,4,wall,sheet,119),
                new STile(1,5,floor,sheet,227),
                new STile(2,5,floor,sheet,210),
                new STile(2,5,wall,sheet,195),
                new STile(3,5,floor,sheet,210),
                new STile(3,5,wall,sheet,224),
                new STile(4,5,floor,sheet,210)
            });
            this.BuildStructure(ref map, "SmallHole", 4, 4, 10, 3, 5, new ITile[] {
                new STile(1,1,floor,sheet,77),
                new STile(1,2,floor,sheet,77),
                new STile(2,1,floor,sheet,77),
                new STile(2,2,floor,sheet,77),
                new STile(1,1,wall,sheet,243),
                new STile(2,1,wall,sheet,245),
                new STile(1,2,wall,sheet,278),
                new STile(2,2,wall,sheet,280)
            });
            /*
            for (int t = this.Seeder.Next(0, 2); t < 4; t++)
                if (FindRegionOfSize(ref map, 7, 2, 10, out var point5))
                {
                    Report("Structure spawned: HRail1");
                    wall.Tiles[point5.X, point5.Y] = new StaticTile(wall, rsheet, BlendMode.Additive, 3);
                    floor.Tiles[point5.X + 1, point5.Y] = new StaticTile(floor, rsheet, BlendMode.Additive, 2);
                    floor.Tiles[point5.X + 1, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 20);
                    floor.Tiles[point5.X + 2, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 1);
                    floor.Tiles[point5.X + 3, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 21);
                    floor.Tiles[point5.X + 4, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 1);
                    floor.Tiles[point5.X + 5, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 1);
                    wall.Tiles[point5.X + 6, point5.Y + 1] = new StaticTile(wall, rsheet, BlendMode.Additive, 5);
                }
                else
                    Report("Spawn attempt failed: HRail1");
                    */
            for (int t = this.Seeder.Next(0, 4); t < 8; t++)
                if (FindRegionOfSize(ref map, 3, 3, 10, out var point5))
                {
                    Report("Structure spawned: SmallStalagmite");
                    front.Tiles[point5.X + 1, point5.Y] = new StaticTile(front, s1sheet, BlendMode.Additive, 0);
                    wall.Tiles[point5.X + 1, point5.Y + 1] = new StaticTile(wall, s1sheet, BlendMode.Additive, 1);
                }
                else
                    Report("Spawn attempt failed: SmallStalagmite");
            for (int t = this.Seeder.Next(0, 3); t < 6; t++)
                if (FindRegionOfSize(ref map, 4, 3, 10, out var point5))
                {
                    Report("Structure spawned: LargeStalagmite");
                    afront.Tiles[point5.X + 1, point5.Y - 1] = new StaticTile(afront, s2sheet, BlendMode.Additive, 0);
                    afront.Tiles[point5.X + 2, point5.Y - 1] = new StaticTile(afront, s2sheet, BlendMode.Additive, 1);
                    front.Tiles[point5.X + 1, point5.Y] = new StaticTile(front, s2sheet, BlendMode.Additive, 2);
                    front.Tiles[point5.X + 2, point5.Y] = new StaticTile(front, s2sheet, BlendMode.Additive, 3);
                    wall.Tiles[point5.X + 1, point5.Y + 1] = new StaticTile(wall, s2sheet, BlendMode.Additive, 4);
                    wall.Tiles[point5.X + 2, point5.Y + 1] = new StaticTile(wall, s2sheet, BlendMode.Additive, 5);
                }
                else
                    Report("Spawn attempt failed: LargeStalagmite");
            /*
            for (int t = this.Seeder.Next(0, 2); t < 4; t++)
                if (FindRegionOfSize(ref map, 4, 5, 10, out var point5))
                {
                    Report("Structure spawned: StalagRail");
                    afront.Tiles[point5.X + 1, point5.Y - 1] = new StaticTile(afront, s2sheet, BlendMode.Additive, 0);
                    afront.Tiles[point5.X + 2, point5.Y - 1] = new StaticTile(afront, s2sheet, BlendMode.Additive, 1);
                    front.Tiles[point5.X + 1, point5.Y] = new StaticTile(front, s2sheet, BlendMode.Additive, 2);
                    front.Tiles[point5.X + 2, point5.Y] = new StaticTile(front, s2sheet, BlendMode.Additive, 3);
                    wall.Tiles[point5.X + 1, point5.Y + 1] = new StaticTile(wall, s2sheet, BlendMode.Additive, 4);
                    wall.Tiles[point5.X + 2, point5.Y + 1] = new StaticTile(wall, s2sheet, BlendMode.Additive, 5);
                    wall.Tiles[point5.X, point5.Y] = new StaticTile(wall, rsheet, BlendMode.Additive, 3);
                    floor.Tiles[point5.X + 1, point5.Y] = new StaticTile(floor, rsheet, BlendMode.Additive, 21);
                    floor.Tiles[point5.X + 2, point5.Y] = new StaticTile(floor, rsheet, BlendMode.Additive, 1);
                    floor.Tiles[point5.X + 3, point5.Y] = new StaticTile(floor, rsheet, BlendMode.Additive, 2);
                    floor.Tiles[point5.X + 3, point5.Y + 1] = new StaticTile(floor, rsheet, BlendMode.Additive, 10);
                    floor.Tiles[point5.X + 3, point5.Y + 2] = new StaticTile(floor, rsheet, BlendMode.Additive, 22);
                    floor.Tiles[point5.X + 2, point5.Y + 2] = new StaticTile(floor, rsheet, BlendMode.Additive, 0);
                    wall.Tiles[point5.X+2, point5.Y+3] = new StaticTile(wall, rsheet, BlendMode.Additive, 6);
                }
                else
                    Report("Spawn attempt failed: StalagRail");
                    */
            this.BuildStructure(ref map, "LootChest", 3, 3, 5, 1, 2, new ITile[]{
                        new STile(0, 0, floor, sheet, 240),
                        new STile(1, 0, floor, sheet, 241),
                        new STile(2, 0, floor, sheet, 242),
                        new STile(0, 1, floor, sheet, 256),
                        new STile(1, 1, floor, sheet, 257),
                        new STile(2, 1, floor, sheet, 258),
                        new STile(0, 2, floor, sheet, 272),
                        new STile(1, 2, floor, sheet, 273),
                        new STile(2, 2, floor, sheet, 274),
                        new STile(1, 1, wall, sheet, 237),
                        new PTile(1,1,wall,"Action","DDLoot General 3 true 0.3 Mimic")
                    });
            for (int t = this.Seeder.Next(0, 1); t < 2; t++)
                if (FindRegionOfSize(ref map, 4, 4, 5, out var point5))
                {
                    Report("Structure spawned: CavedSkeleton");
                    wall.Tiles[point5.X + 1, point5.Y + 1] = new StaticTile(wall, csheet, BlendMode.Additive, 0);
                    wall.Tiles[point5.X + 2, point5.Y + 1] = new StaticTile(wall, csheet, BlendMode.Additive, 1);
                    wall.Tiles[point5.X + 1, point5.Y + 2] = new StaticTile(wall, csheet, BlendMode.Additive, 2);
                    wall.Tiles[point5.X + 2, point5.Y + 2] = new StaticTile(wall, csheet, BlendMode.Additive, 3);
                    wall.Tiles[point5.X + 1, point5.Y + 2].Properties.Add("Action", "DDLoot Supplies 2 0.1 Skeleton");
                    wall.Tiles[point5.X + 2, point5.Y + 2].Properties.Add("Action", "DDLoot Supplies 2 0.1 Skeleton");
                }
                else
                    Report("Spawn attempt failed: CavedSkeleton");
            if (FindRegionOfSize(ref map, 5, 4, 3, out var point2))
            {
                Report("Structure spawned: GemRock");
                // Gem Rock
                int x = point2.X;
                int y = point2.Y;
                front.Tiles[x + 1, y] = new AnimatedTile(front, new StaticTile[] { new StaticTile(front, bsheet, BlendMode.Additive, 0), new StaticTile(front, bsheet, BlendMode.Additive, 3) }, 500);
                front.Tiles[x + 2, y] = new AnimatedTile(front, new StaticTile[] { new StaticTile(front, bsheet, BlendMode.Additive, 1), new StaticTile(front, bsheet, BlendMode.Additive, 4) }, 500);
                front.Tiles[x + 3, y] = new AnimatedTile(front, new StaticTile[] { new StaticTile(front, bsheet, BlendMode.Additive, 2), new StaticTile(front, bsheet, BlendMode.Additive, 5) }, 500);
                wall.Tiles[x + 1, y + 1] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 6), new StaticTile(wall, bsheet, BlendMode.Additive, 9) }, 500);
                wall.Tiles[x + 2, y + 1] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 7), new StaticTile(wall, bsheet, BlendMode.Additive, 10) }, 500);
                wall.Tiles[x + 3, y + 1] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 8), new StaticTile(wall, bsheet, BlendMode.Additive, 11) }, 500);
                wall.Tiles[x + 1, y + 2] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 12), new StaticTile(wall, bsheet, BlendMode.Additive, 15) }, 500);
                wall.Tiles[x + 2, y + 2] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 13), new StaticTile(wall, bsheet, BlendMode.Additive, 16) }, 500);
                wall.Tiles[x + 3, y + 2] = new AnimatedTile(wall, new StaticTile[] { new StaticTile(wall, bsheet, BlendMode.Additive, 14), new StaticTile(wall, bsheet, BlendMode.Additive, 17) }, 500);
                wall.Tiles[x + 1, y + 2].Properties.Add("Action", "DDLoot Gems 1");
                wall.Tiles[x + 2, y + 2].Properties.Add("Action", "DDLoot Gems 1");
                wall.Tiles[x + 3, y + 2].Properties.Add("Action", "DDLoot Gems 1");
            }
            else
                Report("Spawn attempt failed: GemRock");
            if (this.Floor % 3 == 0)
                this.BuildStructure(ref map, "DwardVendor", 4, 3, 50, 1, 1, new ITile[]
                {
                    new ATile(0, -1, afront, new STile[]{
                        new STile(0,0,afront,vsheet,0),
                        new STile(0,0,afront,vsheet,2),
                        new STile(0,0,afront,vsheet,10),
                        new STile(0,0,afront,vsheet,18),
                        new STile(0,0,afront,vsheet,4),
                        new STile(0,0,afront,vsheet,12),
                        new STile(0,0,afront,vsheet,20),
                        new STile(0,0,afront,vsheet,6),
                        new STile(0,0,afront,vsheet,14),
                        new STile(0,0,afront,vsheet,22),
                    },500),
                    new ATile(1, -1, afront, new STile[]{
                        new STile(0,0,afront,vsheet,1),
                        new STile(0,0,afront,vsheet,3),
                        new STile(0,0,afront,vsheet,11),
                        new STile(0,0,afront,vsheet,19),
                        new STile(0,0,afront,vsheet,5),
                        new STile(0,0,afront,vsheet,13),
                        new STile(0,0,afront,vsheet,21),
                        new STile(0,0,afront,vsheet,7),
                        new STile(0,0,afront,vsheet,15),
                        new STile(0,0,afront,vsheet,23),
                    },500),
                    new STile(0,0,front,vsheet,8),
                    new STile(1,0,front,vsheet,9),
                    new STile(0,1,wall,vsheet,16),
                    new STile(1,1,wall,vsheet,17),
                    new PTile(0,1,wall,"Action","DDShop DwarfVendor"),
                    new PTile(1,1,wall,"Action","DDShop DwarfVendor")
                });
            for (int c = 0; c < this.Seeder.Next(3, 7); c++)
                GenerateTrack(map, rsheet);
            // Randomise floor
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    if (floor.Tiles[x, y] != null && floor.Tiles[x, y].TileIndex == 138 && this.Seeder.NextDouble() < 0.05)
                        floor.Tiles[x, y].TileIndex = floorTiles[(x * y) % floorTiles.Length];
            Point entry = GetFloorPoint();
            front.Tiles[entry.X, entry.Y-4] = new StaticTile(wall, sheet, BlendMode.Additive, 67);
            front.Tiles[entry.X, entry.Y - 3] = new StaticTile(wall, sheet, BlendMode.Additive, 83);
            front.Tiles[entry.X, entry.Y - 2] = new StaticTile(wall, sheet, BlendMode.Additive, 99);
            wall.Tiles[entry.X, entry.Y - 1] = new StaticTile(wall, sheet, BlendMode.Additive, 115);
            floor.Tiles[entry.X, entry.Y] = new StaticTile(floor, sheet, BlendMode.Additive, 204);
            return map;
        }
        public Point GetFloorPoint()
        {
            for (int y = 0; y < this.Height; y++)
                for (int x = 0; x < this.Width; x++)
                    if (this.CMap[x, y] == Color.White)
                        return new Point(x, y);
            return default(Point);
        }
    }
}
