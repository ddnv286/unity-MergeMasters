# MergeMasters
https://play.google.com/store/apps/details?id=com.fusee.MergeMaster

UI
+ Gold to upgrade (currency)
+ 2 upgrade buttons
+ Battlefield
+ Characters
+ 3D Camera

Battlefield
+ Gridline (5x3)
+ Cube character (reduced to cube from 3d model)
+ 2 sides (allies and enemies)

-- 2D flat field--
+ plane
+ cube
field is processed by x and z axis, snap to coordinate by using matrix
 
--Unit (Cube 3D)-- (scale 0.75) -> Represents Unit class (component)
+ Dino and hunter components will be inherited from unit class
+ SetUnitPosition (x,z)
E.g: (0, 0) will be bottom left position
+ MouseEnter() or MoseClicked()
Convert mouse position to unity position then field game coordinate int(x, z)
+ Set unit state to selected
+ Drag and drop unit function
* OnMouseOver()
* OnMouseMove()
* OnMouseExit()
* Set position(new position)

-- Finding Target --
* public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta);
-- this returns the new position in Vector3 and doesn't overshoot target, we can control the speed of movement with maxDistanceDelta and it can be multiplied with Time.deltaTime or Time.fixedDeltaTime to make the speed not affected by frame rate.

-- Flow --
* If the unit's current status is Idle, change it to Moving when Attack or Move button is clicked.
* Move the unit by maxDistanceDelta from current towards target per frame
* If target is in unitRange, stop moving and change the unit's current status to Attacking.
* Add attackSpeed attrib, set the delay between each attack.
* If there is no enemy left, change the status to Idle.
* If the unit's health is reduced to 0, change the status to Dead.
* In case the target unit is changed to Status.Dead before an ally reach the target, either change the ally unit status to Idle or find NearestTarget(ally)
