using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x02000928 RID: 2344
	public class InverseKinematics : MonoBehaviour
	{
		// Token: 0x0600349D RID: 13469 RVA: 0x000E59FE File Offset: 0x000E3BFE
		private void OnEnable()
		{
			if (this.rigReader == null)
			{
				this.DetectRig();
			}
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x000E5A10 File Offset: 0x000E3C10
		private void LateUpdate()
		{
			Core.Solvers solvers = this.solver;
			if (solvers != Core.Solvers.CyclicDescend)
			{
				if (solvers == Core.Solvers.FastReach)
				{
					for (int i = 0; i < this.otherChains.Length; i++)
					{
						FastReachSolver.Process(this.otherChains[i]);
					}
					FastReachSolver.Process(this.rLeg);
					FastReachSolver.Process(this.lLeg);
					FastReachSolver.Process(this.rArm);
					FastReachSolver.Process(this.lArm);
				}
			}
			else
			{
				CyclicDescendSolver.Process(this.rLeg);
				CyclicDescendSolver.Process(this.lLeg);
				CyclicDescendSolver.Process(this.rArm);
				CyclicDescendSolver.Process(this.lArm);
				for (int j = 0; j < this.otherChains.Length; j++)
				{
					CyclicDescendSolver.Process(this.otherChains[j]);
				}
			}
			for (int k = 0; k < this.otherKChains.Length; k++)
			{
				ChainKinematicSolver.Process(this.otherKChains[k]);
			}
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x000E5AF4 File Offset: 0x000E3CF4
		public void DetectRig()
		{
			if (!this.animator)
			{
				this.animator = base.GetComponent<Animator>();
			}
			this.rigReader = new RigReader(this.animator);
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x000E5B20 File Offset: 0x000E3D20
		public void BuildRig()
		{
			this.rArm = this.rigReader.RightArmChain();
			this.lArm = this.rigReader.LeftArmChain();
			this.rLeg = this.rigReader.RightLegChain();
			this.lLeg = this.rigReader.LeftLegChain();
		}

		// Token: 0x04003416 RID: 13334
		public Core.Solvers solver;

		// Token: 0x04003417 RID: 13335
		public Core.Chain rArm;

		// Token: 0x04003418 RID: 13336
		public Core.Chain lArm;

		// Token: 0x04003419 RID: 13337
		public Core.Chain rLeg;

		// Token: 0x0400341A RID: 13338
		public Core.Chain lLeg;

		// Token: 0x0400341B RID: 13339
		public Core.Chain[] otherChains;

		// Token: 0x0400341C RID: 13340
		public Core.KinematicChain[] otherKChains;

		// Token: 0x0400341D RID: 13341
		public RigReader rigReader;

		// Token: 0x0400341E RID: 13342
		public Animator animator;
	}
}
