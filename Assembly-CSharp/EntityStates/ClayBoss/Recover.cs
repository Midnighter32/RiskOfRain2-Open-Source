using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001BA RID: 442
	public class Recover : BaseState
	{
		// Token: 0x060008A8 RID: 2216 RVA: 0x0002B5D4 File Offset: 0x000297D4
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			if (base.characterBody)
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

		// Token: 0x060008A9 RID: 2217 RVA: 0x0002B684 File Offset: 0x00029884
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

		// Token: 0x060008AA RID: 2218 RVA: 0x0002B830 File Offset: 0x00029A30
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

		// Token: 0x060008AB RID: 2219 RVA: 0x0002B888 File Offset: 0x00029A88
		public override void OnExit()
		{
			this.DestroyTethers();
			if (this.mulchEffect)
			{
				EntityState.Destroy(this.mulchEffect);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
			Util.PlaySound(Recover.stopMulchSoundString, base.gameObject);
			base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
			base.OnExit();
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x0002B8E4 File Offset: 0x00029AE4
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

		// Token: 0x060008AD RID: 2221 RVA: 0x0002B91C File Offset: 0x00029B1C
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

		// Token: 0x04000B95 RID: 2965
		public static float duration = 15f;

		// Token: 0x04000B96 RID: 2966
		public static float maxTetherDistance = 40f;

		// Token: 0x04000B97 RID: 2967
		public static float tetherMulchDistance = 5f;

		// Token: 0x04000B98 RID: 2968
		public static float tetherMulchDamageScale = 2f;

		// Token: 0x04000B99 RID: 2969
		public static float tetherMulchTickIntervalScale = 0.5f;

		// Token: 0x04000B9A RID: 2970
		public static float damagePerSecond = 2f;

		// Token: 0x04000B9B RID: 2971
		public static float damageTickFrequency = 3f;

		// Token: 0x04000B9C RID: 2972
		public static float entryDuration = 1f;

		// Token: 0x04000B9D RID: 2973
		public static GameObject mulchEffectPrefab;

		// Token: 0x04000B9E RID: 2974
		public static string enterSoundString;

		// Token: 0x04000B9F RID: 2975
		public static string beginMulchSoundString;

		// Token: 0x04000BA0 RID: 2976
		public static string stopMulchSoundString;

		// Token: 0x04000BA1 RID: 2977
		private GameObject mulchEffect;

		// Token: 0x04000BA2 RID: 2978
		private Transform muzzleTransform;

		// Token: 0x04000BA3 RID: 2979
		private List<TarTetherController> tetherControllers;

		// Token: 0x04000BA4 RID: 2980
		private float stopwatch;

		// Token: 0x04000BA5 RID: 2981
		private uint soundID;

		// Token: 0x04000BA6 RID: 2982
		private Recover.SubState subState;

		// Token: 0x020001BB RID: 443
		private enum SubState
		{
			// Token: 0x04000BA8 RID: 2984
			Entry,
			// Token: 0x04000BA9 RID: 2985
			Tethers
		}
	}
}
