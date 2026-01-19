using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Word;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
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

            var styles = new StyleCache();
            DrawGroup(drawingContext, root, styles);
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

    private static string GetStringAttribute(this XElement element, string name)
    {
        return element.Attribute(name)?.Value ?? string.Empty;
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


        //string text = element.Value.Replace("\r", "").Replace("\n", "");

        //foreach (var line in text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        //{

        //    Match match = Regex.Match(line, @"\.(\w+){([^}]*)}", RegexOptions.Singleline);
        //    if (match.Success)
        //    {
        //        StyleRule style = new StyleRule();
        //        style.Name = match.Groups[1].Value;
        //        string value = match.Groups[2].Value;
        //        var values = value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //        foreach (var v in values)
        //        {
        //            var l = value.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //            string attr = l[0];
        //            string val = l[1];
        //            switch (attr)
        //            {
        //            case "fill":
        //                style.Fill = new SolidColorBrush(GetColor(val));
        //                break;
        //            default:
        //                throw new InvalidCastException();
        //            }

        //        }

        //        svgStyles.Styles.Add(style.Name, style);

        //    }
        //}

        //return svgStyles;

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

    private static void HanleStyleAtributes(XElement element, StyleCache styleCache, ref Pen? stroke, ref Brush? fill)
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
    }

    private static void DrawPath(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);

        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        string d = element.Attribute("d")?.Value ?? string.Empty;
        drawingContext.DrawGeometry(fill, stroke, Geometry.Parse(d));
    }

    private static void DrawCircle(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double r = element.GetDoubleAttribute("r");
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), r, r);
    }

    private static void DrawEllipse(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        double cx = element.GetDoubleAttribute("cx");
        double cy = element.GetDoubleAttribute("cy");
        double rx = element.GetDoubleAttribute("rx");
        double ry = element.GetDoubleAttribute("ry");
        drawingContext.DrawEllipse(fill, stroke, new Point(cx, cy), rx, ry);
    }

    private static void DrawRect(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        int x = element.GetIntAttribute("x");
        int y = element.GetIntAttribute("y");
        int width = element.GetIntAttribute("width");
        int height = element.GetIntAttribute("height");
        int rx = element.GetIntAttribute("rx");
        int ry = element.GetIntAttribute("ry");
        //drawingContext.DrawRectangle(fill, null, new Rect(x, y, width, height));
        drawingContext.DrawRoundedRectangle(fill, stroke, new Rect(x, y, width, height), rx, ry);
    }

    private static void DrawLine(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
        ////fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        int x1 = element.GetIntAttribute("x1");
        int y1 = element.GetIntAttribute("y1");
        int x2 = element.GetIntAttribute("x2");
        int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        drawingContext.DrawLine(stroke, new Point(x1, y1), new Point(x2,y2));
    }

    private static void DrawPolyline(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
        //fill = element.GetFillAttribute(fill);
        //stroke = element.GetStrokeAttribute(stroke);

        // TODO


        //int x1 = element.GetIntAttribute("x1");
        //int y1 = element.GetIntAttribute("y1");
        //int x2 = element.GetIntAttribute("x2");
        //int y2 = element.GetIntAttribute("y2");
        //int width = element.GetIntAttribute("width");
        //drawingContext.DrawLine(null, new Point(x1, y1), new Point(x2, y2));
        throw new NotImplementedException();
    }

    private static void DrawText(DrawingContext drawingContext, XElement element, StyleCache styles, Pen? stroke, Brush? fill)
    {
        HanleStyleAtributes(element, styles, ref stroke, ref fill);
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
        HanleStyleAtributes(element, styles, ref stroke, ref fill);

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
        HanleStyleAtributes(element, styles, ref stroke, ref fill);

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
                
        drawingContext.DrawGeometry(fill, stroke, pathGeometry);
    }

    #region CSS Parser

    private class CssDocument 
    {
        public List<CssRule> Rules { get; set; } = [];
    }

    private class CssRule
    {
        public string Selector { get; set; } = string.Empty;
        public List<CssDeclaration> Declarations { get; set; } = [];
    }

    private class CssDeclaration
    {
        public string Property { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    private enum CssState { Selector, Property, Value, BehindValue }

    // rule = selector, '{', declaration-list, '}';
    // declaration-list = { declaration, ';' };
    // declaration = property, ':', value;

    private static CssDocument ParseCss(string text)
    {
        text = text.Replace("\r", "").Replace("\n", "");

        var doc = new CssDocument();


        CssState state = CssState.Selector;

        CssRule rule = new();
        CssDeclaration declaration = new ();
        StringBuilder builder = new();

        foreach (char c in text)
        {
            switch (state)
            {
            case CssState.Selector:
                switch (c)
                {
                case '{':
                    rule.Selector = builder.ToString();
                    builder.Clear();
                    state = CssState.Property;
                    break;
                default:
                    builder.Append(c);
                    break;
                }
                break;
            case CssState.Property:
                switch (c)
                {
                case ':':
                    declaration.Property = builder.ToString();
                    builder.Clear();
                    state = CssState.Value;
                    break;
                default:
                    builder.Append(c);
                    break;
                }
                break;
            case CssState.Value:
                switch (c)
                {
                case ';':
                    declaration.Value = builder.ToString();
                    builder.Clear();
                    state = CssState.BehindValue;
                    rule.Declarations.Add(declaration);
                    declaration = new ();
                    break;
                default:
                    builder.Append(c);
                    break;
                }
                break;
            case CssState.BehindValue:
                switch (c)
                { 
                case '}':
                    // end of declaration list
                    break;
                default:
                    // next declaration in list
                    builder.Append(c);
                    state = CssState.Selector;
                    break;
                }
                break;
            }
        }
        return doc;
    }
    
    #endregion
}
