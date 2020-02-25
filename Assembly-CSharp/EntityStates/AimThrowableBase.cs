using System;
using RoR2;
using RoR2.Projectile;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020006F5 RID: 1781
	public abstract class AimThrowableBase : BaseSkillState
	{
		// Token: 0x0600295A RID: 10586 RVA: 0x000ADDA8 File Offset: 0x000ABFA8
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.arcVisualizerPrefab)
			{
				this.arcVisualizerLineRenderer = UnityEngine.Object.Instantiate<GameObject>(this.arcVisualizerPrefab, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
				this.calculateArcPointsJob = default(AimThrowableBase.CalculateArcPointsJob);
				this.completeArcPointsVisualizerJobMethod = new Action(this.CompleteArcVisualizerJob);
				RoR2Application.onLateUpdate += this.completeArcPointsVisualizerJobMethod;
			}
			if (this.endpointVisualizerPrefab)
			{
				this.endpointVisualizerTransform = UnityEngine.Object.Instantiate<GameObject>(this.endpointVisualizerPrefab, base.transform.position, Quaternion.identity).transform;
			}
			if (base.characterBody)
			{
				base.characterBody.hideCrosshair = true;
			}
			ProjectileSimple component = this.projectilePrefab.GetComponent<ProjectileSimple>();
			if (component)
			{
				this.projectileBaseSpeed = component.velocity;
			}
			Rigidbody component2 = this.projectilePrefab.GetComponent<Rigidbody>();
			if (component2)
			{
				this.useGravity = component2.useGravity;
			}
			this.minimumDuration = this.baseMinimumDuration / this.attackSpeedStat;
			ProjectileImpactExplosion component3 = this.projectilePrefab.GetComponent<ProjectileImpactExplosion>();
			if (component3)
			{
				this.detonationRadius = component3.blastRadius;
				if (this.endpointVisualizerTransform)
				{
					this.endpointVisualizerTransform.localScale = new Vector3(this.detonationRadius, this.detonationRadius, this.detonationRadius);
				}
			}
			this.UpdateVisualizers(this.currentTrajectoryInfo);
			SceneCamera.onSceneCameraPreRender += this.OnPreRenderSceneCam;
		}

		// Token: 0x0600295B RID: 10587 RVA: 0x000ADF24 File Offset: 0x000AC124
		public override void OnExit()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
			if (!this.outer.destroying)
			{
				if (base.isAuthority)
				{
					this.FireProjectile();
				}
				this.OnProjectileFiredLocal();
			}
			if (base.characterBody)
			{
				base.characterBody.hideCrosshair = false;
			}
			this.calculateArcPointsJobHandle.Complete();
			if (this.arcVisualizerLineRenderer)
			{
				EntityState.Destroy(this.arcVisualizerLineRenderer.gameObject);
				this.arcVisualizerLineRenderer = null;
			}
			if (this.completeArcPointsVisualizerJobMethod != null)
			{
				RoR2Application.onLateUpdate -= this.completeArcPointsVisualizerJobMethod;
				this.completeArcPointsVisualizerJobMethod = null;
			}
			this.calculateArcPointsJob.Dispose();
			this.pointsBuffer = Array.Empty<Vector3>();
			if (this.endpointVisualizerTransform)
			{
				EntityState.Destroy(this.endpointVisualizerTransform.gameObject);
				this.endpointVisualizerTransform = null;
			}
			base.OnExit();
		}

		// Token: 0x0600295C RID: 10588 RVA: 0x000AE005 File Offset: 0x000AC205
		protected virtual bool KeyIsDown()
		{
			return base.IsKeyDownAuthority();
		}

		// Token: 0x0600295D RID: 10589 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnProjectileFiredLocal()
		{
		}

		// Token: 0x0600295E RID: 10590 RVA: 0x000AE010 File Offset: 0x000AC210
		protected virtual void FireProjectile()
		{
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				crit = base.RollCrit(),
				owner = base.gameObject,
				position = this.currentTrajectoryInfo.finalRay.origin,
				projectilePrefab = this.projectilePrefab,
				rotation = Util.QuaternionSafeLookRotation(this.currentTrajectoryInfo.finalRay.direction, Vector3.up),
				speedOverride = this.currentTrajectoryInfo.speedOverride,
				damage = this.damageCoefficient * this.damageStat
			};
			if (this.setFuse)
			{
				fireProjectileInfo.fuseOverride = this.currentTrajectoryInfo.travelTime;
			}
			this.ModifyProjectile(ref fireProjectileInfo);
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x0600295F RID: 10591 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
		{
		}

		// Token: 0x06002960 RID: 10592 RVA: 0x000AE0DC File Offset: 0x000AC2DC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && !this.KeyIsDown() && base.fixedAge >= this.minimumDuration)
			{
				this.UpdateTrajectoryInfo(out this.currentTrajectoryInfo);
				EntityState entityState = this.PickNextState();
				if (entityState != null)
				{
					this.outer.SetNextState(entityState);
					return;
				}
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002961 RID: 10593 RVA: 0x0000AC7F File Offset: 0x00008E7F
		protected virtual EntityState PickNextState()
		{
			return null;
		}

		// Token: 0x06002962 RID: 10594 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06002963 RID: 10595 RVA: 0x000AE13B File Offset: 0x000AC33B
		public override void Update()
		{
			base.Update();
			if (CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				this.UpdateTrajectoryInfo(out this.currentTrajectoryInfo);
				this.UpdateVisualizers(this.currentTrajectoryInfo);
			}
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x000AE168 File Offset: 0x000AC368
		protected virtual void UpdateTrajectoryInfo(out AimThrowableBase.TrajectoryInfo dest)
		{
			dest = default(AimThrowableBase.TrajectoryInfo);
			Ray aimRay = base.GetAimRay();
			RaycastHit raycastHit = default(RaycastHit);
			bool flag = false;
			if (this.rayRadius > 0f && Util.CharacterSpherecast(base.gameObject, aimRay, this.rayRadius, out raycastHit, this.maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal) && raycastHit.collider.GetComponent<HurtBox>())
			{
				flag = true;
			}
			if (!flag)
			{
				flag = Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, this.maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal);
			}
			if (flag)
			{
				dest.hitPoint = raycastHit.point;
				dest.hitNormal = raycastHit.normal;
			}
			else
			{
				dest.hitPoint = aimRay.GetPoint(this.maxDistance);
				dest.hitNormal = -aimRay.direction;
			}
			Vector3 vector = dest.hitPoint - aimRay.origin;
			if (this.useGravity)
			{
				float num = this.projectileBaseSpeed;
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				float magnitude = vector2.magnitude;
				float y = Trajectory.CalculateInitialYSpeed(magnitude / num, vector.y);
				Vector3 a = new Vector3(vector2.x / magnitude * num, y, vector2.y / magnitude * num);
				dest.speedOverride = a.magnitude;
				dest.finalRay = new Ray(aimRay.origin, a / dest.speedOverride);
				dest.travelTime = Trajectory.CalculateGroundTravelTime(num, magnitude);
				return;
			}
			dest.speedOverride = this.projectileBaseSpeed;
			dest.finalRay = aimRay;
			dest.travelTime = this.projectileBaseSpeed / vector.magnitude;
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x000AE310 File Offset: 0x000AC510
		private void CompleteArcVisualizerJob()
		{
			this.calculateArcPointsJobHandle.Complete();
			if (this.arcVisualizerLineRenderer)
			{
				Array.Resize<Vector3>(ref this.pointsBuffer, this.calculateArcPointsJob.outputPositions.Length);
				this.calculateArcPointsJob.outputPositions.CopyTo(this.pointsBuffer);
				this.arcVisualizerLineRenderer.SetPositions(this.pointsBuffer);
			}
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x000AE378 File Offset: 0x000AC578
		private void UpdateVisualizers(AimThrowableBase.TrajectoryInfo trajectoryInfo)
		{
			if (this.arcVisualizerLineRenderer && this.calculateArcPointsJobHandle.IsCompleted)
			{
				this.calculateArcPointsJob.SetParameters(trajectoryInfo.finalRay.origin, trajectoryInfo.finalRay.direction * trajectoryInfo.speedOverride, trajectoryInfo.travelTime, this.arcVisualizerLineRenderer.positionCount, this.useGravity ? Physics.gravity.y : 0f);
				this.calculateArcPointsJobHandle = this.calculateArcPointsJob.Schedule(this.calculateArcPointsJob.outputPositions.Length, 32, default(JobHandle));
			}
			if (this.endpointVisualizerTransform)
			{
				this.endpointVisualizerTransform.SetPositionAndRotation(trajectoryInfo.hitPoint, Util.QuaternionSafeLookRotation(trajectoryInfo.hitNormal));
				if (!this.endpointVisualizerRadiusScale.Equals(0f))
				{
					this.endpointVisualizerTransform.localScale = new Vector3(this.endpointVisualizerRadiusScale, this.endpointVisualizerRadiusScale, this.endpointVisualizerRadiusScale);
				}
			}
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x000AE488 File Offset: 0x000AC688
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			if (this.arcVisualizerLineRenderer)
			{
				this.arcVisualizerLineRenderer.renderingLayerMask = ((sceneCam.cameraRigController.target == base.gameObject) ? 1U : 0U);
			}
			if (this.endpointVisualizerTransform)
			{
				this.endpointVisualizerTransform.gameObject.layer = ((sceneCam.cameraRigController.target == base.gameObject) ? LayerIndex.defaultLayer.intVal : LayerIndex.noDraw.intVal);
			}
		}

		// Token: 0x04002553 RID: 9555
		[SerializeField]
		public float maxDistance;

		// Token: 0x04002554 RID: 9556
		[SerializeField]
		public float rayRadius;

		// Token: 0x04002555 RID: 9557
		[SerializeField]
		public GameObject arcVisualizerPrefab;

		// Token: 0x04002556 RID: 9558
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002557 RID: 9559
		[SerializeField]
		public GameObject endpointVisualizerPrefab;

		// Token: 0x04002558 RID: 9560
		[SerializeField]
		public float endpointVisualizerRadiusScale;

		// Token: 0x04002559 RID: 9561
		[SerializeField]
		public bool setFuse;

		// Token: 0x0400255A RID: 9562
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x0400255B RID: 9563
		[SerializeField]
		public float baseMinimumDuration;

		// Token: 0x0400255C RID: 9564
		protected LineRenderer arcVisualizerLineRenderer;

		// Token: 0x0400255D RID: 9565
		protected Transform endpointVisualizerTransform;

		// Token: 0x0400255E RID: 9566
		protected float projectileBaseSpeed;

		// Token: 0x0400255F RID: 9567
		protected float detonationRadius;

		// Token: 0x04002560 RID: 9568
		protected float minimumDuration;

		// Token: 0x04002561 RID: 9569
		protected bool useGravity;

		// Token: 0x04002562 RID: 9570
		private AimThrowableBase.CalculateArcPointsJob calculateArcPointsJob;

		// Token: 0x04002563 RID: 9571
		private JobHandle calculateArcPointsJobHandle;

		// Token: 0x04002564 RID: 9572
		private Vector3[] pointsBuffer = Array.Empty<Vector3>();

		// Token: 0x04002565 RID: 9573
		private Action completeArcPointsVisualizerJobMethod;

		// Token: 0x04002566 RID: 9574
		protected AimThrowableBase.TrajectoryInfo currentTrajectoryInfo;

		// Token: 0x020006F6 RID: 1782
		private struct CalculateArcPointsJob : IJobParallelFor, IDisposable
		{
			// Token: 0x06002969 RID: 10601 RVA: 0x000AE528 File Offset: 0x000AC728
			public void SetParameters(Vector3 origin, Vector3 velocity, float totalTravelTime, int positionCount, float gravity)
			{
				this.origin = origin;
				this.velocity = velocity;
				if (this.outputPositions.Length != positionCount)
				{
					if (this.outputPositions.IsCreated)
					{
						this.outputPositions.Dispose();
					}
					this.outputPositions = new NativeArray<Vector3>(positionCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				}
				this.indexMultiplier = totalTravelTime / (float)(positionCount - 1);
				this.gravity = gravity;
			}

			// Token: 0x0600296A RID: 10602 RVA: 0x000AE58E File Offset: 0x000AC78E
			public void Dispose()
			{
				if (this.outputPositions.IsCreated)
				{
					this.outputPositions.Dispose();
				}
			}

			// Token: 0x0600296B RID: 10603 RVA: 0x000AE5A8 File Offset: 0x000AC7A8
			public void Execute(int index)
			{
				float t = (float)index * this.indexMultiplier;
				this.outputPositions[index] = Trajectory.CalculatePositionAtTime(this.origin, this.velocity, t, this.gravity);
			}

			// Token: 0x04002567 RID: 9575
			[ReadOnly]
			private Vector3 origin;

			// Token: 0x04002568 RID: 9576
			[ReadOnly]
			private Vector3 velocity;

			// Token: 0x04002569 RID: 9577
			[ReadOnly]
			private float indexMultiplier;

			// Token: 0x0400256A RID: 9578
			[ReadOnly]
			private float gravity;

			// Token: 0x0400256B RID: 9579
			[WriteOnly]
			public NativeArray<Vector3> outputPositions;
		}

		// Token: 0x020006F7 RID: 1783
		protected struct TrajectoryInfo
		{
			// Token: 0x0400256C RID: 9580
			public Ray finalRay;

			// Token: 0x0400256D RID: 9581
			public Vector3 hitPoint;

			// Token: 0x0400256E RID: 9582
			public Vector3 hitNormal;

			// Token: 0x0400256F RID: 9583
			public float travelTime;

			// Token: 0x04002570 RID: 9584
			public float speedOverride;
		}
	}
}
