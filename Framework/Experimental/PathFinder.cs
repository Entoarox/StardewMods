using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Layers;

namespace Entoarox.Framework.Experimental
{
    internal static class PathFinderExtension
    {
        /*********
        ** Public methods
        *********/
        public static List<PathNode> FindPath(this GameLocation location, Point start, Point end)
        {
            List<PathNode> nodes = new List<PathNode>();
            PathNode finish = null;
            bool finished = false;

            void LookNext(PathNode node)
            {
                if (finished)
                    return;
                if (!nodes.Contains(node))
                {
                    if (!location.isTileLocationTotallyClearAndPlaceable(node.X, node.Y))
                        return;
                    nodes.Add(node);
                    if (node.X == start.X && node.Y == start.Y)
                    {
                        finished = true;
                        finish = node;
                    }
                }

                if (!finished)
                {
                    Task[] tasks =
                    {
                        Task.Run(() => { LookNext(new PathNode(node.X - 1, node.Y, node)); }),
                        Task.Run(() => { LookNext(new PathNode(node.X + 1, node.Y, node)); }),
                        Task.Run(() => { LookNext(new PathNode(node.X, node.Y - 1, node)); }),
                        Task.Run(() => { LookNext(new PathNode(node.X, node.Y + 1, node)); })
                    };
                    Task.WaitAll(tasks);
                }
            }

            LookNext(new PathNode(start.X, start.Y));
            List<PathNode> points = new List<PathNode>();
            if (finish == null)
                return null;
            points.Add(finish);
            while (finish.Parent != null)
            {
                finish = finish.Parent;
                points.Add(finish);
            }

            return points;
        }
    }
}
