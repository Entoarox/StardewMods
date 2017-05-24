using Microsoft.Xna.Framework;

namespace Entoarox.Framework
{
    public interface IMessageHelper
    {
        void Add(string message, string name, Color color);
        void Add(string message, Color color);
    }
}
