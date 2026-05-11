using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

var ctx = new AssemblyLoadContext("test", true);
var asm = ctx.LoadFromAssemblyPath(@"d:\HireCoonectBackend\services\job-service\bin\Debug\net8.0\HireConnect.Shared.dll");
foreach(var t in asm.GetTypes()) {
    Console.WriteLine($"{t.FullName}");
    foreach(var p in t.GetProperties()) Console.WriteLine($"  {p.PropertyType.Name} {p.Name}");
}
