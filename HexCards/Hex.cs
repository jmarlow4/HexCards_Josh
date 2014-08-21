using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexCards
{
    class Hex
    // This class describes the positions (slots) on the hex board
    {
        public Rectangle drawRectangle;                            //Draw rectangle for the hex
        float scale;                                        //Scale
        int hexWidth;                                       //Width
        int hexHeight;                                      //Height
        float radius;                                       //Radius of click circle
        Texture2D graphic;                                  //Graphic for the hex slot

        //These aren't even used at the moment
        public int Column { get; set; }
        public int Row { get; set; }
        public int GridNumber { get; set; }
        public Point Position { get; set; }

        public Hex(ContentManager cm, float scale)
        {
            //Loading and assigning stuff
            graphic = cm.Load<Texture2D>("blankHex");
            this.scale = scale;
            hexWidth = (int)(graphic.Width);
            hexHeight = (int)(graphic.Height);
            radius = hexWidth / 2 * scale;

            drawRectangle = new Rectangle(0, 0, (int)(hexWidth * scale), (int)(hexHeight * scale));
            //position = new Vector2(Column, Row);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(graphic, drawRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, .002f);
        }

        public virtual void PositionCard(Point center)
        {
            drawRectangle.X = center.X - drawRectangle.Width / 2;
            drawRectangle.Y = center.Y - drawRectangle.Height / 2;
        }

        public bool Contains(Vector2 pos)
        {
            if (radius >= Math.Sqrt(Math.Pow(pos.X - drawRectangle.X - radius, 2) + Math.Pow(pos.Y - drawRectangle.Y - radius, 2)))
                return true;
            else
                return false;
        }
    }
}
