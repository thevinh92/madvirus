#region Using Statements
using System;
using System.Collections.Generic;

using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Physics2D;

using WaveEngine.Components.Animation;
using WaveEngine.ImageEffects;
using WaveEngine.TiledMap;

using TiledMapMadVirusProject;

using WaveEngine.Components.UI;
using WaveEngine.Framework.UI;
#endregion

namespace TiledMapMadVirusProject
{
    struct VirusCoord
    {
        private int row;
        private int column;
        public int R
        {
            get
            {
                return row;
            }
        }

        public int Q
        {
            get
            {
                return column;
            }
        }

        public VirusCoord(int r, int q)
        {
            this.row = r;
            this.column = q;
        }

    }

    public delegate void ChangeClickableState();
    public class MyScene : Scene
    {
        public event ChangeClickableState changeClickableEvent;
        private TiledMap tileMap;
        // virusIndexArray[i,j] = (int) x
        // x =  the color of virus at row i and column j
        private int[,] virusIndexArray;
        private Entity[,] virusEntityArray;
        private List<VirusCoord> selectedVirusList;
        private VirusCoord[] startPos;
        private VirusCoord[,] directions = new VirusCoord[,] 
        {
            // q is even
            {new VirusCoord(0,1), new VirusCoord(-1,1), new VirusCoord(-1,0), new VirusCoord(-1,-1), new VirusCoord(0,-1), new VirusCoord(1,0)},
            // q is odd
            {new VirusCoord(1,1), new VirusCoord(0,1), new VirusCoord(-1,0), new VirusCoord(0,-1), new VirusCoord(1,-1), new VirusCoord(1,0)}
        };

