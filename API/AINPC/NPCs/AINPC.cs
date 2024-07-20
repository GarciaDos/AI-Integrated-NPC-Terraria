using Steamworks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
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
        private string apiResponse = "Hello Traveler.";
        private static bool isOverlayOpen = false;



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
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0) {
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
            NPC.DeathSound =SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Wizard; //Try animation style of the Wizard//
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
                Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
            }

            catch (HttpRequestException e)
            {
                apiResponse = "Error fetching dialogue.";
                Main.NewText(apiResponse, Microsoft.Xna.Framework.Color.Orange);
        }
        }

        public override string GetChat()
        {

             

                
            return apiResponse;
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
                SteamFriends.ActivateGameOverlayToWebPage(url);
                isOverlayOpen = true;
            }
        }


        public const string ShopName = "Shop";

        public override void AddShops(){
            var npcShop = new NPCShop(Type, ShopName);

            npcShop.Add(ItemID.WandofSparking);
            npcShop.Add(new Item(ItemID.DirtBlock) { shopCustomPrice = Item.buyPrice(silver: 10)});

            npcShop.Register();
        }
    
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
            projType = ProjectileID.MagicMissile;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 16f;
        randomOffset = 5f;

        }

        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 1f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]{

                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("Mods.TownNPCGuide.NPCs.Antithesis.Bestiary")

            });
        }

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void Load(){

            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }


        public override ITownNPCProfile TownNPCProfile() {
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

 

}
