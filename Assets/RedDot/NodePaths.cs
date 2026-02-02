
using System.Collections.Generic;
public static class NodePaths
{
    public const string ALLRoot = "AllRoot";
    public const string Root = "AllRoot/Root";

    public const string ModelA = "AllRoot/Root/ModelA";
    public const string ModelA_Sub_1 = "AllRoot/Root/ModelA/ModelA_Sub_1";
    public const string ModelA_Sub_2 = "AllRoot/Root/ModelA/ModelA_Sub_2";

    public static readonly List<string> AllPaths = new List<string>
        {
            ALLRoot,
            Root,
            ModelA,
            ModelA_Sub_1,
            ModelA_Sub_2,
        };
}
