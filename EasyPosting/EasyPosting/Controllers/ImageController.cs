using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Caching;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using EasyPosting.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class ImageController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        // GET: Image
        public ActionResult Index()
        {
            return View();
        }

        // GET: Image
        public ActionResult Represent()
        {
            return View();
        }

        // 이미지 업로드 시 실행 (fileinput)
        [ValidateAntiForgeryToken]
        public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files)
        {
            string[] pathArr = new string[10];
            int[] widthArr = new int[10];
            int[] heightArr = new int[10];
            int count = 0;
            string errorMessage = "";

            // 받은 데이터 검사
            if (files != null && files.Count() > 0)
            {
                foreach (var file in files)
                {
                    // 선택한 파일 검사
                    if (file != null && file.ContentLength > 0)
                    {
                        // 경로 지정
                        var folderName = "/Temp";
                        var serverPath = HttpContext.Server.MapPath(folderName);
                        if (Directory.Exists(serverPath) == false)
                        {
                            Directory.CreateDirectory(serverPath);
                        }

                        // 파일이름 지정
                        string _name = "";
                        string[] _name_split = file.FileName.Split('.');
                        for (int i = 0; i < _name_split.Length - 1; i++)
                        {
                            _name += _name_split[i];
                            if (i != _name_split.Length - 2) _name += '.';
                        }
                        _name = _name + Guid.NewGuid().ToString() + "1." + _name_split[_name_split.Length - 1];
                        var fileName = Path.GetFileName(_name);
                        var img = new WebImage(file.InputStream);
                        string fullFileName = Path.Combine(serverPath, fileName);

                        // 이미지 저장
                        if (System.IO.File.Exists(fullFileName)) System.IO.File.Delete(fullFileName);
                        img.Save(fullFileName);

                        fileName = Path.GetFileName(img.FileName);
                        var webPath = Path.Combine(folderName, fileName);
                        pathArr[count] = webPath.Replace("/", "\\");
                        widthArr[count] = img.Width;
                        heightArr[count] = img.Height;
                        count++;
                    }
                    else
                    {
                        errorMessage = "파일 중에 유효하지 않는 파일이 있습니다."; //failure
                        return Json(new { success = false, errorMessage = errorMessage });
                    }
                }
                return Json(new { success = true, fileName = pathArr, width = widthArr, height = heightArr, nums = count }); // success
            }
            errorMessage = "파일이 유효하지 않습니다."; //failure
            return Json(new { success = false, errorMessage = errorMessage });
        }

        // 스토리지 이미지 업로드
        [HttpPost]
        public ActionResult SaveStorageImage(string fileName)
        {
            try
            {
                var fn = Path.Combine(Server.MapPath("~/User/Posts/Image"), Path.GetFileName(fileName));
                var img = new WebImage(fn);

                string newFileName = "/Temp/" + Path.GetFileName(fn);
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }
                img.Save(newFileLocation, null, false);
                return Json(new { success = true, filePath = newFileName, width = img.Width, height = img.Height });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload clip file.\nERRORINFO: " + ex.Message });
            }
        }

        // 이미지 저장 메소드
        private string SaveTemporaryFile(HttpPostedFileBase file)
        {
            // Define destination
            var folderName = "/Temp";
            var serverPath = HttpContext.Server.MapPath(folderName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            //CleanUpTempFolder(1);

            return Path.Combine(folderName, fileName);
        }

        // 이미지 임시 저장
        private string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            string fullFileName = Path.Combine(serverPath, fileName);

            if (System.IO.File.Exists(fullFileName)) System.IO.File.Delete(fullFileName);
            img.Save(fullFileName);
            return Path.GetFileName(img.FileName);
        }

        // hoursOld만큼 시간이 지난 폴더 내 파일 제거
        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                DateTime fileCreationTime;
                DateTime currentUtcNow = DateTime.UtcNow;

                var serverPath = HttpContext.Server.MapPath("/Temp");
                if (Directory.Exists(serverPath))
                {
                    string[] fileEntries = Directory.GetFiles(serverPath);
                    foreach (var fileEntry in fileEntries)
                    {
                        fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                        var res = currentUtcNow - fileCreationTime;
                        if (res.TotalHours > hoursOld)
                        {
                            System.IO.File.Delete(fileEntry);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        // 파일명에 인덱스 추가
        private string AddIndexImagePath(string oldPath)
        {
            string _name = oldPath;
            int i = _name.Length - 1;
            while (_name[i] != '.') i--;
            string temp = _name.Substring(0, i - 1);
            int idx = (int)_name[i - 1] - 47;
            temp = temp + idx + _name.Substring(i, _name.Length - i);
            return temp;
        }

        // crop후 이미지 저장
        [HttpPost]
        public ActionResult SaveClipImage(string t, string l, string h, string w, string ah, string aw, string fileName)
        {
            double k;
            try
            {
                // Temp폴더에서 파일 가져오기
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));
                var img = new WebImage(fn);

                // 이미지 크기 비율 맞추기
                int height = (int)(Convert.ToInt32(h.Replace("-", "").Replace("px", "")));
                int width = (int)(Convert.ToInt32(w.Replace("-", "").Replace("px", "")));
                if (width > height)
                    k = (double)(img.Width / width);

                else
                    k = (double)(img.Height / height);

                if (k < 1) k = 1;
                width = (int)(k * width);
                height = (int)(k * height);
                int top = (int)(k * Convert.ToInt32(t.Replace("-", "").Replace("px", "")));
                int left = (int)(k * Convert.ToInt32(l.Replace("-", "").Replace("px", "")));
                int a_height = (int)(k * Convert.ToInt32(ah.Replace("-", "").Replace("px", "")));
                int a_width = (int)(k * Convert.ToInt32(aw.Replace("-", "").Replace("px", "")));

                // 이미지 resize
                img.Resize(width, height, false, false);

                // 이미지 crop
                img.Crop(top, left, height - top - a_height, width - left - a_width);

                // 현재 경로 이미지 제거
                // System.IO.File.Delete(fn);

                // 새로운 이미지 생성
                string newFileName = "/Temp/" + AddIndexImagePath(Path.GetFileName(fn));
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }
                img.Save(newFileLocation, null, false);
                return Json(new { success = true, filePath = newFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload clip file.\nERRORINFO: " + ex.Message });
            }
        }

        // 캔버스 이미지 저장
        [HttpPost]
        public ActionResult SaveByteArrayAsImage(string filename, string base64String)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                EP_POSTS EP_POST = new EP_POSTS();
                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }
                string newFileName = Guid.NewGuid().ToString();
                string newFilePath = "/User/Posts/Image/" + newFileName + ".png";
                string newFileLocation = HttpContext.Server.MapPath(newFilePath);

                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }
                image.Save(newFileLocation, System.Drawing.Imaging.ImageFormat.Png);
                TransferImageDB(newFileName, EP_POST);
                return Json(new { success = true, avatarFileLocation = newFilePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to save canvas.\nERRORINFO: " + ex.Message });
            }
        }

        private void TransferImageDB(string guid, [Bind(Include = "ID,GUID,UserID,Ref,Publish1,Publish1_time,Publish2,Publish2_time,Type,Mime_type,Title,Category,Slug,Date,Body")] EP_POSTS EP_POST)
        {
            EP_POST.GUID = guid;
            EP_POST.UserID = User.Identity.GetUserId();
            EP_POST.Title = guid + ".png";
            EP_POST.Body = guid + ".png";
            EP_POST.Mime_type = "Image/png";
            EP_POST.Date = DateTime.Now;
            EP_POST.Type = "Image";
            db.EP_POST.Add(EP_POST);
            db.SaveChanges();
        }
        //------------------------------  이미지 효과  -------------------------------------//
        // 모든 효과 섬네일 이미지 생성
        [HttpPost]
        public ActionResult SavethumnailImages(string fileName)
        {
            int nums = 8, size = 100;
            Image[] image = new Image[nums];
            try
            {
                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));
                var img = new WebImage(fn);
                img.Resize(size, size, false, false);
                byte[] imageByte = img.GetBytes();

                using (MemoryStream ms = new MemoryStream(imageByte))
                {
                    for (int i = 0; i < nums; i++)
                        image[i] = Image.FromStream(ms);
                }
                image[0] = (Image)SetBright(20, image[0]);
                image[1] = (Image)SetBright(-20, image[1]);
                image[2] = (Image)SetGrayscale(image[2]);
                image[3] = (Image)SetSmoothly(image[3]);
                image[4] = (Image)SetClearly(image[4]);
                image[5] = (Image)SetCartoonize(image[5]);
                image[6] = (Image)SetMirrorX(image[6]);
                image[7] = (Image)SetMirrorY(image[7]);

                // ... and save the new one.
                string[] newFileName = new string[nums];
                newFileName[0] = "/Temp/bright-" + Path.GetFileName(fn);
                newFileName[1] = "/Temp/dark-" + Path.GetFileName(fn);
                newFileName[2] = "/Temp/gray-" + Path.GetFileName(fn);
                newFileName[3] = "/Temp/smooth-" + Path.GetFileName(fn);
                newFileName[4] = "/Temp/clear-" + Path.GetFileName(fn);
                newFileName[5] = "/Temp/cartoon-" + Path.GetFileName(fn);
                newFileName[6] = "/Temp/mirrorx-" + Path.GetFileName(fn);
                newFileName[7] = "/Temp/mirrory-" + Path.GetFileName(fn);

                string[] newFileLocation = new string[nums];
                for (int i = 0; i < nums; i++)
                {
                    newFileLocation[i] = HttpContext.Server.MapPath(newFileName[i]);

                    if (Directory.Exists(Path.GetDirectoryName(newFileLocation[i])) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation[i]));
                    }
                    image[i].Save(newFileLocation[i]);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to effect file.\nERRORINFO: " + ex.Message });
            }
        }

        // 모든 섬네일 이미지 제거
        [HttpPost]
        public ActionResult DeletethumnailImages(string fileName)
        {
            int nums = 8;
            try
            {
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));

                // 섬네일 파일 경로 
                string[] thumnails = new string[nums];
                thumnails[0] = "/Temp/bright-" + Path.GetFileName(fn);
                thumnails[1] = "/Temp/dark-" + Path.GetFileName(fn);
                thumnails[2] = "/Temp/gray-" + Path.GetFileName(fn);
                thumnails[3] = "/Temp/smooth-" + Path.GetFileName(fn);
                thumnails[4] = "/Temp/clear-" + Path.GetFileName(fn);
                thumnails[5] = "/Temp/cartoon-" + Path.GetFileName(fn);
                thumnails[6] = "/Temp/mirrorx-" + Path.GetFileName(fn);
                thumnails[7] = "/Temp/mirrory-" + Path.GetFileName(fn);

                // 섬네일 이미지 제거
                for (int i = 0; i < nums; i++)
                {
                    fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(thumnails[i]));
                    System.IO.File.Delete(fn);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to effect file.\nERRORINFO: " + ex.Message });
            }
        }

        // 모든 프레임 섬네일 이미지 생성
        [HttpPost]
        public ActionResult SaveFramethumnailImages(string fileName)
        {
            int nums = 12, size = 100;
            Image[] image = new Image[nums];
            try
            {
                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));
                var img = new WebImage(fn);
                img.Resize(size, size, false, false);
                byte[] imageByte = img.GetBytes();

                using (MemoryStream ms = new MemoryStream(imageByte))
                {
                    for (int i = 0; i < nums; i++)
                        image[i] = Image.FromStream(ms);
                }
                for (int i = 0; i < 9; i++)
                {
                    image[i] = (Image)SetFrame(i + 1, image[i]);
                }

                for (int i = 9; i < 12; i++)
                {
                    image[i] = (Image)SetPolaroid(i - 8, image[i]);
                }
                // ... and save the new one.
                string[] newFileName = new string[nums];
                for (int i = 0; i < nums; i++)
                {
                    newFileName[i] = "/Temp/frame" + (int)(i + 1) + "-" + Path.GetFileName(fn);
                }

                string[] newFileLocation = new string[nums];
                for (int i = 0; i < nums; i++)
                {
                    newFileLocation[i] = HttpContext.Server.MapPath(newFileName[i]);

                    if (Directory.Exists(Path.GetDirectoryName(newFileLocation[i])) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation[i]));
                    }
                    image[i].Save(newFileLocation[i]);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to effect file.\nERRORINFO: " + ex.Message });
            }
        }

        // 모든 프레임 섬네일 이미지 제거
        [HttpPost]
        public ActionResult DeleteFramethumnailImages(string fileName)
        {
            int nums = 12;
            try
            {
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));

                // 섬네일 파일 경로 
                string[] thumnails = new string[nums];
                for (int i = 0; i < nums; i++)
                {
                    thumnails[i] = "/Temp/frame" + (int)(i + 1) + "-" + Path.GetFileName(fn);
                }

                // 섬네일 이미지 제거
                for (int i = 0; i < nums; i++)
                {
                    fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(thumnails[i]));
                    System.IO.File.Delete(fn);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to effect file.\nERRORINFO: " + ex.Message });
            }
        }


        // 효과 적용한 이미지 저장
        [HttpPost]
        public ActionResult SaveaffectedImage(string effect, string fileName)
        {
            int width, height, max = 700;
            Image image;
            try
            {
                // Temp폴더에서 파일 가져오기
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));
                var img = new WebImage(fn);

                width = (int)img.Width;
                height = (int)img.Height;
                if (width > 700 || height > 700)
                {
                    if (width > height)
                    {
                        height = (int)(max * ((double)height / width));
                        width = max;
                    }
                    else
                    {
                        width = (int)(max * ((double)width / height));
                        height = max;
                    }
                    img.Resize(width, height, false, false);
                }
                byte[] imageByte = img.GetBytes();

                using (MemoryStream ms = new MemoryStream(imageByte))
                {
                    image = Image.FromStream(ms);
                }
                switch (effect)
                {
                    case "brightly":
                        image = (Image)SetBright(20, image);
                        break;
                    case "darkly":
                        image = (Image)SetBright(-20, image);
                        break;
                    case "gray":
                        image = (Image)SetGrayscale(image);
                        break;
                    case "smoothly":
                        image = (Image)SetSmoothly(image);
                        break;
                    case "clearly":
                        image = (Image)SetClearly(image);
                        break;
                    case "frame1":
                        image = (Image)SetFrame(1, image);
                        break;
                    case "frame2":
                        image = (Image)SetFrame(2, image);
                        break;
                    case "frame3":
                        image = (Image)SetFrame(3, image);
                        break;
                    case "frame4":
                        image = (Image)SetFrame(4, image);
                        break;
                    case "frame5":
                        image = (Image)SetFrame(5, image);
                        break;
                    case "frame6":
                        image = (Image)SetFrame(6, image);
                        break;
                    case "frame7":
                        image = (Image)SetFrame(7, image);
                        break;
                    case "frame8":
                        image = (Image)SetFrame(8, image);
                        break;
                    case "frame9":
                        image = (Image)SetFrame(9, image);
                        break;
                    case "polaroid1":
                        image = (Image)SetPolaroid(1, image);
                        break;
                    case "polaroid2":
                        image = (Image)SetPolaroid(2, image);
                        break;
                    case "polaroid3":
                        image = (Image)SetPolaroid(3, image);
                        break;
                    case "cartoonize":
                        image = (Image)SetCartoonize(image);
                        break;
                    case "mirrorx":
                        image = (Image)SetMirrorX(image);
                        break;
                    case "mirrory":
                        image = (Image)SetMirrorY(image);
                        break;
                    default:
                        break;
                }

                // 새로운 이미지 생성
                string newFileName = "/Temp/" + AddIndexImagePath(Path.GetFileName(fn));
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }
                image.Save(newFileLocation);
                return Json(new { success = true, filePath = newFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to effect file.\nERRORINFO: " + ex.Message });
            }
        }

        //흑백
        private Bitmap SetGrayscale(Image currentImage)
        {
            Bitmap temp = new Bitmap(currentImage);
            Color c;
            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    c = temp.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    temp.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return temp;
        }

        //밝기조정
        private Bitmap SetBright(int brightness, Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    c = temp.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;
                    if (cR < 0) cR = 1; if (cR > 255) cR = 255;
                    if (cG < 0) cG = 1; if (cG > 255) cG = 255;
                    if (cB < 0) cB = 1; if (cB > 255) cB = 255;
                    temp.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            return temp;
        }

        //흐리게 
        private Bitmap SetSmoothly(Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            temp = ConvolutionFilter(temp, filter, 1.0 / 9.0, 0);
            return temp;
        }

        //프레임
        private Bitmap SetFrame(int idx, Image currentImage)
        {
            Bitmap temp = new Bitmap(currentImage);
            int k = idx % 3 == 0 ? 3 : idx % 3;
            int thickness = temp.Width / (30 - k * 5);
            using (Graphics g = Graphics.FromImage(temp))
            {
                if (idx < 4) g.DrawRectangle(new Pen(Brushes.White, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
                else if (idx < 7) g.DrawRectangle(new Pen(Brushes.Silver, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
                else g.DrawRectangle(new Pen(Brushes.Tan, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
            }
            return temp;
        }

        //폴라로이드
        private Bitmap SetPolaroid(int idx, Image currentImage)
        {
            Bitmap temp = new Bitmap(currentImage);
            int thickness;
            if (temp.Width < temp.Height) thickness = temp.Width / 20;
            else thickness = temp.Height / 20;
            using (Graphics g = Graphics.FromImage(temp))
            {
                if (idx == 1)
                {
                    g.DrawRectangle(new Pen(Brushes.White, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
                    g.DrawRectangle(new Pen(Brushes.White, thickness * 4),
                                    new Rectangle(0, temp.Height - thickness * 2, temp.Width, thickness * 6));
                }
                else if (idx == 2)
                {
                    g.DrawRectangle(new Pen(Brushes.Silver, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
                    g.DrawRectangle(new Pen(Brushes.Silver, thickness * 3),
                                    new Rectangle(0, temp.Height - thickness * 2, temp.Width, thickness * 6));
                }
                else
                {
                    g.DrawRectangle(new Pen(Brushes.Tan, thickness), new Rectangle(0, 0, temp.Width, temp.Height));
                    g.DrawRectangle(new Pen(Brushes.Tan, thickness * 3),
                                    new Rectangle(0, temp.Height - thickness * 2, temp.Width, thickness * 6));
                }
            }
            return temp;
        }

        // 좌우반전
        private Bitmap SetMirrorX(Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            Color c1, c2;

            for (int i = 0; i < temp.Width / 2; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    c1 = temp.GetPixel(i, j);
                    c2 = temp.GetPixel(temp.Width - 1 - i, j);
                    temp.SetPixel(i, j, Color.FromArgb((byte)c2.R, (byte)c2.G, (byte)c2.B));
                    temp.SetPixel(temp.Width - 1 - i, j, Color.FromArgb((byte)c1.R, (byte)c1.G, (byte)c1.B));
                }
            }
            return temp;
        }

        // 상하반전
        private Bitmap SetMirrorY(Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            Color c1, c2;

            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height / 2; j++)
                {
                    c1 = temp.GetPixel(i, j);
                    c2 = temp.GetPixel(i, temp.Height - 1 - j);
                    temp.SetPixel(i, j, Color.FromArgb((byte)c2.R, (byte)c2.G, (byte)c2.B));
                    temp.SetPixel(i, temp.Height - 1 - j, Color.FromArgb((byte)c1.R, (byte)c1.G, (byte)c1.B));
                }
            }
            return temp;
        }

        //카툰 이펙트
        private Bitmap SetCartoonize(Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            byte threshold = 200;
            temp = ConvolutionFilter(temp, filter, 1.0 / 9.0, 0);

            BitmapData sourceData =
                       temp.LockBits(new Rectangle(0, 0,
                       temp.Width, temp.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            temp.UnlockBits(sourceData);

            int byteOffset = 0;
            int blueGradient, greenGradient, redGradient = 0;
            double blue = 0, green = 0, red = 0;

            bool exceedsThreshold = false;

            for (int offsetY = 1; offsetY <
                 temp.Height - 1; offsetY++)
            {
                for (int offsetX = 1; offsetX <
                    temp.Width - 1; offsetX++)
                {
                    byteOffset = offsetY * sourceData.Stride +
                                 offsetX * 4;

                    blueGradient =
                    Math.Abs(pixelBuffer[byteOffset - 4] -
                    pixelBuffer[byteOffset + 4]);

                    blueGradient +=
                    Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                    pixelBuffer[byteOffset + sourceData.Stride]);

                    byteOffset++;

                    greenGradient =
                    Math.Abs(pixelBuffer[byteOffset - 4] -
                    pixelBuffer[byteOffset + 4]);

                    greenGradient +=
                    Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                    pixelBuffer[byteOffset + sourceData.Stride]);

                    byteOffset++;

                    redGradient =
                    Math.Abs(pixelBuffer[byteOffset - 4] -
                    pixelBuffer[byteOffset + 4]);

                    redGradient +=
                    Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                    pixelBuffer[byteOffset + sourceData.Stride]);

                    if (blueGradient + greenGradient + redGradient > threshold)
                    { exceedsThreshold = true; }
                    else
                    {
                        byteOffset -= 2;

                        blueGradient = Math.Abs(pixelBuffer[byteOffset - 4] -
                                                pixelBuffer[byteOffset + 4]);
                        byteOffset++;

                        greenGradient = Math.Abs(pixelBuffer[byteOffset - 4] -
                                                 pixelBuffer[byteOffset + 4]);
                        byteOffset++;

                        redGradient = Math.Abs(pixelBuffer[byteOffset - 4] -
                                               pixelBuffer[byteOffset + 4]);

                        if (blueGradient + greenGradient + redGradient > threshold)
                        { exceedsThreshold = true; }
                        else
                        {
                            byteOffset -= 2;

                            blueGradient =
                            Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                            pixelBuffer[byteOffset + sourceData.Stride]);

                            byteOffset++;

                            greenGradient =
                            Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                            pixelBuffer[byteOffset + sourceData.Stride]);

                            byteOffset++;

                            redGradient =
                            Math.Abs(pixelBuffer[byteOffset - sourceData.Stride] -
                            pixelBuffer[byteOffset + sourceData.Stride]);

                            if (blueGradient + greenGradient +
                                      redGradient > threshold)
                            { exceedsThreshold = true; }
                            else
                            {
                                byteOffset -= 2;

                                blueGradient =
                                Math.Abs(pixelBuffer[byteOffset - 4 - sourceData.Stride] -
                                pixelBuffer[byteOffset + 4 + sourceData.Stride]);

                                blueGradient +=
                                Math.Abs(pixelBuffer[byteOffset - sourceData.Stride + 4] -
                                pixelBuffer[byteOffset + sourceData.Stride - 4]);

                                byteOffset++;

                                greenGradient =
                                Math.Abs(pixelBuffer[byteOffset - 4 - sourceData.Stride] -
                                pixelBuffer[byteOffset + 4 + sourceData.Stride]);

                                greenGradient +=
                                Math.Abs(pixelBuffer[byteOffset - sourceData.Stride + 4] -
                                pixelBuffer[byteOffset + sourceData.Stride - 4]);

                                byteOffset++;

                                redGradient =
                                Math.Abs(pixelBuffer[byteOffset - 4 - sourceData.Stride] -
                                pixelBuffer[byteOffset + 4 + sourceData.Stride]);

                                redGradient +=
                                Math.Abs(pixelBuffer[byteOffset - sourceData.Stride + 4] -
                                pixelBuffer[byteOffset + sourceData.Stride - 4]);

                                if (blueGradient + greenGradient + redGradient > threshold)
                                { exceedsThreshold = true; }
                                else
                                { exceedsThreshold = false; }
                            }
                        }
                    }
                    byteOffset -= 2;
                    if (exceedsThreshold)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;
                    }
                    else
                    {
                        blue = pixelBuffer[byteOffset];
                        green = pixelBuffer[byteOffset + 1];
                        red = pixelBuffer[byteOffset + 2];
                    }
                    blue = (blue > 255 ? 255 :
                           (blue < 0 ? 0 :
                            blue));
                    green = (green > 255 ? 255 :
                            (green < 0 ? 0 :
                             green));
                    red = (red > 255 ? 255 :
                          (red < 0 ? 0 :
                           red));
                    resultBuffer[byteOffset] = (byte)blue;
                    resultBuffer[byteOffset + 1] = (byte)green;
                    resultBuffer[byteOffset + 2] = (byte)red;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }
            Bitmap resultBitmap = new Bitmap(temp.Width, temp.Height);
            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }

        // 선명하게
        private Bitmap SetClearly(Image currentImage)
        {
            Bitmap temp = (Bitmap)currentImage;
            float factor = 1.0f;

            Bitmap blurBitmap = ConvolutionFilter(temp, filter, 1.0 / 9.0);

            Bitmap resultBitmap = SubtractAddFactorImage(temp, blurBitmap, factor);

            return resultBitmap;
        }
        private static Bitmap SubtractAddFactorImage(Bitmap subtractFrom, Bitmap subtractValue, float factor = 1.0f)
        {
            BitmapData sourceData =
                       subtractFrom.LockBits(new Rectangle(0, 0,
                       subtractFrom.Width, subtractFrom.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] sourceBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0,
                                        sourceBuffer.Length);

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];


            BitmapData subtractData =
                       subtractValue.LockBits(new Rectangle(0, 0,
                       subtractValue.Width, subtractValue.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] subtractBuffer = new byte[subtractData.Stride *
                                             subtractData.Height];

            Marshal.Copy(subtractData.Scan0, subtractBuffer, 0,
                                         subtractBuffer.Length);

            subtractFrom.UnlockBits(sourceData);
            subtractValue.UnlockBits(subtractData);

            double blue = 0;
            double green = 0;
            double red = 0;

            for (int k = 0; k < resultBuffer.Length &&
                           k < subtractBuffer.Length; k += 4)
            {
                blue = sourceBuffer[k] +
                      (sourceBuffer[k] -
                       subtractBuffer[k]) * factor;

                green = sourceBuffer[k + 1] +
                       (sourceBuffer[k + 1] -
                        subtractBuffer[k + 1]) * factor;

                red = sourceBuffer[k + 2] +
                     (sourceBuffer[k + 2] -
                      subtractBuffer[k + 2]) * factor;


                blue = (blue < 0 ? 0 : (blue > 255 ? 255 : blue));
                green = (green < 0 ? 0 : (green > 255 ? 255 : green));
                red = (red < 0 ? 0 : (red > 255 ? 255 : red));

                resultBuffer[k] = (byte)blue;
                resultBuffer[k + 1] = (byte)green;
                resultBuffer[k + 2] = (byte)red;
                resultBuffer[k + 3] = 255;
            }

            Bitmap resultBitmap = new Bitmap(subtractFrom.Width,
                                             subtractFrom.Height);

            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        //이미지 컨볼루전
        private static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;
            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);
            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;
            int byteOffset = 0;

            for (int offsetY = 0; offsetY < sourceBitmap.Height; offsetY++)
            {
                for (int offsetX = 0; offsetX < sourceBitmap.Width; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;
                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    if ((filterOffset > offsetX || offsetX >= sourceBitmap.Width - filterOffset) ||
                         (filterOffset > offsetY || offsetY >= sourceBitmap.Height - filterOffset))
                    {
                        resultBuffer[byteOffset] = pixelBuffer[byteOffset];
                        resultBuffer[byteOffset + 1] = pixelBuffer[byteOffset + 1];
                        resultBuffer[byteOffset + 2] = pixelBuffer[byteOffset + 2];
                        resultBuffer[byteOffset + 3] = 255;
                    }
                    else
                    {
                        for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                            {

                                calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filterMatrix[filterY + filterOffset, filterX + filterOffset];

                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filterMatrix[filterY + filterOffset, filterX + filterOffset];

                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            }
                        }
                        blue = factor * blue + bias;
                        green = factor * green + bias;
                        red = factor * red + bias;

                        blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                        green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                        red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                        resultBuffer[byteOffset] = (byte)(blue);
                        resultBuffer[byteOffset + 1] = (byte)(green);
                        resultBuffer[byteOffset + 2] = (byte)(red);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                }
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }
        // mean 필터
        public static double[,] filter
        {
            get
            {
                return new double[,]  
                { 
                  { 1, 1, 1, }, 
                  { 1, 1, 1, }, 
                  { 1, 1, 1, }, 
                };
            }
        }

        //------------------------------  이미지 효과 끝  ------------------------------------//
    }
}
