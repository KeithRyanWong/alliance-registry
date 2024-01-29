#if TOOLS
using Godot;

[Tool]
public partial class AllianceRegistryPlugin : EditorPlugin
{
	private const string AllianceComponent = "AllianceComponent";
	private const string AllianceController = "AllianceController";
	private AllianceComponentPlugin _allianceComponentPlugin;
	public override void _EnterTree()
	{
		// Initialize the registry singleton
		AddAutoloadSingleton(AllianceController, "res://addons/AllianceRegistry/AllianceController.tscn");
		// Register TeamId Node
		var script = GD.Load<Script>("res://addons/AllianceRegistry/AllianceComponent.cs");
		var texture = GD.Load<Texture2D>("res://addons/AllianceRegistry/AllianceComponent.svg");
		AddCustomType(AllianceComponent, "Node", script, texture);
		// Add Inspector Plugin
		_allianceComponentPlugin = new AllianceComponentPlugin();
		AddInspectorPlugin(_allianceComponentPlugin);
	}

	public override void _ExitTree()
	{
		
		// Remove registry singleton
		RemoveAutoloadSingleton(AllianceController);
		// Remove TeamIdNode
		RemoveCustomType(AllianceComponent);
		// Remove Inspector Plugin
		if (_allianceComponentPlugin != null)
		{
			RemoveInspectorPlugin(_allianceComponentPlugin);
		}
	}
}
#endif
