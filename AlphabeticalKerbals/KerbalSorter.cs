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
        static public bool Sort_Kerbals()
        {
            if (CrewAssignmentDialog.Instance == null)
            {
                print("OnEditorCrewOpened has no CrewAssignmentDialog yet...");
            }
            else
            {
                UIList avail = CrewAssignmentDialog.Instance.scrollListAvail;

                UIList_QSort(avail, 0, avail.Count-1);

                for (int i = 0; i < avail.Count; i++)
                {
                    UIListItem li = avail.GetUilistItemAt(i);
                    CrewListItem crew = li.GetComponent<CrewListItem>();
                    print("AFTER SORT = " + crew.GetName());
                }

                return true;
                
                /*
                print("AlphabeticalKerbals: OnEditorCrewOpened has CrewAssignmentDialog yay!!!");
                UIList avail = CrewAssignmentDialog.Instance.scrollListAvail;
                UIListItem first = avail.GetUilistItemAt(0);

                print("AlphabeticalKerbals: Got first item in UIList");

                if (first == null)
                {
                    //happens on first load
                    print("AlphabeticalKerbals: Uhhh.... first is null?");
                }
                if (first.gameObject == null)
                {
                    print("AlphabeticalKerbals: Uhhh.... gameObject is null?");
                }
                Component[] comps = first.gameObject.GetComponents<Component>();
                print("AlphabeticalKerbals: Uhhh.... got components???");
                for (int i = 0; i < comps.Length; ++i)
                {
                    print($"Component {i}: {comps[i].GetType()}");
                }

                CrewListItem firstdata = first.GetComponent<CrewListItem>();
                print("AlphabeticalKerbals: Got a component for crew list item");
                print("AlphabeticalKerbals: Got crew name: " + firstdata.GetName());

                print("AlphabeticalKerbals: Got component list");
                foreach (Component comp in firstdata)
                {
                    print("AlphabeticalKerbals: Got a component");
                    print("AlphabeticalKerbals: Component name is " + comp.name);
                    print("AlphabeticalKerbals: Component type is " + comp.GetType().Name);
                    print("AlphabeticalKerbals: Component string is " + comp.ToString());
                }
                */

            }


            return false;
        }

        /// <summary>
        /// Return value positive means left > right, return value negative means left < right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static int UIList_Cmp(UIListItem left, UIListItem right)
        {
            if (left == null) { return 1; } // null values are considered greatest, so they are sorted at the end
            if (right == null) { return -1; } // null values are considered greatest, so they are sorted at the end

            CrewListItem ldata = null;
            CrewListItem rdata = null;
            try { ldata = left.GetComponent<CrewListItem>(); } catch { return 1; }
            try { rdata = right.GetComponent<CrewListItem>(); } catch { return -1; }

            if (ldata == null) { return 1; }
            if (rdata == null) { return -1; }

            return ldata.GetName().CompareTo(rdata.GetName());
        }

        static int UIList_Partition(UIList list, int li, int ri)
        {
            // Select a random pivot point
            int pi = UnityEngine.Random.Range(li+1, ri+1);
            UIListItem pivot = list.GetUilistItemAt(pi);
            print("Selected pivot is " + pivot.GetComponent<CrewListItem>().GetName() + " at pos " + pi.ToString());

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
                    print("Pivotname is " + pivot.GetComponent<CrewListItem>().GetName() + " selname is " + selected.GetComponent<CrewListItem>().GetName());
                    UIList_DebugPrint(list, i, j, "Partition-swapping (" + i.ToString() + "," + j.ToString() + "," + UIList_Cmp(selected, pivot) + ")");
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
                print("Before partition from " + li.ToString() + "-" + ri.ToString());
                UIList_DebugPrint(list, li, ri, "Initial");

                int pi = UIList_Partition(list, li, ri);

                print ("After partition to pivot " + pi.ToString());
                UIList_DebugPrint(list, li, pi-1, "Left");
                UIList_DebugPrint(list, pi, pi, "Pivot");
                UIList_DebugPrint(list, pi+1, ri, "Right");

                UIList_QSort(list, li, pi - 1);
                UIList_QSort(list, pi + 1, ri);
            }


        }

        
        static void UIList_DebugPrint(UIList list, int li, int ri, string msg)
        {
            string outmsg = msg + " -- ";

            for (int i = li; i < ri+1; i++)
            {
                UIListItem curitem = list.GetUilistItemAt(i);
                CrewListItem crew = curitem.GetComponent<CrewListItem>();
                outmsg = outmsg + crew.GetName().Split(' ')[0] + "[" + i.ToString() + "], ";
            }

            print(outmsg);
        }

       
    }
}
