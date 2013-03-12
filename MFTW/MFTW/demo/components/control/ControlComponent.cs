using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Input;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.util;
using FeInwork.core.Base;
using FeInwork.core.managers;
using FeInwork.core.util;
using FeInwork.FeInwork.world;

namespace FeInwork.FeInwork.components
{
    public class ControlComponent : BaseControlComponent
    {

        public ControlComponent(IEntity owner)
        {
            this.owner = owner;
            initialize();
        }

        // no es necesario llamar a este metodo pues BaseComponent lo llama en su 
        // unico constructor
        public override void initialize()
        {
            //carga sus valores y crap
            //Program.GAME.ComponentManager.addComponent(this);
            // nuevo, agregar al control manager
            ControlManager.Instance.addController(this);
           // EventManager.Instance.addListener(EventType.CONTROL_ENABLE_STATE_CHANGE_EVENT, this);
            isEnabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            //Si se presiona A y no hay delay se genera el ataque
            if (InputManager.isNewPressKeyOrButton(Keys.Escape, Buttons.Back))
            {
                Program.GAME.Exit();
            }

            if (!owner.getState(EntityState.Dead)) //Si no está muerto
            {
                //Si se presiona A y no hay delay se genera el ataque
                if (InputManager.isNewPressKeyOrButton(Keys.A, Buttons.X) && !this.owner.getState(util.EntityState.Attacking))
                {
                    EventManager.Instance.fireEvent(PhysicalAttackEvent.Create(this.owner));
                }

                if (InputManager.isCurPressKeyOrButton(Keys.S, Buttons.Y))
                {
                    EventManager.Instance.fireEvent(MoveEvent.Create(this, MoveEvent.MOVE_TYPE.ACTION));
                }

                if (!this.owner.getState(util.EntityState.Attacking)) //Si no está atacando
                {
                    if (InputManager.isCurPressKeyOrButton(Keys.Right, Buttons.LeftThumbstickRight))
                    {
                        if (owner.getState(EntityState.IsAvailable))
                        {
                            owner.changeState(EntityState.FacingRight, true, true);
                        }
                        owner.changeIntProperty(EntityProperty.HorizontalDirection, 1, true);
                        EventManager.Instance.fireEvent(MoveEvent.Create(this, MoveEvent.MOVE_TYPE.WALK));
                        owner.changeState(EntityState.Running, true, true);
                    }
                    else if (InputManager.isCurPressKeyOrButton(Keys.Left, Buttons.LeftThumbstickLeft))
                    {
                        if (owner.getState(EntityState.IsAvailable))
                        {
                            owner.changeState(EntityState.FacingRight, false, true);
                        }

                        owner.changeIntProperty(EntityProperty.HorizontalDirection, -1, true);
                        EventManager.Instance.fireEvent(MoveEvent.Create(this, MoveEvent.MOVE_TYPE.WALK));
                        owner.changeState(EntityState.Running, true, true);
                    }
                }
            }


            if (InputManager.isNewPressKeyOrButton(Keys.Z, Buttons.A))
            {
                EventManager.Instance.fireEvent(MoveEvent.Create(this, MoveEvent.MOVE_TYPE.JUMP));
            }
            if (InputManager.isCurPressKeyOrButton(Keys.Up, Buttons.RightThumbstickUp))
            {
                Program.GAME.Camera.moveY(-10);
            }
            if (InputManager.isCurPressKeyOrButton(Keys.Down, Buttons.RightThumbstickDown))
            {
                Program.GAME.Camera.moveY(10);
            }
            if(InputManager.isCurPressKeyOrButton(Keys.NumPad8, Buttons.RightThumbstickRight))
            {
                Program.GAME.Camera.zoomIn();
            }
            if (InputManager.isCurPressKeyOrButton(Keys.NumPad2, Buttons.RightThumbstickLeft))
            {
                Program.GAME.Camera.zoomOut();
            }
            // ahora para que regrese al zoom normal
            if (InputManager.isOldPressKeyOrButton(Keys.NumPad8, Buttons.RightThumbstickUp) || InputManager.isOldPressKeyOrButton(Keys.NumPad2, Buttons.RightThumbstickDown))
            {
                Program.GAME.Camera.resetZoom();
            }

            if(InputManager.isNewPress(Keys.D)){
                DialogParameters[] pars = new DialogParameters[5];
                pars[0] = new DialogParameters(false);
                pars[0].VerticalAlign = GameConstants.VerticalAlign.VerticalCenter;
                pars[0].HorizontalAlign = GameConstants.HorizontalAlign.Right;
                //pars[0].IsAdaptToText = true;
                pars[1] = new DialogParameters(false);
                
                //
                pars[2] = new DialogParameters(false);
                pars[2].VerticalAlign = GameConstants.VerticalAlign.FreeY;
                pars[2].HorizontalAlign = GameConstants.HorizontalAlign.FreeX;
                pars[2].IsAdaptToText = true;
                pars[2].FreeX = 130;
                pars[2].FreeY = 205;
                // bloqueo por 5 segundos
                pars[3] = new DialogParameters(false);
                pars[3].BlockSeconds = 3;
                // bloquea y continua automaticamente
                pars[4] = new DialogParameters(false);
                pars[4].VerticalAlign = GameConstants.VerticalAlign.VerticalCenter;
                pars[4].HorizontalAlign = GameConstants.HorizontalAlign.HorizontalCenter;
                pars[4].IsAdaptToText = true;
                pars[4].BlockSeconds = 2;
                pars[4].IsAutoNextAfterBlock = true;
                //pars[4].CustomAssetName = "teststage/goemonAvatar";
                pars[4].AvatarPixelsToShow = 160;
                DialogManager.Instance.beginDialog("string[] words = text.Split(' '); $e StringBuilder sb = new StringBuilder();  $0 float linewidth = 0f;  float maxLine = 250f;  //a bit smaller than the box so you can have some padding...etc $2 float spaceWidth = spriteFont.MeasureString().X; foreach (string word in words) Vector2 $3 size = spriteFont.MeasureString(word) if (linewidth + size.X < 250)   {     sb.Append(word);   linewidth += size.X + spaceWidth;   }    else   {     sb.Append( + word + ); $4    linewidth = size.X + spaceWidth;  } } return sb.ToString();", pars);
            }
        }

        // esto es temporal solo para darle control a yae
        public new IEntity Owner
        {
            set { this.owner = value; }
        }

    }

    
}
