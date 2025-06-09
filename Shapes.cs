using System;
using System.Drawing.Drawing2D;

namespace Case1;

public static class Extensions
{
    /// <summary>
    /// Bir noktanın çokgen içinde olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="points"> Çokgenin köşe noktaları.</param>
    /// <param name="point"> Kontrol edilecek nokta.</param>
    /// <returns> Eğer nokta çokgenin içindeyse `true`, aksi halde `false`.</returns>
    public static bool Contain(this Point[] points, Point point)
    {
        if (points == null || points.Length < 3)
        {
            return false;
        }

        bool doesContain = false;
        int j = points.Length - 1; // Son köşe

        for (int i = 0; i < points.Length; ++i)
        {
            // Yatay çizgi kontrolü.
            if (
                points[i].Y < point.Y && points[j].Y >= point.Y
                || points[j].Y < point.Y && points[i].Y >= point.Y
            )
            {
                // Lineer interpolasyon. https://en.wikipedia.org/wiki/Linear_interpolation
                // Kesişim noktasının x ekseninin solunda olup olmadığını kontrol et.
                if (
                    points[i].X
                        + (point.Y - points[i].Y)
                            / (points[j].Y - points[i].Y)
                            * (points[j].X - points[i].X)
                    < point.X
                )
                {
                    doesContain = !doesContain;
                }
            }

            j = i; // Sonraki köşeye git.
        }
        return doesContain;
    }
}

// Şekil sınıfı, tüm şekillerin temelini oluşturur.
public abstract class Shape
{
    public Point Center { get; set; } // Şeklin merkezi
    public int Radius { get; set; } // Şeklin yarıçapı (kullanılan şekle göre değişebilir)
    public Color Color { get; set; } // Şeklin rengi

    /// <summary>
    /// Şekil sınıfını oluşturur.
    /// </summary>
    /// <param name="center"> Şeklin merkezi.</param>
    /// <param name="radius"> Şeklin yarıçapı.</param>
    /// <param name="color"> Şeklin rengi.</param>
    protected Shape(Point center, int radius, Color color)
    {
        if (radius <= 0)
        {
            throw new ArgumentException("Radius must be greater than 0");
        }

        Center = center;
        Radius = radius;
        Color = color;
    }

    /// <summary>
    /// Şekli çizer.
    /// </summary>
    /// <param name="g"> Çizim için Graphics nesnesi.</param>
    public abstract void Draw(Graphics g);

    /// <summary>
    /// Şeklin içine bir noktanın düşüp düşmediğini kontrol eder.
    /// </summary>
    /// <param name="point"> Kontrol edilecek nokta.</param>
    /// <returns> Eğer nokta şeklin içindeyse `true`, aksi halde `false`.</returns>
    public abstract bool HitTest(Point point);

    /// <summary>
    /// Şeklin önizlemesini çizer.
    /// </summary>
    /// <param name="g">Çizim için Graphics nesnesi.</param>
    public virtual void DrawPreview(Graphics g)
    {
        Color originalColor = Color;
        Color = Color.FromArgb(100, originalColor); // Yarı saydamlaştır.

        try
        {
            Draw(g);

            // Sınırları asıl rengiyle çiz
            using (var pen = new Pen(originalColor, 2))
            {
                DrawBorder(g, pen);
            }
        }
        finally
        {
            Color = originalColor; // Çizdikten sonra asıl rengini geri yükle
        }
    }

    /// <summary>
    /// Şekli seçili olarak çizer.
    /// </summary>
    /// <param name="g"> Çizim için Graphics nesnesi.</param>
    public virtual void DrawSelected(Graphics g)
    {
        Draw(g);

        // Seçili sınırları çizer.
        using (var pen = new Pen(Color.Orange, 3))
        {
            pen.DashStyle = DashStyle.Dash; // Kesikli çizgi stili.
            DrawBorder(g, pen);
        }
    }

    /// <summary>
    /// Şeklin sınırlarını çizer.
    /// </summary>
    /// <param name="g"> Çizim için Graphics nesnesi.</param>
    /// <param name="pen"> Çizim için Pen nesnesi.</param>
    protected abstract void DrawBorder(Graphics g, Pen pen);
}

// Dikdörtgen
public class RectangleShape : Shape
{
    public RectangleShape(Point center, int radius, Color color)
        : base(center, radius, color) { }

