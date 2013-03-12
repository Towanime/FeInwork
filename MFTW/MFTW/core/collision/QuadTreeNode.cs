using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.util;
using FeInwork.core.collision.bodies;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;

namespace FeInwork.core.collision
{
    public class QuadTreeNode
    {
        private int maxBodyCount;
        public int depth;
        private float x1;
        private float y1;
        private float x2;
        private float y2;
        private float width;
        private float height;
        //private QuadTreeNode parent;
        private QuadTreeNode[] children;
        private XboxHashSet<CollisionBody> items;
        private Vector2 pointA;
        private Vector2 pointB;
        private Vector2 pointC;
        private Vector2 pointD;
        private int maxDepth;

        public void reset(/*QuadTreeNode parent, */float x, float y, float width, float height, int depth, int maxBodyCount, int maxDepth)
        {
            this.depth = depth;
            this.x1 = x;
            this.y1 = y;
            this.x2 = x1 + width;
            this.y2 = y1 + height;
            this.width = width;
            this.height = height;
            //this.parent = null;
            this.children = null;
            this.items = Program.GAME.CollisionManager.obtainCollisionBodySet();
            this.pointA = new Vector2();
            this.pointA.X = this.x1; 
            this.pointA.Y = this.y1;
            this.pointB = new Vector2();
            this.pointB.X = this.x1 + this.width;
            this.pointB.Y = this.y1;
            this.pointC = new Vector2();
            this.pointC.X = this.x1 + this.width;
            this.pointC.Y = this.y1 + this.height;
            this.pointD = new Vector2();
            this.pointD.X = this.x1;
            this.pointD.Y = this.y1 + this.height;
            this.maxBodyCount = maxBodyCount;
            this.maxDepth = maxDepth;
        }

        public void SubDivide()
        {
            int newLevel = depth + 1;
            this.children = Program.GAME.CollisionManager.obtainNodeArray();
            this.children[0].reset(/*this, */this.x1, this.y1, this.width / 2, this.height / 2, newLevel, this.maxBodyCount, this.maxDepth);
            this.children[1].reset(/*this, */this.x1 + (this.width / 2), this.y1, this.width / 2, this.height / 2, newLevel, this.maxBodyCount, this.maxDepth);
            this.children[2].reset(/*this, */this.x1, this.y1 + (this.height / 2), this.width / 2, this.height / 2, newLevel, this.maxBodyCount, this.maxDepth);
            this.children[3].reset(/*this, */this.x1 + (this.width / 2), this.y1 + (this.height / 2), this.width / 2, this.height / 2, newLevel, this.maxBodyCount, this.maxDepth);

            foreach (CollisionBody body in items)
            {
                float minX = 0, minY = 0, maxX = 0, maxY = 0;
                body.BuildAABB(out minX, out minY, out maxX, out maxY);

                for (int i = 0; i < 4; i++)
                {
                    bool inside = true;
                    QuadTreeNode child = this.children[i];

                    if (child.x1 > maxX || child.x2 < minX || child.y1 > maxY || child.y2 < minY) inside = false;

                    if (inside)
                    {
                        child.AddObject(body, minX, minY, maxX, maxY);
                    }
                }
            }
        }

        public void Delete()
        {
            if (this.children != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.children[i].Delete();
                }

                Program.GAME.CollisionManager.redeemNodeArray(this.children);
                this.children = null;
            }

            Program.GAME.CollisionManager.redeemCollisionBodySet(this.items);
        }

