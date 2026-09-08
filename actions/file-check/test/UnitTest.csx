#load "./Validations/Test_AssemblyFileVersion.csx"
#load "./Validations/Test_AssemblyVersion.csx"
#load "./Validations/Test_ElTabelle.csx"
#load "./Validations/Test_Bin.csx"
#load "./Validations/Test_Obj.csx"

Test_AssemblyFileVersion.Exec();
Test_AssemblyVersion.Exec();
Test_ElTabelle.Exec();
Test_Bin.Exec();
Test_Obj.Exec();
