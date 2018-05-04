Given a robot cleaner in a room modeled as a grid.
Each cell in the grid can be empty or blocked.. 
The robot cleaner with 4 given APIs can move forward, turn left or turn right.
When it tries to move into a blocked cell,
its bumper sensor detects the obstacle and it stays on the current cell.

The 4 APIs are:
clean(): clean the current location.
turnleft(): turn left 
turnrigt(): turn right 
move(): move forward for 1 position, return False if that’s not possible.