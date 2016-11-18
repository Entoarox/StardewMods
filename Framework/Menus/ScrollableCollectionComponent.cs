using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Menus
{
    public class ScrollableCollectionComponent : GenericCollectionComponent
    {
        protected int ScrollOffset=0;
        protected int InnerHeight;
        protected int BarOffset;
        public ScrollableCollectionComponent(Rectangle area, List<IMenuComponent> components = null):base(area,components)
        {
            
        }
        protected override void UpdateDrawOrder()
        {
            base.UpdateDrawOrder();
            int height = Area.Height;
            foreach(IMenuComponent c in DrawOrder)
            {
                if (!c.Visible)
                    return;
                Rectangle r = c.GetRegion();
                int b = r.Y + r.Height;
                if (b > height)
                    height = b;
            }
            if (height > Area.Height)
                BarOffset = zoom12;
            else
                BarOffset = 0;
            InnerHeight = (int)Math.Ceiling((height - Area.Height) / (double)zoom10);
        }
        public override void Scroll(int d, Point p, Point o, IComponentCollection c, FrameworkMenu m)
        {
            int change = d / 120;
            ScrollOffset = Math.Max(0, Math.Min(ScrollOffset - change, InnerHeight));
            base.Scroll(d, p, o, c, m);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            SpriteBatch Batch = new SpriteBatch(b.GraphicsDevice);
            RenderTarget2D Target = new RenderTarget2D(b.GraphicsDevice, Area.Width - BarOffset, Area.Height);
            b.GraphicsDevice.SetRenderTarget(Target);
            b.GraphicsDevice.Clear(Color.Transparent);

            Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            Point o2=new Point(0, -(ScrollOffset * zoom10));
            foreach (IMenuComponent el in DrawOrder)
                el.Draw(Batch, o2);
            Batch.End();

            b.GraphicsDevice.SetRenderTarget((Game1.game1 as StardewModdingAPI.Inheritance.SGame).ZoomLevelIsOne ? null : (Game1.game1 as StardewModdingAPI.Inheritance.SGame).Screen);

            b.Draw(Target, new Rectangle(Area.X + o.X, Area.Y + o.Y, Area.Width - BarOffset, Area.Height), Color.White);
        }
    }
}
