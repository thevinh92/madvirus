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
    public delegate void ChooseColor(int color);
    class VirusButtonBehavior : Behavior
    {
        public event ChooseColor click;
        public VirusButtonBehavior(int color)
        {
            colorID = color;
        }
        private int colorID;
        private bool isClickable = true;
        public int Color { get { return colorID; } }
        protected override void Update(TimeSpan gameTime)
        {
            // Touch
            var touch = this.Owner.FindComponent<TouchGestures>();
            touch.TouchTap += (s, o) =>
            {
                //System.Console.WriteLine(colorID.ToString());
                if (click != null && isClickable)
                {
                    click(colorID);
                    isClickable = false;
                }
            };
        }

        public void changeClickableEvent()
        {
            isClickable = true;
        }

    }
}
