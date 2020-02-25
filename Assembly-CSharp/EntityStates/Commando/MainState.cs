using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando
{
	// Token: 0x020008B2 RID: 2226
	public class MainState : BaseState
	{
		// Token: 0x060031E8 RID: 12776 RVA: 0x000D7250 File Offset: 0x000D5450
		public override void OnEnter()
		{
			base.OnEnter();
			GenericSkill[] components = base.gameObject.GetComponents<GenericSkill>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].skillName == "FirePistol")
				{
					this.skill1 = components[i];
				}
				else if (components[i].skillName == "FireFMJ")
				{
					this.skill2 = components[i];
				}
				else if (components[i].skillName == "Roll")
				{
					this.skill3 = components[i];
				}
				else if (components[i].skillName == "FireBarrage")
				{
					this.skill4 = components[i];
				}
			}
			this.modelAnimator = base.GetModelAnimator();
			this.previousPosition = base.transform.position;
			if (this.modelAnimator)
			{
				int layerIndex = this.modelAnimator.GetLayerIndex("Body");
				this.modelAnimator.CrossFadeInFixedTime("Walk", 0.1f, layerIndex);
				this.modelAnimator.Update(0f);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				AimAnimator component = modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
			}
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x000D7384 File Offset: 0x000D5584
		public override void OnExit()
		{
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				AimAnimator component = modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = false;
				}
			}
			if (base.isAuthority && base.characterMotor)
			{
				base.characterMotor.moveDirection = Vector3.zero;
			}
			base.OnExit();
		}

		// Token: 0x060031EA RID: 12778 RVA: 0x000D73E4 File Offset: 0x000D55E4
		public override void Update()
		{
			base.Update();
			if (base.inputBank.skill1.down)
			{
				this.skill1InputRecieved = true;
			}
			if (base.inputBank.skill2.down)
			{
				this.skill2InputRecieved = true;
			}
			if (base.inputBank.skill3.down)
			{
				this.skill3InputRecieved = true;
			}
			if (base.inputBank.skill4.down)
			{
				this.skill4InputRecieved = true;
			}
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x000D745C File Offset: 0x000D565C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Vector3 position = base.transform.position;
			if (Time.fixedDeltaTime != 0f)
			{
				this.estimatedVelocity = (position - this.previousPosition) / Time.fixedDeltaTime;
			}
			if (base.isAuthority)
			{
				Vector3 moveVector = base.inputBank.moveVector;
				if (base.characterMotor)
				{
					base.characterMotor.moveDirection = moveVector;
					if (this.skill3InputRecieved)
					{
						if (this.skill3)
						{
							this.skill3.ExecuteIfReady();
						}
						this.skill3InputRecieved = false;
					}
				}
				if (base.characterDirection)
				{
					if (base.characterBody && base.characterBody.shouldAim)
					{
						base.characterDirection.moveVector = base.inputBank.aimDirection;
					}
					else
					{
						base.characterDirection.moveVector = moveVector;
					}
				}
				if (this.skill1InputRecieved)
				{
					if (this.skill1)
					{
						this.skill1.ExecuteIfReady();
					}
					this.skill1InputRecieved = false;
				}
				if (this.skill2InputRecieved)
				{
					if (this.skill2)
					{
						this.skill2.ExecuteIfReady();
					}
					this.skill2InputRecieved = false;
				}
				if (this.skill4InputRecieved)
				{
					if (this.skill4)
					{
						this.skill4.ExecuteIfReady();
					}
					this.skill4InputRecieved = false;
				}
			}
			if (this.modelAnimator && base.characterDirection)
			{
				Vector3 lhs = this.estimatedVelocity;
				lhs.y = 0f;
				Vector3 forward = base.characterDirection.forward;
				Vector3 rhs = Vector3.Cross(Vector3.up, forward);
				float magnitude = lhs.magnitude;
				float value = Vector3.Dot(lhs, forward);
				float value2 = Vector3.Dot(lhs, rhs);
				this.modelAnimator.SetBool("isMoving", magnitude != 0f);
				this.modelAnimator.SetFloat("walkSpeed", magnitude);
				this.modelAnimator.SetFloat("forwardSpeed", value, 0.2f, Time.fixedDeltaTime);
				this.modelAnimator.SetFloat("rightSpeed", value2, 0.2f, Time.fixedDeltaTime);
			}
			this.previousPosition = position;
		}

		// Token: 0x0400307B RID: 12411
		private Animator modelAnimator;

		// Token: 0x0400307C RID: 12412
		private GenericSkill skill1;

		// Token: 0x0400307D RID: 12413
		private GenericSkill skill2;

		// Token: 0x0400307E RID: 12414
		private GenericSkill skill3;

		// Token: 0x0400307F RID: 12415
		private GenericSkill skill4;

		// Token: 0x04003080 RID: 12416
		private bool skill1InputRecieved;

		// Token: 0x04003081 RID: 12417
		private bool skill2InputRecieved;

		// Token: 0x04003082 RID: 12418
		private bool skill3InputRecieved;

		// Token: 0x04003083 RID: 12419
		private bool skill4InputRecieved;

		// Token: 0x04003084 RID: 12420
		private Vector3 previousPosition;

		// Token: 0x04003085 RID: 12421
		private Vector3 estimatedVelocity;
	}
}
