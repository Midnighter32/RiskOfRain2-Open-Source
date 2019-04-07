using System;
using RoR2;
using RoR2.Projectile;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000A4 RID: 164
	public abstract class AimThrowableBase : BaseState
	{
		// Token: 0x0600030A RID: 778 RVA: 0x0000C330 File Offset: 0x0000A530
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
			this.projectileBaseSpeed = this.projectilePrefab.GetComponent<ProjectileSimple>().velocity;
			this.minimumDuration = this.baseMinimumDuration / this.attackSpeedStat;
			ProjectileImpactExplosion component = this.projectilePrefab.GetComponent<ProjectileImpactExplosion>();
			if (component)
			{
				this.detonationRadius = component.blastRadius;
				if (this.endpointVisualizerTransform)
				{
					this.endpointVisualizerTransform.localScale = new Vector3(this.detonationRadius, this.detonationRadius, this.detonationRadius);
				}
			}
			this.UpdateVisualizers();
			SceneCamera.onSceneCameraPreRender += this.OnPreRenderSceneCam;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000C47C File Offset: 0x0000A67C
		public override void OnExit()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
			if (!this.outer.destroying && base.isAuthority)
			{
				this.FireProjectile();
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

		// Token: 0x0600030C RID: 780 RVA: 0x0000C557 File Offset: 0x0000A757
		protected virtual bool KeyIsDown()
		{
			return base.inputBank && base.inputBank.skill2.down;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000C578 File Offset: 0x0000A778
		protected virtual void FireProjectile()
		{
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				crit = base.RollCrit(),
				owner = base.gameObject,
				position = this.finalRay.origin,
				projectilePrefab = this.projectilePrefab,
				rotation = Util.QuaternionSafeLookRotation(this.finalRay.direction, Vector3.up),
				speedOverride = this.speedOverride,
				damage = this.damageCoefficient * this.damageStat
			};
			if (this.setFuse)
			{
				fireProjectileInfo.fuseOverride = this.travelTime;
			}
			this.ModifyProjectile(ref fireProjectileInfo);
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
		{
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000C62D File Offset: 0x0000A82D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && !this.KeyIsDown() && base.fixedAge >= this.minimumDuration)
			{
				this.UpdateTrajectoryInfo();
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000C664 File Offset: 0x0000A864
		public override void Update()
		{
			base.Update();
			if (CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				this.UpdateTrajectoryInfo();
				this.UpdateVisualizers();
			}
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000C688 File Offset: 0x0000A888
		private void UpdateTrajectoryInfo()
		{
			this.aimRay = base.GetAimRay();
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(base.gameObject, this.aimRay, out raycastHit, this.maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
			{
				this.hitPoint = raycastHit.point;
				this.hitNormal = raycastHit.normal;
			}
			else
			{
				this.hitPoint = this.aimRay.GetPoint(this.maxDistance);
				this.hitNormal = -this.aimRay.direction;
			}
			float num = this.projectileBaseSpeed;
			Vector3 vector = this.hitPoint - this.aimRay.origin;
			Vector2 vector2 = new Vector2(vector.x, vector.z);
			float magnitude = vector2.magnitude;
			float y = Trajectory.CalculateInitialYSpeed(magnitude / num, vector.y);
			Vector3 a = new Vector3(vector2.x / magnitude * num, y, vector2.y / magnitude * num);
			this.speedOverride = a.magnitude;
			this.finalRay = new Ray(this.aimRay.origin, a / this.speedOverride);
			this.travelTime = Trajectory.CalculateGroundTravelTime(num, magnitude);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000C7D8 File Offset: 0x0000A9D8
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

		// Token: 0x06000314 RID: 788 RVA: 0x0000C840 File Offset: 0x0000AA40
		private void UpdateVisualizers()
		{
			if (this.arcVisualizerLineRenderer && this.calculateArcPointsJobHandle.IsCompleted)
			{
				this.calculateArcPointsJob.SetParameters(this.finalRay.origin, this.finalRay.direction * this.speedOverride, this.travelTime, this.arcVisualizerLineRenderer.positionCount);
				this.calculateArcPointsJobHandle = this.calculateArcPointsJob.Schedule(this.calculateArcPointsJob.outputPositions.Length, 32, default(JobHandle));
			}
			if (this.endpointVisualizerTransform)
			{
				this.endpointVisualizerTransform.SetPositionAndRotation(this.hitPoint, Util.QuaternionSafeLookRotation(this.hitNormal));
			}
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000C8FC File Offset: 0x0000AAFC
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			if (this.arcVisualizerLineRenderer)
			{
				this.arcVisualizerLineRenderer.renderingLayerMask = ((sceneCam.cameraRigController.target == base.gameObject) ? 1u : 0u);
			}
			if (this.endpointVisualizerTransform)
			{
				this.endpointVisualizerTransform.gameObject.layer = ((sceneCam.cameraRigController.target == base.gameObject) ? LayerIndex.defaultLayer.intVal : LayerIndex.noDraw.intVal);
			}
		}

		// Token: 0x040002E7 RID: 743
		[SerializeField]
		public float maxDistance;

		// Token: 0x040002E8 RID: 744
		[SerializeField]
		public GameObject arcVisualizerPrefab;

		// Token: 0x040002E9 RID: 745
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x040002EA RID: 746
		[SerializeField]
		public GameObject endpointVisualizerPrefab;

		// Token: 0x040002EB RID: 747
		[SerializeField]
		public bool setFuse;

		// Token: 0x040002EC RID: 748
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x040002ED RID: 749
		[SerializeField]
		public float baseMinimumDuration;

		// Token: 0x040002EE RID: 750
		protected LineRenderer arcVisualizerLineRenderer;

		// Token: 0x040002EF RID: 751
		protected Transform endpointVisualizerTransform;

		// Token: 0x040002F0 RID: 752
		protected float projectileBaseSpeed;

		// Token: 0x040002F1 RID: 753
		protected float detonationRadius;

		// Token: 0x040002F2 RID: 754
		protected float minimumDuration;

		// Token: 0x040002F3 RID: 755
		private AimThrowableBase.CalculateArcPointsJob calculateArcPointsJob;

		// Token: 0x040002F4 RID: 756
		private JobHandle calculateArcPointsJobHandle;

		// Token: 0x040002F5 RID: 757
		private Vector3[] pointsBuffer = Array.Empty<Vector3>();

		// Token: 0x040002F6 RID: 758
		private Action completeArcPointsVisualizerJobMethod;

		// Token: 0x040002F7 RID: 759
		private Ray aimRay;

		// Token: 0x040002F8 RID: 760
		private Ray finalRay;

		// Token: 0x040002F9 RID: 761
		private Vector3 hitPoint;

		// Token: 0x040002FA RID: 762
		private Vector3 hitNormal;

		// Token: 0x040002FB RID: 763
		private float travelTime;

		// Token: 0x040002FC RID: 764
		private float speedOverride;

		// Token: 0x020000A5 RID: 165
		private struct CalculateArcPointsJob : IJobParallelFor, IDisposable
		{
			// Token: 0x06000317 RID: 791 RVA: 0x0000C99C File Offset: 0x0000AB9C
			public void SetParameters(Vector3 origin, Vector3 velocity, float totalTravelTime, int positionCount)
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
				this.gravity = Physics.gravity.y;
			}

			// Token: 0x06000318 RID: 792 RVA: 0x0000CA0A File Offset: 0x0000AC0A
			public void Dispose()
			{
				if (this.outputPositions.IsCreated)
				{
					this.outputPositions.Dispose();
				}
			}

			// Token: 0x06000319 RID: 793 RVA: 0x0000CA24 File Offset: 0x0000AC24
			public void Execute(int index)
			{
				float t = (float)index * this.indexMultiplier;
				this.outputPositions[index] = Trajectory.CalculatePositionAtTime(this.origin, this.velocity, t, this.gravity);
			}

			// Token: 0x040002FD RID: 765
			[ReadOnly]
			private Vector3 origin;

			// Token: 0x040002FE RID: 766
			[ReadOnly]
			private Vector3 velocity;

			// Token: 0x040002FF RID: 767
			[ReadOnly]
			private float indexMultiplier;

			// Token: 0x04000300 RID: 768
			[ReadOnly]
			private float gravity;

			// Token: 0x04000301 RID: 769
			[WriteOnly]
			public NativeArray<Vector3> outputPositions;
		}
	}
}
