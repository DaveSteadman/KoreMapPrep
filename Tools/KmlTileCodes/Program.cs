using System;
using System.Text;
using System.IO;
using System.Linq;

class KmlGenerator
{
    private const string TileStyle = "TileLabelStyle";

    public void GenerateTileCodeKmlFile_Lvl0()
    {
        string filePath = "KmlLatLong_30x30TileCodes.kml";
        var kmlContent = new StringBuilder();

        WriteKmlHeader(kmlContent);

        string[] rows = "ABCDEF".ToCharArray().Select(c => c.ToString()).ToArray();  // 6 rows
        string[] columns = "ABCDEFGHIJKL".ToCharArray().Select(c => c.ToString()).ToArray();  // 12 columns

        int rowIndex = 0;
        for (int lat = 90; lat > -90; lat -= 30)  // From 90N to 90S (6 rows)
        {
            int colIndex = 0;
            for (int lon = -180; lon < 180; lon += 30)  // From 180W to 180E (12 columns)
            {
                string label = $"{rows[rowIndex]}{columns[colIndex]}";
                double centerLat = lat - 15;  // Center latitude
                double centerLon = lon + 15;  // Center longitude

                AddTileLabel(kmlContent, centerLat, centerLon, TileStyle, label);

                colIndex++;
            }
            rowIndex++;
        }

        WriteKmlFooter(kmlContent);
        File.WriteAllText(filePath, kmlContent.ToString());
    }

    public void GenerateTileCodeKmlFile_Lvl1()
    {
        string filePath = "KmlLatLong_5x5TileCodes.kml";
        var kmlContent = new StringBuilder();

        WriteKmlHeader(kmlContent);

        string[] majorRows = "ABCDEF".ToCharArray().Select(c => c.ToString()).ToArray();  // 6 rows
        string[] majorColumns = "ABCDEFGHIJKL".ToCharArray().Select(c => c.ToString()).ToArray();  // 12 columns

        string[] subRows = "ABCDEF".ToCharArray().Select(c => c.ToString()).ToArray();  // 6 rows within each tile
        string[] subColumns = "ABCDEF".ToCharArray().Select(c => c.ToString()).ToArray();  // 6 columns within each tile

        int majorRowIndex = 0;
        for (int lat = 90; lat > -90; lat -= 30)  // From 90N to 90S (6 rows)
        {
            int majorColIndex = 0;
            for (int lon = -180; lon < 180; lon += 30)  // From 180W to 180E (12 columns)
            {
                string majorLabel = $"{majorRows[majorRowIndex]}{majorColumns[majorColIndex]}";

                int subRowIndex = 0;
                for (int subLat = lat; subLat > lat - 30 && subRowIndex < subRows.Length; subLat -= 5)  // 6x6 subtiles in latitude
                {
                    int subColIndex = 0;
                    for (int subLon = lon; subLon < lon + 30 && subColIndex < subColumns.Length; subLon += 5)  // 6x6 subtiles in longitude
                    {
                        string subLabel = $"{subRows[subRowIndex]}{subColumns[subColIndex]}";
                        string label = $"{majorLabel}_{subLabel}";
                        double centerLat = subLat - 2.5;  // Center latitude for subtile
                        double centerLon = subLon + 2.5;  // Center longitude for subtile

                        AddTileLabel(kmlContent, centerLat, centerLon, TileStyle, label);

                        subColIndex++;
                    }
                    subRowIndex++;
                }

                majorColIndex++;
            }
            majorRowIndex++;
        }

        WriteKmlFooter(kmlContent);
        File.WriteAllText(filePath, kmlContent.ToString());
    }


    private void WriteKmlHeader(StringBuilder kmlContent)
    {
        kmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        kmlContent.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
        kmlContent.AppendLine("<Document>");
        kmlContent.AppendLine("<name>30x30 Degree Tile Codes</name>");
        AddStyles(kmlContent);
    }

    private void WriteKmlFooter(StringBuilder kmlContent)
    {
        kmlContent.AppendLine("</Document>");
        kmlContent.AppendLine("</kml>");
    }

    private void AddStyles(StringBuilder kmlContent)
    {
        kmlContent.AppendLine("<Style id=\"TileLabelStyle\"><LabelStyle><color>ff0000ff</color><scale>1.0</scale></LabelStyle></Style>");
    }

    private void AddTileLabel(StringBuilder kmlContent, double lat, double lon, string styleUrl, string label)
    {
        kmlContent.AppendLine("<Placemark>");
        kmlContent.AppendLine($"  <styleUrl>#{styleUrl}</styleUrl>");
        kmlContent.AppendLine($"  <name>{label}</name>");
        kmlContent.AppendLine("  <Point>");
        kmlContent.AppendLine("    <coordinates>");
        kmlContent.AppendLine($"      {lon},{lat},0");
        kmlContent.AppendLine("    </coordinates>");
        kmlContent.AppendLine("  </Point>");
        kmlContent.AppendLine("</Placemark>");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var generator = new KmlGenerator();
        generator.GenerateTileCodeKmlFile_Lvl0();
        generator.GenerateTileCodeKmlFile_Lvl1();
    }
}
