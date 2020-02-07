using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AStarDemo.Models
{
	public class GridSquare
	{
		public bool IsOccupied
		{
			get;
			set;
		}

		public bool IsBlocked
		{
			get;
			set;
		}

		public int CoordX
		{
			get;
			set;
		}

		public int CoordY
		{
			get;
			set;
		}

		public GridSquare Parent
		{
			get;
			set;
		}

		public GridSquare ClosestBlob
		{
			get;
			set;
		}

		public int GCost
		{
			get;
			set;
		}

		public int HCost
		{
			get;
			set;
		}

		public int FCost
		{
			get;
			set;
		}

		public GridSquare(int x, int y)
		{
			CoordX = x;
			CoordY = y;
			IsOccupied = false;
			IsBlocked = false;
			Parent = null;
		}

		public int Visit()
		{
			return 0;
		}

		public void ComputeGCost()
		{
			if (Parent != null)
			{
				GCost = Parent.GCost + 10;
			}
			else 
			{
				Debug.WriteLine("We're starting right here: " + this.CoordX + ", " + this.CoordY);
			}
		}

		public void LowestHCost(List<GridSquare> blob) {
			int finalCost = int.MaxValue;
			int candidateCost;
			foreach (var s in blob) {
				candidateCost = ComputeHCost(s.CoordX, s.CoordY);
				if (candidateCost < finalCost) {
					finalCost = candidateCost;
					ClosestBlob = s;
				}
			}
			HCost = finalCost;
		}

		public int ComputeHCost(int tcx, int tcy)
		{
			return (10 * (Math.Abs(tcx - CoordX) + Math.Abs(tcy - CoordY)));
		}

		public void ComputeFCost()
		{
			FCost = HCost + GCost;
		}

		public bool CompareParentTo(GridSquare candidate) {
			if (Parent != null) 
			{
				return (FCost > (candidate.GCost + 10 + HCost));
			} 
			else throw new Exception("Pathing Error. Corrupt data"); 
		}

		public List<GridSquare> VisitNeighbors(int w, int h, Grid g) {
			List<GridSquare> neighbors = new List<GridSquare>();
			//Check North
			if (CoordY - 1 >= 0) neighbors.Add(g.Nodes[CoordX, CoordY - 1]);
			//Check South
			if (CoordY + 1 < h) neighbors.Add(g.Nodes[CoordX, CoordY + 1]);
			//Check East
			if (CoordX + 1 < w) neighbors.Add(g.Nodes[CoordX + 1, CoordY]);
			//Check West
			if (CoordX - 1 >= 0) neighbors.Add(g.Nodes[CoordX - 1, CoordY]);
			if (neighbors.Count < 2 || neighbors.Count > 4) 
			{
				Debug.WriteLine("Neighbor find Error! Number of neighbors is invalid!!!! --" + neighbors.Count);
			}
			Debug.WriteLine("At " + CoordX + ", " + CoordY + " My neighbors are...");
			foreach (var gs in neighbors) {
				Debug.WriteLine(gs.CoordX + ", " + gs.CoordY);
			}
			return neighbors;
		}
	}
}