using System;
using System.Collections.Generic;
using Entoarox.Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SFarmer = StardewValley.Farmer;

namespace Entoarox.Framework.Indev
{
    [Obsolete("Feature still in development!", true)]
    internal class VanillaSkill : ISkill
    {
        /*********
        ** Fields
        *********/
        private static readonly Color[] SkillColors = new Color[5];
        private static readonly Dictionary<int, IProfession[]> _Professions = new Dictionary<int, IProfession[]>
        {
            {
                0, new IProfession[]
                {
                    new VanillaProfession("Rancher", null, 5),
                    new VanillaProfession("Tiller", null, 5),
                    new VanillaProfession("Coopmaster", "Rancher", 10),
                    new VanillaProfession("Shepherd", "Rancher", 10),
                    new VanillaProfession("Artisan", "Tiller", 10),
                    new VanillaProfession("Agriculturist", "Tiller", 10)
                }
            },
            {
                1, new IProfession[]
                {
                    new VanillaProfession("Miner", null, 5),
                    new VanillaProfession("Geologist", null, 5),
                    new VanillaProfession("Blacksmith", "Miner", 10),
                    new VanillaProfession("Prospector", "Miner", 10),
                    new VanillaProfession("Excavator", "Geologist", 10),
                    new VanillaProfession("Gemologist", "Geologist", 10)
                }
            },
            {
                2, new IProfession[]
                {
                    new VanillaProfession("Forester", null, 5),
                    new VanillaProfession("Gatherer", null, 5),
                    new VanillaProfession("Lumberjack", "Forester", 10),
                    new VanillaProfession("Tapper", "Forester", 10),
                    new VanillaProfession("Botanist", "Gatherer", 10),
                    new VanillaProfession("Tracker", "Gatherer", 10)
                }
            },
            {
                3, new IProfession[]
                {
                    new VanillaProfession("Fisher", null, 5),
                    new VanillaProfession("Trapper", null, 5),
                    new VanillaProfession("Angler", "Fisher", 10),
                    new VanillaProfession("Pirate", "Fisher", 10),
                    new VanillaProfession("Mariner", "Trapper", 10),
                    new VanillaProfession("Luremaster", "Trapper", 10)
                }
            },
            {
                4, new IProfession[]
                {
                    new VanillaProfession("Fighter", null, 5),
                    new VanillaProfession("Scout", null, 5),
                    new VanillaProfession("Brute", "Fighter", 10),
                    new VanillaProfession("Defender", "Fighter", 10),
                    new VanillaProfession("Acrobat", "Scout", 10),
                    new VanillaProfession("Desperado", "Scout", 10)
                }
            }
        };
        private readonly int VSkill;


        /*********
        ** Accessors
        *********/
        public int MaxSkillLevel => 10;
        public string DisplayName => SFarmer.getSkillDisplayNameFromIndex(this.VSkill);
        public Texture2D Icon => throw new NotImplementedException();
        public IProfession[] Professions => VanillaSkill._Professions[this.VSkill];
        public bool CanPrestige => true;
        public int MaxPrestigeLevel => 40;
        public Color Color => VanillaSkill.SkillColors[this.VSkill];
        public Action<int, IComponentCollection> Callback => this._Callback;
        public float ExperienceMultiplier => 1;


        /*********
        ** Public methods
        *********/
        public VanillaSkill(int vskill)
        {
            this.VSkill = vskill;
        }


        /*********
        ** Protected methods
        *********/
        private void _Callback(int level, IComponentCollection collection)
        {
            switch (this.VSkill)
            {
                case 0:
                    switch (level)
                    {
                        case 1:
                            collection.AddComponent(new RecipeComponent(0)); // Scarecrow
                            collection.AddComponent(new RecipeComponent(0, false)); // Basic Fertilizer
                            break;

                        case 2:
                            collection.AddComponent(new RecipeComponent(0)); // Mayonnaise Machine
                            collection.AddComponent(new RecipeComponent(0)); // Stone Fence
                            collection.AddComponent(new RecipeComponent(0)); // Sprinkler
                            break;
                    }
                    break;
            }
        }
    }
}
