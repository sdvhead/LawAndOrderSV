using HarmonyLib;
using StardewValley;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace LawAndOrderSV.SecretLab
{
    public class Comms
    {
        internal static bool initialPowerMessage = false;
        internal static bool jiaIntroduced = false;
        public static List<Message> messageQueue = new();
        public static Dictionary<string, Message> messages = new();
        const int messagetype_message = 0; //message with no portrait
        const int messagetype_dialogue = 1; //message with portrait
        const int messagetype_question = 2; //question with answer options

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_Comms_Interact", Comms_Interact);
            InitMessages();

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(Comms),
                                          nameof(Comms.AnswerQuestion))
               );
        }

        internal static void InitMessages()
        {
            // message format:
            //    int messageType,
            //    string? nextMessage = null,
            //    Vector2? requiredPoint = null,
            //    string? content = null, - the dialogue content or message question
            //    Response[]? responses = null,
            //    string? questionKey = null

            messages.Add("InitialPower", new Message(messagetype_message, "JiaIntroduce", null, I18n.SecretLabCommsInitialPower()));
            messages.Add("JiaIntroduce", new Message(messagetype_dialogue, "JiaIntroQuestion", null, I18n.SecretLabCommsJiaIntroduce()));

            string[] JiaIntroduce_Responses = { "SecretLab_Comms_JiaIntroduce_Answer1", "SecretLab_Comms_JiaIntroduce_Answer2", "SecretLab_Comms_JiaIntroduce_Answer3" };
            Response[] responses = CreateCommsResponses("JiaIntroQuestion", JiaIntroduce_Responses);
            messages.Add("JiaIntroQuestion", new Message(messagetype_question, null, null, I18n.SecretLabCommsJiaIntroduceQuestion(), responses, "sdvhead.LawAndOrderSV_JiaIntroQuestion"));
            messageQueue.Add(messages["InitialPower"]);
             
        }

        public struct Message
        {
            public int MessageType { get; set; }
            public string? NextMessage { get; set; }
            public Vector2? RequiredPoint { get; set; }
            public string? Content { get; set; }
            public Response[]? Responses { get; set; }
            public string? QuestionKey { get; set;  }

            public Message(
                int messageType,
                string? nextMessage = null,
                Vector2? requiredPoint = null,
                string? content = null,
                Response[]? responses = null,
                string? questionKey = null
                )
            {
                MessageType = messageType;
                NextMessage = nextMessage;
                RequiredPoint = requiredPoint;
                Content = content;
                Responses = responses;
                QuestionKey = questionKey;
            }
        }

        public static bool AnswerQuestion(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            ModEntry.Log("comms interact answer: " + questionAndAnswer);

            if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a1")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse1()));
                messageQueue.Add(messages[questionAndAnswer]);
                DelayedAction.functionAfterDelay(() => {ProcessMessageQueue();}, 100);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a2")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse2()));
                messageQueue.Add(messages[questionAndAnswer]);
                DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 100);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a3")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse3()));
                messageQueue.Add(messages[questionAndAnswer]);
                DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 100);
            }
            return true;
        }

        public static Response[] CreateCommsResponses(string questionKey, string[] i18nAnswerKeys)
        {
            ModEntry.Log("createCommsResponse: " + questionKey + ": " + i18nAnswerKeys[0]);
            List<Response> responses = new();

            for (int i = 0; i < i18nAnswerKeys.Length; i++)
            {
                string answer = I18n.GetByKey(i18nAnswerKeys[i]);
                responses.Add(new Response("a" + (i+1), answer));
            }

            return responses.ToArray();
        }

        public static void ProcessMessageQueue()
        {
            ProcessMessageQueue(null, null, null, null);
        }
        public static void ProcessMessageQueue(GameLocation? location, string[]? args,Farmer? farmer,Point? point)
        {
            if (messageQueue.Count > 0)
            {
                var msg = messageQueue[0];
                messageQueue.RemoveAt(0);

                if (msg.NextMessage != null && messages.ContainsKey(msg.NextMessage))
                {
                    messageQueue.Add(messages[msg.NextMessage]);
                }

                if (msg.MessageType == messagetype_message)
                {
                    Game1.drawObjectDialogue(msg.Content);
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        ProcessMessageQueue(location, args, farmer, point);
                    });
                }
                else if (msg.MessageType == messagetype_dialogue)
                {
                    NPC nPC = Game1.getCharacterFromName("sdvhead.LawAndOrderSV_Shannon");
                    nPC = new NPC(nPC.Sprite, Vector2.Zero, "", 0, nPC.Name, nPC.Portrait, eventActor: false);
                    //nPC.displayName = nPC.displayName;

                    Game1.DrawDialogue(new Dialogue(nPC, null, msg.Content));
                    

                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        ProcessMessageQueue(location, args, farmer, point);
                    });

                }
                else if (msg.MessageType == messagetype_question)
                {
                    ModEntry.Log("Creating question- key: " + msg.QuestionKey);
                    Game1.currentLocation.createQuestionDialogue(msg.Content, msg.Responses, msg.QuestionKey);

                }
            }
        }

        public static bool Comms_Interact(GameLocation location, string[] args, Farmer farmer, Point point)
        {
            
            if (PowerStation.powered_comms)
            {
                ProcessMessageQueue(location, args, farmer, point);
            }
            else
            {
                Game1.drawObjectDialogue(I18n.SecretLabCommsNoPower());

            }

            return true;
        }

    }
}
