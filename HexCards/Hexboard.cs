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
    class Hexboard
    {
        Texture2D board;                                //The graphic that the hex slots sit on top of
        Texture2D blankHex;                             //The blank slot grapic
        public Rectangle drawRectangle;                 //The draw rectangle for the board graphic

        Point firstCardPos;                             //Where to place the first hex slot
        int tileWidth;                                  //The width of the hex slot

        //Some text used for debugging
        string drawText = "";
        SpriteFont myFont;

        //Arrays to hold the slots and the cards. I'm currently having trouble with the card array
        Hex[] blankHexes = new Hex[19];
        Card[] cards = new Card[19]; //not sure we need this, the playerhand is keeping track of the cards

        public Hexboard(ContentManager cm, Point center, float scale)
        {
            //Load and assign stuff... it's a mess, I know
            board = cm.Load<Texture2D>("Board");
            blankHex = cm.Load<Texture2D>("blankHex");
            myFont = cm.Load<SpriteFont>("Arial");
            int boardWidth = (int)(board.Width * scale);
            int boardHeight = (int)(board.Height * scale);
            drawRectangle = new Rectangle(center.X - boardWidth / 2,
                center.Y - boardHeight / 2, boardWidth, boardHeight);
            tileWidth = (int)(blankHex.Width * scale);
            int tileHeight = (int)(blankHex.Height * scale);

            #region Assigning and Placing Tiles
            //So the way this works is I start at the top row and work my way left to right and top to bottom
            for (int h = 0; h < blankHexes.Length; h++)
            {
                //Load new hex objects from hex class
                blankHexes[h] = new Hex(cm, scale);

                //Assign grid number
                blankHexes[h].GridNumber = h;

                //Assigning Columns
                if (h == 0 || h == 4 || h == 9 || h == 14 || h == 8)
                {
                    blankHexes[h].Column = 0;
                }
                else if (h == 3 || h == 8 || h == 13 || h == 17)
                {
                    blankHexes[h].Column = -1;
                }
                else if (h == 1 || h == 5 || h == 10 || h == 15)
                {
                    blankHexes[h].Column = 1;
                }
                else if (h == 7 || h == 12 || h == 16)
                {
                    blankHexes[h].Column = -2;
                }
                else if (h == 2 || h == 6 || h == 11)
                {
                    blankHexes[h].Column = 2;
                }
            }

            //Placing tiles and assigning rows
            for (int r = 0; r < 5; r++)
            {
                // Assign the first position its values. There are some magic numbers but it works no matter what scale.
                firstCardPos = new Point((int)(drawRectangle.X + boardWidth / 2 - tileWidth), (int)(drawRectangle.Y + boardHeight / 2 - 258 * scale));

                //For the first row, there are only 3 slots
                if (r == 0)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        //Position tiles from left to right
                        blankHexes[c].Position = new Point(firstCardPos.X + c*tileWidth, firstCardPos.Y);
                        blankHexes[c].PositionCard(blankHexes[c].Position);

                        //Assign row value
                        blankHexes[c].Row = -2;
                    }
                }

                firstCardPos = new Point((int)(drawRectangle.X + boardWidth / 2 - tileWidth * 4.5), (int)(drawRectangle.Y + boardHeight / 2 - 129 * scale));
                if (r == 1)
                {
                    for (int c = 3; c < 7; c++)
                    {
                        blankHexes[c].Position = new Point(firstCardPos.X + c * tileWidth, firstCardPos.Y);
                        blankHexes[c].PositionCard(blankHexes[c].Position);
                        blankHexes[c].Row = -1;
                    }
                }

                firstCardPos = new Point((int)(drawRectangle.X + boardWidth / 2 - tileWidth * 9), (int)(drawRectangle.Y + boardHeight / 2));
                if (r == 2)
                {
                    for (int c = 7; c < 12; c++)
                    {
                        blankHexes[c].Position = new Point(firstCardPos.X + c * tileWidth, firstCardPos.Y);
                        blankHexes[c].PositionCard(blankHexes[c].Position);
                        blankHexes[c].Row = 0;
                    }
                }

                firstCardPos = new Point((int)(drawRectangle.X + boardWidth / 2 - tileWidth * 13.5), (int)(drawRectangle.Y + boardHeight / 2 + 129 * scale));
                if (r == 3)
                {
                    for (int c = 12; c < 16; c++)
                    {
                        blankHexes[c].Position = new Point(firstCardPos.X + c * tileWidth, firstCardPos.Y);
                        blankHexes[c].PositionCard(blankHexes[c].Position);
                        blankHexes[c].Row = 1;
                    }
                }

                firstCardPos = new Point((int)(drawRectangle.X + boardWidth / 2 - tileWidth * 17), (int)(drawRectangle.Y + boardHeight / 2 + 258 * scale));
                if (r == 4)
                {
                    for (int c = 16; c < blankHexes.Length; c++)
                    {
                        blankHexes[c].Position = new Point(firstCardPos.X + c * tileWidth, firstCardPos.Y);
                        blankHexes[c].PositionCard(blankHexes[c].Position);
                        blankHexes[c].Row = 2;
                    }
                }
            }
            #endregion

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Hex t in blankHexes)
            {
                t.Draw(spriteBatch);
            }

        }
        
        //Does this look familiar?
        Point CalculatePoint(Point centerOfBoard, float distance, float angle)
        {
            //bx = ax + d*cos(t);
            //by = ay + d*sin(t)

            Point finalPoint = new Point();
            finalPoint.X = (int)(centerOfBoard.X + distance * Math.Cos(angle));
            finalPoint.Y = (int)(centerOfBoard.Y + distance * Math.Sin(angle));

            return finalPoint;
        }

        //I'm having massive trouble with the bottom two methods. This is supposed to place the card on the hex board on the exact
        //position of the hex slot underneath it when the player releases his mouse button or finger
        public void PlaceCard(Card card)
        {
            //Loop through all the hex slots to see which the player has released the mouse (or finger) over.
            for (int i = 0; i < blankHexes.Length; i++)
            {
                // If the hex that the loop is currently on contains the drawRecangle's location
                Point centerOfCard = card.drawRectangle.Center;
                if (blankHexes[i].drawRectangle.Contains(centerOfCard))
                {
                    //Assign this card the values of the hex and place it in that position in the cards array and the right position
                    //on the screen
                    cards[i] = card;
                    card.Row = blankHexes[i].Row;
                    card.Column = blankHexes[i].Column;
                    card.GridNumber = i;
                    card.drawRectangle.Location = blankHexes[i].drawRectangle.Location;
                    card.onBoard = true;
                    return;
                }
            }

            //if couldn't place the card then it returns offboard again
            card.onBoard = false;
        }

        //Checks to see if the point lays inside the drawrectangle of the board
        public bool ContainsPoint(Vector2 point)
        {
            foreach (Hex h in blankHexes)
            {
                if (h.drawRectangle.Contains(point)) return true;
            }

            return false;
        }
    }
}
