using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ArcSoft_Face_Demo_X64
{
    public class ArcSoft_FACE_API
    {
        public  class FSDK_FACE_SHARE
        {
            public static string app_Id_X64     = "3yUbUj1c1YKeRuyW616PU8F3rDRU6UzazKPvGkEdoYkR";
            public static string sdk_Key_FD_X64 = "FshWfRcQPgrq9YGbmNRA6q7sp1wyGXVgfmwJacKZ1Vtn";
            public static string sdk_Key_FR_X64 = "FshWfRcQPgrq9YGbmNRA6q7zyRD81y9qz9FvhJDdiwdv";

            public static IntPtr detectEngine = IntPtr.Zero;
            public static IntPtr recogitionEngine = IntPtr.Zero;
                                
            //这是一个城市使用的案例
            public static int detectSize = 500 * 1024 * 1024;
            public static int recogitionSize = 500 * 1024 * 1024;
           
            public static int nScale = 16;
            public static int nMaxFaceNum = 10;


            //人脸矩形框信息
            public struct MRECT
            {
                public int left;                    //左边
                public int top;                     //上边
                public int right;                   //右边
                public int bottom;                  //下边
            }

            //定义图像格式空间
            public struct ASVLOFFSCREEN
            {
                public int u32PixelArrayFormat;     //像素阵列格式
                public int i32Width;                //宽度
                public int i32Height;               //高度
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.SysUInt)]
                public System.IntPtr[] ppu8Plane;   //平面
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
                public int[] pi32Pitch;             //倾斜
            }


            public static byte[] ReadBmpToByte(Bitmap image, ref int width, ref int height, ref int pitch)
            {//将Bitmap锁定到系统内存中,获得BitmapData
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;
                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];
                //将bitmap中的内容拷贝到ptr_bgr数组中
                Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength); width = data.Width;
                height = data.Height;
                pitch = Math.Abs(data.Stride);

                int line = width * 3;
                int bgr_len = line * height;
                byte[] destBitArray = new byte[bgr_len];
                for (int i = 0; i < height; ++i)
                {
                    Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
                }
                pitch = line;
                image.UnlockBits(data);
                return destBitArray;
            }

            public static Bitmap CutFace(Bitmap srcImage, int StartX, int StartY, int iWidth, int iHeight)
            {
                if (srcImage == null)
                {
                    return null;
                }
                int w = srcImage.Width;
                int h = srcImage.Height;
                if (StartX >= w || StartY >= h)
                {
                    return null;
                }
                if (StartX + iWidth > w)
                {
                    iWidth = w - StartX;
                }
                if (StartY + iHeight > h)
                {
                    iHeight = h - StartY;
                }
                try
                {
                    Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(bmpOut);
                    g.DrawImage(srcImage, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                    g.Dispose();
                    return bmpOut;
                }
                catch
                {
                    return null;
                }
            }

            public static Image DrawRectangleInPicture2(Image bmp, MRECT[] rectArr, Color RectColor, int LineWidth, DashStyle ds)
            {
                if (bmp == null) return null;
                Graphics g = Graphics.FromImage(bmp);
                Brush brush = new SolidBrush(RectColor);
                Pen pen = new Pen(brush, LineWidth);
                pen.DashStyle = ds;
                for (int i = 0; i < rectArr.Length; i++)
                {
                    Point p0 = new Point(rectArr[i].left, rectArr[i].top);
                    Point p1 = new Point(rectArr[i].right, rectArr[i].bottom);
                    g.DrawRectangle(pen, new Rectangle(p0.X, p0.Y, Math.Abs(p0.X - p1.X), Math.Abs(p0.Y - p1.Y)));

                }
                g.Dispose();
                return bmp;
            }

            /// <summary>
            /// 将一个字节数组转换为24位真彩色图
            /// </summary>
            /// <param name="imageArray">字节数组</param>
            /// <param name="width">图像的宽度</param>
            /// <param name="height">图像的高度</param>
            /// <returns>位图对象</returns>
            public static Bitmap ToGrayBitmap(byte[] imageArray, int width, int height)
            {

                //将用户指定的imageArray二维数组转换为一维数组rawValues
                byte[] rawValues = imageArray;
                //申请目标位图的变量，并将其内存区域锁定
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                //获得图像的参数
                int stride = bmpData.Stride; //扫描线的宽度

                // int offset = stride - width;  转换为8位灰度图时
                int offset = stride - width * 3; //显示宽度与扫描线宽度的间隙，
                                                 //与8位灰度图不同width*3很重要，因为此时一个像素占3字节

                IntPtr iptr = bmpData.Scan0; //获得 bmpData的内存起始位置
                int scanBytes = stride * height; //用Stride宽度,表示内存区域的大小

                //下面把原始的显示大小字节数组转换为内存中的实际存放的字节数组
                int posScan = 0, posReal = 0; //分别设置两个位置指针指向源数组和目标数组
                byte[] pixelValues = new byte[scanBytes]; //为目标数组分配内存

                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        //转换为8位灰度图时
                        //pixelValues[posScan++]=rawValues[posReal++];
                        //}
                        //posScan+=offset;
                        //此处也与8位灰度图不同，分别对R,G,B分量赋值,R=G=B
                        //posScan也由posScan++变为posScan+= 3;      

                        pixelValues[posScan] = pixelValues[posScan + 1] = pixelValues[posScan + 2] = rawValues[posReal++];
                        posScan += 3;
                    }
                    posScan += offset; //行扫描结束，要将目标位置指针移过那段间隙
                }



                //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中
                System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
                bmp.UnlockBits(bmpData); //解锁内存区域

                ////// ---------------------------------------------------------------------------

                ////下面的代码是8位灰度索引图时才需要的，是为了修改生成位图的索引表，从伪彩修改为灰度
                //ColorPalette tempPalette;
                //using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                //{
                //    tempPalette = tempBmp.Palette;
                //}
                //for (int i = 0; i < 256; i++)
                //{
                //    tempPalette.Entries[i] = Color.FromArgb(i, i, i);
                //}

                //bmp.Palette = tempPalette;
                //-----------------------------------------------------------------------------
                //// 算法到此结束，返回结果
                return bmp;
            }

            public static Bitmap ToGrayBitmap2(byte[] imageArray, int width, int height)
            {

                //将用户指定的imageArray二维数组转换为一维数组rawValues
                byte[] rawValues = imageArray;
                //申请目标位图的变量，并将其内存区域锁定
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                //获得图像的参数
                int stride = bmpData.Stride; //扫描线的宽度

                // int offset = stride - width;  转换为8位灰度图时
                int offset = stride - width * 3; //显示宽度与扫描线宽度的间隙，
                                                 //与8位灰度图不同width*3很重要，因为此时一个像素占3字节

                IntPtr iptr = bmpData.Scan0; //获得 bmpData的内存起始位置
                int scanBytes = stride * height; //用Stride宽度,表示内存区域的大小

                //下面把原始的显示大小字节数组转换为内存中的实际存放的字节数组
                int posScan = 0, posReal = 0; //分别设置两个位置指针指向源数组和目标数组
                byte[] pixelValues = new byte[scanBytes]; //为目标数组分配内存

                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        //pixelValues[posScan] = pixelValues[posScan + 1] = pixelValues[posScan + 2] = rawValues[posReal++];

                        pixelValues[posScan] = rawValues[posReal++];
                        pixelValues[posScan + 1] = rawValues[posReal++];
                        pixelValues[posScan + 2] = rawValues[posReal++];
                        posScan += 3;
                    }
                    posScan += offset; //行扫描结束，要将目标位置指针移过那段间隙
                }

                //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中
                System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
                bmp.UnlockBits(bmpData); //解锁内存区域

                return bmp;
            }


        }


        public class FSDK_FACE_DETECTION
        {
            //定义SDK版本信息--FD
            public struct AFD_FSDK_Version
            {
                public int lCodebase;               //代码库版本号
                public int lMajor;                  //主版本号
                public int lMinor;                  //次版本号
                public int lBuild;                  //编译版本号，递增
                public IntPtr Version;              //字符串形式的版本号
                public IntPtr BuildDate;            //编译时间
                public IntPtr CopyRight;            //copyright
            }

            //定义人脸检查结果中人脸的角度
            public enum AFD_FSDK_OrientCode
            {
                AFD_FSDK_FOC_0 = 1,
                AFD_FSDK_FOC_90 = 2,
                AFD_FSDK_FOC_270 = 3,
                AFD_FSDK_FOC_180 = 4,
                AFD_FSDK_FOC_30 = 5,
                AFD_FSDK_FOC_60 = 6,
                AFD_FSDK_FOC_120 = 7,
                AFD_FSDK_FOC_150 = 8,
                AFD_FSDK_FOC_210 = 9,
                AFD_FSDK_FOC_240 = 10,
                AFD_FSDK_FOC_300 = 11,
                AFD_FSDK_FOC_330 = 12
            }

            //定义脸部检查角度的优先级
            public enum AFD_FSDK_OrientPriority
            {
                AFD_FSDK_OPF_0_ONLY = 1,            //检测 0 度方向
                AFD_FSDK_OPF_90_ONLY = 2,           //检测 90 度方向
                AFD_FSDK_OPF_270_ONLY = 3,          //检测 270 度方向
                AFD_FSDK_OPF_180_ONLY = 4,          //检测 180 度方向
                AFD_FSDK_OPF_0_HIGHER_EXT = 5       //检测 0， 90， 180， 270 四个方向,0 度更优先
            }

            //检测到的脸部信息
            public struct AFD_FSDK_FACERES
            {
                public int nFace;                   //人脸个数
                public IntPtr rcFace;               //人脸矩形框信息 — MRECT
                public IntPtr lfaceOrient;          //人脸角度信息 — AFD_FSDK_OrientCode
            }

            /******************************************——ArcSoft Face Detection——******************************/
            /*
            *初始化脸部检测引擎
            *函数原形
                MRESULT AFD_FSDK_InitialFaceEngine(
                    MPChar AppId,
                    MPChar SDKKey,
                    MByte *pMem,
                    MInt32 lMemSize,
                    MHandle *pEngine,
                    AFD_FSDK_OrientPriority iOrientPriority,
                    MInt32 nScale,
                    MInt32 nMaxFaceNum
                );
                AppId [in] 				用户申请 SDK 时获取的 App Id
                SDKKey [in] 			用户申请 SDK 时获取的 SDK Key
                pMem [in] 				分配给引擎使用的内存地址
                lMemSize [in] 			分配给引擎使用的内存大小
                pEngine [out] 			引擎 handle
                iOrientPriority [in] 	期望的脸部检测角度的优先级
                nScale [in] 			用于数值表示的最小人脸尺寸 有效值范围[2,50] 推荐值 16
                nMaxFaceNum [in] 		用户期望引擎最多能检测出的人脸数 有效值范围[1,100]
            */
            [DllImport("libarcsoft_fsdk_face_detection.dll", EntryPoint = "AFD_FSDK_InitialFaceEngine", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFD_FSDK_InitialFaceEngine(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine, int iOrientPriority, int nScale, int nMaxFaceNum);


            /*
            *根据输入的图像检测出人脸位置，一般用于静态图像检测
            *函数原形
                MRESULT AFD_FSDK_StillImageFaceDetection(
                    MHandle hEngine,
                    LPASVLOFFSCREEN pImgData,
                    LPAFD_FSDK_FACERES pFaceRes
                );
                hEngine [in] 			引擎 handle
                pImgData [in] 			带检测图像信息
                pFaceRes [out] 			人脸检测结果			
            */
            [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFD_FSDK_StillImageFaceDetection(IntPtr pEngine, IntPtr pImgData, ref IntPtr pFaceRes);


            /*
            *获取 SDK 版本信息
            *函数原形
                const AFD_FSDK_Version * AFD_FSDK_GetVersion(
                    MHandle hEngine
                );
                hEngine [in] 			引擎 handle

            */
            [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr AFD_FSDK_GetVersion(IntPtr pEngine);


            /*
            *销毁引擎，释放相应资源
            *函数原形
                MRESULT AFD_FSDK_UninitialFaceEngine(
                    MHandle hEngine
                );
                hEngine [in] 			引擎 handle
            */
            [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFD_FSDK_UninitialFaceEngine(IntPtr pEngine);
            /*********************************************************************************************************/

        }


        public class FSDK_FACE_RECOGNITION
        {
            public struct AFR_FSDK_VERSION
            {
                public int lCodebase;               //代码库版本号
                public int lMajor;                  //主版本号
                public int lMinor;                  //次版本号
                public int lBuild;                  //编译版本号，递增
                public int lFeatureLevel;           //特征库版本号
                public IntPtr Version;              //字符串形式的版本号
                public IntPtr BuildDate;            //编译时间
                public IntPtr CopyRight;            //copyright
            }

            //脸部信息
            public struct AFR_FSDK_FACEINPUT
            {
                public FSDK_FACE_SHARE.MRECT rcFace;                //脸部矩形框信息  — MRECT
                public int lfaceOrient;                             //脸部旋转角度 — AFD_FSDK_OrientCode
            }


            //脸部特征信息
            public struct AFR_FSDK_FACEMODEL
            {
                public IntPtr pbFeature;            //提取到的脸部特征
                public int lFeatureSize;            //特征信息长度
            }

            public Int32 AFD_FSDK_OrientPriority;
            public Int32 AFD_FSDK_OrientCode;

            /*************************************——ArcSoft Face Recognition——***********************************/
            /*
            *初始化引擎
            *函数原形
                MRESULT AFR_FSDK_InitialEngine(
                    MPChar AppId,
                    MPChar SDKKey,
                    Mbyte *pMem,
                    MInt32 lMemSize,
                    MHandle *phEngine
                );
                Appid [in] 				用户申请 SDK 时获取的 id
                SDKKey [in] 			用户申请 SDK 时获取的 id
                pMem [in] 				分配给引擎使用的内存地址
                lMemSize [in] 			分配给引擎使用的内存大小
                phEngine [out] 			引擎 handle
            */
            [DllImport("libarcsoft_fsdk_face_recognition.dll", EntryPoint = "AFR_FSDK_InitialEngine", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFR_FSDK_InitialEngine(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine);

            /*
            *获取脸部特征
            *函数原形		
                MRESULT AFR_FSDK_ExtractFRFeature (
                    MHandle hEngine,
                    LPASVLOFFSCREEN pInputImage,
                    LPAFR_FSDK_FACEINPUT pFaceRes,
                    LPAFR_FSDK_FACEMODEL pFaceModels
                );
                hEngine [in] 			引擎 handle
                pInputImage [in] 		输入的图像数据
                pFaceRes [in] 			已检测到到的脸部信息
                pFaceModels [out] 		提取的脸部特征信息
            */
            [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFR_FSDK_ExtractFRFeature(IntPtr pEngine, IntPtr pImgData, IntPtr pFaceRes, IntPtr pFaceModels);


            /*
            *脸部特征比较
            *函数原形		
                MRESULT AFR_FSDK_FacePairMatching(
                    MHandle hEngine,
                    AFR_FSDK_FACEMODEL *reffeature,
                    AFR_FSDK_FACEMODEL *probefeature,
                    MFloat *pfSimilScore
                );
                hEngine [in] 			引擎 handle
                reffeature [in] 		已有脸部特征信息
                probefeature [in] 		被比较的脸部特征信
                pfSimilScore [out] 		相似程度数值		
            */
            [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFR_FSDK_FacePairMatching(IntPtr pEngine, IntPtr reffeature, IntPtr probefeature, ref float pfSimilScore);

            /*
            *结束引擎
            *函数原形		
                MRESULT AFR_FSDK_UninitialEngine(
                    MHandle hEngine
                );
                hEngine [in] 			引擎 handle
            */
            [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AFR_FSDK_UninitialEngine(IntPtr pEngine);

            /*
            *获取 SDK 版本信息
            *函数原形
                const AFR_FSDK_VERSION * AFR_FSDK_GetVersion(
                    MHandle hEngine
                );
                hEngine [in] 			引擎 handle
            */
            [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr AFR_FSDK_GetVersion(IntPtr pEngine);
            /*****************************************************************************************************/
        }

















        public class AFDFunction
        {







        }
    }
}
