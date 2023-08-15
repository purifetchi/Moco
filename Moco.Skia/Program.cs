using Moco;
using Moco.Skia.Backend;

var mocoBackend = new SkiaMocoBackend();
var moco = new MocoEngine(mocoBackend);
moco.LoadSwf(@"C:\Users\nano\Downloads\osaka-escalator.swf");
mocoBackend.Run();