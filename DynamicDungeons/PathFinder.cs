using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile;
using xTile.Layers;

namespace Entoarox.DynamicDungeons
{
    internal static class PathFinder
    {
        /*********
        ** Public methods
        *********/
        public static List<Point> FindPath(Map location, Point start, Point end)
        {
            Layer backLayer = location.GetLayer("Back");
            Layer buildingsLayer = location.GetLayer("Buildings");
            List<PathNode> nodes = new List<PathNode>();
            sbyte[,] array =
            {
                { -1, 0 },
                { 1, 0 },
                { 0, 1 },
                { 0, -1 }
            };

            bool IsOccupied(int x, int y)
            {
                return x < 0 || x >= buildingsLayer.LayerWidth || y < 0 || y > buildingsLayer.DisplayHeight || buildingsLayer.Tiles[x, y] != null || backLayer.Tiles[x, y]?.TileIndex != 138;
            }

            List<Point> ReconstructPath(PathNode node)
            {
                List<Point> myp = new List<Point>
                {
                    new Point(node.x, node.y)
                };
                while (node.parent != null)
                {
                    node = node.parent;
                    myp.Add(new Point(node.x, node.y));
                }

                return myp;
            }

            PriorityQueue queue = new PriorityQueue();
            int limit = buildingsLayer.LayerWidth * buildingsLayer.LayerHeight / 2;
            queue.Enqueue(new PathNode(start.X, start.Y, null), Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y));
            while (!queue.IsEmpty())
            {
                PathNode node = queue.Dequeue();
                if (node.x == end.X && node.y == end.Y)
                    return ReconstructPath(node);
                if (nodes.Contains(node))
                    continue;
                nodes.Add(node);
                for (int i = 0; i < 4; i++)
                {
                    PathNode node2 = new PathNode(node.x + array[i, 0], node.y + array[i, 1], node)
                    {
                        g = (byte)(node.g + 1)
                    };
                    int priority = node2.g + Math.Abs(end.X - node2.x) + Math.Abs(end.Y - node.y);
                    if (!IsOccupied(node2.x, node2.y) && !queue.Contains(node2, priority))
                        queue.Enqueue(node2, priority);
                }

                limit--;
                if (limit < 1)
                    return null;
            }

            return null;
        }
    }
}
