using System;
using System.Collections.Generic;
using System.Text;

namespace CentralStationWebApi.Internal;

internal class CSFile(string fileName, string file)
{
    public  string FileName { get; } = fileName;

    public string FileText { get; } = file;   
}
