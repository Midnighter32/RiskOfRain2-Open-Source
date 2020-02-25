using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200028A RID: 650
	public class ModelLocator : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000E73 RID: 3699 RVA: 0x000403FC File Offset: 0x0003E5FC
		// (set) Token: 0x06000E74 RID: 3700 RVA: 0x00040404 File Offset: 0x0003E604
		public Transform modelTransform
		{
			get
			{
				return this._modelTransform;
			}
			set
			{
				if (this._modelTransform == value)
				{
					return;
				}
				if (this.modelDestructionNotifier != null)
				{
					this.modelDestructionNotifier.subscriber = null;
					UnityEngine.Object.Destroy(this.modelDestructionNotifier);
					this.modelDestructionNotifier = null;
				}
				this._modelTransform = value;
				if (this._modelTransform)
				{
					this.modelDestructionNotifier = this._modelTransform.gameObject.AddComponent<ModelLocator.DestructionNotifier>();
					this.modelDestructionNotifier.subscriber = this;
				}
				Action<Transform> action = this.onModelChanged;
				if (action == null)
				{
					return;
				}
				action(this._modelTransform);
			}
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06000E75 RID: 3701 RVA: 0x00040490 File Offset: 0x0003E690
		// (remove) Token: 0x06000E76 RID: 3702 RVA: 0x000404C8 File Offset: 0x0003E6C8
		public event Action<Transform> onModelChanged;

		// Token: 0x06000E77 RID: 3703 RVA: 0x000404FD File Offset: 0x0003E6FD
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

		// Token: 0x06000E78 RID: 3704 RVA: 0x00040534 File Offset: 0x0003E734
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

		// Token: 0x06000E79 RID: 3705 RVA: 0x000405A4 File Offset: 0x0003E7A4
		private void SmoothNormals(float deltaTime)
		{
			this.currentNormal = Vector3.SmoothDamp(this.currentNormal, this.targetNormal, ref this.normalSmoothdampVelocity, 0.1f, float.PositiveInfinity, deltaTime);
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x000405D0 File Offset: 0x0003E7D0
		private void UpdateTargetNormal()
		{
			if (!this.normalizeToFloor)
			{
				this.targetNormal = Vector3.up;
				return;
			}
			if (this.characterMotor)
			{
				this.targetNormal = (this.characterMotor.isGrounded ? this.characterMotor.estimatedGroundNormal : Vector3.up);
				return;
			}
			this.characterMotor = base.GetComponent<CharacterMotor>();
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x00040630 File Offset: 0x0003E830
		public void LateUpdate()
		{
			if (this.autoUpdateModelTransform)
			{
				this.UpdateModelTransform(Time.deltaTime);
			}
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x00040648 File Offset: 0x0003E848
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

		// Token: 0x06000E7D RID: 3709 RVA: 0x0004069B File Offset: 0x0003E89B
		public void OnDeathStart()
		{
			if (!this.dontReleaseModelOnDeath)
			{
				this.preserveModel = true;
			}
		}

		// Token: 0x04000E55 RID: 3669
		[Tooltip("The transform of the child gameobject which acts as the model for this entity.")]
		[FormerlySerializedAs("modelTransform")]
		[SerializeField]
		private Transform _modelTransform;

		// Token: 0x04000E56 RID: 3670
		private ModelLocator.DestructionNotifier modelDestructionNotifier;

		// Token: 0x04000E57 RID: 3671
		[Tooltip("The transform of the child gameobject which acts as the base for this entity's model. If provided, this will be detached from the hierarchy and positioned to match this object's position.")]
		public Transform modelBaseTransform;

		// Token: 0x04000E58 RID: 3672
		[Tooltip("Whether or not the model reference should be released upon the death of this character.")]
		public bool dontReleaseModelOnDeath;

		// Token: 0x04000E59 RID: 3673
		[Tooltip("Whether or not to update the model transforms automatically.")]
		public bool autoUpdateModelTransform = true;

		// Token: 0x04000E5A RID: 3674
		public bool dontDetatchFromParent;

		// Token: 0x04000E5B RID: 3675
		private Transform modelParentTransform;

		// Token: 0x04000E5D RID: 3677
		public bool noCorpse;

		// Token: 0x04000E5E RID: 3678
		public bool normalizeToFloor;

		// Token: 0x04000E5F RID: 3679
		private const float normalSmoothdampTime = 0.1f;

		// Token: 0x04000E60 RID: 3680
		private Vector3 normalSmoothdampVelocity;

		// Token: 0x04000E61 RID: 3681
		private Vector3 targetNormal = Vector3.up;

		// Token: 0x04000E62 RID: 3682
		private Vector3 currentNormal = Vector3.up;

		// Token: 0x04000E63 RID: 3683
		private CharacterMotor characterMotor;

		// Token: 0x04000E64 RID: 3684
		public bool preserveModel;

		// Token: 0x0200028B RID: 651
		private class DestructionNotifier : MonoBehaviour
		{
			// Token: 0x170001D0 RID: 464
			// (get) Token: 0x06000E80 RID: 3712 RVA: 0x000406DA File Offset: 0x0003E8DA
			// (set) Token: 0x06000E7F RID: 3711 RVA: 0x000406D1 File Offset: 0x0003E8D1
			public ModelLocator subscriber { private get; set; }

			// Token: 0x06000E81 RID: 3713 RVA: 0x000406E2 File Offset: 0x0003E8E2
			private void OnDestroy()
			{
				if (this.subscriber != null)
				{
					this.subscriber.modelTransform = null;
				}
			}
		}
	}
}
