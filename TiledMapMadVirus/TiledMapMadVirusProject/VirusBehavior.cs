using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Framework;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Components.Gestures;

namespace TiledMapMadVirusProject
{
    class VirusBehavior : Behavior
    {
        private int colorID;
        public int Color { get { return colorID; } }

        protected override void Update(TimeSpan gameTime)
        {
            var touch = this.Owner.FindComponent<TouchGestures>();
            touch.TouchPressed += (s, o) =>
            {
                System.Console.WriteLine(colorID.ToString());
            };
        }
    }
}
