using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200021F RID: 543
	[RequireComponent(typeof(TeamFilter))]
	public class HealingWard : NetworkBehaviour
	{
		// Token: 0x06000C14 RID: 3092 RVA: 0x00035DC9 File Offset: 0x00033FC9
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x00035DD8 File Offset: 0x00033FD8
		private void Start()
		{
			RaycastHit raycastHit;
			if (this.floorWard && Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				base.transform.up = raycastHit.normal;
			}
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00035E44 File Offset: 0x00034044
		private void Update()
		{
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(this.rangeIndicator.localScale.x, this.radius, ref this.rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x00035E98 File Offset: 0x00034098
		private void FixedUpdate()
		{
			this.healTimer -= Time.fixedDeltaTime;
			if (this.healTimer <= 0f && NetworkServer.active)
			{
				this.healTimer = this.interval;
				this.HealOccupants();
			}
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x00035ED4 File Offset: 0x000340D4
		private void HealOccupants()
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(this.teamFilter.teamIndex);
			float num = this.radius * this.radius;
			Vector3 position = base.transform.position;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
				{
					HealthComponent component = teamMembers[i].GetComponent<HealthComponent>();
					if (component)
					{
						float num2 = this.healPoints + component.fullHealth * this.healFraction;
						if (num2 > 0f)
						{
							component.Heal(num2, default(ProcChainMask), true);
						}
					}
				}
			}
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000C1B RID: 3099 RVA: 0x00035FA0 File Offset: 0x000341A0
		// (set) Token: 0x06000C1C RID: 3100 RVA: 0x00035FB3 File Offset: 0x000341B3
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1U);
			}
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00035FC8 File Offset: 0x000341C8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00036034 File Offset: 0x00034234
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.radius = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.radius = reader.ReadSingle();
			}
		}

		// Token: 0x04000C16 RID: 3094
		[Tooltip("The area of effect.")]
		[SyncVar]
		public float radius;

		// Token: 0x04000C17 RID: 3095
		[Tooltip("How long between heal pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x04000C18 RID: 3096
		[Tooltip("How many hit points to restore each pulse.")]
		public float healPoints;

		// Token: 0x04000C19 RID: 3097
		[Tooltip("What fraction of the healee max health to restore each pulse.")]
		public float healFraction;

		// Token: 0x04000C1A RID: 3098
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x04000C1B RID: 3099
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x04000C1C RID: 3100
		private TeamFilter teamFilter;

		// Token: 0x04000C1D RID: 3101
		private float healTimer;

		// Token: 0x04000C1E RID: 3102
		private float rangeIndicatorScaleVelocity;
	}
}
