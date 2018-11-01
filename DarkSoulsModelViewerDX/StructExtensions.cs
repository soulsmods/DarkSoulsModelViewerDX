﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsModelViewerDX
{
    public static class StructExtensions
    {
        public static Vector3 GetCenter(this BoundingBox bb)
        {
            return (bb.Min + bb.Max) / 2;
        }

        public static RasterizerState GetCopyOfState(this RasterizerState rs)
        {
            return new RasterizerState()
            {
                CullMode = rs.CullMode,
                DepthBias = rs.DepthBias,
                DepthClipEnable = rs.DepthClipEnable,
                FillMode = rs.FillMode,
                MultiSampleAntiAlias = rs.MultiSampleAntiAlias,
                Name = rs.Name,
                ScissorTestEnable = rs.ScissorTestEnable,
                SlopeScaleDepthBias = rs.SlopeScaleDepthBias,
            };
        }

        public static Vector2 TopLeftCorner(this Rectangle r) => new Vector2(r.Left, r.Top);
        public static Vector2 TopRightCorner(this Rectangle r) => new Vector2(r.Right, r.Top);
        public static Vector2 BottomLeftCorner(this Rectangle r) => new Vector2(r.Left, r.Bottom);
        public static Vector2 BottomRightCorner(this Rectangle r) => new Vector2(r.Right, r.Bottom);

        public static Rectangle GetUniformShrunkFromBorder(this Rectangle r, int shrinkAmount)
        {
            return new Rectangle(Math.Min(r.X + shrinkAmount, r.X + r.Width / 2),
                Math.Min(r.Y + shrinkAmount, r.Y + r.Height / 2), 
                Math.Max(r.Width - shrinkAmount * 2, 0), 
                Math.Max(r.Height - shrinkAmount * 2, 0));
        }

    }
}