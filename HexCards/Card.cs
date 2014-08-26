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
    class Card
    {
        //Texture2D cardSheet;                          //Spritesheet containing all card art textures
        Texture2D backOfCard;                           //Image of the back of card
        Texture2D frameSheet;                           //Spritesheet containing the different color frames
        Texture2D numberSheet;                          //Spritesheet containing all numbers that go on the cards

        public Rectangle drawRectangle;                 //drawRectangle for the card
        Rectangle frameRectangle;                       //source rectangle for the frame sheet

        //Source rectangle for the numbers
        Rectangle topNumSource;
        Rectangle leftNumSource;
        Rectangle rightNumSource;

        float scale;
        public int hexWidth;                            //Card Width
        public int hexHeight;                           //Card Height
        float radius;                                   //Card Radius
        public bool onBoard = false;

        //I'm not even using these
        public int Column { get; set; }
        public int Row { get; set; }
        public int GridNumber { get; set; }

        //Which color the frame is
        public CardColor cardColor;

        //The numbers on the card
        public byte TopNumber { get; private set; }
        public byte LeftNumber { get; private set; }
        public byte RightNumber { get; private set; }

        public Point origPos;

        public Card(ContentManager cm, float scale, CardColor cardColor, int cardID)
        {
            //Load content
            backOfCard = cm.Load<Texture2D>("backofcard");
            frameSheet = cm.Load<Texture2D>("frameSheet");
            numberSheet = cm.Load<Texture2D>("numbers");

            this.scale = scale;
            hexWidth = (int)(backOfCard.Width * scale);
            hexHeight = (int)(backOfCard.Height * scale);
            radius = hexWidth / 2 * scale;

            //Assign Numbers
            this.cardColor = cardColor;
            Tuple<byte, byte, byte> cardNumbers = CardNumbers(cardID);
            TopNumber = cardNumbers.Item1;
            LeftNumber = cardNumbers.Item2;
            RightNumber = cardNumbers.Item3;

            //Position Card Elements
            drawRectangle = new Rectangle(0, 0, hexWidth, hexHeight);
            frameRectangle = new Rectangle((int)cardColor * backOfCard.Width, 0, backOfCard.Width, backOfCard.Height);

            //This just defines the size of the draw rectangle for the numbers
            topNumSource = new Rectangle(TopNumber * 18, 0, 18, 22);
            leftNumSource = new Rectangle(LeftNumber * 18, 0, 18, 22);
            rightNumSource = new Rectangle(RightNumber * 18, 0, 18, 22);
        }

        public void SetPosition(Point position)
        {
            drawRectangle.Location = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(backOfCard, drawRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, .003f);
            spriteBatch.Draw(frameSheet, drawRectangle, frameRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0031f);

            DrawNumbers(spriteBatch);
        }

        //public void Update(GameTime time, MouseState mouse, TouchCollection touchColl)
        //{
           
        //}

        //Contains is a circle because if it were a rectangle it would overlap with other cards
        public bool Contains(Vector2 pos)
        {
            if (radius >= Math.Sqrt(Math.Pow(pos.X - drawRectangle.X - radius, 2) + Math.Pow(pos.Y - drawRectangle.Y - radius, 2)))
                return true;
            else
                return false;
        }

        // This draws the numbers on the card according to the position of the card's draw rectangle. There are magic numbers galore.
        // The numbers on the end is the sorting order. It goes from 0-1, the higher it is the closer it is to the viewer.
        public void DrawNumbers(SpriteBatch sb)
        {
            sb.Draw(numberSheet, new Rectangle(
                (int)(drawRectangle.X + (75 - 9) * scale), (int)(drawRectangle.Y + (18 - 11)* scale), 
                (int)(18 * scale), (int)(22 * scale)), topNumSource, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0032f);
            sb.Draw(numberSheet, new Rectangle(
                (int)(drawRectangle.X + (16 - 9) * scale), (int)(drawRectangle.Y + (120 - 11) * scale),
                (int)(18 * scale), (int)(22 * scale)), leftNumSource, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0032f);
            sb.Draw(numberSheet, new Rectangle(
                (int)(drawRectangle.X + (134 - 9) * scale), (int)(drawRectangle.Y + (120 - 11) * scale),
                (int)(18 * scale), (int)(22 * scale)), rightNumSource, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0032f);
        }

        //Here I just define a whole bunch of different cards. In production, a database would contain this information but for now
        //it works just fine over here.
        public Tuple<byte, byte, byte> CardNumbers(int index)
        {
            switch (index)
            {
                case 0:
                    return new Tuple<byte, byte, byte>(0, 0, 0);
                case 1:
                    return new Tuple<byte, byte, byte>(1, 1, 1);
                case 2:
                    return new Tuple<byte, byte, byte>(2, 2, 2);
                case 3:
                    return new Tuple<byte, byte, byte>(3, 3, 3);
                case 4:
                    return new Tuple<byte, byte, byte>(4, 4, 4);
                case 5:
                    return new Tuple<byte, byte, byte>(5, 5, 5);
                case 6:
                    return new Tuple<byte, byte, byte>(6, 6, 6);
                case 7:
                    return new Tuple<byte, byte, byte>(7, 7, 7);
                case 8:
                    return new Tuple<byte, byte, byte>(8, 8, 8);
                case 9:
                    return new Tuple<byte, byte, byte>(9, 9, 9);
                case 10:
                    return new Tuple<byte, byte, byte>(10, 10, 10);
                case 11:
                    return new Tuple<byte, byte, byte>(1, 0, 0);
                case 12:
                    return new Tuple<byte, byte, byte>(1, 1, 0);
                case 13:
                    return new Tuple<byte, byte, byte>(1, 0, 1);
                case 14:
                    return new Tuple<byte, byte, byte>(0, 1, 1);
                case 15:
                    return new Tuple<byte, byte, byte>(2, 0, 0);
                case 16:
                    return new Tuple<byte, byte, byte>(2, 1, 0);
                case 17:
                    return new Tuple<byte, byte, byte>(1, 2, 0);
                case 18:
                    return new Tuple<byte, byte, byte>(0, 2, 1);
                case 19:
                    return new Tuple<byte, byte, byte>(0, 1, 2);
                case 20:
                    return new Tuple<byte, byte, byte>(2, 0, 1);
                case 21:
                    return new Tuple<byte, byte, byte>(1, 0, 2);
                case 22:
                    return new Tuple<byte, byte, byte>(2, 2, 0);
                case 23:
                    return new Tuple<byte, byte, byte>(2, 0, 2);
                case 24:
                    return new Tuple<byte, byte, byte>(0, 2, 2);
                case 25:
                    return new Tuple<byte, byte, byte>(3, 2, 1);
                case 26:
                    return new Tuple<byte, byte, byte>(3, 0, 2);
                case 27:
                    return new Tuple<byte, byte, byte>(0, 3, 1);
                case 28:
                    return new Tuple<byte, byte, byte>(2, 0, 3);
                case 29:
                    return new Tuple<byte, byte, byte>(2, 1, 3);
                case 30:
                    return new Tuple<byte, byte, byte>(2, 3, 1);
                case 31:
                    return new Tuple<byte, byte, byte>(1, 0, 3);
                case 32:
                    return new Tuple<byte, byte, byte>(1, 2, 3);
                case 33:
                    return new Tuple<byte, byte, byte>(3, 2, 1);
                case 34:
                    return new Tuple<byte, byte, byte>(3, 2, 0);
                case 35:
                    return new Tuple<byte, byte, byte>(3, 1, 0);
                case 36:
                    return new Tuple<byte, byte, byte>(3, 0, 1);
                case 37:
                    return new Tuple<byte, byte, byte>(3, 1, 1);
                case 38:
                    return new Tuple<byte, byte, byte>(1, 3, 2);
                case 39:
                    return new Tuple<byte, byte, byte>(1, 1, 3);
                case 40:
                    return new Tuple<byte, byte, byte>(1, 3, 1);
            }
            return new Tuple<byte, byte, byte>(1, 1, 1);
        }
    }
}

//Card color enumeration
enum CardColor { Gray, Blue, Red }
