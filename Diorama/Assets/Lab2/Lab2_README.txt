Name: Aijaisarma Sabaratnasarma
Student Number: 218706051

Description: 
My scene has a large ground plane with cubes as targets. A cylinder is used as the launcher which can shoot spheres at the cubes using VR headset for gaze and keyboard controls.   

Elements of Scene:
 - Ground plane for targets, launcher and projectiles to be placed and land on. 
 - Launcher which is a cylinder primitive angled and placed at the edge of ground plane. Launcher can be controlled via rotations
 - 2 types of Targets:
         - A collision target which uses RigidBody Physics to simulate physical forces and interaction when the projectile hits the cubic target
         - A solid non-RigidBody target which does not react to the forces impacted by the sphereical projectile. 
 - VR Camera Rig which acts as the camera for the VR headset to control camera gaze to aim with the headset, and as the main camera when using keyboard controls. 

Controls/Scripts: 

VR/Main Camera: 
V = Switch between VR mode and Main Camera mode.  
G = Toggles gaze mode, where the headset can be used to aim the launcher at the targets

Object Switching:
C = Toggles a view switch where camera is placed with projectile and the world is seen from the perspective of the current projectile.
K = Toggles a view switch where camera is placed with target and the world is seen from the perspective of the current target.

Launcher Controls:
Left Arrow = rotate launcher to the left
Right Arrow = rotate launcher to the right
Up Arrow = increase launch force on projectile 
Down Arrow = decrease launch force on projectile

SPACE = Shoots projectile from launcher
T = If held before launch (before SPACE pressed) then torque applied increases 

References:
1. Sacred Grove Background: https://www.architectureofzelda.com/sacred-grove-and-the-temple-of-time.html

2. Stone Texture For Ground: https://assetstore.unity.com/packages/2d/textures-materials/4k-realistic-outdoor-materials-295565

3. Ruins Asset: https://assetstore.unity.com/packages/3d/environments/ruins-creation-kit-2235

4. Built-in Unity Assets: Unity Engine

5. Unity Tutorials: https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/

6. Scripting Aid: https://docs.unity3d.com/6000.0/Documentation/Manual/, YouTube.com
 