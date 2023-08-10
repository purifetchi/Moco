using Moco;
using Moco.Skia.Backend;

var mocoBackend = new SkiaMocoBackend();
var moco = new MocoEngine(mocoBackend);
moco.LoadSwf("C:\\Users\\nano\\Downloads\\z0r-de_806(1).swf");
mocoBackend.Run();