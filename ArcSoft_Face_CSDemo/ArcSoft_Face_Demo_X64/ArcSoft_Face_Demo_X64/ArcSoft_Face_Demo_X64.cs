using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcSoft_Face_Demo_X64
{
    public partial class ArcSoft_Face_Demo_X64 : Form
    {
        private VideoCapture capture;
        bool runflag = false;
        bool pictureflag = false;
        byte[] pictureFeature;
        byte[] captureFeature;

        public ArcSoft_Face_Demo_X64()
        {
            InitializeComponent();
        }


        private void ImageGrabbedProcess(object sender, EventArgs arg)
        {
            Mat mat = new Mat();
            DateTime now = DateTime.Now;
            capture.Retrieve(mat);   // 捕获摄像头图片

            //if (index++ < 10)
            //{
            //    mat.Bitmap.Save("C:\\face"+index+".bmp");
            //}
            //else {
            //    Application.Exit();
            //}
            //return;
            #region 检查人脸
            Bitmap bitmap = mat.Bitmap;

            int width = 0;
            int height = 0;
            int pitch = 0;
            byte[] imageDataX = ArcSoft_FACE_API.FSDK_FACE_SHARE.ReadBmpToByte(bitmap, ref width, ref height, ref pitch);
            IntPtr imageDataPtrX = Marshal.AllocHGlobal(imageDataX.Length);
            Marshal.Copy(imageDataX, 0, imageDataPtrX, imageDataX.Length);

            ArcSoft_FACE_API.FSDK_FACE_SHARE.ASVLOFFSCREEN offInputX = new ArcSoft_FACE_API.FSDK_FACE_SHARE.ASVLOFFSCREEN();
            offInputX.u32PixelArrayFormat = 513;
            offInputX.ppu8Plane = new IntPtr[4];
            offInputX.ppu8Plane[0] = imageDataPtrX;
            offInputX.i32Width = width;
            offInputX.i32Height = height;
            offInputX.pi32Pitch = new int[4];
            offInputX.pi32Pitch[0] = pitch;
            IntPtr offInputPtrX = Marshal.AllocHGlobal(Marshal.SizeOf(offInputX));
            Marshal.StructureToPtr(offInputX, offInputPtrX, false);

            ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES faceResX = new ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES();
            IntPtr faceResPtrX = Marshal.AllocHGlobal(Marshal.SizeOf(faceResX));

            int detectResult = ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_StillImageFaceDetection(ArcSoft_FACE_API.FSDK_FACE_SHARE.detectEngine, offInputPtrX, ref faceResPtrX);
            if (detectResult == 0)
            {
                faceResX = (ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES)Marshal.PtrToStructure(faceResPtrX, typeof(ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES));
                #region 绘制人脸框
                ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT[] rectArr = new ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT[faceResX.nFace];
                long longPtrX = faceResX.rcFace.ToInt64();
                for (int i = 0; i < rectArr.Length; i++)
                {
                    IntPtr rectPtrX = new IntPtr(longPtrX);
                    var a = Marshal.PtrToStructure(rectPtrX, typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                    var b = (ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)a;
                    rectArr[i] = b;
                    longPtrX += Marshal.SizeOf(typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                }
                ArcSoft_FACE_API.FSDK_FACE_SHARE.DrawRectangleInPicture2(bitmap, rectArr, Color.Red, 2, DashStyle.Dash);

                if (pictureflag)
                {
                    captureFeature = detectAndExtractFeature(bitmap, pictureBox0);
                    float similar = 0;
                    similar = recogitionAndFacePairMatching(pictureFeature, captureFeature);
                    setControlText(label1, "相似度结果： " + similar.ToString());
                    pictureflag = false;
                    //pictureBox0.Image = null;
                    //pictureBox1.Image = null;
                    //pictureBox2.Image = null;
                    pictureFeature = null;
                    captureFeature = null;
                }
                #endregion
            }
            // 设置显示图片
            imageBox0.Image = mat;
            Marshal.FreeHGlobal(imageDataPtrX);
            #endregion
        }


        private void DetectAndRecogition_Init()
        {
            int retCode = 0;
            //初始化人脸检测引擎
            IntPtr pMemDetect = Marshal.AllocHGlobal(ArcSoft_FACE_API.FSDK_FACE_SHARE.detectSize);
            retCode = ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_InitialFaceEngine(
                ArcSoft_FACE_API.FSDK_FACE_SHARE.app_Id_X64,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.sdk_Key_FD_X64,
                pMemDetect,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.detectSize,
                ref ArcSoft_FACE_API.FSDK_FACE_SHARE.detectEngine,
                (int)ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_OrientPriority.AFD_FSDK_OPF_0_HIGHER_EXT,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.nScale,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.nMaxFaceNum
            );
            if (retCode != 0)
            {
                Console.WriteLine("人脸检测引擎初始化失败:错误码为:" + retCode);
                this.Close();
            }
            else
            {
                Console.WriteLine("人脸检测引擎初始化成功");
            }
            //初始化人脸识别引擎
            IntPtr pMemRecogition = Marshal.AllocHGlobal(ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionSize);
            retCode = ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_InitialEngine(
                ArcSoft_FACE_API.FSDK_FACE_SHARE.app_Id_X64,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.sdk_Key_FR_X64,
                pMemRecogition,
                ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionSize,
                ref ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionEngine
            );
            if (retCode != 0)
            {
                Console.WriteLine("人脸识别引擎初始化失败:错误码为:" + retCode);
                this.Close();
            }
            else
            {
                Console.WriteLine("人脸识别引擎初始化成功");
            }
        }



        //检测人脸、提取特征
        private byte[] detectAndExtractFeature(Image imageParam, PictureBox picbox)
        {
            Byte[] feature = null;
            try
            {
                Console.WriteLine();
                Console.WriteLine("############### Face Detect Start #########################");
                int width = 0;
                int height = 0;
                int pitch = 0;
                Bitmap bitmap = new Bitmap(imageParam);
                byte[] imageData = ArcSoft_FACE_API.FSDK_FACE_SHARE.ReadBmpToByte(bitmap, ref width, ref height, ref pitch);
                IntPtr imageDataPtr = Marshal.AllocHGlobal(imageData.Length);
                Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
                //Marshal.StructureToPtr(imageData, imageDataPtr, false);

                ArcSoft_FACE_API.FSDK_FACE_SHARE.ASVLOFFSCREEN offInput = new ArcSoft_FACE_API.FSDK_FACE_SHARE.ASVLOFFSCREEN();
                offInput.u32PixelArrayFormat = 513;
                offInput.ppu8Plane = new IntPtr[4];
                offInput.ppu8Plane[0] = imageDataPtr;
                offInput.i32Width = width;
                offInput.i32Height = height;
                offInput.pi32Pitch = new int[4];
                offInput.pi32Pitch[0] = pitch;
                IntPtr offInputPtr = Marshal.AllocHGlobal(Marshal.SizeOf(offInput));
                Marshal.StructureToPtr(offInput, offInputPtr, false);

                ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES faceRes = new ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES();
                IntPtr faceResPtr = Marshal.AllocHGlobal(Marshal.SizeOf(faceRes));

                Console.WriteLine("StartTime:{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
                Stopwatch watchTime = new Stopwatch();
                watchTime.Start();
                //人脸检测
                int detectResult = ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_StillImageFaceDetection(
                    ArcSoft_FACE_API.FSDK_FACE_SHARE.detectEngine,
                    offInputPtr,
                    ref faceResPtr
                );
                watchTime.Stop();
                Console.WriteLine(String.Format("检测耗时:{0}ms", watchTime.ElapsedMilliseconds));

                /*
                    #region 原始方法1
                    long longPtr = faceRes.rcFace.ToInt64();
                    for (int i = 0; i < rectArr.Length; i++)
                    {
                        IntPtr rectPtr = new IntPtr(longPtr);
                        var a = Marshal.PtrToStructure(rectPtr, typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                        var b = (ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)a;
                        rectArr[i] = b;
                        longPtr += Marshal.SizeOf(typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                    }
                    imageParam = ArcSoft_FACE_API.FSDK_FACE_SHARE.DrawRectangleInPicture2(imageParam, rectArr, Color.Red, 2, DashStyle.Dash);
                    if (rectArr.Length > 0)
                    {
                        ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT rect = rectArr[0];
                        Image imgResult = ArcSoft_FACE_API.FSDK_FACE_SHARE.CutFace((Bitmap)imageParam, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                        picbox.Image = imgResult;
                    }
                    else
                    {
                        picbox.Image = null;
                    }
                    #endregion
                */


                /*  
                  #region 原始方法2
                 faceRes = (ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES)Marshal.PtrToStructure(faceResPtr, typeof(ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES));
                  Console.WriteLine("  Face Count: " + faceRes.nFace);
                  for (int i = 0; i < faceRes.nFace; i++)
                  {
                      ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT rect = (ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)Marshal.PtrToStructure(faceRes.rcFace + Marshal.SizeOf(typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)) * i, typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                      int orient = (int)Marshal.PtrToStructure(faceRes.lfaceOrient + Marshal.SizeOf(typeof(int)) * i, typeof(int));
                      if (i == 0)
                      {
                          Image image = ArcSoft_FACE_API.FSDK_FACE_SHARE.CutFace(bitmap, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                          picbox.Image = image;
                      }
                      Console.WriteLine("left:" + rect.left + "\n" + "top:" + rect.top + "\n" + "right:" + rect.right + "\n" + "bottom:" + rect.bottom + "\n" + "orient:" + orient);

                  }
                  #endregion
                */

                #region 自己修改
                faceRes = (ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES)Marshal.PtrToStructure(faceResPtr, typeof(ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES));
                ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT[] rectArr = new ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT[faceRes.nFace];

                for (int j = 0; j < rectArr.Length; j++)
                {
                    rectArr[j] = (ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)Marshal.PtrToStructure(faceRes.rcFace + Marshal.SizeOf(typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)) * j, typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                }
                imageParam = ArcSoft_FACE_API.FSDK_FACE_SHARE.DrawRectangleInPicture2(imageParam, rectArr, Color.Red, 3, DashStyle.Dash);
                if (rectArr.Length > 0)
                {
                    for (int j = 0; j < rectArr.Length; j++)
                    {
                        ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT rect = rectArr[0];
                        Image imgResult = ArcSoft_FACE_API.FSDK_FACE_SHARE.CutFace((Bitmap)imageParam, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                        picbox.Image = imgResult;
                    }
                }
                else
                {
                    picbox.Image = null;
                }
                #endregion


                Console.WriteLine("  EndTime:{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
                Console.WriteLine("############### Face Detect End   #########################");

                if (faceRes.nFace > 0)
                {
                    Console.WriteLine("############### Face Recognition Start #########################");

                    ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEINPUT faceResult = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEINPUT();
                    int orient = (int)Marshal.PtrToStructure(faceRes.lfaceOrient, typeof(int));
                    faceResult.lfaceOrient = orient;
                    faceResult.rcFace = new ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT();
                    ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT rect = (ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT)Marshal.PtrToStructure(faceRes.rcFace, typeof(ArcSoft_FACE_API.FSDK_FACE_SHARE.MRECT));
                    faceResult.rcFace = rect;
                    IntPtr faceResultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(faceResult));
                    Marshal.StructureToPtr(faceResult, faceResultPtr, false);

                    ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL localFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();
                    IntPtr localFaceModelsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(localFaceModels));
                    //Marshal.StructureToPtr(localFaceModels, localFaceModelsPtr, false);

                    watchTime.Start();
                    int extractResult = ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_ExtractFRFeature(ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionEngine, offInputPtr, faceResultPtr, localFaceModelsPtr);
                    Marshal.FreeHGlobal(faceResultPtr);
                    Marshal.FreeHGlobal(offInputPtr);
                    watchTime.Stop();
                    Console.WriteLine(String.Format("抽取特征耗时:{0}ms", watchTime.ElapsedMilliseconds));

                    localFaceModels = (ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL)Marshal.PtrToStructure(localFaceModelsPtr, typeof(ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL));
                    Marshal.FreeHGlobal(localFaceModelsPtr);
                    Console.WriteLine("" + localFaceModels.lFeatureSize);

                    feature = new byte[localFaceModels.lFeatureSize];
                    Marshal.Copy(localFaceModels.pbFeature, feature, 0, localFaceModels.lFeatureSize);
                    localFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();

                    Console.WriteLine("############### Face Recognition End   #########################");
                }
                bitmap.Dispose();
                imageData = null;
                Marshal.FreeHGlobal(imageDataPtr);
                offInput = new ArcSoft_FACE_API.FSDK_FACE_SHARE.ASVLOFFSCREEN();
                faceRes = new ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_FACERES();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return feature;
        }

        //获取相似度
        private float recogitionAndFacePairMatching(byte[] firstFeature, byte[] secondFeature)
        {
            float similar = 0f;
            ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL firstFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();
            IntPtr firstFeaturePtr = Marshal.AllocHGlobal(firstFeature.Length);
            Marshal.Copy(firstFeature, 0, firstFeaturePtr, firstFeature.Length);
            firstFaceModels.lFeatureSize = firstFeature.Length;
            firstFaceModels.pbFeature = firstFeaturePtr;
            IntPtr firstPtr = Marshal.AllocHGlobal(Marshal.SizeOf(firstFaceModels));
            Marshal.StructureToPtr(firstFaceModels, firstPtr, false);


            ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL secondFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();
            IntPtr secondFeaturePtr = Marshal.AllocHGlobal(secondFeature.Length);
            Marshal.Copy(secondFeature, 0, secondFeaturePtr, secondFeature.Length);
            secondFaceModels.lFeatureSize = secondFeature.Length;
            secondFaceModels.pbFeature = secondFeaturePtr;
            IntPtr secondPtr = Marshal.AllocHGlobal(Marshal.SizeOf(secondFaceModels));
            Marshal.StructureToPtr(secondFaceModels, secondPtr, false);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int result = ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FacePairMatching(ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionEngine, firstPtr, secondPtr, ref similar);
            stopwatch.Stop();
            //Console.WriteLine("相似度:" + similar.ToString() + " 耗时:" + stopwatch.ElapsedMilliseconds.ToString() + "ms");

            firstFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();
            secondFaceModels = new ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_FACEMODEL();
            Marshal.FreeHGlobal(firstFeaturePtr);
            Marshal.FreeHGlobal(secondFeaturePtr);
            Marshal.FreeHGlobal(firstPtr);
            Marshal.FreeHGlobal(secondPtr);
            return similar;
        }


        //多线程设置控件的文本
        private void setControlText(Control control, string value)
        {
            control.Invoke(new Action<Control, string>((ct, v) => { ct.Text = v; }), new object[] { control, value });
        }




        private void ArcSoft_Face_Demo_X64_Load(object sender, EventArgs e)
        {
            DetectAndRecogition_Init();
            //检测摄像头是否安装          
            if (!runflag)
            {
                capture = new VideoCapture();
                capture.Start();
                if (!capture.IsOpened)
                {
                    Console.WriteLine("摄像头检测失败，请确认摄像头是否安装！");
                    this.Close();
                }
                capture.ImageGrabbed += new EventHandler(ImageGrabbedProcess);
                capture.Start();
                runflag = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.png|所有文件|*.*;";
            openFile.Multiselect = false;
            openFile.FileName = "";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                Image image = Image.FromFile(openFile.FileName);
                pictureBox1.Image = new Bitmap(image);
                image.Dispose();
                pictureFeature = detectAndExtractFeature(pictureBox1.Image, pictureBox2);
                if (pictureFeature.Length > 0)
                {
                    pictureflag = true;
                }
                else
                {
                    pictureflag = false;
                }
            }
        }



        private void ArcSoftFaceDemoX64_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Okay
            }
            else
            {
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            }
        }


        private void ArcSoftFaceDemoX64_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            /*
            capture.ImageGrabbed += null;
             capture.Stop();
            capture.Dispose();
            runflag = false;
            ArcSoft_FACE_API.FSDK_FACE_DETECTION.AFD_FSDK_UninitialFaceEngine(ArcSoft_FACE_API.FSDK_FACE_SHARE.detectEngine);
            ArcSoft_FACE_API.FSDK_FACE_RECOGNITION.AFR_FSDK_UninitialEngine(ArcSoft_FACE_API.FSDK_FACE_SHARE.recogitionEngine);
            */
        }


    }
}
