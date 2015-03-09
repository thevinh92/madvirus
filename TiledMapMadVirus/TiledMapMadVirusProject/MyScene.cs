#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;

using WaveEngine.Components.Animation;
using WaveEngine.ImageEffects;
using WaveEngine.TiledMap;
#endregion

namespace TiledMapMadVirusProject
{
    public class MyScene : Scene
    {
        private TiledMap tileMap;
        protected override void CreateScene()
        {
            //Insert your scene definition here.
            this.CreateCamera();
            this.CreateTileMap();
        }

        private void CreateCamera()
        {
            // Create a 2D camera
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = new Color("#e7e7e7")
            };

            var camera2DComponent = camera.Entity.FindComponent<Camera2D>();
            camera2DComponent.Zoom = Vector2.One / 2.5f;

            #region Lens Effects
            ////if (WaveServices.Platform.PlatformType == PlatformType.Windows ||
            ////    WaveServices.Platform.PlatformType == PlatformType.Linux ||
            ////    WaveServices.Platform.PlatformType == PlatformType.MacOS)
            ////{
            ////    camera.Entity.AddComponent(ImageEffects.FishEye());
            ////    camera.Entity.AddComponent(new ChromaticAberrationLens() { AberrationStrength = 5.5f });
            ////    camera.Entity.AddComponent(new RadialBlurLens() { Center = new Vector2(0.5f, 0.75f), BlurWidth = 0.02f, Nsamples = 5 });
            ////    camera.Entity.AddComponent(ImageEffects.Vignette());
            ////    camera.Entity.AddComponent(new FilmGrainLens() { GrainIntensityMin = 0.075f, GrainIntensityMax = 0.15f });
            ////}  
            #endregion

            EntityManager.Add(camera);
        }

        private void CreateTileMap()
        {
            var map = new Entity("map")
                .AddComponent(new Transform2D())
                .AddComponent(this.tileMap = new TiledMap("Content/testMapOdd-q.tmx")
                {
                    MinLayerDrawOrder = -10,
                    MaxLayerDrawOrder = -0
                });

            this.EntityManager.Add(map);
        }

        protected override void Start()
        {
            base.Start();

            // This method is called after the CreateScene and Initialize methods and before the first Update.
        }
    }
}
