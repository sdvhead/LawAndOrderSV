using ContentPatcher;
using StardewModdingAPI.Events;

namespace LawAndOrderSV
{
    internal class MurderMysteryFestival
    {
        internal static void Init()
        {
            ModEntry.imh.Events.GameLoop.GameLaunched += OnGameLaunched!;
        }

        private static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {

            var cpapi = ModEntry.imh.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (cpapi != null)
            {

                string[] strWeapons = { "knife", "knife", "knife", "bat", "bat", "bat", "bottle", "bottle", "bottle", "mirror", "mirror", "mirror", "wrench", "wrench", "wrench" };
                string[] strCandidates = { "Leah", "Harvey", "Abigail", "Alex", "Shannon", "Sam", "Shane", "Penny", "Russell", "Haley", "Elliott", "Emily", "Maru", "Sebastian", "Clint" };
                string[] strCandidateIDs = { "Leah", "Harvey", "Abigail", "Alex", "sdvhead.LawAndOrderSV_Shannon", "Sam", "Shane", "Penny", "sdvhead.LawAndOrderSV_Russell", "Haley", "Elliott", "Emily", "Maru", "Sebastian", "Clint" };
                string[] strGenders = { "woman", "man", "woman", "man", "woman", "man", "man", "woman", "man", "woman", "man", "woman", "woman", "man", "man" };
                string[] strLikesA = { "poppyseedmuffin", "pickles", "quartz", "egg", "maplebar", "maplebar", "egg", "poppyseedmuffin", "pickles", "cake", "notpizza", "quartz", "quartz", "quartz", "ruby" };
                string[] strLikesB = { "notpizza", "notpizza", "cake", "notquartz", "ruby", "pizza", "pizza", "emerald", "egg", "notquartz", "notquartz", "ruby", "notpickles", "notpickles", "emerald" };
                string[] strLikesDeceive = { "pizza", "pizza", "pickles", "quartz", "pickles", "quartz", "pickles", "quartz", "cake", "quartz", "pizza", "notquartz", "pickles", "pickles", "quartz" };
                string[] strKnows = { "Sam", "Maru", "Sam", "Haley", "Abigail", "Sebastian", "Emily", "Maru", "Willy", "Emily", "Willy", "Haley", "Sebastian", "Abigail", "Emily" };
                string[] strLiveWithParents = { "No", "No", "Yes", "No", "No", "Yes", "No", "Yes", "No", "No", "No", "No", "Yes", "Yes", "No" };

                string[] strWeaponSpots = { "Spa", "HatMouse", "Museum", "Sheriff", "Beach", "Swing", "Quarry" };
                string[] strWeaponMaps = { "BathHouse_Pool", "Forest", "ArchaeologyHouse", "Mountain", "Beach", "Forest", "Mountain" };
                string[] strWeaponMapXs = { "10", "38", "37", "2", "10", "35", "80" };
                string[] strWeaponMapYs = { "3", "95", "4", "39", "5", "6", "30" };
                string[] strWeaponMapFarmerXYs = { "9 3 1", "39 95 3", "36 4 1", "3 39 3", "11 5 3", "34 6 1", "81 30 3" };

                List<string> lstCandidates = strCandidates.ToList();
                List<string> lstCandidateIDs = strCandidateIDs.ToList();
                List<string> lstGenders = strGenders.ToList();
                Random r = new Random();
                int i = r.Next(lstCandidates.Count);
                string strKiller = lstCandidates[i];
                string strKillerID = lstCandidateIDs[i];
                string strKillerGender = lstGenders[i];
                string strKillerGenderDeceive = "man";
                if (strKillerGender == strKillerGenderDeceive)
                {
                    strKillerGenderDeceive = "woman";
                }


                string strWeapon = strWeapons[i];
                string strLikeA = strLikesA[i];
                string strLikeB = strLikesB[i];
                string strLikeDeceive = strLikesDeceive[i];
                string strKnow = strKnows[i];
                string strLiveWithParent = strLiveWithParents[i];
                string strLiveWithParentDeceive = "No";
                if (strLiveWithParent == strLiveWithParentDeceive)
                {
                    strLiveWithParentDeceive = "Yes";
                }

                lstCandidates.RemoveAt(i); //killer has now been removed from the list
                lstCandidateIDs.RemoveAt(i);


                //use a separate list for weapon hinters so that weapon hinters never give their own name
                Random wrand = new Random();
                List<string> weaponHinters = lstCandidateIDs.ToList();

                List<string> knifeHinters = weaponHinters.ToList();
                List<string> knifeUsers = new List<string> { "Leah", "Harvey", "Abigail" };
                knifeHinters.RemoveAll(x => knifeUsers.Contains(x));
                int w = wrand.Next(knifeHinters.Count);
                string knifeHinter = knifeHinters[w];

                //remove the hinter from the weaponHinters list and the candidates list
                int kh = lstCandidateIDs.IndexOf(knifeHinter);
                lstCandidates.RemoveAt(kh);
                lstCandidateIDs.RemoveAt(kh);
                weaponHinters.Remove(knifeHinter);

                List<string> batHinters = weaponHinters.ToList();
                List<string> batUsers = new List<string> { "Alex", "sdvhead.LawAndOrderSV_Shannon", "Sam" };
                batHinters.RemoveAll(x => batUsers.Contains(x));
                w = wrand.Next(batHinters.Count);
                string batHinter = batHinters[w];
                kh = lstCandidateIDs.IndexOf(batHinter);
                lstCandidates.RemoveAt(kh);
                lstCandidateIDs.RemoveAt(kh);
                weaponHinters.Remove(batHinter);

                List<string> bottleHinters = weaponHinters.ToList();
                List<string> bottleUsers = new List<string> { "Shane", "Penny", "sdvhead.LawAndOrderSV_Russell" };
                bottleHinters.RemoveAll(x => bottleUsers.Contains(x));
                w = wrand.Next(bottleHinters.Count);
                string bottleHinter = bottleHinters[w];
                kh = lstCandidateIDs.IndexOf(bottleHinter);
                lstCandidates.RemoveAt(kh);
                lstCandidateIDs.RemoveAt(kh);
                weaponHinters.Remove(bottleHinter);

                List<string> mirrorHinters = weaponHinters.ToList();
                List<string> mirrorUsers = new List<string> { "Hailey", "Elliott", "Emily" };
                mirrorHinters.RemoveAll(x => mirrorUsers.Contains(x));
                w = wrand.Next(mirrorHinters.Count);
                string mirrorHinter = mirrorHinters[w];
                kh = lstCandidateIDs.IndexOf(mirrorHinter);
                lstCandidates.RemoveAt(kh);
                lstCandidateIDs.RemoveAt(kh);
                weaponHinters.Remove(mirrorHinter);

                List<string> wrenchHinters = weaponHinters.ToList();
                List<string> wrenchUsers = new List<string> { "Maru", "Sebastian", "Clint" };
                wrenchHinters.RemoveAll(x => wrenchUsers.Contains(x));
                w = wrand.Next(wrenchHinters.Count);
                string wrenchHinter = wrenchHinters[w];
                kh = lstCandidateIDs.IndexOf(wrenchHinter);
                lstCandidates.RemoveAt(kh);
                lstCandidateIDs.RemoveAt(kh);
                weaponHinters.Remove(wrenchHinter);


                //start identifying other hinters
                i = r.Next(lstCandidates.Count);
                string strHinterLikeA = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterLikeB = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterLikeC = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterLikeD = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterKnow = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterWeaponSpotA = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterWeaponSpotB = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterGender = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);
                i = r.Next(lstCandidates.Count);
                string strHinterLiveWithParent = lstCandidateIDs[i];

                lstCandidates.RemoveAt(i);
                lstCandidateIDs.RemoveAt(i);

                int ws = r.Next(strWeaponSpots.Length);

                string strWeaponSpot = strWeaponSpots[ws];
                //string strWeaponSpotFriendly = strWeaponSpotsFriendly[ws];
                string strWeaponMap = strWeaponMaps[ws];
                string strWeaponMapX = strWeaponMapXs[ws];
                string strWeaponMapY = strWeaponMapYs[ws];
                string strWeaponMapFarmerXY = strWeaponMapFarmerXYs[ws];

                /// <summary>Register a simple token.</summary>
                /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> in your entry class).</param>
                /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
                /// <param name="getValue">A function which returns the current token value. If this returns a null or empty list, the token is considered unavailable in the current context and any patches or dynamic tokens using it are disabled.</param>
                //void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>?> getValue);


