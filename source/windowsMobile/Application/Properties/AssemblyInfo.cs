using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// La información general sobre un ensamblado se controla mediante el siguiente
// conjunto de atributos. Cambie estos atributos para modificar la información
// asociada con un ensamblado.
[assembly: AssemblyTitle("High Level MANET Protocol")]
[assembly: AssemblyDescription("High Level MANET Protocol")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("High Level MANET Protocol")]
[assembly: AssemblyCopyright("© Juan Rodriguez-Covili 2009-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Si establece ComVisible como false hace que los tipos de este ensamblado no sean visibles
// para los componentes COM. Si necesita obtener acceso a un tipo de este ensamblado desde
// COM, establezca el atributo ComVisible como true en este tipo.
[assembly: ComVisible(false)]

// El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
[assembly: Guid("4b08b4d9-b774-4a7d-8180-8561e3dd1ab4")]

// La información de versión de un ensamblado consta de los cuatro valores siguientes:
//
//      Versión principal
//      Versión secundaria
//      Número de versión de compilación
//      Revisión
//
[assembly: AssemblyVersion("2.4.*")]

// El atributo siguiente es para suprimir la advertencia sobre FxCop "CA2232 : Microsoft.Usage : Agregue STAThreadAttribute al ensamblado"
// como aplicación para dispositivos no admite el subproceso STA.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2232:MarkWindowsFormsEntryPointsWithStaThread")]
