using AStarDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AStarDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()

        {
            Grid MyGraph = new Grid();
            return View(MyGraph);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public JsonResult FindPath(GData clientData)
        {
            GData data = new GData
            {
                blocked = clientData.blocked,
                blob = clientData.blob,
                target = clientData.target
            };
            Debug.WriteLine("View from Home: " + "\nBlocked: " + data.blocked[0].x + ", " + data.blocked[0].y + "\nBlob: " + data.blob[0].x + ", " + data.blob[0].y + "\nTarget: " + data.target.x + ", " + data.target.y);
            PathData finalPath = new PathData
            {
                path = GetPath(data).path
            };
            string myDebug = "";
            foreach (var pt in finalPath.path) {
                myDebug = myDebug + pt.x + ", " + pt.y + "\n";
            }
            Debug.WriteLine("The final path is: \n" + myDebug);
            return Json(finalPath, JsonRequestBehavior.AllowGet);
        }

        public class GData
        {
            public Point[] blocked { get; set; }
            public Point[] blob { get; set; }
            public Point target { get; set; }
        }

        public class Point
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class PathData
        {
            public Point[] path { get; set; }
        }

        public GridSquare ToGridSquare(Point p)
        {
            return new GridSquare(p.x, p.y);
        }

        public List<GridSquare> ToGSList(Point[] pts)
        {
            List<GridSquare> fList = new List<GridSquare>();
            foreach (var pt in pts)
            {
                fList.Add(ToGridSquare(pt));
            }
            return fList;
        }

        public Point ToPoint(GridSquare g) {
            Point p = new Point();
            p.x = g.CoordX;
            p.y = g.CoordY;
            return p;
        }

        public PathData GetPath(GData d)
        {
            Grid grid = new Grid();
            List<GridSquare> rPath = grid.FindPath(d);
            List<Point> allPts = new List<Point>();
            PathData fPath = new PathData();
            foreach (var gs in rPath)
            {
                allPts.Add(ToPoint(gs));
            }
            fPath.path = allPts.ToArray();
            return fPath;
        }
    }
}