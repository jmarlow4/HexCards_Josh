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
                    cardPosition = new Point((i-(numberOfCards/2)) * c.hexWidth+c.hexWidth/2, bgRectangle.Top+(int)(c.hexHeight*0.75));
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

            if (selectedCard != null) selectedCard.SetPosition(new Point((int)currentMousePosition.X - selectedCard.hexWidth / 2, (int)currentMousePosition.Y - selectedCard.hexHeight / 2));

            this.touchColl = touchColl;

            CheckForLeftButtonDown();
            CheckForLeftButtonRelease();
            CheckTouchDown();
            CheckTouchRelease();
            CheckScroll();

            oldMouse = currentMouse;
            oldTouch = touchColl;

        }

        private void CheckScroll()
        {
            //scroll left
            if (currentMousePosition.X < pixelsFromEdge && currentMousePosition.Y > bgRectangle.Top)
            {
                if (scrollPosition > 0)
                {
                    scrollPosition -= scrollSpeed;
                    foreach (Card card in cards)
                    {
                        if (!card.onBoard)
                        {
                            card.drawRectangle.X += scrollSpeed;

                        }
                        card.origPos.X += scrollSpeed;
                    }
                }
            }
            //scroll right
            else if (currentMousePosition.X > bgRectangle.Right - pixelsFromEdge && currentMousePosition.Y > bgRectangle.Top)
            {
                if (scrollPosition < 500)
                {
                    scrollPosition += scrollSpeed;
                    foreach (Card card in cards)
                    {
                        if (!card.onBoard)
                        {
                            card.drawRectangle.X -= scrollSpeed;

                        }
                        card.origPos.X -= scrollSpeed;
                    }
                }
            }
        }
        private void CheckForLeftButtonDown()
        {
            if (currentMouse.LeftButton == ButtonState.Pressed)
            {
                //if this Update() is a new click - store the mouse-down position
                if (oldMouse.LeftButton == ButtonState.Released)
                {
                    mouseDownPosition = currentMousePosition;
                }

                foreach (Card card in cards)
                {
                    if (card.drawRectangle.Contains(mouseDownPosition))
                    {
                        selectedCard = card;
                    }
                }

            }
        }

        private void CheckForLeftButtonRelease()
        {
            //if the user just released the mousebutton - set _isDragging to false, and check if we should add the tile to the board
            if (oldMouse.LeftButton == ButtonState.Pressed && currentMouse.LeftButton == ButtonState.Released && selectedCard != null)
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
                selectedCard = null;
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
