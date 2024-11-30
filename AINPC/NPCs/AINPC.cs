using Steamworks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.Utilities;
using Terraria.GameInput;
using System.Diagnostics;

namespace AINPC.Content.NPCs.TownNPCs
{

    [AutoloadHead]
    public class Antithesis : ModNPC
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string apiResponse = "Missing";
        private string apiIntent = "Missing";
        public static string apiPI = "Missing";
        private string Message = "Hello Traveler";
        private string htmlStatus = "offline";
        private static bool isOverlayOpen = false;
        private static string phase = "missing";

        public static int debug = 0;



        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 2;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Velocity = 1f,
                Direction = -1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPC.Happiness
                    .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                    .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                    .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
            ;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Wizard; //Try animation style of the Wizard//
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;

            if (Main.GameUpdateCount % 60 == 0) // Check time every second (60 ticks per second)
            {
                _ = CheckTimeOfDay(); // This will call the method to check the time of day
            }

            if (Main.GameUpdateCount % 60 == 0) // Every 10 seconds (60 ticks per second)
            {
                _ = SendLoctoAPI(player);
            }

            if (Main.GameUpdateCount % 120 == 0) // Every 10 seconds (60 ticks per second)
            {
                _ = CheckHtmlStatus();
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                BossStatusDisplay(); //removed _ =!!!
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                _ = Bossdeader();
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                _ = GetPI();
            }

        }

        public void BossStatusDisplay()
        { // test boss defeat mechanic
            if (debug == 1)
            {
                Main.NewText(BossDefeated());
            }

        }



        public string BossDefeated()
        {

            if (NPC.downedBoss3 == true)
            {
                if (Main.hardMode == true)
                {
                    if (NPC.downedMechBoss1 == true && NPC.downedMechBoss2 == true && NPC.downedMechBoss3 == true)
                    {
                        if (NPC.downedPlantBoss)
                        {
                            if (NPC.downedGolemBoss == true)
                            {
                                if (NPC.downedAncientCultist == true)
                                {
                                    if (NPC.downedMoonlord == true)
                                    {

                                        phase = "END";

                                    }
                                    else
                                    {
                                        phase = "Hardmode-PreMoonLord";
                                    }
                                }
                                else
                                {
                                    phase = "Hardmode-PreLunaCultists";
                                }
                            }
                            else
                            {
                                phase = "Hardmode-Pregolem";
                            }
                        }
                        else
                        {
                            phase = "Hardmode-PrePlantera";
                        }
                    }
                    else
                    {
                        phase = "Hardmode-PreMech";
                    }
                }
                else
                {
                    phase = "pre-wof";
                }
            }
            else
            {
                phase = "pre-bosses";
            }

            return phase;


        }

        public async Task CheckTimeOfDay()
        {
            try
            {
                bool dayTime = Main.dayTime;  // Check if it's day or night

                // Prepare request body for API
                var requestBody = new
                {
                    dayTime = dayTime
                };

                // Serialize the request body to JSON
                string jsonString = JsonSerializer.Serialize(requestBody);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Send the POST request to the FastAPI server
                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/check_time", content);
                response.EnsureSuccessStatusCode();

                // Read the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the response JSON
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                string timeOfDay = jsonResponse["time_of_day"];

                // Display the result in-game
                if (debug == 1)
                {
                    Main.NewText($"Current time of day: {timeOfDay}", Microsoft.Xna.Framework.Color.Orange);
                }
                
            }
            catch (HttpRequestException e)
            {
                if (debug == 1)
                {
                    Main.NewText("Error checking time of day.", Microsoft.Xna.Framework.Color.Red);
                }
                
            }
        }


        public async Task SendTexttoAPI(string submitted_text)
        {
            try
            {

                var requestBody = new
                {
                    question = submitted_text
                };

                string jsonString = JsonSerializer.Serialize(requestBody);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/generate_text", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                apiResponse = jsonResponse["answer"];
                apiIntent = jsonResponse["intent"];
                apiPI = jsonResponse["PI"];
                if (debug == 1)
                {
                    Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
                }

            }

            catch (HttpRequestException e)
            {
                apiResponse = "Error fetching dialogue.";
                if (debug == 1)
                {
                    Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
                }

            }
        }


        public async Task Bossdeader()
        {
            try
            {
                var requestBody = new
                {
                    bossStatus = BossDefeated()
                };

                string JsonString = JsonSerializer.Serialize(requestBody);
                StringContent content = new StringContent(JsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/Bossdead", content);//add "" in transfor
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                //Parse the JSON response
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                phase = jsonResponse["bossStatus"];
                if (debug == 1)
                {
                    Main.NewText("Phase updated:" + phase, Microsoft.Xna.Framework.Color.Orange);
                }


            }
            catch (HttpRequestException e)
            {
                apiResponse = "Error fetching phase";
                if (debug == 1)
                {
                    Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
                }


            }
        }

        public async Task GetPI()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("http://localhost:8000/get_PI");
                response.EnsureSuccessStatusCode();

                // Read the response as a string (assuming PI is returned as a simple value)
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

                // Assign the PI value to a variable
                apiPI = jsonResponse["PI"];

                // Display PI value in the game or log

                if (debug == 1)
                {
                    Main.NewText("PI: " + apiPI, Microsoft.Xna.Framework.Color.Orange);
                }


            }
            catch (HttpRequestException e)
            {
                apiPI = "Error fetching PI.";
                if (debug == 1)
                {
                    Main.NewText(apiPI, Microsoft.Xna.Framework.Color.Orange);
                }

            }
        }

        public async Task CheckHtmlStatus()
        {
            try
            {
                var requestBody = new
                {
                    status = "check"
                };

                string jsonString = JsonSerializer.Serialize(requestBody);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/page_opened", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                htmlStatus = jsonResponse["html_status"];
                string message = jsonResponse["message"];
                string htmlActive = jsonResponse["html_active"];
                if (debug == 1)
                {
                    Main.NewText("htmlStatus: " + htmlStatus + ", message: " + message + ", htmlActive: " + htmlActive, Microsoft.Xna.Framework.Color.Orange);
                }

            }
            catch (Exception ex)
            {
                apiResponse = "Error fetching dialogue.";
                if (debug == 1)
                {
                    Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
                }

            }
        }

        public async Task SendLoctoAPI(Player player) //Send location to API
        {
            try
            {
                float distanceToTarget = NPC.Center.Distance(player.Center);

                var requestBody = new
                {
                    message = "test",
                    npcName = "Antithesis",
                    location = new
                    {
                        x = NPC.position.X,
                        y = NPC.position.Y
                    },
                    distanceToTarget
                };

                string jsonString = JsonSerializer.Serialize(requestBody);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/npc_location", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                if (debug == 1)
                {
                    Main.NewText($"Location Sent: {responseBody}", Microsoft.Xna.Framework.Color.Green);
                }

            }

            catch (HttpRequestException e)
            {
                if (debug == 1)
                {
                    Main.NewText("Error Sending Location.", Microsoft.Xna.Framework.Color.Red);
                }

            }
        }

        public override string GetChat()
        {
            return Message;
        }


        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return true;
        }


        //Shop 

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "CHAT";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = ShopName;
            }
            else
            {
                string url = @"http://localhost:8000/";

                if (htmlStatus == "online")
                {
                    SteamFriends.ActivateGameOverlay("Friends");
                    isOverlayOpen = true;
                }
                else if (htmlStatus == "offline")
                {
                    SteamFriends.ActivateGameOverlayToWebPage(url);
                    isOverlayOpen = true;
                }
            }
        }


        public const string ShopName = "Shop";

        public override void AddShops()
        {

            var npcShop = new NPCShop(Type, ShopName);

            npcShop.Add(ItemID.WandofSparking);
            npcShop.Add(ItemID.WandofFrosting);
            npcShop.Add(ItemID.LifeCrystal);
            npcShop.Add(ItemID.ManaCrystal);
            npcShop.Add(ItemID.WizardHat);
            npcShop.Add(ItemID.WizardsHat);



            npcShop.Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            apiPI = Antithesis.apiPI;

            foreach (Item item in items)
            {
                if (item is null)
                {
                    continue;
                }

                if (apiPI == "positive")
                {
                    item.shopCustomPrice = (int?)(item.GetStoreValue() * 0.5f);
                }
                else if (apiPI == "negative")
                {
                    item.shopCustomPrice = (int?)(item.GetStoreValue() * 2.0f);
                }
                else if (apiPI == "neutral")
                {
                    item.shopCustomPrice = (int?)(item.GetStoreValue() * 1.0f);
                }
                else if (apiPI == "Missing")
                {
                    item.shopCustomPrice = (int?)(item.GetStoreValue() * 0.1f);
                }
                else
                {
                    item.shopCustomPrice = (int?)(item.GetStoreValue() * 100f);
                }
            }
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            apiPI = Antithesis.apiPI;

            if (apiPI == "positive")
            {
                projType = ProjectileID.Electrosphere;
            }
            else if (apiPI == "negative")
            {
                projType = ProjectileID.TNTBarrel;
            }
            else if (apiPI == "neutral")
            {
                projType = ProjectileID.MagicMissile;
            }
            else if (apiPI == "Missing")
            {
                projType = ProjectileID.MagicMissile;
            }
            else
            {
                projType = ProjectileID.Electrosphere;
            }
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 16f;
            randomOffset = 5f;

        }

        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 1f;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]{

                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("Mods.TownNPCGuide.NPCs.Antithesis.Bestiary")

            });
        }

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void Load()
        {

            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }


        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        //add gore

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }

        public override bool UsesPartyHat()
        {
            return true;
        }


    }

    public class AttDebugCommand : ModCommand
    {
        // Command type is 'World' since it can be run in both Single Player and Multiplayer.
        public override CommandType Type => CommandType.World;

        // Command to trigger: "/att_debug"
        public override string Command => "att_debug";

        // The usage of the command (although it's very simple in this case)
        public override string Usage => "/att_debug - Sets the global 'debug' flag to 1.";

        // Description of the command
        public override string Description => "Sets the global 'debug' flag to 1 for debugging purposes.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Antithesis.debug == 0)
            {
                Antithesis.debug = 1;

                caller.Reply("Debug mode has been activated (debug = 1).");
            }
            else
            {
                Antithesis.debug = 0;

                caller.Reply("Debug mode has been deactivated (debug = 0).");
            }
        }
    }


}
