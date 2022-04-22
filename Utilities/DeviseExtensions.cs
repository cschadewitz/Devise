using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Devise.Utilities
{


  //Idea taken from here - https://stackoverflow.com/questions/3502493/is-there-any-generic-parse-function-that-will-convert-a-string-to-any-type-usi
  public static class DeviseExtensions
  {
    public static T? ConvertToOrDefault<T>(this string value) where T : IConvertible
    {
      if (value is T variable) return variable;
      try
      {
        //Handling Nullable types i.e, int?, double?, bool? .. etc
        if (Nullable.GetUnderlyingType(typeof(T)) != null)
        {
          return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
        }
        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch (Exception)
      {
        return default;
      }
    }

    public static T ConvertTo<T>(this string value) where T : IConvertible
    {
      if (value is T variable) return variable;

      if (Nullable.GetUnderlyingType(typeof(T)) != null)
        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);

      return (T)Convert.ChangeType(value, typeof(T));
    }

    public static string TrimQuotes(this string name)
    {
      if (String.IsNullOrEmpty(name)) return string.Empty;
      return name.Trim(new char[] { '"' });
    }
    public static string Capitalize(this string name)
    {
      if (String.IsNullOrEmpty(name)) return string.Empty;
      if (name.Length == 1) return $"{char.ToUpper(name[0])}";
      return char.ToUpper(name[0]) + name.Substring(1);
    }
  }
}
