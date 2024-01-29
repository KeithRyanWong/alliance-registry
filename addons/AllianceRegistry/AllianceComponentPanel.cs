using System.Collections.Generic;
using Godot;

[Tool]
public partial class AllianceComponentPanel : PanelContainer
{

    public AllianceComponent AllianceComponent;
    private AllianceController AllianceController => GetNode<AllianceController>("/root/AllianceController");
    public Dictionary<string, int> CurrentOptions(OptionButton optionButton) {
        var options = new Dictionary<string, int>();
        for (int i = 0; i < optionButton.ItemCount; i++)
        { 
            options.Add(optionButton.GetItemText(i), optionButton.GetItemId(i));   
        }

        return options;
    }
    
    // Connected to option via editor, probably safer to do when making tools

    public void OnOptionButtonReady()
    {
        UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer/MarginContainer2/OptionButton"));
        AllianceController.AllyIdAdded += () => UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer/MarginContainer2/OptionButton"));
    }

    public void OnOptionButton2Ready()
    {
        UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer/OptionButton")); 
        AllianceController.AllyIdAdded += () => UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer/OptionButton")); 
    }

    public void OnOptionButton3Ready()
    {
        UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer2/OptionButton2"));   
        AllianceController.AllyIdAdded += () => UpdateOptions(GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer2/OptionButton2"));  
    }

    public void OnRegisterAlliance()
    {
        var requester =
            GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer/OptionButton");
        var ally = GetNode<OptionButton>("VBoxContainer/HBoxContainer2/VBoxContainer/MarginContainer2/OptionButton2");
        AllianceController.RegisterAlliance(requester.GetItemText(requester.Selected), ally.GetItemText(ally.Selected));
    }
    public void OnShowAlliances()
    {
        AllianceController.Debug();
    }
    
    public void UpdateOptions(OptionButton optionButton)
    {
        var options = CurrentOptions(optionButton);
        
        foreach (var allyId in AllianceController.AllyIds)
        {
            if (options.ContainsKey(allyId.Key)) continue;
            optionButton.AddItem(allyId.Key, allyId.Value);
        }

        options = CurrentOptions(optionButton);
        if (AllianceComponent is not null &&
            !string.IsNullOrWhiteSpace(AllianceComponent.AllyId) &&
            options.TryGetValue(AllianceComponent.AllyId, out var id)
           )
        {
            optionButton.Select(id);
        }
        
    }
    
    public void OnItemSelected(int idx)
    {
        var optionButton = GetNode<OptionButton>("VBoxContainer/HBoxContainer/MarginContainer2/OptionButton");
        AllianceComponent.AllyId = optionButton.GetItemText(idx);
    }


    public void OnAddAllyId()
    {
        var textEdit = GetNode<TextEdit>("VBoxContainer/HBoxContainer3/AllyIDTextEdit");
        if (string.IsNullOrWhiteSpace(textEdit.Text)) return;
        AllianceController.RegisterAllyId(textEdit.Text);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
