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
