using System.Windows.Forms;
using StardewModdingAPI;

namespace Entoarox.DavrialPersonalConfirmClose
{
    public class DavrialPersonalConfirmCloseMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            ((Form)Control.FromHandle(StardewValley.Game1.game1.Window.Handle)).FormClosing += a;
        }
        private void a(object s, FormClosingEventArgs e)
        {
            e.Cancel = e.CloseReason == CloseReason.UserClosing && MessageBox.Show("Do you really want to exit?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No;
        }
    }
}