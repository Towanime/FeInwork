using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FeInwork.core.collision;
using FeInwork.core.collision.bodies;
using FeInwork.core.util;
using FeInwork.Core.Util;

namespace FeInwork.core.managers
{
    public class CollisionManager
    {
        private static CollisionManager instance;
        private XboxHashSet<CollisionBody> bodyGlobalSet;
        private List<CollisionBody> bodyGroupList;
        private XboxHashSet<CollisionBody> bodyGroupHashSet;
        private SpriteFont font;
        private PrimitiveBatch primitiveBatch;
        /// <summary>
        /// Diccionario que contiene pares de QuadTreeNode con la capa correspodiente,
        /// cada nodo se separa en nodos mas pequeños posteriormente
        /// </summary>
        private Dictionary<float, QuadTreeNode> layersRoots = new Dictionary<float, QuadTreeNode>();
        /// <summary>
        /// Modo de dibujado actual de collision bodies
        /// </summary>
        private DRAW_TYPE currentDrawType;
        /// <summary>
        /// Capa actual de colisión que se está dibujando
        /// </summary>
        private float currentDrawingLayer;

        private int layerX;
        private int layerY;
        private int layerWidth;
        private int layerHeight;
        private int nodeDepth = 6;
        private int maxNodeCapacity = 7;
        private List<QuadTreeNode[]> nodeArrayPool;
        private List<XboxHashSet<CollisionBody>> bodySetPool;
        private string layerText = "DRAW ALL LAYERS";

        /// <summary>
        /// Modo de dibujado de collision bodies
        /// </summary>
        private enum DRAW_TYPE
        {
            DRAW_ALL,
            DRAW_ONE,
            DRAW_NONE
        }

        private CollisionManager()
        {
            this.currentDrawType = DRAW_TYPE.DRAW_ALL;
            this.bodyGlobalSet = new XboxHashSet<CollisionBody>();
            this.bodyGroupList = new List<CollisionBody>();
            this.bodyGroupHashSet = new XboxHashSet<CollisionBody>();
            this.primitiveBatch = new PrimitiveBatch(Program.GAME.GraphicsDevice);
            font = Program.GAME.Content.Load<SpriteFont>("MenuFont");

            int poolSize = (int)Math.Pow(4, nodeDepth - 2);

            this.nodeArrayPool = new List<QuadTreeNode[]>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                QuadTreeNode[] array = new QuadTreeNode[4];
                array[0] = new QuadTreeNode();
                array[1] = new QuadTreeNode();
                array[2] = new QuadTreeNode();
                array[3] = new QuadTreeNode();
                this.nodeArrayPool.Add(array);
            }

