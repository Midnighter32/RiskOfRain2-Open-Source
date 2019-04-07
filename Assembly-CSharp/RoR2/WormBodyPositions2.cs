using System;
using System.Collections.Generic;
using EntityStates.MagmaWorm;
using RoR2.Projectile;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000429 RID: 1065
	[RequireComponent(typeof(CharacterBody))]
	public class WormBodyPositions2 : NetworkBehaviour, ITeleportHandler, IEventSystemHandler, ILifeBehavior, IPainAnimationHandler
	{
		// Token: 0x060017AA RID: 6058 RVA: 0x000704C8 File Offset: 0x0006E6C8
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.boneTransformationBuffer = new WormBodyPositions2.PositionRotationPair[this.bones.Length + 1];
			this.totalLength = 0f;
			for (int i = 0; i < this.segmentLengths.Length; i++)
			{
				this.totalLength += this.segmentLengths[i];
			}
			if (NetworkServer.active)
			{
				this.travelCallbacks = new List<WormBodyPositions2.TravelCallback>();
			}
			this.boneDisplacements = new Vector3[this.bones.Length];
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00070566 File Offset: 0x0006E766
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.PopulateInitialKeyFrames();
				this.previousSurfaceTestEnd = this.chasePosition;
				this.velocity = this.characterDirection.forward;
			}
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00070592 File Offset: 0x0006E792
		private void OnDestroy()
		{
			this.travelCallbacks = null;
			Util.PlaySound("Stop_magmaWorm_burrowed_loop", this.bones[0].gameObject);
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x000705B4 File Offset: 0x0006E7B4
		private void BakeSegmentLengths()
		{
			this.segmentLengths = new float[this.bones.Length];
			Vector3 a = this.bones[0].position;
			for (int i = 1; i < this.bones.Length; i++)
			{
				Vector3 position = this.bones[i].position;
				float magnitude = (a - position).magnitude;
				this.segmentLengths[i - 1] = magnitude;
				a = position;
			}
			this.segmentLengths[this.bones.Length - 1] = 2f;
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00070638 File Offset: 0x0006E838
		[Server]
		private void PopulateInitialKeyFrames()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::PopulateInitialKeyFrames()' called on client");
				return;
			}
			Vector3 vector = base.transform.position + Vector3.down * this.spawnDepth;
			this.chasePosition = vector;
			for (int i = this.bones.Length - 1; i >= 0; i--)
			{
				this.AttemptToGenerateKeyFrame(vector + Vector3.down * (float)(i + 1) * this.segmentLengths[i]);
			}
			this.AttemptToGenerateKeyFrame(vector);
			this.headDistance = 0f;
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000706D0 File Offset: 0x0006E8D0
		private Vector3 EvaluatePositionAlongCurve(float positionDownBody)
		{
			float num = 0f;
			foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
			{
				float b = num;
				num += keyFrame.length;
				if (num >= positionDownBody)
				{
					float t = Mathf.InverseLerp(num, b, positionDownBody);
					CubicBezier3 curve = keyFrame.curve;
					return curve.Evaluate(t);
				}
			}
			if (this.keyFrames.Count > 0)
			{
				return this.keyFrames[this.keyFrames.Count - 1].curve.Evaluate(1f);
			}
			return Vector3.zero;
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00070794 File Offset: 0x0006E994
		private void UpdateBones()
		{
			float num = this.totalLength;
			this.boneTransformationBuffer[this.boneTransformationBuffer.Length - 1] = new WormBodyPositions2.PositionRotationPair
			{
				position = this.EvaluatePositionAlongCurve(this.headDistance + num),
				rotation = Quaternion.identity
			};
			for (int i = this.boneTransformationBuffer.Length - 2; i >= 0; i--)
			{
				num -= this.segmentLengths[i];
				Vector3 vector = this.EvaluatePositionAlongCurve(this.headDistance + num);
				Quaternion rotation = Util.QuaternionSafeLookRotation(vector - this.boneTransformationBuffer[i + 1].position, Vector3.up);
				this.boneTransformationBuffer[i] = new WormBodyPositions2.PositionRotationPair
				{
					position = vector,
					rotation = rotation
				};
			}
			Vector3 forward = this.bones[0].forward;
			for (int j = 0; j < this.bones.Length; j++)
			{
				this.bones[j].position = this.boneTransformationBuffer[j].position + this.boneDisplacements[j];
				this.bones[j].forward = forward;
				this.bones[j].up = this.boneTransformationBuffer[j].rotation * -Vector3.forward;
				forward = this.bones[j].forward;
			}
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x00070910 File Offset: 0x0006EB10
		[Server]
		private void AttemptToGenerateKeyFrame(Vector3 position)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::AttemptToGenerateKeyFrame(UnityEngine.Vector3)' called on client");
				return;
			}
			WormBodyPositions2.KeyFrame keyFrame = this.newestKeyFrame;
			CubicBezier3 curve = CubicBezier3.FromVelocities(keyFrame.curve.p1, -keyFrame.curve.v1, position, -this.velocity * (this.keyFrameGenerationInterval * 0.25f));
			float length = curve.ApproximateLength(50);
			WormBodyPositions2.KeyFrame keyFrame2 = new WormBodyPositions2.KeyFrame
			{
				curve = curve,
				length = length,
				time = WormBodyPositions2.GetSynchronizedTimeStamp()
			};
			if (keyFrame2.length >= 0f)
			{
				this.headDistance += keyFrame2.length;
				this.AddKeyFrame(ref keyFrame2);
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000709D4 File Offset: 0x0006EBD4
		private void AddKeyFrame(ref WormBodyPositions2.KeyFrame newKeyFrame)
		{
			this.newestKeyFrame = newKeyFrame;
			this.keyFrames.Insert(0, newKeyFrame);
			this.keyFramesTotalLength += newKeyFrame.length;
			bool flag = false;
			float num = this.keyFramesTotalLength;
			float num2 = this.totalLength + this.headDistance + 4f;
			while (this.keyFrames.Count > 0 && (num -= this.keyFrames[this.keyFrames.Count - 1].length) > num2)
			{
				this.keyFrames.RemoveAt(this.keyFrames.Count - 1);
				flag = true;
			}
			if (flag)
			{
				this.keyFramesTotalLength = 0f;
				foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
				{
					this.keyFramesTotalLength += keyFrame.length;
				}
			}
			if (NetworkServer.active)
			{
				this.CallRpcSendKeyFrame(newKeyFrame);
			}
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00070AF4 File Offset: 0x0006ECF4
		[ClientRpc]
		private void RpcSendKeyFrame(WormBodyPositions2.KeyFrame newKeyFrame)
		{
			if (!NetworkServer.active)
			{
				this.AddKeyFrame(ref newKeyFrame);
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x00070B05 File Offset: 0x0006ED05
		private void Update()
		{
			this.UpdateBoneDisplacements(Time.deltaTime);
			this.UpdateHeadOffset();
			if (this.animateJaws)
			{
				this.UpdateJaws();
			}
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x00070B28 File Offset: 0x0006ED28
		private void UpdateJaws()
		{
			if (this.animator)
			{
				float value = Mathf.Clamp01(Util.Remap((this.bones[0].position - base.transform.position).magnitude, this.jawClosedDistance, this.jawOpenDistance, 0f, 1f));
				this.animator.SetFloat(this.jawMecanimCycleParameter, value, this.jawMecanimDampTime, Time.deltaTime);
			}
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x00070BA8 File Offset: 0x0006EDA8
		private void UpdateHeadOffset()
		{
			float num = this.headDistance;
			int num2 = this.keyFrames.Count - 1;
			float num3 = 0f;
			float num4 = WormBodyPositions2.GetSynchronizedTimeStamp() - this.followDelay;
			for (int i = 0; i < num2; i++)
			{
				float time = this.keyFrames[i + 1].time;
				float length = this.keyFrames[i].length;
				if (time < num4)
				{
					num = num3 + length * Mathf.InverseLerp(this.keyFrames[i].time, time, num4);
					break;
				}
				num3 += length;
			}
			this.OnTravel(this.headDistance - num);
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00070C4D File Offset: 0x0006EE4D
		private void OnTravel(float distance)
		{
			this.headDistance -= distance;
			this.UpdateBones();
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00070C64 File Offset: 0x0006EE64
		[Server]
		private void SurfaceTest()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::SurfaceTest()' called on client");
				return;
			}
			Vector3 vector = this.chasePosition;
			Vector3 vector2 = vector - this.previousSurfaceTestEnd;
			float magnitude = vector2.magnitude;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.previousSurfaceTestEnd, vector2, out raycastHit, magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				if (!this.entranceCollider)
				{
					this.OnChaserEnterSurface(raycastHit.point, raycastHit.normal);
				}
				this.entranceCollider = raycastHit.collider;
			}
			else
			{
				this.entranceCollider = null;
			}
			if (Physics.Raycast(vector, -vector2, out raycastHit, magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				if (!this.exitCollider)
				{
					this.OnChaserExitSurface(raycastHit.point, raycastHit.normal);
				}
				this.exitCollider = raycastHit.collider;
			}
			else
			{
				this.exitCollider = null;
			}
			this.previousSurfaceTestEnd = vector;
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00070D64 File Offset: 0x0006EF64
		private void AddTravelCallback(WormBodyPositions2.TravelCallback newTravelCallback)
		{
			int index = this.travelCallbacks.Count;
			float time = newTravelCallback.time;
			for (int i = 0; i < this.travelCallbacks.Count; i++)
			{
				if (time < this.travelCallbacks[i].time)
				{
					index = i;
					break;
				}
			}
			this.travelCallbacks.Insert(index, newTravelCallback);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00070DC0 File Offset: 0x0006EFC0
		[Server]
		private void OnChaserEnterSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnChaserEnterSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay,
				callback = delegate()
				{
					this.OnEnterSurface(point, surfaceNormal);
				}
			});
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay - 0.5f,
				callback = new Action(this.RpcPlaySurfaceImpactSound)
			});
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00070E70 File Offset: 0x0006F070
		[Server]
		private void OnChaserExitSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnChaserExitSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.warningEffectPrefab)
			{
				EffectManager.instance.SpawnEffect(this.warningEffectPrefab, new EffectData
				{
					origin = point,
					rotation = Util.QuaternionSafeLookRotation(surfaceNormal)
				}, true);
			}
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay,
				callback = delegate()
				{
					this.OnExitSurface(point, surfaceNormal);
				}
			});
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay - 0.5f,
				callback = new Action(this.RpcPlaySurfaceImpactSound)
			});
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00070F5D File Offset: 0x0006F15D
		[ClientRpc]
		private void RpcPlaySurfaceImpactSound()
		{
			Util.PlaySound("Play_magmaWorm_M1", this.bones[0].gameObject);
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00070F78 File Offset: 0x0006F178
		[Server]
		private void OnEnterSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnEnterSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.enterTriggerCooldownTimer > 0f)
			{
				return;
			}
			if (this.shouldTriggerDeathEffectOnNextImpact && Run.instance.fixedTime - this.deathTime >= DeathState.duration - 3f)
			{
				this.shouldTriggerDeathEffectOnNextImpact = false;
				return;
			}
			this.enterTriggerCooldownTimer = this.impactCooldownDuration;
			EffectManager.instance.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x0007104B File Offset: 0x0006F24B
		public void OnDeathStart()
		{
			this.deathTime = Run.instance.fixedTime;
			this.shouldTriggerDeathEffectOnNextImpact = true;
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x00071064 File Offset: 0x0006F264
		[Server]
		private void PerformDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::PerformDeath()' called on client");
				return;
			}
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i])
				{
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MagmaWormDeathDust"), new EffectData
					{
						origin = this.bones[i].position,
						rotation = UnityEngine.Random.rotation,
						scale = 1f
					}, true);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x000710F8 File Offset: 0x0006F2F8
		[Server]
		private void OnExitSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnExitSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.exitTriggerCooldownTimer > 0f)
			{
				return;
			}
			this.exitTriggerCooldownTimer = this.impactCooldownDuration;
			EffectManager.instance.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x000711A0 File Offset: 0x0006F3A0
		private void FireMeatballs(Vector3 impactNormal, Vector3 impactPosition, Vector3 forward, int meatballCount, float meatballAngle, float meatballForce)
		{
			float num = 360f / (float)meatballCount;
			Vector3 normalized = Vector3.ProjectOnPlane(forward, impactNormal).normalized;
			Vector3 point = Vector3.RotateTowards(impactNormal, normalized, meatballAngle * 0.017453292f, float.PositiveInfinity);
			for (int i = 0; i < meatballCount; i++)
			{
				Vector3 forward2 = Quaternion.AngleAxis(num * (float)i, impactNormal) * point;
				ProjectileManager.instance.FireProjectile(this.meatballProjectile, impactPosition, Util.QuaternionSafeLookRotation(forward2), base.gameObject, this.characterBody.damage * this.meatballDamageCoefficient, meatballForce, Util.CheckRoll(this.characterBody.crit, this.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			BlastAttack blastAttack = new BlastAttack();
			blastAttack.baseDamage = this.characterBody.damage * this.blastAttackDamageCoefficient;
			blastAttack.procCoefficient = this.blastAttackProcCoefficient;
			blastAttack.baseForce = this.blastAttackForce;
			blastAttack.bonusForce = Vector3.up * this.blastAttackBonusVerticalForce;
			blastAttack.crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
			blastAttack.radius = this.blastAttackRadius;
			blastAttack.damageType = DamageType.IgniteOnHit;
			blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
			blastAttack.attacker = base.gameObject;
			blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
			blastAttack.position = impactPosition;
			blastAttack.Fire();
			if (NetworkServer.active)
			{
				EffectManager.instance.SpawnEffect(this.blastAttackEffect, new EffectData
				{
					origin = impactPosition,
					scale = this.blastAttackRadius
				}, true);
			}
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x00071338 File Offset: 0x0006F538
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.enterTriggerCooldownTimer -= Time.fixedDeltaTime;
				this.exitTriggerCooldownTimer -= Time.fixedDeltaTime;
				Vector3 position = this.referenceTransform.position;
				float d = this.characterBody.moveSpeed * this.speedMultiplier;
				Vector3 normalized = (position - this.chasePosition).normalized;
				float num = (this.underground ? this.maxTurnSpeed : (this.maxTurnSpeed * this.turnRateCoefficientAboveGround)) * 0.017453292f;
				Vector3 vector = new Vector3(this.velocity.x, 0f, this.velocity.z);
				Vector3 a = new Vector3(normalized.x, 0f, normalized.z);
				vector = Vector3.RotateTowards(vector, a * d, num * Time.fixedDeltaTime, float.PositiveInfinity);
				vector = vector.normalized * d;
				float num2 = position.y - this.chasePosition.y;
				float num3 = -this.velocity.y * this.yDamperConstant;
				float num4 = num2 * this.ySpringConstant;
				if (Mathf.Abs(this.velocity.y) < this.yShoveVelocityThreshold && Mathf.Abs(num2) < this.yShovePositionThreshold)
				{
					this.velocity.y = this.velocity.y + this.yShoveForce * Time.fixedDeltaTime;
				}
				if (!this.underground)
				{
					num4 *= this.wormForceCoefficientAboveGround;
					num3 *= this.wormForceCoefficientAboveGround;
				}
				this.velocity.y = this.velocity.y + (num4 + num3) * Time.fixedDeltaTime;
				this.velocity += Physics.gravity * Time.fixedDeltaTime;
				this.velocity = new Vector3(vector.x, this.velocity.y, vector.z);
				this.chasePosition += this.velocity * Time.fixedDeltaTime;
				this.chasePositionVisualizer.position = this.chasePosition;
				this.underground = (-num2 < this.undergroundTestYOffset);
				this.keyFrameGenerationTimer -= Time.deltaTime;
				if (this.keyFrameGenerationTimer <= 0f)
				{
					this.keyFrameGenerationTimer = this.keyFrameGenerationInterval;
					this.AttemptToGenerateKeyFrame(this.chasePosition);
				}
				this.SurfaceTest();
				float synchronizedTimeStamp = WormBodyPositions2.GetSynchronizedTimeStamp();
				while (this.travelCallbacks.Count > 0 && this.travelCallbacks[0].time <= synchronizedTimeStamp)
				{
					ref WormBodyPositions2.TravelCallback ptr = this.travelCallbacks[0];
					this.travelCallbacks.RemoveAt(0);
					ptr.callback();
				}
			}
			bool flag = this.bones[0].transform.position.y - base.transform.position.y < this.undergroundTestYOffset;
			if (flag != this.playingBurrowSound)
			{
				if (flag)
				{
					Util.PlaySound("Play_magmaWorm_burrowed_loop", this.bones[0].gameObject);
				}
				else
				{
					Util.PlaySound("Stop_magmaWorm_burrowed_loop", this.bones[0].gameObject);
				}
				this.playingBurrowSound = flag;
			}
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x00071668 File Offset: 0x0006F868
		private void DrawKeyFrame(WormBodyPositions2.KeyFrame keyFrame)
		{
			Gizmos.color = Color.Lerp(Color.green, Color.black, 0.5f);
			Gizmos.DrawRay(keyFrame.curve.p0, keyFrame.curve.v0);
			Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f);
			Gizmos.DrawRay(keyFrame.curve.p1, keyFrame.curve.v1);
			for (int i = 1; i <= 20; i++)
			{
				float num = (float)i * 0.05f;
				Gizmos.color = Color.Lerp(Color.red, Color.green, num);
				Vector3 vector = keyFrame.curve.Evaluate(num - 0.05f);
				Vector3 a = keyFrame.curve.Evaluate(num);
				Gizmos.DrawRay(vector, a - vector);
			}
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x0007173C File Offset: 0x0006F93C
		private void OnDrawGizmos()
		{
			foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
			{
				this.DrawKeyFrame(keyFrame);
			}
			for (int i = 0; i < this.boneTransformationBuffer.Length; i++)
			{
				Gizmos.matrix = Matrix4x4.TRS(this.boneTransformationBuffer[i].position, this.boneTransformationBuffer[i].rotation, Vector3.one * 3f);
				Gizmos.DrawRay(-Vector3.forward, Vector3.forward * 2f);
				Gizmos.DrawRay(-Vector3.right, Vector3.right * 2f);
				Gizmos.DrawRay(-Vector3.up, Vector3.up * 2f);
			}
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00071840 File Offset: 0x0006FA40
		public void OnTeleport(Vector3 oldPosition, Vector3 newPosition)
		{
			Vector3 b = newPosition - oldPosition;
			for (int i = 0; i < this.keyFrames.Count; i++)
			{
				WormBodyPositions2.KeyFrame keyFrame = this.keyFrames[i];
				CubicBezier3 curve = keyFrame.curve;
				curve.a += b;
				curve.b += b;
				curve.c += b;
				curve.d += b;
				keyFrame.curve = curve;
				this.keyFrames[i] = keyFrame;
			}
			this.chasePosition += b;
			this.previousSurfaceTestEnd += b;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00071920 File Offset: 0x0006FB20
		private int FindNearestBone(Vector3 worldPosition)
		{
			int result = -1;
			float num = float.PositiveInfinity;
			for (int i = 0; i < this.bones.Length; i++)
			{
				float sqrMagnitude = (this.bones[i].transform.position - worldPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x00071974 File Offset: 0x0006FB74
		private void UpdateBoneDisplacements(float deltaTime)
		{
			int i = 0;
			int num = this.boneDisplacements.Length;
			while (i < num)
			{
				this.boneDisplacements[i] = Vector3.MoveTowards(this.boneDisplacements[i], Vector3.zero, this.painDisplacementRecoverySpeed * deltaTime);
				i++;
			}
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x000719C0 File Offset: 0x0006FBC0
		void IPainAnimationHandler.HandlePain(float damage, Vector3 damagePosition)
		{
			int num = this.FindNearestBone(damagePosition);
			if (num != -1)
			{
				this.boneDisplacements[num] = UnityEngine.Random.onUnitSphere * this.maxPainDisplacementMagnitude;
			}
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x000719F5 File Offset: 0x0006FBF5
		private static float GetSynchronizedTimeStamp()
		{
			return Run.instance.time;
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x00071A04 File Offset: 0x0006FC04
		private static void WriteKeyFrame(NetworkWriter writer, WormBodyPositions2.KeyFrame keyFrame)
		{
			writer.Write(keyFrame.curve.a);
			writer.Write(keyFrame.curve.b);
			writer.Write(keyFrame.curve.c);
			writer.Write(keyFrame.curve.d);
			writer.Write(keyFrame.length);
			writer.Write(keyFrame.time);
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x00071A70 File Offset: 0x0006FC70
		private static WormBodyPositions2.KeyFrame ReadKeyFrame(NetworkReader reader)
		{
			WormBodyPositions2.KeyFrame result = default(WormBodyPositions2.KeyFrame);
			result.curve.a = reader.ReadVector3();
			result.curve.b = reader.ReadVector3();
			result.curve.c = reader.ReadVector3();
			result.curve.d = reader.ReadVector3();
			result.length = reader.ReadSingle();
			result.time = reader.ReadSingle();
			return result;
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x00071AE8 File Offset: 0x0006FCE8
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint syncVarDirtyBits = base.syncVarDirtyBits;
			if (initialState)
			{
				writer.Write((ushort)this.keyFrames.Count);
				for (int i = 0; i < this.keyFrames.Count; i++)
				{
					WormBodyPositions2.WriteKeyFrame(writer, this.keyFrames[i]);
				}
			}
			return !initialState && syncVarDirtyBits > 0u;
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x00071B44 File Offset: 0x0006FD44
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.keyFrames.Clear();
				int num = (int)reader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					this.keyFrames.Add(WormBodyPositions2.ReadKeyFrame(reader));
				}
			}
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00071C28 File Offset: 0x0006FE28
		static WormBodyPositions2()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcSendKeyFrame, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcSendKeyFrame));
			WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound = 2010133795;
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcPlaySurfaceImpactSound));
			NetworkCRC.RegisterBehaviour("WormBodyPositions2", 0);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x00071CB1 File Offset: 0x0006FEB1
		protected static void InvokeRpcRpcSendKeyFrame(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSendKeyFrame called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcSendKeyFrame(GeneratedNetworkCode._ReadKeyFrame_WormBodyPositions2(reader));
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00071CDA File Offset: 0x0006FEDA
		protected static void InvokeRpcRpcPlaySurfaceImpactSound(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcPlaySurfaceImpactSound called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcPlaySurfaceImpactSound();
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x00071D00 File Offset: 0x0006FF00
		public void CallRpcSendKeyFrame(WormBodyPositions2.KeyFrame newKeyFrame)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSendKeyFrame called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)WormBodyPositions2.kRpcRpcSendKeyFrame);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteKeyFrame_WormBodyPositions2(networkWriter, newKeyFrame);
			this.SendRPCInternal(networkWriter, 0, "RpcSendKeyFrame");
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00071D74 File Offset: 0x0006FF74
		public void CallRpcPlaySurfaceImpactSound()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcPlaySurfaceImpactSound called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcPlaySurfaceImpactSound");
		}

		// Token: 0x04001AE3 RID: 6883
		private Vector3 velocity;

		// Token: 0x04001AE4 RID: 6884
		private Vector3 chasePosition;

		// Token: 0x04001AE5 RID: 6885
		public Transform referenceTransform;

		// Token: 0x04001AE6 RID: 6886
		public Transform[] bones;

		// Token: 0x04001AE7 RID: 6887
		private WormBodyPositions2.PositionRotationPair[] boneTransformationBuffer;

		// Token: 0x04001AE8 RID: 6888
		private Vector3[] boneDisplacements;

		// Token: 0x04001AE9 RID: 6889
		public float[] segmentLengths;

		// Token: 0x04001AEA RID: 6890
		private float headDistance;

		// Token: 0x04001AEB RID: 6891
		[Tooltip("How far behind the chaser the head is, in seconds.")]
		public float followDelay = 2f;

		// Token: 0x04001AEC RID: 6892
		[Tooltip("Whether or not the jaw will close/open.")]
		public bool animateJaws;

		// Token: 0x04001AED RID: 6893
		public Animator animator;

		// Token: 0x04001AEE RID: 6894
		public string jawMecanimCycleParameter;

		// Token: 0x04001AEF RID: 6895
		public float jawMecanimDampTime;

		// Token: 0x04001AF0 RID: 6896
		public float jawClosedDistance;

		// Token: 0x04001AF1 RID: 6897
		public float jawOpenDistance;

		// Token: 0x04001AF2 RID: 6898
		public GameObject warningEffectPrefab;

		// Token: 0x04001AF3 RID: 6899
		public GameObject burrowEffectPrefab;

		// Token: 0x04001AF4 RID: 6900
		public float maxPainDisplacementMagnitude = 2f;

		// Token: 0x04001AF5 RID: 6901
		public float painDisplacementRecoverySpeed = 8f;

		// Token: 0x04001AF6 RID: 6902
		private float totalLength;

		// Token: 0x04001AF7 RID: 6903
		private const float endBonusLength = 4f;

		// Token: 0x04001AF8 RID: 6904
		private const float fakeEndSegmentLength = 2f;

		// Token: 0x04001AF9 RID: 6905
		private CharacterBody characterBody;

		// Token: 0x04001AFA RID: 6906
		private CharacterDirection characterDirection;

		// Token: 0x04001AFB RID: 6907
		private ModelLocator modelLocator;

		// Token: 0x04001AFC RID: 6908
		private readonly List<WormBodyPositions2.KeyFrame> keyFrames = new List<WormBodyPositions2.KeyFrame>();

		// Token: 0x04001AFD RID: 6909
		private float keyFramesTotalLength;

		// Token: 0x04001AFE RID: 6910
		private WormBodyPositions2.KeyFrame newestKeyFrame;

		// Token: 0x04001AFF RID: 6911
		public float spawnDepth = 30f;

		// Token: 0x04001B00 RID: 6912
		private static readonly Quaternion boneFixRotation = Quaternion.Euler(-90f, 0f, 0f);

		// Token: 0x04001B01 RID: 6913
		public float keyFrameGenerationInterval = 0.25f;

		// Token: 0x04001B02 RID: 6914
		private float keyFrameGenerationTimer;

		// Token: 0x04001B03 RID: 6915
		public bool underground;

		// Token: 0x04001B04 RID: 6916
		private Collider entranceCollider;

		// Token: 0x04001B05 RID: 6917
		private Collider exitCollider;

		// Token: 0x04001B06 RID: 6918
		private Vector3 previousSurfaceTestEnd;

		// Token: 0x04001B07 RID: 6919
		private List<WormBodyPositions2.TravelCallback> travelCallbacks;

		// Token: 0x04001B08 RID: 6920
		private const float impactSoundPrestartDuration = 0.5f;

		// Token: 0x04001B09 RID: 6921
		public float impactCooldownDuration = 0.1f;

		// Token: 0x04001B0A RID: 6922
		private float enterTriggerCooldownTimer;

		// Token: 0x04001B0B RID: 6923
		private float exitTriggerCooldownTimer;

		// Token: 0x04001B0C RID: 6924
		private bool shouldTriggerDeathEffectOnNextImpact;

		// Token: 0x04001B0D RID: 6925
		private float deathTime = float.NegativeInfinity;

		// Token: 0x04001B0E RID: 6926
		public GameObject meatballProjectile;

		// Token: 0x04001B0F RID: 6927
		public GameObject blastAttackEffect;

		// Token: 0x04001B10 RID: 6928
		public int meatballCount;

		// Token: 0x04001B11 RID: 6929
		public float meatballAngle;

		// Token: 0x04001B12 RID: 6930
		public float meatballDamageCoefficient;

		// Token: 0x04001B13 RID: 6931
		public float meatballProcCoefficient;

		// Token: 0x04001B14 RID: 6932
		public float meatballForce;

		// Token: 0x04001B15 RID: 6933
		public float blastAttackDamageCoefficient;

		// Token: 0x04001B16 RID: 6934
		public float blastAttackProcCoefficient;

		// Token: 0x04001B17 RID: 6935
		public float blastAttackRadius;

		// Token: 0x04001B18 RID: 6936
		public float blastAttackForce;

		// Token: 0x04001B19 RID: 6937
		public float blastAttackBonusVerticalForce;

		// Token: 0x04001B1A RID: 6938
		public Transform chasePositionVisualizer;

		// Token: 0x04001B1B RID: 6939
		public float maxTurnSpeed = 180f;

		// Token: 0x04001B1C RID: 6940
		public float speedMultiplier = 2f;

		// Token: 0x04001B1D RID: 6941
		public float verticalTurnSquashFactor = 2f;

		// Token: 0x04001B1E RID: 6942
		public float ySpringConstant = 100f;

		// Token: 0x04001B1F RID: 6943
		public float yDamperConstant = 1f;

		// Token: 0x04001B20 RID: 6944
		public float yShoveVelocityThreshold;

		// Token: 0x04001B21 RID: 6945
		public float yShovePositionThreshold;

		// Token: 0x04001B22 RID: 6946
		public float yShoveForce;

		// Token: 0x04001B23 RID: 6947
		public float turnRateCoefficientAboveGround;

		// Token: 0x04001B24 RID: 6948
		public float wormForceCoefficientAboveGround;

		// Token: 0x04001B25 RID: 6949
		public float undergroundTestYOffset;

		// Token: 0x04001B26 RID: 6950
		private bool playingBurrowSound;

		// Token: 0x04001B27 RID: 6951
		private static int kRpcRpcSendKeyFrame = 874152969;

		// Token: 0x04001B28 RID: 6952
		private static int kRpcRpcPlaySurfaceImpactSound;

		// Token: 0x0200042A RID: 1066
		private struct PositionRotationPair
		{
			// Token: 0x04001B29 RID: 6953
			public Vector3 position;

			// Token: 0x04001B2A RID: 6954
			public Quaternion rotation;
		}

		// Token: 0x0200042B RID: 1067
		[Serializable]
		private struct KeyFrame
		{
			// Token: 0x04001B2B RID: 6955
			public CubicBezier3 curve;

			// Token: 0x04001B2C RID: 6956
			public float length;

			// Token: 0x04001B2D RID: 6957
			public float time;
		}

		// Token: 0x0200042C RID: 1068
		private struct TravelCallback
		{
			// Token: 0x04001B2E RID: 6958
			public float time;

			// Token: 0x04001B2F RID: 6959
			public Action callback;
		}
	}
}
