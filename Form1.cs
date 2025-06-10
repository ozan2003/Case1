using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Case1;

/// <summary>
/// Uygulama modları
/// </summary>
public enum InteractionMode
{
    Creating, // Şekil oluşturma
    Moving, // Şekil taşıma
    Deleting, // Şekil silme
}

/// Şekil türlerini temsil eden enum
public enum ShapeType
{
    Circle,
    Rectangle,
    Triangle,
    Hexagon,
}

public partial class Form1 : Form
{
    private readonly List<Shape> shapes; // Tüm şekilleri tutan liste

    // Seçili şekil tipi
    private ShapeType? selectedType = ShapeType.Rectangle; // Hiçbir şekil seçilmemişse dikdörtgen varsayılsın.
    private Color selectedColor = Color.Black;

    private InteractionMode currentMode = InteractionMode.Creating;

    private bool isDrawing = false;
    private bool isMoving = false;

    private Point startPoint;
    private Point currentPoint; // Önizleme için kullanılan nokta
    private Point lastMousePosition; // Son fare konumu, şekil taşıma için kullanılacak

    private Shape? previewShape; // Sürüklenen şekil önizlemesi
    private Shape? selectedShape; // Seçili şekil
    private Shape? movingShape; // Hareket ettirilen şekil

    private const int MinimumShapeRadius = 5;

    public Form1()
    {
        InitializeComponent();

        shapes = [];
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer,
            true
        );

