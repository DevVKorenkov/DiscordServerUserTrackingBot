using System.DirectoryServices;

namespace DsBot.ActiveDirectorySync.Extentions;

public static class ActiveDirectoryExtentions
{
    public static int PropertyCount(this SearchResult sr, string propertyName) 
        => sr.Properties[propertyName].Count;
}