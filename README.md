# AtlassLASLib
A Lidar LAS Library for Managing Data and other Operations.



#Reading a LAS File (Example)

using (var readerobj = new TcLasReader(input))
{
  var points = readerobj.ReadPoints(readerobj.TotalPoints, readerobj.Header);
}



yup.
