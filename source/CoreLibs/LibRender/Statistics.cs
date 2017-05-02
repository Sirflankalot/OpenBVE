using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibRender {
	public class Statistics {
		public struct InfoCategory {
            public int collected;
            public int total;
        }
        public struct InfoCategoryRender {
            public int rendered;
            public int collected;
            public int total;
        }
        public struct SubFrameTime {
            public float garbage_collection;
            public float updating;
            public float geometry_pass;
            public float lighting_pass;
            public float transparent_pass;
            public float hdr_pass;
            public float text_pass;
            public float textcopy_pass;
		}
        public struct PrimitivesDrawn {
            public int deferred;
            public int forward;
            public int ui;
        }

        internal int val_frame_count;
        internal float val_frame_time;
        internal SubFrameTime val_subframe_time;
        internal PrimitivesDrawn val_primitives_drawn;
        internal InfoCategory val_meshes;
        internal InfoCategory val_textures;
        internal InfoCategoryRender val_objects;
        internal InfoCategory val_flatmeshes;
        internal InfoCategoryRender val_texts;
        internal InfoCategoryRender val_uielements;
        internal InfoCategory val_conelights;
        internal InfoCategory val_pointlights;
        internal InfoCategory val_cameras;

        public int FrameCount { get { return val_frame_count; } }
        public float FrameTime { get { return val_frame_time; } }
        public SubFrameTime SubFrameTimings { get { return val_subframe_time; } }
        public PrimitivesDrawn DrawnPrimitives { get { return val_primitives_drawn; } }
        public InfoCategory Meshes { get { return val_meshes; } }
        public InfoCategory Textures { get { return val_textures; } }
        public InfoCategoryRender Objects { get { return val_objects; } }
        public InfoCategory FlatMeshes { get { return val_flatmeshes; } }
        public InfoCategoryRender Texts { get { return val_texts; } }
        public InfoCategoryRender UIElement { get { return val_uielements; } }
        public InfoCategory ConeLights { get { return val_conelights; } }
        public InfoCategory PointLights { get { return val_pointlights; } }
        public InfoCategory Cameras { get { return val_cameras; } }
    }

    public partial class Renderer {
        internal Statistics statistics = new Statistics();

        public Statistics GetStatistics() {
            return statistics;
        }
    }
}
