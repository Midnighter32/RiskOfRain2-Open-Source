using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando
{
	// Token: 0x0200019F RID: 415
	public class DeathState : BaseState
	{
		// Token: 0x06000809 RID: 2057 RVA: 0x00027CE0 File Offset: 0x00025EE0
		public override void OnEnter()
		{
			base.OnEnter();
			this.PlayDeathSound();
			Transform modelTransform = base.GetModelTransform();
			Vector3 vector = Vector3.up * 3f;
			if (base.characterMotor)
			{
				vector += base.characterMotor.velocity;
				base.characterMotor.enabled = false;
			}
			if (base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(ItemIndex.LunarDagger) > 0)
			{
				this.isBrittle = true;
			}
			if (base.modelLocator)
			{
				base.modelLocator.modelTransform = null;
			}
			if (modelTransform)
			{
				RagdollController component = modelTransform.GetComponent<RagdollController>();
				if (component)
				{
					component.BeginRagdoll(vector);
				}
				if (this.isBrittle)
				{
					TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.5f;
					temporaryOverlay.destroyObjectOnEnd = true;
					temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matShatteredGlass");
					temporaryOverlay.destroyEffectPrefab = (GameObject)Resources.Load("Prefabs/Effects/BrittleDeath");
					temporaryOverlay.destroyEffectChildString = "Chest";
					temporaryOverlay.inspectorCharacterModel = modelTransform.gameObject.GetComponent<CharacterModel>();
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
					temporaryOverlay.animateShaderAlpha = true;
				}
			}
			if (base.cameraTargetParams)
			{
				ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					Transform transform = component2.FindChild("Chest");
					if (transform)
					{
						base.cameraTargetParams.cameraPivotTransform = transform;
						base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
						base.cameraTargetParams.dontRaycastToPivot = true;
					}
				}
			}
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x00027E95 File Offset: 0x00026095
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			base.FixedUpdate();
			if (NetworkServer.active && this.stopwatch > 4f)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0000E681 File Offset: 0x0000C881
		protected void PlayDeathSound()
		{
			if (base.sfxLocator && base.sfxLocator.deathSound != "")
			{
				Util.PlaySound(base.sfxLocator.deathSound, base.gameObject);
			}
		}

		// Token: 0x04000A82 RID: 2690
		private Vector3 previousPosition;

		// Token: 0x04000A83 RID: 2691
		private float upSpeedVelocity;

		// Token: 0x04000A84 RID: 2692
		private float upSpeed;

		// Token: 0x04000A85 RID: 2693
		private Animator modelAnimator;

		// Token: 0x04000A86 RID: 2694
		private float stopwatch;

		// Token: 0x04000A87 RID: 2695
		private bool isBrittle;
	}
}
