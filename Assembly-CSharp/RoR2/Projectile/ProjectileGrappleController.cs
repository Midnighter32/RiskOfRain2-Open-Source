using System;
using EntityStates;
using EntityStates.Loader;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200050A RID: 1290
	[RequireComponent(typeof(EntityStateMachine))]
	[RequireComponent(typeof(ProjectileSimple))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileStickOnImpact))]
	public class ProjectileGrappleController : MonoBehaviour
	{
		// Token: 0x06001E95 RID: 7829 RVA: 0x00084060 File Offset: 0x00082260
		private void Awake()
		{
			this.projectileStickOnImpactController = base.GetComponent<ProjectileStickOnImpact>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileSimple = base.GetComponent<ProjectileSimple>();
			this.resolvedOwnerHookStateType = this.ownerHookStateType.stateType;
			if (this.ropeEndTransform)
			{
				this.soundID = Util.PlaySound(this.enterSoundString, this.ropeEndTransform.gameObject);
			}
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x000840CC File Offset: 0x000822CC
		private void FixedUpdate()
		{
			if (this.ropeEndTransform)
			{
				float in_value = Util.Remap((this.ropeEndTransform.transform.position - base.transform.position).magnitude, this.minHookDistancePitchModifier, this.maxHookDistancePitchModifier, 0f, 100f);
				AkSoundEngine.SetRTPCValueByPlayingID(this.hookDistanceRTPCstring, in_value, this.soundID);
			}
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x00084140 File Offset: 0x00082340
		private void AssignHookReferenceToBodyStateMachine()
		{
			FireHook fireHook;
			if (this.owner.stateMachine && (fireHook = (this.owner.stateMachine.state as FireHook)) != null)
			{
				fireHook.SetHookReference(base.gameObject);
			}
			Transform modelTransform = this.owner.gameObject.GetComponent<ModelLocator>().modelTransform;
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(this.muzzleStringOnBody);
					if (transform)
					{
						this.ropeEndTransform.SetParent(transform, false);
					}
				}
			}
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x000841D3 File Offset: 0x000823D3
		private void Start()
		{
			this.owner = new ProjectileGrappleController.OwnerInfo(this.projectileController.owner);
			this.AssignHookReferenceToBodyStateMachine();
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x000841F4 File Offset: 0x000823F4
		private void OnDestroy()
		{
			if (this.ropeEndTransform)
			{
				Util.PlaySound(this.exitSoundString, this.ropeEndTransform.gameObject);
				UnityEngine.Object.Destroy(this.ropeEndTransform.gameObject);
				return;
			}
			AkSoundEngine.StopPlayingID(this.soundID);
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x00084241 File Offset: 0x00082441
		private bool OwnerIsInFiringState()
		{
			return this.owner.stateMachine && this.owner.stateMachine.state.GetType() == this.resolvedOwnerHookStateType;
		}

		// Token: 0x04001BF9 RID: 7161
		private ProjectileController projectileController;

		// Token: 0x04001BFA RID: 7162
		private ProjectileStickOnImpact projectileStickOnImpactController;

		// Token: 0x04001BFB RID: 7163
		private ProjectileSimple projectileSimple;

		// Token: 0x04001BFC RID: 7164
		public SerializableEntityStateType ownerHookStateType;

		// Token: 0x04001BFD RID: 7165
		public float acceleration;

		// Token: 0x04001BFE RID: 7166
		public float lookAcceleration = 4f;

		// Token: 0x04001BFF RID: 7167
		public float lookAccelerationRampUpDuration = 0.25f;

		// Token: 0x04001C00 RID: 7168
		public float initialLookImpulse = 5f;

		// Token: 0x04001C01 RID: 7169
		public float initiallMoveImpulse = 5f;

		// Token: 0x04001C02 RID: 7170
		public float moveAcceleration = 4f;

		// Token: 0x04001C03 RID: 7171
		public string enterSoundString;

		// Token: 0x04001C04 RID: 7172
		public string exitSoundString;

		// Token: 0x04001C05 RID: 7173
		public string hookDistanceRTPCstring;

		// Token: 0x04001C06 RID: 7174
		public float minHookDistancePitchModifier;

		// Token: 0x04001C07 RID: 7175
		public float maxHookDistancePitchModifier;

		// Token: 0x04001C08 RID: 7176
		public AnimationCurve lookAccelerationRampUpCurve;

		// Token: 0x04001C09 RID: 7177
		public Transform ropeEndTransform;

		// Token: 0x04001C0A RID: 7178
		public string muzzleStringOnBody = "MuzzleLeft";

		// Token: 0x04001C0B RID: 7179
		[Tooltip("The minimum distance the hook can be from the target before it detaches.")]
		public float nearBreakDistance;

		// Token: 0x04001C0C RID: 7180
		[Tooltip("The maximum distance this hook can travel.")]
		public float maxTravelDistance;

		// Token: 0x04001C0D RID: 7181
		public float escapeForceMultiplier = 2f;

		// Token: 0x04001C0E RID: 7182
		public float normalOffset = 1f;

		// Token: 0x04001C0F RID: 7183
		public float yankMassLimit;

		// Token: 0x04001C10 RID: 7184
		private Type resolvedOwnerHookStateType;

		// Token: 0x04001C11 RID: 7185
		private ProjectileGrappleController.OwnerInfo owner;

		// Token: 0x04001C12 RID: 7186
		private bool didStick;

		// Token: 0x04001C13 RID: 7187
		private uint soundID;

		// Token: 0x0200050B RID: 1291
		private struct OwnerInfo
		{
			// Token: 0x06001E9C RID: 7836 RVA: 0x000842E4 File Offset: 0x000824E4
			public OwnerInfo(GameObject ownerGameObject)
			{
				this = default(ProjectileGrappleController.OwnerInfo);
				this.gameObject = ownerGameObject;
				if (this.gameObject)
				{
					this.characterBody = this.gameObject.GetComponent<CharacterBody>();
					this.characterMotor = this.gameObject.GetComponent<CharacterMotor>();
					this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
					this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.gameObject);
					EntityStateMachine[] components = this.gameObject.GetComponents<EntityStateMachine>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].customName == "Hook")
						{
							this.stateMachine = components[i];
							return;
						}
					}
				}
			}

			// Token: 0x04001C14 RID: 7188
			public readonly GameObject gameObject;

			// Token: 0x04001C15 RID: 7189
			public readonly CharacterBody characterBody;

			// Token: 0x04001C16 RID: 7190
			public readonly CharacterMotor characterMotor;

			// Token: 0x04001C17 RID: 7191
			public readonly Rigidbody rigidbody;

			// Token: 0x04001C18 RID: 7192
			public readonly EntityStateMachine stateMachine;

			// Token: 0x04001C19 RID: 7193
			public readonly bool hasEffectiveAuthority;
		}

		// Token: 0x0200050C RID: 1292
		private class BaseState : EntityStates.BaseState
		{
			// Token: 0x1700033F RID: 831
			// (get) Token: 0x06001E9D RID: 7837 RVA: 0x00084388 File Offset: 0x00082588
			// (set) Token: 0x06001E9E RID: 7838 RVA: 0x00084390 File Offset: 0x00082590
			private protected bool ownerValid { protected get; private set; }

			// Token: 0x17000340 RID: 832
			// (get) Token: 0x06001E9F RID: 7839 RVA: 0x00084399 File Offset: 0x00082599
			protected ref ProjectileGrappleController.OwnerInfo owner
			{
				get
				{
					return ref this.grappleController.owner;
				}
			}

			// Token: 0x06001EA0 RID: 7840 RVA: 0x000843A8 File Offset: 0x000825A8
			private void UpdatePositions()
			{
				this.aimOrigin = this.grappleController.owner.characterBody.aimOrigin;
				this.position = base.transform.position + base.transform.up * this.grappleController.normalOffset;
			}

			// Token: 0x06001EA1 RID: 7841 RVA: 0x00084404 File Offset: 0x00082604
			public override void OnEnter()
			{
				base.OnEnter();
				this.grappleController = base.GetComponent<ProjectileGrappleController>();
				this.ownerValid = (this.grappleController && this.grappleController.owner.gameObject);
				if (this.ownerValid)
				{
					this.UpdatePositions();
				}
			}

			// Token: 0x06001EA2 RID: 7842 RVA: 0x0008445C File Offset: 0x0008265C
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (this.ownerValid)
				{
					this.ownerValid &= this.grappleController.owner.gameObject;
					if (this.ownerValid)
					{
						this.UpdatePositions();
						this.FixedUpdateBehavior();
					}
				}
				if (NetworkServer.active && !this.ownerValid)
				{
					this.ownerValid = false;
					EntityState.Destroy(base.gameObject);
					return;
				}
			}

			// Token: 0x06001EA3 RID: 7843 RVA: 0x000844CF File Offset: 0x000826CF
			protected virtual void FixedUpdateBehavior()
			{
				if (base.isAuthority && !this.grappleController.OwnerIsInFiringState())
				{
					this.outer.SetNextState(new ProjectileGrappleController.ReturnState());
					return;
				}
			}

			// Token: 0x06001EA4 RID: 7844 RVA: 0x000844F8 File Offset: 0x000826F8
			protected Ray GetOwnerAimRay()
			{
				if (!this.owner.characterBody)
				{
					return default(Ray);
				}
				return this.owner.characterBody.inputBank.GetAimRay();
			}

			// Token: 0x04001C1A RID: 7194
			protected ProjectileGrappleController grappleController;

			// Token: 0x04001C1C RID: 7196
			protected Vector3 aimOrigin;

			// Token: 0x04001C1D RID: 7197
			protected Vector3 position;
		}

		// Token: 0x0200050D RID: 1293
		private class FlyState : ProjectileGrappleController.BaseState
		{
			// Token: 0x06001EA6 RID: 7846 RVA: 0x00084536 File Offset: 0x00082736
			public override void OnEnter()
			{
				base.OnEnter();
				this.duration = this.grappleController.maxTravelDistance / this.grappleController.GetComponent<ProjectileSimple>().velocity;
			}

			// Token: 0x06001EA7 RID: 7847 RVA: 0x00084560 File Offset: 0x00082760
			protected override void FixedUpdateBehavior()
			{
				base.FixedUpdateBehavior();
				if (base.isAuthority)
				{
					if (this.grappleController.projectileStickOnImpactController.stuck)
					{
						EntityState entityState = null;
						if (this.grappleController.projectileStickOnImpactController.stuckBody)
						{
							Rigidbody component = this.grappleController.projectileStickOnImpactController.stuckBody.GetComponent<Rigidbody>();
							if (component && component.mass < this.grappleController.yankMassLimit)
							{
								CharacterBody component2 = component.GetComponent<CharacterBody>();
								if (!component2 || !component2.isPlayerControlled || component2.teamComponent.teamIndex != base.projectileController.teamFilter.teamIndex)
								{
									entityState = new ProjectileGrappleController.YankState();
								}
							}
						}
						if (entityState == null)
						{
							entityState = new ProjectileGrappleController.GripState();
						}
						this.DeductOwnerStock();
						this.outer.SetNextState(entityState);
						return;
					}
					if (this.duration <= base.fixedAge)
					{
						this.outer.SetNextState(new ProjectileGrappleController.ReturnState());
						return;
					}
				}
			}

			// Token: 0x06001EA8 RID: 7848 RVA: 0x00084658 File Offset: 0x00082858
			private void DeductOwnerStock()
			{
				if (base.ownerValid && base.owner.hasEffectiveAuthority)
				{
					SkillLocator component = base.owner.gameObject.GetComponent<SkillLocator>();
					if (component)
					{
						GenericSkill secondary = component.secondary;
						if (secondary)
						{
							secondary.DeductStock(1);
						}
					}
				}
			}

			// Token: 0x04001C1E RID: 7198
			private float duration;
		}

		// Token: 0x0200050E RID: 1294
		private class BaseGripState : ProjectileGrappleController.BaseState
		{
			// Token: 0x06001EAA RID: 7850 RVA: 0x000846B1 File Offset: 0x000828B1
			public override void OnEnter()
			{
				base.OnEnter();
				this.currentDistance = Vector3.Distance(this.aimOrigin, this.position);
			}

			// Token: 0x06001EAB RID: 7851 RVA: 0x000846D0 File Offset: 0x000828D0
			protected override void FixedUpdateBehavior()
			{
				base.FixedUpdateBehavior();
				this.currentDistance = Vector3.Distance(this.aimOrigin, this.position);
				if (base.isAuthority)
				{
					bool flag = !this.grappleController.projectileStickOnImpactController.stuck;
					bool flag2 = this.currentDistance < this.grappleController.nearBreakDistance;
					bool flag3 = !this.grappleController.OwnerIsInFiringState();
					bool flag4;
					if (base.owner.stateMachine)
					{
						BaseSkillState baseSkillState = base.owner.stateMachine.state as BaseSkillState;
						flag4 = (baseSkillState == null || !baseSkillState.IsKeyDownAuthority());
					}
					else
					{
						flag4 = true;
					}
					if (flag4 || flag3 || flag2 || flag)
					{
						this.outer.SetNextState(new ProjectileGrappleController.ReturnState());
						return;
					}
				}
			}

			// Token: 0x04001C1F RID: 7199
			protected float currentDistance;
		}

		// Token: 0x0200050F RID: 1295
		private class GripState : ProjectileGrappleController.BaseGripState
		{
			// Token: 0x06001EAD RID: 7853 RVA: 0x00084790 File Offset: 0x00082990
			private void DeductStockIfStruckNonPylon()
			{
				GameObject victim = this.grappleController.projectileStickOnImpactController.victim;
				if (victim)
				{
					GameObject gameObject = victim;
					EntityLocator component = gameObject.GetComponent<EntityLocator>();
					if (component)
					{
						gameObject = component.entity;
					}
					gameObject.GetComponent<ProjectileController>();
				}
			}

			// Token: 0x06001EAE RID: 7854 RVA: 0x000847DC File Offset: 0x000829DC
			public override void OnEnter()
			{
				base.OnEnter();
				this.lastDistance = Vector3.Distance(this.aimOrigin, this.position);
				if (base.ownerValid)
				{
					this.grappleController.didStick = true;
					if (base.owner.characterMotor)
					{
						Vector3 direction = base.GetOwnerAimRay().direction;
						Vector3 vector = base.owner.characterMotor.velocity;
						vector = ((Vector3.Dot(vector, direction) < 0f) ? Vector3.zero : Vector3.Project(vector, direction));
						vector += direction * this.grappleController.initialLookImpulse;
						vector += base.owner.characterMotor.moveDirection * this.grappleController.initiallMoveImpulse;
						base.owner.characterMotor.velocity = vector;
					}
				}
			}

			// Token: 0x06001EAF RID: 7855 RVA: 0x000848C0 File Offset: 0x00082AC0
			protected override void FixedUpdateBehavior()
			{
				base.FixedUpdateBehavior();
				float num = this.grappleController.acceleration;
				if (this.currentDistance > this.lastDistance)
				{
					num *= this.grappleController.escapeForceMultiplier;
				}
				this.lastDistance = this.currentDistance;
				if (base.owner.hasEffectiveAuthority && base.owner.characterMotor && base.owner.characterBody)
				{
					Ray ownerAimRay = base.GetOwnerAimRay();
					Vector3 normalized = (base.transform.position - base.owner.characterBody.aimOrigin).normalized;
					Vector3 a = normalized * num;
					float time = Mathf.Clamp01(base.fixedAge / this.grappleController.lookAccelerationRampUpDuration);
					float num2 = this.grappleController.lookAccelerationRampUpCurve.Evaluate(time);
					float num3 = Util.Remap(Vector3.Dot(ownerAimRay.direction, normalized), -1f, 1f, 1f, 0f);
					a += ownerAimRay.direction * (this.grappleController.lookAcceleration * num2 * num3);
					a += base.owner.characterMotor.moveDirection * this.grappleController.moveAcceleration;
					base.owner.characterMotor.ApplyForce(a * (base.owner.characterMotor.mass * Time.fixedDeltaTime), true, true);
				}
			}

			// Token: 0x04001C20 RID: 7200
			private float lastDistance;
		}

		// Token: 0x02000510 RID: 1296
		private class YankState : ProjectileGrappleController.BaseGripState
		{
			// Token: 0x06001EB1 RID: 7857 RVA: 0x00084A50 File Offset: 0x00082C50
			public override void OnEnter()
			{
				base.OnEnter();
				this.stuckBody = this.grappleController.projectileStickOnImpactController.stuckBody;
			}

			// Token: 0x06001EB2 RID: 7858 RVA: 0x00084A70 File Offset: 0x00082C70
			protected override void FixedUpdateBehavior()
			{
				base.FixedUpdateBehavior();
				if (this.stuckBody)
				{
					if (Util.HasEffectiveAuthority(this.stuckBody.gameObject))
					{
						Vector3 a = this.aimOrigin - this.position;
						IDisplacementReceiver component = this.stuckBody.GetComponent<IDisplacementReceiver>();
						if ((Component)component && base.fixedAge >= ProjectileGrappleController.YankState.delayBeforeYanking)
						{
							component.AddDisplacement(a * (ProjectileGrappleController.YankState.yankSpeed * Time.fixedDeltaTime));
						}
					}
					if (base.owner.hasEffectiveAuthority && base.owner.characterMotor && base.fixedAge < ProjectileGrappleController.YankState.hoverTimeLimit)
					{
						Vector3 velocity = base.owner.characterMotor.velocity;
						if (velocity.y < 0f)
						{
							velocity.y = 0f;
							base.owner.characterMotor.velocity = velocity;
						}
					}
				}
			}

			// Token: 0x04001C21 RID: 7201
			public static float yankSpeed;

			// Token: 0x04001C22 RID: 7202
			public static float delayBeforeYanking;

			// Token: 0x04001C23 RID: 7203
			public static float hoverTimeLimit = 0.5f;

			// Token: 0x04001C24 RID: 7204
			private CharacterBody stuckBody;
		}

		// Token: 0x02000511 RID: 1297
		private class ReturnState : ProjectileGrappleController.BaseState
		{
			// Token: 0x06001EB5 RID: 7861 RVA: 0x00084B68 File Offset: 0x00082D68
			public override void OnEnter()
			{
				base.OnEnter();
				if (base.ownerValid)
				{
					this.returnSpeed = this.grappleController.projectileSimple.velocity;
					this.returnSpeedAcceleration = this.returnSpeed * 2f;
				}
				if (NetworkServer.active && this.grappleController)
				{
					this.grappleController.projectileStickOnImpactController.Detach();
					this.grappleController.projectileStickOnImpactController.ignoreCharacters = true;
					this.grappleController.projectileStickOnImpactController.ignoreWorld = true;
				}
				Collider component = base.GetComponent<Collider>();
				if (component)
				{
					component.enabled = false;
				}
			}

			// Token: 0x06001EB6 RID: 7862 RVA: 0x00084C08 File Offset: 0x00082E08
			protected override void FixedUpdateBehavior()
			{
				base.FixedUpdateBehavior();
				if (base.rigidbody)
				{
					this.returnSpeed += this.returnSpeedAcceleration * Time.fixedDeltaTime;
					base.rigidbody.velocity = (this.aimOrigin - this.position).normalized * this.returnSpeed;
					if (NetworkServer.active)
					{
						Vector3 endPosition = this.position + base.rigidbody.velocity * Time.fixedDeltaTime;
						if (HGMath.Overshoots(this.position, endPosition, this.aimOrigin))
						{
							EntityState.Destroy(base.gameObject);
							return;
						}
					}
				}
			}

			// Token: 0x04001C25 RID: 7205
			private float initialReturnSpeed = 120f;

			// Token: 0x04001C26 RID: 7206
			private float returnSpeedAcceleration = 240f;

			// Token: 0x04001C27 RID: 7207
			private float returnSpeed;
		}
	}
}
