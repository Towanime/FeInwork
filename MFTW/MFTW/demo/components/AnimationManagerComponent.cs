using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.core.collision.bodies;
using FeInwork.core.collision;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using System.Reflection;
using FeInwork.FeInwork.components;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.FeInwork.events;
using FeInwork.Core.Base;
using FeInwork.FeInwork.util;
using FeInwork.Core.Base.Animation;

namespace FeInwork.FeInwork.renderers.animation
{
    public class AnimationManagerComponent : BaseComponent, IUpdateableFE
    {
        private bool isEnabled;
        private bool isRepeating;
        private Animation currentAction;
        private Dictionary<int, Animation> actions;
        private List<ActiveBody> activeBodies;
        private BasicActionChooser actionChooser;
        private String creationCollisionInfo;
        private int iterationValue = 1;
        private int currentIteration = 0;
        private object[] ownerArray;

        public int IterationValue
        {
            get
            {
                return iterationValue;
            }
            set
            {
                iterationValue = value;
            }
        }

        public int CurrentIteration
        {
            get
            {
                return currentIteration;
            }
            set
            {
                currentIteration = value;
            }
        }

        public AnimationManagerComponent(IEntity owner, BasicActionChooser actionChooser, Animation defaultAnimation)
            : base(owner)
        {
            actions = new Dictionary<int, Animation>();
            this.owner = owner;
            activeBodies = new List<ActiveBody>();
            owner.changeIntProperty(EntityProperty.Action, ActionsList.Default, true);
            this.addAction(defaultAnimation);
            this.currentAction = defaultAnimation;
            this.actionChooser = actionChooser;
            initialize();
        }

        // no es necesario llamar a este metodo pues BaseComponent lo llama en su 
        // unico constructor
        public override void initialize()
        {
            //carga sus valores y crap
            Program.GAME.ComponentManager.addComponent(this);
            this.ownerArray = new object[] { this.owner };

            isEnabled = true;
        }

        public bool Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public void addAction(Animation animation)
        {
            actions.Add(animation.Id, animation);
        }

        public bool IsRepeating
        {
            get { return isRepeating; }
            set { isRepeating = value; }
        }

        /// <summary>
        /// Establece una acción de animación que se repite por defecto
        /// </summary>
        /// <param name="actionName">Id de la acción</param>
        public void setAction(int actionName)
        {
            setAction(actionName, false);
        }

        /// <summary>
        /// Establece una acción indicando si se repite o no al finalizar
        /// </summary>
        /// <param name="actionName">Id de la acción</param>
        /// <param name="isRepeating">Indica si se repite</param>
        public void setAction(int actionName, bool isRepeating)
        {
            if (currentAction.Id != actionName)
            {
                currentAction.resetFrames();
                actions.TryGetValue(actionName, out currentAction);
                creationCollisionInfo = currentAction.refillRemainingFrames();
                this.isRepeating = isRepeating;
            }
        }

        public void Update(GameTime gameTime)
        {
            //Establece la acción del pesonaje
            setAction(owner.getIntProperty(EntityProperty.Action));

            //Si está en modo Vanquish entonces no actua hasta que haya consumido
            //todos los frames correspondientes al valor de iteración            
            currentIteration--;
            
            if (currentIteration > 0)
            {
                return;
            }
            else
            {
                currentIteration = iterationValue;
            }

            String createBodiesInstructions = currentAction.consumeFrame(this.isRepeating) ?? creationCollisionInfo;
            creationCollisionInfo = null;

            if (!String.IsNullOrEmpty(createBodiesInstructions)) //Si se necesitan crear bodies
            {
                if (createBodiesInstructions.Equals("EOF"))
                {
                    owner.changeIntProperty(EntityProperty.Action, actionChooser.getNewAction(), false);
                    setAction(owner.getIntProperty(EntityProperty.Action));
                    createBodiesInstructions = currentAction.consumeFrame(this.isRepeating) ?? creationCollisionInfo;
                    creationCollisionInfo = null;
                    if (!String.IsNullOrEmpty(createBodiesInstructions)) createBodies(createBodiesInstructions);
                }
                else
                {
                    createBodies(createBodiesInstructions);
                }
            }

            for (int i = activeBodies.Count - 1; i >= 0; i--)
            {
                if (--activeBodies[i].timeAlive <= 0)
                {
                    AbstractCollisionComponent ownerCollisionComponent = this.owner.findByBaseClass<AbstractCollisionComponent>()[0];
                    ownerCollisionComponent.removeBody(activeBodies[i].Body);
                    EventManager.Instance.removeCollisionListenerForCollisionBody(activeBodies[i].Body);
                    activeBodies.RemoveAt(i);
                }
            }
            
        }

        public void createBodies(String createBodiesInstructions)
        {
            //Utiliza el separador ; para diferenciar entre bodies y responses
            String[] parts = createBodiesInstructions.Split(';');
            //part[0] contiene la definición de las shapes
            List<CollisionBody> bodiesList = ShapeFactory.CreateShapeFromString(parts[0], this.owner);
            //Response de las shapes
            String[] shapeResponses = parts[1].Split(',');

            for (int i = 0; i < shapeResponses.Length; i++)
            {
                if (shapeResponses[i].Length == 0) continue; //Si no contiene response a crear continua
                if (shapeResponses[i].Contains('!')) //Por reflection crea la response necesaria y la asocia a un target
                {
                    String[] responseAndTarget = shapeResponses[i].Split('!');
                    CollisionListener collisionListener = (CollisionListener)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(responseAndTarget[0]), this.ownerArray);
                    bodiesList[i].addCollisionListener(collisionListener);
                }
                else //Si no se especifica un target
                {
                    CollisionListener collisionListener = (CollisionListener)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(shapeResponses[i]), this.ownerArray);
                    EventManager.Instance.addCollisionListener(this.owner, collisionListener);
                }
            }

            String[] bodiesFrameLive = parts[2].Split(',');

            for (int i = 0; i < bodiesFrameLive.Length; i++) //Crea los bodies activos a mantener en memoria, con sus frames de vida
            {
                activeBodies.Add(new ActiveBody(bodiesList[i], Int32.Parse(bodiesFrameLive[i])));
            }

            for (int i = 0; i < bodiesList.Count; i++) //Le agrega todos los bodies al collision component del owner
            {
                AbstractCollisionComponent collisionComponent = this.owner.findByBaseClass<AbstractCollisionComponent>()[0];
                collisionComponent.addBody(bodiesList[i]);
            }
        }

        public AnimationFrame getCurrentAnimationFrame()
        {
            return currentAction.getCurrentFrame();
        }

        /// <summary>
        /// Una estructura que especifica un Body con un tiempo de vida definido
        /// </summary>
        public class ActiveBody
        {
            public CollisionBody Body;
            public int timeAlive;

            public ActiveBody(CollisionBody body, int time)
            {
                this.Body = body;
                this.timeAlive = time;
            }
        }

    }
}