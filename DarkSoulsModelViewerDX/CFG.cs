using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkSoulsModelViewerDX
{
    public class CFG
    {
        private static object _lock_SaveLoadCFG = new object();

        public const string FileName = "DarkSoulsModelViewerDX_UserConfig.json";
        private static CFG Current = null;

        private static bool NoSaveWarn = false;

        public static void Load()
        {
            lock (_lock_SaveLoadCFG)
            {
                Current = Newtonsoft.Json.JsonConvert.DeserializeObject<CFG>(
                File.ReadAllText(FileName));

                InterrootLoader.Interroot = Current.InterrootLoader_Interroot;
                InterrootLoader.Type = Current.InterrootLoader_Type;

                GFX.LODMode = Current.GFX_LODMode;
                GFX.LOD1Distance = Current.GFX_LOD1Distance;
                GFX.LOD2Distance = Current.GFX_LOD2Distance;
                GFX.EnableFrustumCulling = Current.GFX_EnableFrustumCulling;
                GFX.EnableTextures = Current.GFX_EnableTextures;
                GFX.Wireframe = Current.GFX_Wireframe;
                GFX.EnableLighting = Current.GFX_EnableLighting;

                DBG.ShowModelNames = Current.DBG_ShowModelNames;
                DBG.ShowModelBoundingBoxes = Current.DBG_ShowModelBoundingBoxes;
                DBG.ShowModelSubmeshBoundingBoxes = Current.DBG_ShowModelSubmeshBoundingBoxes;
                DBG.ShowPrimitiveNametags = Current.DBG_ShowPrimitiveNametags;
                DBG.ShowGrid = Current.DBG_ShowGrid;
                DBG.ShowFancyTextLabels = Current.DBG_ShowFancyTextLabels;

                GFX.ModelDrawer.GoToModelsAsTheySpawn = Current.GFX_ModelDrawer_GoToModelsAsTheySpawn;

                GFX.World.CameraMoveSpeed = Current.GFX_World_CameraMoveSpeed;
                GFX.World.CameraTurnSpeedGamepad = Current.GFX_World_CameraTurnSpeedGamepad;
                GFX.World.CameraTurnSpeedMouse = Current.GFX_World_CameraTurnSpeedMouse;
                GFX.World.FieldOfView = Current.GFX_World_FieldOfView;
                GFX.World.NearClipDistance = Current.GFX_World_NearClipDistance;
                GFX.World.FarClipDistance = Current.GFX_World_FarClipDistance;

                GFX.Display.Width = Current.GFX_Display_Width;
                GFX.Display.Height = Current.GFX_Display_Height;
                GFX.Display.Format = Current.GFX_Display_Format;
                GFX.Display.Vsync = Current.GFX_Display_Vsync;
                GFX.Display.Fullscreen = Current.GFX_Display_Fullscreen;
                GFX.Display.SimpleMSAA = Current.GFX_Display_SimpleMSAA;
            }
        }

        public static void Save()
        {
            try
            {
                lock (_lock_SaveLoadCFG)
                {
                    Current.InterrootLoader_Interroot = InterrootLoader.Interroot;
                    Current.InterrootLoader_Type = InterrootLoader.Type;

                    Current.GFX_LODMode = GFX.LODMode;
                    Current.GFX_LOD1Distance = GFX.LOD1Distance;
                    Current.GFX_LOD2Distance = GFX.LOD2Distance;
                    Current.GFX_EnableFrustumCulling = GFX.EnableFrustumCulling;
                    Current.GFX_EnableTextures = GFX.EnableTextures;
                    Current.GFX_Wireframe = GFX.Wireframe;
                    Current.GFX_EnableLighting = GFX.EnableLighting;

                    Current.DBG_ShowModelNames = DBG.ShowModelNames;
                    Current.DBG_ShowModelBoundingBoxes = DBG.ShowModelBoundingBoxes;
                    Current.DBG_ShowModelSubmeshBoundingBoxes = DBG.ShowModelSubmeshBoundingBoxes;
                    Current.DBG_ShowPrimitiveNametags = DBG.ShowPrimitiveNametags;
                    Current.DBG_ShowGrid = DBG.ShowGrid;
                    Current.DBG_ShowFancyTextLabels = DBG.ShowFancyTextLabels;

                    Current.GFX_ModelDrawer_GoToModelsAsTheySpawn = GFX.ModelDrawer.GoToModelsAsTheySpawn;

                    Current.GFX_World_CameraMoveSpeed = GFX.World.CameraMoveSpeed;
                    Current.GFX_World_CameraTurnSpeedGamepad = GFX.World.CameraTurnSpeedGamepad;
                    Current.GFX_World_CameraTurnSpeedMouse = GFX.World.CameraTurnSpeedMouse;
                    Current.GFX_World_FieldOfView = GFX.World.FieldOfView;
                    Current.GFX_World_NearClipDistance = GFX.World.NearClipDistance;
                    Current.GFX_World_FarClipDistance = GFX.World.FarClipDistance;

                    Current.GFX_Display_Width = GFX.Display.Width;
                    Current.GFX_Display_Height = GFX.Display.Height;
                    Current.GFX_Display_Format = GFX.Display.Format;
                    Current.GFX_Display_Vsync = GFX.Display.Vsync;
                    Current.GFX_Display_Fullscreen = GFX.Display.Fullscreen;
                    Current.GFX_Display_SimpleMSAA = GFX.Display.SimpleMSAA;

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                        Current, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(FileName, json);
                }
            }
            catch (Exception e)
            {
                if (!NoSaveWarn)
                {
                    NoSaveWarn = true;
                    MessageBox.Show("Error: Failed to save configuration file. Your permissions may be incorrect. Your settings may not be saved upon exiting.");
                }
            }
        }

        public static void Init()
        {
            if (File.Exists(FileName))
            {
                Load();
            }
            else
            {
                Current = new CFG();
                Save();
            }
        }

        public string InterrootLoader_Interroot { get; set; } 
            = @"C:\Program Files (x86)\steam\steamapps\common\Dark Souls Prepare to Die Edition\DATA";

        public InterrootLoader.InterrootType InterrootLoader_Type { get; set; }
            = InterrootLoader.InterrootType.InterrootDS1;

        public LODMode GFX_LODMode { get; set; } = LODMode.Automatic;
        public float GFX_LOD1Distance { get; set; } = 200.0f;
        public float GFX_LOD2Distance { get; set; } = 400.0f;
        public bool GFX_EnableTextures { get; set; } = true;
        public bool GFX_Wireframe { get; set; } = false;
        public bool GFX_EnableFrustumCulling { get; set; } = false;
        public bool GFX_EnableLighting { get; set; } = true;

        public bool DBG_ShowModelNames { get; set; } = false;
        public bool DBG_ShowModelBoundingBoxes { get; set; } = false;
        public bool DBG_ShowModelSubmeshBoundingBoxes { get; set; } = false;
        public bool DBG_ShowGrid { get; set; } = true;
        public bool DBG_ShowPrimitiveNametags { get; set; } = false;
        public bool DBG_ShowFancyTextLabels { get; set; } = false;

        public bool GFX_ModelDrawer_GoToModelsAsTheySpawn { get; set; } = true;

        public float GFX_World_FieldOfView { get; set; } = 43.0f;
        public float GFX_World_CameraTurnSpeedGamepad { get; set; } = 1.5f;
        public float GFX_World_CameraTurnSpeedMouse { get; set; } = 1.5f;
        public float GFX_World_CameraMoveSpeed { get; set; } = 10.0f;
        public float GFX_World_NearClipDistance { get; set; } = 0.1f;
        public float GFX_World_FarClipDistance { get; set; } = 10000.0f;

        public int GFX_Display_Width { get; set; } = 1600;
        public int GFX_Display_Height { get; set; } = 1600;
        public SurfaceFormat GFX_Display_Format { get; set; } = SurfaceFormat.Color;
        public bool GFX_Display_Vsync { get; set; } = true;
        public bool GFX_Display_Fullscreen { get; set; } = false;
        public bool GFX_Display_SimpleMSAA { get; set; } = true;
    }
}