            this.bodySetPool = new List<XboxHashSet<CollisionBody>>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                this.bodySetPool.Add(new XboxHashSet<CollisionBody>(maxNodeCapacity + 1));
            }
        }

        public QuadTreeNode[] obtainNodeArray()
        {
            if (this.nodeArrayPool.Count > 0)
            {
                QuadTreeNode[] array = this.nodeArrayPool[0];
                nodeArrayPool.RemoveAt(0);
                return array;
            }
            else
            {
                QuadTreeNode[] array = null;
                for (int i = 0; i < 10; i++)
                {
                    array = new QuadTreeNode[4];
                    array[0] = new QuadTreeNode();
                    array[1] = new QuadTreeNode();
                    array[2] = new QuadTreeNode();
                    array[3] = new QuadTreeNode();

                    if (i < 9) nodeArrayPool.Add(array);
                }
                return array;
            }
        }

        public XboxHashSet<CollisionBody> obtainCollisionBodySet()
        {
            if (this.bodySetPool.Count > 0)
            {
                XboxHashSet<CollisionBody> set = this.bodySetPool[0];
                bodySetPool.RemoveAt(0);
                return set;
            }
            else
            {
                XboxHashSet<CollisionBody> set = null;
                for (int i = 0; i < 10; i++)
                {
                    set = new XboxHashSet<CollisionBody>(this.maxNodeCapacity + 1);
                    if (i < 9) bodySetPool.Add(set);
                }
                return set;
            }
        }

        public void redeemNodeArray(QuadTreeNode[] array)
        {
            this.nodeArrayPool.Add(array);
        }

        public void redeemCollisionBodySet(XboxHashSet<CollisionBody> set)
        {
            set.Clear();
            this.bodySetPool.Add(set);
        }

        public void resetState(int layerX, int layerY, int layerWidth, int layerHeight)
        {
            foreach (CollisionBody body in this.bodyGlobalSet)
            {
                body.lastBigNode.depth = 0;
            }

            foreach (float key in this.layersRoots.Keys)
            {
                this.layersRoots[key].Delete();
            }

            this.bodyGlobalSet.Clear();
            this.layersRoots.Clear();
            this.layerX = layerX;
            this.layerY = layerY;
            this.layerHeight = layerHeight;
            this.layerWidth = layerWidth;
        }

        /// <summary>
        /// Agrega un CollisionBody tanto a la lista global de bodies
        /// como a su capa respectiva
        /// </summary>
        /// <param name="body">CollisionBody a agregar</param>
        public void addContainer(CollisionBody body)
        {
            if (!this.layersRoots.ContainsKey(body.Layer))
            {
                QuadTreeNode root = new QuadTreeNode();
                root.reset(/*null, */layerX, layerY, layerWidth, layerHeight, 1, this.maxNodeCapacity, this.nodeDepth);
                this.layersRoots.Add(body.Layer, root);
            }

            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            body.BuildAABB(out minX, out minY, out maxX, out maxY);

            this.layersRoots[body.Layer].AddObject(body, minX, minY, maxX, maxY);
            this.bodyGlobalSet.Add(body);
        }

        /// <summary>
        /// Remueve un CollisionBody tanto de la lista global de bodies
        /// como de su capa respectiva
        /// </summary>
        /// <param name="body">CollisionBody a remover</param>
        public void removeContainer(CollisionBody body)
        {            
            QuadTreeNode root;
            if (this.layersRoots.TryGetValue(body.Layer, out root))
            {
                float minX = 0, minY = 0, maxX = 0, maxY = 0;
                body.BuildAABB(out minX, out minY, out maxX, out maxY);
                root.RemoveObject(body, minX, minY, maxX, maxY);
            }

            this.bodyGlobalSet.Remove(body);
        }

        /// <summary>
        /// Obtiene todos los CollisionBody que se encuentren en rango
        /// de un CollisionBody especifico para así poder minimizar
        /// los calculos de colisión
        /// </summary>
        /// <param name="body">Body utilizado para la busqueda</param>
        /// <returns>Lista de bodies en rango</returns>
        public XboxHashSet<CollisionBody> GetBodyGroup(CollisionBody body)
        {
            //this.bodyGroupList.Clear();
            this.bodyGroupHashSet.Clear();

            QuadTreeNode root;
            if (this.layersRoots.TryGetValue(body.Layer, out root))
            {
                float minX = 0, minY = 0, maxX = 0, maxY = 0;
                body.BuildAABB(out minX, out minY, out maxX, out maxY);

                if (body.lastBigNode.depth == 0)
                {
                    root.GetCollidableObjects(this.bodyGroupHashSet, body, minX, minY, maxX, maxY);
                }
                else
                {
                    body.lastBigNode.GetCollidableObjects(this.bodyGroupHashSet, body, minX, minY, maxX, maxY);
                }
            }
            return this.bodyGroupHashSet;
        }

        /// <summary>
        /// Obtiene todos los CollisionBody que se encuentren en rango
        /// de bounding box en una capa especifica para así poder minimizar
        /// los calculos de colisión
        /// </summary>
        /// <param name="boundingBox">Area o Rango utilizado para intersectar los cuerpos</param>
        /// <param name="layer">Capa en la cual se buscará</param>
        /// <returns>Lista de bodies en rango</returns>
        public XboxHashSet<CollisionBody> GetBodyGroup(Rectangle boundingBox, float layer)
        {
            //this.bodyGroupList.Clear();
            this.bodyGroupHashSet.Clear();

            QuadTreeNode root;
            if (this.layersRoots.TryGetValue(layer, out root))
            {
                root.GetCollidableObjects(this.bodyGroupHashSet, ref boundingBox);
                /*foreach (CollisionBody bodyFromHash in this.bodyGroupHashSet)
                {
                    this.bodyGroupList.Add(bodyFromHash);
                }*/
            }
            return this.bodyGroupHashSet;
        }

        public XboxHashSet<CollisionBody> BodyGlobalSet
        {
            get
            {
                return bodyGlobalSet;
            }
        }

        /*public XboxHashSet<CollisionBody> BodyList
        {
            get
            {
                XboxHashSet<CollisionBody> bodyList = new XboxHashSet<CollisionBody>();
                foreach (float layer in layersRoots.Keys)
                {
                    bodyList = layersRoots[layer].GetObjectList(bodyList);
                }
                return bodyList;
            }
        }*/

        /// <summary>
        /// Cambia el modo de dibujado de CollisionBodies
        /// </summary>
        public void changeDrawingLayer()
        {
            if (currentDrawType == DRAW_TYPE.DRAW_ONE)
            {
                bool isGreaterThan = false;
                Dictionary<float, QuadTreeNode>.KeyCollection keyColl = this.layersRoots.Keys;
                foreach (float key in keyColl)
                {
                    if (key > this.currentDrawingLayer)
                    {
                        this.currentDrawingLayer = key;                        
                        layerText = "Collisions: DRAWING LAYER " + currentDrawingLayer.ToString();
                        isGreaterThan = true;
                        break;
                    }
                }

                if (!isGreaterThan)
                {
                    this.currentDrawingLayer = 0.0f;
                    layerText = "Collisions: DRAW ALL LAYERS";
                    this.currentDrawType = DRAW_TYPE.DRAW_ALL;
                }
            }
            else if (currentDrawType == DRAW_TYPE.DRAW_ALL)
            {
                layerText = "Collisions: DRAW ALL LAYERS";
                this.currentDrawType = DRAW_TYPE.DRAW_NONE;
            }
            else if (currentDrawType == DRAW_TYPE.DRAW_NONE)
            {
                layerText = "Collisions: NOT DRAWING ANY LAYER";
                this.currentDrawType = DRAW_TYPE.DRAW_ONE;
            }
        }

        public static CollisionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CollisionManager();
                }
                return instance;
            }
        }

        public PrimitiveBatch PrimitiveBatch
        {
            get { return primitiveBatch; }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchDebugHud();

            sb.DrawString(font, layerText, new Vector2(10, Program.GAME.ResolutionHeight - 40), Color.White, 0, Vector2.Zero, new Vector2(0.8f, 0.8f), SpriteEffects.None, 0);

            if (currentDrawType == DRAW_TYPE.DRAW_NONE)
            {
                return;
            }

            XboxHashSet<CollisionBody> bodySet = null;

            if (currentDrawType == DRAW_TYPE.DRAW_ONE)
            {
                if (!this.layersRoots.ContainsKey(currentDrawingLayer)) return;
                primitiveBatch.Begin(PrimitiveType.LineList, Program.GAME.Camera.Transform);

                this.layersRoots[currentDrawingLayer].Draw(primitiveBatch);
                bodySet = this.layersRoots[currentDrawingLayer].GetObjectList();
                foreach (CollisionBody body in bodySet)
                {
                    if (body.Layer == currentDrawingLayer)
                    {
                        body.Draw(primitiveBatch);
                    }
                }

                primitiveBatch.End();
            }
            else if (currentDrawType == DRAW_TYPE.DRAW_ALL)
            {
                bodySet = this.BodyGlobalSet;
                primitiveBatch.Begin(PrimitiveType.LineList, Program.GAME.Camera.Transform);

                foreach (CollisionBody body in bodySet)
                {
                    body.Draw(primitiveBatch);
                }

                primitiveBatch.End();
            }
        }

    }
}
