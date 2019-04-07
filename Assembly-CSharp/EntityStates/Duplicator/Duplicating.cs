using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Duplicator
{
	// Token: 0x02000191 RID: 401
	public class Duplicating : BaseState
	{
		// Token: 0x060007B6 RID: 1974 RVA: 0x0002640C File Offset: 0x0002460C
		public override void OnEnter()
		{
			base.OnEnter();
			ChildLocator component = base.GetModelTransform().GetComponent<ChildLocator>();
			if (component)
			{
				this.muzzleTransform = component.FindChild(Duplicating.muzzleString);
			}
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x00026444 File Offset: 0x00024644
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

		// Token: 0x060007B8 RID: 1976 RVA: 0x000264C8 File Offset: 0x000246C8
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
					EffectManager.instance.SimpleMuzzleFlash(Duplicating.releaseEffectPrefab, base.gameObject, Duplicating.muzzleString, false);
				}
			}
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0002653C File Offset: 0x0002473C
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

		// Token: 0x04000A00 RID: 2560
		public static float initialDelayDuration = 1f;

		// Token: 0x04000A01 RID: 2561
		public static float timeBetweenStartAndDropDroplet = 3f;

		// Token: 0x04000A02 RID: 2562
		public static string muzzleString;

		// Token: 0x04000A03 RID: 2563
		public static GameObject bakeEffectPrefab;

		// Token: 0x04000A04 RID: 2564
		public static GameObject releaseEffectPrefab;

		// Token: 0x04000A05 RID: 2565
		private GameObject bakeEffectInstance;

		// Token: 0x04000A06 RID: 2566
		private bool hasStartedCooking;

		// Token: 0x04000A07 RID: 2567
		private bool hasDroppedDroplet;

		// Token: 0x04000A08 RID: 2568
		private Transform muzzleTransform;
	}
}
