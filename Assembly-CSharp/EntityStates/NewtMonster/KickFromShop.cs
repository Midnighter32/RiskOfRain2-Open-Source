using System;
using RoR2;
using UnityEngine;

namespace EntityStates.NewtMonster
{
	// Token: 0x020000FF RID: 255
	public class KickFromShop : BaseState
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x00014D3C File Offset: 0x00012F3C
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(KickFromShop.attackSoundString, base.gameObject, this.attackSpeedStat);
			base.PlayCrossfade("Body", "Stomp", "Stomp.playbackRate", KickFromShop.duration, 0.1f);
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("StompMuzzle");
					if (transform)
					{
						this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(KickFromShop.chargeEffectPrefab, transform);
					}
				}
			}
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00014DE3 File Offset: 0x00012FE3
		public override void OnExit()
		{
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			base.OnExit();
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00014E04 File Offset: 0x00013004
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("Stomp.hitBoxActive") > 0.5f && !this.hasAttacked)
			{
				Util.PlayScaledSound(KickFromShop.stompSoundString, base.gameObject, this.attackSpeedStat);
				EffectManager.instance.SimpleMuzzleFlash(KickFromShop.stompEffectPrefab, base.gameObject, "HealthBarOrigin", false);
				if (SceneInfo.instance)
				{
					GameObject gameObject = SceneInfo.instance.transform.Find("KickOutOfShop").gameObject;
					if (gameObject)
					{
						gameObject.gameObject.SetActive(true);
					}
				}
				if (base.isAuthority)
				{
					HurtBoxGroup component = this.modelTransform.GetComponent<HurtBoxGroup>();
					if (component)
					{
						HurtBoxGroup hurtBoxGroup = component;
						int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
						hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
					}
				}
				this.hasAttacked = true;
				EntityState.Destroy(this.chargeEffectInstance);
			}
			if (base.fixedAge >= KickFromShop.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040004CF RID: 1231
		public static float duration = 3.5f;

		// Token: 0x040004D0 RID: 1232
		public static string attackSoundString;

		// Token: 0x040004D1 RID: 1233
		public static string stompSoundString;

		// Token: 0x040004D2 RID: 1234
		public static GameObject chargeEffectPrefab;

		// Token: 0x040004D3 RID: 1235
		public static GameObject stompEffectPrefab;

		// Token: 0x040004D4 RID: 1236
		private Animator modelAnimator;

		// Token: 0x040004D5 RID: 1237
		private Transform modelTransform;

		// Token: 0x040004D6 RID: 1238
		private bool hasAttacked;

		// Token: 0x040004D7 RID: 1239
		private GameObject chargeEffectInstance;
	}
}
