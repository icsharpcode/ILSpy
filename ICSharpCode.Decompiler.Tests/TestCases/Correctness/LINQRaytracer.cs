// This test case is taken from https://blogs.msdn.microsoft.com/lukeh/2007/10/01/taking-linq-to-objects-to-extremes-a-fully-linqified-raytracer/

using System;
using System.Collections.Generic;
using System.Linq;

namespace RayTracer
{
	public class RayTracer
	{
		static void Main()
		{
			const int width = 50;
			const int height = 50;

			RayTracer rayTracer = new RayTracer(width, height, (int x, int y, Color color) => {
				Console.Write("{0},{1}:{2};", x, y, color);
			});
			rayTracer.Render(rayTracer.DefaultScene);
		}

		private int screenWidth;
		private int screenHeight;
		private const int MaxDepth = 5;

		public Action<int, int, Color> setPixel;

		public RayTracer(int screenWidth, int screenHeight, Action<int, int, Color> setPixel)
		{
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;
			this.setPixel = setPixel;
		}

		private class Wrap<T>
		{
			public readonly Func<Wrap<T>, T> It;
			public Wrap(Func<Wrap<T>, T> it) { It = it; }
		}

		public static Func<T, U> Y<T, U>(Func<Func<T, U>, Func<T, U>> f)
		{
			Func<Wrap<Func<T, U>>, Func<T, U>> g = wx => f(wx.It(wx));
			return g(new Wrap<Func<T, U>>(wx => f(y => wx.It(wx)(y))));
		}

		class TraceRayArgs
		{
			public readonly Ray Ray;
			public readonly Scene Scene;
			public readonly int Depth;

			public TraceRayArgs(Ray ray, Scene scene, int depth) { Ray = ray; Scene = scene; Depth = depth; }
		}

		internal void Render(Scene scene)
		{
			var pixelsQuery =
				from y in Enumerable.Range(0, screenHeight)
				let recenterY = -(y - (screenHeight / 2.0)) / (2.0 * screenHeight)
				select from x in Enumerable.Range(0, screenWidth)
					   let recenterX = (x - (screenWidth / 2.0)) / (2.0 * screenWidth)
					   let point =
						   Vector.Norm(Vector.Plus(scene.Camera.Forward,
												   Vector.Plus(Vector.Times(recenterX, scene.Camera.Right),
															   Vector.Times(recenterY, scene.Camera.Up))))
					   let ray = new Ray() { Start = scene.Camera.Pos, Dir = point }
					   let computeTraceRay = (Func<Func<TraceRayArgs, Color>, Func<TraceRayArgs, Color>>)
						(f => traceRayArgs =>
						 (from isect in
							  from thing in traceRayArgs.Scene.Things
							  select thing.Intersect(traceRayArgs.Ray)
						  where isect != null
						  orderby isect.Dist
						  let d = isect.Ray.Dir
						  let pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start)
						  let normal = isect.Thing.Normal(pos)
						  let reflectDir = Vector.Minus(d, Vector.Times(2 * Vector.Dot(normal, d), normal))
						  let naturalColors =
							  from light in traceRayArgs.Scene.Lights
							  let ldis = Vector.Minus(light.Pos, pos)
							  let livec = Vector.Norm(ldis)
							  let testRay = new Ray() { Start = pos, Dir = livec }
							  let testIsects = from inter in
												   from thing in traceRayArgs.Scene.Things
												   select thing.Intersect(testRay)
											   where inter != null
											   orderby inter.Dist
											   select inter
							  let testIsect = testIsects.FirstOrDefault()
							  let neatIsect = testIsect == null ? 0 : testIsect.Dist
							  let isInShadow = !((neatIsect > Vector.Mag(ldis)) || (neatIsect == 0))
							  where !isInShadow
							  let illum = Vector.Dot(livec, normal)
							  let lcolor = illum > 0 ? Color.Times(illum, light.Color) : Color.Make(0, 0, 0)
							  let specular = Vector.Dot(livec, Vector.Norm(reflectDir))
							  let scolor = specular > 0
											 ? Color.Times(Math.Pow(specular, isect.Thing.Surface.Roughness),
														   light.Color)
											 : Color.Make(0, 0, 0)
							  select Color.Plus(Color.Times(isect.Thing.Surface.Diffuse(pos), lcolor),
												Color.Times(isect.Thing.Surface.Specular(pos), scolor))
						  let reflectPos = Vector.Plus(pos, Vector.Times(.001, reflectDir))
						  let reflectColor = traceRayArgs.Depth >= MaxDepth
											  ? Color.Make(.5, .5, .5)
											  : Color.Times(isect.Thing.Surface.Reflect(reflectPos),
															f(new TraceRayArgs(new Ray() {
																Start = reflectPos,
																Dir = reflectDir
															},
																			   traceRayArgs.Scene,
																			   traceRayArgs.Depth + 1)))
						  select naturalColors.Aggregate(reflectColor,
														 (color, natColor) => Color.Plus(color, natColor))
						 ).DefaultIfEmpty(Color.Background).First())
					   let traceRay = Y(computeTraceRay)
					   select new { X = x, Y = y, Color = traceRay(new TraceRayArgs(ray, scene, 0)) };