    public override void Draw(Graphics g)
    {
        Rectangle bounds = new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

        using (var brush = new SolidBrush(Color))
        {
            g.FillRectangle(brush, bounds);
        }
    }

    public override bool HitTest(Point point)
    {
        Rectangle bounds = new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);
        return bounds.Contains(point);
    }

    protected override void DrawBorder(Graphics g, Pen pen)
    {
        Rectangle bounds = new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);
        g.DrawRectangle(pen, bounds);
    }
}

// Daire
public class CircleShape : Shape
{
    public CircleShape(Point center, int radius, Color color)
        : base(center, radius, color) { }

    public override void Draw(Graphics g)
    {
        // Width = Height = Radius * 2
        Rectangle bounds = new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

        using (var brush = new SolidBrush(Color))
        {
            g.FillEllipse(brush, bounds);
        }
    }

    public override bool HitTest(Point point)
    {
        // Merkezden bir noktanın iki boyutlu uzaklığı.
        int dx = point.X - Center.X;
        int dy = point.Y - Center.Y;

        double distance = Math.Sqrt(dx * dx + dy * dy);

        return distance <= Radius;
    }

    protected override void DrawBorder(Graphics g, Pen pen)
    {
        Rectangle bounds = new(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);
        g.DrawEllipse(pen, bounds);
    }
}

// Üçgen
public class TriangleShape : Shape
{
    public TriangleShape(Point center, int radius, Color color)
        : base(center, radius, color) { }

    /// <summary>
    /// Üçgen şeklini çizer.
    /// </summary>
    /// <param name="g">Çizim için Graphics nesnesi.</param>
    public override void Draw(Graphics g)
    {
        Point[] points =
        [
            new(Center.X, Center.Y - Radius), // Üst merkez
            new(Center.X - Radius, Center.Y + Radius), // Sol alt köşe
            new(Center.X + Radius, Center.Y + Radius), // Sağ alt köşe
        ];

        using (var brush = new SolidBrush(Color))
        {
            g.FillPolygon(brush, points);
        }
    }

    public override bool HitTest(Point point)
    {
        Point[] points =
        [
            new(Center.X, Center.Y - Radius), // Üst merkez
            new(Center.X - Radius, Center.Y + Radius), // Sol alt köşe
            new(Center.X + Radius, Center.Y + Radius), // Sağ alt köşe
        ];

        return points.Contain(point);
    }

    protected override void DrawBorder(Graphics g, Pen pen)
    {
        Point[] points =
        [
            new(Center.X, Center.Y - Radius), // Üst merkez
            new(Center.X - Radius, Center.Y + Radius), // Sol alt köşe
            new(Center.X + Radius, Center.Y + Radius), // Sağ alt köşe
        ];
        g.DrawPolygon(pen, points);
    }
}

// Altıgen
public class HexagonShape : Shape
{
    public HexagonShape(Point center, int radius, Color color)
        : base(center, radius, color) { }

    public override void Draw(Graphics g)
    {
        Point[] points = new Point[6];
        for (int i = 0; i < 6; ++i)
        {
            double angle = i * Math.PI / 3; // 60 derecenin radyan karşılığı
            // Her nokta için merkezden 60 derece dön.
            points[i] = new Point(
                (int)(Center.X + Radius * Math.Cos(angle)),
                (int)(Center.Y + Radius * Math.Sin(angle))
            );
        }

        using (var brush = new SolidBrush(Color))
        {
            g.FillPolygon(brush, points);
        }
    }

    /// <summary>
    /// Altıgenin köşe noktalarını hesaplar.
    /// </summary>
    /// <returns> Altıgenin bütün köşelerini içeren bir dizi. </returns>
    private Point[] Points()
    {
        Point[] points = new Point[6];
        for (int i = 0; i < 6; ++i)
        {
            double angle = i * Math.PI / 3; // 60 derecenin radyan karşılığı
            points[i] = new Point(
                (int)(Center.X + Radius * Math.Cos(angle)),
                (int)(Center.Y + Radius * Math.Sin(angle))
            );
        }
        return points;
    }

    public override bool HitTest(Point point)
    {
        Point[] points = Points();

        return points.Contain(point);
    }

    protected override void DrawBorder(Graphics g, Pen pen)
    {
        Point[] points = Points();
        g.DrawPolygon(pen, points);
    }
}
