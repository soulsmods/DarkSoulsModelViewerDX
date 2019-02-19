using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Windows.Forms;

namespace DarkSoulsModelViewerDX.DbgMenus
{
    public class DbgMenuItemSpawnObj : DbgMenuItem
    {
        public static List<int> IDList = new List<int>();
        private static bool NeedsTextUpdate = false;

        public int IDIndex = 0;

        public static void UpdateSpawnIDs()
        {
            try
            {
                string[] objFiles = null;

                if (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS1)
                {
                    objFiles = Directory.GetFiles(InterrootLoader.GetInterrootPath(@"\obj\"), @"*.objbnd")
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                }
                else if (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS2)
                {
                    objFiles = Directory.GetFiles(InterrootLoader.GetInterrootPath(@"\model\obj\"), @"*.bnd")
                        .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                        .Select(Path.GetFileNameWithoutExtension) //Remove .objbnd
                        .ToArray();
                }
                else if (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootNB)
                {
                    objFiles = Directory.GetFiles(InterrootLoader.GetInterrootPath(@"\obj\"), @"*.bnd")
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                }
                else
                {
                    objFiles = Directory.GetFiles(InterrootLoader.GetInterrootPath(@"\obj\"), @"*.objbnd.dcx")
                        .Select(Path.GetFileNameWithoutExtension) //Remove .dcx
                        .Select(Path.GetFileNameWithoutExtension) //Remove .objbnd
                        .ToArray();
                }

                IDList = new List<int>();
                var IDSet = new HashSet<int>();
                foreach (var cf in objFiles)
                {
                    if (int.TryParse((InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS3 || InterrootLoader.Type == InterrootLoader.InterrootType.InterrootBloodborne)
                        ? cf.Substring(1, 6) : (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS2) ? cf.Substring(1, 7).Replace("_", "") : cf.Substring(1, 4), out int id))
                    {
                        IDList.Add(id);
                        IDSet.Add(id);
                    }
                }

                NeedsTextUpdate = true;
            }
            catch (Exception e)
            {
                IDList = new List<int>();
                NeedsTextUpdate = true;
                MessageBox.Show("An error occured when populating the obj list: " + e.Message, e.StackTrace);
            }
        }

        public DbgMenuItemSpawnObj()
        {
            UpdateSpawnIDs();
            UpdateText();
        }

        private void UpdateText()
        {
            if (IDList.Count == 0)
            {
                IDIndex = 0;
                Text = $"Click to Spawn OBJ [Invalid Data Root Selected]";
            }
            else
            {
                if (IDIndex >= IDList.Count)
                    IDIndex = IDList.Count - 1;

                if (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS3)
                    Text = $"Click to Spawn OBJ [ID: <o{IDList[IDIndex]:D6}>]";
                else
                    Text = $"Click to Spawn OBJ [ID: <o{IDList[IDIndex]:D4}>]";
            }
        }

        public override void OnIncrease(bool isRepeat, int incrementAmount)
        {
            int prevIndex = IDIndex;
            IDIndex += incrementAmount;

            //If upper bound reached
            if (IDIndex >= IDList.Count)
            {
                //If already at end and just tapped button
                if (prevIndex == IDList.Count - 1 && !isRepeat)
                    IDIndex = 0; //Wrap Around
                else
                    IDIndex = IDList.Count - 1; //Stop
            }

            UpdateText();
        }

        public override void OnDecrease(bool isRepeat, int incrementAmount)
        {
            int prevIndex = IDIndex;
            IDIndex -= incrementAmount;

            //If upper bound reached
            if (IDIndex < 0)
            {
                //If already at end and just tapped button
                if (prevIndex == 0 && !isRepeat)
                    IDIndex = IDList.Count - 1; //Wrap Around
                else
                    IDIndex = 0; //Stop
            }

            UpdateText();
        }

        public override void OnResetDefault()
        {
            IDIndex = 0;
            UpdateText();
        }

        public override void OnClick()
        {
            if (IDList.Count == 0)
                return;

            GFX.ModelDrawer.AddObj(IDList[IDIndex],
                GFX.World.GetSpawnPointInFrontOfCamera(distance: 5,
                faceBackwards: false, lockPitch: true, alignToFloor: true));
        }

        public override void UpdateUI()
        {
            if (NeedsTextUpdate)
            {
                UpdateText();
                NeedsTextUpdate = false;
            }
        }

        public override void OnRequestTextRefresh()
        {
            UpdateSpawnIDs();
            UpdateText();
        }
    }
}
