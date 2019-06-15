# Main

## Folder Details
This folder contains the main project.
- For GUI/HUD Frontend (interaction) scripts, go to /Assets/Scripts.
- For Hand Serialization details, go to /Assets/Scripts/ForTesting, /Assets/Scripts/General, and /Assets/LeapMotion/Core/Scripts.

## Usage Instructions:
1. Load this project into Unity.
2. Scene options:
    - For easier debugging purposes (transform, rotation, scaling logic), launch HUD2D.unity.
    - For the full application, launch HUD3D.unity.
3. The HUD for the debugging scene is self-explanatory. However, the 3DHUD is relatively barebones. Before launching the application, make sure that the LeapMotion device is attached to a helmet or cap such that it will be able to capture the user's hands from a first person POV.
    - There are four buttons:
      - Spawn Joint
      - Spawn Link
      - Attach Joint
      - Delete/Detach Joint
    - To interact with a spawned object, bring your hands close to the object. Two sets of arrows should appear. To interact with them, simply pinch on them with two fingers and drag. The last object that you interacted with will be highlighted.
    - To attach a link to a joint (and vice versa), bring the two desired objects as close as possible and press on the attach joint button with your hand.
    - To detach or delete a link, highlight the desired object and press the delete/detach joint button.

## TODO:
- Fix scaling with children.
- Add high level constraints.