        private int seletedColor;
        private int row_height;
        private int column_width;
        protected override void CreateScene()
        {
            //Insert your scene definition here.
            this.CreateCamera();
            //this.CreateTileMap();
            row_height = 8;
            column_width = 15;
            virusIndexArray = this.generateRandomMap(row_height, column_width, 1);
            // Add starPos to selectVirus
            selectedVirusList = new List<VirusCoord>();
            selectedVirusList.Add(startPos[0]);

            this.print2DArray(virusIndexArray);

            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMap(virusIndexArray);

            // Create the virus button for user to interative
            this.CreateVirusButton();

            // Test
            List<VirusCoord> list = this.FindNeighbor(5, 6);
            this.PrintListCoord(list);
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
                for (int j = 0; j < arr.GetLength(1); j++)
                 {
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

        private int[,] generateRandomMap(int r, int q, int startPosCount)
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

            // Pick a valid random virus as a start position
            startPos = new VirusCoord[startPosCount];
            for (int i = 0; i < startPosCount; i++)
            {
                int a = 0;
                while (a == 0)
                {
                    System.Random rndR = new System.Random();
                    System.Random rndQ = new System.Random();
                    if (arrayMap[rndR.Next(r - 1), rndQ.Next(q - 1)] <= 0)
                    {
                        continue;
                    }
                    VirusCoord virusCoord = new VirusCoord(rndR.Next(r - 1), rndQ.Next(q - 1));
                    a = 1;
                    startPos[i] = virusCoord;
                }
            }

            // Add startPos to virus map
            for (int i = 0; i < startPosCount; i++ )
            {
                arrayMap[startPos[i].R, startPos[i].Q] = -rnd.Next(6);
            }

                return arrayMap;
        }

        // Create new Virus and Draw (add to EntityManager)
        private void CreateVirus(int row, int collumn, int id)
        {
            if(id != 0)
            {
                String spriteName = "Content/virus_" + id.ToString() + ".wpk";
                Entity virus = new Entity("virus" + row.ToString() + collumn.ToString())
                 .AddComponent(new Transform2D()
                 {
                     X = collumn * MadVirusConstants.VIRUS_SPRITE_WIDTH * 3 / 4, // Don't ask why, I just found the formula
                     Y = row * MadVirusConstants.VIRUS_SPRITE_HEIGHT + (collumn % 2) * 74 / 2 //  Don't ask why, I just found the formula
                 })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                ;
                EntityManager.Add(virus);
                virusEntityArray[row, collumn] = virus;

                this.PrintVirusCoordAndId(row, collumn, id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        private void UpdateSpriteOfVirus(int row, int column, int id)
        {
            String spriteName = "Content/virus_" + id.ToString() + ".wpk";
            Entity virus = virusEntityArray[row, column];
            virus.RemoveComponent<Sprite>();
            virus.RemoveComponent<SpriteRenderer>();
            virus.AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
        }

        // Show Button to choose virus color in mobile
        private void CreateVirusButton()
        {
            for(int i = 0; i<6; i++)
            {
                String spriteName = "Content/virus_" + (i+1).ToString() + ".wpk";
                Entity virusButton = new Entity("virusButton" + (i + 1).ToString())
                .AddComponent(new Transform2D()
                {
                    X = (column_width + 1)*MadVirusConstants.VIRUS_SPRITE_WIDTH*3/4,
                    Y = MadVirusConstants.VIRUS_SPRITE_HEIGHT*(i)*4/3,
                    Scale = new Vector2(1.5f, 1.5f)
                })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new TouchGestures()
                {
                    EnabledGestures = SupportedGesture.Translation
                })
                .AddComponent(new VirusButtonBehavior(i+1));
                EntityManager.Add(virusButton);

                // Add event click
                var virusButtonBehavior = virusButton.FindComponent<VirusButtonBehavior>();
                virusButtonBehavior.click += this.PlayWithColor;
                this.changeClickableEvent += virusButtonBehavior.changeClickableEvent;
            }
        }

        /// <summary>
        /// Find all the neighbors that valid and unseleted
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private List<VirusCoord> FindNeighbor(VirusCoord coord)
        {
            int r = coord.R;
            int q = coord.Q;
            List<VirusCoord> neighborList = new List<VirusCoord>();
            var parity = q%2;
            for (int i = 0; i < directions.GetLength(1); i ++ )
            {
                int r2 = r + directions[parity, i].R;
                int q2 = q + directions[parity, i].Q;
                if(IsCoordValid(r2,q2))
                {
                    neighborList.Add(new VirusCoord(r2, q2));
                }

            }
                return neighborList;
        }

        private List<VirusCoord> FindNeighbor(int r, int q)
        {
            List<VirusCoord> neighborList = new List<VirusCoord>();
            var parity = q % 2;
            for (int i = 0; i < directions.GetLength(1); i++)
            {
                int r2 = r + directions[parity, i].R;
                int q2 = q + directions[parity, i].Q;
                if (IsCoordValid(r2, q2) && virusIndexArray[r2, q2] > 0)
                {
                    neighborList.Add(new VirusCoord(r2, q2));
                }

            }
            return neighborList;
        }
        private bool IsCoordValid(int r, int q)
        {
            if (r >= 0 && r < row_height && q >= 0 && q < column_width)
                return true;
            return false;
        }

        private void PlayWithColor(int color)
        {
            System.Console.WriteLine(color.ToString());
            if(changeClickableEvent != null)
            {
                // Because the code to get touch in the VirusButtonBehavior.cs has been called many times
                // So after click to virus button, I disable them, and after call this method, I enable them. 
                changeClickableEvent();
            }
            this.seletedColor = color;
            //  iterate through the selectedVirus
            
            this.IteratingVirusListWithColor(color, selectedVirusList);

            // Update the sprite of virus
            for (int i = 0; i < virusIndexArray.GetLength(0); i++)
            {
                for (int j = 0; j < virusIndexArray.GetLength(1); j++)
                {
                    if(virusIndexArray[i,j] < 0)
                    {
                        UpdateSpriteOfVirus(i, j, -color);
                    }
                }
            }
        }

        private void IteratingVirusListWithColor(int color, List<VirusCoord> listVirus)
        {
            List<VirusCoord> tempSelectedVirusList = new List<VirusCoord>();

            // virus that all it's neighbor is selected will not use to find
            List<VirusCoord> listVirusWillDeleteFromSelected = new List<VirusCoord>();
            foreach (var item in listVirus)
            {
                List<VirusCoord> neighborList = this.FindNeighbor(item);
                if(neighborList.Count > 0)
                {
                    foreach (var neighbor in neighborList)
                    {
                        // if the color is the same with the color we select
                        if (virusIndexArray[neighbor.R, neighbor.Q] == color)
                        {
                            // Choose this virus
                            tempSelectedVirusList.Add(new VirusCoord(neighbor.R, neighbor.Q));
                            // Change the color id
                            virusIndexArray[neighbor.R, neighbor.Q] = -color;
                        }
                    }
                }
                else
                {
                    listVirusWillDeleteFromSelected.Add(item);
                }
            }

            if (listVirusWillDeleteFromSelected.Count > 0)
            {
                foreach (var item in listVirusWillDeleteFromSelected)
                {
                    selectedVirusList.Remove(item);
                }
            }

            // Update the virus map
            if (tempSelectedVirusList.Count > 0)
            {
                foreach (var item in tempSelectedVirusList)
                {
                    selectedVirusList.Add(item);
                }
                this.IteratingVirusListWithColor(color, tempSelectedVirusList);
            }
        }
        protected override void Start()
        {
            base.Start();

        }

        #region Debug
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

        private void PrintListCoord(List<VirusCoord> list)
        {
            foreach (var item in  list)
            {
                Console.WriteLine("Coord at row {0}, column {1}", item.R, item.Q);
            }
        }

        private void PrintVirusCoordAndId(int r, int q,int id)
        {
            // Print Text block to debug
            TextBlock title = new TextBlock()
            {
                Text = r.ToString() + q.ToString() + id.ToString(),
                Width = MadVirusConstants.VIRUS_SPRITE_WIDTH,
                Foreground = Color.White,
                Margin = new Thickness(q * MadVirusConstants.VIRUS_SPRITE_WIDTH * 3 / 4 + 20,
                    r * MadVirusConstants.VIRUS_SPRITE_HEIGHT + (q % 2) * 74 / 2 + 20,
                    0, 0)
            };
            EntityManager.Add(title);
        }
        #endregion
    }
}
