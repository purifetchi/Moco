using Moco;
using Moco.Skia.Backend;

var mocoBackend = new SkiaMocoBackend();
var moco = new MocoEngine(mocoBackend);
moco.LoadSwf(@"E:\brkngame\flashes\323.swf");
mocoBackend.Run();