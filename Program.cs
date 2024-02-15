using SoulsFormats;

namespace HarmoniousRenewal
{
    class Program {
        static readonly string Root = Directory.GetCurrentDirectory() + "\\";
        
        static string Player = "";
        static string Enemy = "";

        static Dictionary<string, PARAM> Params = [];
        
        static string GamePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\ELDEN RING\\Game\\eldenring.exe";

        static EMEVD Common = new();

        static FMG WeaponNameFMG = new();
        static FMG WeaponEffectFMG = new();
        static readonly Dictionary<int, FMG.Entry> WeaponName = [];
        static readonly Dictionary<int, FMG.Entry> WeaponEffect = [];

        static readonly Dictionary<string, string[]> Cache = new() {
            { "Stats", [ "Strength", "Agility", "Magic", "Faith", "Luck" ] },
            { "Damages", [ "attackBasePhysics", "attackBaseMagic", "attackBaseFire", "attackBaseThunder", "attackBaseDark" ] },
            { "Effects", [ "spEffectBehaviorId", "residentSpEffectId", "spEffectMsgId" ] },
            { "Statuses", [ "poizonAttackPower", "diseaseAttackPower", "bloodAttackPower", "curseAttackPower", "freezeAttackPower", "sleepAttackPower", "madnessAttackPower", ] },
            { "Extras", [
                "spAttribute", "reinforceTypeId",
                "correctType_Physics", "correctType_Magic", "correctType_Fire", "correctType_Thunder", "correctType_Dark", "correctType_Poison", "correctType_Blood", "correctType_Sleep", "correctType_Madness",
                "attackElementCorrectId"] }
        };


        /// <summary>
        /// Random Item Generator
        /// Have a dictionary of effects, based on tiers and rarity
        /// Have it generated for each weapon, changing the stats of the effect based on the weapons stats e.g. heavy weapon lifesteal more
        /// Then create a talisman for each weapon for each effect, giving a random one just like the previous bell bearing generator thing
        /// -Don't need to care about duplicates this way, because only one talisman can be equipped of the same type, and can just sell or trade the other talismans
        /// e.g. "With a gravediggers staff equipped, consume health instead of mana",
        /// "With a giantslayer equipped regenerate health on hit (Scaling with giantslayer hit speed)",
        /// "With a glintstone crown equipped, gain 4 agility, 3 strength, slowly regenerate mana, and gain health after casting a spell (Legendary)"
        /// -+- So, basically we'd have a big dictionary of possible effects and the items/talismans would randomly gain these effects, with the stats of the effect also being random depending on the rarity of the talisman
        /// This would also mean that a "seed" of some sort would need to be greated for each roll due to the random nature of the effects and talismans.
        /// Give a warning to the people that they should only reroll the seed if they are going to use new characters, otherwise they must keep the old seed.
        /// 
        /// Talismans get 5 resident speffects, whcich means that instead of randomizing the effects themselves, we could randomize the effects a talisman gets.
        /// With that being the case, it would mean that for each new item with random effects, it would only take a single row in equipparamaccessory and one-two rows of text for the description and name
        /// </summary>
        static void RogueLikeEquipment(BND4 paramBnd) {
            //Instruction 1000[101] Instruction 1014[00]
            EMEVD.Event Testeroniaest = CreateNewEvent(10606000, [
                new(2000, 2, (List<object>)[(byte)0]),
                new(3, 16, [(sbyte)1, (byte)3, (int)999999, (byte)1]),
                new(1000, 101, [(byte)0, (byte)0, (sbyte)1]),
                new(2003, 17, [(uint)10606000, (uint)10606000+95000, (sbyte)1])], 0);

            // Does not get much more efficient than this. Looped through and created 195000 accessory param rows in under 5 seconds. This is what to use when generating all the crazy amount of items for the above.
            // Also created a 190000 line event in 0.6 seconds
            // Created and wrote the event and accessories in 0.787
            PARAM parerm = Params["EquipParamAccessory"];
            List<PARAM.Row> rowss = parerm.Rows;
            PARAMDEF deff = parerm.AppliedParamdef;
            
            PARAM parama = Params["SpEffectParam"];
            List<PARAM.Row> rawoana = [];
            
            double timermemr = GetTime();
            for (int i = 0; i < 65000; i++)
            {
                int row = 9999999 - i;
                Random a = new();
                string nameee = i + " -- " + a.NextDouble()*5;
                rowss.Add(new(row, "", deff));
                rowss.Last().Name = nameee;
                
                int ID = 0;
                while (true)
                    if (parama[row] != null)
                        ID--;
                    else break;
                rawoana.Add(new(parama[0]) {ID = ID, Name = nameee});
                Console.WriteLine(nameee);

                Testeroniaest.Instructions.AddRange([
                    new(1003, 1, [(byte)1, (byte)0, (byte)0, (uint)(10606000+i)]),
                    new(2003, 4, [(int)row]) ]);
            }

            parama.Rows.AddRange(rawoana);

            Console.WriteLine(GetTime(timermemr) + " seconds");

            Testeroniaest.Instructions.Add(new(1014, 0));
            
            Common.Write("C:/Users/fear8/Downloads/test/common.emevd.dcx");
            
            // Save each param, then the parambnd
            foreach (BinderFile file in paramBnd.Files) {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (Params.TryGetValue(name, out PARAM? value))
                    file.Bytes = value.Write();
            }
            paramBnd.Write("C:/Users/fear8/Downloads/test/regulation.bin");
            SFUtil.EncryptERRegulation("C:/Users/fear8/Downloads/test/regulation.bin", BND4.Read("C:/Users/fear8/Downloads/test/regulation.bin"));

            Environment.Exit(0);
        }

        // Seems that weapons which inflict status like bloodloss, do not show how much they inflict if the effect itself is not in the weapon behavior

        // May.. Be able to replace it with a bleed calculation of my own, such as replacing <?bleedatkpower?> with something else.
        // Or just leave effects that inflict blood and such in
        // - Best way to fix it may to be that if a behavior effect is found to have blood or frost or etc scaling, then whatever of that is need is then moved over the the replacement effect

        // probably enemy level scaling as well
        // probably increase enemy health, maybe sight and chase distance
        // Maybe remove nose dist, reduce hearing power, which would make stealth much consistent
        // Maybe give the enemies that have good hearing, crazy good hearing but barely any eyesight, same for eyesight no hear enemy

        // need to make two hand buff mod

        // Maybe introduce randomized loot for items that have no use, such as bad talismans or armor

        // add some WetSpEffectId00 in weatherparam to make fog and snow and stuff reduce visibility and such
        // Also change up the weatherlotparam to make weather not change so quickly


        // Seems that the ash of war generator generates 0 unique ashes of war on a vanilla regulation
        // Also is a little strange since so many of them don't work with other weapons

        // !!!! -- Maybe only give the ash of war, when the weapon is equipped, so that they only see that ash of war for the weapon, and can change the affinity


        // !!!! -- to completely remove the need for backups and stuff, ask for the directory of the mod engine, then just add the folder wherever the files are written to the top of config_eldenring.toml



        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // WEAPONS SPEFFECT OFFSET FOR +1 +2 ETC ENHANCE INCREMENTS THE SPEFFECT AS WELL, REMOVING THE NEEDED SPEFFECTS
        // There's a chance that status effects don't scale with arcane, because they are being applied through hks
        // How to fix?
        // Maybe have a dictionary that stores each status effect buildup, and then an hks function that applies a status effect
        // Maybe if there's a chance that the stateinfo is not need, have the effect be on the weapon effect, then cycle all the needed effects on the enemy for every status independently
        
        // No fix, need to find a way to make the effects fit into the behavior ids
        // If a weapon has innate bleed, then an affinity adds bleed, turn the innate bleed into a bonus in arcane

