using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000853 RID: 2131
	public class FireFist : BaseState
	{
		// Token: 0x0600302B RID: 12331 RVA: 0x000CEB4C File Offset: 0x000CCD4C
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

		// Token: 0x0600302C RID: 12332 RVA: 0x000CECF0 File Offset: 0x000CCEF0
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

		// Token: 0x0600302D RID: 12333 RVA: 0x000CED24 File Offset: 0x000CCF24
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

		// Token: 0x0600302E RID: 12334 RVA: 0x000CEEC3 File Offset: 0x000CD0C3
		protected virtual void PlacePredictedAttack()
		{
			this.PlaceSingleDelayBlast(this.predictedTargetPosition, 0f);
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x000CEED8 File Offset: 0x000CD0D8
		protected void PlaceSingleDelayBlast(Vector3 position, float delay)
		{
			EffectManager.SpawnEffect(this.predictedPositionEffectPrefab, new EffectData
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

		// Token: 0x04002E1E RID: 11806
		public static float entryDuration = 1f;

		// Token: 0x04002E1F RID: 11807
		public static float fireDuration = 2f;

		// Token: 0x04002E20 RID: 11808
		public static float exitDuration = 1f;

		// Token: 0x04002E21 RID: 11809
		[SerializeField]
		public GameObject chargeEffectPrefab;

		// Token: 0x04002E22 RID: 11810
		[SerializeField]
		public GameObject fistEffectPrefab;

		// Token: 0x04002E23 RID: 11811
		[SerializeField]
		public GameObject fireEffectPrefab;

		// Token: 0x04002E24 RID: 11812
		[SerializeField]
		public GameObject predictedPositionEffectPrefab;

		// Token: 0x04002E25 RID: 11813
		public static float maxDistance = 40f;

		// Token: 0x04002E26 RID: 11814
		public static float trackingDuration = 0.5f;

		// Token: 0x04002E27 RID: 11815
		public static float fistDamageCoefficient = 2f;

		// Token: 0x04002E28 RID: 11816
		public static float fistForce = 2000f;

		// Token: 0x04002E29 RID: 11817
		public static float fistVerticalForce;

		// Token: 0x04002E2A RID: 11818
		public static float fistRadius = 5f;

		// Token: 0x04002E2B RID: 11819
		public static string chargeFistAttackSoundString;

		// Token: 0x04002E2C RID: 11820
		private bool hasShownPrediction;

		// Token: 0x04002E2D RID: 11821
		private bool predictionOk;

		// Token: 0x04002E2E RID: 11822
		protected Vector3 predictedTargetPosition;

		// Token: 0x04002E2F RID: 11823
		private AimAnimator aimAnimator;

		// Token: 0x04002E30 RID: 11824
		private GameObject chargeEffect;

		// Token: 0x04002E31 RID: 11825
		private Transform fistTransform;

		// Token: 0x04002E32 RID: 11826
		private float stopwatch;

		// Token: 0x04002E33 RID: 11827
		private FireFist.SubState subState;

		// Token: 0x04002E34 RID: 11828
		private FireFist.Predictor predictor;

		// Token: 0x04002E35 RID: 11829
		private GameObject predictorDebug;

		// Token: 0x02000854 RID: 2132
		private enum SubState
		{
			// Token: 0x04002E37 RID: 11831
			Prep,
			// Token: 0x04002E38 RID: 11832
			FireFist,
			// Token: 0x04002E39 RID: 11833
			Exit
		}

		// Token: 0x02000855 RID: 2133
		private class Predictor
		{
			// Token: 0x06003032 RID: 12338 RVA: 0x000CF039 File Offset: 0x000CD239
			public Predictor(Transform bodyTransform)
			{
				this.bodyTransform = bodyTransform;
			}

			// Token: 0x17000442 RID: 1090
			// (get) Token: 0x06003033 RID: 12339 RVA: 0x000CF048 File Offset: 0x000CD248
			public bool hasTargetTransform
			{
				get
				{
					return this.targetTransform;
				}
			}

			// Token: 0x17000443 RID: 1091
			// (get) Token: 0x06003034 RID: 12340 RVA: 0x000CF055 File Offset: 0x000CD255
			public bool isPredictionReady
			{
				get
				{
					return this.collectedPositions > 2;
				}
			}

			// Token: 0x06003035 RID: 12341 RVA: 0x000CF060 File Offset: 0x000CD260
			private void PushTargetPosition(Vector3 newTargetPosition)
			{
				this.targetPosition2 = this.targetPosition1;
				this.targetPosition1 = this.targetPosition0;
				this.targetPosition0 = newTargetPosition;
				this.collectedPositions++;
			}

			// Token: 0x06003036 RID: 12342 RVA: 0x000CF090 File Offset: 0x000CD290
			public void SetTargetTransform(Transform newTargetTransform)
			{
				this.targetTransform = newTargetTransform;
				this.targetPosition2 = (this.targetPosition1 = (this.targetPosition0 = newTargetTransform.position));
				this.collectedPositions = 1;
			}

			// Token: 0x06003037 RID: 12343 RVA: 0x000CF0C9 File Offset: 0x000CD2C9
			public void Update()
			{
				if (this.targetTransform)
				{
					this.PushTargetPosition(this.targetTransform.position);
				}
			}

			// Token: 0x06003038 RID: 12344 RVA: 0x000CF0EC File Offset: 0x000CD2EC
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

			// Token: 0x04002E3A RID: 11834
			private Transform bodyTransform;

			// Token: 0x04002E3B RID: 11835
			private Transform targetTransform;

			// Token: 0x04002E3C RID: 11836
			private Vector3 targetPosition0;

			// Token: 0x04002E3D RID: 11837
			private Vector3 targetPosition1;

			// Token: 0x04002E3E RID: 11838
			private Vector3 targetPosition2;

			// Token: 0x04002E3F RID: 11839
			private int collectedPositions;

			// Token: 0x02000856 RID: 2134
			private enum ExtrapolationType
			{
				// Token: 0x04002E41 RID: 11841
				None,
				// Token: 0x04002E42 RID: 11842
				Linear,
				// Token: 0x04002E43 RID: 11843
				Polar
			}
		}
	}
}
