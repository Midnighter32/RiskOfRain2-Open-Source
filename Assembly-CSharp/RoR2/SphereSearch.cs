using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000429 RID: 1065
	public class SphereSearch
	{
		// Token: 0x06001981 RID: 6529 RVA: 0x0006D7FC File Offset: 0x0006B9FC
		public SphereSearch RefreshCandidates()
		{
			Collider[] array = Physics.OverlapSphere(this.origin, this.radius, this.mask, this.queryTriggerInteraction);
			SphereSearch.Candidate[] array2 = new SphereSearch.Candidate[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Collider collider = array[i];
				ref SphereSearch.Candidate ptr = ref array2[i];
				ptr.collider = collider;
				MeshCollider meshCollider;
				if ((meshCollider = (collider as MeshCollider)) != null && !meshCollider.convex)
				{
					ptr.position = collider.ClosestPointOnBounds(this.origin);
				}
				else
				{
					ptr.position = collider.ClosestPoint(this.origin);
				}
				ptr.difference = ptr.position - this.origin;
				ptr.distanceSqr = ptr.difference.sqrMagnitude;
			}
			this.searchData = new SphereSearch.SearchData(array2);
			return this;
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0006D8D3 File Offset: 0x0006BAD3
		public SphereSearch OrderCandidatesByDistance()
		{
			this.searchData.OrderByDistance();
			return this;
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0006D8E1 File Offset: 0x0006BAE1
		public SphereSearch FilterCandidatesByHurtBoxTeam(TeamMask mask)
		{
			this.searchData.FilterByHurtBoxTeam(mask);
			return this;
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0006D8F0 File Offset: 0x0006BAF0
		public SphereSearch FilterCandidatesByDistinctHurtBoxEntities()
		{
			this.searchData.FilterByHurtBoxEntitiesDistinct();
			return this;
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0006D8FE File Offset: 0x0006BAFE
		public SphereSearch FilterCandidatesByProjectileControllers()
		{
			this.searchData.FilterByProjectileControllers();
			return this;
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0006D90C File Offset: 0x0006BB0C
		public HurtBox[] GetHurtBoxes()
		{
			return this.searchData.GetHurtBoxes();
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x0006D919 File Offset: 0x0006BB19
		public void GetHurtBoxes(List<HurtBox> dest)
		{
			this.searchData.GetHurtBoxes(dest);
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x0006D927 File Offset: 0x0006BB27
		public void GetProjectileControllers(List<ProjectileController> dest)
		{
			this.searchData.GetProjectileControllers(dest);
		}

		// Token: 0x040017AE RID: 6062
		public float radius;

		// Token: 0x040017AF RID: 6063
		public Vector3 origin;

		// Token: 0x040017B0 RID: 6064
		public LayerMask mask;

		// Token: 0x040017B1 RID: 6065
		public QueryTriggerInteraction queryTriggerInteraction;

		// Token: 0x040017B2 RID: 6066
		private SphereSearch.SearchData searchData = SphereSearch.SearchData.empty;

		// Token: 0x0200042A RID: 1066
		private struct Candidate
		{
			// Token: 0x0600198A RID: 6538 RVA: 0x0006D948 File Offset: 0x0006BB48
			public static bool HurtBoxHealthComponentIsValid(SphereSearch.Candidate candidate)
			{
				return candidate.hurtBox.healthComponent;
			}

			// Token: 0x040017B3 RID: 6067
			public Collider collider;

			// Token: 0x040017B4 RID: 6068
			public HurtBox hurtBox;

			// Token: 0x040017B5 RID: 6069
			public Vector3 position;

			// Token: 0x040017B6 RID: 6070
			public Vector3 difference;

			// Token: 0x040017B7 RID: 6071
			public float distanceSqr;

			// Token: 0x040017B8 RID: 6072
			public Transform root;

			// Token: 0x040017B9 RID: 6073
			public ProjectileController projectileController;
		}

		// Token: 0x0200042B RID: 1067
		private struct SearchData
		{
			// Token: 0x0600198B RID: 6539 RVA: 0x0006D95C File Offset: 0x0006BB5C
			public SearchData(SphereSearch.Candidate[] candidatesBuffer)
			{
				this.candidatesBuffer = candidatesBuffer;
				this.candidatesMapping = new int[candidatesBuffer.Length];
				this.candidatesCount = candidatesBuffer.Length;
				for (int i = 0; i < candidatesBuffer.Length; i++)
				{
					this.candidatesMapping[i] = i;
				}
				this.hurtBoxesLoaded = false;
				this.rootsLoaded = false;
				this.projectileControllersLoaded = false;
				this.filteredByHurtBoxes = false;
				this.filteredByHurtBoxHealthComponents = false;
				this.filteredByProjectileControllers = false;
			}

			// Token: 0x0600198C RID: 6540 RVA: 0x0006D9C8 File Offset: 0x0006BBC8
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private ref SphereSearch.Candidate GetCandidate(int i)
			{
				return ref this.candidatesBuffer[this.candidatesMapping[i]];
			}

			// Token: 0x0600198D RID: 6541 RVA: 0x0006D9DD File Offset: 0x0006BBDD
			private void RemoveCandidate(int i)
			{
				HGArrayUtilities.ArrayRemoveAt<int>(ref this.candidatesMapping, ref this.candidatesCount, i, 1);
			}

			// Token: 0x0600198E RID: 6542 RVA: 0x0006D9F4 File Offset: 0x0006BBF4
			public void LoadHurtBoxes()
			{
				if (this.hurtBoxesLoaded)
				{
					return;
				}
				for (int i = 0; i < this.candidatesCount; i++)
				{
					ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
					candidate.hurtBox = candidate.collider.GetComponent<HurtBox>();
				}
				this.hurtBoxesLoaded = true;
			}

			// Token: 0x0600198F RID: 6543 RVA: 0x0006DA3C File Offset: 0x0006BC3C
			public void LoadRoots()
			{
				if (this.rootsLoaded)
				{
					return;
				}
				for (int i = 0; i < this.candidatesCount; i++)
				{
					ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
					candidate.root = candidate.collider.transform.root;
				}
				this.rootsLoaded = true;
			}

			// Token: 0x06001990 RID: 6544 RVA: 0x0006DA88 File Offset: 0x0006BC88
			public void LoadProjectileControllers()
			{
				if (this.projectileControllersLoaded)
				{
					return;
				}
				this.LoadRoots();
				for (int i = 0; i < this.candidatesCount; i++)
				{
					ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
					candidate.projectileController = (candidate.root ? candidate.root.GetComponent<ProjectileController>() : null);
				}
				this.projectileControllersLoaded = true;
			}

			// Token: 0x06001991 RID: 6545 RVA: 0x0006DAE8 File Offset: 0x0006BCE8
			public void FilterByProjectileControllers()
			{
				if (this.filteredByProjectileControllers)
				{
					return;
				}
				this.LoadProjectileControllers();
				for (int i = this.candidatesCount - 1; i >= 0; i--)
				{
					if (!this.GetCandidate(i).projectileController)
					{
						this.RemoveCandidate(i);
					}
				}
				this.filteredByProjectileControllers = true;
			}

			// Token: 0x06001992 RID: 6546 RVA: 0x0006DB38 File Offset: 0x0006BD38
			public void FilterByHurtBoxes()
			{
				if (this.filteredByHurtBoxes)
				{
					return;
				}
				this.LoadHurtBoxes();
				for (int i = this.candidatesCount - 1; i >= 0; i--)
				{
					if (!this.GetCandidate(i).hurtBox)
					{
						this.RemoveCandidate(i);
					}
				}
				this.filteredByHurtBoxes = true;
			}

			// Token: 0x06001993 RID: 6547 RVA: 0x0006DB88 File Offset: 0x0006BD88
			public void FilterByHurtBoxHealthComponents()
			{
				if (this.filteredByHurtBoxHealthComponents)
				{
					return;
				}
				this.FilterByHurtBoxes();
				for (int i = this.candidatesCount - 1; i >= 0; i--)
				{
					if (!this.GetCandidate(i).hurtBox.healthComponent)
					{
						this.RemoveCandidate(i);
					}
				}
				this.filteredByHurtBoxHealthComponents = true;
			}

			// Token: 0x06001994 RID: 6548 RVA: 0x0006DBE0 File Offset: 0x0006BDE0
			public void FilterByHurtBoxTeam(TeamMask teamMask)
			{
				this.FilterByHurtBoxes();
				for (int i = this.candidatesCount - 1; i >= 0; i--)
				{
					ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
					if (!teamMask.HasTeam(candidate.hurtBox.teamIndex))
					{
						this.RemoveCandidate(i);
					}
				}
			}

			// Token: 0x06001995 RID: 6549 RVA: 0x0006DC2C File Offset: 0x0006BE2C
			public void FilterByHurtBoxEntitiesDistinct()
			{
				this.FilterByHurtBoxHealthComponents();
				for (int i = this.candidatesCount - 1; i >= 0; i--)
				{
					ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
					for (int j = i - 1; j >= 0; j--)
					{
						ref SphereSearch.Candidate candidate2 = ref this.GetCandidate(j);
						if (candidate.hurtBox.healthComponent == candidate2.hurtBox.healthComponent)
						{
							this.RemoveCandidate(i);
							break;
						}
					}
				}
			}

			// Token: 0x06001996 RID: 6550 RVA: 0x0006DC94 File Offset: 0x0006BE94
			public void OrderByDistance()
			{
				if (this.candidatesCount == 0)
				{
					return;
				}
				bool flag = true;
				while (flag)
				{
					flag = false;
					ref SphereSearch.Candidate ptr = ref this.GetCandidate(0);
					int i = 1;
					int num = this.candidatesCount - 1;
					while (i < num)
					{
						ref SphereSearch.Candidate candidate = ref this.GetCandidate(i);
						if (ptr.distanceSqr > candidate.distanceSqr)
						{
							Util.Swap<int>(ref this.candidatesMapping[i - 1], ref this.candidatesMapping[i]);
							flag = true;
						}
						else
						{
							ptr = ref candidate;
						}
						i++;
					}
				}
			}

			// Token: 0x06001997 RID: 6551 RVA: 0x0006DD10 File Offset: 0x0006BF10
			public HurtBox[] GetHurtBoxes()
			{
				this.FilterByHurtBoxes();
				HurtBox[] array = new HurtBox[this.candidatesCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetCandidate(i).hurtBox;
				}
				return array;
			}

			// Token: 0x06001998 RID: 6552 RVA: 0x0006DD50 File Offset: 0x0006BF50
			public void GetHurtBoxes(List<HurtBox> dest)
			{
				int num = dest.Count + this.candidatesCount;
				if (dest.Capacity < num)
				{
					dest.Capacity = num;
				}
				for (int i = 0; i < this.candidatesCount; i++)
				{
					dest.Add(this.GetCandidate(i).hurtBox);
				}
			}

			// Token: 0x06001999 RID: 6553 RVA: 0x0006DDA0 File Offset: 0x0006BFA0
			public void GetProjectileControllers(List<ProjectileController> dest)
			{
				int num = dest.Count + this.candidatesCount;
				if (dest.Capacity < num)
				{
					dest.Capacity = num;
				}
				for (int i = 0; i < this.candidatesCount; i++)
				{
					dest.Add(this.GetCandidate(i).projectileController);
				}
			}

			// Token: 0x040017BA RID: 6074
			private SphereSearch.Candidate[] candidatesBuffer;

			// Token: 0x040017BB RID: 6075
			private int[] candidatesMapping;

			// Token: 0x040017BC RID: 6076
			private int candidatesCount;

			// Token: 0x040017BD RID: 6077
			private bool hurtBoxesLoaded;

			// Token: 0x040017BE RID: 6078
			private bool rootsLoaded;

			// Token: 0x040017BF RID: 6079
			private bool projectileControllersLoaded;

			// Token: 0x040017C0 RID: 6080
			private bool filteredByHurtBoxes;

			// Token: 0x040017C1 RID: 6081
			private bool filteredByHurtBoxHealthComponents;

			// Token: 0x040017C2 RID: 6082
			private bool filteredByProjectileControllers;

			// Token: 0x040017C3 RID: 6083
			public static readonly SphereSearch.SearchData empty = new SphereSearch.SearchData
			{
				candidatesBuffer = Array.Empty<SphereSearch.Candidate>(),
				candidatesMapping = Array.Empty<int>(),
				candidatesCount = 0,
				hurtBoxesLoaded = false
			};

			// Token: 0x0200042C RID: 1068
			private struct DistanceComparer : IComparer<int>
			{
				// Token: 0x0600199B RID: 6555 RVA: 0x0006DE33 File Offset: 0x0006C033
				private ref SphereSearch.Candidate GetCandidate(int i)
				{
					return ref this.candidatesBuffer[this.candidatesMapping[i]];
				}

				// Token: 0x0600199C RID: 6556 RVA: 0x0006DE48 File Offset: 0x0006C048
				public int Compare(int candidateIdA, int candidateIdB)
				{
					return this.GetCandidate(candidateIdA).distanceSqr.CompareTo(this.GetCandidate(candidateIdB).distanceSqr);
				}

				// Token: 0x040017C4 RID: 6084
				public SphereSearch.Candidate[] candidatesBuffer;

				// Token: 0x040017C5 RID: 6085
				public int[] candidatesMapping;
			}
		}
	}
}
