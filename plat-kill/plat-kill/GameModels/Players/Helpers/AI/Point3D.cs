﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace plat_kill.GameModels.Players.Helpers.AI
{
    /// <summary>    
    /// Author: Roy Triesscheijn (http://www.roy-t.nl)
    /// Point3D class mimics some of the Microsoft.Xna.Framework.Vector3
    /// but uses Int32's instead of floats.
    /// </summary>
    public class Point3D
    {
        public int X;
        public int Y;
        public int Z;

        public const int vectortrans = 20;

        public Point3D(Vector3 vec)
        {
            this.X = (int)vec.X / vectortrans;
            this.Y = 0;
            this.Z = (int)vec.Z / vectortrans;

        }

        public Point3D(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Point3D(Point3D p1, Point3D p2)
        {
            this.X = p1.X + p2.X;
            this.Y = p1.Y + p2.Y;
            this.Z = p1.Z + p2.Z;
        }

        public Vector3 toVector3(float y)
        {
            return new Vector3(this.X*vectortrans,y,this.Z*vectortrans);
        }

        public Vector2 toVector2()
        {
            return new Vector2((this.X * vectortrans), (this.Z * vectortrans));
        }

        public int GetDistanceSquared(Point3D point)
        {
            int dx = this.X - point.X;
            int dy = this.Y - point.Y;
            int dz = this.Z - point.Z;
            return (dx * dx) + (dy * dy) + (dz * dz);            
        }

        public bool EqualsSS(Point3D p)
        {
            return p.X == this.X && p.Z == this.Z && p.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return (X + " " + Y + " " + Z).GetHashCode();
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public static bool operator ==(Point3D one, Point3D two)
        {
            return one.EqualsSS(two);
        }

        public static bool operator !=(Point3D one, Point3D two)
        {
            return !one.EqualsSS(two);
        }

        public static Point3D operator +(Point3D one, Point3D two)
        {
            return new Point3D(one.X + two.X, one.Y + two.Y, one.Z + two.Z);
        }

        public static Point3D operator -(Point3D one, Point3D two)
        {
            return new Point3D(one.X - two.X, one.Y - two.Y, one.Z - two.Z);
        }

        public static Point3D Zero = new Point3D(0, 0, 0);        
    }
}
