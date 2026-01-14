using CentralStationWebApi.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace CentralStationDemo;

public static class SvgConverter
{
    public static ImageSource ConvertSvg([StringSyntax("Uri")] string uri)
    {
        return RenderBitmap(XDocument.Load(uri));
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
            var root = doc.Root;
            if (root != null)
            {
                string viewbox = root.Attribute("viewBox")?.Value ?? string.Empty;
                viewBoxRect = viewbox.ViewBoxToRect();

                // background for testing
                drawingContext.DrawGeometry(new SolidColorBrush(Colors.Blue), new Pen(new SolidColorBrush(Colors.Yellow), 3), new RectangleGeometry(viewBoxRect));

                foreach (var element in root.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                    case "path":
                        DrawPath(drawingContext, element);
                        break;
                    case "circle":
                        DrawCircle(drawingContext, element);
                        break;
                    case "rect":
                        DrawRect(drawingContext, element);
                        break;
                    case "polygon":
                        DrawPolygon(drawingContext, element);
                        break;
                    }

                }

                //GeometryDrawing(null, TrackPen, new PathGeometry(

                //Drawing
            }


            //layer.Rails!.ForEach(r => r.DrawRailItem(drawingContext, RailViewMode.Terrain, layer));

        }
    
        RenderTargetBitmap bitmap = new((int)viewBoxRect.Width, (int)viewBoxRect.Height, 96, 96, PixelFormats.Default);
        bitmap.Render(drawingVisual);
        return bitmap;
    }

    private static Rect ViewBoxToRect(this string viewbox)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(viewbox);

        var values = viewbox.Split(' ').Select(i => int.Parse(i)).ToArray();
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, 4);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, 4);
        return new Rect(values[0], values[1], values[2], values[3]);
    }

    public static void DrawPath(DrawingContext drawingContext, XElement element)
    { }
    public static void DrawCircle(DrawingContext drawingContext, XElement element)
    { }
    public static void DrawRect(DrawingContext drawingContext, XElement element)
    { }
    public static void DrawPolygon(DrawingContext drawingContext, XElement element)
    { }
}
