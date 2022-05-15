using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ScoutingAppBase
{
  /// <summary>
  /// A Behavior for numeric inputs that makes the input turn red when a non-numeric input is given.
  /// </summary>
  public class NumericValidationBehavior : Behavior<Entry>
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="min">The minimum value allowed for the input</param>
    /// <param name="max">The maximum value allowed for the input</param>
    /// <param name="isInt">Whether the input is an integer. False if it's a double</param>
    public NumericValidationBehavior(double min, double max, bool isInt)
    {
      Min = min;
      Max = max;
      IsInt = isInt;
    }

    public readonly double Min;
    public readonly double Max;
    public readonly bool IsInt;

    protected override void OnAttachedTo(Entry entry)
    {
      entry.TextChanged += OnEntryTextChanged;
      base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
      entry.TextChanged -= OnEntryTextChanged;
      base.OnDetachingFrom(entry);
    }

    void OnEntryTextChanged(object sender, TextChangedEventArgs args)
    {
      double input;
      bool parseable;
      if (IsInt)
      {
        parseable = int.TryParse(args.NewTextValue, out int parsedInt);
        input = parsedInt;
      }
      else
      {
        parseable = double.TryParse(args.NewTextValue, out input);
      }
      ((Entry)sender).TextColor = parseable && input >= Min && input <= Max ? Color.Default : Color.Red;
    }
  }
}
