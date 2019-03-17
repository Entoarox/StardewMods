using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace DialogueFramework
{
    interface IModApi
    {
        event EventHandler<DialogueEventArgs> ChoiceDialogueOpened;
    }
}