        static void Main() {//dotnet publish -c Release -r win-x64
            try {
                // Introduction
                introduction:
                Console.WriteLine("Welcome to Harmonious Renewal!\n\nTo get started, point me towards the mod folder you would like to install to by putting the path below.\nIf the path does not exist, this text will repeat and another chance will be given.\n(Example: \"C:\\Downloads\\ModEngine-2.1.0.0-win64.zip\\ModEngine-2.1.0.0-win64\\mod\" or \"C:\\Downloads\\Elden Ring Unalloyed\\mod\")\n(If you have nothing like that, install and use ModEngine for elden ring)\n");
                string? ModFolder = Console.ReadLine();
                if (ModFolder == null) goto introduction;
                else ModFolder = ModFolder.Replace("\"", "").Trim();
                ModFolder = ModFolder.EndsWith('\\') ? ModFolder.Remove(ModFolder.Length - 1) : ModFolder;
                if (Directory.Exists(ModFolder))
                    if (File.Exists(ModFolder.Remove(ModFolder.Length - 3) + "config_eldenring.toml"))
                        Console.WriteLine("\nGreat! Let's move forward.");
                    else {
                        Console.WriteLine("\nIt seems that ModEngine is not installed. Please install ModEngine.\n");
                        Thread.Sleep(500);
                        Console.ReadKey();
                        goto introduction;
                    }
                else goto introduction;
                string ModEngine = ModFolder.Remove(ModFolder.Length - 3) + "config_eldenring.toml";
                //DateTime.Now.ToString("dddd.MMMM h.mm tt")
                string ModDate = "mod\\" + ModFolder.Replace("\\", ".").Replace(":", "") + "\\";
                // Get oo2core_6_win64.dll
                if (!File.Exists(Root + "oo2core_6_win64.dll")) {
                    GetGamePath();
                    File.Copy(GamePath + "oo2core_6_win64.dll", Root + "oo2core_6_win64.dll");
                }
                // Get HKS
                if (File.Exists(ModFolder + "\\action/script/c0000.hks"))
                    Player = File.ReadAllText(ModFolder + "\\action/script/c0000.hks");
                else
                    Player = File.ReadAllText(Root + "mod/action/script/c0000.hks");
                if (File.Exists(ModFolder + "\\action/script/c9997.hks"))
                    Enemy = File.ReadAllText(ModFolder + "\\action/script/c9997.hks");
                else
                    Enemy = File.ReadAllText(Root + "mod/action/script/c9997.hks");
                // FMG and EMEVD
                BND4 itemBND;
                if (File.Exists(ModFolder + "\\msg/engus/item.msgbnd.dcx") && File.Exists(ModFolder + "\\event/common.emevd.dcx")) {
                    itemBND = BND4.Read(ModFolder + "\\msg/engus/item.msgbnd.dcx");
                    Common = EMEVD.Read(ModFolder + "\\event/common.emevd.dcx");
                }
                else {
                    GetGamePath();
                    newnotfound:
                    if (!File.Exists(GamePath + "/msg/engus/item.msgbnd.dcx") || !File.Exists(GamePath + "/event/common.emevd.dcx")) {
                        Console.WriteLine("\nNo item.msgbnd.dcx / common.emevd.dcx found in mod folder, open the UXM Selective Unpacker that you installed from Nexus Mods, then follow the instructions below.\n(https://www.nexusmods.com/eldenring/mods/1651)" +
                            "\n1. Check the box next to \"Use Selected Files\"\n2. Click on \"View Files\" (If the new window has no options then the executable path within UXM is incorrect)\n3. Now click the arrows on (>EldenRing >msg >engus (Check)item.msgbnd.dcx) & (>EldenRing >event (Check)common.emevd.dcx)\n4. Click \"Ok\" then \"Unpack\"\n\nWait for it to finish, then press any key to continue.");
                        Thread.Sleep(500);
                        Console.ReadKey();
                        goto newnotfound;
                    } else {
                        itemBND = BND4.Read(GamePath + "/msg/engus/item.msgbnd.dcx");
                        Common = EMEVD.Read(GamePath + "/event/common.emevd.dcx");
                    }
                }
                WeaponNameFMG = FMG.Read(itemBND.Files[1].Bytes);
                foreach (var Entry in WeaponNameFMG.Entries)
                    WeaponName.Add(Entry.ID, Entry);
                WeaponEffectFMG = FMG.Read(itemBND.Files[23].Bytes);
                foreach (var Entry in WeaponEffectFMG.Entries)
                    WeaponEffect.Add(Entry.ID, Entry);
                // Loading a parambnd
                string regulation = ModFolder + "\\regulation.bin";
                regulationnotfound:
                if (!File.Exists(regulation)) {
                    GetGamePath();
                    regulation = GamePath + "regulation.bin";
                    goto regulationnotfound;
                }
                BND4 paramBnd = SFUtil.DecryptERRegulation(regulation);
                Params = InitializeParams(paramBnd);

                // TESTERONIEST
                //RogueLikeEquipment(paramBnd);

                // Change speffect 0 into a copyable one
                Params["SpEffectParam"][0].Name = "HarmoniousRenewal";
                Params["SpEffectParam"][0]["effectEndurance"].Value = 0;
                Params["SpEffectParam"][0]["hpRecoverRate"].Value = 1;
                Params["SpEffectParam"][0]["equipWeightChangeRate"].Value = 1;
                Params["SpEffectParam"][0]["soulRate"].Value = 1;
                Params["SpEffectParam"][0]["fallDamageRate"].Value = 1;
                Params["SpEffectParam"][0]["lifeReductionRate"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetSelf"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetFriend"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetEnemy"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetPlayer"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetAI"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetLive"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetGhost"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetOpposeTarget"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetFriendlyTarget"].Value = 1;
                Params["SpEffectParam"][0]["effectTargetSelfTarget"].Value = 1;
                Params["SpEffectParam"][0]["isDisableNetSync"].Value = 0;
                Params["SpEffectParam"][0]["isIgnoreNoDamage"].Value = 1;
                for (int i = 0; i <= 15; i++) { Params["SpEffectParam"][0]["vowType" + i].Value = 1; }
                Params["SpEffectParam"][0]["allItemWeightChangeRate"].Value = 1;
                Params["SpEffectParam"][0]["soulStealRate"].Value = 1;
                Params["SpEffectParam"][0]["isDisableNetSync"].Value = 1;
                // Starting time
                double StartTime = GetTime();
                // Start the mod
                Console.WriteLine("\nScaling Weapons And Affinities... ");
                ScaleWeapons();
                Console.WriteLine("\nOverhauling Players, Enemies And Combat... ");
                OverhaulCombat();
                Console.WriteLine("\nSpicing Up Merchants And Weather... ");
                WeatheredMerchants();
                Console.WriteLine("\nEnabling And Unchaining Weapon Buffs... ");
                BuffWeapons();
                Console.WriteLine("\nGenerating Ashes Of War... ");
                //AshOfWar();
                
                // Write files
                string WriteDirectory = Root + ModDate;
                if (Directory.Exists(WriteDirectory)) Directory.Delete(WriteDirectory, true);
                Directory.CreateDirectory(WriteDirectory + "/action/script");
                Directory.CreateDirectory(WriteDirectory + "/event");
                // Create project.json
                File.WriteAllText(WriteDirectory + "\\project.json",
                    "{\n" +
                    "   \"ProjectName\": \"HarmoniousRenewal\", \n" +
                    "   \"GameRoot\": \"C:/Program Files (x86)/Steam/steamapps/common/ELDEN RING/Game\", \n" +
                    "   \"GameType\": 8, \n" +
                    "   \"PinnedParams\": [], \n" +
                    "   \"PinnedRows\": {}, \n" +
                    "   \"PinnedFields\": {}, \n" +
                    "   \"UseLooseParams\": false, \n" +
                    "   \"PartialParams\": false, \n" +
                    "   \"LastFmgLanguageUsed\": \"\" \n" +
                    "}\n");
                // Write config_eldenring.toml
                List<string> config = [.. File.ReadAllLines(ModEngine)];
                for (int i = config.Count - 1; i >= 0; i--)
                    if (config[i].Contains("HarmoniousRenewal"))
                        config.RemoveAt(i);
                string noConfig = string.Join(Environment.NewLine, config);
                File.WriteAllText(ModEngine, noConfig.Replace("mods = [", "mods = [\n{ enabled = true, name = \"HarmoniousRenewal\", path = \"" + WriteDirectory.Replace("\\", "\\\\") + "\" },"));
                // Save each param, then the parambnd
                foreach (BinderFile file in paramBnd.Files) {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    if (Params.TryGetValue(name, out PARAM? value))
                        file.Bytes = value.Write();
                }
                paramBnd.Write(WriteDirectory + "regulation.bin");
                SFUtil.EncryptERRegulation(WriteDirectory + "regulation.bin", BND4.Read(WriteDirectory + "regulation.bin"));
                // Write FMG
                itemBND.Files[1].Bytes = WeaponNameFMG.Write();
                itemBND.Files[23].Bytes = WeaponEffectFMG.Write();
                itemBND.Write(WriteDirectory + "msg/engus/item.msgbnd.dcx");
                // Write HKS
                Directory.CreateDirectory(WriteDirectory + "action/script/");
                File.WriteAllText(WriteDirectory + "action/script/c0000.hks", Player);
                File.WriteAllText(WriteDirectory + "action/script/c9997.hks", Enemy);
                // Write EMEVD
                Common.Write(WriteDirectory + "event/common.emevd.dcx");
                // Ending notification
                Console.Beep();
                Console.WriteLine("\nFinished in " + GetTime(StartTime) + " seconds!\nPress any key to exit.");
                Thread.Sleep(500);
                Console.ReadKey();
            }
            catch (Exception exception) {
                Console.WriteLine($"An error occurred: {exception.Message}");
                Console.WriteLine($"Stack trace: {exception.StackTrace}");
                Console.WriteLine("Press any key to exit...");
                Thread.Sleep(500);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static void ScaleWeapons() {
            // Starting time
            double StartTime = GetTime();
            // Get base damage that all weapons scale with for the purpose of correction, including flexibility for modded increases
            double Corbase = 69;
            double Overbase = 0;
            foreach (var Stat in Cache["Stats"])
                Overbase += Value(Params["EquipParamWeapon"][18110000]["correct" + Stat]);
            Overbase /= Corbase;
            // Initialize all dictionaries
            Dictionary<double, List<double>> Averages = [];
            Dictionary<double, List<double>> Extras = [];
            Dictionary<double, List<double>> Damages = [];
            Dictionary<double, List<double>> Corrects = [];
            Dictionary<double, List<double>> Behaviors = [];
            // List of all 0000 ID's
            List<PARAM.Row> Parents = [];
            Params["EquipParamWeapon"].Rows.ForEach(Row => {
                // If it is indeed a weapon
                if (IsWeapon(Row)) {
                    // If row does not have any number in last four digits
                    if ((Row.ID % 10000) == 0) {
                        Parents.Add(Row);
                        // Fill up dictionary with average values for each weapon class
                        Averages.TryAdd(Value(Row["wepType"]), []);
                        for (int i = 0; i < Cache["Stats"].Length; i++)
                            Averages[Value(Row["wepType"])].Add(Value(Row["proper" + Cache["Stats"][i]]));
                    }
                }
            });
            // Create speffects for hks affinities
            List<int> SpEffinity = [];
            Dictionary<double, List<double>> SpEffinityMsg = [];
            int NoAffinity = New(Params["SpEffectParam"]);
            Player = Player.Replace("-- Core: Global Variables\n------------------------",
                "-- Core: Global Variables\n------------------------\n" +
                "AFFINITY_HARMONY = {}\n");
            Player = Player.Replace("function Update()",
                "function ClearAffinityHarmony()\n" +
                "end\n\n" +
                "function Update()");
            Player = Player.Replace("function Update()",
                "function HarmonizeAffinity()\n" +
                "    ClearAffinityHarmony()\n" +
                "end\n\n" +
                "function Update()");
            Player = Player.Replace("function Update()",
                "function Update()\n" +
                "    if (env(GetSpEffectID, AFFINITY_HARMONY[1]) == FALSE or env(GetSpEffectID, AFFINITY_HARMONY[2]) == FALSE) and (AFFINITY_HARMONY[1] ~= nil or AFFINITY_HARMONY[2] ~= nil) then\n" +
                "        AFFINITY_HARMONY = {}\n" +
                "        HarmonizeAffinity()\n" +
                "    end\n");
            // Void to add affinities to hks
            static void AffinityHks(int ID, List<double> Resident) {
                // Add in a check for each affinity speffect
                Player = Player.Replace("    ClearAffinityHarmony()",
                        "    ClearAffinityHarmony()\n" +
                        "    if env(GetSpEffectID, " + ID + ") == TRUE then\n" +
                        "        if AFFINITY_HARMONY[1] == nil then\n" +
                        "            AFFINITY_HARMONY[1] = " + ID + "\n" +
                        "        else\n" +
                        "            AFFINITY_HARMONY[2] = " + ID + "\n" +
                        "        end\n" +
                        "    end");
                // Loop through each effect
                foreach (var Effect in Resident)
                    if (Effect != -1 && Params["SpEffectParam"][Convert.ToInt32(Effect)] != null) {
                        if (!Player.Contains("    act(9001, " + Effect + ")"))
                            Player = Player.Replace("function ClearAffinityHarmony()",
                            "function ClearAffinityHarmony()\n" +
                            "    act(9001, " + Effect + ")");
                        Player = Player.Replace("if env(GetSpEffectID, " + ID + ") == TRUE then",
                            "if env(GetSpEffectID, " + ID + ") == TRUE then\n" +
                            "        act(AddSpEffect, " + Effect + ")");
                    }
            }
            // Loop through all affinities until the next one is not found, with an extra safety incase some mod goes past 9999 somehow
            int currow = 100;
            while (currow < 9999) {
                PARAM.Row? Row = Params["EquipParamWeapon"][18110000 + currow];
                if (Row != null) {
                    // Setup affinity speffect list
                    SpEffinity.Add(New(Params["SpEffectParam"]));
                    Params["SpEffectParam"][SpEffinity.Last()].Name = WeaponName[18110000 + currow].Text.Replace("Guardian's ", "").Replace(" Swordspear", "");
                    Params["SpEffectParam"][SpEffinity.Last()]["effectEndurance"].Value = -1;
                    // HKS sense
                    List<double> Resident = [];
                    Resident.Add(Value(Row["residentSpEffectId"]) != Value(Params["EquipParamWeapon"][18110000]["residentSpEffectId"]) ? Value(Row["residentSpEffectId"]) : -1);
                    Resident.Add(Value(Row["residentSpEffectId1"]) != Value(Params["EquipParamWeapon"][18110000]["residentSpEffectId1"]) ? Value(Row["residentSpEffectId1"]) : -1);
                    Resident.Add(Value(Row["residentSpEffectId2"]) != Value(Params["EquipParamWeapon"][18110000]["residentSpEffectId2"]) ? Value(Row["residentSpEffectId2"]) : -1);
                    if (!Resident.All(Effect => Effect == -1 || Params["SpEffectParam"][Convert.ToInt32(Effect)] == null))
                        AffinityHks(SpEffinity.Last(), Resident);
                    // Fill all dictionaries with current row
                    Extras.Add(currow, []);
                    Damages.Add(currow, []);
                    Corrects.Add(currow, []);
                    Behaviors.Add(currow, []);
                    for (int y = 0; y <= 2; y++)
                        if (Value(Row["spEffectBehaviorId" + y]) != Value(Params["EquipParamWeapon"][18110000]["spEffectBehaviorId" + y]))
                            Behaviors[currow].Add(Value(Row["spEffectBehaviorId" + y]));
                        else Behaviors[currow].Add(-1);
                    SpEffinityMsg.Add(currow, []);
                    // Set up affinity msg's
                    SpEffinityMsg[currow].Add(Value(Row["spEffectMsgId0"]) != Value(Params["EquipParamWeapon"][18110000]["spEffectMsgId0"]) ? Value(Row["spEffectMsgId0"]) : -1);
                    SpEffinityMsg[currow].Add(Value(Row["spEffectMsgId1"]) != Value(Params["EquipParamWeapon"][18110000]["spEffectMsgId1"]) ? Value(Row["spEffectMsgId1"]) : -1);
                    SpEffinityMsg[currow].Add(Value(Row["spEffectMsgId2"]) != Value(Params["EquipParamWeapon"][18110000]["spEffectMsgId2"]) ? Value(Row["spEffectMsgId2"]) : -1);
                    // Get all extra stats, which are just direct overwrites
                    foreach (var Extra in Cache["Extras"])
                        if (Value(Row[Extra]) != Value(Params["EquipParamWeapon"][18110000][Extra]))
                            Extras[currow].Add(Value(Row[Extra]));
                        else
                            Extras[currow].Add(-1);
                    // Find which damage types are present for each affinity
                    foreach (var Damage in Cache["Damages"])
                        if (Value(Row[Damage]) != 0)
                            Damages[currow].Add(1);
                        else
                            Damages[currow].Add(-1);
                    // Get all scaling changes for affinities
                    foreach (var Stat in Cache["Stats"])
                        if (Stat is "Strength" or "Agility")
                            Corrects[currow].Add(Value(Row["correct" + Stat]));
                        else
                            Corrects[currow].Add(Value(Row["correct" + Stat]) * 1.5);
                } else break;
                // Increment current row by 100 (Affinity increment)
                currow += 100;
            }
            // Handle hks for weapons
            Player = Player.Replace("-- Core: Global Variables\n------------------------",
                "-- Core: Global Variables\n------------------------\n" +
                "WEAPON_HARMONY = {}\n");
            Player = Player.Replace("function Update()",
                "function ClearWeaponHarmony()\n" +
                "end\n\n" +
                "function Update()");
            Player = Player.Replace("function Update()",
                "function HarmonizeWeapon()\n" +
                "    ClearWeaponHarmony()\n" +
                "end\n\n" +
                "function Update()");
            Player = Player.Replace("function Update()",
                "function Update()\n" +
                "    if (env(GetSpEffectID, WEAPON_HARMONY[1]) == FALSE or env(GetSpEffectID, WEAPON_HARMONY[2]) == FALSE) and (WEAPON_HARMONY[1] ~= nil or WEAPON_HARMONY[2] ~= nil) then\n" +
                "        WEAPON_HARMONY = {}\n" +
                "        HarmonizeWeapon()\n" +
                "    end\n");
            // Void for setting weapons into the hks
            static void WeaponHks(int ID, List<double> Resident) {
                Player = Player.Replace("    ClearWeaponHarmony()",
                        "    ClearWeaponHarmony()\n" +
                        "    if env(GetSpEffectID, " + ID + ") == TRUE then\n" +
                        "        if WEAPON_HARMONY[1] == nil then\n" +
                        "            WEAPON_HARMONY[1] = " + ID + "\n" +
                        "        else\n" +
                        "            WEAPON_HARMONY[2] = " + ID + "\n" +
                        "        end\n" +
                        "    end");
                // Loop through each effect
                foreach (var Effect in Resident)
                    if (Effect != -1 && Params["SpEffectParam"][Convert.ToInt32(Effect)] != null)
                    {
                        if (!Player.Contains("    act(9001, " + Effect + ")"))
                            Player = Player.Replace("function ClearWeaponHarmony()",
                            "function ClearWeaponHarmony()\n" +
                            "    act(9001, " + Effect + ")");
                        Player = Player.Replace("if env(GetSpEffectID, " + ID + ") == TRUE then",
                            "if env(GetSpEffectID, " + ID + ") == TRUE then\n" +
                            "        act(AddSpEffect, " + Effect + ")");
                    }
            }
            // Dictionary for keeping track of effects
            Dictionary<string, int> CheckMSG = [];
            // Loop through parent weapons
            Parents.ForEach(Row => {
                // Set weapon to equip gems
                Row["isEnhance"].Value = 1;
                Row["gemMountType"].Value = 2;
                Row["disableGemAttr"].Value = 0;
                // Allow for upgrading to +25
                for (int i = 1; i <= 25; i++)
                    Row["originEquipWep" + i].Value = Value(Row["originEquipWep"]);
                // Go through each stat, giving more proper to the ones that have correct but no proper
                foreach (var Stat in Cache["Stats"])
                    if (Value(Row["proper" + Stat]) <= 0 && Value(Row["correct" + Stat]) > 0)
                        Row["proper" + Stat].Value = Value(Row["proper" + Stat]) + Value(Row["correct" + Stat]) / Overbase;
                    else if (Value(Row["correct" + Stat]) > 0)
                        Row["proper" + Stat].Value = Value(Row["proper" + Stat]) + Value(Row["correct" + Stat]) / Overbase / 4.25;
                // Set the correct based on proper, giving more correct if correct was at 0
                for (int i = 0; i < Cache["Stats"].Length; i++) {
                    double correct = (Corbase * Overbase) * Math.Pow(Difference(Row["proper" + Cache["Stats"][i]], Averages[Value(Row["wepType"])].Average()), 2.25) / 2.25;
                    Row["correct" + Cache["Stats"][i]].Value = Value(Row["correct" + Cache["Stats"][i]]) <= 0 ? correct * 2.5 : correct;
                }
                // Remove special scaling from weapons
                Row["reinforceTypeId"].Value = Value(Params["EquipParamWeapon"][18110000]["reinforceTypeId"]);
                // Get weapon base damage and weapon base scaling
                double BasePhysicalDamage = 0;
                double BaseMagicalDamage = 0;
                double BasePhysicalCorrect = 0;
                double BaseMagicalCorrect = 0;
                for (int i = 0; i <= 4; i++) {
                    if (double.IsNaN(Value(Row["correct" + Cache["Stats"][i]])))
                        Row["correct" + Cache["Stats"][i]].Value = 0;
                    else if (double.IsNaN(Value(Row[Cache["Damages"][i]])))
                        Row[Cache["Damages"][i]].Value = 0;
                    if (Cache["Damages"][i] == "attackBasePhysics") {
                        BasePhysicalDamage += Value(Row[Cache["Damages"][i]]);
                        BaseMagicalDamage += Value(Row[Cache["Damages"][i]]) * 1.65;
                    } else {
                        BasePhysicalDamage += Value(Row[Cache["Damages"][i]]) / 2.385;
                        BaseMagicalDamage += Value(Row[Cache["Damages"][i]]);
                    }
                    if (Cache["Stats"][i] is "Strength" or "Agility") {
                        BasePhysicalCorrect += Value(Row["correct" + Cache["Stats"][i]]);
                        BaseMagicalCorrect += Value(Row["correct" + Cache["Stats"][i]]) * 1.65;
                    } else {
                        BasePhysicalCorrect += Value(Row["correct" + Cache["Stats"][i]]) / 2.385;
                        BaseMagicalCorrect += Value(Row["correct" + Cache["Stats"][i]]);
                    }
                }
                // Extra stats based on stamina
                if (Value(Row["stealthAtkRate"]) > 0)
                    Row["stealthAtkRate"].Value = Math.Pow(Value(Row["staminaConsumptionRate"]), -10) * 20;
                if (Value(Row["throwAtkRate"]) > 0)
                    Row["throwAtkRate"].Value = Math.Pow(Value(Row["staminaConsumptionRate"]), -10) * 20;
                if (Value(Row["bowDistRate"]) > -1)
                    Row["bowDistRate"].Value = Math.Pow(Value(Row["staminaConsumptionRate"]), 10) * 20;
                // Add weapon specific catcher
                int WeaponId = -1;
                // HKS sense
                List<double> Resident = [];
                Resident.Add(Value(Row["residentSpEffectId"]));
                Resident.Add(Value(Row["residentSpEffectId1"]));
                Resident.Add(Value(Row["residentSpEffectId2"]));
                if (!Resident.All(Effect => Effect == -1 || Params["SpEffectParam"][Convert.ToInt32(Effect)] == null)) {
                    if (WeaponId == -1)
                        WeaponId = New(Params["SpEffectParam"]);
                    Params["SpEffectParam"][WeaponId].Name = WeaponName[Row.ID].Text;
                    Params["SpEffectParam"][WeaponId]["effectEndurance"].Value = -1;
                    Row["residentSpEffectId"].Value = WeaponId;
                    WeaponHks(WeaponId, Resident);
                } else
                    Row["residentSpEffectId"].Value = -1;
                Row["residentSpEffectId1"].Value = -1;
                Row["residentSpEffectId2"].Value = -1;
                // Generate weapon affinities
                for (int i = 1; i <= Extras.Count; i++) {
                    // Create new row, overwriting any previous one
                    Copy(Params["EquipParamWeapon"], Row.ID, Row.ID + i * 100);
                    PARAM.Row Affinity = Params["EquipParamWeapon"][Row.ID + i * 100];
                    string Name = Params["SpEffectParam"][SpEffinity[i - 1]].Name + " " + WeaponName[Row.ID].Text;
                    Affinity.Name = Name;
                    Affinity["sortId"].Value = Value(Affinity["sortId"]) + i;
                    // Go through each extra and overwrite the affinity with it
                    for (int y = 0; y < Cache["Extras"].Length; y++)
                        if (Extras[i * 100][y] != -1)
                            Affinity[Cache["Extras"][y]].Value = Extras[i * 100][y];
                    // Deal with the affinity weapon damage, concatenated with scaling values
                    for (int y = 0; y < Cache["Damages"].Length; y++) {
                        // Store some values for smaller code
                        double damage = Value(Affinity[Cache["Damages"][y]]);
                        double correct = Value(Affinity["correct" + Cache["Stats"][y]]);
                        double scale = Corrects[i * 100][y] / Corrects[i * 100].Sum();
                        // Scale weapon damage values, with a flat division to make out of use stats less tall
                        if (Damages[i * 100][y] != -1)
                            if (Cache["Damages"][y] == "attackBasePhysics")
                                Affinity[Cache["Damages"][y]].Value = (damage + BasePhysicalDamage) / 2;
                            else
                                Affinity[Cache["Damages"][y]].Value = (damage + BaseMagicalDamage) / 2;
                        else
                            Affinity[Cache["Damages"][y]].Value = damage / 1.35;
                        // Scale weapon correct values, with a flat division to make out of use stats less tall
                        if (Cache["Stats"][y] is "Strength" or "Agility")
                            Affinity["correct" + Cache["Stats"][y]].Value = ((correct > BasePhysicalCorrect / 1.5 ? correct * 1.75 : correct)  + BasePhysicalCorrect * scale) / 2.25;
                        else
                            Affinity["correct" + Cache["Stats"][y]].Value = ((correct > BaseMagicalCorrect / 1.5 ? correct * 1.75 : correct) + BaseMagicalCorrect * scale) / 2.25;
                        Affinity[Cache["Damages"][y]].Value = Value(Affinity[Cache["Damages"][y]]) > correct * 1.65 ? Value(Affinity[Cache["Damages"][y]]) / 1.45 : Value(Affinity[Cache["Damages"][y]]);
                        Affinity["correct" + Cache["Stats"][y]].Value = Value(Affinity["correct" + Cache["Stats"][y]]) > correct * 1.65 ? Value(Affinity["correct" + Cache["Stats"][y]]) / 1.45 : Value(Affinity["correct" + Cache["Stats"][y]]);
                    }
                    // Go through weapon behaviors
                    Dictionary<string, PARAM.Row> AffinityAttribute = [];
                    Dictionary<string, PARAM.Row> ParentAttribute = [];
                    List<int> Extra = [];
                    List<int> CompiledBehavior = [];
                    // loop through all, if has attackpowertype add to Attribute, else add to Extra
                    for (int y = 0; y <= 2; y++) {
                        PARAM.Row? Aff = Params["SpEffectParam"][Convert.ToInt32(Behaviors[i * 100][y])];
                        PARAM.Row? Par = Params["SpEffectParam"][Convert.ToInt32(Row["spEffectBehaviorId" + y].Value)];
                        foreach (var Status in Cache["Statuses"]) {
                            if (Aff != null && Value(Aff[Status]) != 0)
                                AffinityAttribute.Add(Status, Aff);
                            if (Par != null && Value(Par[Status]) != 0)
                                ParentAttribute.Add(Status, Par);
                        }
                        if (Aff != null && !AffinityAttribute.ContainsValue(Aff)) Extra.Add(Aff.ID);
                        if (Par != null && !ParentAttribute.ContainsValue(Par)) Extra.Add(Par.ID);
                    }
                    foreach (var Status in Cache["Statuses"]) {
                        AffinityAttribute.TryGetValue(Status, out PARAM.Row? AffVal);
                        ParentAttribute.TryGetValue(Status, out PARAM.Row? ParVal);
                        if (ParVal != null && AffVal != null) {
                            CompiledBehavior.Add(ParVal.ID);
                            Affinity["correctLuck"].Value = Value(Affinity["correctLuck"]) * 1.45;
                        } else if (AffVal != null)
                            CompiledBehavior.Add(AffVal.ID);
                        else if (ParVal != null)
                            CompiledBehavior.Add(ParVal.ID);
                    }
                    CompiledBehavior.AddRange(Extra);
                    //CompiledBehavior = CompiledBehavior.Distinct().ToList();
                    for (int y = 0; y <= 2; y++)
                        Affinity["spEffectBehaviorId" + y].Value = CompiledBehavior.Count > y ? CompiledBehavior[y] : -1;
                    // Combine effect fmg's into one
                    AddFMG(WeaponNameFMG, WeaponName, Affinity.ID, Name);
                    int ID = 9999999;
                    string Message = "";
                    for (int y = 0; y <= 2; y++) {
                        if (Value(Affinity["spEffectMsgId" + y]) > -1)
                            if (!Message.Contains(WeaponEffect[Convert.ToInt32(Affinity["spEffectMsgId" + y].Value)].Text))
                                Message += WeaponEffect[Convert.ToInt32(Affinity["spEffectMsgId" + y].Value)].Text + "\n";
                        if (SpEffinityMsg[i * 100][y] > -1)
                            if (!Message.Contains(WeaponEffect[Convert.ToInt32(SpEffinityMsg[i * 100][y])].Text))
                                Message += WeaponEffect[Convert.ToInt32(SpEffinityMsg[i * 100][y])].Text + "\n";
                    }
                    if (Message != "")
                        if (CheckMSG.TryGetValue(Message, out int value))
                            Affinity["spEffectMsgId0"].Value = value;
                        else {
                            while (true)
                                if (!WeaponEffect.ContainsKey(ID)) {
                                    AddFMG(WeaponEffectFMG, WeaponEffect, ID, Message);
                                    break;
                                } else ID--;
                            CheckMSG.Add(Message, ID);
                            Affinity["spEffectMsgId0"].Value = ID;
                        }
                    // Clear and set speffects
                    Affinity["residentSpEffectId2"].Value = -1;
                    Affinity["residentSpEffectId1"].Value = SpEffinity[i - 1];
                    Affinity["spEffectMsgId1"].Value = -1;
                    Affinity["spEffectMsgId2"].Value = -1;
                    // Spice up proper values
                    foreach (var Stat in Cache["Stats"])
                        Affinity["proper" + Stat].Value = Value(Affinity["proper" + Stat]) / 2.25 + Value(Affinity["correct" + Stat]) / Overbase / 2.25;
                    // Spice up side values
                    void SpiceValue(string V1, string V2) => Affinity[V1].Value = Value(Row[V1]) + Value(Affinity[V2]) / 1.5;
                    SpiceValue("attackBaseStamina", "properStrength");
                    SpiceValue("saWeaponDamage", "properStrength");
                    SpiceValue("physGuardCutRate", "properStrength");
                    SpiceValue("magGuardCutRate", "properMagic");
                    SpiceValue("fireGuardCutRate", "properMagic");
                    SpiceValue("thunGuardCutRate", "properFaith");
                    SpiceValue("darkGuardCutRate", "properFaith");
                    SpiceValue("staminaGuardDef", "properStrength");
                    SpiceValue("poisonGuardResist", "properLuck");
                    SpiceValue("diseaseGuardResist", "properLuck");
                    SpiceValue("bloodGuardResist", "properLuck");
                    SpiceValue("curseGuardResist", "properFaith");
                    SpiceValue("freezeGuardResist", "properMagic");
                    SpiceValue("sleepGuardResist", "properMagic");
                    SpiceValue("madnessGuardResist", "properFaith");
                    
                    // After scaling finished, remove some stat requirements from the weapon, so that they are usuable at lower levels
                    foreach (var Stat in Cache["Stats"])
                        Affinity["proper" + Stat].Value = Value(Affinity["proper" + Stat]) / 3 + Value(Affinity["correct" + Stat]) / 5;
                }
                
                // After scaling finished, remove some stat requirements from the weapon, so that they are usuable at lower levels
                foreach (var Stat in Cache["Stats"])
                    Row["proper" + Stat].Value = Value(Row["proper" + Stat]) / 2.25;
            });
            // Make somber also go up to +25
            Params["EquipMtrlSetParam"][2208].ID = 2222;
            Copy(Params["EquipMtrlSetParam"], 2222, 2223);
            Params["EquipMtrlSetParam"][2209].ID = 2224;
            Params["EquipMtrlSetParam"][2210].ID = 2225;
            Copy(Params["EquipMtrlSetParam"], 2209, 2224);
            List<int> MtrlId = [2201, 2202, 2203, 2204, 2205, 2206, 2207];
            List<int> NMtrlId = [2201, 2204, 2207, 2210, 2213, 2216, 2219];
            for (int i = MtrlId.Count - 1; i > 0; i--)
                Params["EquipMtrlSetParam"][MtrlId[i]].ID = NMtrlId[i];
            for (int i = 0; i < MtrlId.Count; i++) {
                Copy(Params["EquipMtrlSetParam"], NMtrlId[i], NMtrlId[i] + 1);
                Copy(Params["EquipMtrlSetParam"], NMtrlId[i], NMtrlId[i] + 2);
            }
            // Remove speffect offsets
            Params["ReinforceParamWeapon"].Rows.ForEach(Row => {
                Row["spEffectId1"].Value = 0;
                Row["spEffectId2"].Value = 0;
                Row["spEffectId3"].Value = 0;
                Row["residentSpEffectId1"].Value = 0;
                Row["residentSpEffectId2"].Value = 0;
                Row["residentSpEffectId3"].Value = 0;
            });
            
            // Ending message
            Console.WriteLine("- Generated & Scaled " + Parents.Count * Extras.Count + " weapons in " + GetTime(StartTime) + " seconds!");
        }
        
        public static void OverhaulCombat() {
            // Starting time
            double StartTime = GetTime();
            // Void for creating cooldowns
            static int Cooldown(string Name = "", double Cooldown = 0) {
                int id = New(Params["SpEffectParam"]);
                Params["SpEffectParam"][id].Name = Name;
                Params["SpEffectParam"][id]["effectEndurance"].Value = Cooldown;
                Params["SpEffectParam"][id]["spCategory"].Value = 20;
                return id;
            }
            // Replace and create an on-hit function
            Enemy = Enemy.Replace("env(GetDamageLevel)", "HarmonizeHit()");
            Enemy = Enemy.Replace("function Update()",
                string.Format("function HarmonizeHit()\n" +
                "    damage_level = env(GetDamageLevel)\n" +
                "    if damage_level == DAMAGE_LEVEL_SMALL and env(GetSpEffectID, {0}) == FALSE then\n" +
                "        act(AddSpEffect, {0})\n" +
                "        return DAMAGE_LEVEL_SMALL\n" +
                "    elseif damage_level == DAMAGE_LEVEL_MIDDLE and env(GetSpEffectID, {1}) == FALSE then\n" +
                "        act(AddSpEffect, {1})\n" +
                "        return DAMAGE_LEVEL_MIDDLE\n" +
                "    elseif damage_level == DAMAGE_LEVEL_LARGE and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_LARGE\n" +
                "    elseif damage_level == DAMAGE_LEVEL_LARGE_BLOW and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_LARGE_BLOW\n" +
                "    elseif damage_level == DAMAGE_LEVEL_PUSH and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_PUSH\n" +
                "    elseif damage_level == DAMAGE_LEVEL_FLING and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_FLING\n" +
                "    elseif damage_level == DAMAGE_LEVEL_SMALL_BLOW and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_SMALL_BLOW\n" +
                "    elseif damage_level == DAMAGE_LEVEL_MINIMUM and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_MINIMUM\n" +
                "    elseif damage_level == DAMAGE_LEVEL_UPPER and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_UPPER\n" +
                "    elseif damage_level == DAMAGE_LEVEL_EX_BLAST and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_EX_BLAST\n" +
                "    elseif damage_level == DAMAGE_LEVEL_BREATH and env(GetSpEffectID, {2}) == FALSE then\n" +
                "        act(AddSpEffect, {2})\n" +
                "        return DAMAGE_LEVEL_BREATH\n" +
                "    else\n" +
                "        return DAMAGE_LEVEL_NONE\n" +
                "    end\n" +
                "end\n\n" +
                "function Update()", Cooldown("SmallCooldown", 6), Cooldown("MediumCooldown", 5), Cooldown("LargeCooldown", 4)));
            // Introduce animation cancels for players
            int Animation = Cooldown("AnimationCooldown", 0.65);
            int Backstep = Cooldown("BackstepCooldown", 0.75);
            int L2 = Cooldown("L2Cooldown", 0.75);
            Player = Player.Replace(
                "    if env(ActionRequest, ACTION_ARM_JUMP) == FALSE and env(700) == FALSE then",
                "    if env(ActionRequest, ACTION_ARM_JUMP) == FALSE and env(700) == FALSE and (env(ActionCancelRequest, ACTION_ARM_JUMP) == FALSE or env(GetSpEffectID, " + Animation + ") == TRUE) or (env(ActionDuration, ACTION_ARM_L1) > 100 or env(IsBeingThrown) == TRUE or env(IsThrowing) == TRUE) then");
            Player = Player.Replace(
                "function ExecEvasion(backstep_limit, estep, is_usechainrecover)\n" +
                "    if c_HasActionRequest == FALSE",
                "function ExecEvasion(backstep_limit, estep, is_usechainrecover)\n" +
                "    if c_HasActionRequest == FALSE and env(ActionCancelRequest, ACTION_ARM_ROLLING) == FALSE and env(ActionCancelRequest, ACTION_ARM_BACKSTEP) == FALSE or env(IsBeingThrown) == TRUE or env(IsThrowing) == TRUE");
            Player = Player.Replace(
                "elseif request == ATTACK_REQUEST_INVALID then",
                "elseif request == ATTACK_REQUEST_INVALID and env(ActionCancelRequest, ACTION_ARM_ROLLING) == FALSE and env(ActionCancelRequest, ACTION_ARM_BACKSTEP) == FALSE or env(IsBeingThrown) == TRUE or env(IsThrowing) == TRUE then");
            Player = Player.Replace(
                "    if request == ATTACK_REQUEST_ROLLING then",
                "    if request == ATTACK_REQUEST_ROLLING or (env(ActionCancelRequest, ACTION_ARM_ROLLING) == TRUE and env(GetSpEffectID, " + Animation + ") == FALSE) then\n" +
                "        act(AddSpEffect, " + Animation + ")");
            Player = Player.Replace(
                "    elseif request == ATTACK_REQUEST_BACKSTEP then",
                "    elseif request == ATTACK_REQUEST_BACKSTEP or (env(ActionCancelRequest, ACTION_ARM_BACKSTEP) == TRUE and env(GetSpEffectID, " + Animation + ") == FALSE) and env(GetSpEffectID, " + Backstep + ") == FALSE then\n" +
                "        act(AddSpEffect, " + Animation + ")\n" +
                "        act(AddSpEffect, " + Backstep + ")");
            Player = Player.Replace(
                "    if env(IsGuardFromAtkCancel) == FALSE then",
                "    if env(IsGuardFromAtkCancel) == FALSE and env(ActionCancelRequest, ACTION_ARM_L1) == False or env(ActionDuration, ACTION_ARM_L1) > 100 or env(GetSpEffectID, " + Animation + ") == TRUE or env(IsBeingThrown) == TRUE or env(IsThrowing) == TRUE then");
            Player = Player.Replace(
                "    elseif request_l2 == TRUE then",
                "    elseif request_l2 == TRUE or (env(ActionCancelRequest, ACTION_ARM_L2) == TRUE and env(GetSpEffectID, " + Animation + ") == FALSE and env(GetSpEffectID, " + L2 + ") == FALSE and env(IsBeingThrown) == FALSE and env(IsThrowing) == FALSE and (c_SwordArtsID == 92 or c_SwordArtsID == 93 or c_SwordArtsID == 97)) then\n" +
                "        act(AddSpEffect, " + Animation + ")\n" +
                "        act(AddSpEffect, " + L2 + ")");
            // Strong attack scaling
            Params["AtkParam_Pc"].Rows.ForEach(Row =>
            {
                if (Convert.ToInt32(Row["spEffectId0"].Value) is <= 6909 and >= 6900)
                {
                    Row["atkStamCorrection"].Value = Convert.ToDouble(Row["atkStamCorrection"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["atkSuperArmorCorrection"].Value = Convert.ToDouble(Row["atkSuperArmorCorrection"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["guardBreakCorrection"].Value = Convert.ToDouble(Row["guardBreakCorrection"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["statusAilmentAtkPowerCorrectRate"].Value = Convert.ToDouble(Row["statusAilmentAtkPowerCorrectRate"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["spEffectAtkPowerCorrectRate_byRate"].Value = Convert.ToDouble(Row["spEffectAtkPowerCorrectRate_byRate"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["spEffectAtkPowerCorrectRate_byDmg"].Value = Convert.ToDouble(Row["spEffectAtkPowerCorrectRate_byDmg"].Value) * (0.9 + 0.05 * (Convert.ToDouble(Row["spEffectId0"].Value) - 6900));
                    Row["isDisableBothHandsAtkBonus"].Value = 0;
                }
            });
            // Change bullet mechanics
            Params["Bullet"].Rows.ForEach(Row =>
            {
                if (Row["sfxId_Hit"].Value.ToString() != "-1" && Row["maxVellocity"].Value.ToString() != "0" && Row["initVellocity"].Value.ToString() != "0" && Row["gravityInRange"].Value.ToString() != "0" && Row["dist"].Value.ToString() != "0" && Convert.ToDouble(Row["life"].Value) >= 1)
                {
                    Row["life"].Value = Convert.ToDouble(Row["life"].Value) * 7.25;
                    Row["homingAngle"].Value = Convert.ToDouble(Row["homingAngle"].Value) + 55;
                    Row["maxVellocity"].Value = Convert.ToDouble(Row["maxVellocity"].Value) * 1.65;
                }
            });
            int Enemies = 0;
            Params["NpcParam"].Rows.ForEach(Row => {
                Enemies++;
                Row["superArmorDurability"].Value = Value(Row["superArmorDurability"]) * 2.45;
            });
            // Ending message
            Console.WriteLine("- Overhauled players, " + Enemies + " enemies and combat in " + GetTime(StartTime) + " seconds!");
        }

        public static void WeatheredMerchants() {
            // Starting time
            double StartTime = GetTime();
            // Randomize Weather
            List<double> Weather = [0, 1, 10, 11, 20, 21, 30, 31, 40, 41, 50, 51, 52, 60, 81, 82];
            Params["WeatherLotParam"].Rows.ForEach(Row => {
                for (int i = 0; i <= 15; i++) {
                    Row["lotteryWeight" + i].Value = 66;
                    Row["weatherType" + i].Value = Weather[i];
                    Row["timezoneLimit"].Value = 6;
                    Row["timezoneStartHour"].Value = 0;
                    Row["timezoneStartMinute"].Value = 0;
                    Row["timezoneEndHour"].Value = 23;
                    Row["timezoneEndMinute"].Value = 59;
                }
            });
            // Overhaul shop pricing
            static PARAM equipType(int Type)
            {
                if (Type == 0) { return Params["EquipParamWeapon"]; }
                else if (Type == 1) { return Params["EquipParamProtector"]; }
                else if (Type == 2) { return Params["EquipParamAccessory"]; }
                else if (Type == 3) { return Params["EquipParamGoods"]; }
                else { return Params["EquipParamGem"]; }
            }
            List<int> ID = [];
            Dictionary<int, List<double>> Prices = new()
            {
                { 0, new() },
                { 1, new() },
                { 2, new() },
                { 3, new() },
                { 4, new() },
            };
            Params["ShopLineupParam"].Rows.ForEach(Row =>
            {
                if (Row["costType"].Value.ToString() == "0" && equipType(Convert.ToInt32(Row["equipType"].Value))[Convert.ToInt32(Row["equipId"].Value)] != null)
                {
                    Row["value"].Value = Convert.ToInt32(Row["value"].Value) / (1.45 + (new Random().NextDouble() * 2.15)) * (Convert.ToDouble(equipType(Convert.ToInt32(Row["equipType"].Value))[Convert.ToInt32(Row["equipId"].Value)]["rarity"].Value) / 2 + 1.25);
                }
            });
            // Ending message
            Console.WriteLine("- Overhauled merchant prices and unchained weather in " + GetTime(StartTime) + " seconds!");
        }

        public static void BuffWeapons() {
            // Starting time
            double StartTime = GetTime();
            
            List<PARAM.Row> Buffs = [];
            Params["SpEffectParam"].Rows.ForEach(Row => {
                PARAM.Row? vfx = Params["SpEffectVfxParam"][Convert.ToInt32(Row["vfxId"].Value)];
                if (Value(Row["vfxId"]) > -1 && Value(Row["wepParamChange"]) is 1 or 2 && vfx != null && Value(vfx["effectType"]) == 1)
                    Buffs.Add(Row);
            });

            Buffs.ForEach(Row => {
                Dictionary<string, int> VfxStore = [];
                for (int i = 0; i <= 7; i++) {
                    var Key = "vfxId" + (i > 0 ? i : "");
                    if (Value(Row[Key]) > -1 && Params["SpEffectVfxParam"][Convert.ToInt32(Row[Key].Value)] != null)
                        VfxStore.Add(Key, Convert.ToInt32(Row[Key].Value));
                }
                int Vfx1 = Copy(Params["SpEffectParam"], Row.ID);
                Params["SpEffectParam"][Vfx1]["effectEndurance"].Value = 0.05;
                Params["SpEffectParam"][Vfx1]["motionInterval"].Value = 0;
                Params["SpEffectParam"][Vfx1]["spCategory"].Value = 0;
                int Vfx2 = Copy(Params["SpEffectParam"], Vfx1);
                Params["SpEffectParam"][Vfx1]["cycleOccurrenceSpEffectId"].Value = Vfx2;
                Params["SpEffectParam"][Vfx2]["cycleOccurrenceSpEffectId"].Value = Convert.ToInt32(Row["cycleOccurrenceSpEffectId"].Value);
                Params["SpEffectParam"][Vfx2]["wepParamChange"].Value = 2;
                Row["effectEndurance"].Value = Value(Row["effectEndurance"]) * 1.25;
                Row["motionInterval"].Value = 0;
                Row["cycleOccurrenceSpEffectId"].Value = Vfx1;
                Row["stateInfo"].Value = 0;
                Row["replaceSpEffectId"].Value = -1;
                Row["atkOccurrenceSpEffectId"].Value = -1;
                Row["wepParamChange"].Value = 4;
                foreach (var Vfx in VfxStore) {
                    int New = Copy(Params["SpEffectVfxParam"], Vfx.Value);
                    Params["SpEffectVfxParam"][New]["effectType"].Value = 2;
                    for (var i = 0; i <= 7; i++) { Params["SpEffectVfxParam"][New]["enchantStartDmyId_" + i].Value = Convert.ToInt32(Params["SpEffectVfxParam"][New]["enchantStartDmyId_" + i].Value) + 10000; }
                    Row[Vfx.Key].Value = -1;
                    Params["SpEffectParam"][Vfx2][Vfx.Key].Value = New;
                }
            });
            // Ending message
            Console.WriteLine("- Generated and edited " + Buffs.Count + " weapon buffs in " + GetTime(StartTime) + " seconds!");
        }
        
        public static void AshOfWar() {
            // Starting time
            double StartTime = GetTime();
            EMEVD.Event Event = CreateNewEvent(1029350000, [ new(2000, 2, (List<object>)[(byte)0])], 0, null, EMEVD.Event.RestBehaviorType.Restart);
            List<int> Occupied = [];
            int Count = 0;
            Params["EquipParamGem"].Rows.ForEach(Row => Occupied.Add(Convert.ToInt32(Row["swordArtsParamId"].Value)));

            Dictionary<int, int> WeaponGems = [];
            Params["EquipParamWeapon"].Rows.ForEach(Row => {
                if(IsWeapon(Row) && (Row.ID % 10000) == 0 && !Occupied.Contains(Convert.ToInt32(Row["swordArtsParamId"].Value)))
                    WeaponGems.TryAdd(Convert.ToInt32(Row["swordArtsParamId"].Value), Row.ID);
            });

            Params["SwordArtsParam"].Rows.ForEach(Row => {
                if (!Occupied.Contains(Row.ID) && Value(Row["swordArtsType"]) > 0 && Row.ID >= 10) {
                    Count++;
                    Random random = new();
                    PARAM.Row Gem = Params["EquipParamGem"][New(Params["EquipParamGem"], 9999999+Convert.ToInt32(Row["swordArtsType"].Value))];
                    Gem["rarity"].Value = 3;
                    Gem["iconId"].Value = WeaponGems.TryGetValue(Row.ID, out int ID) ? Value(Params["EquipParamWeapon"][ID]["iconId"]) : Value(Row["iconId"]);
                    Gem["swordArtsParamId"].Value = Row.ID;
                    Gem["sortGroupId"].Value = 225;
                    Gem["sellValue"].Value = 500 * (random.NextDouble() * 10);
                    Gem["isDiscard"].Value = 1;
                    Gem["isDrop"].Value = 1;
                    Gem["isDeposit"].Value = 1;
                    Gem["showDialogCondType"].Value = 2;
                    Gem["showLogCondType"].Value = 1;
                    int ItemLot = New(Params["ItemLotParam_map"]);
                    Params["ItemLotParam_map"][ItemLot]["lotItemId01"].Value = Gem.ID;
                    Params["ItemLotParam_map"][ItemLot]["lotItemCategory01"].Value = 5;
                    Params["ItemLotParam_map"][ItemLot]["lotItemNum01"].Value = 1;
                    Params["ItemLotParam_map"][ItemLot]["lotItemBasePoint01"].Value = 1000;
                    
                    Event.Instructions.AddRange([
                        new(3, 4, [(sbyte)-1, (sbyte)0, (int)(WeaponGems.TryGetValue(Row.ID, out int value) ? value : 110000), (sbyte)1]),
                        new(1000, 1, [(byte)3, (byte)1, (sbyte)-1]),
                        new(1003, 1, [(byte)2, (byte)1, (byte)0, (uint)(1029350000+Value(Row["swordArtsType"]))]),
                        new(2003, 66, [(byte)0, (uint)(1029350000+Value(Row["swordArtsType"])), (byte)1]),
                        new(2003, 4, (List<object>)[(int)ItemLot])]);
                }
            });
            Params["EquipParamGem"].Rows.ForEach(Row => {
                foreach (var Cell in Row.Cells)
                    if (Cell.ToString().Contains("canMountWep_") || Cell.ToString().Contains("configurableWepAttr"))
                        Cell.Value = 1;
                Row["mountWepTextId"].Value = 63907;
            });
            // Ending message
            Console.WriteLine("- Generated missing ashes of war for " + Count + " unique weapons in " + GetTime(StartTime) + " seconds!");
        }

        public static Dictionary<string, PARAM> InitializeParams(BND4 paramBnd) {
            var paramDefinitions = new List<PARAMDEF>();
            foreach (string path in Directory.GetFiles(Root + "/Definitions/Defs", "*.xml"))
            {
                var paramDefinition = PARAMDEF.XmlDeserialize(path);
                paramDefinitions.Add(paramDefinition);
            }
            
            var Params = new Dictionary<string, PARAM>();
            foreach (BinderFile file in paramBnd.Files)
            {
                string Name = Path.GetFileNameWithoutExtension(file.Name);
                var Param = PARAM.Read(file.Bytes);
                
                if (Param.ApplyParamdefCarefully(paramDefinitions))
                    Params[Name] = Param;
            }
            
            return Params;
        }
        
        static int Copy(PARAM Param, int ID, int NID = 9999999) {
            if (NID == 9999999)
                while (true)
                    if (Param[NID] != null)
                        NID--;
                    else break;
            if (Param[ID] == null)
                return -1;
            if (Param[NID] != null)
                Param.Rows.Remove(Param[NID]);
            Param.Rows.Add(new(Param[ID]) { ID = NID });
            return NID;
        }

        static int New(PARAM Param, int ID = 9999999) {
            if (ID == 9999999)
                while(true)
                    if (Param[ID] != null)
                        ID--;
                    else break;
            if (Param[0] != null && Param[0].Name == "HarmoniousRenewal") {
                Param.Rows.Add(new(Param[0]) { ID = ID });
            } else {
                Param.Rows.Add(new(ID, "", Param.AppliedParamdef));
            }
            return ID;
        }
        
        static double Change(PARAM.Cell V1, double V2) => Value(V1) == 0 || V2 == 0 ? 0 : Math.Abs((V2 - Value(V1)) / Value(V1));
        static double Difference(PARAM.Cell V1, double V2) => Value(V1) == 0 || V2 == 0 ? 0 : (Value(V1) - V2) / ((Value(V1) + V2) / 2);
        
        static bool IsWeapon(PARAM.Row Row) {
            if (Value(Row["wepType"]) > 0 && Value(Row["materialSetId"]) > -1 &&  Value(Row["reinforceShopCategory"]) > 0 && Value(Row["sortGroupId"]) < 255
                && WeaponName.TryGetValue(Row.ID, out FMG.Entry? value) && value.Text is not "%null%" or "[ERROR]" or null)
                return true;
            return false;
        }
        
        static void AddFMG(FMG fmg, Dictionary<int, FMG.Entry> dictionary, int ID, string Text) {
            if (dictionary.TryGetValue(ID, out FMG.Entry? value))
                fmg.Entries.Remove(value);
            fmg.Entries.Add(new(ID, Text));
            if (!dictionary.TryAdd(ID, fmg.Entries.Last()))
                dictionary[ID] = fmg.Entries.Last();
        }

        static double Value(PARAM.Cell Cell) => Convert.ToDouble(Cell.Value);
        
        static void GetGamePath() {
            gamepathnotfound:
            if (!File.Exists(GamePath) && GamePath.Contains("eldenring.exe"))
            {
                Console.WriteLine("\nPut the path to your eldenring.exe below.\n(Found in the steam files.)\nThe usual location is " + GamePath + "eldenring.exe\n");
                string? Input = Console.ReadLine();
                if (Input != null && File.Exists(Input.Replace("\"", "")) && Input.Contains("eldenring.exe"))
                    GamePath = Input.Replace("\"", "");
                else
                    goto gamepathnotfound;
            }
            GamePath = GamePath.Replace("eldenring.exe", "");
        }

        public static EMEVD.Event CreateNewEvent(
            int DestinationID,
            ICollection<EMEVD.Instruction> Instructions,
            int InitializeID = -1,
            EMEVD? Emevd = null,
            EMEVD.Event.RestBehaviorType Rest = EMEVD.Event.RestBehaviorType.Default)
        {
            EMEVD.Event Event = new(DestinationID, Rest);
            Event.Instructions.AddRange(Instructions);
            Emevd ??= Common;
            Emevd.Events.Add(Event);
            
            if (InitializeID != -1)
                Emevd.Events[InitializeID].Instructions.Add(new EMEVD.Instruction(2000, 0, new List<object> { 0, (uint)DestinationID, (uint)0 }));

            return Emevd.Events.Last();
        }

        static double GetTime(double Time = 0) {
            string[] add = DateTime.Now.ToString("HH:mm:ss.fff").Split(":");
            double totalTime =
                Convert.ToDouble(add[0]) * 60 * 60 +
                Convert.ToDouble(add[1]) * 60 +
                Convert.ToDouble(add[2]);
            if (Time != 0)
                return Math.Round(totalTime - Time, 3);
            else
                return totalTime;
        }
    }
}