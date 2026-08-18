using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Point = Microsoft.Xna.Framework.Point;

namespace LawAndOrderSV.SecretLab
{
    public class Comms
    {
        internal static bool initialPowerMessage = false;
        internal static bool jiaIntroduced = false;
        public static List<Message> messageQueue = new List<Message>();
        public static Dictionary<string, Message> messages = new();
        const int messagetype_message = 0; //message with no portrait
        const int messagetype_dialogue = 1; //message with portrait
        const int messagetype_question = 2; //question with answer options

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_Comms_Interact", Comms_Interact);
            InitMessages();
        }

        internal static void InitMessages()
        {
            messages.Add("InitialPower", new Message(messagetype_message, "JiaIntroduce", new Vector2(0, 0), I18n.SecretLabCommsInitialPower()));
            messages.Add("JiaIntroduce", new Message(messagetype_message, null, new Vector2(0, 0), I18n.SecretLabCommsJiaIntroduce()));


            messageQueue.Add(messages["InitialPower"]);
             
        }

        public struct Message
        {
            public int MessageType { get; set; }
            public string? NextMessage { get; set; }
            public Vector2? RequiredPoint { get; set; }
            public string? Content { get; set; }
            public string? Question { get; set; }
            public List<string>? Answers { get; set; }

            public Message(
                int messageType,
                string? nextMessage = null,
                Vector2? requiredPoint = null,
                string? content = null,
                string? question = null,
                List<string>? answers = null)
            {
                MessageType = messageType;
                NextMessage = nextMessage;
                RequiredPoint = requiredPoint;
                Content = content;
                Question = question;
                Answers = answers;
            }
        }

        public static void Comms_ProcessQueue(GameLocation location, string[] args, Farmer farmer, Point point)
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
                        //Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduceRespondFarmer());
                        Comms_ProcessQueue(location, args, farmer, point);
                    });
                }
            }
        }

        public static bool Comms_Interact(GameLocation location, string[] args, Farmer farmer, Point point)
        {
            
            if ((PowerStation.totalPowerPercent > 0))
            {
                Comms_ProcessQueue(location, args, farmer, point);
                /*
                bool sentMessage = false;
                if (!initialPowerMessage)
                {
                    Game1.drawObjectDialogue(I18n.SecretLabCommsInitialPower());
                    sentMessage = true;
                    initialPowerMessage = true;
                }
                */



                /*
                if (!jiaIntroduced)
                {
                    if(sentMessage)
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduce());
                        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                        {
                            Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduceRespondFarmer());
                        });
                    }
                    else
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduce());
                    }
                    jiaIntroduced = true;

                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduceRespondFarmer());
                    });


                    
                    //if (question_and_answer == "DesertScholar_Yes")
                    //{
                    //    _currentScholarQuestion++;
                    //    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Intro2"));
                    //    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    //    {
                    //        generateNextScholarQuestion();
                    //    });
                    //}
                    
                }
                */
            }
            else
            {
                Game1.drawObjectDialogue(I18n.SecretLabCommsNoPower());

            }

            return true;
        }

    }
}
