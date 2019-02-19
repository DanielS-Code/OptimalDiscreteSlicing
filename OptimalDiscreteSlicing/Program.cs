using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalDiscreteSlicing
{
    class Program
    {

        static bool test = true;

        public static Dictionary<Tuple<int, int>, Tuple<int, int>> optDiscreteSlicingAlgo(Dictionary<Tuple<int, int>, int> errorDic, HashSet<int> height, int N)
        {
            Dictionary<Tuple<int, int>, Tuple<int, int>> ETmy_t_err = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
            List<List<int>> E = new List<List<int>>();
            List<List<int>> phi = new List<List<int>>();
            int tMax = height.Max();
            int tMin = height.Min();
            for (int y = 1 - tMax; y <= 0; y++)
            {
                E[0][y] = 0;
            }
            for (int y = 1; y <= N + tMax - 1; y++)
            {
                for (int m = 1; m <= (int)Math.Round((double)((N - 2) / tMin)); m++)
                {
                    E[m][y] = int.MaxValue;//inf
                    foreach (int t in height)
                    {
                        if (E[m - 1][y - t] + E[y - t][y] < E[m][y])
                        {
                            phi[m][y] = t;
                            E[m][y] = E[m - 1][y - t] + E[y - t][y];
                        }

                    }
                }
            }
            for (int y = 1; y <= N + tMax - 1; y++)
            {
                for (int m = 1; m <= (int)Math.Round((double)((N - 2) / tMin)); m++)
                {
                    var tuple = new Tuple<int, int>(m, y);
                    var value = new Tuple<int, int>(E[m][y], phi[m][y]);
                    ETmy_t_err[tuple] = value;
                }
            }

            return ETmy_t_err;

        }
        public static Tuple<int, int> findStartPoint(Dictionary<Tuple<int, int>, Tuple<int, int>> ETmy_t_err, int N, int tMax, int tMin)
        {
            int y = N;
            int minErr = -1;
            bool firstIter = true;
            int yMin = -1;
            int mMin = -1;
            for (; y <= N + tMax - 1; y++)
            {
                for (int m = 1; m <= (int)Math.Round((double)((N - 2) / tMin)); m++)
                {
                    if (firstIter)
                    {
                        Tuple<int, int> tmp = new Tuple<int, int>(m, y);
                        if (ETmy_t_err[tmp] != null)//CHECK IT!!!! IF null or -1???
                        {
                            minErr = ETmy_t_err[tmp].Item2;
                            firstIter = false;
                            yMin = y;
                            mMin = m;

                        }

                    }
                    else
                    {
                        Tuple<int, int> tmp = new Tuple<int, int>(m, y);
                        if (minErr > ETmy_t_err[tmp].Item2)
                        {
                            minErr = ETmy_t_err[tmp].Item2;
                            yMin = y;
                            mMin = m;
                        }
                    }
                }
            }
            Tuple<int, int> retVal = new Tuple<int, int>(mMin, yMin);

            return retVal;
        }
        public static int getOptMperConstY(Dictionary<Tuple<int, int>, Tuple<int, int>> ETmy_t_err, int y, int tMin, int N)
        {
            int mOpt = -1;
            int minErr = -1;
            for (int m = 1; m <= (int)Math.Round((double)((N - 2) / tMin)); m++)
            {
                Tuple<int, int> tmp = new Tuple<int, int>(m, y);
                if (ETmy_t_err[tmp] != null)
                {//CHECK IT!!!! IF null or -1???
                    if (minErr > ETmy_t_err[tmp].Item2)
                    {
                        mOpt = m;
                    }
                }
            }
            return mOpt;
        }
        public static List<int> getOptSlice(Tuple<int, int> startPoint, Dictionary<Tuple<int, int>, Tuple<int, int>> ETmy_t_err, int tMin, int N)
        {
            List<int> path = new List<int>();
            Tuple<int, int> tmp = new Tuple<int, int>(startPoint.Item1, startPoint.Item2);
            int yNew = startPoint.Item2;
            int mNew = -1;
            Boolean first = true;
            path.Add(startPoint.Item2);


            while (yNew != 1 || first)//Check if stop on 1 or 0!
            {

                yNew = yNew - ETmy_t_err[tmp].Item2; //z-t
                mNew = getOptMperConstY(ETmy_t_err, yNew, tMin, N);
                path.Add(yNew);
                if (first) first = false; else first = false;
                tmp = new Tuple<int, int>(mNew, yNew);
            }
            return path;
        }



        public static List<int> getIntersections(int x, int z, Bitmap3 bmp)
        {
            List<int> interList = new List<int>();
            bool inShape = false;
            for (int y = 0; y < bmp.Dimensions.y; y++)
            {
                //we were out of shape and now going in shape
                if (bmp.Get(createVector(x, y, z)) == true && inShape == false)
                {
                    inShape = true;
                    interList.Add(y);
                }
                //we were in shape and now going out of shape
                if (bmp.Get(createVector(x, y, z)) == false && inShape == true)
                {
                    inShape = false;
                    interList.Add(y);
                }
            }
            return interList;
        }

        public static Tuple<Dictionary<Tuple<int, int>, int>, Dictionary<Tuple<int, int, int, int>, int>> calcErrorAndSum(Bitmap3 bmp, int tmax)
        {
            Dictionary<Tuple<int, int>, int> errorDic = new Dictionary<Tuple<int, int>, int>(); //key: zi,zj value: total error
            Dictionary<Tuple<int, int, int, int>, int> sumDic = new Dictionary<Tuple<int, int, int, int>, int>(); //key: zi,zj,x,z value: sum
            List<int> intersectionList;
            for (int x = 0; x < bmp.Dimensions.x; x++)
            {
                for (int z = 0; z < bmp.Dimensions.z; z++)
                {                                                       //run for each x and z (2D looking from top of object)
                    intersectionList = getIntersections(x, z, bmp);
                    for (int k = 0; k < intersectionList.Count; k++)
                    {
                        //check for null pointer exception 
                        int zi;
                        if (k > 0)
                        {
                            zi = Math.Max(intersectionList[k] - tmax, intersectionList[k - 1]);
                        }
                        else
                        {
                            zi = intersectionList[k] - tmax;
                        }
                        for (; zi <= intersectionList[k]; zi++)
                        { //check ranges!
                            for (int zj = intersectionList[k]; zj <= zi + tmax; zj++)
                            {
                                int s = (int)Math.Pow(-1, k) * (zi - intersectionList[k]);
                                int l = k + 1;
                                while (l<intersectionList.Count && intersectionList[l] < zj)
                                {
                                    s += (int)Math.Pow(-1, l) * (intersectionList[l - 1] - intersectionList[l]);
                                    l++;
                                }
                                s += (int)Math.Pow(-1, l) * (intersectionList[l - 1] - zj);
                                Tuple<int, int, int, int> sumKey = new Tuple<int,int,int,int>(zi,zj,x,z);
                                Tuple<int, int> key = new Tuple<int, int>(zi, zj);
                                sumDic.Add(sumKey, s);
                                int error = zj - zi - s;
                                if (errorDic.ContainsKey(key)) //check if need to add prev error
                                {
                                    errorDic[key] = errorDic[key] + error;
                                }
                                else
                                {
                                    errorDic[key] = error;
                                }
                            }
                        }
                    }
                }
            }
            return new Tuple<Dictionary<Tuple<int,int>,int>,Dictionary<Tuple<int,int,int,int>,int>>(errorDic,sumDic);
        }

        public static Vector3i createVector(int x, int y, int z)
        {
            return new Vector3i(x, y, z);
        }

        public static Bitmap3 createVoxelizedRepresentation(String objPath)
        {
            DMesh3 mesh = StandardMeshReader.ReadMesh(objPath);

            int num_cells = 128;
            double cell_size = mesh.CachedBounds.MaxDim / num_cells;

            MeshSignedDistanceGrid sdf = new MeshSignedDistanceGrid(mesh, cell_size);
            sdf.Compute();

            //** voxels**//
            Bitmap3 bmp = new Bitmap3(sdf.Dimensions);
            Console.WriteLine(bmp.Dimensions.x + " " + bmp.Dimensions.y + " " + bmp.Dimensions.z);
            foreach (Vector3i idx in bmp.Indices())
            {
                float f = sdf[idx.x, idx.y, idx.z];
                bmp.Set(idx, (f < 0) ? true : false);
                //for bunny only removes bottom
                if (idx.y < 8)
                {
                    bmp.Set(idx, false);
                }

                if (test) //take only one line from top
                {
                    if (idx.z != 50 || idx.x != 60)
                    {
                        bmp.Set(idx, false);
                    }
                    else
                    {
                        Console.WriteLine(bmp.Get(idx));
                    }
                }

            }
            return bmp;
        }

        public static void printVoxelizedRepresentation(Bitmap3 bmp, String outputPath)
        {
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

        public static Bitmap3 createNewObjectForPriniting( List<int> path ,Dictionary<Tuple<int, int, int, int>, int> sumDic,Vector3i newObjDim){
            Bitmap3 voxPrintResult = new Bitmap3(newObjDim);
            foreach (Vector3i idx in voxPrintResult.Indices()) //initialize object
            {
                voxPrintResult.Set(idx, false);
            }
            for (int i =0; i<path.Count-1; i++){ //-1 because we want to take every time the range Zj to Zi where Zj > Zi
                int zj = path[i];
                int zi = path[i+1];
                for (int x=0; x<voxPrintResult.Dimensions.x; x++){
                    for(int z=0; z<voxPrintResult.Dimensions.z; z++){
                        Tuple<int,int,int,int> key = new Tuple<int,int,int,int>(zi,zj,x,z);
                        if(sumDic.ContainsKey(key) && sumDic[key]>=0){
                             for(int y=zi; y<=zj; y++)
                             {
                                  voxPrintResult.Set(createVector(x,y,z),true);
                             }
                        }
                    }
                }
            }
            return voxPrintResult; 
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Insert T");
            HashSet<int> legitSliceHights = new HashSet<int>();
            legitSliceHights.Add(2);
            Bitmap3 bmp = createVoxelizedRepresentation("C:\\Users\\Daniel\\Desktop\\bunny.obj");
            printVoxelizedRepresentation(bmp, "C:\\Users\\Daniel\\Desktop\\inputVox.obj");
            if (test)
            {
                getIntersections(60, 50, bmp).ForEach(Console.WriteLine);
            }
            Tuple<Dictionary<Tuple<int, int>, int>, Dictionary<Tuple<int, int, int, int>, int>> errorAndSum = calcErrorAndSum(bmp,legitSliceHights.Max());
            Dictionary<Tuple<int, int>, Tuple<int, int>> algResults =  optDiscreteSlicingAlgo(errorAndSum.Item1,legitSliceHights,bmp.Dimensions.y);
            Tuple<int, int> startPoint = findStartPoint(algResults,bmp.Dimensions.y,legitSliceHights.Max(),legitSliceHights.Min());
            List<int> path = getOptSlice(startPoint,algResults,legitSliceHights.Min(),bmp.Dimensions.y); //frop top to bottom
            Vector3i newObjDim = createVector(bmp.Dimensions.x,path.First()-path.Last(),bmp.Dimensions.z);
            Bitmap3 outputObj = createNewObjectForPriniting(path,errorAndSum.Item2,newObjDim);
            printVoxelizedRepresentation(outputObj,"C:\\Users\\Daniel\\Desktop\\outputVox.obj");



            //optDiscreteSlicingAlgo(calcError(bmp),....)
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