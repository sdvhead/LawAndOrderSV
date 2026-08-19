using HarmonyLib;
using StardewValley;
using StardewValley.TokenizableStrings;
using static HarmonyLib.Code;
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
        public static bool receivingMessage = false;
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

            string[] Codeword_Responses = { "SecretLab_Comms_Codeword_Answer1", "SecretLab_Comms_Codeword_Answer2", "SecretLab_Comms_Codeword_Answer3", "SecretLab_Comms_Codeword_Answer4" };
            messages.Add("CodewordQuestion", new Message(messagetype_question, null, null, I18n.SecretLabCommsCodewordQuestion(), CreateCommsResponses("CodewordQuestion", Codeword_Responses), "sdvhead.LawAndOrderSV_CodewordQuestion"));

            messages.Add("JiaInstructionsA", new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaInstructionsA()));
            //messages.Add("JiaInstructionsB", new Message(messagetype_dialogue, "JiaInstructionsC", null, I18n.SecretLabCommsJiaInstructionsB()));
            //messages.Add("JiaInstructionsC", new Message(messagetype_dialogue, "JiaInstructionsD", null, I18n.SecretLabCommsJiaInstructionsC()));
            //messages.Add("JiaInstructionsD", new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaInstructionsD()));

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
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => {ProcessMessageQueue();}, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a2")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse2()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a3")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse3()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a1")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse1()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a2")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse2()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a3")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse3()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a4")
            {
                messages.Add(questionAndAnswer, new Message(messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse4()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
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

                receivingMessage = true;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();

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
                    NPC nPC = Game1.getCharacterFromName("sdvhead.LawAndOrderSV_Jia");
                    nPC = new NPC(nPC.Sprite, Vector2.Zero, "", 0, nPC.Name, nPC.Portrait, eventActor: false);
                    //nPC.displayName = nPC.displayName;


                    string dialogueText = TokenParser.ParseText(msg.Content);
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
            else
            {
                receivingMessage = false;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
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
