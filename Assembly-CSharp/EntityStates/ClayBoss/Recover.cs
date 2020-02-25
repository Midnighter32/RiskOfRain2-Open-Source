using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D5 RID: 2261
	public class Recover : BaseState
	{
		// Token: 0x060032A7 RID: 12967 RVA: 0x000DB1EC File Offset: 0x000D93EC
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.AddBuff(BuffIndex.ArmorBoost);
			}
			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild("MuzzleMulch");
				}
			}
			this.subState = Recover.SubState.Entry;
			base.PlayCrossfade("Body", "PrepSiphon", "PrepSiphon.playbackRate", Recover.entryDuration, 0.1f);
			this.soundID = Util.PlayScaledSound(Recover.enterSoundString, base.gameObject, this.attackSpeedStat);
		}

		// Token: 0x060032A8 RID: 12968 RVA: 0x000DB2A4 File Offset: 0x000D94A4
		private void FireTethers()
		{
			Vector3 position = this.muzzleTransform.position;
			float breakDistanceSqr = Recover.maxTetherDistance * Recover.maxTetherDistance;
			List<GameObject> list = new List<GameObject>();
			this.tetherControllers = new List<TarTetherController>();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = position;
			bullseyeSearch.maxDistanceFilter = Recover.maxTetherDistance;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.searchDirection = Vector3.up;
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(base.gameObject);
			List<HurtBox> list2 = bullseyeSearch.GetResults().ToList<HurtBox>();
			Debug.Log(list2);
			for (int i = 0; i < list2.Count; i++)
			{
				GameObject gameObject = list2[i].healthComponent.gameObject;
				if (gameObject)
				{
					list.Add(gameObject);
				}
			}
			float tickInterval = 1f / Recover.damageTickFrequency;
			float damageCoefficientPerTick = Recover.damagePerSecond / Recover.damageTickFrequency;
			float mulchDistanceSqr = Recover.tetherMulchDistance * Recover.tetherMulchDistance;
			GameObject original = Resources.Load<GameObject>("Prefabs/NetworkedObjects/TarTether");
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(original, position, Quaternion.identity);
				TarTetherController component = gameObject2.GetComponent<TarTetherController>();
				component.NetworkownerRoot = base.gameObject;
				component.NetworktargetRoot = list[j];
				component.breakDistanceSqr = breakDistanceSqr;
				component.damageCoefficientPerTick = damageCoefficientPerTick;
				component.tickInterval = tickInterval;
				component.tickTimer = (float)j * 0.1f;
				component.mulchDistanceSqr = mulchDistanceSqr;
				component.mulchDamageScale = Recover.tetherMulchDamageScale;
				component.mulchTickIntervalScale = Recover.tetherMulchTickIntervalScale;
				this.tetherControllers.Add(component);
				NetworkServer.Spawn(gameObject2);
			}
		}

		// Token: 0x060032A9 RID: 12969 RVA: 0x000DB450 File Offset: 0x000D9650
		private void DestroyTethers()
		{
			if (this.tetherControllers != null)
			{
				for (int i = this.tetherControllers.Count - 1; i >= 0; i--)
				{
					if (this.tetherControllers[i])
					{
						EntityState.Destroy(this.tetherControllers[i].gameObject);
					}
				}
			}
		}

		// Token: 0x060032AA RID: 12970 RVA: 0x000DB4A8 File Offset: 0x000D96A8
		public override void OnExit()
		{
			this.DestroyTethers();
			if (this.mulchEffect)
			{
				EntityState.Destroy(this.mulchEffect);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
			Util.PlaySound(Recover.stopMulchSoundString, base.gameObject);
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
			}
			base.OnExit();
		}

		// Token: 0x060032AB RID: 12971 RVA: 0x000DB518 File Offset: 0x000D9718
		private static void RemoveDeadTethersFromList(List<TarTetherController> tethersList)
		{
			for (int i = tethersList.Count - 1; i >= 0; i--)
			{
				if (!tethersList[i])
				{
					tethersList.RemoveAt(i);
				}
			}
		}

		// Token: 0x060032AC RID: 12972 RVA: 0x000DB550 File Offset: 0x000D9750
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.subState == Recover.SubState.Entry)
			{
				if (this.stopwatch >= Recover.entryDuration)
				{
					this.subState = Recover.SubState.Tethers;
					this.stopwatch = 0f;
					base.PlayAnimation("Body", "ChannelSiphon");
					Util.PlaySound(Recover.beginMulchSoundString, base.gameObject);
					if (NetworkServer.active)
					{
						this.FireTethers();
						this.mulchEffect = UnityEngine.Object.Instantiate<GameObject>(Recover.mulchEffectPrefab, this.muzzleTransform.position, Quaternion.identity);
						ChildLocator component = this.mulchEffect.gameObject.GetComponent<ChildLocator>();
						if (component)
						{
							component.FindChild("AreaIndicator").localScale = new Vector3(Recover.maxTetherDistance * 2f, Recover.maxTetherDistance * 2f, Recover.maxTetherDistance * 2f);
						}
						this.mulchEffect.transform.parent = this.muzzleTransform;
						return;
					}
				}
			}
			else if (this.subState == Recover.SubState.Tethers && NetworkServer.active)
			{
				Recover.RemoveDeadTethersFromList(this.tetherControllers);
				if ((this.stopwatch >= Recover.duration || this.tetherControllers.Count == 0) && base.isAuthority)
				{
					this.outer.SetNextState(new RecoverExit());
					return;
				}
			}
		}

		// Token: 0x040031BD RID: 12733
		public static float duration = 15f;

		// Token: 0x040031BE RID: 12734
		public static float maxTetherDistance = 40f;

		// Token: 0x040031BF RID: 12735
		public static float tetherMulchDistance = 5f;

		// Token: 0x040031C0 RID: 12736
		public static float tetherMulchDamageScale = 2f;

		// Token: 0x040031C1 RID: 12737
		public static float tetherMulchTickIntervalScale = 0.5f;

		// Token: 0x040031C2 RID: 12738
		public static float damagePerSecond = 2f;

		// Token: 0x040031C3 RID: 12739
		public static float damageTickFrequency = 3f;

		// Token: 0x040031C4 RID: 12740
		public static float entryDuration = 1f;

		// Token: 0x040031C5 RID: 12741
		public static GameObject mulchEffectPrefab;

		// Token: 0x040031C6 RID: 12742
		public static string enterSoundString;

		// Token: 0x040031C7 RID: 12743
		public static string beginMulchSoundString;

		// Token: 0x040031C8 RID: 12744
		public static string stopMulchSoundString;

		// Token: 0x040031C9 RID: 12745
		private GameObject mulchEffect;

		// Token: 0x040031CA RID: 12746
		private Transform muzzleTransform;

		// Token: 0x040031CB RID: 12747
		private List<TarTetherController> tetherControllers;

		// Token: 0x040031CC RID: 12748
		private float stopwatch;

		// Token: 0x040031CD RID: 12749
		private uint soundID;

		// Token: 0x040031CE RID: 12750
		private Recover.SubState subState;

		// Token: 0x020008D6 RID: 2262
		private enum SubState
		{
			// Token: 0x040031D0 RID: 12752
			Entry,
			// Token: 0x040031D1 RID: 12753
			Tethers
		}
	}
}
