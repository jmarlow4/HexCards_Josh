﻿using System;
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
        List<Card> cards;
        SpriteFont myFont;
        float scale;
        string drawText = "";
        Hexboard board;

        Vector2 currentMousePosition;
        Vector2 mouseDownPosition;
        bool isDragging = false;
        bool isTouchDragging = false;
        MouseState oldMouse, currentMouse;
        TouchCollection touchColl, oldTouch;
        Vector2 touchDownPosition;
        
        public PlayerHand(ContentManager cm, int screenWidth, int screenHeight, float scale, Hexboard board)
        {
            this.scale = scale;
            bg = cm.Load<Texture2D>("playerHandBG");
            int bgWidth = (int)(bg.Width * scale);
            int bgHeight = (int)(bg.Height * scale);
            
            for (int i = 0; i < 9; i++)
            {
                Card c = new Card(cm, scale, i, CardColor.Red, 40);
                cards.Add(c);
            }           
           
            myFont = cm.Load<SpriteFont>("Arial");
            this.board = board;
        }

        public void Draw(SpriteBatch sb)
        {
            //TODO: we'll want to iterate over a collection of cards instead of just one
            sb.DrawString(myFont, drawText, new Vector2(0, 0), Color.White);
            sb.Draw(bg, drawRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, .0001f);
            if (isDragging)
                card1.Draw(sb, new Point((int)(currentMousePosition.X - card1.hexWidth / 2 * scale), (int)(currentMousePosition.Y - card1.hexHeight / 2 * scale)));
            else if (isTouchDragging)
                card1.Draw(sb, new Point((int)(touchColl[0].Position.X - card1.hexWidth / 2 * scale), (int)(touchColl[0].Position.Y - 50 - card1.hexHeight / 2 * scale)));
            else if (card1.onBoard)
                card1.Draw(sb, card1.drawRectangle.Location);
            else
                card1.Draw(sb, origPos);
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

            //if (IsMouseInsideBoard())
            //    drawText = "inside board!";                
            //else
            //    drawText = "not inside board";

            oldMouse = currentMouse;
            oldTouch = touchColl;

            //drawText = mouse.Position.ToString();
            //card1.RealignCardElements();
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

                //if the mousedown was within the draggable tile 
                //and the mouse has been moved more than 10 pixels:
                //start dragging
                if ((mouseDownPosition - currentMousePosition).Length() > 10 && card1.Contains(mouseDownPosition))
                {
                    isDragging = true;
                }
            }
        }

        private void CheckForLeftButtonRelease()
        {
            //if the user just released the mousebutton - set _isDragging to false, and check if we should add the tile to the board
            if (oldMouse.LeftButton == ButtonState.Pressed && currentMouse.LeftButton == ButtonState.Released && isDragging)
            {
                isDragging = false;
                board.PlaceCard(card1);
                              
                if (IsMouseInsideBoard())
                {
                    drawText = "on board";
                }
                else
                    drawText = "off board";
            }
        }

        //These next two methods does the same things as the ones above but for touch
        private void CheckTouchDown()
        {
            if (touchColl.Count > 0)
            {
                if (oldTouch.Count == 0)
                    touchDownPosition = touchColl[0].Position;

                if ((touchDownPosition - touchColl[0].Position).Length() > 10 && card1.Contains(touchDownPosition))
                    isTouchDragging = true;
            }
        }
        private void CheckTouchRelease()
        {
            if (oldTouch.Count > 0 && touchColl.Count == 0 && isTouchDragging)
                isTouchDragging = false;
        }

        private bool IsMouseInsideBoard()
        {
           
                if (board.ContainsPoint(currentMousePosition))
                    return true;
                else
                    return false;
            
        }
    }
}
