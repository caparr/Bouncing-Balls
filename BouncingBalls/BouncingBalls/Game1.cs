using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BouncingBalls
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum part
        {
            ONE = 0,
            TWO
        }

        #region Camera
        private Vector3 thirdPersonReference;
        private Vector3 cameraPosition;
        private Vector3 lookAt;
        
        private Matrix view;
        private Matrix projection;        
        #endregion


        #region Required        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        part currentState;
        public static Random randomNums = new Random();
        KeyboardState lastKeyboardState;
        
        #endregion


        #region models
        
        SpriteFont font;
        Model cubeModel;
        Model ballModel;
        
        #endregion

                
        #region part one        
        
        Shape cube;
        Shape ball;
        
        private const float GRAVITY = -0.98f;
        private float cubeSize;
        private float ballRadius;
        private float timeSinceCollision;

        private Vector3 cubePosition;
        private Vector3 ballPosition;
        private Vector3 initialVelocity;

        private bool moveOn;
        private bool dragOn;
        private bool gravityOn;
        private bool kinecticOn;
        #endregion
        

        #region part two

        private Shape[] piecewisePoints;
        private Shape[] interpolationPoints;

        private Shape followThisBall;
        private int numberOfPoints;
        private int nextPoint;

        private int index;
        private int piecewiseSegments;
        private int interpolationSegments;

        private bool piecewise;
        private bool interpolation;
        private bool approximation;

        private float timeToComplete;
        private float timeTaken;

        private CurvePlotting curve;
        
        #endregion       


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }


        #region Initialize Methods        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();           

            currentState = part.ONE;

            switch (currentState)
            {
                case part.ONE:
                    PartOneInitialize();
                    break;
                case part.TWO:
                    PartTwoInitialize();
                    break;
            }
        }

        /// <summary>
        /// Initializes all the variables needed for Part One of the assignment
        /// </summary>
        private void PartOneInitialize()
        {
            moveOn = true;
            dragOn = false;
            gravityOn = false;
            kinecticOn = false;

            cubeSize = 10.0f;
            ballRadius = 1.0f;

            cubePosition = new Vector3(0.0f, 0.0f, 0.0f);
            ballPosition = new Vector3((float)randomNums.NextDouble(), (float)randomNums.NextDouble(), (float)randomNums.NextDouble());
            initialVelocity = new Vector3((float)randomNums.NextDouble(), (float)randomNums.NextDouble(), (float)randomNums.NextDouble());            

            cube = new Shape(cubeModel, cubePosition, new Vector3(0.0f, 0.0f, 0.0f), 0.05f, cubeSize, Color.Firebrick.ToVector3());
            ball = new Shape(ballModel, ballPosition, initialVelocity, 1.0f, ballRadius, Color.Azure.ToVector3());
            
            //  camera settings
            cameraPosition = new Vector3(22.0f, 15.5f, -20.5f); //cameraPosition = {X:21.91004 Y:15.44271 Z:-20.90045}
            lookAt = new Vector3(9.0f, 6.5f, -12.0f); // lookAt = {X:9.221636 Y:6.635674 Z:-11.73702}            
            view = Matrix.CreateLookAt(cameraPosition, lookAt, new Vector3(0.0f, 1.0f, 0.0f));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);

        }

        /// <summary>
        /// Initializes all the variables needed for Part Two of the assignment
        /// </summary>
        private void PartTwoInitialize()
        {
            moveOn = true;
            piecewise = true;
            interpolation = false;
            approximation = false;


            //  Determines the rate of ease-in/ease-out
            index = 0;  //  numerator
            piecewiseSegments = 1000;   //  denominator
            interpolationSegments = 1500;
            numberOfPoints = 8;    //  Points that will make up the path

            if (piecewise)
            {
                InitializePieceWise();
            }
            else if (interpolation)
            {
                InitializeInterpolation();                
            }
            else if (approximation)
            {
                InitializeApproximation();
            }

            //  camera settings
            thirdPersonReference = new Vector3(0, 50, -15);
            cameraPosition = thirdPersonReference + followThisBall.position;
            lookAt = followThisBall.position;

            view = Matrix.CreateLookAt(cameraPosition, lookAt, new Vector3(0.0f, 1.0f, 0.0f));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        }

        private void InitializePieceWise()
        {
            piecewisePoints = new Shape[numberOfPoints];            
            for (int i = 0; i < numberOfPoints; i++)
            {
                Vector3 xyz = new Vector3((float)randomNums.Next(40), (float)randomNums.Next(40), (float)randomNums.Next(40));
                piecewisePoints[i] = new Shape(ballModel, xyz, Vector3.Zero, 1.0f, 0.5f, Color.Green.ToVector3());
            }   
            followThisBall = new Shape(ballModel, piecewisePoints[0].position, Vector3.Zero, 1.0f, 0.75f, Color.Red.ToVector3());
            nextPoint = 1;

        }

        private void InitializeInterpolation()
        {
            curve = new CurvePlotting(numberOfPoints);
            followThisBall = new Shape(ballModel, curve.GetPosition(0), Vector3.Zero, 1.0f, 0.75f, Color.Red.ToVector3());
            interpolationPoints = new Shape[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                interpolationPoints[i] = new Shape(ballModel, curve.midPoints[i], Vector3.Zero, 1.0f, 0.75f, Color.Green.ToVector3());
            }

        }

        private void InitializeApproximation()
        {

        }
        #endregion


        #region Load Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here            
            cubeModel = Content.Load<Model>("CubeWireframe2");
            ballModel = Content.Load<Model>("ballDesign");
            font = Content.Load<SpriteFont>("font");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion


        #region Update Methods
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            switch(currentState)
            {
                case part.ONE:
                    PartOneUpdate(elapsedTime);
                    break;
                case part.TWO:
                    PartTwoUpdate(elapsedTime);
                    break;
            }

            base.Update(gameTime);
        }
        
        private void PartOneUpdate(float elapsedTime)
        {
            PartOneKeyboardActions();

            if (moveOn)
            {
                if (gravityOn)
                {
                    timeSinceCollision += elapsedTime;
                }

                Collision(cube, ball);

                if (gravityOn)
                {                    
                    float velocityByGravity = initialVelocity.Y + timeSinceCollision * GRAVITY;
                    ball.velocity = new Vector3(ball.velocity.X, velocityByGravity, ball.velocity.Z);
                }

                if (dragOn)
                {
                    ball.velocity = new Vector3(ball.velocity.X * 0.99f, ball.velocity.Y * 0.99f, ball.velocity.Z * 0.99f);
                }            
                ball.Move();
            }

            cameraPosition = new Vector3(22.0f, 15.5f, -20.5f);
            lookAt = new Vector3(9.0f, 6.5f, -12.0f);
            view = Matrix.CreateLookAt(cameraPosition, lookAt, new Vector3(0.0f, 1.0f, 0.0f));

        }

        private void PartTwoUpdate(float elapsedTime)
        {
            PartTwoKeyboardActions();

            if (moveOn)
            {
                if (piecewise)
                {
                    if (nextPoint == numberOfPoints)
                    {
                        nextPoint = 1;
                        followThisBall.position = piecewisePoints[0].position;
                    }
                    else
                    {
                        followThisBall.position = PiecewiseLinearMotion(piecewisePoints[nextPoint - 1].position, followThisBall.position, piecewisePoints[nextPoint].position);
                    }
                }
                else if (interpolation)
                {                    
                    followThisBall.position = PolynomialInterpolationMotion(elapsedTime);
                }
                else if (approximation)
                {

                }
            }            

            cameraPosition = thirdPersonReference + followThisBall.position;
            lookAt = followThisBall.position;
            view = Matrix.CreateLookAt(cameraPosition, lookAt, new Vector3(0.0f, 1.0f, 0.0f));

        }
        #endregion


        #region Draw Methods
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (currentState)
            {
                case part.ONE:
                    PartOneDraw();
                    break;
                case part.TWO:
                    PartTwoDraw();
                    break;
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
                        
        private void PartOneDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, String.Format("Controls:"), new Vector2(10, 0), Color.Black);            
            spriteBatch.DrawString(font, String.Format("(1) Gravity:  {0}", (gravityOn)? "Yes" : "No" ), new Vector2(10, 20), Color.Black);
            spriteBatch.DrawString(font, String.Format("(2) Drag:     {0}", (dragOn)? "Yes" : "No" ), new Vector2(10, 40), Color.Black);
            spriteBatch.DrawString(font, String.Format("(3) Kinectic: {0}", (kinecticOn) ? "Yes" : "No"), new Vector2(10, 60), Color.Black);
            spriteBatch.DrawString(font, String.Format("(4) Turn Everything On"), new Vector2(10, 80), Color.Black);
            spriteBatch.DrawString(font, String.Format("(R) Reset (new initial position and velocity)"), new Vector2(10, 100), Color.Black);
            spriteBatch.DrawString(font, String.Format("(S) Start/Stop"), new Vector2(10, 120), Color.Black);
            spriteBatch.DrawString(font, String.Format("(F1) Part One"), new Vector2(10, 140), Color.Black);
            spriteBatch.DrawString(font, String.Format("(F2) Part Two"), new Vector2(10, 160), Color.Black);
            spriteBatch.DrawString(font, String.Format("(ESC) Exit"), new Vector2(10, 180), Color.Black);
            spriteBatch.End();

            ball.DrawModel(view, projection);

            cube.DrawModel(view, projection);
        }

        private void PartTwoDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.DrawString(font, String.Format("Controls:"), new Vector2(10, 0), Color.Black);
            spriteBatch.DrawString(font, String.Format("(1) Piecewise"), new Vector2(10, 20), Color.Black);
            spriteBatch.DrawString(font, String.Format("(2) Interpolation"), new Vector2(10, 40), Color.Black);            
            spriteBatch.DrawString(font, String.Format("(R) Reset (new points are given)"), new Vector2(10, 60), Color.Black);
            spriteBatch.DrawString(font, String.Format("(S) Start/Stop"), new Vector2(10, 80), Color.Black);
            spriteBatch.DrawString(font, String.Format("(F1) Part One"), new Vector2(10, 100), Color.Black);
            spriteBatch.DrawString(font, String.Format("(F2) Part Two"), new Vector2(10, 120), Color.Black);
            spriteBatch.DrawString(font, String.Format("(ESC) Exit"), new Vector2(10, 140), Color.Black);
            spriteBatch.End();

            if (piecewise)
            {
                foreach (Shape point in piecewisePoints)
                {
                    point.DrawModel(view, projection);
                }
            }
            else if (interpolation)
            {
                foreach (Shape shape in interpolationPoints)
                {
                    shape.DrawModel(view, projection);
                }
            }
            else if (approximation)
            {

            }

            followThisBall.DrawModel(view, projection);

        }
        #endregion

        //  Logic taken from here to control user input 
        //  http://www.dreamincode.net/forums/topic/257077-good-xna-keyboard-input/
        #region Keyboard Actions
        /// <summary>
        /// Keyboard actions for Part One of the assignment
        /// </summary>
        private void PartOneKeyboardActions()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            Keys[] keys = currentKeyboardState.GetPressedKeys();
            foreach (Keys key in keys)
            {
                if (CheckPressedKeys(key))
                {
                    switch (key)
                    {
                        case Keys.R:
                            moveOn = true;
                            gravityOn = false;
                            dragOn = false;
                            kinecticOn = false;
                            ball.velocity = new Vector3((float)randomNums.NextDouble(), (float)randomNums.NextDouble(), (float)randomNums.NextDouble());
                            ball.position = new Vector3((float)randomNums.Next(-5, 5), (float)randomNums.Next(-5, 5), (float)randomNums.Next(-5, 5));
                            break;
                        case Keys.D1:
                            // Turn Gravity on
                            gravityOn = !gravityOn;
                            break;
                        case Keys.D2:
                            // Turn Drag on
                            dragOn = !dragOn;
                            break;
                        case Keys.D3:
                            // Turn Loss of Kinect Energy
                            kinecticOn = !kinecticOn;
                            break;
                        case Keys.D4:
                            // Turn Everything On
                            gravityOn = true;
                            dragOn = true;
                            kinecticOn = true;
                            break;
                        case Keys.S:
                            moveOn = !moveOn;
                            break;
                        case Keys.F2:
                            currentState = part.TWO;
                            PartTwoInitialize();
                            break;
                        case Keys.Escape:
                            this.Exit();
                            break;
                    }
                }
            }

            lastKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Keyboard actions for Part Two of the assignment
        /// </summary>
        private void PartTwoKeyboardActions()
        {
            KeyboardState curretKeyboardState = Keyboard.GetState();
            Keys[] keys = curretKeyboardState.GetPressedKeys();
            foreach (Keys key in keys)
            {
                if (CheckPressedKeys(key))
                {
                    switch (key)
                    {
                        case Keys.R:
                            if (piecewise)
                            {
                                InitializePieceWise();
                            }
                            else if (interpolation)
                            {
                                InitializeInterpolation();
                            }
                            else if (approximation)
                            {

                            }
                            break;
                        case Keys.D1:
                            // Piecewise linear motion pathing
                            piecewise = true;
                            interpolation = false;
                            approximation = false;
                            InitializePieceWise();
                            break;
                        case Keys.D2:
                            // Polynomial interpolation pathing
                            piecewise = false;
                            interpolation = true;
                            approximation = false;
                            InitializeInterpolation();
                            break;
                        case Keys.D3:
                            // Polynomial approximation pathing - Does nothing                        
                            break;
                        case Keys.S:
                            moveOn = !moveOn;
                            break;
                        case Keys.F1:
                            currentState = part.ONE;
                            PartOneInitialize();
                            break;
                        case Keys.Escape:
                            this.Exit();
                            break;
                    }
                }
            }

            lastKeyboardState = curretKeyboardState;
        }


        private bool CheckPressedKeys(Keys key)
        {
            bool keyPressed = false;
            
            if(lastKeyboardState.IsKeyUp(key))
            {
                keyPressed = true;                
            }            
            
            return keyPressed;
        }
        #endregion
        

        #region Part 1 of assignment: Collision        
        /// <summary>
        /// Calculations for collision with respect to loss of kinectic energy if on
        /// </summary>
        /// <param name="aCube"></param>
        /// <param name="aBall"></param>
        private void Collision(Shape aCube, Shape aBall)
        {
            float boundary = cubeSize;
            float xVel = aBall.velocity.X;
            float yVel = aBall.velocity.Y;
            float zVel = aBall.velocity.Z;

            if (aBall.position.X + aBall.velocity.X + ballRadius > boundary || aBall.position.X + aBall.velocity.X - ballRadius < -boundary)
            {
                xVel = -xVel;
                if (kinecticOn)
                {
                    xVel *= 0.90f;
                }
                aBall.velocity = new Vector3(xVel, yVel, zVel);
            }

            if (aBall.position.Y + aBall.velocity.Y + ballRadius > boundary || aBall.position.Y + aBall.velocity.Y - ballRadius < -boundary)
            {
                timeSinceCollision = 0.0f;
                
                yVel = -yVel;
                if (kinecticOn)
                {
                    yVel *= 0.90f;
                }
                aBall.velocity = new Vector3(xVel, yVel, zVel);
                initialVelocity = aBall.velocity; // At a collision point, this defines a new initial velocity needed to compute the gravity velocity
            }

            if (aBall.position.Z + aBall.velocity.Z  + ballRadius > boundary || aBall.position.Z + aBall.velocity.Z - ballRadius < -boundary)
            {
                zVel = -zVel;
                if (kinecticOn)
                {
                    zVel *= 0.90f;
                }
                aBall.velocity = new Vector3(xVel, yVel, zVel);
            }
        }
        #endregion


        #region Part 2 of assignment: Curve calculations

        /// <summary>
        /// Algorithm to translate the ball in a piecewise linear motion to various points with a cubic ease in - ease out 
        /// </summary>
        /// <param name="aInitialPosition"></param>
        /// <param name="aCurrentPosition"></param>
        /// <param name="aTargetPosition"></param>
        private Vector3 PiecewiseLinearMotion(Vector3 aInitialPosition, Vector3 aCurrentPosition, Vector3 aTargetPosition)
        {
            Vector3 next;
            Vector3 distanceLeft = aTargetPosition - aCurrentPosition;

            float distanceLeftLength = distanceLeft.Length();            
            float ratio = index / (float)piecewiseSegments;
            
            //  sinusoidal ease-in/out moves too fast
            //float nextPosition = (float)((0.5) * Math.Sin((ratio - 0.5)) * Math.PI + 1); // sinusoidal ease-in/out slide 149           
            float nextPosition = (float)(Math.Pow(ratio, 2.0) * (3 - 2 * ratio)); // cubic ease-in/out slide 148           

            // To speed up the ease-in/ease-out, if the ball is pretty much on the targetted point, to move it directly on the point
            if (index == piecewiseSegments || distanceLeftLength < 0.01f)
            {
                index = 0;
                next = aTargetPosition;
                nextPoint += 1;
            }
            else
            {
                next = followThisBall.position + distanceLeft * nextPosition; 
                index += 1;
            }

            return next;            
        }

        /// <summary>
        /// Casteljau's Algorithm to translate the ball to various points with a cubic ease in - ease out for polynomial interpolation
        /// </summary>
        /// <param name="aInitialPosition"></param>
        /// <param name="aCurrentPosition"></param>
        /// <param name="aTargetPosition"></param>
        private Vector3 PolynomialInterpolationMotion(float elapsedTime)
        {
            float ratio = index / (float)interpolationSegments;
            float nextPosition = (float)(Math.Pow(ratio, 2.0) * (3 - 2 * ratio)); // cubic ease-in/out slide 148
            if (ratio > 1)
            {
                index = 0;
            }
            index += 1;
            return curve.GetPosition(nextPosition);
        }

        /// <summary>
        /// Didn't do, but this would be the method to display a ball following a path by polynomial approximation
        /// </summary>
        /// <param name="aInitialPosition"></param>
        /// <param name="aCurrentPosition"></param>
        /// <param name="aTargetPosition"></param>
        private void PolynomialApproximationMotion(Vector3 aInitialPosition, Vector3 aCurrentPosition, Vector3 aTargetPosition)
        {

        }


        #endregion
    }
}
