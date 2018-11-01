﻿using DarkSoulsModelViewerDX.DbgMenus;
using DarkSoulsModelViewerDX.DebugPrimitives;
using DarkSoulsModelViewerDX.GFXShaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsModelViewerDX
{
    public enum GFXDrawStep : byte
    {
        _1_Opaque = 1,
        _2_DbgPrim = 2,
        _3_AlphaEdge = 3,
        _4_GUI  = 4,
    }

    public static class GFX
    {
        public static GFXDrawStep CurrentStep = GFXDrawStep._1_Opaque;

        public static readonly GFXDrawStep[] DRAW_STEP_LIST;
        static GFX()
        {
            DRAW_STEP_LIST = (GFXDrawStep[])Enum.GetValues(typeof(GFXDrawStep));
        }

        public const int LOD_MAX = 2;
        public static int ForceLOD = -1;
        public static float LOD1Distance = 200;
        public static float LOD2Distance = 400;

        // These are leftovers from when they would actually SWITCH during an expirement.
        // Too lazy to refactor lol. I'll keep them in case I decide that I need to swap
        // out shaders in the future.
        public static Effect CurrentFlverRenderEffect => FlverShader;
        public static Effect CurrentDbgPrimRenderEffect => DbgPrimShader;
        public static IGFXShader CurrentFlverGFXShader => FlverShader;
        public static IGFXShader CurrentDbgPrimGFXShader => DbgPrimShader;

        public static Stopwatch FpsStopwatch = new Stopwatch();
        private static FrameCounter FpsCounter = new FrameCounter();

        public static float FPS => FpsCounter.CurrentFramesPerSecond;
        public static float AverageFPS => FpsCounter.AverageFramesPerSecond;

        public static bool EnableFrustumCulling = false;
        public static bool EnableTextures = true;

        private static RasterizerState HotSwapRasterizerState_BackfaceCullingOff;
        private static RasterizerState HotSwapRasterizerState_BackfaceCullingOn;

        private static DepthStencilState DepthStencilState_Normal;
        private static DepthStencilState DepthStencilState_DontWriteDepth;

        public static WorldView World = new WorldView();

        public static ModelDrawer ModelDrawer = new ModelDrawer();

        public static GraphicsDevice Device;
        public static FlverShader FlverShader;
        public static DbgPrimShader DbgPrimShader;
        public static SpriteBatch SpriteBatch;
        const string FlverShader__Name = @"Content\NormalMapShader";

        public static DbgPrimGrid DbgPrim_Grid;

        public static bool BackfaceCulling
        {
            set => Device.RasterizerState = value ? HotSwapRasterizerState_BackfaceCullingOn : HotSwapRasterizerState_BackfaceCullingOff;
        }

        private static void CompletelyChangeRasterizerState(RasterizerState rs)
        {
            HotSwapRasterizerState_BackfaceCullingOff = rs.GetCopyOfState();
            HotSwapRasterizerState_BackfaceCullingOff.CullMode = CullMode.None;

            HotSwapRasterizerState_BackfaceCullingOn = rs.GetCopyOfState();
            HotSwapRasterizerState_BackfaceCullingOn.CullMode = CullMode.CullClockwiseFace;
        }

        public static void InitDepthStencil()
        {
            switch (CurrentStep)
            {
                case GFXDrawStep._1_Opaque:
                    Device.DepthStencilState = DepthStencilState_Normal;
                    break;
                case GFXDrawStep._3_AlphaEdge:
                case GFXDrawStep._2_DbgPrim:
                case GFXDrawStep._4_GUI:
                    Device.DepthStencilState = DepthStencilState_DontWriteDepth;
                    break;
            }
        }

        public static void Init(ContentManager c)
        {

            DepthStencilState_Normal = new DepthStencilState()
            {
                //CounterClockwiseStencilDepthBufferFail = Device.DepthStencilState.CounterClockwiseStencilDepthBufferFail,
                //CounterClockwiseStencilFail = Device.DepthStencilState.CounterClockwiseStencilFail,
                //CounterClockwiseStencilFunction = Device.DepthStencilState.CounterClockwiseStencilFunction,
                //CounterClockwiseStencilPass = Device.DepthStencilState.CounterClockwiseStencilPass,
                //DepthBufferEnable = Device.DepthStencilState.DepthBufferEnable,
                //DepthBufferFunction = Device.DepthStencilState.DepthBufferFunction,
                DepthBufferWriteEnable = true,
                //ReferenceStencil = Device.DepthStencilState.ReferenceStencil,
                //StencilDepthBufferFail = Device.DepthStencilState.StencilDepthBufferFail,
                //StencilEnable = Device.DepthStencilState.StencilEnable,
                //StencilFail = Device.DepthStencilState.StencilFail,
                //StencilFunction = Device.DepthStencilState.StencilFunction,
                //StencilMask = Device.DepthStencilState.StencilMask,
                //StencilPass = Device.DepthStencilState.StencilPass,
                //StencilWriteMask = Device.DepthStencilState.StencilWriteMask,
                //TwoSidedStencilMode = Device.DepthStencilState.TwoSidedStencilMode,
            };

            DepthStencilState_DontWriteDepth = new DepthStencilState()
            {
                //CounterClockwiseStencilDepthBufferFail = Device.DepthStencilState.CounterClockwiseStencilDepthBufferFail,
                //CounterClockwiseStencilFail = Device.DepthStencilState.CounterClockwiseStencilFail,
                //CounterClockwiseStencilFunction = Device.DepthStencilState.CounterClockwiseStencilFunction,
                //CounterClockwiseStencilPass = Device.DepthStencilState.CounterClockwiseStencilPass,
                //DepthBufferEnable = Device.DepthStencilState.DepthBufferEnable,
                //DepthBufferFunction = Device.DepthStencilState.DepthBufferFunction,
                DepthBufferWriteEnable = false,
                //ReferenceStencil = Device.DepthStencilState.ReferenceStencil,
                //StencilDepthBufferFail = Device.DepthStencilState.StencilDepthBufferFail,
                //StencilEnable = Device.DepthStencilState.StencilEnable,
                //StencilFail = Device.DepthStencilState.StencilFail,
                //StencilFunction = Device.DepthStencilState.StencilFunction,
                //StencilMask = Device.DepthStencilState.StencilMask,
                //StencilPass = Device.DepthStencilState.StencilPass,
                //StencilWriteMask = Device.DepthStencilState.StencilWriteMask,
                //TwoSidedStencilMode = Device.DepthStencilState.TwoSidedStencilMode,
            };

            FlverShader = new FlverShader(c.Load<Effect>(FlverShader__Name));

            FlverShader.AmbientColor = Vector4.One;
            FlverShader.AmbientIntensity = 0.5f;
            FlverShader.DiffuseColor = Vector4.One;
            FlverShader.DiffuseIntensity = 0.75f;
            FlverShader.SpecularColor = Vector4.One;
            FlverShader.SpecularPower = 10f;
            FlverShader.NormalMapCustomZ = 1.0f;

            DbgPrimShader = new DbgPrimShader(Device);

            SpriteBatch = new SpriteBatch(Device);

            HotSwapRasterizerState_BackfaceCullingOff = Device.RasterizerState.GetCopyOfState();
            HotSwapRasterizerState_BackfaceCullingOff.MultiSampleAntiAlias = true;
            HotSwapRasterizerState_BackfaceCullingOff.CullMode = CullMode.None;

            HotSwapRasterizerState_BackfaceCullingOn = Device.RasterizerState.GetCopyOfState();
            HotSwapRasterizerState_BackfaceCullingOn.MultiSampleAntiAlias = true;
            HotSwapRasterizerState_BackfaceCullingOn.CullMode = CullMode.CullClockwiseFace;

            DbgPrim_Grid = new DbgPrimGrid(Color.Green, Color.Lime * 0.5f, 10, 1);
        }

        public static void BeginDraw()
        {
            InitDepthStencil();
            //InitBlendState();

            World.ApplyViewToShader(CurrentDbgPrimGFXShader);
            World.ApplyViewToShader(CurrentFlverGFXShader);

            GFX.Device.SamplerStates[0] = SamplerState.LinearWrap;

            GFX.FlverShader.EyePosition = GFX.World.CameraTransform.Position;
            GFX.FlverShader.LightDirection = GFX.World.LightDirectionVector;
            GFX.FlverShader.ColorMap = MODEL_VIEWER_MAIN.DEFAULT_TEXTURE_DIFFUSE;
            GFX.FlverShader.NormalMap = MODEL_VIEWER_MAIN.DEFAULT_TEXTURE_NORMAL;
            GFX.FlverShader.SpecularMap = MODEL_VIEWER_MAIN.DEFAULT_TEXTURE_SPECULAR;

        }

        private static void DoDrawStep(GameTime gameTime)
        {
            switch (CurrentStep)
            {
                case GFXDrawStep._1_Opaque:
                case GFXDrawStep._3_AlphaEdge:
                    ModelDrawer.Draw();
                    ModelDrawer.DebugDrawAll();
                    break;
                case GFXDrawStep._2_DbgPrim:
                    if (DBG.ShowGrid)
                        DbgPrim_Grid.Draw();
                    break;
                case GFXDrawStep._4_GUI:
                    DbgMenuItem.CurrentMenu.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
                    break;
            }
        }

        private static void DoDraw(GameTime gameTime)
        {
            if (MODEL_VIEWER_MAIN.DISABLE_DRAW_ERROR_HANDLE)
            {
                DoDrawStep(gameTime);
            }
            else
            {
                try
                {
                    DoDrawStep(gameTime);
                }
                catch
                {
                    var errText = $"Draw Call Failed ({CurrentStep.ToString()})";
                    var errTextSize = DBG.DEBUG_FONT_HQ.MeasureString(errText);
                    DBG.DrawOutlinedText(errText, new Vector2(Device.Viewport.Width / 2, Device.Viewport.Height / 2), Color.Red, DBG.DEBUG_FONT_HQ, 0, 0.25f, errTextSize / 2);
                }
            }

            
            
        }

        public static void DrawScene(GameTime gameTime)
        {
            Device.Clear(Color.Gray);

            for (int i = 0; i < DRAW_STEP_LIST.Length; i++)
            {
                CurrentStep = DRAW_STEP_LIST[i];
                BeginDraw();
                DoDraw(gameTime);
            }

            GFX.UpdateFPS((float)FpsStopwatch.Elapsed.TotalSeconds);
            DBG.DrawOutlinedText($"FPS: {(Math.Round(GFX.AverageFPS))}", new Vector2(0, GFX.Device.Viewport.Height - 24), Color.Yellow);
            FpsStopwatch.Restart();
        }

        public static void UpdateFPS(float elapsedSeconds)
        {
            FpsCounter.Update(elapsedSeconds);
        }
    }
}
