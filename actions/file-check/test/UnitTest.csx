#load "../Validation.csx"
#load "../Definitions.csx"

using static Definitions;

const string file = "./files/AssemblyInfo.vb";
static Version expectedVersion = new(1, 2, 3, 4);
Validation.AssemblyFileVersion(file, expectedVersion);
Console.WriteLine("Test passed");
