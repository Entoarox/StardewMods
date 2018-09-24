using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Interface
{
    public interface IComponent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>What layer this component should be on, with higher layer numbers being closer to the screen.</summary>
        int Layer { get; set; }

        /// <summary>Components will only handle Update and Draw events when this value is true. If this component is a <see cref="IDynamicComponent" /> or <see cref="IInputComponent" />, those events are also disabled. A <see cref="IDynamicComponent" /> will still receive the FocusLost event if it gained focus while visible.</summary>
        bool Visible { get; set; }

        /// <summary>The boundary of the component in screen pixels, all values must be a multiple of <see cref="StardewValley.Game1.pixelZoom" />.</summary>
        Rectangle OuterBounds { get; set; }

        /// <summary>The name of the component, used by the IComponentCollection that holds it to allow direct access to the component.</summary>
        string Name { get; }

        /// <summary>A reference to the collection that holds the component, must be set during Attach and null-ed during Detach.</summary>
        IComponentContainer Owner { get; }

        /// <summary>If this component is attached right now, must be set to true during Attach and set to false during Detach.</summary>
        bool IsAttached { get; }


        /*********
        ** Methods
        *********/
        /// <summary>Triggers when this component is added to a collection. Components can only exist in one collection at a time.</summary>
        /// <param name="container">The collection this component is being added to.</param>
        void Attach(IComponentContainer container);

        /// <summary>Triggers when this component is removed from a collection. Components can only exist in one collection at a time.</summary>
        /// <param name="container">The collection this component is being removed from.</param>
        void Detach(IComponentContainer container);

        /// <summary>Triggers during every update tick that the menu this component is a part of is on the screen.</summary>
        /// <param name="time">The current <see cref="GameTime" /> instance.</param>
        void Update(GameTime time);

        /// <summary>Triggers for every draw tick that the menu this component is a part of is on the screen.</summary>
        /// <param name="offset">The offset that this component should treat as 0,0.</param>
        /// <param name="batch">The current <see cref="SpriteBatch" /> used for drawing.</param>
        void Draw(Point offset, SpriteBatch batch);
    }
}
