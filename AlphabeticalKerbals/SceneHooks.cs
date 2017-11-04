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

    public class AlphabetStatic
    {
        public static readonly TimeSpan update_interval = new TimeSpan(0, 0, 0, 0, 750);
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AlphabetVAB : MonoBehaviour
    {
        public bool HasBeenSorted;
        public EditorScreen CurrentEditorScreen;
        private int sortAttempts;
        private DateTime last_update;


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
                if (CurrentEditorScreen != EditorScreen.Crew && screen == EditorScreen.Crew)
                {
                    last_update = DateTime.Now;
                    HasBeenSorted = false;
                    sortAttempts = 0;
                    OnEditorCrewOpened();
                }
            }
            catch (Exception e)
            {
                print("AlphabeticalKerbals: There was an error in OnEditorScreenChange");
            }
            CurrentEditorScreen = screen;
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

        public void Update()
        {
            if (CurrentEditorScreen == EditorScreen.Crew)
            {
                if (last_update + AlphabetStatic.update_interval < DateTime.Now)
                {
                    // polling 3-4 times per second for crew panel updates
                    OnCrewPanelTick();
                    last_update = DateTime.Now;
                }

            }
        }

        public void OnCrewPanelTick()
        {
            KerbalSorter.Sort_Kerbals_If_Needed();
        }


    }



    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class AlphabetSC : MonoBehaviour
    {
        public bool DialogUp = false;

        private DateTime last_update;

        public void Start()
        {
            print("AlphabeticalKerbals: AlphabetSC started!");
        }

        public void Update()
        {
            if (!DialogUp)
            {
                if (VesselSpawnDialog.Instance != null && VesselSpawnDialog.Instance.Visible)
                {
                    if (CrewAssignmentDialog.Instance != null && CrewAssignmentDialog.Instance.isActiveAndEnabled)
                    {
                        OnLaunchDialog();
                        DialogUp = true;
                        last_update = DateTime.Now;
                    }
                }

            }
            else if (DialogUp)
            {
                if (VesselSpawnDialog.Instance == null || !VesselSpawnDialog.Instance.Visible)
                {
                    if (CrewAssignmentDialog.Instance == null || !CrewAssignmentDialog.Instance.isActiveAndEnabled)
                    {
                        OnLaunchDialogClose();
                        DialogUp = false;
                    }

                }
                else if (last_update + AlphabetStatic.update_interval < DateTime.Now)
                {
                    // polling 3-4 times per second for crew panel updates
                    OnLaunchDialogTick();
                    last_update = DateTime.Now;
                }

            }
        }

        public void OnLaunchDialog()
        {
            print("Alphabetical Kerbals: Vessel Spawn Dialog detected.");

            KerbalSorter.Sort_Kerbals();

        }

        public void OnLaunchDialogClose()
        {
        }

        public void OnLaunchDialogTick()
        {
            KerbalSorter.Sort_Kerbals_If_Needed();
        }


    }
}
