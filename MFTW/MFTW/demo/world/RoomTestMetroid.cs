using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Util;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.renderers;
using FeInwork.FeInwork.renderers.animation;
using FeInwork.FeInwork.components;
using FeInwork.FeInwork.entities;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.actions;
using FeInwork.FeInwork.draweffects;
using FeInwork.collision.responses;
using FeInwork.core.Base;
using FeInwork.core.collision;
using FeInwork.core.collision.bodies;
using FeInwork.core.physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FeInwork.FeInwork.world
{
    public class AreaTestMetroid : AreaEntity
    {
        int blockSize = GameConstants.BLOCK_SIZE;

        public AreaTestMetroid()
            : base(64, 16, "AreaTestMetroid", "")
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();

            List<CollisionBody> bodyList = new List<CollisionBody>();
            int i = 0;
            // poligonos de arriba
            List<Vector2> polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 2, blockSize * 7), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 4, blockSize * 2), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 7, blockSize * 2), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 3, GameConstants.CAMERA_INNER_BORDER + blockSize * 3));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 3, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 9, blockSize * 3), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 12, blockSize * 5), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 15, blockSize * 5), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize * 4));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 4, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 21, blockSize * 2), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 26, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 29, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 33, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 38, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 48, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 3, GameConstants.CAMERA_INNER_BORDER + blockSize * 3));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 3, GameConstants.CAMERA_INNER_BORDER + 0));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 59, blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            // poligonos de abajo
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 4, blockSize * 12), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize * 2));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 2, GameConstants.CAMERA_INNER_BORDER + blockSize * 2));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 6, blockSize * 13), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 10, blockSize * 14), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 22, blockSize * 14), false, GameLayers.MIDDLE_PLAY_AREA));
            polyPoints = new List<Vector2>();
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + 0));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize, GameConstants.CAMERA_INNER_BORDER + blockSize));
            polyPoints.Add(new Vector2(GameConstants.CAMERA_INNER_BORDER + 0, GameConstants.CAMERA_INNER_BORDER + blockSize));
            bodyList.Add(ShapeFactory.CreatePolygon("Polygon" + ++i, this, true, false, polyPoints, new Vector2(blockSize * 58, blockSize * 12), false, GameLayers.MIDDLE_PLAY_AREA));
            i = 0;
            // bloques de arriba
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 2, blockSize * 8, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 0, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 2, blockSize * 3, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 2, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 1, blockSize * 4, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 2, GameConstants.CAMERA_INNER_BORDER + blockSize * 3), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 8, blockSize * 2, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 4, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 4, blockSize, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 8, GameConstants.CAMERA_INNER_BORDER + blockSize * 2), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 5, blockSize, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 16, GameConstants.CAMERA_INNER_BORDER + blockSize * 5), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 5, blockSize * 2, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 21, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 38, blockSize * 1, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 26, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 3, blockSize * 1, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 30, GameConstants.CAMERA_INNER_BORDER + blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 9, blockSize * 1, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 39, GameConstants.CAMERA_INNER_BORDER + blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            bodyList.Add(ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 2, blockSize * 3, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 62, GameConstants.CAMERA_INNER_BORDER + blockSize * 1), false, GameLayers.MIDDLE_PLAY_AREA));
            // bloques de abajo
            List<AreaCollisionBodyWrapper> bodyWrapperList = new List<AreaCollisionBodyWrapper>();
            AreaCollisionBodyWrapper wrapper = null;
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 4, blockSize * 4, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 0, GameConstants.CAMERA_INNER_BORDER + blockSize * 12), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 2, blockSize * 3, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 4, GameConstants.CAMERA_INNER_BORDER + blockSize * 13), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 5, blockSize, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 6, GameConstants.CAMERA_INNER_BORDER + blockSize * 15), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 11, blockSize * 2, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 11, GameConstants.CAMERA_INNER_BORDER + blockSize * 14), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 36, blockSize, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 22, GameConstants.CAMERA_INNER_BORDER + blockSize * 15), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize, blockSize * 3, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 58, GameConstants.CAMERA_INNER_BORDER + blockSize * 13), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);
            wrapper = new AreaCollisionBodyWrapper(this, ShapeFactory.CreateRectangle("Block" + ++i, this, true, false, blockSize * 5, blockSize * 4, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 59, GameConstants.CAMERA_INNER_BORDER + blockSize * 12), false, GameLayers.MIDDLE_PLAY_AREA));
            wrapper.addBoolProperty(EntityProperty.Earth, true);
            bodyWrapperList.Add(wrapper);            
            CollisionComponent.addBodies(bodyList);
            CollisionComponent.addWrappers(bodyWrapperList);
            EventManager.Instance.addCollisionListener(this, new SurfaceCollisionResponse(this));

            PhysicsData boxPhysics = new PhysicsData();
            boxPhysics.MinimumVelocity = new Vector2(-500, -5000);
            boxPhysics.MaximumVelocity = new Vector2(500, 2000);
            boxPhysics.Weight = 15;
            boxPhysics.RunningImpulse = 30000;
            boxPhysics.AirImpulse = 15000;
            boxPhysics.JumpImpulse = 1200000;

            Dictionary<ElementType, float> boxElementMultipliers =
                UtilMethods.getDamageMultipliers(DamageMultipliers.NORMAL_DAMAGE, DamageMultipliers.HIGH_DAMAGE, 
                DamageMultipliers.NORMAL_DAMAGE, DamageMultipliers.NORMAL_DAMAGE, DamageMultipliers.NORMAL_DAMAGE);
            
            // Conjunto de bloques que desaparecen visualmente cuando 
            // se activa un trigger
            DrawableEntity fadeWall = new DrawableEntity();
            fadeWall.addDrawEffect(new FadeEffect(fadeWall, 0.02f, 0.4f, 1.0f));

            // Trigger utilizado para desaparecer 
            // visualmente una entidad cuando se entra
            // en el rango del trigger
            BaseTrigger baseTrigger = new BaseTrigger();
            Box cameraTriggerBody = ShapeFactory.CreateRectangle("trigger", baseTrigger, false, false, blockSize * 9, blockSize * 5, new Vector2(GameConstants.CAMERA_INNER_BORDER + blockSize * 12, GameConstants.CAMERA_INNER_BORDER + blockSize * 0), false);
            baseTrigger.Zone = cameraTriggerBody;
            FadeInTrigger ct = new FadeInTrigger(baseTrigger, fadeWall);
            this.areaItems.Add(baseTrigger);
        }
    }
}
