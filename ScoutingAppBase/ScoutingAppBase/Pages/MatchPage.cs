using System.Collections.Generic;
using Xamarin.Forms;
using ScoutingAppBase.Data;

namespace ScoutingAppBase.Pages
{
  public sealed class MatchPage : ContentPage
  {
    private readonly MatchData Match;
    private readonly DataManager DataManager;

    /// <summary>
    /// Whether this match was already synced when this page was opened
    /// </summary>
    private readonly bool StartedSynced;

    public MatchPage(MatchData match, List<FieldConfig> fieldConfigs, DataManager dataManager)
    {
      Match = match;
      DataManager = dataManager;
      StartedSynced = Match.Synced;
      var layout = new StackLayout();

      var backButton = new Button {Text = "Back"};
      backButton.Clicked += (sender, e) => Navigation.PopAsync();
      layout.Children.Add(backButton);

      foreach (var fieldConfig in fieldConfigs)
      {
        layout.Children.Add(FieldConfigToElement(fieldConfig));
      }

      Content = layout;
    }

    protected override void OnDisappearing()
    {
      if (StartedSynced && !Match.Synced)
      {
        DataManager.SendMatch(Match);
      }
    }

    private View FieldConfigToElement(FieldConfig config)
    {
      switch (config.Type)
      {
        case FieldType.Num:
          return NumElement(config);
        case FieldType.Bool:
          return BoolElement(config);
        case FieldType.Choice:
          return RadioElement(config);
        default:
          return TextElement(config);
      }
    }

    private View NumElement(FieldConfig fieldConfig)
    {
      var defaultVal = fieldConfig.Min;
      Match[fieldConfig] = defaultVal.ToString();

      var valueLabel = new Label {Text = defaultVal.ToString()};
      var stepper = new Stepper
      {
        Value = defaultVal,
        Minimum = fieldConfig.Min,
        Maximum = fieldConfig.Max,
        Increment = fieldConfig.Inc
      };
      stepper.ValueChanged += (_, e) =>
      {
        var newVal = e.NewValue.ToString();
        valueLabel.Text = newVal;
        Match[fieldConfig] = newVal;
        Match.Synced = false;
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children =
        {
          new Label {Text = fieldConfig.Name},
          valueLabel,
          stepper
        }
      };
    }

    private View BoolElement(FieldConfig fieldConfig)
    {
      Match.Fields[fieldConfig.Name] = "false";

      var checkbox = new CheckBox();
      checkbox.CheckedChanged += (_, e) =>
      {
        Match.Fields[fieldConfig.Name] = e.Value.ToString();
        Match.Synced = false;
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children =
        {
          new Label {Text = fieldConfig.Name},
          checkbox
        }
      };
    }

    private View RadioElement(FieldConfig fieldConfig)
    {
      var layout = new StackLayout
      {
        Orientation = StackOrientation.Vertical,
        Children = {new Label {Text = fieldConfig.Name}}
      };

      foreach (var choice in fieldConfig.Choices)
      {
        var isSelected = fieldConfig.DefaultChoice == choice;
        Match.Fields[fieldConfig.Name] = isSelected.ToString();

        var button = new RadioButton
        {
          Content = choice,
          IsChecked = isSelected,
        };
        button.CheckedChanged += (_, e) =>
        {
          Match.Fields[fieldConfig.Name] = e.Value.ToString();
          Match.Synced = false;
        };

        layout.Children.Add(button);
      }

      return layout;
    }

    private View TextElement(FieldConfig fieldConfig)
    {
      Match[fieldConfig] = "";

      var text = new Entry();
      text.TextChanged += (_, e) =>
      {
        Match[fieldConfig] = e.NewTextValue;
        Match.Synced = false;
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children =
        {
          new Label {Text = fieldConfig.Name},
          text
        }
      };
    }
  }
}