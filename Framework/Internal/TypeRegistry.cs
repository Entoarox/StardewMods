using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Entoarox.Framework
{
    internal class TypeRegistry : ITypeRegistry
    {
        internal static ITypeRegistry Singleton { get; } = new TypeRegistry();
        List<Type> ITypeRegistry.GetInjectedTypes()
        {
            return InjectedTypes;
        }
        void ITypeRegistry.RegisterType<T>()
        {
            if (_injected)
                EntoFramework.Logger.Log("RegisterType<T> called too late, must be called prior to Game1.Initialize or it will likely not be added to the serialiser",StardewModdingAPI.LogLevel.Error);
            if (!typeof(T).IsPublic)
            {
                EntoFramework.Logger.Log("Types added to RegisterType<T> must be set as public",StardewModdingAPI.LogLevel.Error);
            }
            else
            {
                Type type = typeof(T);
                if (!InjectedTypes.Contains(type))
                    InjectedTypes.Add(type);
            }
        }
        internal static readonly List<Type> InjectedTypes = new List<Type>();
        internal static Type[] _serialiserTypes = new Type[27]
        {
            typeof (Tool), typeof (GameLocation), typeof (Crow), typeof (Duggy), typeof (Bug), typeof (BigSlime),
            typeof (Fireball), typeof (Ghost), typeof (Child), typeof (Pet), typeof (Dog),
            typeof (StardewValley.Characters.Cat),
            typeof (Horse), typeof (GreenSlime), typeof (LavaCrab), typeof (RockCrab), typeof (ShadowGuy),
            typeof (SkeletonMage),
            typeof (SquidKid), typeof (Grub), typeof (Fly), typeof (DustSpirit), typeof (Quest), typeof (MetalHead),
            typeof (ShadowGirl),
            typeof (Monster), typeof (TerrainFeature)
        };

        internal static Type[] _farmerTypes = new Type[1] {
            typeof (Tool)
        };

        internal static Type[] _locationTypes = new Type[26]
        {
            typeof (Tool), typeof (Crow), typeof (Duggy), typeof (Fireball), typeof (Ghost),
            typeof (GreenSlime), typeof (LavaCrab), typeof (RockCrab), typeof (ShadowGuy), typeof (SkeletonWarrior),
            typeof (Child), typeof (Pet), typeof (Dog), typeof (StardewValley.Characters.Cat), typeof (Horse),
            typeof (SquidKid),
            typeof (Grub), typeof (Fly), typeof (DustSpirit), typeof (Bug), typeof (BigSlime),
            typeof (BreakableContainer),
            typeof (MetalHead), typeof (ShadowGirl), typeof (Monster), typeof (TerrainFeature)
        };

        internal static XmlSerializer injectedSerializer;
        internal static XmlSerializer injectedFarmerSerializer;
        internal static XmlSerializer injectedLocationSerializer;

        private static MethodInfo FarmHandRegistry;

        private static bool _injected = false;

        internal static void Update(object s, EventArgs e)
        {
            if (_injected)
            {
                SaveGame.serializer = injectedSerializer;
                SaveGame.farmerSerializer = injectedFarmerSerializer;
                SaveGame.locationSerializer = injectedLocationSerializer;
            }
        }
        internal static void Init()
        {
            try
            {
                if (_injected)
                    return;
                var typeArray = InjectedTypes.ToArray();
                injectedSerializer = new XmlSerializer(typeof(SaveGame), _serialiserTypes.Concat(InjectedTypes).ToArray());
                injectedFarmerSerializer = new XmlSerializer(typeof(StardewValley.Farmer), _farmerTypes.Concat(InjectedTypes).ToArray());
                injectedLocationSerializer = new XmlSerializer(typeof(GameLocation), _locationTypes.Concat(InjectedTypes).ToArray());
                _injected = true;
            }
            catch(Exception err)
            {
                EntoFramework.Logger.ExitGameImmediately("A unexpected error occured trying to inject the custom serializer", err);
            }
        }
    }
}
