using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

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
        public DungeonBuilder()
        {
            this.Seed = _Random.Next();
            this.GenerateMap();
        }
        public DungeonBuilder(int seed)
        {
            this.Seed = seed;
            this.GenerateMap();
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
            // Setup the seeder that this map will be using
            this.Seeder = new Random(this.Seed);
            // Define the width & height
            this.Width = this.Seeder.Next(56, 64);
            this.Height = this.Seeder.Next(56, 64);
            // Setup the map
            this.Map = new bool[this.Width, this.Height];
            // Seed the initial automaton state
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    if(this.Seeder.NextDouble() < _ChanceToStartAlive)
                        this.Map[x, y] = true;
            // Simulate the automaton to get a rough map
            for (int s = 0; s < _SimulationSteps; s++)
                NextStep();
            // Clean up dangling single-tile walls
            for (int s = 0; s < _SimulationSteps*2; s++)
                NextClean();
            // Convert the true/false map into a color-capable one
            ColorConvert();
            // Perform flood filling to find & isolate a large dungeon region
            FloodFill();
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
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
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
                }
            this.Map = updated;
        }
        private void NextClean()
        {
            bool[,] updated = new bool[this.Width, this.Height];
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Map[x, y])
                    {
                        int xc = this.GetNeighborCountX(x, y);
                        int yc = this.GetNeighborCountY(x, y);
                        if (xc+yc < 3 && (xc == 0 || yc == 0))
                            updated[x, y] = false;
                        else
                            updated[x, y] = true;
                    }
                    else
                    {
                        updated[x, y] = false;
                    }
                }
            this.Map = updated;
        }
        private void FloodFill()
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
                        }
                        else
                        {
                            FloodAway(Color.Red);
                            FloodFill();
                        }
                        goto exitLoop;
                    }
                }
            this.Seed = this.Seeder.Next();
            this.GenerateMap();
            exitLoop:
            return;
        }
        private void FloodAway(Color color, Color? replacement=null)
        {
            Color outColor = replacement ?? Color.Black;
            Color[,] updated = new Color[this.Width, this.Height];
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    if (this.CMap[x, y] == color)
                        updated[x, y] = outColor;
                    else
                        updated[x, y] = this.CMap[x, y];
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
        public Texture2D GetMiniMap()
        {
            Color[] mapping = new Color[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
                for (int x = 0; x < this.Width; x++)
                     mapping[y * this.Width + x] = this.CMap[x,y];
            Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, this.Width, this.Height);
            texture.SetData(mapping);
            return texture;
        }
    }
}
