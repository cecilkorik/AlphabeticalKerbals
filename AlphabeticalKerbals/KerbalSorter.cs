using System;
using KSP;
using KSP.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphabeticalKerbals
{
   class KerbalSorter
    {
        static void print(string s) { MonoBehaviour.print("AlphabeticalKerbals: " + s); }

        /// <summary>
        /// Sorts the kerbals in the CrewAssignmentDialog
        /// Will not work on dialogs that do not use the CrewAssignmentDialog GUI element
        /// </summary>
        /// <returns>success</returns>
        static public bool Sort_Kerbals()
        {
            if (CrewAssignmentDialog.Instance == null)
            {
                print("OnEditorCrewOpened has no CrewAssignmentDialog yet...");
            }
            else
            {
                UIList avail = CrewAssignmentDialog.Instance.scrollListAvail;

                UIList_QSort(avail, 0, avail.Count - 1);

#if DEBUG
                for (int i = 0; i < avail.Count; i++)
                {
                    UIListItem li = avail.GetUilistItemAt(i);
                    CrewListItem crew = li.GetComponent<CrewListItem>();
                    print("AFTER SORT = " + crew.GetName());
                }
#endif

                return true;

                /*
                for (int i = 0; i < comps.Length; ++i)
                {
                    print($"Component {i}: {comps[i].GetType()}");
                }

                foreach (Component comp in firstdata)
                {
                    print("Got a component");
                    print("Component name is " + comp.name);
                    print("Component type is " + comp.GetType().Name);
                    print("Component string is " + comp.ToString());
                }
                */

            }


            return false;
        }

        /// <summary>
        /// Sorts Kerbals, but first checks the list to make sure it's not already sorted.
        /// Overall, this ends up using more resources than just blind-sorting, but in cases 
        /// where the list is almost always going to be correctly sorted, it may save some time.
        /// </summary>
        /// <returns>success</returns>
        static public bool Sort_Kerbals_If_Needed()
        {
            if (CrewAssignmentDialog.Instance == null)
            {
                print("OnEditorCrewOpened has no CrewAssignmentDialog yet...");
                return false;
            }
            else
            {
                UIList avail = CrewAssignmentDialog.Instance.scrollListAvail;
                bool unsorted = false;

                int i = 0;
                for (; i < (avail.Count - 1); i++)
                {
                    UIListItem li = avail.GetUilistItemAt(i);
                    UIListItem li2 = avail.GetUilistItemAt(i + 1);
                    CrewListItem crew = li.GetComponent<CrewListItem>();
                    if (UIList_Cmp(li, li2) > 0)
                    {
                        unsorted = true;

                        break;
                    }

                }

                if (unsorted)
                {
                    print("Change in Kerbal List detected, re-sorting");
#if DEBUG
                    print("Sort_If_Needed: Sort IS needed due to i = " + i.ToString());
                    for (i = 0; i < avail.Count; i++)
                    {
                        UIListItem li = avail.GetUilistItemAt(i);
                        CrewListItem crew = li.GetComponent<CrewListItem>();
                        print("BEFORE SORT = " + crew.GetName());
                    }
#endif
                    Sort_Kerbals();
                }
                return true;
            }
        }

        /// <summary>
        /// Return value positive means left > right, return value negative means left < right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static int UIList_Cmp(UIListItem left, UIListItem right)
        {
            CrewListItem ldata = null;
            CrewListItem rdata = null;
            try { ldata = left.GetComponent<CrewListItem>(); } catch { return 1; }
            try { rdata = right.GetComponent<CrewListItem>(); } catch { return -1; }

#if DEBUG
            if (ldata == null) { return 1; }
            if (rdata == null) { return -1; }
#endif

            bool ltourist = ldata.GetCrewRef().type == ProtoCrewMember.KerbalType.Tourist;
            bool rtourist = rdata.GetCrewRef().type == ProtoCrewMember.KerbalType.Tourist;

            if (ltourist && !rtourist) { return 1; } // tourists are also sorted to the end
            if (!ltourist && rtourist) { return -1; } // tourists are also sorted to the end

            return ldata.GetName().CompareTo(rdata.GetName());
        }

        static int UIList_Partition(UIList list, int li, int ri)
        {
            // Select a random pivot point
            int pi = UnityEngine.Random.Range(li+1, ri+1);
            UIListItem pivot = list.GetUilistItemAt(pi);
#if DEBUG
            print("Selected pivot is " + pivot.GetComponent<CrewListItem>().GetName() + " at pos " + pi.ToString());
#endif

            if (pi < ri)
            {
                // Move the pivot to the very end
                list.SwapItems(pivot, list.GetUilistItemAt(ri));
                pi = ri;
            }
            UIList_DebugPrint(list, li, ri, "Pivot-selection");

            // Iterate over the list and sort above or below pivot as we go
            int i = li;
            UIListItem prev = list.GetUilistItemAt(i);

            for (int j = li; j < ri; j++)
            {
                UIListItem selected = list.GetUilistItemAt(j);
                if (UIList_Cmp(selected, pivot) < 1)
                {
#if DEBUG
                    print("Pivotname is " + pivot.GetComponent<CrewListItem>().GetName() + " selname is " + selected.GetComponent<CrewListItem>().GetName());
                    UIList_DebugPrint(list, i, j, "Partition-swapping (" + i.ToString() + "," + j.ToString() + "," + UIList_Cmp(selected, pivot) + ")");
#endif
                    if (i < j)
                    {
                        list.SwapItems(prev, selected);
                    }
                    UIList_DebugPrint(list, li, ri, "Afterswap i=" + i.ToString());
                    i++;
                    prev = list.GetUilistItemAt(i);
                }
            }


            UIListItem pivot_target = list.GetUilistItemAt(i);
            if (UIList_Cmp(pivot_target, pivot) >= 0)
            {
                UIList_DebugPrint(list, li, ri, "Finalswap i=" + i.ToString());
                list.SwapItems(list.GetUilistItemAt(i), pivot);
                UIList_DebugPrint(list, li, ri, "Finalswap done i=" + i.ToString());
                return i;
            }
            else
            {
                return ri;
            }
            
        }
        static void UIList_QSort(UIList list, int li, int ri)
        {

            if (list == null || list.Count <= 1)
                return;

            if (li < ri)
            {
#if DEBUG
                print("Before partition from " + li.ToString() + "-" + ri.ToString());
                UIList_DebugPrint(list, li, ri, "Initial");
#endif

                int pi = UIList_Partition(list, li, ri);

#if DEBUG
                print("After partition to pivot " + pi.ToString());
                UIList_DebugPrint(list, li, pi-1, "Left");
                UIList_DebugPrint(list, pi, pi, "Pivot");
                UIList_DebugPrint(list, pi+1, ri, "Right");
#endif

                UIList_QSort(list, li, pi - 1);
                UIList_QSort(list, pi + 1, ri);
            }


        }

        
        static void UIList_DebugPrint(UIList list, int li, int ri, string msg)
        {
#if DEBUG
            string outmsg = msg + " -- ";

            for (int i = li; i < ri+1; i++)
            {
                UIListItem curitem = list.GetUilistItemAt(i);
                CrewListItem crew = curitem.GetComponent<CrewListItem>();
                outmsg = outmsg + crew.GetName().Split(' ')[0] + "[" + i.ToString() + "], ";
            }

            print(outmsg);
#endif
        }

       
    }
}
