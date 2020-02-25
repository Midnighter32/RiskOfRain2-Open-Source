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
	// Token: 0x02000385 RID: 901
	[RequireComponent(typeof(CharacterBody))]
	public class WormBodyPositions2 : NetworkBehaviour, ITeleportHandler, IEventSystemHandler, ILifeBehavior, IPainAnimationHandler
	{
		// Token: 0x060015E4 RID: 5604 RVA: 0x0005D7E4 File Offset: 0x0005B9E4
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

		// Token: 0x060015E5 RID: 5605 RVA: 0x0005D882 File Offset: 0x0005BA82
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.PopulateInitialKeyFrames();
				this.previousSurfaceTestEnd = this.chasePosition;
				this.velocity = this.characterDirection.forward;
			}
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x0005D8AE File Offset: 0x0005BAAE
		private void OnDestroy()
		{
			this.travelCallbacks = null;
			Util.PlaySound("Stop_magmaWorm_burrowed_loop", this.bones[0].gameObject);
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x0005D8D0 File Offset: 0x0005BAD0
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

		// Token: 0x060015E8 RID: 5608 RVA: 0x0005D954 File Offset: 0x0005BB54
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

		// Token: 0x060015E9 RID: 5609 RVA: 0x0005D9EC File Offset: 0x0005BBEC
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

		// Token: 0x060015EA RID: 5610 RVA: 0x0005DAB0 File Offset: 0x0005BCB0
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

		// Token: 0x060015EB RID: 5611 RVA: 0x0005DC2C File Offset: 0x0005BE2C
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

		// Token: 0x060015EC RID: 5612 RVA: 0x0005DCF0 File Offset: 0x0005BEF0
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

		// Token: 0x060015ED RID: 5613 RVA: 0x0005DE10 File Offset: 0x0005C010
		[ClientRpc]
		private void RpcSendKeyFrame(WormBodyPositions2.KeyFrame newKeyFrame)
		{
			if (!NetworkServer.active)
			{
				this.AddKeyFrame(ref newKeyFrame);
			}
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x0005DE21 File Offset: 0x0005C021
		private void Update()
		{
			this.UpdateBoneDisplacements(Time.deltaTime);
			this.UpdateHeadOffset();
			if (this.animateJaws)
			{
				this.UpdateJaws();
			}
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0005DE44 File Offset: 0x0005C044
		private void UpdateJaws()
		{
			if (this.animator)
			{
				float value = Mathf.Clamp01(Util.Remap((this.bones[0].position - base.transform.position).magnitude, this.jawClosedDistance, this.jawOpenDistance, 0f, 1f));
				this.animator.SetFloat(this.jawMecanimCycleParameter, value, this.jawMecanimDampTime, Time.deltaTime);
			}
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x0005DEC4 File Offset: 0x0005C0C4
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

		// Token: 0x060015F1 RID: 5617 RVA: 0x0005DF69 File Offset: 0x0005C169
		private void OnTravel(float distance)
		{
			this.headDistance -= distance;
			this.UpdateBones();
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x0005DF80 File Offset: 0x0005C180
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

		// Token: 0x060015F3 RID: 5619 RVA: 0x0005E080 File Offset: 0x0005C280
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

		// Token: 0x060015F4 RID: 5620 RVA: 0x0005E0DC File Offset: 0x0005C2DC
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

		// Token: 0x060015F5 RID: 5621 RVA: 0x0005E18C File Offset: 0x0005C38C
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
				EffectManager.SpawnEffect(this.warningEffectPrefab, new EffectData
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

		// Token: 0x060015F6 RID: 5622 RVA: 0x0005E274 File Offset: 0x0005C474
		[ClientRpc]
		private void RpcPlaySurfaceImpactSound()
		{
			Util.PlaySound("Play_magmaWorm_M1", this.bones[0].gameObject);
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x0005E290 File Offset: 0x0005C490
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
			EffectManager.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			if (this.shouldFireMeatballsOnImpact)
			{
				this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
			}
			if (this.shouldFireBlastAttackOnImpact)
			{
				this.FireImpactBlastAttack(point + surfaceNormal);
			}
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x0005E37B File Offset: 0x0005C57B
		public void OnDeathStart()
		{
			this.deathTime = Run.instance.fixedTime;
			this.shouldTriggerDeathEffectOnNextImpact = true;
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x0005E394 File Offset: 0x0005C594
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
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MagmaWormDeathDust"), new EffectData
					{
						origin = this.bones[i].position,
						rotation = UnityEngine.Random.rotation,
						scale = 1f
					}, true);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x0005E424 File Offset: 0x0005C624
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
			EffectManager.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			if (this.shouldFireMeatballsOnImpact)
			{
				this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
			}
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x0005E4CC File Offset: 0x0005C6CC
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
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0005E580 File Offset: 0x0005C780
		private void FireImpactBlastAttack(Vector3 impactPosition)
		{
			BlastAttack blastAttack = new BlastAttack();
			blastAttack.baseDamage = this.characterBody.damage * this.blastAttackDamageCoefficient;
			blastAttack.procCoefficient = this.blastAttackProcCoefficient;
			blastAttack.baseForce = this.blastAttackForce;
			blastAttack.bonusForce = Vector3.up * this.blastAttackBonusVerticalForce;
			blastAttack.crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
			blastAttack.radius = this.blastAttackRadius;
			blastAttack.damageType = DamageType.PercentIgniteOnHit;
			blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
			blastAttack.attacker = base.gameObject;
			blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
			blastAttack.position = impactPosition;
			blastAttack.Fire();
			if (NetworkServer.active)
			{
				EffectManager.SpawnEffect(this.blastAttackEffect, new EffectData
				{
					origin = impactPosition,
					scale = this.blastAttackRadius
				}, true);
			}
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x0005E66C File Offset: 0x0005C86C
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.enterTriggerCooldownTimer -= Time.fixedDeltaTime;
				this.exitTriggerCooldownTimer -= Time.fixedDeltaTime;
				Vector3 position = this.referenceTransform.position;
				float d = this.speedMultiplier;
				Vector3 normalized = (position - this.chasePosition).normalized;
				float num = (this.underground ? this.maxTurnSpeed : (this.maxTurnSpeed * this.turnRateCoefficientAboveGround)) * 0.017453292f;
				Vector3 vector = new Vector3(this.velocity.x, 0f, this.velocity.z);
				Vector3 a = new Vector3(normalized.x, 0f, normalized.z);
				vector = Vector3.RotateTowards(vector, a * d, num * Time.fixedDeltaTime, float.PositiveInfinity);
				vector = vector.normalized * d;
				float num2 = position.y - this.chasePosition.y;
				float num3 = -this.velocity.y * this.yDamperConstant;
				float num4 = num2 * this.ySpringConstant;
				if (this.allowShoving && Mathf.Abs(this.velocity.y) < this.yShoveVelocityThreshold && num2 > this.yShovePositionThreshold)
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

		// Token: 0x060015FE RID: 5630 RVA: 0x0005E994 File Offset: 0x0005CB94
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

		// Token: 0x060015FF RID: 5631 RVA: 0x0005EA68 File Offset: 0x0005CC68
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

		// Token: 0x06001600 RID: 5632 RVA: 0x0005EB6C File Offset: 0x0005CD6C
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

		// Token: 0x06001601 RID: 5633 RVA: 0x0005EC4C File Offset: 0x0005CE4C
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

		// Token: 0x06001602 RID: 5634 RVA: 0x0005ECA0 File Offset: 0x0005CEA0
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

		// Token: 0x06001603 RID: 5635 RVA: 0x0005ECEC File Offset: 0x0005CEEC
		void IPainAnimationHandler.HandlePain(float damage, Vector3 damagePosition)
		{
			int num = this.FindNearestBone(damagePosition);
			if (num != -1)
			{
				this.boneDisplacements[num] = UnityEngine.Random.onUnitSphere * this.maxPainDisplacementMagnitude;
			}
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0005ED21 File Offset: 0x0005CF21
		private static float GetSynchronizedTimeStamp()
		{
			return Run.instance.time;
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0005ED30 File Offset: 0x0005CF30
		private static void WriteKeyFrame(NetworkWriter writer, WormBodyPositions2.KeyFrame keyFrame)
		{
			writer.Write(keyFrame.curve.a);
			writer.Write(keyFrame.curve.b);
			writer.Write(keyFrame.curve.c);
			writer.Write(keyFrame.curve.d);
			writer.Write(keyFrame.length);
			writer.Write(keyFrame.time);
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x0005ED9C File Offset: 0x0005CF9C
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

		// Token: 0x06001607 RID: 5639 RVA: 0x0005EE14 File Offset: 0x0005D014
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
			return !initialState && syncVarDirtyBits > 0U;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0005EE70 File Offset: 0x0005D070
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

		// Token: 0x0600160A RID: 5642 RVA: 0x0005EF60 File Offset: 0x0005D160
		static WormBodyPositions2()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcSendKeyFrame, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcSendKeyFrame));
			WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound = 2010133795;
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcPlaySurfaceImpactSound));
			NetworkCRC.RegisterBehaviour("WormBodyPositions2", 0);
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x0005EFE9 File Offset: 0x0005D1E9
		protected static void InvokeRpcRpcSendKeyFrame(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSendKeyFrame called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcSendKeyFrame(GeneratedNetworkCode._ReadKeyFrame_WormBodyPositions2(reader));
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0005F012 File Offset: 0x0005D212
		protected static void InvokeRpcRpcPlaySurfaceImpactSound(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcPlaySurfaceImpactSound called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcPlaySurfaceImpactSound();
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x0005F038 File Offset: 0x0005D238
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

		// Token: 0x0600160F RID: 5647 RVA: 0x0005F0AC File Offset: 0x0005D2AC
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

		// Token: 0x04001479 RID: 5241
		private Vector3 velocity;

		// Token: 0x0400147A RID: 5242
		private Vector3 chasePosition;

		// Token: 0x0400147B RID: 5243
		public Transform referenceTransform;

		// Token: 0x0400147C RID: 5244
		public Transform[] bones;

		// Token: 0x0400147D RID: 5245
		private WormBodyPositions2.PositionRotationPair[] boneTransformationBuffer;

		// Token: 0x0400147E RID: 5246
		private Vector3[] boneDisplacements;

		// Token: 0x0400147F RID: 5247
		public float[] segmentLengths;

		// Token: 0x04001480 RID: 5248
		private float headDistance;

		// Token: 0x04001481 RID: 5249
		[Tooltip("How far behind the chaser the head is, in seconds.")]
		public float followDelay = 2f;

		// Token: 0x04001482 RID: 5250
		[Tooltip("Whether or not the jaw will close/open.")]
		public bool animateJaws;

		// Token: 0x04001483 RID: 5251
		public Animator animator;

		// Token: 0x04001484 RID: 5252
		public string jawMecanimCycleParameter;

		// Token: 0x04001485 RID: 5253
		public float jawMecanimDampTime;

		// Token: 0x04001486 RID: 5254
		public float jawClosedDistance;

		// Token: 0x04001487 RID: 5255
		public float jawOpenDistance;

		// Token: 0x04001488 RID: 5256
		public GameObject warningEffectPrefab;

		// Token: 0x04001489 RID: 5257
		public GameObject burrowEffectPrefab;

		// Token: 0x0400148A RID: 5258
		public float maxPainDisplacementMagnitude = 2f;

		// Token: 0x0400148B RID: 5259
		public float painDisplacementRecoverySpeed = 8f;

		// Token: 0x0400148C RID: 5260
		public bool shouldFireMeatballsOnImpact = true;

		// Token: 0x0400148D RID: 5261
		public bool shouldFireBlastAttackOnImpact = true;

		// Token: 0x0400148E RID: 5262
		private float totalLength;

		// Token: 0x0400148F RID: 5263
		private const float endBonusLength = 4f;

		// Token: 0x04001490 RID: 5264
		private const float fakeEndSegmentLength = 2f;

		// Token: 0x04001491 RID: 5265
		private CharacterBody characterBody;

		// Token: 0x04001492 RID: 5266
		private CharacterDirection characterDirection;

		// Token: 0x04001493 RID: 5267
		private ModelLocator modelLocator;

		// Token: 0x04001494 RID: 5268
		private readonly List<WormBodyPositions2.KeyFrame> keyFrames = new List<WormBodyPositions2.KeyFrame>();

		// Token: 0x04001495 RID: 5269
		private float keyFramesTotalLength;

		// Token: 0x04001496 RID: 5270
		private WormBodyPositions2.KeyFrame newestKeyFrame;

		// Token: 0x04001497 RID: 5271
		public float spawnDepth = 30f;

		// Token: 0x04001498 RID: 5272
		private static readonly Quaternion boneFixRotation = Quaternion.Euler(-90f, 0f, 0f);

		// Token: 0x04001499 RID: 5273
		public float keyFrameGenerationInterval = 0.25f;

		// Token: 0x0400149A RID: 5274
		private float keyFrameGenerationTimer;

		// Token: 0x0400149B RID: 5275
		public bool underground;

		// Token: 0x0400149C RID: 5276
		private Collider entranceCollider;

		// Token: 0x0400149D RID: 5277
		private Collider exitCollider;

		// Token: 0x0400149E RID: 5278
		private Vector3 previousSurfaceTestEnd;

		// Token: 0x0400149F RID: 5279
		private List<WormBodyPositions2.TravelCallback> travelCallbacks;

		// Token: 0x040014A0 RID: 5280
		private const float impactSoundPrestartDuration = 0.5f;

		// Token: 0x040014A1 RID: 5281
		public float impactCooldownDuration = 0.1f;

		// Token: 0x040014A2 RID: 5282
		private float enterTriggerCooldownTimer;

		// Token: 0x040014A3 RID: 5283
		private float exitTriggerCooldownTimer;

		// Token: 0x040014A4 RID: 5284
		private bool shouldTriggerDeathEffectOnNextImpact;

		// Token: 0x040014A5 RID: 5285
		private float deathTime = float.NegativeInfinity;

		// Token: 0x040014A6 RID: 5286
		public GameObject meatballProjectile;

		// Token: 0x040014A7 RID: 5287
		public GameObject blastAttackEffect;

		// Token: 0x040014A8 RID: 5288
		public int meatballCount;

		// Token: 0x040014A9 RID: 5289
		public float meatballAngle;

		// Token: 0x040014AA RID: 5290
		public float meatballDamageCoefficient;

		// Token: 0x040014AB RID: 5291
		public float meatballProcCoefficient;

		// Token: 0x040014AC RID: 5292
		public float meatballForce;

		// Token: 0x040014AD RID: 5293
		public float blastAttackDamageCoefficient;

		// Token: 0x040014AE RID: 5294
		public float blastAttackProcCoefficient;

		// Token: 0x040014AF RID: 5295
		public float blastAttackRadius;

		// Token: 0x040014B0 RID: 5296
		public float blastAttackForce;

		// Token: 0x040014B1 RID: 5297
		public float blastAttackBonusVerticalForce;

		// Token: 0x040014B2 RID: 5298
		public Transform chasePositionVisualizer;

		// Token: 0x040014B3 RID: 5299
		public float maxTurnSpeed = 180f;

		// Token: 0x040014B4 RID: 5300
		public float speedMultiplier = 2f;

		// Token: 0x040014B5 RID: 5301
		public float verticalTurnSquashFactor = 2f;

		// Token: 0x040014B6 RID: 5302
		public float ySpringConstant = 100f;

		// Token: 0x040014B7 RID: 5303
		public float yDamperConstant = 1f;

		// Token: 0x040014B8 RID: 5304
		public bool allowShoving;

		// Token: 0x040014B9 RID: 5305
		public float yShoveVelocityThreshold;

		// Token: 0x040014BA RID: 5306
		public float yShovePositionThreshold;

		// Token: 0x040014BB RID: 5307
		public float yShoveForce;

		// Token: 0x040014BC RID: 5308
		public float turnRateCoefficientAboveGround;

		// Token: 0x040014BD RID: 5309
		public float wormForceCoefficientAboveGround;

		// Token: 0x040014BE RID: 5310
		public float undergroundTestYOffset;

		// Token: 0x040014BF RID: 5311
		private bool playingBurrowSound;

		// Token: 0x040014C0 RID: 5312
		private static int kRpcRpcSendKeyFrame = 874152969;

		// Token: 0x040014C1 RID: 5313
		private static int kRpcRpcPlaySurfaceImpactSound;

		// Token: 0x02000386 RID: 902
		private struct PositionRotationPair
		{
			// Token: 0x040014C2 RID: 5314
			public Vector3 position;

			// Token: 0x040014C3 RID: 5315
			public Quaternion rotation;
		}

		// Token: 0x02000387 RID: 903
		[Serializable]
		private struct KeyFrame
		{
			// Token: 0x040014C4 RID: 5316
			public CubicBezier3 curve;

			// Token: 0x040014C5 RID: 5317
			public float length;

			// Token: 0x040014C6 RID: 5318
			public float time;
		}

		// Token: 0x02000388 RID: 904
		private struct TravelCallback
		{
			// Token: 0x040014C7 RID: 5319
			public float time;

			// Token: 0x040014C8 RID: 5320
			public Action callback;
		}
	}
}
