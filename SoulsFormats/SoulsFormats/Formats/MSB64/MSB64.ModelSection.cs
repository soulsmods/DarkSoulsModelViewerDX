﻿using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB64
    {
        /// <summary>
        /// A section containing all the models available to parts in this map.
        /// </summary>
        public class ModelSection : Section<Model>
        {
            /// <summary>
            /// The MSB type string for this section.
            /// </summary>
            public override string Type => "MODEL_PARAM_ST";

            /// <summary>
            /// Map piece models in this section.
            /// </summary>
            public List<Model.MapPiece> MapPieces;

            /// <summary>
            /// Object models in this section.
            /// </summary>
            public List<Model.Object> Objects;
            
            /// <summary>
            /// Enemy models in this section.
            /// </summary>
            public List<Model.Enemy> Enemies;

            /// <summary>
            /// Player models in this section.
            /// </summary>
            public List<Model.Player> Players;

            /// <summary>
            /// Collision models in this section.
            /// </summary>
            public List<Model.Collision> Collisions;

            /// <summary>
            /// Other models in this section.
            /// </summary>
            public List<Model.Other> Others;

            internal ModelSection(BinaryReaderEx br, int unk1) : base(br, unk1)
            {
                MapPieces = new List<Model.MapPiece>();
                Objects = new List<Model.Object>();
                Enemies = new List<Model.Enemy>();
                Players = new List<Model.Player>();
                Collisions = new List<Model.Collision>();
                Others = new List<Model.Other>();
            }

            public override List<Model> GetEntries()
            {
                return Util.ConcatAll<Model>(
                    MapPieces, Objects, Enemies, Players, Collisions, Others);
            }

            internal override Model ReadEntry(BinaryReaderEx br)
            {
                ModelType type = br.GetEnum32<ModelType>(br.Position + 8);

                switch (type)
                {
                    case ModelType.MapPiece:
                        var mapPiece = new Model.MapPiece(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case ModelType.Object:
                        var obj = new Model.Object(br);
                        Objects.Add(obj);
                        return obj;

                    case ModelType.Enemy:
                        var enemy = new Model.Enemy(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case ModelType.Player:
                        var player = new Model.Player(br);
                        Players.Add(player);
                        return player;

                    case ModelType.Collision:
                        var collision = new Model.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    case ModelType.Other:
                        var other = new Model.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unsupported model type: {type}");
                }
            }

            internal override Model ReadEntryBB(BinaryReaderEx br)
            {
                ModelType type = br.GetEnum32<ModelType>(br.Position + 8);

                switch (type)
                {
                    case ModelType.MapPiece:
                        var mapPiece = new Model.MapPiece(br, MSBVersion.MSBVersionBB);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case ModelType.Object:
                        var obj = new Model.Object(br, MSBVersion.MSBVersionBB);
                        Objects.Add(obj);
                        return obj;

                    case ModelType.Enemy:
                        var enemy = new Model.Enemy(br, MSBVersion.MSBVersionBB);
                        Enemies.Add(enemy);
                        return enemy;

                    case ModelType.Item:
                        var enemy2 = new Model.Enemy(br);
                        Enemies.Add(enemy2);
                        return enemy2;

                    case ModelType.Player:
                        var player = new Model.Player(br, MSBVersion.MSBVersionBB);
                        Players.Add(player);
                        return player;

                    case ModelType.Collision:
                        var collision = new Model.Collision(br, MSBVersion.MSBVersionBB);
                        Collisions.Add(collision);
                        return collision;
                        /*
                    case ModelType.Other:
                        var other = new Model.Other(br);
                        Others.Add(other);
                        return other;
                        */
                    default:
                        return null;

                        //throw new NotImplementedException($"Unsupported model type: {type}");
                }
            }

            internal override void WriteEntries(BinaryWriterEx bw, List<Model> entries)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    bw.FillInt64($"Offset{i}", bw.Position);
                    entries[i].Write(bw);
                }
            }
        }
        
        internal enum ModelType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Item = 3,
            Player = 4,
            Collision = 5,
            Navmesh = 6,
            DummyObject = 7,
            DummyEnemy = 8,
            Other = 0xFFFFFFFF
        }

        /// <summary>
        /// A model available for use by parts in this map.
        /// </summary>
        public abstract class Model : Entry
        {
            internal abstract ModelType Type { get; }

            /// <summary>
            /// The name of this model.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// The placeholder used for this model in MapStudio.
            /// </summary>
            public string Placeholder;

            /// <summary>
            /// The ID of this model.
            /// </summary>
            public int ID;

            /// <summary>
            /// The number of instances of this model in the map.
            /// </summary>
            public int InstanceCount;

            public Model(Model clone)
            {
                Name = clone.Name;
                Placeholder = clone.Placeholder;
                ID = clone.ID;
                InstanceCount = clone.InstanceCount;
            }

            internal Model(BinaryReaderEx br, MSBVersion version=MSBVersion.MSBVersionDS3)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                //br.AssertUInt32((uint)Type);
                br.ReadInt32();
                ID = br.ReadInt32();
                long placeholderOffset = br.ReadInt64();
                InstanceCount = br.ReadInt32();
                br.AssertInt32(0);

                Name = br.GetUTF16(start + nameOffset);
                Placeholder = br.GetUTF16(start + placeholderOffset);

                ReadSpecific(br, start, version);
            }

            internal abstract void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version);

            internal void Write(BinaryWriterEx bw)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(ID);
                bw.ReserveInt64("PlaceholderOffset");
                bw.WriteInt32(InstanceCount);
                bw.WriteInt32(0);
                bw.ReserveInt64("UnkOffset");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.FillInt64("PlaceholderOffset", bw.Position - start);
                bw.WriteUTF16(Placeholder, true);
                bw.Pad(8);

                WriteSpecific(bw, start);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw, long start);

            /// <summary>
            /// Returns the model type, ID and name of this model.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {ID} : {Name}";
            }

            /// <summary>
            /// A fixed part of the level geometry.
            /// </summary>
            public class MapPiece : Model
            {
                internal override ModelType Type => ModelType.MapPiece;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk1;

                public int Unk2;

                public int Unk3;

                internal MapPiece(BinaryReaderEx br) : base(br) { }
                internal MapPiece(BinaryReaderEx br, MSBVersion version) : base(br, version) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    long unkOffset = br.ReadInt64();
                    br.Position = start + unkOffset;
                    Unk1 = br.ReadInt32();
                    br.AssertInt32(0);
                    if (version == MSBVersion.MSBVersionBB)
                    {
                        Unk2 = br.ReadInt32();
                        Unk3 = br.ReadInt32();
                    }
                    else
                    {
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                    }
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", bw.Position - start);
                    bw.WriteInt32(Unk1);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A dynamic or interactible entity.
            /// </summary>
            public class Object : Model
            {
                internal override ModelType Type => ModelType.Object;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk1;

                public Object(Object clone) : base(clone)
                {
                    Unk1 = clone.Unk1;
                }

                internal Object(BinaryReaderEx br) : base(br) { }
                internal Object(BinaryReaderEx br, MSBVersion version) : base(br, version) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    long unkOffset = br.ReadInt64();
                    br.Position = start + unkOffset;
                    Unk1 = br.ReadInt32();
                    if (version != MSBVersion.MSBVersionBB)
                    {
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                    }
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", bw.Position - start);
                    bw.WriteInt32(Unk1);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Any character in the map that is not the player.
            /// </summary>
            public class Enemy : Model
            {
                internal override ModelType Type => ModelType.Enemy;

                internal Enemy(BinaryReaderEx br) : base(br) { }
                internal Enemy(BinaryReaderEx br, MSBVersion version) : base(br, version) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    br.AssertInt64(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", 0);
                }
            }

            /// <summary>
            /// The player character.
            /// </summary>
            public class Player : Model
            {
                internal override ModelType Type => ModelType.Player;

                internal Player(BinaryReaderEx br) : base(br) { }
                internal Player(BinaryReaderEx br, MSBVersion version) : base(br, version) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    br.AssertInt64(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", 0);
                }
            }

            /// <summary>
            /// The invisible physical surface of the map.
            /// </summary>
            public class Collision : Model
            {
                internal override ModelType Type => ModelType.Collision;

                internal Collision(BinaryReaderEx br) : base(br) { }
                internal Collision(BinaryReaderEx br, MSBVersion version) : base(br, version) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    br.AssertInt64(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", 0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Other : Model
            {
                internal override ModelType Type => ModelType.Other;

                internal Other(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br, long start, MSBVersion version)
                {
                    br.AssertInt64(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("UnkOffset", 0);
                }
            }
        }
    }
}
