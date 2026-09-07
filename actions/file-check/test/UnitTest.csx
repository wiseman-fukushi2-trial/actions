#load "../Validation.csx"
#load "../Definitions.csx"

using static Definitions;

// 引数が空の場合は処理を終了する
if (Args is null || Args.Count < 1)
{
	throw new("required workspace");
}

// ルートディレクトリ
string workspace = Args[0];

const string file = $"{workspace}/files/AssemblyInfo.vb";
static Version expectedVersion = new(1, 2, 3, 4);
Validation.AssemblyFileVersion(file, expectedVersion);
Console.WriteLine("Test passed");
