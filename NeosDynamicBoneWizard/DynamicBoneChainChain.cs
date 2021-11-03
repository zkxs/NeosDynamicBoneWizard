using BaseX;
using FrooxEngine;
using FrooxEngine.UIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeosDynamicBoneWizard
{
    [Category("Plugin/runtime")]
    public class DynamicBoneChainChain : Component, ICustomInspector
    {
        [Range(0.0f, 1f, "0.00")]
        public readonly Sync<float> Inertia;
        [Range(-10f, 10f, "0.00")]
        public readonly Sync<float> InertiaForce;
        [Range(0.0f, 100f, "0.00")]
        public readonly Sync<float> Damping;
        [Range(0.0f, 1000f, "0.00")]
        public readonly Sync<float> Elasticity;
        [Range(0.0f, 1f, "0.0000")]
        public readonly Sync<float> Stiffness;
        public readonly Sync<bool> SimulateTerminalBones;
        public readonly Sync<float> BaseBoneRadius;
        public readonly Sync<bool> DynamicPlayerCollision;
        public readonly Sync<bool> CollideWithOwnBody;
        public readonly Sync<VibratePreset> HandCollisionVibration;
        public readonly Sync<bool> CollideWithHead;
        public readonly Sync<bool> CollideWithBody;
        public readonly Sync<bool> CollideWithLeftHand;
        public readonly Sync<bool> CollideWithRightHand;
        public readonly Sync<float3> Gravity;
        public readonly RootSpace GravitySpace;
        public readonly Sync<bool> UseUserGravityDirection;
        public readonly Sync<float3> LocalForce;
        [Range(0.1f, 2f, "0.00")]
        public readonly Sync<float> GlobalStretch;
        [Range(1f, 2f, "0.00")]
        public readonly Sync<float> MaxStretchRatio;
        public readonly Sync<float> StretchRestoreSpeed;
        public readonly Sync<bool> UseLocalUserSpace;
        public readonly RootSpace SimulationSpace;
        public readonly Sync<bool> IsGrabbable;
        public readonly Sync<bool> ActiveUserRootOnly;
        public readonly Sync<bool> AllowSteal;
        public readonly Sync<int> GrabPriority;
        public readonly Sync<bool> IgnoreGrabOnFirstBone;
        [Range(1f, 4f, "0.00")]
        public readonly Sync<float> GrabRadiusTolerance;
        public readonly Sync<float> GrabReleaseDistance;
        public readonly Sync<bool> GrabSlipping;
        public readonly Sync<bool> GrabTerminalBones;
        public readonly Sync<VibratePreset> GrabVibration;
        public readonly Sync<bool> IgnoreOwnLeftHand;
        public readonly Sync<bool> IgnoreOwnRightHand;
        public readonly Sync<SetupModes> SetupMode;
        public readonly SyncRefList<Slot> DirectTargets;
        public readonly SyncRefList<Slot> TargetParents;

        protected override void OnAwake()
        {
            base.OnAwake();
            Inertia.Value = 0.2f;
            Damping.Value = 5f;
            Elasticity.Value = 100f;
            Stiffness.Value = 0.2f;
            InertiaForce.Value = 2f;
            BaseBoneRadius.Value = 0.025f;
            UseLocalUserSpace.Value = true;
            GrabRadiusTolerance.Value = 1.25f;
            GrabReleaseDistance.Value = 1f;
            AllowSteal.Value = true;
            SimulateTerminalBones.Value = true;
            DynamicPlayerCollision.Value = true;
            CollideWithHead.Value = true;
            CollideWithBody.Value = true;
            CollideWithLeftHand.Value = true;
            CollideWithRightHand.Value = true;
            GlobalStretch.Value = 1f;
            MaxStretchRatio.Value = 1f;
            StretchRestoreSpeed.Value = 6f;
            HandCollisionVibration.Value = VibratePreset.None;
            UseUserGravityDirection.Value = true;
        }

        void ICustomInspector.BuildInspectorUI(UIBuilder ui)
        {
            WorkerInspector.BuildInspectorUI(this, ui);
            UIBuilder uiBuilder = ui;
            ui.Button("Create/Update Bones", new ButtonEventHandler(ActivateAction));
        }

        private void ActivateAction(IButton button, ButtonEventData eventData)
        {
            HashSet<Slot> slots = new HashSet<Slot>();
            foreach (Slot slot in DirectTargets)
            {
                slots.Add(slot);
            }

            foreach (Slot slot in TargetParents)
            {
                foreach (Slot innerSlot in slot.Children)
                {
                    slots.Add(innerSlot);
                }
            }

            foreach(Slot slot in slots)
            {
                UpdateSlot(slot);
            }

            button.LabelText = $"Create/Update Bones ({slots.Count} updated)";
        }

        private void UpdateSlot(Slot slot)
        {
            DynamicBoneChain boneChain = slot.GetComponent<DynamicBoneChain>();
            if (boneChain == null)
            {
                boneChain = slot.AttachComponent<DynamicBoneChain>();
            }
            if (boneChain != null)
            {
                //copy values over
                boneChain.Inertia.Value = Inertia.Value;
                boneChain.InertiaForce.Value = InertiaForce.Value;
                boneChain.Damping.Value = Damping.Value;
                boneChain.Elasticity.Value = Elasticity.Value;
                boneChain.Stiffness.Value = Stiffness.Value;
                boneChain.SimulateTerminalBones.Value = SimulateTerminalBones.Value;
                boneChain.BaseBoneRadius.Value = BaseBoneRadius.Value;
                boneChain.DynamicPlayerCollision.Value = DynamicPlayerCollision.Value;
                boneChain.CollideWithOwnBody.Value = CollideWithOwnBody.Value;
                boneChain.HandCollisionVibration.Value = HandCollisionVibration.Value;
                boneChain.CollideWithHead.Value = CollideWithHead.Value;
                boneChain.CollideWithBody.Value = CollideWithBody.Value;
                boneChain.CollideWithLeftHand.Value = CollideWithLeftHand.Value;
                boneChain.CollideWithRightHand.Value = CollideWithRightHand.Value;
                boneChain.Gravity.Value = Gravity.Value;
                CopyRootSpace(boneChain.GravitySpace, GravitySpace);
                boneChain.UseUserGravityDirection.Value = UseUserGravityDirection.Value;
                boneChain.LocalForce.Value = LocalForce.Value;
                boneChain.GlobalStretch.Value = GlobalStretch.Value;
                boneChain.MaxStretchRatio.Value = MaxStretchRatio.Value;
                boneChain.StretchRestoreSpeed.Value = StretchRestoreSpeed.Value;
                boneChain.UseLocalUserSpace.Value = UseLocalUserSpace.Value;
                CopyRootSpace(boneChain.SimulationSpace, SimulationSpace);
                boneChain.IsGrabbable.Value = IsGrabbable.Value;
                boneChain.ActiveUserRootOnly.Value = ActiveUserRootOnly.Value;
                boneChain.AllowSteal.Value = AllowSteal.Value;
                boneChain.GrabPriority.Value = GrabPriority.Value;
                boneChain.IgnoreGrabOnFirstBone.Value = IgnoreGrabOnFirstBone.Value;
                boneChain.GrabRadiusTolerance.Value = GrabRadiusTolerance.Value;
                boneChain.GrabReleaseDistance.Value = GrabReleaseDistance.Value;
                boneChain.GrabSlipping.Value = GrabSlipping.Value;
                boneChain.GrabTerminalBones.Value = GrabTerminalBones.Value;
                boneChain.GrabVibration.Value = GrabVibration.Value;
                boneChain.IgnoreOwnLeftHand.Value = IgnoreOwnLeftHand.Value;
                boneChain.IgnoreOwnRightHand.Value = IgnoreOwnRightHand.Value;

                if (boneChain.Bones.Count == 0)
                {
                    switch (SetupMode.Value)
                    {
                        case SetupModes.Children:
                            boneChain.SetupFromChildren(slot);
                            break;
                        case SetupModes.AllChildren:
                            boneChain.SetupFromChildren(slot, true);
                            break;
                        case SetupModes.RigChildren:
                            Rig rig = slot.GetComponentInParentsOrChildren<Rig>();
                            if (rig == null)
                                rig = slot.GetObjectRoot().GetComponentInChildren<Rig>();
                            if (rig != null)
                                boneChain.SetupFromChildren(slot, filter: s => rig.IsBone(s));
                            break;
                        default:
                            break;
                    }
                    
                }
            }
        }

        private void CopyRootSpace(RootSpace target, RootSpace source)
        {
            target.LocalSpace.Target = source.LocalSpace.Target;
            target.UseParentSpace.Value = source.UseParentSpace.Value;
            target.OverrideRootSpace.Target = source.OverrideRootSpace.Target;
        }

        public enum SetupModes
        {
            Children,
            AllChildren,
            RigChildren
        }
    }
}