        SetUpEventHandlers();
    }

    /// <summary>
    /// Arayüz bileşenleri için event handler'ları ayarlar.
    /// </summary>
    private void SetUpEventHandlers()
    {
        // Uygulama modları.
        btnCreating.Click += (s, e) => SetMode(InteractionMode.Creating);
        btnMoving.Click += (s, e) => SetMode(InteractionMode.Moving);
        btnDeleting.Click += (s, e) => SetMode(InteractionMode.Deleting);

        // Şekil seçici
        btnRectangle.Click += (s, e) => SetTool(ShapeType.Rectangle);
        btnCircle.Click += (s, e) => SetTool(ShapeType.Circle);
        btnTriangle.Click += (s, e) => SetTool(ShapeType.Triangle);
        btnHexagon.Click += (s, e) => SetTool(ShapeType.Hexagon);

        // Renkler
        btnBlack.Click += (s, e) => SetColor(Color.Black);
        btnRed.Click += (s, e) => SetColor(Color.Red);
        btnBlue.Click += (s, e) => SetColor(Color.Blue);
        btnGreen.Click += (s, e) => SetColor(Color.Green);
        btnYellow.Click += (s, e) => SetColor(Color.Yellow);

        // Dosya işlemleri
        btnSave.Click += BtnSave_Click;
        btnOpen.Click += BtnOpen_Click;

        btnClear.Click += BtnClear_Click;
    }

    /// <summary>
    /// Form'un Paint olayını işler. Tüm şekilleri ve önizleme şekillerini çizer.
    /// </summary>
    /// <param name="e"> Çizim olay bilgisi </param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw all shapes
        foreach (Shape shape in shapes)
        {
            if (shape == selectedShape)
            {
                shape.DrawSelected(g);
            }
            else
            {
                shape.Draw(g);
            }
        }

        // Önizlemeyi çiz.
        if (previewShape != null)
        {
            DrawPreview(g, previewShape);
        }
    }

    /// <summary>
    /// Önizleme şekli çizer. Bu, fare hareketi sırasında şekil oluşturma için kullanılır.
    /// </summary>
    /// <param name="g"> Çizim için Graphics nesnesi.</param>
    /// <param name="shape"> Önizleme şekli </param>
    private static void DrawPreview(Graphics g, Shape shape)
    {
        shape.DrawPreview(g);
    }

    /// <summary>
    /// Fare tıklaması ile şekil oluşturma, taşıma veya silme işlemlerini yönetir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            switch (currentMode)
            {
                case InteractionMode.Creating:
                    HandleCreatingMouseDown(e);
                    break;
                case InteractionMode.Moving:
                    HandleMovingMouseDown(e);
                    break;
                case InteractionMode.Deleting:
                    HandleDeletingMouseDown(e);
                    break;
            }
        }
    }

    /// <summary>
    /// Fare bırakma ile şekil oluşturma, taşıma veya silme işlemlerini yönetir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            switch (currentMode)
            {
                case InteractionMode.Creating:
                    HandleCreatingMouseUp(e);
                    break;
                case InteractionMode.Moving:
                    HandleMovingMouseUp(e);
                    break;
            }
        }
    }

    /// <summary>
    /// Fare hareketi ile şekil oluşturma, taşıma veya silme işlemlerini yönetir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        switch (currentMode)
        {
            case InteractionMode.Creating:
                HandleCreatingMouseMove(e);
                break;
            case InteractionMode.Moving:
                HandleMovingMouseMove(e);
                break;
            case InteractionMode.Deleting:
                HandleDeletingMouseMove(e);
                break;
        }
    }

    // Şekil oluşturma için event handler'lar
    /// <summary>
    /// Fare tıklamasıyla şekil oluşturma modunda başlangıç noktasını ayarlar.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleCreatingMouseDown(MouseEventArgs e)
    {
        isDrawing = true;
        startPoint = e.Location;
        currentPoint = e.Location;
        selectedShape = null; // Şekil seçimini sıfırla
    }

    /// <summary>
    /// Fare bırakma ile şekil oluşturma işlemini tamamlar.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleCreatingMouseUp(MouseEventArgs e)
    {
        if (isDrawing)
        {
            isDrawing = false;
            previewShape = null; // Önizleme şekli sıfırla
            Point center = startPoint;
            int radius = GetRadius(center, e.Location);

            if (radius > MinimumShapeRadius)
            {
                Shape newShape = CreateShape(selectedType, center, radius, selectedColor);
                shapes.Add(newShape);
            }
            Invalidate(); // Arayüzü güncelle
        }
    }

    /// <summary>
    /// Fare hareketi ile şekil oluşturma önizlemesini günceller.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleCreatingMouseMove(MouseEventArgs e)
    {
        if (isDrawing)
        {
            currentPoint = e.Location;
            Point center = startPoint;
            int radius = GetRadius(center, currentPoint);

            previewShape = CreateShape(selectedType, center, radius, selectedColor);
            Invalidate();
        }
    }

    // Şekil taşıma ve silme için event handler'lar
    /// <summary>
    /// Fare tıklamasıyla şekil taşıma modunda şekli seçer ve hareket ettirir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleMovingMouseDown(MouseEventArgs e)
    {
        // Farenin tıklandığı yerdeki şekli bul.
        Shape? hitShape = null;
        for (int i = shapes.Count - 1; i >= 0; --i)
        {
            if (shapes[i].HitTest(e.Location))
            {
                hitShape = shapes[i];
                break;
            }
        }

        if (hitShape != null)
        {
            selectedShape = hitShape;
            movingShape = hitShape;

            isMoving = true;
            lastMousePosition = e.Location;

            this.Cursor = Cursors.Hand;
        }
        else
        {
            selectedShape = null;
            movingShape = null;

            isMoving = false;

            this.Cursor = Cursors.Default;
        }
        Invalidate();
    }

    /// <summary>
    /// Fare bırakma ile şekil taşıma işlemini tamamlar.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleMovingMouseUp(MouseEventArgs _e)
    {
        if (isMoving)
        {
            isMoving = false;
            movingShape = null;
            this.Cursor = Cursors.Default;

            if (selectedShape != null)
            {
                // Seçilen şeklin rengini güncelle.
                selectedShape.Color = selectedColor;
            }
        }
    }

    /// <summary>
    /// Fare hareketi ile şekil taşıma önizlemesini günceller veya imleci değiştirir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleMovingMouseMove(MouseEventArgs e)
    {
        if (isMoving && movingShape != null)
        {
            // Ofset hesapla ve şekli taşı
            int deltaX = e.X - lastMousePosition.X;
            int deltaY = e.Y - lastMousePosition.Y;

            movingShape.Center = new Point(
                movingShape.Center.X + deltaX,
                movingShape.Center.Y + deltaY
            );

            lastMousePosition = e.Location;
            Invalidate();
        }
        else
        {
            // İmleci, şekil üzerinde olup olmadığına göre değiştir
            bool isOverShape = false;
            for (int i = shapes.Count - 1; i >= 0; --i)
            {
                if (shapes[i].HitTest(e.Location))
                {
                    isOverShape = true;
                    break;
                }
            }

            this.Cursor = isOverShape ? Cursors.Hand : Cursors.Default;
        }
    }

    // Şekil silme için event handler'lar
    /// <summary>
    /// Fare tıklamasıyla şekil silme modunda, tıklanan şekli siler.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleDeletingMouseDown(MouseEventArgs e)
    {
        // Farenin tıklandığı yerdeki şekli bul ve sil.
        for (int i = shapes.Count - 1; i >= 0; --i)
        {
            if (shapes[i].HitTest(e.Location))
            {
                shapes.RemoveAt(i);
                selectedShape = null;

                Invalidate();
                break;
            }
        }
    }

    /// <summary>
    /// Fare hareketi ile şekil silme modunda imleci değiştirir.
    /// </summary>
    /// <param name="e"> Fare olay bilgisi </param>
    private void HandleDeletingMouseMove(MouseEventArgs e)
    {
        // Silme modunda, imleci şekil üzerinde olup olmadığını kontrol et.
        bool overShape = false;
        for (int i = shapes.Count - 1; i >= 0; --i)
        {
            if (shapes[i].HitTest(e.Location))
            {
                overShape = true;
                break;
            }
        }

        this.Cursor = overShape ? Cursors.No : Cursors.Default;
    }

    /// <summary>
    /// Verilen merkez ve kenar noktası arasındaki yarıçapı hesaplar.
    /// </summary>
    /// <param name="center"> Şeklin merkezi </param>
    /// <param name="edge"> Şeklin kenar noktası </param>
    /// <returns></returns>
    private static int GetRadius(Point center, Point edge)
    {
        // Kenar noktasının merkezden iki boyutlu uzaklığı
        int deltaX = Math.Abs(edge.X - center.X);
        int deltaY = Math.Abs(edge.Y - center.Y);

        return Math.Max(deltaX, deltaY);
    }

    // Şekil oluşturma araçlarını ve renklerini ayarlamak için yardımcı metotlar
    /// <summary>
    /// Seçilen aracı ayarlar.
    /// </summary>
    /// <param name="tool"> Seçilen araç adı </param>
    public void SetTool(ShapeType type) => selectedType = type;

    /// <summary>
    /// Seçilen rengi ayarlar.
    /// </summary>
    /// <param name="color"> Seçilen renk </param>
    public void SetColor(Color color) => selectedColor = color;

    /// <summary>
    /// Etkileşim modunu ayarlar ve arayüzü günceller.
    /// </summary>
    /// <param name="mode"> Etkileşim modu </param>
    public void SetMode(InteractionMode mode)
    {
        currentMode = mode;
        ResetInteractionState();
        UpdateButtonAppearances(mode);
        UpdateButtonStates(mode);
        UpdateWindowTitle(mode);

        Invalidate();
    }

    /// <summary>
    /// Etkileşim durumunu sıfırlar.
    /// </summary>
    private void ResetInteractionState()
    {
        selectedShape = null;

        isDrawing = false;
        isMoving = false;

        previewShape = null;
        movingShape = null;

        this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Güncel etkileşim moduna göre butonların arka plan renklerini günceller.
    /// </summary>
    /// <param name="mode"> Uygulama modu </param>
    private void UpdateButtonAppearances(InteractionMode mode)
    {
        btnCreating.BackColor =
            mode == InteractionMode.Creating ? Color.LightBlue : SystemColors.Control;
        btnMoving.BackColor =
            mode == InteractionMode.Moving ? Color.LightGreen : SystemColors.Control;
        btnDeleting.BackColor =
            mode == InteractionMode.Deleting ? Color.LightCoral : SystemColors.Control;
    }

    /// <summary>
    /// Şekil oluşturma moduna göre butonların etkinlik durumlarını günceller.
    /// </summary>
    /// <param name="mode"> Uygulama modu </param>
    private void UpdateButtonStates(InteractionMode mode)
    {
        // Eğer oluşturma modu ise, şekil ve renk butonlarını etkinleştir
        bool shapeButtonsEnabled = mode == InteractionMode.Creating || mode == InteractionMode.Moving;
        btnRectangle.Enabled = shapeButtonsEnabled;
        btnCircle.Enabled = shapeButtonsEnabled;
        btnTriangle.Enabled = shapeButtonsEnabled;
        btnHexagon.Enabled = shapeButtonsEnabled;

        bool colorButtonsEnabled = shapeButtonsEnabled;
        btnBlack.Enabled = colorButtonsEnabled;
        btnRed.Enabled = colorButtonsEnabled;
        btnBlue.Enabled = colorButtonsEnabled;
        btnGreen.Enabled = colorButtonsEnabled;
        btnYellow.Enabled = colorButtonsEnabled;
    }

    /// <summary>
    /// Uygulama moduna göre pencere başlığını günceller.
    /// </summary>
    /// <param name="mode"> Uygulama modu </param>
    private void UpdateWindowTitle(InteractionMode mode)
    {
        this.Text = $"{mode} Mode";
    }

    /// <summary>
    /// Şekilleri JSON dosyasına kaydeder.
    /// </summary>
    /// <param name="sender"> Butonun tıklanma olayı. </param>
    /// <param name="e"> Buton tıklama olay bilgisi. </param>
    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        using (var saveDialog = new SaveFileDialog())
        {
            saveDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveDialog.DefaultExt = "json";
            saveDialog.AddExtension = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await SaveShapesToFile(saveDialog.FileName);
                    MessageBox.Show(
                        "Shapes saved successfully!",
                        "Save",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception exc)
                {
                    MessageBox.Show(
                        $"Error saving file: {exc.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }

    /// <summary>
    /// JSON dosyasından şekilleri yükler.
    /// </summary>
    /// <param name="sender"> Butonun tıklanma olayı. </param>
    /// <param name="e"> Buton tıklama olay bilgisi. </param>
    private async void BtnOpen_Click(object? sender, EventArgs e)
    {
        using (var openDialog = new OpenFileDialog())
        {
            openDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await LoadShapesFromFile(openDialog.FileName);
                    MessageBox.Show(
                        "Shapes loaded successfully!",
                        "Load",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    Invalidate(); // Refresh the display
                }
                catch (Exception exc)
                {
                    MessageBox.Show(
                        $"Error loading file: {exc.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }

    /// <summary>
    /// Şekilleri JSON dosyasına kaydeder.
    /// </summary>
    /// <param name="fileName"> JSON dosyasının adı. </param>
    private async Task SaveShapesToFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty");
        }

        List<ShapeData> shapeDatas = shapes.Select(shape => new ShapeData(shape)).ToList();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new PointJsonConverter(),
                new ColorJsonConverter(),
                new JsonStringEnumConverter() // Add this for enum serialization
                ,
            },
        };

        var json = JsonSerializer.Serialize(shapeDatas, options);
        await File.WriteAllTextAsync(fileName, json);
    }

    /// <summary>
    /// JSON dosyasından şekilleri yükler.
    /// </summary>
    /// <param name="fileName"> JSON dosyasının adı. </param>
    private async Task LoadShapesFromFile(string fileName)
    {
        string json = await File.ReadAllTextAsync(fileName);
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new PointJsonConverter(),
                new ColorJsonConverter(),
                new JsonStringEnumConverter(),
            },
        };

        List<ShapeData>? shapeDatas = JsonSerializer.Deserialize<List<ShapeData>>(json, options);

        if (shapeDatas != null)
        {
            shapes.Clear();
            foreach (ShapeData data in shapeDatas)
            {
                Shape shape = CreateShape(data.Type, data.Center, data.Radius, data.Color);
                shapes.Add(shape);
            }
        }
    }

    private void toolPanel_Paint(object sender, PaintEventArgs e)
    {
        // This method was auto-generated but can be left empty
    }

    /// <summary>
    /// Verilen şekil tipine göre yeni bir şekil oluşturur.
    /// </summary>
    /// <param name="type"> Şekil tipi </param>
    /// <param name="center"> Şeklin merkezi </param>
    /// <param name="radius"> Şeklin yarıçapı </param>
    /// <param name="color"> Şeklin rengi </param>
    /// <returns> Yeni oluşturulan şekil </returns>
    /// <exception cref="ArgumentNullException"> Verilen şekil tipi bilinmiyorsa fırlatılır. </exception>
    private static Shape CreateShape(ShapeType? type, Point center, int radius, Color color)
    {
        return type switch
        {
            ShapeType.Rectangle => new RectangleShape(center, radius, color),
            ShapeType.Circle => new CircleShape(center, radius, color),
            ShapeType.Triangle => new TriangleShape(center, radius, color),
            ShapeType.Hexagon => new HexagonShape(center, radius, color),
            null => throw new ArgumentNullException(nameof(type), "Shape type cannot be null"),
            _ => throw new ArgumentException($"Unknown shape type: {type}"),
        };
    }

    /// <summary>
    /// Buton tıklaması ile tüm şekilleri temizler.
    /// </summary>
    /// <param name="sender"> Butonun tıklanma olayı. </param>
    /// <param name="e"> Buton tıklama olay bilgisi. </param>
    private void BtnClear_Click(object? sender, EventArgs e)
    {
        shapes.Clear();
        Invalidate();
    }
}

// Shape sınıfının JSON'da temsili
public class ShapeData
{
    public ShapeType Type { get; set; }
    public Point Center { get; set; }
    public int Radius { get; set; }
    public Color Color { get; set; }

    public ShapeData() { } // Varsayılan

    public ShapeData(Shape shape)
    {
        Type = shape switch
        {
            RectangleShape => ShapeType.Rectangle,
            CircleShape => ShapeType.Circle,
            TriangleShape => ShapeType.Triangle,
            HexagonShape => ShapeType.Hexagon,
            _ => throw new ArgumentException($"Unknown shape type: {shape.GetType().Name}"),
        };
        Center = shape.Center;
        Radius = shape.Radius;
        Color = shape.Color;
    }
}

#region JSONConverters

// Point ve Color'u JSON'a dönüştürmek için özel JsonConverter sınıfları
public class PointJsonConverter : JsonConverter<Point>
{
    public override Point Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        try
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (
                    !root.TryGetProperty("X", out var xProp)
                    || !root.TryGetProperty("Y", out var yProp)
                )
                {
                    throw new JsonException("Missing X or Y property");
                }

                return new Point(xProp.GetInt32(), yProp.GetInt32());
            }
        }
        catch (Exception exc)
        {
            throw new JsonException($"Failed to deserialize Point: {exc.Message}", exc);
        }
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            return Color.FromArgb(
                root.GetProperty("A").GetInt32(),
                root.GetProperty("R").GetInt32(),
                root.GetProperty("G").GetInt32(),
                root.GetProperty("B").GetInt32()
            );
        }
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("A", value.A);
        writer.WriteNumber("R", value.R);
        writer.WriteNumber("G", value.G);
        writer.WriteNumber("B", value.B);
        writer.WriteEndObject();
    }
}

#endregion JSONConverters