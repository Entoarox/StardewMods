using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public interface IDynamicComponent : IComponent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Dynamic components will only receive dynamic events while this value is true. If this component is also a <see cref="IInputComponent" /> then input events are also disabled. A component will still receive the FocusLost event if it gained focus while enabled.</summary>
        bool Enabled { get; set; }

        string Tooltip { get; }


        /*********
        ** Methods
        *********/
        /// <summary>If a given position falls within the boundary of the component.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position being checked.</param>
        bool InBounds(Point offset, Point position);

        /// <summary>Triggers when the left mouse button is pressed.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void LeftClick(Point offset, Point position);

        /// <summary>Triggers when the left mouse button is kept pressed for a certain duration of time after the last Click or Press was triggered.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void LeftHeld(Point offset, Point position);

        /// <summary>Triggers when the left mouse button is released.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void LeftUp(Point offset, Point position);

        /// <summary>Triggers when the right mouse button is pressed.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void RightClick(Point offset, Point position);

        /// <summary>Triggers when the scroll wheel is scrolled by any amount.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        /// <param name="amount">How much force the wheel was scrolled with, translated into zoom pixels.</param>
        bool Scroll(Point offset, Point position, int amount);

        /// <summary>Triggers when the mouse enters the bounds of this component.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void HoverIn(Point offset, Point position);

        /// <summary>Triggers when the mouse leaves the bounds of this component.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void HoverOut(Point offset, Point position);

        /// <summary>Triggers for every Update while the mouse is inside the bounds of this component.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="position">The on-screen position of the mouse.</param>
        void HoverOver(Point offset, Point position);

        /// <summary>Triggers when this component gains focus.</summary>
        void FocusGained();

        /// <summary>Triggers when this component loses focus.</summary>
        void FocusLost();
    }
}
