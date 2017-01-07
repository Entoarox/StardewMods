using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile;

namespace Entoarox.Framework.Abstraction.Interfaces
{
    public interface IGameLocation
    {
        bool HasTile(int x, int y, string layer);
        void RemoveTile(int x, int y, string layer);
        void SetTile(int x, int y, string layer, int index, string sheet = null);
        void SetTile(int x, int y, string layer, int[] indexes, long interval, string sheet = null);
        void SetTileProperty(int x, int y, string layer, string key, string value);
        string GetTileProperty(int x, int y, string layer, string key);
        void SetWarp(int x, int y, string destination, int destinationX, int destinationY, bool replace = false);
        void SetWarp(int x, int y, Warp warp, bool replace = false);
        Warp GetWarp(int x, int y);

        bool TryRemoveTile(int x, int y, string layer);
        bool TrySetTile(int x, int y, string layer, int index, string sheet = null);
        bool TrySetTile(int x, int y, string layer, int[] indexes, long interval, string sheet = null);
        bool TrySetTileProperty(int x, int y, string layer, string key, string value);
        bool TryGetTileProperty(int x, int y, string layer, string key, out string value);
        bool TrySetWarp(int x, int y, string destination, int destinationX, int destinationY, bool replace = false);
        bool TrySetWarp(int x, int y, Warp warp, bool replace = false);
        bool TryGetWarp(int x, int y, out Warp warp);

        WarpList Warps { get;}
        Map Map { get; }
    }
}
