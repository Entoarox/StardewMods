using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

using FieldAttributes = Mono.Cecil.FieldAttributes;

namespace LanguagePatcher
{
    class Program
    {
        const string PatcherName = "LanguagePatcher";
        const string PatcherVersion = "1.0.0";
        static readonly Dictionary<string, string> Languages = new Dictionary<string, string>()
        {
            ["nl"] = "nl-NL",
            ["ko"] = "ko-KR"
        };
        static void Main(string[] args)
        {
            try
            {
                Console.WindowWidth = 100;
                Console.WriteLine("This program will patch your Stardew Valley to provide increased language support.");
                Console.WriteLine("A copy of the unmodified stardew valley exe will be created in the process so that no damage can occur.\n");
                Console.WriteLine("Press Y to continue, press any other key to exit.");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                    return;
                Console.WriteLine("\nAttempting to patch stardew valley...");
                bool failed = false;
                string file;
                if (File.Exists("Stardew Valley.exe"))
                    file = "Stardew Valley";
                else if (File.Exists("StardewValley.exe"))
                    file = "StardewValley";
                else
                {
                    ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Unable to patch, stardew exe could not be found." + Environment.NewLine + "Press any key to exit.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
                string filePath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), file + ".exe");
                string copyPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), file + "_Before_" + PatcherName + PatcherVersion + ".exe");
                /**
                using (var assembly = AssemblyDefinition.ReadAssembly(filePath))
                {
                    if (assembly.CustomAttributes.Any(a => a.AttributeType.Name.Equals("AssemblyMetadataAttribute") && a.ConstructorArguments[0].Value.Equals(PatcherName) && a.ConstructorArguments[1].Value.Equals(PatcherVersion)))
                    {
                        ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Unable to patch, stardew exe already has the patch installed." + Environment.NewLine + "Press any key to exit.", ConsoleColor.DarkYellow);
                        Console.ReadKey();
                        return;
                    }
                }
                /**/
                if (File.Exists(copyPath))
                    File.Delete(filePath);
                else
                    File.Move(filePath, copyPath);
                using (var assembly = AssemblyDefinition.ReadAssembly(copyPath))
                {
                    try
                    {
                        // Add the custom attribute to the assembly so that other things can check for it
#pragma warning disable CS0618 // Type or member is obsolete
                        var attribute = new CustomAttribute(assembly.MainModule.Import(typeof(AssemblyMetadataAttribute).GetConstructor(new Type[] { typeof(string), typeof(string) })));
                        attribute.ConstructorArguments.Add(new CustomAttributeArgument(assembly.MainModule.Import(typeof(string)), PatcherName));
                        attribute.ConstructorArguments.Add(new CustomAttributeArgument(assembly.MainModule.Import(typeof(string)), PatcherVersion));
#pragma warning restore CS0618 // Type or member is obsolete
                        assembly.CustomAttributes.Add(attribute);

                        // Add extra languages to the list of allowed languages
                        TypeDefinition typeDef = assembly.MainModule.GetType("StardewValley.LocalizedContentManager").NestedTypes.First(a => a.IsEnum);
                        MethodDefinition methodDef = assembly.MainModule.GetType("StardewValley.LocalizedContentManager").Methods.First(a => a.Name.Equals("LanguageCodeString"));
                        Instruction exit = methodDef.Body.Instructions.First(a => a.OpCode == OpCodes.Ldloc_0);
                        Instruction prev = exit.Previous;
                        List<Instruction> languageInsts = new List<Instruction>();
                        string[] langs = Languages.Keys.ToArray();
                        foreach(string lang in langs)
                        {
                            if (typeDef.Fields.Any(a => a.Name.Equals(lang)))
                            {
                                ConsoleHelper.WriteLine("Skipping already available language: " + lang);
                                Languages.Remove(lang);
                            }
                        }
                        langs = Languages.Keys.ToArray();
                        Instruction[] targets = new Instruction[langs.Length];
                        for (int c=0;c<langs.Length;c++)
                        {
                            ConsoleHelper.WriteLine("Patching in language: "+langs[c]);
                            FieldDefinition fieldDef = new FieldDefinition(langs[c], FieldAttributes.Literal | FieldAttributes.Static | FieldAttributes.Public | FieldAttributes.HasDefault, typeDef)
                            {
                                Constant = c + 100
                            };
                            typeDef.Fields.Add(fieldDef);

                            targets[c] = Instruction.Create(OpCodes.Ldstr, Languages[langs[c]]);
                            languageInsts.AddRange(new[] {
                                targets[c],
                                Instruction.Create(OpCodes.Stloc_0),
                                Instruction.Create(OpCodes.Br_S,exit)
                            });
                        }
                        // Modify the language-string function
                        ILProcessor ilp = methodDef.Body.GetILProcessor();
                        Instruction[] insts =
                        {
                            Instruction.Create(OpCodes.Br_S,exit),
                            Instruction.Create(OpCodes.Ldarg_1),
                            Instruction.Create(OpCodes.Ldc_I4,100),
                            Instruction.Create(OpCodes.Sub),
                            Instruction.Create(OpCodes.Switch, targets),
                            Instruction.Create(OpCodes.Br_S,exit),
                        };
                        ilp.Replace(ilp.Body.Instructions.First(a => a.OpCode == OpCodes.Br_S), Instruction.Create(OpCodes.Br_S, insts[1]));
                        foreach(Instruction inst in insts)
                        {
                            ilp.InsertAfter(prev, inst);
                            prev = inst;
                        }
                        foreach (Instruction inst in languageInsts)
                        {
                            ilp.InsertAfter(prev, inst);
                            prev = inst;
                        }

                        assembly.Write(filePath);
                    }
                    catch (Exception e)
                    {
                        failed = true;
                        ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Error encountered during the patching process.\n" + e.ToString(), ConsoleColor.Red);
                    }
                }
                // Check if editing the assembly went as designed (Just in case something unexpected got messed up)
                if (!failed)
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(filePath))
                    {
                        if (!assembly.CustomAttributes.Any(a => a.AttributeType.Name.Equals("AssemblyMetadataAttribute") && a.ConstructorArguments[0].Value.Equals(PatcherName) && a.ConstructorArguments[1].Value.Equals(PatcherVersion)))
                            failed = true;
                    }
                }
                if (failed)
                {
                    // If we failed, we restore the vanilla exe
                    File.Delete(filePath);
                    File.Move(copyPath, filePath);
                    ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Patching failed, vanilla exe has been restored" + Environment.NewLine + "Press any key to exit.", ConsoleColor.Red);
                }
                else
                    ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Patching complete, stardew exe has been modified." + Environment.NewLine + "Press any key to exit.", ConsoleColor.Green);
                Console.ReadKey();
            }
            catch(Exception e)
            {
                ConsoleHelper.WriteLine("\n" + Environment.NewLine + "Error encountered outside the patching process.\n" + e.ToString(), ConsoleColor.Red);
                Console.ReadKey();
            }
        }
    }
    class ConsoleHelper
    {
        public static void Write(string value, ConsoleColor? foreground=null, ConsoleColor? background=null)
        {
            var fgm = Console.ForegroundColor;
            var bgm = Console.BackgroundColor;
            Console.ForegroundColor = foreground ?? Console.ForegroundColor;
            Console.BackgroundColor = background ?? Console.BackgroundColor;
            Console.Write(value);
            Console.ForegroundColor = fgm;
            Console.BackgroundColor = bgm;
        }
        public static void WriteLine(string value, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            var fgm = Console.ForegroundColor;
            var bgm = Console.BackgroundColor;
            Console.ForegroundColor = foreground ?? Console.ForegroundColor;
            Console.BackgroundColor = background ?? Console.BackgroundColor;
            Console.WriteLine(value);
            Console.ForegroundColor = fgm;
            Console.BackgroundColor = bgm;
        }
    }
}
