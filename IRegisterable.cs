using Nanoray.PluginManager;
using Nickel;

namespace Sierra;

internal interface IRegisterable
{
	static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}