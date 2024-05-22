namespace DataStructuresProject.Graphics
{
    public class Vector2
    {
        public static Vector2 Zero { get; } = new Vector2(0, 0);

        public double X { get; set; }
        public double Y { get; set; }

        public Vector2()
        {
        }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        /*public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X / v2.X, v1.Y / v2.Y);
        }*/
    }

    /*public class Vector2I
    {
        public static Vector2I Zero { get; } = new Vector2I(0, 0);

        public int X { get; set; }
        public int Y { get; set; }

        public Vector2I()
        {
        }

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }
    }*/
}
