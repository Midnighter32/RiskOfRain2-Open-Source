using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Duplicator
{
	// Token: 0x02000894 RID: 2196
	public class Duplicating : BaseState
	{
		// Token: 0x06003147 RID: 12615 RVA: 0x000D42B4 File Offset: 0x000D24B4
		public override void OnEnter()
		{
			base.OnEnter();
			ChildLocator component = base.GetModelTransform().GetComponent<ChildLocator>();
			if (component)
			{
				this.muzzleTransform = component.FindChild(Duplicating.muzzleString);
			}
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x000D42EC File Offset: 0x000D24EC
		private void BeginCooking()
		{
			if (this.hasStartedCooking)
			{
				return;
			}
			this.hasStartedCooking = true;
			base.PlayAnimation("Body", "Cook");
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
			if (this.muzzleTransform)
			{
				this.bakeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Duplicating.bakeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
			}
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x000D4370 File Offset: 0x000D2570
		private void DropDroplet()
		{
			if (this.hasDroppedDroplet)
			{
				return;
			}
			this.hasDroppedDroplet = true;
			base.GetComponent<ShopTerminalBehavior>().DropPickup();
			if (this.muzzleTransform)
			{
				if (this.bakeEffectInstance)
				{
					EntityState.Destroy(this.bakeEffectInstance);
				}
				if (Duplicating.releaseEffectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(Duplicating.releaseEffectPrefab, base.gameObject, Duplicating.muzzleString, false);
				}
			}
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x000D43DF File Offset: 0x000D25DF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Duplicating.initialDelayDuration)
			{
				this.BeginCooking();
			}
			if (base.fixedAge >= Duplicating.initialDelayDuration + Duplicating.timeBetweenStartAndDropDroplet)
			{
				this.DropDroplet();
			}
		}

		// Token: 0x04002F9C RID: 12188
		public static float initialDelayDuration = 1f;

		// Token: 0x04002F9D RID: 12189
		public static float timeBetweenStartAndDropDroplet = 3f;

		// Token: 0x04002F9E RID: 12190
		public static string muzzleString;

		// Token: 0x04002F9F RID: 12191
		public static GameObject bakeEffectPrefab;

		// Token: 0x04002FA0 RID: 12192
		public static GameObject releaseEffectPrefab;

		// Token: 0x04002FA1 RID: 12193
		private GameObject bakeEffectInstance;

		// Token: 0x04002FA2 RID: 12194
		private bool hasStartedCooking;

		// Token: 0x04002FA3 RID: 12195
		private bool hasDroppedDroplet;

		// Token: 0x04002FA4 RID: 12196
		private Transform muzzleTransform;
	}
}
