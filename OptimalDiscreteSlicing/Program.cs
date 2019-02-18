using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace OptimalDiscreteSlicing
{
    class Program
    {


        public static List<int> getIntersections(int x, int z, Bitmap3 bmp)
        {
            List<int> interList = new List<int>();
            bool inShape = false;
            for (int y = 0; y < bmp.Dimensions.y; y++)
            {
                //we were out of shape and now going in shape
                if (bmp.Get(getCordinate(x, y, z)) == true && inShape == false)
                {
                    inShape = true;
                    interList.Add(y);
                }
                //we were in shape and now going out of shape
                if (bmp.Get(getCordinate(x, y, z)) == false && inShape == true)
                {
                    inShape = false; 
                    interList.Add(y);
                }   
            }
            return interList;
        }

        public static Dictionary<Tuple<int, int>, int> calcError(Bitmap3 bmp)
        {
            Dictionary<Tuple<int,int>,int> ErrorDic = new Dictionary<Tuple<int,int>,int>();
            List<int> intersectionList;
            for (int x=0; x<bmp.Dimensions.x; x++){
                  for (int z=0; z<bmp.DimensiogetIntersectionsns.z; z++){
                      intersectionList = (x,z,bmp);
                      calcErrorForFixedPosition(x,y,)
                        for (int y=0; y<bmp.Dimensions.y; y++){

                        }
                  }
            }
            return ErrorDic;
        }

        static Vector3i getCordinate(int x, int y, int z)
        {
            return new Vector3i(x, y, z);
        }

        static Bitmap3 createVoxelizedRepresentation(String objPath)
        {
            DMesh3 mesh = StandardMeshReader.ReadMesh(objPath);

            int num_cells = 128;
            double cell_size = mesh.CachedBounds.MaxDim / num_cells;

            MeshSignedDistanceGrid sdf = new MeshSignedDistanceGrid(mesh, cell_size);
            sdf.Compute();

            //** voxels**//
            Bitmap3 bmp = new Bitmap3(sdf.Dimensions);
            Console.WriteLine(bmp.Dimensions.x + " " + bmp.Dimensions.y +" "+ bmp.Dimensions.z);
            foreach (Vector3i idx in bmp.Indices())
            {
                float f = sdf[idx.x, idx.y, idx.z];
                bmp.Set(idx, (f < 0) ? true : false);
                //Console.WriteLine(idx);
                //for bunny only removes bottom
                if (idx.y < 8)
                {
                    //Vector3i v = new Vector3i();
                    //v.x = 4;
                    //v.y = 3;
                    //v.z = 4;
                    ////Console.WriteLine(bmp.Get(v));
                    bmp.Set(idx, false);
                }

            }
                return bmp;
        }

        static void printVoxelizedRepresentation(Bitmap3 bmp, String outputPath){
            VoxelSurfaceGenerator voxGen = new VoxelSurfaceGenerator();
            voxGen.Voxels = bmp;
            //voxGen.ColorSourceF = (idx) =>
            //{
            //    return new Colorf((float)idx.x, (float)idx.y, (float)idx.z) * (1.0f / 4);
            //};
            voxGen.Generate();
            DMesh3 voxMesh = voxGen.Meshes[0];
            Util.WriteDebugMesh(voxMesh, outputPath);
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Hello Daniel");
            Bitmap3 bmp = createVoxelizedRepresentation("C:\\Users\\Daniel\\Desktop\\bunny.obj");
            //optDiscreteSlicingAlgo(calcError(bmp),....)
            printVoxelizedRepresentation(bmp, "C:\\Users\\Daniel\\Desktop\\outputVox.obj");

            //** voxels**//


            //var iso = new DenseGridTrilinearImplicit(sdf.Grid, sdf.GridOrigin, sdf.CellSize);

            //MarchingCubes c = new MarchingCubes();
            //c.Implicit = iso;
            //c.Bounds = mesh.CachedBounds;

            //c.CubeSize = c.Bounds.MaxDim / 128;
            //c.Bounds.Expand(3 * c.CubeSize);
            //c.Generate();

            //DMesh3 outputMesh = c.Mesh;

            //StandardMeshWriter.WriteMesh("C:\\Users\\Daniel\\Desktop\\output.obj", c.Mesh, WriteOptions.Defaults);
        }
    }
}