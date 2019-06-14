# MIT-CSAIL CFG: XR Project

## Overview
This project aims to bring a mixed reality interface for robotic design combining the technologies
of the MagicLeap headset and LeapMotion sensor. This project can/will eventually be able to:
- Utilize the Magic Leap's camera to take image/mesh data from the real world to make accurate predictions about its surroundings.
- Convert real-world hand gestures into commands by using the LeapMotion's gesture sensing technologies.
- Optimize goals for a virtual robot given high-level constraints.

## Folders
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

