using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000169 RID: 361
	[RequireComponent(typeof(TeamFilter))]
	public class BuffWard : NetworkBehaviour
	{
		// Token: 0x060006B1 RID: 1713 RVA: 0x0001B4CB File Offset: 0x000196CB
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0001B4D9 File Offset: 0x000196D9
		private void OnEnable()
		{
			if (this.rangeIndicator)
			{
				this.rangeIndicator.gameObject.SetActive(true);
			}
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001B4F9 File Offset: 0x000196F9
		private void OnDisable()
		{
			if (this.rangeIndicator)
			{
				this.rangeIndicator.gameObject.SetActive(false);
			}
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0001B51C File Offset: 0x0001971C
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

		// Token: 0x060006B5 RID: 1717 RVA: 0x0001B5BC File Offset: 0x000197BC
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

		// Token: 0x060006B6 RID: 1718 RVA: 0x0001B680 File Offset: 0x00019880
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

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001B728 File Offset: 0x00019928
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

		// Token: 0x060006B9 RID: 1721 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x0001B7C4 File Offset: 0x000199C4
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x0001B7D7 File Offset: 0x000199D7
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

		// Token: 0x060006BC RID: 1724 RVA: 0x0001B7EC File Offset: 0x000199EC
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

		// Token: 0x060006BD RID: 1725 RVA: 0x0001B858 File Offset: 0x00019A58
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

		// Token: 0x040006FD RID: 1789
		[Tooltip("The area of effect.")]
		[SyncVar]
		public float radius;

		// Token: 0x040006FE RID: 1790
		[Tooltip("How long between buff pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x040006FF RID: 1791
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x04000700 RID: 1792
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04000701 RID: 1793
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04000702 RID: 1794
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x04000703 RID: 1795
		[Tooltip("Does the ward disappear over time?")]
		public bool expires;

		// Token: 0x04000704 RID: 1796
		[Tooltip("If set, applies to all teams BUT the one selected.")]
		public bool invertTeamFilter;

		// Token: 0x04000705 RID: 1797
		public float expireDuration;

		// Token: 0x04000706 RID: 1798
		public bool animateRadius;

		// Token: 0x04000707 RID: 1799
		public AnimationCurve radiusCoefficientCurve;

		// Token: 0x04000708 RID: 1800
		private TeamFilter teamFilter;

		// Token: 0x04000709 RID: 1801
		private float buffTimer;

		// Token: 0x0400070A RID: 1802
		private float rangeIndicatorScaleVelocity;

		// Token: 0x0400070B RID: 1803
		private float stopwatch;

		// Token: 0x0400070C RID: 1804
		private float calculatedRadius;
	}
}
