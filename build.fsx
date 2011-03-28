#load @"lib\fake.fsx"

build("nutools.sln") 
&& merge_exe(@"bin\grep.exe", [@"bin\grep.exe";@"bin\common.dll"])
&& merge_exe(@"bin\sed.exe", [@"bin\sed.exe";@"bin\common.dll"])
&& package(@"bin\nutools.zip", [@"bin\grep.exe";@"bin\sed.exe"])
