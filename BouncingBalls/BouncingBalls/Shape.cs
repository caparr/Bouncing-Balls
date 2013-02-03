using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BouncingBalls
{
    public class Shape
    {
        public Model model { get; private set; }
        
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public Matrix world { get; private set; }
        
        
        public float alpha { get; set; }
        public float size { get; set; }
        public Vector3 colour { get; set; }

        public Shape(Model newModel, Vector3 newPosition, Vector3 newVelocity, float newAlpha, float newSize, Vector3 newColour)
        {            
            model = newModel;
            position = newPosition;
            velocity = newVelocity;
            size = newSize;
            alpha = newAlpha;

            colour = newColour;
        }

        public void Move()
        {
            position += velocity;
            
        }

        public void DrawModel(Matrix view, Matrix projection)
        {
            world = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    //effect.AmbientLightColor = bubbleColour[colour];
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.LightingEnabled = true;
                    effect.Alpha = alpha;
                    effect.DiffuseColor = colour;
                }
                mesh.Draw();
            }
        }
    }
}
