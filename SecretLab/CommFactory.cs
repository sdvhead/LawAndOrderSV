using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewValley.TokenizableStrings;
using static HarmonyLib.Code;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace LawAndOrderSV.SecretLab
{
    public static class CommFactory
    {

        internal static List<Comm> Comms = new List<Comm>();

        public static bool initialPowerMessage = false;
        public static bool jiaIntroduced = false;
        public static List<Message> messageQueue = new();
        public static Dictionary<string, Message> messages = new();
        public const int messagetype_message = 0; //message with no portrait
        public const int messagetype_dialogue = 1; //message with portrait
        public const int messagetype_question = 2; //question with answer options

        public const int speakerTile_Jia = 0;
        public const int speakerTile_Panick = 1;

        //public static List<Comm> CommList = new List<Comm>();


        public struct Message
        {
            public int? SpeakerTile { get; set; }
            public int? MessageType { get; set; }
            public string? NextMessage { get; set; }
            public int? RequiredCommID { get; set; }
            public string? Content { get; set; }
            public Response[]? Responses { get; set; }
            public string? QuestionKey { get; set; }
           

            public Message(
                int? speakerTile = speakerTile_Jia,
                int? messageType = messagetype_message,
                string? nextMessage = null,
                int? requiredCommID = null,
                string? content = null,
                Response[]? responses = null,
                string? questionKey = null
                )
            {
                SpeakerTile = speakerTile;
                MessageType = messageType;
                NextMessage = nextMessage;
                RequiredCommID = requiredCommID;
                Content = content;
                Responses = responses;
                QuestionKey = questionKey;
            }
        }


        //public static string batteryQuestionKey = ModEntry.ModId + "_SecretLab_BatteryQuestionKey";
        //public static string deliverPowerQuestionKey = ModEntry.ModId + "_SecretLab_DeliverPowerQuestionKey";

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_Comms_Interact", Comms_Interact);
            InitMessages();

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(CommFactory),
                                          nameof(CommFactory.AnswerQuestion))
               );

            AddComms();

        }

        private static void AddComms()
        {
            Comms.Add(new Comm(70,4, "sdvhead.LawAndOrderSV_SecretLab_Cove"));
            Comms.Add(new Comm(65, 13, "sdvhead.LawAndOrderSV_SecretLab_Warehouse"));
        }

        internal static void InitMessages()
        {
            // message format:
            //    int messageType,
            //    string? nextMessage = null,
            //    int? requiredPoint = null,
            //    string? content = null, - the dialogue content or message question
            //    Response[]? responses = null,
            //    string? questionKey = null

            messages.Add("InitialPower", new Message(speakerTile_Jia, messagetype_message, "JiaIntroduce", null, I18n.SecretLabCommsInitialPower()));
            messages.Add("JiaIntroduce", new Message(speakerTile_Jia, messagetype_dialogue, "JiaIntroQuestion", null, I18n.SecretLabCommsJiaIntroduce()));


            string[] JiaIntroduce_Responses = { "SecretLab_Comms_JiaIntroduce_Answer1", "SecretLab_Comms_JiaIntroduce_Answer2", "SecretLab_Comms_JiaIntroduce_Answer3" };
            Response[] responses = CreateCommsResponses("JiaIntroQuestion", JiaIntroduce_Responses);
            messages.Add("JiaIntroQuestion", new Message(speakerTile_Jia, messagetype_question, null, null, I18n.SecretLabCommsJiaIntroduceQuestion(), responses, "sdvhead.LawAndOrderSV_JiaIntroQuestion"));

            string[] Codeword_Responses = { "SecretLab_Comms_Codeword_Answer1", "SecretLab_Comms_Codeword_Answer2", "SecretLab_Comms_Codeword_Answer3", "SecretLab_Comms_Codeword_Answer4" };
            messages.Add("CodewordQuestion", new Message(speakerTile_Jia, messagetype_question, null, null, I18n.SecretLabCommsCodewordQuestion(), CreateCommsResponses("CodewordQuestion", Codeword_Responses), "sdvhead.LawAndOrderSV_CodewordQuestion"));

            messages.Add("JiaInstructionsA", new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsJiaInstructionsA()));
            //messages.Add("JiaInstructionsB", new Message(messagetype_dialogue, "JiaInstructionsC", null, I18n.SecretLabCommsJiaInstructionsB()));
            //messages.Add("JiaInstructionsC", new Message(messagetype_dialogue, "JiaInstructionsD", null, I18n.SecretLabCommsJiaInstructionsC()));
            //messages.Add("JiaInstructionsD", new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaInstructionsD()));

            messages.Add("Panicked1", new Message(speakerTile_Panick, messagetype_message, null, 2, I18n.SecretLabCommsPanickedA()));

            //messages aren't available until they are queued (such as being added here or being set as the 'nextMessage' for one being read)
            messageQueue.Add(messages["InitialPower"]);
            messageQueue.Add(messages["Panicked1"]);

        }

        internal static List<Comm> GetComms(string mapName)
        {
            List<Comm> pbs = new();
            for (int i = 0; i < Comms.Count; i++)
            {
                var pb = Comms[i];
                if (pb.MapName == mapName)
                {
                    pbs.Add(pb);
                }
            }
            return pbs;
        }

        public static Response[] CreateCommsResponses(string questionKey, string[] i18nAnswerKeys)
        {
            ModEntry.Log("createCommsResponse: " + questionKey + ": " + i18nAnswerKeys[0]);
            List<Response> responses = new();

            for (int i = 0; i < i18nAnswerKeys.Length; i++)
            {
                string answer = I18n.GetByKey(i18nAnswerKeys[i]);
                responses.Add(new Response("a" + (i + 1), answer));
            }

            return responses.ToArray();
        }


        private static bool Comms_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            ModEntry.Log("finding closest comm...");
            Comm? comm = FindClosestComm(point, location);

            if (comm != null)
            {
                return comm.Interact(location, args, farmer, point);
            }

            return true;

        }

        internal static Comm? FindClosestComm(Point point, GameLocation location)
        {
            int x = (int)point.X;
            int y = (int)point.Y;

            List<Comm> localComms = new();
            for (var i = 0; i < Comms.Count; i++)
            {
                if (Comms[i].Location() == location)
                {
                    localComms.Add(Comms[i]);
                }
            }

            if (localComms.Count < 1)
            {
                return null;
            }

            var closestComm = localComms[0];

            if (localComms.Count > 1)
            {
                double closestDistance = Utils.GetDistance(x, y, closestComm.X, closestComm.Y);

                for (var i = 1; i < localComms.Count; i++)
                {
                    var comm = localComms[i];
                    double distance = Utils.GetDistance(x, y, comm.X, comm.Y);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestComm = comm;
                    }
                }
            }
            return closestComm;
        }

        public static bool AnswerQuestion(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {

            var comm = FindClosestComm(Game1.player.TilePoint, Game1.currentLocation);
            if(comm != null)
            {
                comm.AnswerQuestion(__instance, questionAndAnswer, questionParams);
            }
            return true;

        }

    }
}
