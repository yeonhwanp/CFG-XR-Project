# MIT-CSAIL CFG: XR Project

Bring robots to life in a mixed reality enviornment using gestures!

## Features
- Record natural gestures with the LeapMotion's sensors to create robot links and joints in a mixed reality space.
- Utilize the MagicLeap's visual technology to extract image and mesh data from the real world and optimize goals for virtual robots given high-level constraints.

## Dependencies
<b>Built with</b>
- [Unity](https://unity.com)
- [MagicLeap](https://magicleap.com)
- [LeapMotion](https://leapmotion.com)

## Install
1. Make sure you have the latest version of [Unity](https://unity3d.com/get-unity/update)
2. Make sure you have the latest verison of [LeapMotion Unity Core Assets and Modules](https://developer.leapmotion.com/unity#5436356)
3. Clone the project: `git clone git@github.com:yeonhwanp/CFG-XR-Project.git`

## Usage
1. Open up Unity -> Add a new project and load in this project
2. Load `/CFG-XR-Project/Main/Assets/Scenes/HUD3D.unity`

## Directories
**1. Archive**
- Contains all outdated files that may still have some use in the future.

**2. Main**
- Contains the main project.
	- TODO: Fix custom object scaling
	- TODO: Incorporate LeapMotion gestures into the MagicLeap application

**3. RobotMoverML**
- Contains necessary files to run virtual simulations on the MagicLeap.
- This application is built in a different project because the MagicLeap has dependencies from an experimental build of Unity that crashes a lot.

**4. NetServer**
- Contains the backend (.NET 3.5) to send and receive ProtoBufs over websockets.
- Also used for debugging purposes.


