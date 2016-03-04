using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Scene;

namespace IrrGame.IrrFPS
{
    public static class CIrrOcclusion
    {
        public static SceneManager sceneManager;
        public static TriangleSelector triangleSelector;

        public static bool Occlusion(Line3Df ray)
        {
            Vector3Df collisionPoint;
            Triangle3Df collisionTri;
            SceneNode collisionNode;

            if (triangleSelector != null && sceneManager != null)
                return sceneManager.SceneCollisionManager.GetCollisionPoint(ray, triangleSelector, out collisionPoint, out collisionTri,out collisionNode);

            return true;
        }
    }
}
