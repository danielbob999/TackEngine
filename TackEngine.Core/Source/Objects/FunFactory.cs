using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Objects;

namespace TackEngine.Core.Source.Objects {
    public static class FunFactory {
        public static void CreateCar(Vector2f position) {
            // Car body
            TackObject body = TackObject.Create("CarBody", position);
            body.Scale = new Vector2f(250f, 84f);
            body.Rotation = 0f;
            body.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White });
            body.AddComponent(new RectanglePhysicsComponent(1f, false, false, false, 1, 0f));

            // Back Wheel
            TackObject wheel1 = TackObject.Create("CarBackWheel", new Vector2f(0, 0));
            wheel1.Scale = new Vector2f(38, 38);
            wheel1.Rotation = 0f;
            wheel1.LocalPosition = new Vector2f(position.X - 80, position.Y - 45);
            wheel1.AddComponent(new SpriteRendererComponent() { Colour = new Colour4b(0, 0, 0, 255) });
            wheel1.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));

            // Wheel 2
            TackObject wheel2 = TackObject.Create("CarFrontWheel", new Vector2f(0, 0));
            wheel2.Scale = new Vector2f(38, 38);
            wheel2.Rotation = 0f;
            wheel2.LocalPosition = new Vector2f(position.X + 80, position.Y - 45);
            wheel2.AddComponent(new SpriteRendererComponent() { Colour = new Colour4b(0, 0, 0, 255) });
            wheel2.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));
        }
    }
}
