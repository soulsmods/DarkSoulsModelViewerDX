using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Windows.Forms;

namespace DarkSoulsModelViewerDX.DbgMenus
{
    public class DbgMenuItemSpawnMap : DbgMenuItem
    {
        public static List<string> IDList = new List<string>();
        private static bool NeedsTextUpdate = false;

        public int IDIndex = 0;

        public enum SpawnerType
        {
            SpawnModel,
            SpawnRegion,
            SpawnCollision,
        };

        public SpawnerType SpawnType;

        public static void UpdateSpawnIDs()
        {
            try
            {
                var path = (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS2 || InterrootLoader.Type == InterrootLoader.InterrootType.InterrootNB) ? @"\map\" : @"\map\MapStudio\";
                var search = (InterrootLoader.Type == InterrootLoader.InterrootType.InterrootDS2 || InterrootLoader.Type == InterrootLoader.InterrootType.InterrootNB) ? @"m*" : @"*.msb";
                var msbFiles = Directory.GetFileSystemEntries(InterrootLoader.GetInterrootPath(path), search)
                    .Select(Path.GetFileNameWithoutExtension);
                IDList = new List<string>();
                var IDSet = new HashSet<string>();
                foreach (var cf in msbFiles)
                {
                    var dotIndex = cf.IndexOf('.');
                    if (dotIndex >= 0)
                    {
                        IDList.Add(cf.Substring(0, dotIndex));
                        IDSet.Add(cf.Substring(0, dotIndex));
                    }
                    else
                    {
                        IDList.Add(cf);
                        IDSet.Add(cf);
                    }
                }

                var msbFilesDCX = Directory.GetFileSystemEntries(InterrootLoader.GetInterrootPath(path), search + ".dcx")
                    .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension);
                foreach (var cf in msbFilesDCX)
                {
                    var dotIndex = cf.IndexOf('.');
                    if (dotIndex >= 0)
                    {
                        if (!IDSet.Contains(cf.Substring(0, dotIndex)))
                            IDList.Add(cf.Substring(0, dotIndex));
                    }
                    else
                    {
                        if (!IDSet.Contains(cf))
                            IDList.Add(cf);
                    }
                }
                NeedsTextUpdate = true;
            }
            catch (Exception e)
            {
                IDList = new List<string>();
                NeedsTextUpdate = true;
                MessageBox.Show("An error occured when populating the map list: " + e.Message, e.StackTrace);
            }
        }

        public DbgMenuItemSpawnMap(SpawnerType spawnerType)
        {
            SpawnType = spawnerType;
            UpdateSpawnIDs();
            UpdateText();
        }

        private void UpdateText()
        {
            string actionText = "";
            if (SpawnType == SpawnerType.SpawnModel)
            {
                actionText = "Click to Spawn MAP - Models";
            }
            else if (SpawnType == SpawnerType.SpawnRegion)
            {
                actionText = "Click to Spawn MAP - Event Regions";
            }
            else if (SpawnType == SpawnerType.SpawnCollision)
            {
                actionText = "Click to Spawn MAP - Collision Meshes";
            }

            if (SpawnType == SpawnerType.SpawnModel)
            {
                CustomColorFunction = () => (
                    LoadingTaskMan.IsTaskRunning($"{nameof(InterrootLoader.LoadMapInBackground)}_Textures[{IDList[IDIndex]}]")
                    || LoadingTaskMan.IsTaskRunning($"{nameof(InterrootLoader.LoadMapInBackground)}_Models[{IDList[IDIndex]}]"))
                    ? Color.Cyan * 0.5f : Color.Cyan;
            }

            if (IDList.Count == 0)
            {
                IDIndex = 0;
                Text = $"{actionText} [Invalid Data Root Selected]";
            }
            else
            {
                if (IDIndex >= IDList.Count)
                    IDIndex = IDList.Count - 1;

                Text = $"{actionText} [ID: <{IDList[IDIndex]}>]";
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
            if (SpawnType == SpawnerType.SpawnRegion)
                InterrootLoader.LoadMsbRegions(IDList[IDIndex]);
            else if (SpawnType == SpawnerType.SpawnModel)
                GFX.ModelDrawer.AddMap(IDList[IDIndex], false);
            else if (SpawnType == SpawnerType.SpawnCollision)
                GFX.ModelDrawer.AddMapCollision(IDList[IDIndex], false);
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
