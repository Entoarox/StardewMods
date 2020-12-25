using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IInteractiveComponent : IComponent
    {
        bool Enabled { get; set; }
        Rectangle FocusRegion { get; }

        IInteractiveComponent NextUp { get; set; }
        IInteractiveComponent NextDown { get; set; }
        IInteractiveComponent NextLeft { get; set; }
        IInteractiveComponent NextRight { get; set; }

        int Scroll(int scrollAmount);

        void MouseIn(Point position);
        void MouseMove(Point position);
        void MouseOut(Point position);

        void LeftDown(Point position);
        void LeftHeld(Point position);
        void LeftUp(Point position);

        void RightDown(Point position);
        void RightHeld(Point position);
        void RightUp(Point position);

        void KeyDown(Keys key);
        void KeyHeld(Keys key);
        void KeyUp(Keys key);

        void FocusGained();
        void FocusLost();

        void Draw(Rectangle screenRect, Rectangle drawRect, SpriteBatch batch);
    }
}
