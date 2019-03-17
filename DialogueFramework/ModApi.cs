using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace DialogueFramework
{
    public class ModApi : IModApi
    {
        public event EventHandler<DialogueEventArgs> ChoiceDialogueOpened;

        internal void FireChoiceDialogueOpened(List<Response> responses)
        {
            ChoiceDialogueOpened?.Invoke(this, new DialogueEventArgs(responses));
        }
    }
}
