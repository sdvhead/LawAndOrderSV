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

namespace LawAndOrderSV.SecretLab
{
    internal class Elevator
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int ID { get; set; }
        public string MapName { get; set; }

        public Elevator(int x, int y, string mapName)
        {
            //x and y are the coordinates for the upper left corner of power box
            //this.ID = id;
            this.X = x;
            this.Y = y;
            this.MapName = mapName;
            //this.BatteryQuestionKey = ModEntry.ModId + "_SecretLab_BatteryQuestionKey_"+id;
        }


        public bool Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (PowerBoxFactory.powered_elevator)
            {
                location.createQuestionDialogue(I18n.SecretLab_Elevator_Question(),ElevatorChoices(),ElevatorFactory.elevatorQuestionKey);
            }
            else
            {
                Game1.drawObjectDialogue(I18n.SecretLab_Elevator_NoPower());
                //if (PowerBoxFactory.unusedPowerPercent > 0)
                //{
                //    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                //}
                //else if (PowerBoxFactory.totalPowerPercent >= 100) //all stations online
                //{
                //    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionMax(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                //}
                //else //all available power being used
                //{
                //    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionFull(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                //}
            }

            return true;
        }

        public Response[] ElevatorChoices()
        {
            List<Response> responses = new List<Response>();

            Response responseCove = new Response("Cove", I18n.SecretLab_Elevator_AnswerCove());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Cove") responses.Add(responseCove);

            Response responseWarehouse = new Response("Warehouse", I18n.SecretLab_Elevator_AnswerWarehouse());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Warehouse") responses.Add(responseWarehouse);

            Response responseOperations = new Response("Operations", I18n.SecretLab_Elevator_AnswerOperations());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Operations") responses.Add(responseOperations);

            Response responseTesting = new Response("Testing", I18n.SecretLab_Elevator_AnswerTesting());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Testing") responses.Add(responseTesting);

            Response responseResearch = new Response("Research", I18n.SecretLab_Elevator_AnswerResearch());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Research") responses.Add(responseResearch);

            Response responseRooftop = new Response("Rooftop", I18n.SecretLab_Elevator_AnswerRooftop());
            if (this.MapName != "sdvhead.LawAndOrderSV_SecretLab_Rooftop") responses.Add(responseRooftop);


            //Response r1 = !powered_lights ? new Response("lightsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOn()) : new Response("lightsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOff());
            //Response r2 = !powered_elevator ? new Response("elevatorOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOn()) : new Response("elevatorOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOff());
            //Response r3 = !powered_doorsbelts ? new Response("doorsBeltsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOn()) : new Response("doorsBeltsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOff());
            //Response r4 = !powered_outlets ? new Response("outletsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOn()) : new Response("outletsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOff());
            //Response r5 = !powered_comms ? new Response("commsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOn()) : new Response("commsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOff());

            //if (powered_lights || unusedPowerPercent > 0) responses.Add(r1);
            //if (powered_elevator || unusedPowerPercent > 0) responses.Add(r2);
            //if (powered_doorsbelts || unusedPowerPercent > 0) responses.Add(r3);
            //if (powered_outlets || unusedPowerPercent > 0) responses.Add(r4);
            //if (powered_comms || unusedPowerPercent > 0) responses.Add(r5);
            //responses.Add(new Response("nothing", I18n.SecretLab_PowerStation_DeliverPowerAnswerNothing()));

            return responses.ToArray();
        }

        public GameLocation location()
        {
            return Game1.getLocationFromName(this.MapName);
        }




    }

}
