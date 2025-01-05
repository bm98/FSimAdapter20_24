using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MSFSAdapter20_24
{
  /// <summary>
  /// Decomposes types
  ///  used to find Type Deltas in the SimConnect modules
  /// </summary>
  public static class NetDecomposer
  {

    /// <summary>
    /// Decompose a Struct
    /// </summary>
    /// <typeparam name="T">A Type</typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string DecompObj( Type type, string tabs )
    {
      StringBuilder sb = new StringBuilder( );

      FieldInfo[] fieldInfo;
      string t = tabs;

      // predecessors
      if ((type.BaseType != null) && type.BaseType.IsClass) {
        sb.AppendLine( DecompObj( type.BaseType, t ) );
        t += "\t";
      }

      if (type.IsValueType) {
        sb.AppendLine( t + $"VTYPE: {type.Name}" );
      }
      else if (type.IsClass) {
        sb.AppendLine( t + $"CLASS: {type.Name}" );
      }

      // Get the type and fields of FieldInfoClass.
      fieldInfo = type.GetFields(/* BindingFlags.NonPublic |*/ BindingFlags.Instance | BindingFlags.Public );
      // loop all fields 
      for (int i = 0; i < fieldInfo.Length; i++) {
        var fi = fieldInfo[i];
        if (fi.Name == "Ident") {
          ;
        }
        string scString = "";
        if (fi.CustomAttributes.Count() > 0) {
          var maAttr = fi.CustomAttributes.FirstOrDefault( ca => ca.AttributeType.Name == "MarshalAsAttribute" );
          if (maAttr != null) {
            var siz = maAttr.NamedArguments.FirstOrDefault( na => na.MemberName == "SizeConst" );
            if (siz != null) {
              ;
              var val = siz.TypedValue;
              scString = $"  SizeConst={val.Value}";
            }
          }
        }
        sb.AppendLine( t + $"\t{fi.Name,40} \t{fi.FieldType.Name}{scString}" );
      }

      return sb.ToString( );
    }

    /// <summary>
    /// Decompose a Struct
    /// </summary>
    /// <typeparam name="T">A Type</typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string DecompEnum( Type t, string tabs )
    {
      StringBuilder sb = new StringBuilder( );

      FieldInfo[] fieldInfo;
      Type myType = t;

      sb.AppendLine( $"\tENUM: {myType.Name}" );

      // Get the type and fields of FieldInfoClass.
      fieldInfo = myType.GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public );
      // loop all fields 
      for (int i = 0; i < fieldInfo.Length; i++) {
        var fi = fieldInfo[i];
        sb.AppendLine( tabs + $"\t{fi.Name,16} \t{fi.FieldType.Name}" );
        var names = t.GetEnumNames( );
        var values = t.GetEnumValues( );
        for (int e = 0; e < values.Length; e++) {
          sb.AppendLine( tabs + $"\t\t{names[e],40} = {Enum.Format(t, values.GetValue( e ),"d")}" );
        }
      }

      return sb.ToString( );
    }

    /// <summary>
    /// Decompose an assembly
    /// </summary>
    /// <param name="assembly">An Assembly</param>
    /// <returns>A string</returns>
    public static string DecompAssembly( Assembly assembly )
    {
      StringBuilder sb = new StringBuilder( );

      sb.AppendLine( $"ASSEMBLY: {assembly.FullName}" );
      sb.AppendLine( $"\tLOC: {assembly.Location}" );
      sb.AppendLine( );

      var types = assembly.ExportedTypes;
      foreach (var type in types) {
        sb.AppendLine( $"NAME: {type.FullName}" );
        if (type.IsEnum) {
          sb.AppendLine( DecompEnum( type, "\t" ) );
        }
        else if (type.IsValueType) {
          // first level
          sb.AppendLine( DecompObj( type, "\t" ) );
        }
        if (type.IsClass) {
          // first level
          sb.AppendLine( DecompObj( type, "\t" ) );
        }
        else {
          ;
        }
      }

      return sb.ToString( );
    }

  }
}
