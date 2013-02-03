Bouncing-Balls
==============
Part One

-	Controls are visible on screen
-	The initial position and velocity of the ball is randomnly generated
-	Reset turns every condition off and give the ball a new initial position and velocity
-	Gravity, Drag and Kinectic are toggable
-	The cube and sphere are models taken from Blender
-	The camera is fixed at one spot

Part Two

-	Controls are visible on the screen
-	The position of each point is randomnly generated
-	Cubic ease-in/ease-out is used for piecewise and interpolation
-	Casteljau was used to calculate the curve for interpolation, heavily
	based on the example that Dr. Grogono posted on Moodle
-	Reset causes new points to be generated
-	When resetting, the ball will remain at the current percentage
	of the path travelled from before since I do not reset the index
	back to 0

Bouncing balls in a cube