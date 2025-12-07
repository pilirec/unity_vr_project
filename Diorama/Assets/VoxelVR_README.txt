Name: Aijaisarma Sabaratnasarma
Student Number: 218706051

Description: 
This project is a VR voxel-based building system that allows users to create, manipulate, and interact with 3D voxel structures in virtual reality. 
The system features a grid-based environment where users can place individual cubes, which automatically connect to form larger structures. 
Using the left controller, users can grab, move, scale, and release these structures back to the grid or drop them to the ground. 
The right controller allows for precise placement and removal of individual voxels. 
This project demonstrates fundamental VR interaction patterns and physics-based manipulations within a creative building context.

Elements of Scene:
 - 3D grid system with customizable dimensions and cell sizes
 - Voxel cubes that can be placed, removed, and manipulated
 - Visual highlighting system showing grid cells and connected structures
 - Custom ray visuals for both controllers with color differentiation
 - Physics-based dropping and collision for structures released away from the grid
 - Material system that visually connects adjacent voxels to show unified structures
 - Ground plane for dropped structures to land on

Components:
 - Directional Light: main light source in scene
 - XR Interaction Manager: manages XR connections with Oculus Rift
 - GridManager: Gameobject that manages GridManager.cs script and AdjacentCubeManager.cs scripts 
 - Ground: The ground plane
 - XR Origin: The VR rig with headset and controllers
      - Main Camera: headset/gaze into the VR scene
      - Left Controller: handles locomotion, grabbing (trigger), scaling (grip) of voxels
      - Right Controller: handles turing, adding (trigger), removing (grip) voxels  
      - Locomotion: locomotion script handling and input action mappings
- Text: Welcome text to project/scene.

Controls/Scripts: 
Right Controller (XRGridInteraction):
 - Trigger Button: Place a new voxel cube at the highlighted grid cell
 - Grip Button: Remove an existing voxel at the highlighted grid cell
 - Ray Pointer (Purple): Used to highlight grid cells for interaction
 - Analog Stick: Turning and changing directions

Left Controller (CubeGrabber):
 - Trigger Button: Grab a voxel or connected voxel structure 
   * While holding: Move controller to reposition the structure
   * Release trigger near grid: Structure snaps back to grid positions with reset orientation
   * Release trigger away from grid: Structure drops to ground with physics and maintains rotation
 - Grip Button: Scale the currently held voxel structure
   * Higher grip value increases the scale
   * Scale is limited between min and max values (0.5x to 4.0x by default)
 - Ray Pointer (Red): Used to target voxels for grabbing
 - Analog Stick: Movement around ground plane

Grid Settings (adjustable in Inspector):
 - Grid Size X/Y/Z: Dimensions of the grid (default 10x10x10)
 - Cell Size: Size of each cell (default 1.5)
 - Grid Spacing: Space between cells (default 0)
 
Grabber Settings (adjustable in Inspector):
 - Scaling Speed: Adjusts how quickly structures scale when using grip (default 3.0)
 - Grid Snap Distance: Maximum distance from grid to snap back when released (default 30.0)
 - Ground Plane Y: Y-coordinate of the ground plane (default -4.0)
 - Falling Speed: Physics-based falling speed for dropped structures (default 9.8)

Script Functionality:
 - GridManager.cs: Core system managing the 3D grid, voxel placement and removal
 - GridCell.cs: Individual grid cells with highlighting capability
 - AdjacentCubeManager.cs: Handles connected voxel structures and material changes
 - CubeGrabber.cs: Left controller functionality for grabbing, scaling, and releasing voxels
 - XRGridInteraction.cs: Right controller functionality for placing and removing voxels
 - XRCustomRayVisual.cs: Visual rays for both controllers with color differentiation

References:
1. Unity XR Interaction Toolkit Documentation: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html

2. Unity Input System: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html

3. Flood Fill Algorithm (for connected structures): https://en.wikipedia.org/wiki/Flood_fill

4. Ruins Asset Pack: https://assetstore.unity.com/packages/3d/environments/ruins-creation-kit-2235

5. Unity Tutorials: https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/

6. Scripting Aid: https://docs.unity3d.com/6000.0/Documentation/Manual/class-MonoBehaviour.html, YouTube.com