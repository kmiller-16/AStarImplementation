using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace AStarDemo.Models
{
	public class Grid
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public GridSquare TargetNode
		{
			get;
			set;
		}

		public GridSquare[,] Nodes
		{
			get;
			set;
		}

		public SimplePriorityQueue<GridSquare> Open
		{
			get;
			set;
		}

		public List<GridSquare> Closed
		{
			get;
			set;
		}

		public List<GridSquare> Blob
		{
			get;
			set;
		}

		public Grid()
		{
			Nodes = BuildGrid();
			Open = new SimplePriorityQueue<GridSquare>();
			Closed = new List<GridSquare>();
			Blob = new List<GridSquare>();
		}

		public GridSquare[,] BuildGrid()
		{
			Width = 10;
			Height = 10;
			GridSquare[,] grd = new GridSquare[Width, Height];
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					grd[i, j] = new GridSquare(i, j);
				}
			}
			
			return grd;
		}
		
		public void ToggleBlocked(int x, int y)
		{
			Nodes[x, y].IsBlocked = !Nodes[x, y].IsBlocked;
		}
		public void AddBlob(int x, int y) {
			this.Blob.Add(Nodes[x, y]);
		}
		public void SetTarget(int x, int y) {
			this.TargetNode = Nodes[x, y];
		}
		public bool IsInBlob(GridSquare s) {
			foreach (var gs in Blob) {
				if (s.CoordX == gs.CoordX && s.CoordY == gs.CoordY) 
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInClosed(GridSquare s)
		{
			foreach (var gs in Closed)
			{
				if (s.CoordX == gs.CoordX && s.CoordY == gs.CoordY)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInOpen(GridSquare s)
		{
			foreach (var gs in Open)
			{
				if (s.CoordX == gs.CoordX && s.CoordY == gs.CoordY)
				{
					return true;
				}
			}
			return false;
		}

		public void ProcessData(Controllers.HomeController.GData d) {
			TargetNode = Nodes[d.target.x, d.target.y];
			foreach (var p in d.blob) {
				Blob.Add(Nodes[p.x, p.y]);
			}
			foreach (var p in d.blocked) {
				ToggleBlocked(p.x, p.y);
			}
			StringBuilder sb = new StringBuilder("Grid Setup Is: \n" +
				"Target: " + TargetNode.CoordX + ", " + TargetNode.CoordY + " \n");
			sb.Append("Blob: ");
			foreach (var p in Blob) {
				sb.Append(p.CoordX + ", " + p.CoordY + " - ");
			}
			sb.Append("\nBlocked: ");
			foreach (var p in d.blocked) {
				sb.Append( p.x + ", " + p.y + " - ");
			}
			Debug.WriteLine(sb.ToString());
			Debug.WriteLine("Begining PathFind");
		}

		public void ProcessNeighbors(List<GridSquare> n, GridSquare p) {
			Debug.WriteLine("processing neighbor set...");
			foreach (var g in n) {

				if (IsInClosed(g))
				{
					Debug.WriteLine("Node closed moving on...");
					continue;
				}
				else if (g.IsBlocked) {
					continue;
				}
				else if (IsInOpen(g))
				{

					Debug.WriteLine("Comparing parents and re-prioritizing...");
					if (g.CompareParentTo(p))
					{
						g.Parent = Nodes[p.CoordX, p.CoordY];
						g.LowestHCost(Blob);
						g.ComputeGCost();
						g.ComputeFCost();
						Open.UpdatePriority(g, g.FCost);
					}
				}

				else if (g.Parent == null)
				{

					g.Parent = Nodes[p.CoordX, p.CoordY];
					g.LowestHCost(Blob);
					g.ComputeGCost();
					g.ComputeFCost();
					Debug.WriteLine("New Node enqueued: " + g.CoordX + ", " + g.CoordY);
					Open.Enqueue(g, g.FCost);

				}
				else
				{
					throw new Exception("WTF you moron");
				}
				
			}
		}

		public List<GridSquare> BacktrackFrom(GridSquare g) {
			List<GridSquare> finalPath = new List<GridSquare>();
			GridSquare next;
;			if (g.Parent == null) return finalPath;
			else 
			{
				do
				{
					next = Nodes[g.Parent.CoordX, g.Parent.CoordY];
					if (next.CoordX == g.CoordX && g.CoordY == next.CoordY)
					{
						Debug.WriteLine("ERROR! Gridsquare points to itself!");
						return finalPath;
					}
					else
					{
						finalPath.Add(Nodes[next.CoordX, next.CoordY]);
						g = next;
					}
				} while (g.Parent != null);
			}
			finalPath.RemoveAt(finalPath.Count - 1);
			return finalPath;
		}

		public List<GridSquare> FindPath(Controllers.HomeController.GData d) {
			ProcessData(d);
			GridSquare next;
			List<GridSquare> path = new List<GridSquare>();
			List<GridSquare> neighbors;
			Debug.WriteLine("Target is: " + TargetNode.CoordX + ", " + TargetNode.CoordY);
			GridSquare current = Nodes[TargetNode.CoordX, TargetNode.CoordY];
			while (!IsInBlob(current)) {
				neighbors = current.VisitNeighbors(Width, Height, this);
				ProcessNeighbors(neighbors, current);
				next = Open.Dequeue();
				Closed.Add(Nodes[current.CoordX, current.CoordY]);
				current = Nodes[next.CoordX, next.CoordY];
			}
			path = BacktrackFrom(current);
			return path;
		}
        //TODO code in pathfinding send out path as something that can be JSON
    }
}