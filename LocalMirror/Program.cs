using GenHTTP.Engine.Internal;

using GenHTTP.Modules.Practices;

using LocalMirror;

var project = Project.Setup();

return await Host.Create()
                 .Handler(project)
                 .Defaults(compression: false)
                 .Console()
#if DEBUG
                 .Development()
#endif
                 .RunAsync();
