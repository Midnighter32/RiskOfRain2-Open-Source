using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035E RID: 862
	public class ModelLocator : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x060011B5 RID: 4533 RVA: 0x00057B74 File Offset: 0x00055D74
		public void Start()
		{
			if (this.modelTransform)
			{
				this.modelParentTransform = this.modelTransform.parent;
				if (!this.dontDetatchFromParent)
				{
					this.modelTransform.parent = null;
				}
			}
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00057BA8 File Offset: 0x00055DA8
		private void UpdateModelTransform(float deltaTime)
		{
			if (this.modelTransform && this.modelParentTransform)
			{
				Vector3 position = this.modelParentTransform.position;
				Quaternion quaternion = this.modelParentTransform.rotation;
				this.UpdateTargetNormal();
				this.SmoothNormals(deltaTime);
				quaternion = Quaternion.FromToRotation(Vector3.up, this.currentNormal) * quaternion;
				this.modelTransform.SetPositionAndRotation(position, quaternion);
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00057C18 File Offset: 0x00055E18
		private void SmoothNormals(float deltaTime)
		{
			this.currentNormal = Vector3.SmoothDamp(this.currentNormal, this.targetNormal, ref this.normalSmoothdampVelocity, 0.1f, float.PositiveInfinity, deltaTime);
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00057C44 File Offset: 0x00055E44
		private void UpdateTargetNormal()
		{
			if (!this.normalizeToFloor)
			{
				this.targetNormal = Vector3.up;
				return;
			}
			if (this.characterMotor)
			{
				this.targetNormal = this.characterMotor.estimatedFloorNormal;
				return;
			}
			this.characterMotor = base.GetComponent<CharacterMotor>();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00057C90 File Offset: 0x00055E90
		public void LateUpdate()
		{
			if (this.autoUpdateModelTransform)
			{
				this.UpdateModelTransform(Time.deltaTime);
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00057CA8 File Offset: 0x00055EA8
		private void OnDestroy()
		{
			if (this.modelTransform)
			{
				if (this.preserveModel)
				{
					if (!this.noCorpse)
					{
						this.modelTransform.gameObject.AddComponent<Corpse>();
					}
					this.modelTransform = null;
					return;
				}
				UnityEngine.Object.Destroy(this.modelTransform.gameObject);
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00057CFB File Offset: 0x00055EFB
		public void OnDeathStart()
		{
			if (!this.dontReleaseModelOnDeath)
			{
				this.preserveModel = true;
			}
		}

		// Token: 0x040015CD RID: 5581
		[Tooltip("The transform of the child gameobject which acts as the model for this entity.")]
		public Transform modelTransform;

		// Token: 0x040015CE RID: 5582
		[Tooltip("The transform of the child gameobject which acts as the base for this entity's model. If provided, this will be detached from the hierarchy and positioned to match this object's position.")]
		public Transform modelBaseTransform;

		// Token: 0x040015CF RID: 5583
		[Tooltip("Whether or not the model reference should be released upon the death of this character.")]
		public bool dontReleaseModelOnDeath;

		// Token: 0x040015D0 RID: 5584
		[Tooltip("Whether or not to update the model transforms automatically.")]
		public bool autoUpdateModelTransform = true;

		// Token: 0x040015D1 RID: 5585
		public bool dontDetatchFromParent;

		// Token: 0x040015D2 RID: 5586
		private Transform modelParentTransform;

		// Token: 0x040015D3 RID: 5587
		public bool noCorpse;

		// Token: 0x040015D4 RID: 5588
		public bool normalizeToFloor;

		// Token: 0x040015D5 RID: 5589
		private const float normalSmoothdampTime = 0.1f;

		// Token: 0x040015D6 RID: 5590
		private Vector3 normalSmoothdampVelocity;

		// Token: 0x040015D7 RID: 5591
		private Vector3 targetNormal = Vector3.up;

		// Token: 0x040015D8 RID: 5592
		private Vector3 currentNormal = Vector3.up;

		// Token: 0x040015D9 RID: 5593
		private CharacterMotor characterMotor;

		// Token: 0x040015DA RID: 5594
		public bool preserveModel;
	}
}
