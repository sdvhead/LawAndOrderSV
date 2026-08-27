using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewValley.TokenizableStrings;
using static LawAndOrderSV.SecretLab.CommFactory;

namespace LawAndOrderSV.SecretLab
{
    internal class Comm
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int ID { get; set; }

        public string MapName { get; set; }

        public bool ReceivingMessage { get; set; }

        public int? SpeakerTile {  get; set; }

        public Comm(int x, int y, string MapName)
        {
            //x and y are the coordinates for the upper left corner of the 4-tile door
            this.X = x;
            this.Y = y;
            this.MapName = MapName;
            this.ID = CommFactory.Comms.Count + 1;
            this.SpeakerTile = null;
        }

        public GameLocation Location()
        {
            return Game1.getLocationFromName(this.MapName);
        }

        public bool Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            ModEntry.Log("comms interact:" + this.ID);
            if (PowerBoxFactory.powered_comms)
            {
                ProcessMessageQueue(location, args, farmer, point);
            }
            else
            {
                Game1.drawObjectDialogue(I18n.SecretLabCommsNoPower());

            }

            return true;
        }

        public void ProcessMessageQueue()
        {
            ProcessMessageQueue(null, null, null, null);
        }

        public void ProcessMessageQueue(GameLocation? location, string[]? args, Farmer? farmer, Point? point)
        {
            bool processedOne = false;
            for (var i = 0; i < messageQueue.Count; i++)
            {
                var msg = messageQueue[i];
                if(msg.RequiredCommID==null || msg.RequiredCommID == this.ID)
                {
                    processedOne = true;
                    messageQueue.RemoveAt(i);
                    ProcessMessage(msg,location,args,farmer,point);
                    
                    break;
                }
            }
            if (!processedOne)
            {
                ReceivingMessage = false;
                this.SpeakerTile = null;
                ResetMaps();
            }
        }

        public void ProcessMessage(Message msg, GameLocation? location, string[]? args, Farmer? farmer, Point? point)
        {
            ReceivingMessage = true;
            if(msg.SpeakerTile!=null) this.SpeakerTile = msg.SpeakerTile;
            ResetMaps();


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

        public void AnswerQuestion(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            ModEntry.Log("commFactory interact answer: " + questionAndAnswer);

            if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a1")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse1()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => {ProcessMessageQueue();}, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a2")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse2()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_JiaIntroQuestion_a3")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsJiaIntroduceResponse3()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["CodewordQuestion"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a1")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse1()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a2")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse2()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a3")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse3()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
            else if (questionAndAnswer == "sdvhead.LawAndOrderSV_CodewordQuestion_a4")
            {
                messages.Add(questionAndAnswer, new Message(speakerTile_Jia, messagetype_dialogue, null, null, I18n.SecretLabCommsCodewordResponse4()));
                messageQueue.Add(messages[questionAndAnswer]);
                messageQueue.Add(messages["JiaInstructionsA"]);
                ProcessMessageQueue();
                //DelayedAction.functionAfterDelay(() => { ProcessMessageQueue(); }, 10);
            }
        }

    }

}
