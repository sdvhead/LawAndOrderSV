using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawAndOrderSV.SecretLab
{
    public class Utils
    {

        public static double GetDistance(int x1, int y1, int x2, int y2)
            //return the distance between two x/y pairs
            //used by map object managers (DoorFactory, PowerBoxFactory) to find the nearest object from the triggering point
        {
            return Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        }

    }
}
