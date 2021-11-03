# Neos Dynamic Bone Wizard
A Neos VR plugin to expedite setting up a large number of dynamic bones. You specify default settings, add a list of slots, then hit the "Create/Update Bones" button.

If a DynamicBoneChain component already exists, it will be updated. If it doesn't exist, it will be created. If the chain has no bones in the list it will automatically be set up as per your SetupMode preference.

Target slots can be given either directly, or via their parent slot by using either DirectTargets or TargetParents. To clarify, DirectTargets will have a DynamicBoneChain installed on themselves, where TargetParents will have a DynamicBoneChain installed on each of their child slots.

![screenshot of component](docs/DynamicBoneChainChain%20screenshot.png)
