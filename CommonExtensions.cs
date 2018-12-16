using System;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using DBSqlite;

#region Attributes

public class TableFieldNameAttribute : Attribute
{
    public string FieldName { get; }

    internal TableFieldNameAttribute(string fieldName)
    {
        if (FieldName == null) FieldName = string.Empty;

        FieldName = fieldName;
    }
}

#endregion

#region Integer Extensions

public static class IntegerExtension
{
    /// <summary>
    /// Returns an empty string if the [TableFieldName] attribute isn't added to the property
    /// </summary>
    public static string TableField(this int value)
    {
        return value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<TableFieldNameAttribute>()
                        .FieldName;
    }

    public static bool IsEmpty(this int value) => value == SQLiteDBCommon.EmptyInt ? true : false;
}
#endregion

#region String Extensions

public static class StringExtension
{
    /// <summary>
    /// Returns an empty string if the [TableFieldName] attribute isn't added to the property
    /// </summary>
    public static string TableField(this string value)
    {
        return value.GetType()
                        .GetMember(value)
                        .First()
                        .GetCustomAttribute<TableFieldNameAttribute>()
                        .FieldName;
    }

    public static bool IsEmpty(this String value) => value == SQLiteDBCommon.EmptyString ? true : false;
}

#endregion

#region  Enumerations Extensions

public static class EnumerationExtension
{
    public static string Description(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        .GetDescription();
    }

    public static string TableFieldName(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<TableFieldNameAttribute>()
                        .FieldName;
    }
}

    #endregion