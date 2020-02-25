using System;
using RoR2;
using UnityEngine;

namespace EntityStates.NewtMonster
{
	// Token: 0x020007B3 RID: 1971
	public class KickFromShop : BaseState
	{
		// Token: 0x06002D0F RID: 11535 RVA: 0x000BE460 File Offset: 0x000BC660
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

		// Token: 0x06002D10 RID: 11536 RVA: 0x000BE507 File Offset: 0x000BC707
		public override void OnExit()
		{
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			base.OnExit();
		}

		// Token: 0x06002D11 RID: 11537 RVA: 0x000BE528 File Offset: 0x000BC728
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.modelAnimator && this.modelAnimator.GetFloat("Stomp.hitBoxActive") > 0.5f && !this.hasAttacked)
			{
				Util.PlayScaledSound(KickFromShop.stompSoundString, base.gameObject, this.attackSpeedStat);
				EffectManager.SimpleMuzzleFlash(KickFromShop.stompEffectPrefab, base.gameObject, "HealthBarOrigin", false);
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

		// Token: 0x06002D12 RID: 11538 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400294F RID: 10575
		public static float duration = 3.5f;

		// Token: 0x04002950 RID: 10576
		public static string attackSoundString;

		// Token: 0x04002951 RID: 10577
		public static string stompSoundString;

		// Token: 0x04002952 RID: 10578
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002953 RID: 10579
		public static GameObject stompEffectPrefab;

		// Token: 0x04002954 RID: 10580
		private Animator modelAnimator;

		// Token: 0x04002955 RID: 10581
		private Transform modelTransform;

		// Token: 0x04002956 RID: 10582
		private bool hasAttacked;

		// Token: 0x04002957 RID: 10583
		private GameObject chargeEffectInstance;
	}
}
