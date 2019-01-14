# CSAIL CFG XR Project

This repo hosts the files to a mixed reality application which will be deployed using the MagicLeap. The application features an interface for the user to manipulate individual robot parts (links, joints, etc.) to create a fully functional robot in real world space. 

This folder is home to all of the files used to create and test the application.

## Folders

**1. Archive:** This folder hosts all of the outdated files that may be of use sometime in the future.

**2. HUD:** 
  - This folder contains the main project.
  - Everything is completed except for the scaling component.
  - Currently working on delivering gestural data from the Leapmotion over to the MagicLeap over websockets.
  
**3. RobotMoverML:** 

  - Used for simulations in MagicLeap.
  - This is in a different project because MagicLeap uses a newer and customized version of Unity. This version crashes a lot.
  
**4. NetServer:**

  - This folder contains the backend (.NET 3.5) to be used with the application.
  - Mainly used for debugging purposes at the moment.
  - Contains an app that simulates the server to send/receive ProtoBufs from.
