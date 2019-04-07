using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200016D RID: 365
	public class FireFist : BaseState
	{
		// Token: 0x06000710 RID: 1808 RVA: 0x00021D58 File Offset: 0x0001FF58
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				this.aimAnimator = base.modelLocator.modelTransform.GetComponent<AimAnimator>();
				if (this.aimAnimator)
				{
					this.aimAnimator.enabled = true;
				}
				if (component)
				{
					this.fistTransform = component.FindChild("RightFist");
					if (this.fistTransform)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(this.chargeEffectPrefab, this.fistTransform);
					}
				}
			}
			this.subState = FireFist.SubState.Prep;
			base.PlayCrossfade("Body", "PrepFist", "PrepFist.playbackRate", FireFist.entryDuration, 0.1f);
			Util.PlayScaledSound(FireFist.chargeFistAttackSoundString, base.gameObject, this.attackSpeedStat);
			if (NetworkServer.active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				if (base.teamComponent)
				{
					bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
				}
				bullseyeSearch.maxDistanceFilter = FireFist.maxDistance;
				bullseyeSearch.maxAngleFilter = 90f;
				Ray aimRay = base.GetAimRay();
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
				bullseyeSearch.RefreshCandidates();
				HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
				if (hurtBox)
				{
					this.predictor = new FireFist.Predictor(base.transform);
					this.predictor.SetTargetTransform(hurtBox.transform);
				}
			}
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00021EFC File Offset: 0x000200FC
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
			EntityState.Destroy(this.predictorDebug);
			this.predictorDebug = null;
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x00021F30 File Offset: 0x00020130
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			switch (this.subState)
			{
			case FireFist.SubState.Prep:
				if (this.predictor != null)
				{
					this.predictor.Update();
				}
				if (this.stopwatch <= FireFist.trackingDuration)
				{
					if (this.predictor != null)
					{
						this.predictionOk = this.predictor.GetPredictedTargetPosition(FireFist.entryDuration - FireFist.trackingDuration, out this.predictedTargetPosition);
						if (this.predictionOk && this.predictorDebug)
						{
							this.predictorDebug.transform.position = this.predictedTargetPosition;
						}
					}
				}
				else if (!this.hasShownPrediction)
				{
					this.hasShownPrediction = true;
					this.PlacePredictedAttack();
				}
				if (this.stopwatch >= FireFist.entryDuration)
				{
					this.predictor = null;
					this.subState = FireFist.SubState.FireFist;
					this.stopwatch = 0f;
					base.PlayAnimation("Body", "FireFist");
					if (this.chargeEffect)
					{
						EntityState.Destroy(this.chargeEffect);
					}
					UnityEngine.Object.Instantiate<GameObject>(this.fireEffectPrefab, this.fistTransform.position, Quaternion.identity, this.fistTransform);
					return;
				}
				break;
			case FireFist.SubState.FireFist:
				if (this.stopwatch >= FireFist.fireDuration)
				{
					this.subState = FireFist.SubState.Exit;
					this.stopwatch = 0f;
					base.PlayCrossfade("Body", "ExitFist", "ExitFist.playbackRate", FireFist.exitDuration, 0.3f);
					return;
				}
				break;
			case FireFist.SubState.Exit:
				if (this.stopwatch >= FireFist.exitDuration && base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000220CF File Offset: 0x000202CF
		protected virtual void PlacePredictedAttack()
		{
			this.PlaceSingleDelayBlast(this.predictedTargetPosition, 0f);
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x000220E4 File Offset: 0x000202E4
		protected void PlaceSingleDelayBlast(Vector3 position, float delay)
		{
			EffectManager.instance.SpawnEffect(this.predictedPositionEffectPrefab, new EffectData
			{
				origin = position,
				scale = FireFist.fistRadius,
				rotation = Quaternion.identity
			}, true);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), position, Quaternion.identity);
			DelayBlast component = gameObject.GetComponent<DelayBlast>();
			component.position = position;
			component.baseDamage = this.damageStat * FireFist.fistDamageCoefficient;
			component.baseForce = FireFist.fistForce;
			component.bonusForce = FireFist.fistVerticalForce * Vector3.up;
			component.attacker = base.gameObject;
			component.radius = FireFist.fistRadius;
			component.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			component.maxTimer = FireFist.entryDuration - FireFist.trackingDuration + delay;
			component.falloffModel = BlastAttack.FalloffModel.None;
			component.explosionEffect = this.fistEffectPrefab;
			gameObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
		}

		// Token: 0x040008B2 RID: 2226
		public static float entryDuration = 1f;

		// Token: 0x040008B3 RID: 2227
		public static float fireDuration = 2f;

		// Token: 0x040008B4 RID: 2228
		public static float exitDuration = 1f;

		// Token: 0x040008B5 RID: 2229
		[SerializeField]
		public GameObject chargeEffectPrefab;

		// Token: 0x040008B6 RID: 2230
		[SerializeField]
		public GameObject fistEffectPrefab;

		// Token: 0x040008B7 RID: 2231
		[SerializeField]
		public GameObject fireEffectPrefab;

		// Token: 0x040008B8 RID: 2232
		[SerializeField]
		public GameObject predictedPositionEffectPrefab;

		// Token: 0x040008B9 RID: 2233
		public static float maxDistance = 40f;

		// Token: 0x040008BA RID: 2234
		public static float trackingDuration = 0.5f;

		// Token: 0x040008BB RID: 2235
		public static float fistDamageCoefficient = 2f;

		// Token: 0x040008BC RID: 2236
		public static float fistForce = 2000f;

		// Token: 0x040008BD RID: 2237
		public static float fistVerticalForce;

		// Token: 0x040008BE RID: 2238
		public static float fistRadius = 5f;

		// Token: 0x040008BF RID: 2239
		public static string chargeFistAttackSoundString;

		// Token: 0x040008C0 RID: 2240
		private bool hasShownPrediction;

		// Token: 0x040008C1 RID: 2241
		private bool predictionOk;

		// Token: 0x040008C2 RID: 2242
		protected Vector3 predictedTargetPosition;

		// Token: 0x040008C3 RID: 2243
		private AimAnimator aimAnimator;

		// Token: 0x040008C4 RID: 2244
		private GameObject chargeEffect;

		// Token: 0x040008C5 RID: 2245
		private Transform fistTransform;

		// Token: 0x040008C6 RID: 2246
		private float stopwatch;

		// Token: 0x040008C7 RID: 2247
		private FireFist.SubState subState;

		// Token: 0x040008C8 RID: 2248
		private FireFist.Predictor predictor;

		// Token: 0x040008C9 RID: 2249
		private GameObject predictorDebug;

		// Token: 0x0200016E RID: 366
		private enum SubState
		{
			// Token: 0x040008CB RID: 2251
			Prep,
			// Token: 0x040008CC RID: 2252
			FireFist,
			// Token: 0x040008CD RID: 2253
			Exit
		}

		// Token: 0x0200016F RID: 367
		private class Predictor
		{
			// Token: 0x06000717 RID: 1815 RVA: 0x00022249 File Offset: 0x00020449
			public Predictor(Transform bodyTransform)
			{
				this.bodyTransform = bodyTransform;
			}

			// Token: 0x1700009C RID: 156
			// (get) Token: 0x06000718 RID: 1816 RVA: 0x00022258 File Offset: 0x00020458
			public bool hasTargetTransform
			{
				get
				{
					return this.targetTransform;
				}
			}

			// Token: 0x1700009D RID: 157
			// (get) Token: 0x06000719 RID: 1817 RVA: 0x00022265 File Offset: 0x00020465
			public bool isPredictionReady
			{
				get
				{
					return this.collectedPositions > 2;
				}
			}

			// Token: 0x0600071A RID: 1818 RVA: 0x00022270 File Offset: 0x00020470
			private void PushTargetPosition(Vector3 newTargetPosition)
			{
				this.targetPosition2 = this.targetPosition1;
				this.targetPosition1 = this.targetPosition0;
				this.targetPosition0 = newTargetPosition;
				this.collectedPositions++;
			}

			// Token: 0x0600071B RID: 1819 RVA: 0x000222A0 File Offset: 0x000204A0
			public void SetTargetTransform(Transform newTargetTransform)
			{
				this.targetTransform = newTargetTransform;
				this.targetPosition2 = (this.targetPosition1 = (this.targetPosition0 = newTargetTransform.position));
				this.collectedPositions = 1;
			}

			// Token: 0x0600071C RID: 1820 RVA: 0x000222D9 File Offset: 0x000204D9
			public void Update()
			{
				if (this.targetTransform)
				{
					this.PushTargetPosition(this.targetTransform.position);
				}
			}

			// Token: 0x0600071D RID: 1821 RVA: 0x000222FC File Offset: 0x000204FC
			public bool GetPredictedTargetPosition(float time, out Vector3 predictedPosition)
			{
				Vector3 lhs = this.targetPosition1 - this.targetPosition2;
				Vector3 vector = this.targetPosition0 - this.targetPosition1;
				lhs.y = 0f;
				vector.y = 0f;
				FireFist.Predictor.ExtrapolationType extrapolationType;
				if (lhs == Vector3.zero || vector == Vector3.zero)
				{
					extrapolationType = FireFist.Predictor.ExtrapolationType.None;
				}
				else
				{
					Vector3 normalized = lhs.normalized;
					Vector3 normalized2 = vector.normalized;
					if (Vector3.Dot(normalized, normalized2) > 0.98f)
					{
						extrapolationType = FireFist.Predictor.ExtrapolationType.Linear;
					}
					else
					{
						extrapolationType = FireFist.Predictor.ExtrapolationType.Polar;
					}
				}
				float num = 1f / Time.fixedDeltaTime;
				predictedPosition = this.targetPosition0;
				switch (extrapolationType)
				{
				case FireFist.Predictor.ExtrapolationType.Linear:
					predictedPosition = this.targetPosition0 + vector * (time * num);
					break;
				case FireFist.Predictor.ExtrapolationType.Polar:
				{
					Vector3 position = this.bodyTransform.position;
					Vector3 v = Util.Vector3XZToVector2XY(this.targetPosition2 - position);
					Vector3 v2 = Util.Vector3XZToVector2XY(this.targetPosition1 - position);
					Vector3 v3 = Util.Vector3XZToVector2XY(this.targetPosition0 - position);
					float magnitude = v.magnitude;
					float magnitude2 = v2.magnitude;
					float magnitude3 = v3.magnitude;
					float num2 = Vector2.SignedAngle(v, v2) * num;
					float num3 = Vector2.SignedAngle(v2, v3) * num;
					float num4 = (magnitude2 - magnitude) * num;
					float num5 = (magnitude3 - magnitude2) * num;
					float num6 = (num2 + num3) * 0.5f;
					float num7 = (num4 + num5) * 0.5f;
					float num8 = magnitude3 + num7 * time;
					if (num8 < 0f)
					{
						num8 = 0f;
					}
					Vector2 vector2 = Util.RotateVector2(v3, num6 * time);
					vector2 *= num8 * magnitude3;
					predictedPosition = position;
					predictedPosition.x += vector2.x;
					predictedPosition.z += vector2.y;
					break;
				}
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(predictedPosition + Vector3.up * 1f, Vector3.down), out raycastHit, 200f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					predictedPosition = raycastHit.point;
					return true;
				}
				return false;
			}

			// Token: 0x040008CE RID: 2254
			private Transform bodyTransform;

			// Token: 0x040008CF RID: 2255
			private Transform targetTransform;

			// Token: 0x040008D0 RID: 2256
			private Vector3 targetPosition0;

			// Token: 0x040008D1 RID: 2257
			private Vector3 targetPosition1;

			// Token: 0x040008D2 RID: 2258
			private Vector3 targetPosition2;

			// Token: 0x040008D3 RID: 2259
			private int collectedPositions;

			// Token: 0x02000170 RID: 368
			private enum ExtrapolationType
			{
				// Token: 0x040008D5 RID: 2261
				None,
				// Token: 0x040008D6 RID: 2262
				Linear,
				// Token: 0x040008D7 RID: 2263
				Polar
			}
		}
	}
}