        public void GetCollidableObjects(XboxHashSet<CollisionBody> list, CollisionBody container, float minX, float minY, float maxX, float maxY)
        {
            if (children == null)
            {
                //if (this.containerList.Contains(container))
                //{
                /*foreach (CollisionBody bodyFromList in this.items)
                {
                    list.Add(bodyFromList);
                }*/
                list.MergeInto(this.items);
                //}
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    bool inside = true;
                    QuadTreeNode child = this.children[i];

                    if (child.x1 > maxX || child.x2 < minX || child.y1 > maxY || child.y2 < minY) inside = false;

                    if (inside)
                    {
                        if (child.children == null)
                        {                            
                            /*foreach (CollisionBody bodyFromList in child.items)
                            {
                                list.Add(bodyFromList);
                            }*/
                            list.MergeInto(child.items);
                        }
                        else
                        {
                            child.GetCollidableObjects(list, container);
                        }
                    }
                }
            }
        }

        public void GetCollidableObjects(XboxHashSet<CollisionBody> list, CollisionBody container)
        {
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            container.BuildAABB(out minX, out minY, out maxX, out maxY);
            this.GetCollidableObjects(list, container, minX, minY, maxX, maxY);
        }

        public void GetCollidableObjects(XboxHashSet<CollisionBody> list, CollisionBody container, ref Rectangle boundingBox)
        {
            float minX = boundingBox.X, minY = boundingBox.Y, maxX = boundingBox.X + boundingBox.Width, maxY = boundingBox.Y + boundingBox.Height;
            this.GetCollidableObjects(list, container, minX, minY, maxX, maxY);
        }

        public void GetCollidableObjects(XboxHashSet<CollisionBody> list, ref Rectangle boundingBox)
        {
            if (children == null)
            {
                foreach (CollisionBody bodyFromList in this.items)
                {
                    if (!boundingBox.Intersects(bodyFromList.AABB)) continue;
                    list.Add(bodyFromList);
                }
            }
            else
            {
                float minX = boundingBox.X, minY = boundingBox.Y, maxX = boundingBox.X + boundingBox.Width, maxY = boundingBox.Y + boundingBox.Height;

                for (int i = 0; i < 4; i++)
                {
                    bool inside = true;
                    QuadTreeNode child = this.children[i];

                    if (child.x1 > maxX || child.x2 < minX || child.y1 > maxY || child.y2 < minY) inside = false;

                    if (inside)
                    {
                        child.GetCollidableObjects(list, ref boundingBox);
                    }
                }
            }
        }

        public void AddObject(CollisionBody obj, float minX, float minY, float maxX, float maxY)
        {
            this.items.Add(obj);

            if (this.depth == 1)
            {
                obj.lastBigNode = this;
            }

            if (children == null)
            {
                if (this.items.Count > this.maxBodyCount && this.depth < this.maxDepth)
                {
                    this.SubDivide();
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    bool inside = true;
                    QuadTreeNode child = this.children[i];

                    if (child.x1 > maxX || child.x2 < minX || child.y1 > maxY || child.y2 < minY) inside = false;

                    if (inside)
                    {
                        if (child.x2 >= maxX
                            && child.x1 <= minX
                            && child.y2 >= maxY
                            && child.y1 <= minY)
                        {
                            obj.lastBigNode = child;
                        }

                        this.children[i].AddObject(obj, minX, minY, maxX, maxY);
                    }
                }
            }
        }

        public void AddObject(CollisionBody obj, ref Rectangle boundingBox)
        {
            float minX = boundingBox.X, minY = boundingBox.Y, maxX = boundingBox.X + boundingBox.Width, maxY = boundingBox.Y + boundingBox.Height;
            this.AddObject(obj, minX, minY, maxX, maxY);
        }

        public void RemoveObject(CollisionBody obj, float minX, float minY, float maxX, float maxY)
        {
            this.items.Remove(obj);

            if (children != null)
            {
                if (this.items.Count < maxBodyCount - 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this.children[i].Delete();
                    }

                    foreach (CollisionBody body in this.items)
                    {
                        if (body.lastBigNode.depth > this.depth) body.lastBigNode = this;
                    }

                    Program.GAME.CollisionManager.redeemNodeArray(this.children);
                    this.children = null;
                    return;
                }

                for (int i = 0; i < 4; i++)
                {
                    bool inside = true;
                    QuadTreeNode child = this.children[i];

                    if (child.x1 > maxX || child.x2 < minX || child.y1 > maxY || child.y2 < minY) inside = false;

                    if (inside)
                    {
                        child.RemoveObject(obj, minX, minY, maxX, maxY);
                    }
                }
            }
        }

        public void RemoveObject(CollisionBody obj, ref Rectangle boundingBox)
        {
            float minX = boundingBox.X, minY = boundingBox.Y, maxX = boundingBox.X + boundingBox.Width, maxY = boundingBox.Y + boundingBox.Height;
            this.RemoveObject(obj, minX, minY, maxX, maxY);
        }

        /*public void RemoveObjectReverse(CollisionBody obj)
        {
            QuadTreeNode parentNode = this.parent;

            while (parentNode != null)
            {
                parentNode.items.Remove(obj);
                parentNode = parentNode.parent;
            }
        }

        public void ClearObjectList()
        {
            if (subNodes == null)
            {
                this.containerList.Clear();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    this.subNodes[i].ClearObjectList();
                }
            }
        }*/

        public XboxHashSet<CollisionBody> GetObjectList()
        {
            return this.items;
        }

        public void Draw(PrimitiveBatch primitiveBatch)
        {
            primitiveBatch.AddVertex(pointA, Color.Green);
            primitiveBatch.AddVertex(pointB, Color.Green);
            primitiveBatch.AddVertex(pointB, Color.Green);
            primitiveBatch.AddVertex(pointC, Color.Green);
            primitiveBatch.AddVertex(pointC, Color.Green);
            primitiveBatch.AddVertex(pointD, Color.Green);
            primitiveBatch.AddVertex(pointD, Color.Green);
            primitiveBatch.AddVertex(pointA, Color.Green);

            if (children != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.children[i].Draw(primitiveBatch);
                }
            }
        }
    }
}
