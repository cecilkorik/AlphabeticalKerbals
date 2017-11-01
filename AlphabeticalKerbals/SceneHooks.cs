using System;
using KSP;
using KSP.UI;
using KSP.UI.Screens;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphabeticalKerbals
{

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AlphabetVAB : MonoBehaviour
    {
        public bool HasBeenSorted;
        public EditorScreen CurrentEditorScreen;
        private int sortAttempts;

        /// <summary>
        /// Module initialization
        /// </summary>
        public void Start()
        {
            print("AlphabeticalKerbals: AlphabetVAB started!");
        }

        /// <summary>
        /// Module startup
        /// </summary>
        public void Awake()
        {
            print("AlphabeticalKerbals: Registering VAB events");
            GameEvents.onEditorLoad.Add(OnShipLoaded);
            GameEvents.onEditorScreenChange.Add(OnEditorScreenChange);
        }
  

        /// <summary>
        /// Module shutdown
        /// </summary>
        public void OnDestroy()
        {
            print("AlphabeticalKerbals: Unregistering VAB events");
            GameEvents.onEditorLoad.Remove(OnShipLoaded);
            GameEvents.onEditorScreenChange.Remove(OnEditorScreenChange);
        }

    /// <summary>
    /// Here when a ship is loaded in the editor.
    /// </summary>
    /// <param name="construct"></param>
    /// <param name="loadType"></param>
    private void OnShipLoaded(ShipConstruct construct, CraftBrowserDialog.LoadType loadType)
        {
            try
            {
                CurrentEditorScreen = EditorScreen.Parts;
                HasBeenSorted = false;
                sortAttempts = 0;
            }
            catch (Exception e)
            {
                print("AlphabeticalKerbals: There was an error in OnShipLoaded");
            }
        }

        /// <summary>
        /// Here when Editor Panel is changed
        /// </summary>
        /// <param name="construct"></param>
        private void OnEditorScreenChange(EditorScreen screen)
        {
            try
            {
                CurrentEditorScreen = screen;
                HasBeenSorted = false;
                sortAttempts = 0;

                if (screen == EditorScreen.Crew)
                {
                    OnEditorCrewOpened();
                }
            }
            catch (Exception e)
            {
                print("AlphabeticalKerbals: There was an error in OnEditorScreenChange");
            }
        }

        /// <summary>
        /// Here when Editor Crew Panel is opened
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="part"></param>
        private void OnEditorCrewOpened()
        {
            try
            {
                if (KerbalSorter.Sort_Kerbals())
                {
                    print("AlphabeticalKerbals: CrewPanel Sorting successful!");
                    HasBeenSorted = true;
                }
            }
            catch (Exception e)
            {
                print("AlphabeticalKerbals: There was an error in OnEditorCrewOpened");
            }
        }

    }



    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class AlphabetSC : MonoBehaviour
    {
        public void Start()
        {
            print("AlphabeticalKerbals: AlphabetSC started!");
        }

    }
}
