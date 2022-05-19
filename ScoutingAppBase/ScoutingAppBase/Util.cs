using System;
using System.Collections.Generic;
using System.Text;

namespace ScoutingAppBase
{
  internal static class Util
  {
    public static R? Let<T, R>(this T? obj, Func<T, R> func) where T : class where R : class
    {
      if (obj == null)
      {
        return null;
      } else
      {
        return func(obj);
      }
    }
  }
}