			foreach (var row in pixelsQuery)
				foreach (var pixel in row)
					setPixel(pixel.X, pixel.Y, pixel.Color);
		}

		internal readonly Scene DefaultScene =
			new Scene() {
				Things = new SceneObject[] {
								new Plane() {
									Norm = Vector.Make(0,1,0),
									Offset = 0,
									Surface = Surfaces.CheckerBoard
								},
								new Sphere() {
									Center = Vector.Make(0,1,0),
									Radius = 1,
									Surface = Surfaces.Shiny
								},
								new Sphere() {
									Center = Vector.Make(-1,.5,1.5),
									Radius = .5,
									Surface = Surfaces.Shiny
								}},
				Lights = new Light[] {
								new Light() {
									Pos = Vector.Make(-2,2.5,0),
									Color = Color.Make(.49,.07,.07)
								},
								new Light() {
									Pos = Vector.Make(1.5,2.5,1.5),
									Color = Color.Make(.07,.07,.49)
								},
								new Light() {
									Pos = Vector.Make(1.5,2.5,-1.5),
									Color = Color.Make(.07,.49,.071)
								},
								new Light() {
									Pos = Vector.Make(0,3.5,0),
									Color = Color.Make(.21,.21,.35)
								}},
				Camera = Camera.Create(Vector.Make(3, 2, 4), Vector.Make(-1, .5, 0))
			};
	}

	static class Surfaces
	{
		// Only works with X-Z plane.
		public static readonly Surface CheckerBoard =
			new Surface() {
				Diffuse = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
									? Color.Make(1, 1, 1)
									: Color.Make(0, 0, 0),
				Specular = pos => Color.Make(1, 1, 1),
				Reflect = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
									? .1
									: .7,
				Roughness = 150
			};


		public static readonly Surface Shiny =
			new Surface() {
				Diffuse = pos => Color.Make(1, 1, 1),
				Specular = pos => Color.Make(.5, .5, .5),
				Reflect = pos => .6,
				Roughness = 50
			};
	}

	class Vector
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		public Vector(double x, double y, double z) { X = x; Y = y; Z = z; }

		public static Vector Make(double x, double y, double z) { return new Vector(x, y, z); }
		public static Vector Times(double n, Vector v)
		{
			return new Vector(v.X * n, v.Y * n, v.Z * n);
		}
		public static Vector Minus(Vector v1, Vector v2)
		{
			return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}
		public static Vector Plus(Vector v1, Vector v2)
		{
			return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}
		public static double Dot(Vector v1, Vector v2)
		{
			return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
		}
		public static double Mag(Vector v) { return Math.Sqrt(Dot(v, v)); }
		public static Vector Norm(Vector v)
		{
			double mag = Mag(v);
			double div = mag == 0 ? double.PositiveInfinity : 1 / mag;
			return Times(div, v);
		}
		public static Vector Cross(Vector v1, Vector v2)
		{
			return new Vector(((v1.Y * v2.Z) - (v1.Z * v2.Y)),
							  ((v1.Z * v2.X) - (v1.X * v2.Z)),
							  ((v1.X * v2.Y) - (v1.Y * v2.X)));
		}
		public static bool Equals(Vector v1, Vector v2)
		{
			return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
		}
	}

	public class Color
	{
		public double R;
		public double G;
		public double B;

		public Color(double r, double g, double b) { R = r; G = g; B = b; }

		public static Color Make(double r, double g, double b) { return new Color(r, g, b); }

		public static Color Times(double n, Color v)
		{
			return new Color(n * v.R, n * v.G, n * v.B);
		}
		public static Color Times(Color v1, Color v2)
		{
			return new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);
		}

		public static Color Plus(Color v1, Color v2)
		{
			return new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
		}
		public static Color Minus(Color v1, Color v2)
		{
			return new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
		}

		public static readonly Color Background = Make(0, 0, 0);
		public static readonly Color DefaultColor = Make(0, 0, 0);

		private double Legalize(double d)
		{
			return d > 1 ? 1 : d;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1},{2}]", R, G, B);
		}
	}

	class Ray
	{
		public Vector Start;
		public Vector Dir;
	}

	class ISect
	{
		public SceneObject Thing;
		public Ray Ray;
		public double Dist;
	}

	class Surface
	{
		public Func<Vector, Color> Diffuse;
		public Func<Vector, Color> Specular;
		public Func<Vector, double> Reflect;
		public double Roughness;
	}

	class Camera
	{
		public Vector Pos;
		public Vector Forward;
		public Vector Up;
		public Vector Right;

		public static Camera Create(Vector pos, Vector lookAt)
		{
			Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
			Vector down = new Vector(0, -1, 0);
			Vector right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
			Vector up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

			return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
		}
	}

	class Light
	{
		public Vector Pos;
		public Color Color;
	}

	abstract class SceneObject
	{
		public Surface Surface;
		public abstract ISect Intersect(Ray ray);
		public abstract Vector Normal(Vector pos);
	}

	class Sphere : SceneObject
	{
		public Vector Center;
		public double Radius;

		public override ISect Intersect(Ray ray)
		{
			Vector eo = Vector.Minus(Center, ray.Start);
			double v = Vector.Dot(eo, ray.Dir);
			double dist;
			if (v < 0)
			{
				dist = 0;
			}
			else
			{
				double disc = Math.Pow(Radius, 2) - (Vector.Dot(eo, eo) - Math.Pow(v, 2));
				dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
			}
			if (dist == 0)
				return null;
			return new ISect() {
				Thing = this,
				Ray = ray,
				Dist = dist
			};
		}

		public override Vector Normal(Vector pos)
		{
			return Vector.Norm(Vector.Minus(pos, Center));
		}
	}

	class Plane : SceneObject
	{
		public Vector Norm;
		public double Offset;

		public override ISect Intersect(Ray ray)
		{
			double denom = Vector.Dot(Norm, ray.Dir);
			if (denom > 0)
				return null;
			return new ISect() {
				Thing = this,
				Ray = ray,
				Dist = (Vector.Dot(Norm, ray.Start) + Offset) / (-denom)
			};
		}

		public override Vector Normal(Vector pos)
		{
			return Norm;
		}
	}

	class Scene
	{
		public SceneObject[] Things;
		public Light[] Lights;
		public Camera Camera;

		public IEnumerable<ISect> Intersect(Ray r)
		{
			return from thing in Things
				   select thing.Intersect(r);
		}
	}
}