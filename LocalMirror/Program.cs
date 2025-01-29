using GenHTTP.Engine.Internal;

using LocalMirror;

var project = Project.Setup();

return await Host.Create()
                 .Handler(project)
                 .Console()
#if DEBUG
                 .Development()
#endif
                 .RunAsync();
