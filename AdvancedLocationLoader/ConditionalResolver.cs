using System.Collections.Generic;
using System.Linq;
using Entoarox.AdvancedLocationLoader.Configs;
using StardewModdingAPI;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal class ConditionalResolver
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<string, ConditionalResolver> Cache = new Dictionary<string, ConditionalResolver>();

        private readonly Conditional Conditional;


        /*********
        ** Public methods
        *********/
        public static ConditionalResolver Request(string list)
        {
            if (!ConditionalResolver.Cache.ContainsKey(list))
                ConditionalResolver.Cache.Add(list, new ConditionalResolver(list));
            return ConditionalResolver.Cache[list];
        }

        public void Init()
        {
            Response[] answers = new Response[2];
            answers[0] = new Response("y", ModEntry.Strings.Get("yesCost", new { amount = this.Conditional.Amount, itemName = this.GetItemName() }));
            answers[1] = new Response("n", ModEntry.Strings.Get("no"));
            Game1.currentLocation.lastQuestionKey = "CompleteConditionalQuestion";
            Game1.currentLocation.createQuestionDialogue(this.Conditional.Question, answers, this.Resolver, null);
        }

        public void Resolver(Farmer who, string answer)
        {
            if (answer == "n")
                return;
            if ((this.Conditional.Item == -1 && who.Money >= this.Conditional.Amount) || who.hasItemInInventory(this.Conditional.Item, this.Conditional.Amount))
            {
                if (this.Conditional.Item == -1)
                    who.Money -= this.Conditional.Amount;
                else
                    who.removeItemsFromInventory(this.Conditional.Item, this.Conditional.Amount);
                who.mailReceived.Add("ALLCondition_" + this.Conditional.Name);
                ModEntry.Logger.Log("Conditional completed: " + this.Conditional.Name, LogLevel.Trace);
                ModEntry.UpdateConditionalEdits();
                if (this.Conditional.Success != null)
                    Game1.drawDialogueBox(this.Conditional.Success);
            }
            else
                Game1.drawObjectDialogue(ModEntry.Strings.Get("notEnough", new { itemName = this.GetItemName() }));
        }


        /*********
        ** Private methods
        *********/
        private ConditionalResolver(string name)
        {
            this.Conditional = ModEntry.PatchData.Conditionals.FirstOrDefault(e => e.Name == name);
        }

        private string GetItemName()
        {
            return this.Conditional.Item == -1
                ? ModEntry.Strings.Get("gold")
                : new Object(this.Conditional.Item, 1).Name;
        }
    }
}
