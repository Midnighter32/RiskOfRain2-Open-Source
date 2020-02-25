using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000339 RID: 825
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillReloader : MonoBehaviour
	{
		// Token: 0x060013A9 RID: 5033 RVA: 0x00053F81 File Offset: 0x00052181
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x00053F8F File Offset: 0x0005218F
		private void Start()
		{
			this.timer = 0f;
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x00053F9C File Offset: 0x0005219C
		private void FixedUpdate()
		{
			if (Util.HasEffectiveAuthority(this.networkIdentity))
			{
				bool flag = this.stateMachine.state.GetType() == typeof(Idle) && !this.stateMachine.HasPendingState();
				if (this.skill.stock < this.skill.maxStock && flag)
				{
					this.timer += Time.fixedDeltaTime;
				}
				else
				{
					this.timer = 0f;
				}
				if (this.timer >= this.reloadDelay || (this.skill.stock == 0 && flag))
				{
					this.stateMachine.SetNextState(EntityState.Instantiate(this.reloadState));
				}
			}
		}

		// Token: 0x04001272 RID: 4722
		private NetworkIdentity networkIdentity;

		// Token: 0x04001273 RID: 4723
		public GenericSkill skill;

		// Token: 0x04001274 RID: 4724
		public EntityStateMachine stateMachine;

		// Token: 0x04001275 RID: 4725
		public SerializableEntityStateType reloadState;

		// Token: 0x04001276 RID: 4726
		public float reloadDelay = 0.2f;

		// Token: 0x04001277 RID: 4727
		private float timer;
	}
}
