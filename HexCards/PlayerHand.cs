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
    class PlayerHand
    {
        Texture2D bg;
        Rectangle bgRectangle;

        Card selectedCard;
        List<Card> cards = new List<Card>();

        SpriteFont myFont;
        float scale;
        string drawText = "";
        Hexboard board;

        Vector2 currentMousePosition;
        Vector2 mouseDownPosition;
        bool isScrolling = false;
        bool isTouchDragging = false;
        MouseState oldMouse, currentMouse;
        TouchCollection touchColl, oldTouch;
        Vector2 touchDownPosition;

        //scroll
        int scrollPosition = 0;
        const int pixelsFromEdge = 50;
        const int scrollSpeed = 4;

        public PlayerHand(ContentManager cm, int screenWidth, int screenHeight, float scale, Hexboard board, int numberOfCards)
        {
            this.scale = scale;
            bg = cm.Load<Texture2D>("playerHandBG");
            int bgWidth = (int)(bg.Width * scale);
            int bgHeight = (int)(bg.Height * scale * 0.85);
            bgRectangle = new Rectangle(0, screenHeight - (int)(bgHeight), screenWidth, bgHeight);
            for (int i = 0; i < numberOfCards; i++)
            {
                Card c = new Card(cm, scale, CardColor.Red, 40);
                Point cardPosition;
                if (i < numberOfCards / 2) //first row
                {
                    cardPosition = new Point(i * c.hexWidth, bgRectangle.Top);
                }
                else //second row
                {
                    cardPosition = new Point((i - (numberOfCards / 2)) * c.hexWidth + c.hexWidth / 2, bgRectangle.Top + (int)(c.hexHeight * 0.75));
                }

                c.drawRectangle.Location = cardPosition;
                c.origPos = cardPosition;
                cards.Add(c);
            }

            myFont = cm.Load<SpriteFont>("Arial");
            this.board = board;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(myFont, drawText, new Vector2(0, 0), Color.White);
            sb.Draw(bg, bgRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0001f);
            foreach (Card c in cards)
            {
                c.Draw(sb);
            }

        }

        public void Update(GameTime time, MouseState mouse, TouchCollection touchColl)
        {
            currentMouse = mouse;
            currentMousePosition = new Vector2(currentMouse.Position.X, currentMouse.Position.Y);

            this.touchColl = touchColl;

            CheckForLeftButtonDown();
            CheckForLeftButtonRelease();
            CheckTouchDown();
            CheckTouchRelease();

            oldMouse = currentMouse;
            oldTouch = touchColl;

        }


        private void CheckForLeftButtonDown()
        {
            //new clicks
            if (currentMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                mouseDownPosition = currentMousePosition;
                foreach (Card card in cards)
                {
                    if (card.drawRectangle.Contains(mouseDownPosition))
                    {
                        selectedCard = card;
                    }
                }
            }

            //Scrolling
            //wait for mouse to move determinately so we know if it's a scroll or a tile movement
            if (currentMouse.LeftButton == ButtonState.Pressed && mouseDownPosition.Y > bgRectangle.Top)
            {
                //scrolling
                if (isScrolling)
                {
                    int difference = (int)(currentMousePosition.X - mouseDownPosition.X);
                    foreach (Card card in cards)
                    {
                        if (!card.onBoard)
                        {
                            card.drawRectangle.X += difference;
                        }
                        card.origPos.X += difference;
                    }
                    mouseDownPosition.X = currentMousePosition.X;
                }
                //check to see if starting to scroll
                else if (Math.Abs(mouseDownPosition.X - currentMousePosition.X) > 10 && Math.Abs(mouseDownPosition.Y - currentMousePosition.Y) < 5) 
                {
                    isScrolling = true;
                    if (selectedCard != null)
                    {
                        selectedCard.SetPosition(selectedCard.origPos);
                        selectedCard = null;
                    }
                }
                //add lag so card doesn't move instantly
                else if (Math.Abs(mouseDownPosition.Y - currentMousePosition.Y) > 15)
                {
                    selectedCard.SetPosition(new Point((int)currentMousePosition.X - selectedCard.hexWidth / 2, (int)currentMousePosition.Y - selectedCard.hexHeight / 2));
                }

            }
            else if (currentMouse.LeftButton == ButtonState.Pressed && selectedCard != null)
            {
                selectedCard.SetPosition(new Point((int)currentMousePosition.X - selectedCard.hexWidth / 2, (int)currentMousePosition.Y - selectedCard.hexHeight / 2));
            }



        }

        private void CheckForLeftButtonRelease()
        {
            //if the user just released the mousebutton - set _isDragging to false, and check if we should add the tile to the board
            if (oldMouse.LeftButton == ButtonState.Pressed && currentMouse.LeftButton == ButtonState.Released)
            {

                if (selectedCard != null)
                {
                    board.PlaceCard(selectedCard);
                    if (board.ContainsPoint(currentMousePosition))
                    {
                        drawText = "on board";
                    }
                    else
                    {
                        drawText = "off board";
                        selectedCard.SetPosition(selectedCard.origPos);
                    }
                }
                selectedCard = null;
                isScrolling = false;
            }
        }

        //These next two methods does the same things as the ones above but for touch
        private void CheckTouchDown()
        {
            if (touchColl.Count > 0)
            {
                if (oldTouch.Count == 0)
                    touchDownPosition = touchColl[0].Position;

                foreach (Card card in cards)
                {
                    if ((touchDownPosition - touchColl[0].Position).Length() > 10 && card.Contains(touchDownPosition))
                        selectedCard = card;
                }

            }
        }
        private void CheckTouchRelease()
        {
            if (oldTouch.Count > 0 && touchColl.Count == 0 && isTouchDragging)
                isTouchDragging = false;
        }


    }
}
