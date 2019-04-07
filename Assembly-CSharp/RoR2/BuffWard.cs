using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000271 RID: 625
	[RequireComponent(typeof(TeamFilter))]
	public class BuffWard : NetworkBehaviour
	{
		// Token: 0x06000BBE RID: 3006 RVA: 0x00039673 File Offset: 0x00037873
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00039684 File Offset: 0x00037884
		private void Start()
		{
			RaycastHit raycastHit;
			if (this.floorWard && Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				base.transform.up = raycastHit.normal;
			}
			if (this.rangeIndicator && this.expires)
			{
				ScaleParticleSystemDuration component = this.rangeIndicator.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.expireDuration;
				}
			}
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00039724 File Offset: 0x00037924
		private void Update()
		{
			this.calculatedRadius = (this.animateRadius ? (this.radius * this.radiusCoefficientCurve.Evaluate(this.stopwatch / this.expireDuration)) : this.radius);
			this.stopwatch += Time.deltaTime;
			if (this.expires && NetworkServer.active && this.expireDuration <= this.stopwatch)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(this.rangeIndicator.localScale.x, this.calculatedRadius, ref this.rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x000397E8 File Offset: 0x000379E8
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.buffTimer -= Time.fixedDeltaTime;
				if (this.buffTimer <= 0f)
				{
					this.buffTimer = this.interval;
					float radiusSqr = this.calculatedRadius * this.calculatedRadius;
					Vector3 position = base.transform.position;
					if (this.invertTeamFilter)
					{
						for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
						{
							if (teamIndex != this.teamFilter.teamIndex)
							{
								this.BuffTeam(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
							}
						}
						return;
					}
					this.BuffTeam(TeamComponent.GetTeamMembers(this.teamFilter.teamIndex), radiusSqr, position);
				}
			}
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x00039890 File Offset: 0x00037A90
		private void BuffTeam(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			foreach (TeamComponent teamComponent in recipients)
			{
				if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
				{
					CharacterBody component = teamComponent.GetComponent<CharacterBody>();
					if (component)
					{
						component.AddTimedBuff(this.buffType, this.buffDuration);
					}
				}
			}
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x0003992C File Offset: 0x00037B2C
		// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x0003993F File Offset: 0x00037B3F
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1u);
			}
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00039954 File Offset: 0x00037B54
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
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

		// Token: 0x06000BC8 RID: 3016 RVA: 0x000399C0 File Offset: 0x00037BC0
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

		// Token: 0x04000FAB RID: 4011
		[Tooltip("The area of effect.")]
		[SyncVar]
		public float radius;

		// Token: 0x04000FAC RID: 4012
		[Tooltip("How long between buff pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x04000FAD RID: 4013
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x04000FAE RID: 4014
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04000FAF RID: 4015
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04000FB0 RID: 4016
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x04000FB1 RID: 4017
		[Tooltip("Does the ward disappear over time?")]
		public bool expires;

		// Token: 0x04000FB2 RID: 4018
		[Tooltip("If set, applies to all teams BUT the one selected.")]
		public bool invertTeamFilter;

		// Token: 0x04000FB3 RID: 4019
		public float expireDuration;

		// Token: 0x04000FB4 RID: 4020
		public bool animateRadius;

		// Token: 0x04000FB5 RID: 4021
		public AnimationCurve radiusCoefficientCurve;

		// Token: 0x04000FB6 RID: 4022
		private TeamFilter teamFilter;

		// Token: 0x04000FB7 RID: 4023
		private float buffTimer;

		// Token: 0x04000FB8 RID: 4024
		private float rangeIndicatorScaleVelocity;

		// Token: 0x04000FB9 RID: 4025
		private float stopwatch;

		// Token: 0x04000FBA RID: 4026
		private float calculatedRadius;
	}
}
