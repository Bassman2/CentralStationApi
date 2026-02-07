using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Xml.Linq;

namespace CentralStationDemo.Misc;

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
            Debug.WriteLine($"Error loading SVG from {uri}: {ex.Message}");
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
            viewBoxRect = root.GetRectAttribute("viewBox");

            // background for testing
            //drawingContext.DrawGeometry(new SolidColorBrush(Colors.Blue), new Pen(new SolidColorBrush(Colors.Yellow), 3), new RectangleGeometry(viewBoxRect));

            DrawGroup(drawingContext, root, new StyleCache(), null, new SolidColorBrush(Colors.Black));
        }
        RenderTargetBitmap bitmap = new((int)viewBoxRect.Width, (int)viewBoxRect.Height, 96, 96, PixelFormats.Default);
        bitmap.Render(drawingVisual);
        return bitmap;
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

    private static string GetStringAttribute(this XElement element, string name)
    {
        return element.Attribute(name)?.Value ?? string.Empty;
    }

    //private static int GetIntAttribute(this XElement element, string name)
    //{
    //    string? value = element.Attribute(name)?.Value;
    //    return int.TryParse(value, out var val) ? val : 0;
    //}

    private static double GetDoubleAttribute(this XElement element, string name)
    {
        string? value = element.Attribute(name)?.Value;
        return double.TryParse(value, CultureInfo.InvariantCulture, out var val) ? val : 0.0;
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

    private static double[] GetDoubleArray(string value)
    {
        value = value.Trim('(', ')');
        var list = value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var items = list?.Select(i => double.Parse(i, CultureInfo.InvariantCulture)).ToArray() ?? [];
        return items;
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

    private static void DrawGroup(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke = null, Brush ? fill = null)
    {
        fill = element.GetFillAttribute(fill);
        stroke = element.GetStrokeAttribute(stroke);

        foreach (var elm in element.Elements())
        {
            switch (elm.Name.LocalName)
            {
            case "path":
                DrawPath(drawingContext, elm, styles, stroke, fill);
                break;
            case "circle":
                DrawCircle(drawingContext, elm, styles, stroke, fill);
                break;
            case "ellipse ":
                DrawEllipse(drawingContext, elm, styles, stroke, fill);
                break;
            case "rect":
                DrawRect(drawingContext, elm, styles, stroke, fill);
                break;
            case "line":
                DrawLine(drawingContext, elm, styles, stroke, fill);
                break;
            case "polyline":
                DrawPolyline(drawingContext, elm, styles, stroke, fill);
                break;
            case "text":
                DrawText(drawingContext, elm, styles, stroke, fill);
                break;
            case "image":
                DrawImage(drawingContext, elm, styles, stroke, fill);
                break;
            case "polygon":
                DrawPolygon(drawingContext, elm, styles, stroke, fill);
                break;
            case "g":
                DrawGroup(drawingContext, elm, styles, stroke, fill);
                break;
            case "style":
                HandleStyle(elm, styles);
                break;
            default:
                throw new InvalidCastException($"Unklnown element {elm.Name.LocalName}");
            }
        }
    }

    private static void HandleStyle(XElement element, StyleCache styleCache)
    {
        string styleType = element.GetStringAttribute("type");
        if (styleType != "text/css") throw new InvalidCastException($"Unknown style type {styleType}");
                

        // get text without linebreak;

        CssDocument cssDocument = ParseCss(element.Value);

        foreach (var rule in cssDocument.Rules)
        {
            var styleRule = new StyleRule(rule.Selector.Trim('.'));

            foreach (var declaration in rule.Declarations)
            {
                switch (declaration.Property)
                {
                case "fill":
                    styleRule.Fill = new SolidColorBrush(GetColor(declaration.Value));
                    break;
                default:
                    throw new InvalidCastException($"{declaration.Property}: {styleType}");
                }
            }
            styleCache.Styles.Add(styleRule.Name, styleRule);
        }
    }

    private class StyleCache
    {
        public Dictionary<string, StyleRule> Styles = [];

    }

    private class StyleRule(string name)    
    {
        public string Name => name;

        public Brush? Fill { get; set; } = null;

        public Pen? Stroke { get; set; } = null;
    }

    private static void HanleStyleAtributes(XElement element, StyleCache styleCache, ref Pen? stroke, ref Brush? fill, out Transform? transform)
    {
        var classAttr = element.Attribute("class");
        if (classAttr != null)
        {
            if(styleCache.Styles.TryGetValue(classAttr.Value ?? "", out var rule))
            {
                if (rule.Fill != null)
                {
                    fill = rule.Fill;
                }
                if (rule.Stroke != null)
                {
                    stroke = rule.Stroke;
                }
            }
        }

        var styleAttr = element.Attribute("style");
        if (styleAttr is not null)
        {
            string style = styleAttr.Value;
            var items = style.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            switch (items[0])
            {
            case "fill":
                fill = new SolidColorBrush(GetColor(items[1]));
                break;
            default:
                throw new InvalidCastException($"Unknown style attribute {items[0]}");
            }
        }

        var fillAttr = element.Attribute("fill");
        if (fillAttr != null)
        {
            fill = new SolidColorBrush(GetColor(fillAttr.Value));
        }

        var strokeAttr = element.Attribute("stroke");
        if (strokeAttr != null)
        {
            var strokeWidthAttr = element.Attribute("stroke-width");

            double width = 1.0;
            double.TryParse(strokeWidthAttr?.Value ?? "", CultureInfo.InvariantCulture, out width);

            stroke = new Pen(new SolidColorBrush(GetColor(strokeAttr.Value)), width);
        }

        var transformAttr = element.Attribute("transform");
        if (transformAttr != null)
        {
            string transf = transformAttr.Value;
            var items = transf.Split('(', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            switch (items[0])
            {
            case "matrix":
                var values = GetDoubleArray(items[1]);
                transform = new MatrixTransform(values[0], values[1], values[2], values[3], values[4], values[5]); 
                break;
            default:
                throw new InvalidCastException($"Unknown transform attribute {items[0]}");
            }
        }
        else
        {
            transform = null;
        }
    }

    private static void DrawPath(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        string d = element.Attribute("d")?.Value ?? string.Empty;

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawGeometry(fill, stroke, Geometry.Parse(d));
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    private static void DrawCircle(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);
        
        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double r = element.GetDoubleAttribute("r");

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), r, r);
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    private static void DrawEllipse(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double rx = element.GetDoubleAttribute("rx");
        double ry = element.GetDoubleAttribute("ry");

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), rx, ry);
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    private static void DrawRect(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        double x = element.GetDoubleAttribute("x");
        double y = element.GetDoubleAttribute("y");
        double width = element.GetDoubleAttribute("width");
        double height = element.GetDoubleAttribute("height");
        double rx = element.GetDoubleAttribute("rx");
        double ry = element.GetDoubleAttribute("ry");

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawRoundedRectangle(fill, stroke, new Rect(x, y, width, height), rx, ry);
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    private static void DrawLine(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        double x1 = element.GetDoubleAttribute("x1");
        double y1 = element.GetDoubleAttribute("y1");
        double x2 = element.GetDoubleAttribute("x2");
        double y2 = element.GetDoubleAttribute("y2");

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawLine(stroke, new Point(x1, y1), new Point(x2,y2));
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    private static void DrawPolyline(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        var points = element.GetPointsAttribute("points");

        
        
        var fig = new PathFigure(points[0], points.Skip(1).Select(p => new LineSegment(p, true)), false);
        PathGeometry pathGeometry = new PathGeometry([fig]);

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawGeometry(null, stroke, pathGeometry);
        if (transform is not null)
        {
            drawingContext.Pop();
        }

    }

    private static void DrawText(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);
        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawImage(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawPolygon(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill, out var transform);

        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

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

        if (transform is not null)
        {
            drawingContext.PushTransform(transform);
        }
        drawingContext.DrawGeometry(fill, stroke, pathGeometry);
        if (transform is not null)
        {
            drawingContext.Pop();
        }
    }

    #region CSS Parser

    private static string ToStringAndClear(this StringBuilder stringBuilder)
    {
        string str = stringBuilder.ToString();
        stringBuilder.Clear();
        return str;
    }

    private class CssDocument 
    {
        public List<CssRule> Rules { get; set; } = [];
    }

    [DebuggerDisplay("Selector: {Selector} (#{Declarations.Count})")]
    private class CssRule(string selector)
    {
        public string Selector => selector;
        public List<CssDeclaration> Declarations { get; set; } = [];
    }

    [DebuggerDisplay("Declaration: Property: {Property} Value: {Value}")]
    private class CssDeclaration(string property)
    {
        public string Property => property;
        public string Value { get; set; } = string.Empty;
    }

    private enum CssState { Selector, Property, Value }

    // rule = selector, '{', declaration-list, '}';
    // declaration-list = { declaration, ';' };
    // declaration = property, ':', value;

    private static CssDocument ParseCss(string text)
    {
        text = text.Replace("\r", "").Replace("\n", "").Replace(" ", "");

        var doc = new CssDocument();

        CssState state = CssState.Selector;

        CssRule? rule = null;
        CssDeclaration? declaration = null;
        StringBuilder builder = new();
                
        foreach (char c in text)
        {
            switch (c)
            {
            case '{':
                if (CssState.Selector == state)
                {
                    rule = new CssRule(builder.ToStringAndClear());
                    state = CssState.Property;
                }
                else throw new Exception();
                break;
            case ':':
                if (CssState.Property == state)
                {
                    declaration = new(builder.ToStringAndClear());
                    state = CssState.Value;
                }
                else throw new Exception();
                break;
            case '}':
                if (CssState.Property == state || CssState.Value == state)
                {
                    if (declaration != null)
                    {
                        declaration.Value = declaration!.Value = builder.ToStringAndClear();
                        rule!.Declarations.Add(declaration);
                        declaration = null;
                    }
                    doc.Rules.Add(rule!);
                    rule = null;
                    state = CssState.Selector;
                }
                else throw new Exception();
                break;
            case ';':
                if (CssState.Value == state)
                {
                    declaration!.Value = builder.ToStringAndClear();
                    rule!.Declarations.Add(declaration);
                    declaration = null;
                    state = CssState.Property;
                }
                else throw new Exception();
                break;
            default:
                builder.Append(c);
                break;
            }
        }
        return doc;
    }
    
    #endregion
}
