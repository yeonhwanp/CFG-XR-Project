# ForwardKinematics

Currently Working:
- Basic RobotTree with Properties:
  - The GameObject (aka link)
  - A string identifying BaseType
  - A boolean identifying root
  - A reference to the parent GameObject (I think)
  - A list with the connected joints
  - Vector3s storing localCOM and intertiaTensor
  - A float storing mass
- Basic RobotTree Methods:
  - Methods to do things from script (a bit iffy and a bit useless atm)
  - Methods to create a new RobotTree if given a root object
