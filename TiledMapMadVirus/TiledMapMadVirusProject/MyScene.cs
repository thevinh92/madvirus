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

using TiledMapMadVirusProject;
#endregion

namespace TiledMapMadVirusProject
{
    public class MyScene : Scene
    {
        private TiledMap tileMap;
        private int[,] virusIndexArray;
        private Entity[,] virusEntityArray;
        protected override void CreateScene()
        {
            //Insert your scene definition here.
            this.CreateCamera();
            //this.CreateTileMap();
            virusIndexArray = this.generateRandomMap(6, 9);
            this.print2DArray(virusIndexArray);
            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMap(virusIndexArray);
        }

        private void CreateCamera()
        {
            // Create a 2D camera
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = new Color("#e7e7e7")
            };

            var camera2DComponent = camera.Entity.FindComponent<Camera2D>();
//            camera2DComponent.Zoom = Vector2.One / 2.5f;

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

        private void CreateVirusMap(int[,] arr)
        {

            System.Console.WriteLine("row count: {0}", arr.GetLength(0));
            System.Console.WriteLine("Column count: {0}", arr.GetLength(1));
            Transform2D sampleTrans = new Transform2D();// by default: X = 0, Y = 0, top-left conner
            for(int i = 0; i < arr.GetLength(0); i++)
            {
                 //for(int j = 0; j < arr.GetLength(1); j++)
                for (int j = 0; j < arr.GetLength(1); j++)
                 {
                        //Entity virus = new Entity("virus" + i.ToString() + j.ToString())
                        // .AddComponent(new Transform2D()
                        // {
                        //     X = j * MadVirusConstants.VIRUS_SPRITE_WIDTH * 3 / 4, // Don't ask why, I just found the formula
                        //     Y = i * MadVirusConstants.VIRUS_SPRITE_HEIGHT + (j % 2) * 74 / 2 //  Don't ask why, I just found the formula
                        // })
                        //.AddComponent(new Sprite("Content/virus_blue.wpk"))
                        //.AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
                        //                        EntityManager.Add(virus);
                     this.CreateVirus(i, j, arr[i, j]);
                    //switch (arr[i,j]) 
                    //{
                    //    case MadVirusConstants.BLUE_VIRUS_ID:
                    //        break;
                    //    case MadVirusConstants.GREEN_VIRUS_ID:
                    //        break;
                    //    case MadVirusConstants.MAGENTA_VIRUS_ID:
                    //        break;
                    //    case MadVirusConstants.ORANGE_VIRUS_ID:
                    //        break;
                    //    case MadVirusConstants.RED_VIRUS_ID:
                    //        break;
                    //    case MadVirusConstants.YELLOW_VIRUS_ID:
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
        }
    }

        private int[,] generateRandomMap(int r, int q)
        {
            int[,] arrayMap = new int[r,q];
            // Init Array of virus entity
            virusEntityArray = new Entity[r,q];
            System.Random rnd = new System.Random();
            for (int i = 0; i < r; i++ )
            {
                for (int j = 0; j< q; j++)
                {
                    
                    arrayMap[i, j] = rnd.Next(6);
                }
            }
            return arrayMap;
        }

        private void print2DArray(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    System.Console.WriteLine("Element({0},{1})={2}", i, j, arr[i, j]);
                }
            }
        }

        // Create new Virus and Draw (add to EntityManager)
        private void CreateVirus(int row, int collumn, int id)
        {
            if(id > 0)
            {
                String spriteName = "Content/virus_" + id.ToString() + ".wpk";
                Entity virus = new Entity("virus" + row.ToString() + collumn.ToString())
                 .AddComponent(new Transform2D()
                 {
                     X = collumn * MadVirusConstants.VIRUS_SPRITE_WIDTH * 3 / 4, // Don't ask why, I just found the formula
                     Y = row * MadVirusConstants.VIRUS_SPRITE_HEIGHT + (collumn % 2) * 74 / 2 //  Don't ask why, I just found the formula
                 })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
                EntityManager.Add(virus);
                virusEntityArray[row, collumn] = virus;
            }
        }
        protected override void Start()
        {
            base.Start();

            // This method is called after the CreateScene and Initialize methods and before the first Update.
        }
    }
}