                cpapi.RegisterToken(ModEntry.manifest, "mmfWeapon", () => { return new[] { strWeapon }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfKiller", () => { return new[] { strKiller }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfKillerID", () => { return new[] { strKillerID }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfGender", () => { return new[] { strKillerGender }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfLikeA", () => { return new[] { strLikeA }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfLikeB", () => { return new[] { strLikeB }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfLikeDeceive", () => { return new[] { strLikeDeceive }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfGenderDeceive", () => { return new[] { strKillerGenderDeceive }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfParentDeceive", () => { return new[] { strLiveWithParentDeceive }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfKnow", () => { return new[] { strKnow }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfLiveWithParent", () => { return new[] { strLiveWithParent }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterLikeA", () => { return new[] { strHinterLikeA }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterLikeB", () => { return new[] { strHinterLikeB }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterLikeC", () => { return new[] { strHinterLikeC }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterLikeD", () => { return new[] { strHinterLikeD }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterKnow", () => { return new[] { strHinterKnow }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterGender", () => { return new[] { strHinterGender }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterLiveWithParent", () => { return new[] { strHinterLiveWithParent }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterWeaponSpotA", () => { return new[] { strHinterWeaponSpotA }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterWeaponSpotB", () => { return new[] { strHinterWeaponSpotB }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterKnifeUsers", () => { return new[] { knifeHinter }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterBatUsers", () => { return new[] { batHinter }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterBottleUsers", () => { return new[] { bottleHinter }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterMirrorUsers", () => { return new[] { mirrorHinter }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfHinterWrenchUsers", () => { return new[] { wrenchHinter }; });

                cpapi.RegisterToken(ModEntry.manifest, "mmfWeaponSpot", () => { return new[] { strWeaponSpot }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfWeaponMap", () => { return new[] { strWeaponMap }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfWeaponMapX", () => { return new[] { strWeaponMapX }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfWeaponMapY", () => { return new[] { strWeaponMapY }; });
                cpapi.RegisterToken(ModEntry.manifest, "mmfWeaponMapFarmerXY", () => { return new[] { strWeaponMapFarmerXY }; });

            }
        }

    }
}
