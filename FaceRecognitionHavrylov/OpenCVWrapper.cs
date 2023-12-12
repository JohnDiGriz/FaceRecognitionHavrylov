using System;
using System.IO;
using System.Linq;
using OpenCvSharp;
using MoreLinq;

namespace FaceRecognitionHavrylov
{
    public class OpenCVWrapper
    {
        public static void Display(Mat im, Point2f[] bbox, string fileName = null)
        {
            for (int i = 0; i < bbox.Count(); i++)
            {
                var j = (i == bbox.Count() - 1 ? 0 : i + 1);
                im.Line(new OpenCvSharp.Point(bbox[i].X, bbox[i].Y), new OpenCvSharp.Point(bbox[j].X, bbox[j].Y), new Scalar(255, 0, 0), 3);
            }
            var resultName = fileName == null ? $"{Directory.GetCurrentDirectory()}/result.png" : $"{fileName}result.{fileName.Split('.').Last()}";
            im.ImWrite(resultName);
        }
        public static int RecognizeFaces(string inputImagePath)
        {
            var srcImage = new Mat(inputImagePath);

            var grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);
            var cascade = new CascadeClassifier(@"..\..\..\..\OpenCVHavrylov\Data\haarcascade_frontalface_alt.xml");
            var nestedCascade = new CascadeClassifier(@"..\..\..\..\OpenCVHavrylov\Data\haarcascade_eye_tree_eyeglasses.xml");
            var faces = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                minSize: new OpenCvSharp.Size(30, 30)
                );

            var count = 1;
            foreach (var faceRect in faces)
            {
                var detectedFaceImage = new Mat(srcImage, faceRect);

                var color = Scalar.FromRgb(255, 0, 0);
                Cv2.Rectangle(srcImage, faceRect, color, 3);


                var detectedFaceGrayImage = new Mat();
                Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversionCodes.BGRA2GRAY);
                var nestedObjects = nestedCascade.DetectMultiScale(
                    image: detectedFaceGrayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 2,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new OpenCvSharp.Size(30, 30)
                    );


                foreach (var nestedObject in nestedObjects)
                {
                    var center = new OpenCvSharp.Point
                    {
                        X = (int)(Math.Round(nestedObject.X + nestedObject.Width * 0.5, MidpointRounding.ToEven) + faceRect.Left),
                        Y = (int)(Math.Round(nestedObject.Y + nestedObject.Height * 0.5, MidpointRounding.ToEven) + faceRect.Top)
                    };
                    var radius = Math.Round((nestedObject.Width + nestedObject.Height) * 0.25, MidpointRounding.ToEven);
                    Cv2.Circle(srcImage, center, (int)radius, color, thickness: 3);
                }

                count++;
            }
            var resultName = inputImagePath == null ? $"{Directory.GetCurrentDirectory()}/result.png" : $"{inputImagePath}result.{inputImagePath.Split('.').Last()}";
            srcImage.ImWrite(resultName);
            srcImage.Dispose();
            return faces.Length;
        }


        private static void DrawBox(Mat image, Rect2d bbox)
        {
            Cv2.Rectangle(image, rect: new OpenCvSharp.Rect((int)bbox.X, (int)bbox.Y, (int)bbox.Width, (int)bbox.Height),
                new Scalar(255, 0, 255), 3, LineTypes.Link8);
            Cv2.PutText(image, "Tracking", new OpenCvSharp.Point(100, 75), HersheyFonts.HersheySimplex, 0.7, new Scalar(0, 255, 0), 2);
        }
    }
}
