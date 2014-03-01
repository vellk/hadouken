﻿using System.Linq;
using Hadouken.Configuration;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule(IPluginEngine pluginEngine, IPackageReader packageReader)
            : base("plugins")
        {
            this.RequiresAuthentication();

            Get["/"] = _ =>
            {
                var type = Request.Query.t;
                var message = Request.Query.msg;

                if (type == "error")
                    type = "danger";

                var plugins = pluginEngine.GetAll();
                var dto = (from plugin in plugins
                    select new PluginListItem
                    {
                        Name = plugin.Manifest.Name,
                        StateMessage = (plugin.State == PluginState.Error ? "Error: " + plugin.ErrorMessage : plugin.State.ToString()),
                        Version = plugin.Manifest.Version
                    }).ToList();

                return
                    View[
                        "Index",
                        new
                        {
                            HasPlugins = dto.Any(),
                            Plugins = dto,
                            HasAlert = !string.IsNullOrEmpty(message),
                            AlertMessage = message,
                            AlertClass = type
                        }];
            };

            Get["/details/{id}"] = _ =>
            {
                IPluginManager plugin = pluginEngine.Get(_.id);

                if (plugin == null)
                {
                    return 404;
                }

                var dto = new PluginDetailsItem
                {
                    Name = plugin.Manifest.Name,
                    Path = plugin.BaseDirectory.FullPath,
                    Version = plugin.Manifest.Version
                };

                return View["Details", dto];
            };

            Get["/uninstall/{id}"] = _ =>
            {
                IPluginManager plugin = pluginEngine.Get(_.id);

                if (plugin == null)
                {
                    return 404;
                }

                string[] deps;
                var canUninstall = pluginEngine.CanUnload(_.id, out deps);

                return View["Uninstall", new {CanUninstall = canUninstall, Dependencies = deps, PluginId = _.id}];
            };

            Post["/uninstall"] = _ =>
            {
                string id = Request.Form.id;
                IPluginManager plugin = pluginEngine.Get(id);

                if (plugin == null)
                {
                    return 404;
                }

                pluginEngine.Unload(id);
                pluginEngine.Uninstall(id);

                return 200;
            };

            Get["/upload"] = _ => View["Upload"];

            Post["/upload"] = _ =>
            {
                if (!Request.Files.Any())
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=no-package");
                }

                var postedFile = Request.Files.First();
                var package = packageReader.Read(postedFile.Value);

                if (package == null)
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=invalid-package");
                }

                pluginEngine.InstallOrUpgrade(package);

                return Response.AsRedirect("/manage/plugins?t=success&msg=package-uploaded");
            };
        }
    }
}
