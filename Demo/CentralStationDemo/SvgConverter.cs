using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace CentralStationDemo;

public static class SvgConverter
{
    public static ImageSource ConvertSvg([StringSyntax("Uri")] string uri)
    {
        return RenderBitmap(XDocument.Load(uri));
    }

    public static ImageSource ConvertSvg(Uri uri)
    {
        try
        {
            if (uri.AbsolutePath.Contains("magicon_a_064"))
            {

            }
            return RenderBitmap(XDocument.Load(uri.AbsoluteUri));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static ImageSource ConvertSvg(Stream stream)
    {   
        return RenderBitmap(XDocument.Load(stream));
    }
    
    private static ImageSource RenderBitmap(XDocument doc)
    {
        Rect viewBoxRect;
        DrawingVisual drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            var root = doc.Root!;
            viewBoxRect = GetRectAttribute(root, "viewBox");

            // background for testing
            //drawingContext.DrawGeometry(new SolidColorBrush(Colors.Blue), new Pen(new SolidColorBrush(Colors.Yellow), 3), new RectangleGeometry(viewBoxRect));

            DrawGroup(drawingContext, root);
        }
        RenderTargetBitmap bitmap = new((int)viewBoxRect.Width, (int)viewBoxRect.Height, 96, 96, PixelFormats.Default);
        bitmap.Render(drawingVisual);
        return (ImageSource)bitmap;
    }

    private static Rect GetRectAttribute(this XElement element, string name)
    {
        string value = element.Attribute(name)?.Value ?? "";
        var values = value.Split(' ').Select(i => int.Parse(i)).ToArray();
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, 4);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, 4);
        return new Rect(values[0], values[1], values[2], values[3]);
    }

    private static int GetIntAttribute(this XElement element, string name)
    {
        string? value = element.Attribute(name)?.Value;
        return int.TryParse(value, out var val) ? val : 0;
    }

    private static double GetDoubleAttribute(this XElement element, string name)
    {
        string? value = element.Attribute(name)?.Value;
        return double.TryParse(value, out var val) ? val : 0.0;
    }

    private static Color GetColor(string value)
    {
        if (value[0] == '#' && value.Length == 7)
        {
            var r = value.Substring(1, 2);
            var g = value.Substring(3, 2);
            var b = value.Substring(5, 2);
            var rv = byte.Parse(r, NumberStyles.HexNumber);
            var gv = byte.Parse(g, NumberStyles.HexNumber);
            var bv = byte.Parse(b, NumberStyles.HexNumber);
            return Color.FromRgb(rv, gv, bv);
        }
        else
        {
            Color color = (Color)ColorConverter.ConvertFromString(value);
            return color;
        }
    }

    private static Brush? GetFillAttribute(this XElement element, Brush? fill)
    {
        var attr = element.Attribute("fill");
        return attr is null ? fill : new SolidColorBrush(GetColor(attr.Value));
    }

    private static Pen? GetStrokeAttribute(this XElement element, Pen? stroke)
    {
        var attr = element.Attribute("stroke");
        double width = double.Parse(element.Attribute("stroke-width")?.Value ?? "1", CultureInfo.InvariantCulture);

        return attr is null ? stroke : new Pen(new SolidColorBrush(GetColor(attr.Value)), width);
    }

    private static Point[] GetPointsAttribute(this XElement element, string name)
    {
        string? value = element.Attribute(name)?.Value;
        return value?.Split(' ')?.Select(i => 
        { 
            var p = i.Split(','); 
            return new Point(double.Parse(p[0], CultureInfo.InvariantCulture), double.Parse(p[1], CultureInfo.InvariantCulture)); 
        }) .ToArray() ?? [];
    }

    private static void DrawGroup(DrawingContext drawingContext, XElement element, Brush? fill = null, Pen? stroke = null)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        foreach (var elm in element.Elements())
        {
            switch (elm.Name.LocalName)
            {
            case "path":
                DrawPath(drawingContext, elm, fill, stroke);
                break;
            case "circle":
                DrawCircle(drawingContext, elm, fill, stroke);
                break;
            case "ellipse ":
                DrawEllipse(drawingContext, elm, fill, stroke);
                break;
            case "rect":
                DrawRect(drawingContext, elm, fill, stroke);
                break;
            case "line":
                DrawLine(drawingContext, elm, fill, stroke);
                break;
            case "polyline":
                DrawPolyline(drawingContext, elm, fill, stroke);
                break;
            case "text":
                DrawText(drawingContext, elm, fill, stroke);
                break;
            case "image":
                DrawImage(drawingContext, elm, fill, stroke);
                break;
            case "polygon":
                DrawPolygon(drawingContext, elm, fill, stroke);
                break;
            case "g":
                DrawGroup(drawingContext, elm, fill, stroke);
                break;
            }
        }
    }

    private static void DrawPath(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        string d = element.Attribute("d")?.Value ?? string.Empty;
        drawingContext.DrawGeometry(fill, stroke, Geometry.Parse(d));
    }

    private static void DrawCircle(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double r = element.GetDoubleAttribute("r");
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), r, r);
    }

    private static void DrawEllipse(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double rx = element.GetDoubleAttribute("rx");
        double ry = element.GetDoubleAttribute("ry");
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), rx, ry);
    }

    private static void DrawRect(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        int x = element.GetIntAttribute("x");
        int y = element.GetIntAttribute("y");
        int width = element.GetIntAttribute("width");
        int height = element.GetIntAttribute("height");
        int rx = element.GetIntAttribute("rx");
        int ry = element.GetIntAttribute("ry");
        //drawingContext.DrawRectangle(fill, null, new Rect(x, y, width, height));
        drawingContext.DrawRoundedRectangle(fill, stroke, new Rect(x, y, width, height), rx, ry);
    }

    private static void DrawLine(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        //fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        int x1 = element.GetIntAttribute("x1");
        int y1 = element.GetIntAttribute("y1");
        int x2 = element.GetIntAttribute("x2");
        int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        drawingContext.DrawLine(stroke, new Point(x1, y1), new Point(x2,y2));
    }

    private static void DrawPolyline(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        //fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        // TODO


        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawText(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawImage(DrawingContext drawingContext, XElement element, Brush? fill, Pen? strokel)
    {
        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawPolygon(DrawingContext drawingContext, XElement element, Brush? fill, Pen? stroke)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        var points = element.GetPointsAttribute("points");
        
        PathGeometry pathGeometry = new PathGeometry();

        LineSegment lineSegment = new LineSegment();

        var fig = new PathFigure();
        fig.IsClosed = true;
        fig.StartPoint = points[0];

        foreach (var point in points.Skip(1))
        {
            fig.Segments.Add(new LineSegment(point, true));
        }
        
        pathGeometry.Figures.Add(fig);
                
        drawingContext.DrawGeometry(fill, stroke, pathGeometry);
    }

    

}
