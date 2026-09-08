#load "validation/Test_AssemblyFileVersion.csx"
#load "validation/Test_AssemblyVersion.csx"
#load "validation/Test_ElTabelle.csx"
#load "validation/Test_Bin.csx"
#load "validation/Test_Obj.csx"

Test_AssemblyFileVersion.Exec();
Test_AssemblyVersion.Exec();
Test_ElTabelle.Exec();
Test_Bin.Exec();
Test_Obj.Exec();
