using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Entoarox.Framework.Experimental
{
    static class PathFinderExtension
    {
        public class Node
        {
            public Node Parent;
            public int X;
            public int Y;

            public Node(int x, int y, Node parent = null)
            {
                this.Parent = parent;
                this.X = x;
                this.Y = y;
            }

            public override bool Equals(object obj)
            {
                return obj is Node node && node.X == this.X && node.Y == this.Y;
            }
            public override int GetHashCode()
            {
                return this.X << 8 | this.Y;
            }
        }
        public static List<Node> FindPath(this GameLocation location, Point start, Point end)
        {
            var layer = location.map.GetLayer("Back");
            List<Node> nodes = new List<Node>();
            Node finish=null;
            bool finished = false;
            void LookNext(Node node)
            {
                if (finished)
                    return;
                if(!nodes.Contains(node))
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
                if(!finished)
                {
                    Task[] tasks ={
                        Task.Run(() => {LookNext(new Node(node.X - 1, node.Y, node));}),
                        Task.Run(() => {LookNext(new Node(node.X + 1, node.Y, node));}),
                        Task.Run(() => {LookNext(new Node(node.X, node.Y - 1, node));}),
                        Task.Run(() => {LookNext(new Node(node.X, node.Y + 1, node));})
                    };
                    Task.WaitAll(tasks);
                }
            }
            LookNext(new Node(start.X, start.Y));
            List<Node> points = new List<Node>();
            if (finish == null)
                return null;
            points.Add(finish);
            while(finish.Parent!=null)
            {
                finish = finish.Parent;
                points.Add(finish);
            }
            return points;
        }
    }
}
